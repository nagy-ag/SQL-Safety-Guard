import fs from 'node:fs';
import path from 'node:path';
import crypto from 'node:crypto';
import { fileURLToPath } from 'node:url';

const corpusDirectory = path.dirname(fileURLToPath(import.meta.url));
const args = process.argv.slice(2);
const outputDirectory = args.shift();
const testEvidencePath = args.shift();
if (!outputDirectory || !testEvidencePath || args.length === 0)
    throw new Error('Usage: node generate-matrix.mjs <output-directory> <tests.trx> <runtime-results.json> [...]');
const load = filename => JSON.parse(fs.readFileSync(filename, 'utf8'));
const digest = filename => crypto.createHash('sha256').update(fs.readFileSync(filename)).digest('hex');
const manifest = load(path.join(corpusDirectory, 'manifest.json'));
const limitations = new Map(load(path.join(corpusDirectory, 'runtime-limitations.json')).limitations.map(entry => [entry.statementType, entry.reason]));
const testXml = fs.readFileSync(testEvidencePath, 'utf8');
const counters = testXml.match(/<Counters\b[^>]*>/)?.[0] ?? '';
const attribute = name => Number(counters.match(new RegExp(name + '="(\\d+)"'))?.[1] ?? -1);
if (attribute('total') < 792 || attribute('passed') !== attribute('total') || attribute('failed') !== 0)
    throw new Error('A complete passing test run (at least 792 tests) is required.');
const staticKinds = new Map();
const inputs = [];
for (const [filename, category] of [['parser-fixtures.json', 'default-parser-sql'], ['dialect-fixtures.json', 'alternate-dialect-sql'], ['ast-only-fixtures.json', 'constructed-ast-only']]) {
    const filenamePath = path.join(corpusDirectory, filename);
    inputs.push({ file: filename, sha256: digest(filenamePath) });
    for (const fixture of load(filenamePath).fixtures) {
        if (staticKinds.has(fixture.statementType)) throw new Error('Duplicate static kind: ' + fixture.statementType);
        staticKinds.set(fixture.statementType, { category, fixtureFile: filename, expectedVerdict: fixture.expectedVerdict });
    }
}
const runtime = new Map();
const runs = args.map(filename => {
    const report = load(filename);
    if (!report.AnalyzerSha256 || !report.HarnessSha256) throw new Error('Missing binary provenance: ' + filename);
    if (report.PlannedFixtures !== undefined && report.PlannedFixtures !== report.ExecutedFixtures)
        throw new Error('Interrupted runtime report: ' + filename);
    for (const result of report.Results) {
        const kinds = result.Fixture?.StatementTypes ?? result.StatementTypes;
        if (!kinds?.length) throw new Error('Missing explicit kind links: ' + result.Id);
        const observation = { evidenceFile: path.basename(filename), fixtureId: result.Id, status: result.Status,
            verdict: result.Verdict?.Overall, stage: result.Stage ?? null, error: result.Error ?? null,
            visibleChanged: result.VisibleChanged ?? null, persistedChanged: result.PersistedChanged ?? null,
            before: result.Before ?? null, after: result.After ?? null,
            scalarChanged: result.Before !== undefined && result.After !== undefined ? result.Before !== result.After : null };
        for (const kind of kinds) {
            if (!staticKinds.has(kind)) throw new Error('Unknown runtime kind: ' + kind);
            if (!runtime.has(kind)) runtime.set(kind, []);
            runtime.get(kind).push(observation);
        }
    }
    return { file: path.basename(filename), sha256: digest(filename), analyzerSha256: report.AnalyzerSha256,
        harnessSha256: report.HarnessSha256, fixturesSha256: report.FixturesSha256 ?? null,
        startedUtc: report.StartedUtc, finishedUtc: report.RunUtc, serverVersion: report.ServerVersion ?? report.Results[0]?.ServerVersion,
        fixtures: report.Results.length, passed: report.Results.filter(result => result.Status === 'pass').length };
});
if (new Set(runs.map(run => run.analyzerSha256)).size !== 1) throw new Error('Analyzer binaries differ between runs.');
const entries = manifest.entries.map(entry => {
    const observations = runtime.get(entry.statementType) ?? [];
    const passed = observations.some(observation => observation.status === 'pass');
    const status = passed ? 'live-pass' : observations.length ? 'live-attempt-failed' : 'not-live-tested';
    const reason = passed ? null : limitations.get(entry.statementType) ?? observations.map(item => item.error).filter(Boolean).join('; ');
    if (!staticKinds.has(entry.statementType) || (!passed && !reason)) throw new Error('Unexplained coverage gap: ' + entry.statementType);
    return { statementType: entry.statementType, staticEvidence: staticKinds.get(entry.statementType), runtimeStatus: status, reason, observations };
});
if (entries.length !== 334 || staticKinds.size !== 334 || new Set(entries.map(entry => entry.statementType)).size !== 334)
    throw new Error('Inventory must account for exactly 334 unique concrete kinds.');
