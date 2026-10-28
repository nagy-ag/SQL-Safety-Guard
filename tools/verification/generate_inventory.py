"""Regenerate the offline decision register. No SQL execution or production instrumentation."""
import argparse
import collections
import hashlib
import json
import re
import shutil
import subprocess
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DEST = ROOT / "docs/verification"
ALL = ["ReadOnly", "TempOnly", "AllowListed", "Unknown", "RealWrite"]
CATEGORIES = {
    "Classifier": "classification", "StatementWalker": "walking",
    "TargetResolver": "resolution", "Scope": "scope", "SqlCmdPreprocessor": "sqlcmd",
    "AllowList": "allow-list", "EnvironmentRules": "environment", "AnalysisWork": "limits",
    "SideEffectScanner": "hidden-effects", "BatchRecovery": "recovery",
    "SqlSafetyAnalyzer": "entry", "AnalysisResult": "aggregation",
    "AnalyzerOptions": "options", "ParserFactory": "parser", "Verdict": "aggregation",
    "Finding": "presentation", "TextPosition": "positions",
}


def apply_file(path, content):
    relative = path.relative_to(ROOT).as_posix()
    launcher = shutil.which("apply_patch")
    if launcher is None:
        raise RuntimeError("apply_patch must be on PATH to regenerate files")
    command = [launcher]
    if Path(launcher).suffix.lower() == ".bat":
        wrapper = Path(launcher).read_text(encoding="utf-8")
        match = re.search(r'"%~dp0([^"\r\n]+)"\s+--codex-run-as-apply-patch', wrapper)
        if match is None:
            raise RuntimeError("Unrecognized apply_patch Windows wrapper")
        command = [str((Path(launcher).parent / match.group(1)).resolve()),
                   "--codex-run-as-apply-patch"]
    def apply(patch):
        subprocess.run(command + [patch], check=True,
                       stdout=subprocess.DEVNULL)
    if path.exists():
        if path.read_text(encoding="utf-8") == content:
            return
        apply(f"*** Begin Patch\n*** Delete File: {relative}\n*** End Patch\n")
    lines = content.splitlines()
    previous = None
    while lines:
        chunk = []
        size = 0
        while lines and size + len(lines[0]) < 9000:
            value = lines.pop(0)
            chunk.append(value)
            size += len(value) + 2
        if not chunk:
            raise ValueError("Single generated line exceeds patch limit")
        if previous is None:
            header = f"*** Add File: {relative}\n"
            suffix = ""
        else:
            header = f"*** Update File: {relative}\n@@\n" + "\n".join(" " + value for value in previous) + "\n"
            suffix = "*** End of File\n"
        apply("*** Begin Patch\n" + header + "\n".join("+" + line for line in chunk) +
              "\n" + suffix + "*** End Patch\n")
        previous = chunk[-40:]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    rules = []
    texts = {path.stem: path.read_text(encoding="utf-8-sig")
             for path in sorted((ROOT / "src/SqlSafetyGuard.Analyzer").glob("*.cs"))}
    def add(identifier, category, condition, verdicts, stem, offset, **extra):
        rules.append(dict(id=identifier, category=category, condition=condition,
                          verdicts=verdicts, source=f"src/SqlSafetyGuard.Analyzer/{stem}.cs:" +
                          str(texts[stem].count("\n", 0, offset) + 1), existingTests=[], **extra))
    classifier = texts["Classifier"]
    tables = [("ReadOnlyProcedures", "safe-procedure", "ReadOnly"),
              ("WritingProcedures", "writing-procedure", "RealWrite"),
              ("ReadOnlyStatementTypes", "green-statement", "ReadOnly"),
              ("ReadOnlyDbcc", "safe-dbcc", "ReadOnly"),
              ("ChangingDbcc", "changing-dbcc", "RealWrite"),
              ("UnknownStatementTypes", "unknown-statement", "Unknown"),
              ("ActionNames", "action-name", None)]
    for table, category, verdict in tables:
        start = classifier.index("\n        {", classifier.index(" " + table + " ="))
        end = classifier.index("\n        };", start)
        pattern = r'DbccCommand\.(\w+)' if "Dbcc" in table else r'"([^"\n]+)"'
        body = classifier[start:end]
        for match in re.finditer(pattern, body):
            if table in ("WritingProcedures", "UnknownStatementTypes", "ActionNames"):
                if not body[max(0, match.start()-1):match.start()] == "[":
                    continue
            name = match.group(1)
            description = f"{table} contains {name}; "
            description += (f"classify as {verdict} after specialized dispatch/qualification overrides."
                            if verdict else "choose this action label; verdict unchanged.")
            add(category + "." + name.lower(), category, description,
                [verdict] if verdict else [], "Classifier", start + match.start(),
                member=name, reviewStatus="reviewed-table", table=table)
    walker = texts["StatementWalker"]
    keyword_start = walker.index(" WriteKeywords =")
    keyword_end = walker.index("        };", keyword_start)
    for match in re.finditer(r"TSqlTokenType\.(\w+)", walker[keyword_start:keyword_end]):
        name = match.group(1)
        add("recovery-keyword." + name.lower(), "recovery-keyword",
            f"Unparsed non-trivia {name} token adds a possible-write Unknown finding; this is not proof the statement executes.",
            ["Unknown"], "StatementWalker", keyword_start + match.start(), member=name,
            reviewStatus="reviewed-table", testMethodNames=["KeywordSafetyNet_FindsWritesInUnparsedText_ButNotInCommentsOrStrings"])
    namespace_start = walker.index('var procedure = name.BaseIdentifier')
    namespace_end = walker.index('private static bool UpdatesUsage', namespace_start)
    for match in re.finditer(r'"(sp_\w+)"', walker[namespace_start:namespace_end]):
        name = match.group(1)
        add("msdb-procedure." + name, "msdb-procedure",
            f"Explicit msdb.sys/msdb.dbo {name} qualifies for system-procedure classification; remote and other schemas remain ineligible.",
            ["ReadOnly", "RealWrite"], "StatementWalker", namespace_start + match.start(), member=name,
            reviewStatus="reviewed-table", testMethodNames=["SystemNamesOutsideSystemNamespaceAreUnknown", "KnownWriters_AreRed"])
    sqlcmd = texts["SqlCmdPreprocessor"]
    start = sqlcmd.index("setvar|r|on|")
    end = sqlcmd.index(")", start)
    for name in sqlcmd[start:end].split("|") + ["!!", "$()"]:
        command = name if name in ("!!", "$()") else ":" + name
        outcomes = ["Unknown"] if command in (":connect", ":r", "!!", "$()") else ["ReadOnly", "Unknown"] if command == ":exit" else ["ReadOnly"]
        add("sqlcmd-command." + {"!!": "os", "$()": "variable"}.get(name, name), "sqlcmd-command",
            f"SQLCMD mode recognizes {command}; connect invalidates context, external execution/substitution warns, exit arguments warn, remaining directives are green. Mode-off syntax warns separately.",
            outcomes, "SqlCmdPreprocessor", start if name not in ("!!", "$()") else sqlcmd.index('Variable =') if name == "$()" else sqlcmd.index('!!'),
            member=command, reviewStatus="reviewed-table", testMethodNames=["Directives_AreReported", "UnexpandedSqlCmdExecutionIsNeverGreen"])
    package = Path.home() / ".nuget/packages/microsoft.sqlserver.transactsql.scriptdom/180.140.0/lib/netstandard2.0/Microsoft.SqlServer.TransactSql.ScriptDom.xml"
    xml = package.read_text(encoding="utf-8-sig")
    for match in re.finditer(r'<member name="F:Microsoft.SqlServer.TransactSql.ScriptDom.SetOptions\.(\w+)">\s*<summary>(.*?)</summary>', xml, re.S):
        name, sql = match.groups()
        if name == "None":
            continue
        sql = " ".join(sql.split())
        add("set-option." + name.lower(), "set-option", f"SET {sql} ON/OFF uses the green SetOnOffStatement base classification; implicit transactions also change tracking.",
            ["ReadOnly", "Unknown"], "Classifier", classifier.index('"SetOnOffStatement"'),
            member=name, sql=sql, reviewStatus="reviewed-table")
    assembly = package.with_suffix(".dll")
    reflection = ("$asm=[Reflection.Assembly]::LoadFrom('" + str(assembly).replace("'", "''") + "'); "
                  "$result=@{}; foreach($name in @('DbccCommand','GeneralSetCommandType','SetOffsets','SetStatisticsOptions')) { "
                  "$result[$name]=[Enum]::GetNames($asm.GetType('Microsoft.SqlServer.TransactSql.ScriptDom.'+$name)) }; "
                  "$result | ConvertTo-Json -Compress")
    members = json.loads(subprocess.check_output(["powershell", "-NoProfile", "-Command", reflection], text=True))
    classified_dbcc = {rule.get("member") for rule in rules if rule["category"] in ("safe-dbcc", "changing-dbcc")}
    for name in members["DbccCommand"]:
        if name in classified_dbcc or name in ("None", "CheckIdent"):
            continue
        add("unknown-dbcc." + name.lower(), "unknown-dbcc", f"DBCC {name} is absent from both command tables and uses Unknown unless an argument override applies.",
            ["Unknown", "RealWrite"], "Classifier", classifier.index('note = "DBCC command whose effect'),
            member=name, reviewStatus="reviewed-table")
    families = {
        "GeneralSetCommandType": {"Language": "LANGUAGE us_english", "DateFormat": "DATEFORMAT ymd", "DateFirst": "DATEFIRST 1",
            "DeadlockPriority": "DEADLOCK_PRIORITY LOW", "LockTimeout": "LOCK_TIMEOUT 1000",
            "QueryGovernorCostLimit": "QUERY_GOVERNOR_COST_LIMIT 0", "ContextInfo": "CONTEXT_INFO 0x01"},
        "SetStatisticsOptions": {"IO": "STATISTICS IO ON", "Profile": "STATISTICS PROFILE ON", "Time": "STATISTICS TIME ON", "Xml": "STATISTICS XML ON"},
        "SetOffsets": {name: "OFFSETS " + {"Order": "ORDER", "Param": "PARAM"}.get(name, name.upper()) + " ON" for name in members["SetOffsets"] if name != "None"}}
    for family, options in families.items():
        for name in members[family]:
            if name == "None":
                continue
            sql = options[name]
            add("set-family." + family.lower() + "." + name.lower(), "set-family",
                f"SET {sql} inherits green session SET classification; no exemption from strict session-residue contract is implied.",
                ["ReadOnly"], "Classifier", classifier.index('"SetCommandStatement"' if family == "GeneralSetCommandType" else '"SetOnOffStatement"'),
                member=name, sql=sql, enumType=family, reviewStatus="reviewed-table")
    for row in (DEST / "semantic-rules.tsv").read_text(encoding="utf-8").splitlines():
        if not row or row.startswith("#"):
            continue
        identifier, stem, anchor, verdicts, condition, tests = row.split("\t")
        offset = texts[stem].index(anchor)
        add(identifier, CATEGORIES[stem], condition, verdicts.split(",") if verdicts else [],
            stem, offset, reviewStatus="reviewed-semantic",
            testMethodNames=tests.split(",") if tests not in ("", "-") else [])
    mapping = {}
    table_test = "SqlSafetyGuard.Tests.RuleInventoryTests.TableDecisionMatchesInventory"
    test_texts = [(path, path.read_text(encoding="utf-8-sig")) for path in
                  sorted((ROOT / "tests/SqlSafetyGuard.Tests").glob("*.cs"))]
    fixture_types = {fixture["statementType"] for fixture in json.loads(
        (ROOT / "tests/SqlSafetyGuard.ServerHarness/Coverage/parser-fixtures.json").read_text(encoding="utf-8"))["fixtures"]}
    for rule in rules:
        if rule["category"] in ("green-statement", "action-name", "unknown-statement") and rule.get("member") in fixture_types:
            rule.setdefault("testMethodNames", []).append("CommittedStatementVerdictsMatchCurrentAnalyzer")
        for name in rule.pop("testMethodNames", []):
            for path, text in test_texts:
                match = re.search(r'\b' + re.escape(name) + r'\s*\(', text)
                if match:
                    rule["existingTests"].append(f"{path.relative_to(ROOT).as_posix()}:{text.count(chr(10), 0, match.start())+1} ({name})")
        if rule["existingTests"]:
            mapping[rule["id"]] = rule["existingTests"]
        if rule["category"] in ("safe-procedure", "writing-procedure", "safe-dbcc", "changing-dbcc", "unknown-dbcc", "set-option", "set-family"):
            mapping[rule["id"]] = [table_test]
            rule["existingTests"] = [table_test]
    rules.sort(key=lambda rule: (rule["category"], rule["id"]))
    counts = dict(sorted(collections.Counter(rule["category"] for rule in rules).items()))
    payload = dict(schemaVersion=2, baseline="3fff9b0", generator="tools/verification/generate_inventory.py",
                   limitations=["Reviewed semantic rules and explicit dispatch tables, not a completeness certificate.",
                                "existingTests records named-method associations, not observed branch coverage. Empty means uncovered.",
                                "Retired overlapping source checkpoints are mapped in rule-id-migrations.json; they are not semantic rules."],
                   categories=counts, rules=rules)
    markdown = ["# Analyzer rule inventory", "", "Generated from rules.json; regenerate with `python tools/verification/generate_inventory.py`.", "",
                "Every entry is a reviewed semantic rule or dispatch-table member. Retired line-level checkpoints are recorded in `rule-id-migrations.json`. Named tests are associations, not server or branch coverage.", "",
                "| Category | Entries |", "| --- | ---: |"]
    markdown += [f"| {category} | {count} |" for category, count in counts.items()]
    for rule in rules:
        markdown += ["", f"## {rule['id']}", "", rule["condition"], "",
                     f"- Outcomes: {', '.join(rule['verdicts']) or 'annotation only'}; review: {rule['reviewStatus']}.",
                     f"- Source: `{rule['source']}`.",
                     "- Existing tests: " + ("; ".join(rule["existingTests"]) or "No reviewed association.")]
        if "sourceExpression" in rule:
            markdown += ["- Decision: `" + rule["sourceExpression"].replace("`", "'") + "`."]
    outputs = {DEST / "rules.json": json.dumps(payload, indent=2) + "\n",
               DEST / "rules.md": "\n".join(markdown) + "\n",
               DEST / "rule-test-map.json": json.dumps(mapping, indent=2) + "\n"}
    for path, content in outputs.items():
        if args.check:
            if not path.exists() or path.read_text(encoding="utf-8") != content:
                raise SystemExit(f"Stale generated file: {path}")
        else:
            apply_file(path, content)
    print(json.dumps(counts), "total", len(rules))


if __name__ == "__main__":
    main()
