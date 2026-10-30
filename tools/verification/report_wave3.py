"""Join durable receipts; report missing execution without manufacturing passes."""
import argparse
import collections
import json
import re
from pathlib import Path

from generate_inventory import apply_file

ROOT = Path(__file__).resolve().parents[2]
CASES = ROOT / 'tests/SqlSafetyGuard.ServerHarness/Campaign/cases'
DOCS = ROOT / 'docs/history/wave3'
WAVE2 = ROOT / 'docs/history/wave2'


def read_json(path):
    return json.loads(path.read_text(encoding='utf-8-sig'))


def load_cases(directory):
    rows = {}
    for path in sorted(directory.rglob('*.jsonl')):
        for line in path.read_text(encoding='utf-8-sig').split('\n'):
            if line.strip():
                row = json.loads(line)
                rows[row['id']] = row
    return rows


def cell(value):
    return excerpt(value).replace('|', '\\|').replace('\r', ' ').replace('\n', ' ')


def excerpt(value):
    text = str(value or '')
    return text if len(text) <= 2500 else text[:2500] + ' [excerpt; full text in durable receipt]'


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--artifacts', type=Path, default=ROOT.parent / 'harness-out/campaign')
    args = parser.parse_args()
    artifact_root = args.artifacts.resolve()
    runs = ['wave3-host-final', 'wave3-host-replay', 'wave3-isolated-final',
            'wave3-supplement', 'wave3-resolution-replay', 'wave3-final-replay']
    latest = {}
    for run in runs:
        for path in sorted((artifact_root / run / 'receipts').glob('*.json')):
            row = read_json(path)['Result']
            latest[row['Case']['Id']] = (run, row)
    focused = load_cases(CASES / 'wave3')
    independent = load_cases(CASES / 'independent')
    manifest = read_json(CASES / 'wave3/focused-manifest.json')
    rules = []
    rule_lines = ['# Wave-3 focused rule execution ledger', '',
                  'All 250 wave-2 unfocused rule IDs are accounted for. A successful tagged positive/boundary is **server-associated evidence**, not instrumentation proving an internal branch ran. Generic negative SELECTs never close a rule. API/presentation/AST guards retain explicit limitations. A SQL error is an attempted case, never a semantic pass.', '',
                  '| Rule | Server-associated positive/boundary | Remaining limitation / evidence |',
                  '| --- | --- | --- |']
    for entry in sorted(manifest, key=lambda item: item['ruleId']):
        identity = entry['ruleId']
        candidates = [row for row in focused.values() if identity in row.get('ruleIds', []) and row.get('family') != 'focused-negative']
        good = []
        failures = []
        for case in candidates:
            evidence = latest.get(case['id'])
            if evidence is None:
                failures.append(case['id'] + ': no receipt')
                continue
            run, result = evidence
            actual_options = result['Case'].get('LocalProbeOptions')
            expected_options = case.get('localProbeOptions')
            options_match = expected_options is None or (actual_options is not None and all(actual_options.get(key[0].upper() + key[1:]) == value for key, value in expected_options.items()))
            if result['Case']['Sql'] != case['sql'] or result['Case'].get('Setup', '') != case.get('setup', '') or not options_match:
                failures.append(case['id'] + ': stale SQL/setup/options receipt; replay required')
                continue
            if not result.get('Errors') and result['Outcome'] not in ['CRASH', 'TIMEOUT', 'NOT EXECUTABLE', 'FALSE GREEN', 'WRONG TARGET']:
                good.append(dict(caseId=case['id'], run=run, outcome=result['Outcome']))
            else:
                failures.append(case['id'] + ': ' + result['Outcome'] + '; ' + '; '.join(result.get('Errors', [])))
        note = entry['note'] if not candidates else '; '.join(failures) or 'Tagged executable contrast; review SQL and verdict in durable receipt.'
        rules.append(dict(ruleId=identity, executed=good, limitation=excerpt(note)))
        rule_lines.append('| `' + identity + '` | ' + (', '.join('`' + item['caseId'] + '` (' + item['outcome'] + ')' for item in good) or '**none**') + ' | ' + cell(note) + ' |')
    apply_file(DOCS / 'rule-coverage-wave3.md', '\n'.join(rule_lines) + '\n')
    feature_path = artifact_root / 'wave3-features-final/isolated-results.json'
    feature_results = read_json(feature_path).get('Results', []) if feature_path.exists() else []
    feature_latest = {row['Id']: row for row in feature_results}
    for extra in ['wave3-ag-attempt/isolated-results.json']:
        extra_path = artifact_root / extra
        if extra_path.exists():
            feature_latest.update({row['Id']: row for row in read_json(extra_path).get('Results', [])})
    feature_results = list(feature_latest.values())
    kinds = [kind for kind in re.findall(r'^\| (\w+) \|', (WAVE2 / 'gaps-wave2.md').read_text(), re.MULTILINE) if kind != 'Kind']
    kind_rows = []
    for kind in kinds:
        matching = [row for row in feature_results if kind in row.get('Fixture', {}).get('StatementTypes', [])]
        passes = [row for row in matching if row['Status'] == 'pass']
        explanation = '; '.join(row['Id'] + ': ' + row['Status'] + '; ' + str(row.get('Error') or '') for row in matching)
        if not matching:
            source = next((line for line in (WAVE2 / 'gaps-wave2.md').read_text().split('\n') if line.startswith('| ' + kind + ' |')), '')
            explanation = 'No successful executable fixture. ' + source.split('|')[-2].strip()
        kind_rows.append(dict(kind=kind, live=bool(passes), evidence=explanation))
    gap_lines = ['# Wave-3 execution limitations', '',
                 'This ledger deliberately distinguishes attempts, server-associated checks, semantic fixture passes, and unsupported/unprovisioned features. Nothing here certifies completeness or safety. Detailed SQL, error numbers, execution context and snapshots are retained in the external receipts.', '',
                 '## Unclosed implementation work', '',
                 'The requested exhaustive semantic closure is not achieved. Fifty API/client/AST rules have no generated SQL triple; other tagged rules still have failed positive/boundary fixtures. Bare undocumented DBCC names or `(0)` arguments are inadequate recipes, not proof of platform impossibility. Recovery/log-rebuild, real EKM packages, external-language packages, and restart-aware observers remain unprovisioned. The exact errors below are retained instead of counting these as passes. The 16 AST-only/legacy statement nodes have no standalone production in the pinned parser; their historical reasons are preserved in the statement table. Availability-group SQL is attempted directly because the pinned parser rejects CLUSTER_TYPE=NONE; an executed-unverified attempt does not close semantic coverage.', '',
                 '## Original statement-kind gaps', '',
                 '| Kind | Live semantic pass | Exact attempt / reason remaining |', '| --- | --- | --- |']
    for entry in kind_rows:
        gap_lines.append('| ' + entry['kind'] + ' | ' + ('yes' if entry['live'] else '**no**') + ' | ' + cell(entry['evidence']) + ' |')
    gap_lines += ['', '## Rule limitations', '',
                  'See `rule-coverage-wave3.md` for every one of the 250 requested rule IDs, exact failed positive/boundary IDs, and the 50 non-generated API/client/AST limitations. An executed contrast is not internal branch instrumentation.', '',
                  '## Latest non-executable or unresolved campaign evidence', '',
                  '| Case | Outcome | Error / stage | SQL template |', '| --- | --- | --- | --- |']
    selected = {}
    for identity, case in {**focused, **independent}.items():
        if identity not in latest:
            continue
        run, result = latest[identity]
        selected[identity] = (run, result)
        if result['Outcome'] in ['NOT EXECUTABLE', 'TIMEOUT', 'CRASH', 'WRONG TARGET', 'FALSE GREEN', 'OBSERVATION GAP', 'UNRESOLVED TARGET']:
            gap_lines.append('| `' + identity + '` | ' + result['Outcome'] + ' | ' + cell('; '.join(result.get('Errors', [])) or result.get('Stage')) + ' | `' + cell(case['sql']).replace('`', '') + '` |')
    apply_file(DOCS / 'gaps-wave3.md', '\n'.join(gap_lines) + '\n')
    counts = {}
    for label, corpus in [('focused', focused), ('independent', independent)]:
        counts[label] = dict(collections.Counter(result['Outcome'] for identity, (_, result) in selected.items() if identity in corpus))
    offline_path = artifact_root / 'wave3-independent-offline/offline-results.jsonl'
    offline = [json.loads(line) for line in offline_path.read_text(encoding='utf-8-sig').split('\n') if line.strip()] if offline_path.exists() else []
    offline_summary = dict(completed=len(offline), mismatches=sum(bool(row.get('Mismatches')) for row in offline),
                           analyzerErrors=sum(bool(row.get('Default', {}).get('Error') or row.get('Configured', {}).get('Error')) for row in offline))
    metrics = dict(artifactRoot=str(artifact_root), receiptRuns=runs, rulesRequested=len(rules),
                   rulesWithServerAssociatedPositiveOrBoundary=sum(bool(row['executed']) for row in rules),
                   originalStatementGaps=len(kind_rows), statementKindsLive=sum(row['live'] for row in kind_rows),
                   counts=counts, independentOffline=offline_summary, rules=rules, statementKinds=kind_rows,
                   cases=[dict(caseId=identity, run=run, outcome=row['Outcome'], errors=[excerpt(error) for error in row.get('Errors', [])], isolationVerified=row.get('IsolationVerified')) for identity, (run, row) in sorted(selected.items())])
    apply_file(DOCS / 'validation-wave3.json', json.dumps(metrics, indent=2) + '\n')
    print(json.dumps({key: value for key, value in metrics.items() if key not in ['rules', 'statementKinds', 'cases']}, indent=2))


if __name__ == '__main__':
    main()
