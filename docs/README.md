# Documentation — start here

Everything on this page is **current**: it is updated in place and always describes the product as it is now. Files with a date or
a wave number in their name are snapshots and live in [history/](history/README.md); they are never edited again.

## Current

| Document | What it answers | Kind |
|---|---|---|
| [user-guide.md](user-guide.md) | How to install, read the colours, configure and share settings. For users. | Living, hand-written |
| [it-deployment.md](it-deployment.md) | All-users install and uninstall, security facts, checks. For IT. | Living, hand-written |
| [release-process.md](release-process.md) | How to version, test, tag and publish a release. | Living, hand-written |
| [vsix-signing.md](vsix-signing.md) | Sign a finished VSIX on a company laptop: key privacy, pilot certificate, trust and release order. | Living, hand-written |
| [verification/status.md](verification/status.md) | Where verification stands right now: latest run, false greens, open gaps, next steps. **Read this first.** | Living, hand-written |
| [verification/policy-questions.md](verification/policy-questions.md) | Behaviours that need a company decision, with the short list on top. | Living, hand-written |
| [statement-verdicts.md](statement-verdicts.md) | Which colour every statement kind gets, for permanent and temp targets. | Generated, test-guarded |
| [verification/contract.md](verification/contract.md) | What green, yellow and red promise. | Living, hand-written |
| [verification/how-to.md](verification/how-to.md) | How to regenerate the rule inventory, run rewrites and collect coverage. | Living, hand-written |
| [verification/rules/rules.md](verification/rules/rules.md) | Every analyzer rule (readable form of `rules.json`). | Generated |
| [verification/rules/uncovered.md](verification/rules/uncovered.md) | Code and rule IDs no test reaches. | Generated |
| [verification/extension/extension-checks.md](verification/extension/extension-checks.md) | SSMS execution checks: which window runs, which commands are intercepted. | Living, hand-written |
| [release-acceptance.md](release-acceptance.md) | The latest release acceptance (build, install, theme checks). | Replaced at each release |

The changelog is [../CHANGELOG.md](../CHANGELOG.md).

Machine-readable companions sit next to their page: `verification/rules/` (`rules.json`, `rule-test-map.json`,
`rule-id-migrations.json`, `semantic-rules.tsv`) and `verification/extension/` (TSV/JSON evidence).

## Rules that keep this page true

1. A current file has no date or wave in its name and is updated in place.
2. A dated result goes to `history/` and starts with a "Snapshot — not maintained" line pointing back to `status.md`.
3. After each verification run: rewrite `verification/status.md` and add the run report under `history/runs/`.
4. Generated files say so in their first lines; regenerate them, don't edit them.
5. `DocsIndexTests` fails when a link here is broken or when a document outside `history/` is not listed on this page.
