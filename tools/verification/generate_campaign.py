"""Deterministic campaign inputs; offline rule probes never silently become live cases."""
import argparse
import hashlib
import itertools
import json
import random
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "tests/SqlSafetyGuard.ServerHarness/Campaign/cases/generated"


def case(identity, sql, family, seed, rules=(), **options):
    return dict(id=identity, sql=sql, family=family, seed=seed, ruleIds=list(rules), **options)


def quote(sql):
    return sql.replace("'", "''")


def wrap(sql, wrapper):
    if wrapper == "plain": return sql
    if wrapper == "block": return f"BEGIN\n{sql}\nEND;"
    if wrapper == "tran": return f"BEGIN TRAN;\n{sql}\nCOMMIT;"
    if wrapper == "try": return f"BEGIN TRY\n{sql}\nEND TRY BEGIN CATCH THROW; END CATCH;"
    if wrapper == "if": return f"IF 1=1 BEGIN\n{sql}\nEND ELSE SELECT 0;"
    if wrapper == "while": return f"DECLARE @iteration int=0; WHILE @iteration<1 BEGIN\n{sql}\nSET @iteration+=1; END;"
    if wrapper == "cursor": return f"DECLARE work_cursor CURSOR LOCAL FAST_FORWARD FOR SELECT 1; DECLARE @key int; OPEN work_cursor; FETCH NEXT FROM work_cursor INTO @key; WHILE @@FETCH_STATUS=0 BEGIN\n{sql}\nFETCH NEXT FROM work_cursor INTO @key; END; CLOSE work_cursor; DEALLOCATE work_cursor;"
    if wrapper == "exec": return f"EXEC(N'{quote(sql)}');"
    if wrapper == "parameterized": return f"EXEC sys.sp_executesql N'{quote(sql)}';"
    if wrapper == "nested": return wrap(wrap(sql, "exec"), "exec")
    if wrapper == "go": return f"SELECT 1;\nGO\n{sql}\nGO\nSELECT 2;"
    raise ValueError(wrapper)


TARGETS = ["dbo.Counter", "app.Counter", "Counter", "[dbo].[Counter]", "{{Sales}}.dbo.Counter", "sales.OpenOrders", "app.OrderEntry", "app.OrderAlias", "app.RemoteCounter", "app.Accounts", "staging.Events", "sales.Orders"]
WRAPPERS = ["plain", "block", "tran", "try", "if", "while", "cursor", "exec", "parameterized", "nested", "go"]
SHAPES = ["read", "update", "alias", "cte", "derived", "merge"]
PRINCIPALS = ["runner", "app_user", "low_user"]
FILTERS = ["Id=1", "Id=-1", "Id IN(SELECT Id FROM dbo.Counter WHERE Id=2)"]


def body(target, shape, predicate, amount):
    column = "Balance" if target == "app.Accounts" else "Payload" if target == "staging.Events" else "Qty" if target in TARGETS[5:8] + ["sales.Orders"] else "Value"
    value = f"CONCAT({column},N'x')" if column == "Payload" else f"{column}+{amount}"
    if shape == "read": return f"SELECT * FROM {target} WHERE {predicate};"
    if shape == "update": return f"UPDATE {target} SET {column}={value} WHERE {predicate};"
    if shape == "alias": return f"UPDATE target SET {column}={value} FROM {target} target WHERE {predicate};"
    if shape == "cte": return f"WITH target AS(SELECT * FROM {target} WHERE {predicate}) UPDATE target SET {column}={value};"
    if shape == "derived": return f"UPDATE target SET {column}={value} FROM (SELECT * FROM {target} WHERE {predicate}) target;"
    if shape == "merge": return f"MERGE {target} AS target USING(SELECT 1 Id) source ON target.Id=source.Id WHEN MATCHED AND target.{predicate} THEN UPDATE SET {column}={value};"
    raise ValueError(shape)


def interactions(row, strength):
    return {(columns, tuple(row[column] for column in columns)) for columns in itertools.combinations(range(len(row)), strength)}


def covering_rows(levels, strength, rng):
    candidates = list(itertools.product(*(range(level) for level in levels)))
    rng.shuffle(candidates)
    uncovered = set().union(*(interactions(row, strength) for row in candidates))
    selected = []
    while uncovered:
        pool = rng.sample(candidates, min(400, len(candidates)))
        best = max(pool, key=lambda row: len(interactions(row, strength) & uncovered))
        hits = interactions(best, strength) & uncovered
        if not hits:
            best = next(row for row in candidates if interactions(row, strength) & uncovered)
            hits = interactions(best, strength) & uncovered
        selected.append(best)
        uncovered.difference_update(hits)
    return selected


