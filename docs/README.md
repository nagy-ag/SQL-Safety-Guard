# Documentation — start here

## Who are you?

| You want to… | Start here | Then |
|---|---|---|
| Use SQL Safety Guard in SSMS | [user/user-guide.md](user/user-guide.md) | [install](user/install.md), [settings](user/settings.md), [statement verdicts](user/statement-verdicts.md) |
| Deploy it for a company | [it/deployment.md](it/deployment.md) | [company signing](it/signing.md) |
| Change or release it | [CONTRIBUTING.md](../CONTRIBUTING.md) | [development](dev/development.md), [architecture](dev/architecture.md) |
| Know whether you can trust it | [status.md](status.md) | [what the colours promise](safety/contract.md), [history](history/README.md) |

The product overview is the [repository README](../README.md). Everything below is **current** and updated in place;
dated evidence lives in [history/](history/README.md) and is never edited again.

## user/ — using SQL Safety Guard

| Document | What it answers |
|---|---|
| [install.md](user/install.md) | Requirements, downloads, installing, updating and uninstalling for yourself. |
| [user-guide.md](user/user-guide.md) | The colours, the safety pane, the F5 confirmation, samples to try, troubleshooting and limitations. |
| [settings.md](user/settings.md) | Every option and its editor, with copyable allow-list, trusted-procedure and environment-rule examples; sharing settings. |
| [statement-verdicts.md](user/statement-verdicts.md) | The colour of every T-SQL statement kind, for permanent and temporary targets. Generated, test-guarded. |

## it/ — deploying in a company

| Document | What it answers |
|---|---|
| [deployment.md](it/deployment.md) | Security facts, all-users install and uninstall, how the install scripts work, post-install checks. |
| [signing.md](it/signing.md) | Using the downloadable company signing kit: input checks, key privacy, pilot certificate, trust and release order. |

## dev/ — developing and releasing

| Document | What it answers |
|---|---|
| [development.md](dev/development.md) | Safe first build, isolated debugging, validation commands and implementation notes. |
| [architecture.md](dev/architecture.md) | Project boundaries, dependencies, traced flows and implementation limits. |
| [ssms-testing.md](dev/ssms-testing.md) | Checks against a real SSMS: visual and settings checks without SQL, and execution routing checks. |
| [verification-tools.md](dev/verification-tools.md) | Where every verification tool lives, the offline checks, and what each kind of check proves. |
| [release-process.md](dev/release-process.md) | How to version, test, tag, publish and record a release. |

## safety/ — what the product promises

| Document | What it answers |
|---|---|
| [contract.md](safety/contract.md) | What green, yellow and red promise, and how a real change is classified. |
| [policy-questions.md](safety/policy-questions.md) | Behaviours that need a company decision, with the short list on top. |

## Status and evidence

| Document | What it answers |
|---|---|
| [status.md](status.md) | How well it is verified right now: at a glance, latest release, open limits. **Read this first.** |
| [verification/rules/rules.md](verification/rules/rules.md) | Every analyzer rule (readable form of `rules.json`). Generated. |
| [verification/rules/uncovered.md](verification/rules/uncovered.md) | Rules without named-test associations; no measured coverage claim. Generated. |
| [history/](history/README.md) | Dated run reports and per-version [release records](history/releases/README.md). Frozen. |

Machine-readable companions sit next to their page in `verification/rules/` (`rules.json`, `rule-test-map.json`,
`rule-id-migrations.json`, `semantic-rules.tsv`). Release downloads are on
[GitHub Releases](https://github.com/nagy-ag/SQL-Safety-Guard/releases); version changes are in the [changelog](../CHANGELOG.md).

## Rules that keep this page true

1. A current file has no date or wave in its name, lives in the folder for its audience and is updated in place.
2. A dated result goes to `history/` and starts with a "Snapshot — not maintained" line pointing back to `status.md`.
3. `status.md` describes now only. After a verification run or release, add the dated record under `history/` and update
   status; never append history to it.
4. Generated files say so in their first lines; regenerate them, don't edit them.
5. A page kept at an old path for old links starts with `# Moved:` and points to the new page.
6. `DocsIndexTests` fails when a link is broken, a current document is not listed here, or a document sits outside its folder.
