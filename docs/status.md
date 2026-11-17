# Status

How well SQL Safety Guard is verified **right now**. This page is rewritten in place; earlier states and every piece of
evidence are frozen in [history/](history/README.md). What the colours promise is defined in the
[contract](safety/contract.md).

## At a glance

| Question | Answer |
|---|---|
| Current release | **1.1.0**, stable and Latest, promoted 2026-11-17 ([promotion record](history/runs/2026-11-17-release-1.1.0-stable.md), [package verification](history/releases/1.1.0.md)). |
| Current checkout | **1.1.1 release candidate; the owner has resumed stable/Latest publication.** Includes the verified expanded-object scrolling fix and themed trusted-routine selector. See the [changelog](../CHANGELOG.md), [scrolling record](history/runs/2026-11-17-expanded-object-scrolling.md) and [dropdown record](history/runs/2026-11-17-trusted-routine-dropdown.md). |
| Known false greens | None confirmed open. All 19 focused probes of the packaged analyzer PASS; five authorized write effects are recorded as policy effects, not read-only proof. The last full campaign remains [2026-10-30](history/runs/2026-10-30-full-run.md). |
| Offline gate | Published 1.1.0 passes 1,913 .NET tests and 10 Python tests; generated references and the 548-rule inventory are fresh; Debug/Release builds and all 93 Release WPF checks pass. |
| Latest pane check | Local candidate: clean Debug/Release rebuilds, 1,913 tests, 14 Release settings checks and two visually inspected Dark captures verify the trusted-routine selector and popup. Owned SSMS closed and settings restored ([record](history/runs/2026-11-17-trusted-routine-dropdown.md)). The separate [scrolling check](history/runs/2026-11-17-expanded-object-scrolling.md) passes 69 Release WPF checks and three live cases. Normal profile retains published 1.1.0. |
| Latest focused server check | 19 probes of the packaged 1.1.0 analyzer PASS: variable SQL, failed USE continuation, repeated control flow, dynamic context restoration and typed approvals; owned databases removed ([record](history/releases/1.1.0.md)). Earlier [variable-SQL campaign](history/runs/2026-11-11-variable-dynamic-sql.md) remains separate evidence. |
| SSMS versions | Focused checks on representative 22.0 to 22.11 builds, including an in-place 22.9 to 22.11 upgrade; the full minor-version matrix is incomplete (open limit 1) |
| Waiting for a decision | Remaining company policy includes permanent tempdb opt-in, logging and other session state. Qualified-trust and USE behavior is owner-approved ([policy questions](safety/policy-questions.md)) |
| Uninspected runtime effects | Deployed bodies and dependencies are not retrieved. Visible module definitions have separate previews; proven local strings and visible remote/job commands are inspected with context labels. Remote dialects, runtime text, Agent tokens and CLR code remain unproved. Physical F5 keys are not synthesized. |

## Latest release: 1.1.0

[SQL Safety Guard 1.1.0](https://github.com/nagy-ag/SQL-Safety-Guard/releases/tag/v1.1.0) is the stable Latest release.
It adds variable-backed dynamic SQL, typed code previews, qualified authorization, blue database context and the
compact six-color pane. **Old short approval rules are preserved but inactive** until explicitly migrated;
read the [upgrade instructions](user/install.md#upgrading-to-110).

The clean committed-source gate, 93 Release WPF checks, 52 Dark functional captures, real settings transfer,
exact Release upgrade/load/uninstall, 19 focused database probes, all five downloads/digests and source/tag CI passed.
The [release record](history/releases/1.1.0.md) distinguishes Debug functional evidence, exact Release checks,
authorized write policy effects and limits. The extension remains text-only and the nesting limits remain 3 and 8.

The owner requested stable promotion on 2026-11-17. The [promotion record](history/runs/2026-11-17-release-1.1.0-stable.md)
verifies the stable/Latest metadata and the same five asset identities and checksums. This promotion uses the original
verified package; it does not represent a new server, routing or theme campaign. The original prerelease verification
record remains a frozen historical snapshot. Previous releases, including [1.0.5](history/releases/1.0.5.md), remain available.

## Open limits

1. **SSMS version matrix.** Each 22.0–22.10 representative passed the 11 strict execution cases with the 1.0.0
   compatibility candidate, and 1.0.3 survived the 22.9 to 22.11 upgrade. Automated execution on the newest build, broad
   Debug routing, a deliberately unknown target, the Release uninstall menu handoff and 22.3 integrated settings remain incomplete. Native isolated Release install/upgrade/uninstall passed for 1.1.0.
2. **Server campaign results.** Unavailable-feature, observation-gap and unresolved-target cases from the last run stay
   as they are; they are not promoted to passes.
3. **Policy decisions.** The items in [policy questions](safety/policy-questions.md) keep their current behaviour until
   the owner decides. Typed confirmation stays disabled.
4. **New campaigns.** A new frozen campaign needs a clean reviewed checkout and an explicitly authorized disposable
   environment; none is scheduled.
5. **Same-version 1.0.0 packages.** The replaced 1.0.0 packages share a version number: uninstall and reinstall, and
   identify them by checksum (published replacement VSIX `944732eb…0d3c87`).
6. **Company signing.** Signing with a company key and verifying the signature chain and timestamp are pending on the
   company computer; GitHub packages stay unsigned.
7. **Cloud test resources.** Resources are deleted and billing unlinked; removal of approval-blocked keys and the actual
   billed cost are still pending.
8. **Variable SQL proof.** Straight-line supported string assignments only; branches/loops/TRY-CATCH, unmodeled
   statements, runtime values, truncation and unsupported types/conversions discard proof. The focused server run
   is representative evidence, not exhaustive coverage of string semantics or existing indirect effects.
9. **Object attribution.** The compact pane preserves analyzed operations; ordinary reads are not attributed to every
   referenced object, unknown targets are not guessed, and decoded operations retain their outer EXEC's execution anchor.
   Optional proven string-source ranges support navigation; missing mappings fall back to EXEC. Dynamic SQL is grouped
   by its EXEC rather than merged into an ordinary object row.
   The mixed three-colour presentation check uses a synthetic fixture, not new analyzer proof.

10. **Visible-body preview limits.** Remote SQL is interpreted as T-SQL without proving its dialect/runtime. Stored Agent
    commands require proven named inputs and a known T-SQL subsystem; positional arguments, tokens and runtime values
    remain opaque. Calls never resolve deployed definitions or dependencies. Qualified built-ins and object methods may
    conservatively warn. Preview counts are not execution counts; nesting and resource limits stay explicit.

## Where the evidence is

| Looking for | Go to |
|---|---|
| Every release, with its acceptance and publication records | [history/releases/](history/releases/README.md) |
| The full evidence ledger and earlier status, as of 2026-11-09 | [status snapshot](history/runs/2026-11-09-status-snapshot.md) |
| All dated runs, newest first | [history/](history/README.md) |
| How each kind of check works and what it proves | [verification tools](dev/verification-tools.md), [testing in SSMS](dev/ssms-testing.md) |