def combinatorial(seed, random_count):
    rng = random.Random(seed)
    dimensions = [TARGETS, SHAPES, WRAPPERS, FILTERS, PRINCIPALS]
    rows = []
    for strength in (2, 3):
        selected = covering_rows([len(values) for values in dimensions], strength, rng)
        for index, row in enumerate(selected):
            target, shape, wrapper, predicate, principal = [values[value] for values, value in zip(dimensions, row)]
            sql = wrap(body(target, shape, predicate, 1), wrapper)
            rows.append(case(f"cover{strength}-{seed}-{index:05d}", sql, f"covering-{strength}", seed,
                             ["write.allow-list", "scope.dynamic"] if wrapper in ("exec", "parameterized", "nested") else ["write.allow-list"], principal=principal))
    for index in range(random_count):
        target, wrapper, predicate, principal = rng.choice(TARGETS), rng.choice(WRAPPERS), rng.choice(FILTERS), rng.choice(PRINCIPALS)
        shape = "read" if rng.random() < .85 else rng.choice(SHAPES[1:])
        sql = wrap(body(target, shape, predicate, rng.randint(1, 5000)), wrapper)
        rows.append(case(f"random-{seed}-{index:05d}", f"-- deterministic sample {seed}:{index}\n" + sql, "seeded-random", seed, ["write.allow-list"], principal=principal))
    return rows


def company(seed):
    rng = random.Random(seed)
    rows = []
    for size in (500, 1500, 5001):
        for variant in range(4):
            lines = ["CREATE TABLE #stage(Id int PRIMARY KEY, Value int);", "INSERT #stage VALUES(1,0),(2,0);", "BEGIN TRY", "BEGIN TRAN;"]
            for index in range(size - 18):
                if index % 89 == 0:
                    lines.append(f"SELECT * INTO #work{index} FROM #stage;")
                elif index % 11 == 0:
                    lines.append("WITH first_step AS(SELECT * FROM #stage), second_step AS(SELECT * FROM first_step), third_step AS(SELECT * FROM second_step) SELECT * FROM (SELECT * FROM (SELECT * FROM third_step) inner_rows) outer_rows;")
                elif index % 37 == 0:
                    lines.append("SELECT stage.Id FROM #stage stage JOIN dbo.Counter reference ON reference.Id=stage.Id WHERE " + " AND ".join(f"stage.Value<={5000+offset}" for offset in range(250)) + ";")
                else:
                    lines.append(f"UPDATE #stage SET Value=Value+{rng.randint(1,3)} WHERE Id={1+index%2};")
            lines.extend(["EXEC(N'SELECT COUNT(*) FROM #stage;');", "DECLARE work_cursor CURSOR LOCAL FAST_FORWARD FOR SELECT Id FROM #stage;", "DECLARE @key int; OPEN work_cursor; FETCH NEXT FROM work_cursor INTO @key;", "WHILE @@FETCH_STATUS=0 BEGIN SELECT @key; FETCH NEXT FROM work_cursor INTO @key; END;", "CLOSE work_cursor; DEALLOCATE work_cursor;", "COMMIT;", "END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK; THROW; END CATCH;"])
            while len(lines) < size: lines.insert(-1, "SELECT COUNT(*) FROM #stage;")
            position = rng.randrange(5, len(lines)-8)
            for permanent in (False, True):
                script = list(lines)
                if permanent: script[position] = "UPDATE dbo.Counter SET Value=Value+1 WHERE Id=1;"
                rows.append(case(f"company-{seed}-{size}-{variant}-{'write' if permanent else 'green'}", "\n".join(script), "company", seed,
                                 ["temp.name", "scope.dynamic", "write.allow-list"], groundTruthPermanentWrite=permanent, buriedLine=position+1 if permanent else None, lineCount=len(script)))
    return rows


