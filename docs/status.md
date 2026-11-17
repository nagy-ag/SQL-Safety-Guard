# Status

How well SQL Safety Guard is verified **right now**. This page is rewritten in place; earlier states and every piece of
evidence are frozen in [history/](history/README.md). What the colours promise is defined in the
[contract](safety/contract.md).

## At a glance

| Question | Answer |
|---|---|
| Current release | **1.0.5**, stable and Latest, published 2026-11-10 ([release record](history/releases/1.0.5.md)) |
| Current checkout | **1.1.0 acceptance candidate**, prepared for prerelease packaging. Includes variable-backed dynamic SQL, typed previews, fully qualified approvals, blue USE context, six adaptive filters, shared line gutter and numbered collapsed summary. The [changelog](../CHANGELOG.md) and [upgrade guide](user/install.md#upgrading-to-110) describe the changes; stable/Latest remains 1.0.5 until acceptance. |
| Known false greens | None confirmed open. The qualified-trust update adds conservative context and approval coverage; 14 focused disposable SQL probes PASS. Approved writes are explicitly authorized policy effects, not proof of read-only implementations. The last full campaign remains [2026-10-30](history/runs/2026-10-30-full-run.md) |
| Offline gate | 1.1.0 preparation passes 1,913 .NET tests and 10 Python tests; generated references and the 548-rule inventory are fresh; Debug and Release builds pass |
| Latest pane check | Isolated SSMS 22.11, verified Dark: 28 captures; 43 collapsed-summary, 19 automatic-pane, 175 typed-body, 21 nested-body and 15 context/gutter/hover checks PASS. Release WPF: 64 pane checks PASS. Isolated settings restored and owned SSMS closed; all 25 normal-profile installed payloads verified with settings unchanged ([report](history/runs/2026-11-17-collapsed-spacing.md)). Earlier themed settings checks remain in the [previous report](history/runs/2026-11-12-context-gutter-themes.md) |
| Latest focused server check | 14 qualified-trust/context probes PASS, including failed USE continuation, repeated control flow, dynamic context restoration, typed approvals and authorized write effects; own disposable databases removed ([report](history/runs/2026-11-12-qualified-trust.md)). Earlier variable-dynamic evidence remains [here](history/runs/2026-11-11-variable-dynamic-sql.md) |
| SSMS versions | Focused checks on representative 22.0 to 22.11 builds, including an in-place 22.9 to 22.11 upgrade; the full minor-version matrix is incomplete (open limit 1) |
| Waiting for a decision | Remaining company policy includes permanent tempdb opt-in, logging and other session state. Qualified-trust and USE behavior is owner-approved ([policy questions](safety/policy-questions.md)) |
| Uninspected runtime effects | Deployed bodies and dependencies are not retrieved. Visible module definitions have separate previews; proven local strings and visible remote/job commands are inspected with context labels. Remote dialects, runtime text, Agent tokens and CLR code remain unproved. Physical F5 keys are not synthesized. |

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