const summary = { concreteKinds: entries.length, staticKinds: staticKinds.size,
    defaultParserKinds: entries.filter(entry => entry.staticEvidence.category === 'default-parser-sql').length,
    alternateDialectKinds: entries.filter(entry => entry.staticEvidence.category === 'alternate-dialect-sql').length,
    astOnlyKinds: entries.filter(entry => entry.staticEvidence.category === 'constructed-ast-only').length,
    livePassedKinds: entries.filter(entry => entry.runtimeStatus === 'live-pass').length,
    failedOnlyKinds: entries.filter(entry => entry.runtimeStatus === 'live-attempt-failed').length,
    notLiveTestedKinds: entries.filter(entry => entry.runtimeStatus === 'not-live-tested').length };
const report = { generatedUtc: new Date().toISOString(), scriptDomVersion: manifest.scriptDomVersion,
    allKindsLiveVerified: summary.livePassedKinds === 334, summary,
    tests: { file: path.basename(testEvidencePath), sha256: digest(testEvidencePath), passed: attribute('passed') },
    manifestSha256: digest(path.join(corpusDirectory, 'manifest.json')),
    runtimeLimitationsSha256: digest(path.join(corpusDirectory, 'runtime-limitations.json')),
    generatorSha256: digest(fileURLToPath(import.meta.url)), inputs, runs, entries };
fs.mkdirSync(outputDirectory, { recursive: true });
fs.writeFileSync(path.join(outputDirectory, 'statement-matrix.json'), JSON.stringify(report, null, 2) + '\n');
const escape = value => String(value ?? '').replaceAll('|', '&#124;').replace(/[\r\n]+/g, ' ');
const markdown = ['# Combined 334-kind verification matrix', '',
    '- Static: ' + summary.staticKinds + '/334 (' + summary.defaultParserKinds + ' default parser, ' + summary.alternateDialectKinds + ' alternate dialect, ' + summary.astOnlyKinds + ' AST-only).',
    '- Live passed: ' + summary.livePassedKinds + '; failed-only: ' + summary.failedOnlyKinds + '; not live tested: ' + summary.notLiveTestedKinds + '.',
    '- A live pass means the representative fixture executed and its declared observer/verdict assertions passed. Unchanged scalar counts do not prove key restoration or physical/external effects.',
    '- No claim of all-clause coverage, all-engine compatibility, complete side-effect observation, or SSMS/VSIX dispatch testing.', '',
    '| Kind | Static evidence | Runtime | Fixture IDs / limitation |', '|---|---|---|---|',
    ...entries.map(entry => '| ' + [entry.statementType, entry.staticEvidence.category, entry.runtimeStatus,
        entry.observations.map(item => item.fixtureId + ': ' + item.status).join(', ') + (entry.reason ? '; ' + entry.reason : '')].map(escape).join(' | ') + ' |')];
fs.writeFileSync(path.join(outputDirectory, 'statement-matrix.md'), markdown.join('\n') + '\n');
console.log(JSON.stringify(summary));
