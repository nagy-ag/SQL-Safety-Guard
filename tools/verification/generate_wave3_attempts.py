import argparse
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--output', required=True)
    args = parser.parse_args()
    missing = re.findall(r'\| (\w+) \| open;', (ROOT / 'docs/history/wave2/gaps-wave2.md').read_text())
    examples = {row['statementType']: row for row in json.loads((ROOT / 'tests/SqlSafetyGuard.ServerHarness/Coverage/parser-fixtures.json').read_text())['fixtures']}
    fixtures = []
    unavailable = []
    for kind in missing:
        if kind not in examples:
            unavailable.append(dict(statementType=kind, reason='No standalone TSql180 parser production/example; AST-only or legacy node. No SELECT surrogate is counted as an attempt.'))
            continue
        example = examples[kind]
        sql = re.sub(r'https://[^\s\x27\x22]+', 'https://127.0.0.1:9/owned-unavailable', example['sql'])
        sql = re.sub(r"'(?:[a-zA-Z]:\\|\\\\)[^']*'", "'/var/opt/mssql/data/owned-unavailable-package'", sql)
        fixtures.append(dict(id='wave3-attempt-' + kind, statementTypes=[kind], setupSql='', sql=sql, expectedVerdict=example['expectedVerdict'], verifySql='SELECT 1;', beforeScalar=1, afterScalar=1, cleanupSql='', attemptOnly=True))
    Path(args.output).write_text(json.dumps(dict(schemaVersion=1, fixtures=fixtures, nonExecutableNodes=unavailable), indent=2) + '\n')
    print(f'{len(fixtures)} exact SQL candidates; {len(unavailable)} AST-only/legacy nodes explicitly unclaimed')


if __name__ == '__main__':
    main()
