# Status

How well SQL Safety Guard is verified **right now**. This page is rewritten in place; earlier states and every piece of
evidence are frozen in [history/](history/README.md). What the colours promise is defined in the
[contract](safety/contract.md).

## At a glance

| Question | Answer |
|---|---|
| Current release | **1.0.5**, stable and Latest, published 2026-11-10 ([release record](history/releases/1.0.5.md)) |
| Source on main | Published 1.0.5 source plus its documentation and verification records; procedure-body inspection, unified row gestures and reversed section chevrons ([functional verification](history/runs/2026-11-10-row-gestures.md)) |
| Known false greens | None open. The last full server campaign (2026-10-30, about 11 hours) found no real false green after single-worker triage ([report](history/runs/2026-10-30-full-run.md)) |
| Offline gate | 1.0.5 passes 1,693 .NET tests and 10 Python tests; the generated rule inventory and statement verdicts are checked for freshness |
| SSMS versions | Focused checks on representative 22.0 to 22.11 builds, including an in-place 22.9 to 22.11 upgrade; the full minor-version matrix is incomplete (open limit 1) |
| Waiting for a decision | Trusted procedures, allow-list effects, permanent tempdb opt-in, logging and session state ([policy questions](safety/policy-questions.md)) |
| Uninspected runtime effects | Bodies of called procedures are not retrieved; triggers and SQL built at runtime are not inspected (yellow, never green). Procedure definitions present in the editor have a separate potential-effects breakdown. Physical F5 key presses are not synthesized in tests. |

## Latest release: 1.0.5

[SQL Safety Guard 1.0.5](https://github.com/nagy-ag/SQL-Safety-Guard/releases/tag/v1.0.5) inspects procedure definitions
present in the editor and nests their potential effects inside the procedure row. Outer and nested objects share
single-click highlighting, double-click expansion and individual statement highlights. Body and safe-section chevrons
point down when collapsed and up when expanded. Execution verdicts, F5 policy, settings formats and the text-only
boundary are retained; deferred body findings do not enter execution counts.

The committed-source gate passed 1,693 .NET tests, 10 Python tests, the Release rebuild and 58 WPF checks.
The functional candidate also passed 21 isolated editor checks plus refresh and 25 procedure checks in Light;
those used Debug-only in-process events. All five published downloads match the local assets and GitHub digests.
No new SQL/server campaign, physical input coverage, all-theme matrix or exact downloaded-asset installation is claimed.
Details: [release record](history/releases/1.0.5.md), [preparation](history/runs/2026-11-10-release-1.0.5-preparation.md)
and [functional verification](history/runs/2026-11-10-row-gestures.md).

## Open limits

1. **SSMS version matrix.** Each 22.0–22.10 representative passed the 11 strict execution cases with the 1.0.0
   compatibility candidate, and 1.0.3 survived the 22.9 to 22.11 upgrade. Automated execution on the newest build, broad
   Debug routing, a deliberately unknown target, Release uninstall and 22.3 integrated settings remain incomplete.
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

## Where the evidence is

| Looking for | Go to |
|---|---|
| Every release, with its acceptance and publication records | [history/releases/](history/releases/README.md) |
| The full evidence ledger and earlier status, as of 2026-11-09 | [status snapshot](history/runs/2026-11-09-status-snapshot.md) |
| All dated runs, newest first | [history/](history/README.md) |
| How each kind of check works and what it proves | [verification tools](dev/verification-tools.md), [testing in SSMS](dev/ssms-testing.md) |