def rule_probes(seed):
    rules = json.loads((ROOT / "docs/verification/rules.json").read_text(encoding="utf-8"))["rules"]
    fixtures = []
    for path in sorted((ROOT / "tests/SqlSafetyGuard.ServerHarness/Coverage").glob("*fixtures.json")):
        fixtures.extend(json.loads(path.read_text(encoding="utf-8-sig")).get("fixtures", []))
    offline, live, manifest = [], [], []
    for rule in rules:
        member = rule.get("member", "")
        candidate = next((fixture for fixture in fixtures if member and (member == fixture.get("statementType") or member in fixture.get("statementTypes", []))), None)
        category = rule["category"]
        if category in ("safe-procedure", "writing-procedure", "msdb-procedure") and member:
            sql = f"EXEC sys.{member};"
        elif category in ("safe-dbcc", "changing-dbcc") and member:
            sql = f"DBCC {member};"
        elif candidate:
            sql = candidate["sql"]
        elif rule["id"] == "hidden.sequence":
            sql = "SELECT NEXT VALUE FOR app.OrderSequence;"
        elif rule["id"] in ("temp.name", "write.allow-list"):
            sql = "UPDATE dbo.Counter SET Value=Value+1 WHERE Id=1;"
        else:
            manifest.append(dict(ruleId=rule["id"], status="needs-focused-options-or-AST-probe", reason=rule["condition"], tests=rule["existingTests"]))
            continue
        probes = [sql, "SELECT 1;", wrap(sql, "block")]
        for label, probe in zip(("positive", "negative-control", "boundary-wrapper"), probes):
            offline.append(case(f"rule-{rule['id']}-{label}", probe, "rule-offline", seed, [rule["id"]], setup=candidate.get("setupSql", "") if candidate else ""))
        manifest.append(dict(ruleId=rule["id"], status="offline-candidate", reason="Arguments, prerequisites and physical assertions require review before live execution; negative control is not proof of branch coverage."))
    attacks = [
        ("temp.name", "CREATE TABLE #work(Id int); INSERT #work VALUES(1);", "TempOnly"),
        ("write.allow-list", "UPDATE dbo.Counter SET Value=Value+1 WHERE Id=1;", "RealWrite"),
        ("hidden.sequence", "SELECT NEXT VALUE FOR app.OrderSequence;", "RealWrite"),
        ("resolve.alias", "UPDATE target SET Value=Value+1 FROM dbo.Counter target WHERE Id=1;", "RealWrite"),
    ]
    ids = {rule["id"] for rule in rules}
    for attack_index, (rule, sql, expected) in enumerate(attacks):
        if rule not in ids: rule = "write.allow-list"
        for wrapper in WRAPPERS:
            live.append(case(f"rule-live-{attack_index}-{rule}-{wrapper}", wrap(sql, wrapper), "rule-live", seed, [rule], expectedClassification=expected))
    for index, sql in enumerate([
        "SELECT * FROM app.LocalWork(1);", "SELECT app.OrderTotal(1);", "SELECT * FROM app.OrderRows();",
        "UPDATE app.Accounts SET Balance=Balance+1 WHERE Id=1;", "TRUNCATE TABLE staging.Events;",
        "UPDATE app.RemoteCounter SET Value=Value+1 WHERE Id=1;", "UPDATE Counter SET Value=Value+1 WHERE Id=1;",
        "DECLARE @dialog uniqueidentifier; BEGIN DIALOG CONVERSATION @dialog FROM SERVICE [SgWork] TO SERVICE N'SgWork' ON CONTRACT [DEFAULT] WITH ENCRYPTION=OFF; SEND ON CONVERSATION @dialog MESSAGE TYPE [DEFAULT](0x01);"
    ]): live.append(case(f"world-{seed}-{index}", sql, "world-features", seed, ["write.allow-list"]))
    return offline, live, manifest


def write_jsonl(path, rows):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text("".join(json.dumps(row, ensure_ascii=True, separators=(",", ":")) + "\n" for row in rows), encoding="utf-8")
    return dict(path=str(path.relative_to(OUTPUT)), cases=len(rows), sha256=hashlib.sha256(path.read_bytes()).hexdigest())


def main():
    parser = argparse.ArgumentParser(__doc__)
    parser.add_argument("--seed", type=int, default=20261028)
    parser.add_argument("--random-count", type=int, default=30000)
    args = parser.parse_args()
    offline, live, rules = rule_probes(args.seed)
    combinations = combinatorial(args.seed, args.random_count)
    companies = company(args.seed)
    files = [write_jsonl(OUTPUT / "host/rules.jsonl", live), write_jsonl(OUTPUT / "host/combinatorial.jsonl", combinations), write_jsonl(OUTPUT / "host/company.jsonl", companies), write_jsonl(OUTPUT / "offline/rules.jsonl", offline)]
    rng = random.Random(args.seed)
    sample = list(live) + list(companies)
    for family in ("covering-2", "covering-3", "seeded-random"):
        candidates = [row for row in combinations if row["family"] == family]
        sample.extend(rng.sample(candidates, min(80, len(candidates))))
    files.append(write_jsonl(OUTPUT / "validation.jsonl", sample))
    (OUTPUT / "manifest.json").write_text(json.dumps(dict(seed=args.seed, files=files, rules=rules, limitations=["Offline candidates are not live coverage.", "SQLCMD client execution is deliberately unsupported by the campaign runner.", "No allow-list/trusted-procedure option injection is implemented in campaign cases."]), indent=2) + "\n", encoding="utf-8")
    print(json.dumps(files, indent=2))


if __name__ == "__main__": main()
