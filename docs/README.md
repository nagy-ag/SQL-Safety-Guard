# Documentation — start here

Everything on this page is **current**: it is updated in place and always describes the product as it is now. Files with a date or
a wave number in their name are snapshots and live in [history/](history/README.md); they are never edited again.
The product overview is the [repository README](../README.md).

## Using SQL Safety Guard

| Document | What it answers |
|---|---|
| [install.md](install.md) | Requirements, downloads, installing, updating and uninstalling for yourself. |
| [user-guide.md](user-guide.md) | The colours, the safety pane, the F5 confirmation, samples to try, troubleshooting and limitations. |
| [settings.md](settings.md) | Every option, with copyable allow-list, trusted-procedure and environment-rule examples; sharing settings. |
| [statement-verdicts.md](statement-verdicts.md) | The colour of every T-SQL statement kind, for permanent and temporary targets. Generated, test-guarded. |

## Deploying in a company

| Document | What it answers |
|---|---|
| [it-deployment.md](it-deployment.md) | Security facts, all-users install and uninstall, how the install scripts work, post-install checks. |
| [vsix-signing.md](vsix-signing.md) | Using the downloadable company signing kit: input checks, key privacy, pilot certificate, trust and release order. |

## Developing and releasing

| Document | What it answers |
|---|---|
| [development.md](development.md) | Safe first build, isolated debugging and validation commands. |
| [architecture.md](architecture.md) | Project boundaries, dependencies, traced flows and implementation limits. |
| [release-process.md](release-process.md) | How to version, capture themes, test, tag and publish a release. |
| [release-acceptance.md](release-acceptance.md) | The latest release acceptance (build, install, theme checks). Replaced at each release. |

## Verification

| Document | What it answers |
|---|---|
| [verification/status.md](verification/status.md) | Where verification stands right now: latest run, false greens, open gaps, next steps. **Read this first.** |
| [verification/policy-questions.md](verification/policy-questions.md) | Behaviours that need a company decision, with the short list on top. |
| [verification/contract.md](verification/contract.md) | What green, yellow and red promise. |
| [verification/how-to.md](verification/how-to.md) | How to regenerate the rule inventory, run rewrites and collect coverage. |
| [verification/rules/rules.md](verification/rules/rules.md) | Every analyzer rule (readable form of `rules.json`). Generated. |
| [verification/rules/uncovered.md](verification/rules/uncovered.md) | Rules without named-test associations; no measured coverage claim. Generated. |
| [verification/extension/extension-checks.md](verification/extension/extension-checks.md) | SSMS execution checks: which window runs, which commands are intercepted. |
| [verification/extension/theme-capture.md](verification/extension/theme-capture.md) | Isolated visual capture, receipts, restoration checks and theme values; no SQL. |

Machine-readable companions sit next to their page: `verification/rules/` (`rules.json`, `rule-test-map.json`,
`rule-id-migrations.json`, `semantic-rules.tsv`). Dated extension TSV/JSON evidence is preserved under history/.
Operational entry points and their SQL-access status are listed in [tools/verification/README.md](../tools/verification/README.md).

Release downloads and their notes are on [GitHub Releases](https://github.com/nagy-ag/SQL-Safety-Guard/releases); version
changes are in the [changelog](../CHANGELOG.md).

## Rules that keep this page true

1. A current file has no date or wave in its name and is updated in place.
2. A dated result goes to `history/` and starts with a "Snapshot — not maintained" line pointing back to `status.md`.
3. After each verification run: rewrite `verification/status.md` and add the run report under `history/runs/`.
4. Generated files say so in their first lines; regenerate them, don't edit them.
5. `DocsIndexTests` fails when a link here is broken or when a document outside `history/` is not listed on this page.
