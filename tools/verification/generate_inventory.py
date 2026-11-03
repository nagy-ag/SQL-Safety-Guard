"""Regenerate the offline decision register. No SQL execution or production instrumentation."""
import argparse
import collections
import hashlib
import json
import re
import tempfile
import os
import subprocess
import xml.etree.ElementTree as ET
from pathlib import Path
from artifact_io import write_generated

ROOT = Path(__file__).resolve().parents[2]
DEST = ROOT / "docs/verification/rules"
ALL = ["ReadOnly", "TempOnly", "AllowListed", "Unknown", "RealWrite"]
CATEGORIES = {
    "Classifier": "classification", "StatementWalker": "walking",
    "TargetResolver": "resolution", "Scope": "scope", "SqlCmdPreprocessor": "sqlcmd",
    "AllowList": "allow-list", "EnvironmentRules": "environment", "GuardSettings": "environment",
    "GuardDecision": "environment", "AnalysisWork": "limits",
    "SideEffectScanner": "hidden-effects", "BatchRecovery": "recovery",
    "SqlSafetyAnalyzer": "entry", "AnalysisResult": "aggregation",
    "AnalyzerOptions": "options", "ParserFactory": "parser", "Verdict": "aggregation",
    "Finding": "presentation", "TextPosition": "positions",
}


def test_declarations():
    # Reuse the harness's existing Roslyn dependency in an explicitly offline mode.
    with tempfile.TemporaryDirectory(prefix="SgRun_test_index_") as output:
        subprocess.run(["dotnet", "run", "--no-restore", "--project", str(ROOT / "tests/SqlSafetyGuard.ServerHarness"),
                        "--verbosity", "quiet", "--", "--test-index",
                        str(ROOT / "tests/SqlSafetyGuard.Tests"), "--out", output],
                       check=True, stdout=subprocess.DEVNULL)
        return json.loads((Path(output) / "test-index.json").read_text(encoding="utf-8"))


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    rules = []
    source_paths = sorted(path for path in (ROOT / "src/SqlSafetyGuard.Analyzer").rglob("*.cs")
                          if not {"bin", "obj"}.intersection(path.relative_to(ROOT).parts))
    if len({path.stem for path in source_paths}) != len(source_paths):
        raise ValueError("Duplicate analyzer file stems require explicit semantic source identities.")
    texts = {path.stem: path.read_text(encoding="utf-8-sig") for path in source_paths}
    sources = {path.stem: path.relative_to(ROOT).as_posix() for path in source_paths}
    def add(identifier, category, condition, verdicts, stem, offset, **extra):
        rules.append(dict(id=identifier, category=category, condition=condition,
                          verdicts=verdicts, source=sources[stem] + ":" +
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
    project = ET.parse(ROOT / "src/SqlSafetyGuard.Analyzer/SqlSafetyGuard.Analyzer.csproj")
    version = next(item.get("Version") for item in project.iter("PackageReference")
                   if item.get("Include") == "Microsoft.SqlServer.TransactSql.ScriptDom")
    packages = Path(os.environ.get("NUGET_PACKAGES", Path.home() / ".nuget/packages"))
    package = packages / f"microsoft.sqlserver.transactsql.scriptdom/{version}/lib/netstandard2.0/Microsoft.SqlServer.TransactSql.ScriptDom.xml"
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
    declarations = test_declarations()
    fixture_types = {fixture["statementType"] for fixture in json.loads(
        (ROOT / "tests/SqlSafetyGuard.ServerHarness/Coverage/parser-fixtures.json").read_text(encoding="utf-8"))["fixtures"]}
    for rule in rules:
        if rule["category"] in ("green-statement", "action-name", "unknown-statement") and rule.get("member") in fixture_types:
            rule.setdefault("testMethodNames", []).append("CommittedStatementVerdictsMatchCurrentAnalyzer")
        for name in rule.pop("testMethodNames", []):
            matches = [entry for entry in declarations if entry["Name"].rsplit(".", 1)[-1] == name or entry["Name"] == name]
            if not matches:
                raise ValueError("No attributed test declaration for " + name)
            rule["existingTests"].extend(entry["Name"] + " @ tests/SqlSafetyGuard.Tests/" +
                                         entry["File"] + ":" + str(entry["Line"]) for entry in matches)
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
               DEST / "rule-test-map.json": json.dumps(mapping, indent=2) + "\n",
               DEST / "uncovered.md": "\n".join([
                   "# Unmapped rule associations", "", "Generated by `tools/verification/generate_inventory.py`.", "",
                   f"{len(mapping)}/{len(rules)} rules have named test associations. These are declarations, not measured coverage.", "",
                   "Missing associations are report-only. Mapped rules may still have untested branches or runtime effects.", "",
                   "The original measured report is preserved in [history](../../history/2026-10-28-offline-coverage/README.md).", "",
                   "## Rules without associations", "", *["- `" + rule["id"] + "`" for rule in rules if rule["id"] not in mapping], ""]) }
    for path, content in outputs.items():
        if args.check:
            if not path.exists() or path.read_text(encoding="utf-8") != content:
                raise SystemExit(f"Stale generated file: {path}")
        else:
            write_generated(path, content)
    print(json.dumps(counts), "total", len(rules))


if __name__ == "__main__":
    main()
