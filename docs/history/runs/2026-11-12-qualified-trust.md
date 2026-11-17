> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Qualified trust, USE context and shared dictionary

Local, unpublished update on 2026-11-12. Published release 1.0.5 is unchanged.

## Behavior

- Approved write targets, procedures and functions require server.database.schema.object identities. Procedure and function approvals are separate; successful approvals are AllowListed, explicitly authorizing reachable effects rather than proving read-only implementations.
- Legacy short rules remain saved but inactive. Existing list editors provide explicit migration from known connection labels, without guessing a schema. Matching is case-sensitive; only the final object name permits wildcards. A written linked-server alias is not a verified destination.
- USE is ReadOnly with a blue context accent. The existing red/yellow/green safety markers, counts and filters remain. Prior and requested databases remain possible after USE because a failed switch can leave the prior database active; repeated control flow is conservative. Dynamic USE does not leak into the caller's context. Destination environment rules can require confirmation even when the initial context is Off.
- Definitions remain writes. Separate potential-effect counts and purple full-container outlines remain; call approvals do not authorize definitions or their body previews. Existing compact rows, inline operations, persistent highlights and joined outlines are retained.
- Settings contain a shared Colors and meanings dictionary. See [dictionary](../../user/dictionary.md), [settings](../../user/settings.md) and the [sample](../../../samples/06-qualified-trust.sql).

## Verification

| Check | Result |
|---|---|
| Locked offline suite | 1,910 .NET tests and 10 Python tests PASS; generated references and 548-rule inventory fresh |
| VSIX builds | Debug and Release PASS; existing threading-analyzer warnings remain |
| Release WPF | 44 pane and 15 confirmation checks PASS |
| Isolated SSMS 22.11, Light | 38 captures; 12 qualified-trust/context/migration checks, 175 typed-body, 21 nested-body, 24 procedure and 15 dynamic checks PASS |
| Existing settings and pane flows | Compact filters/minimum width, 12 compact-settings checks, 11 scrolling checks and four settings-editor workflows PASS |
| Focused disposable SQL probe | 14/14 PASS; approved table/procedure writes observed as POLICY EFFECT, with no false greens against the configured contract |
| Normal-profile installation | All 25 installed payload files match the frozen Release package; settings hash unchanged; extension cache refreshed |

The focused SQL probe covered USE alone, reads after USE, same-context USE, dynamic context restoration, typed approvals and kind mismatch, all possible database contexts, explicit database targets, failed USE followed by a later batch, loop revisits and an approved outer write after dynamic USE. It compared permanent table/schema snapshots and like-for-like session observations. Its own disposable databases were removed. The extension remains text-only and has no production SQL connection or metadata lookup.

An initial probe observer incorrectly expected a zero transaction-state value during its SELECT; it was corrected to compare identical before/after observations. An initial isolated run stopped on an obsolete test expectation for the Try a name message; the assertion was updated and the full final run passed. The installation gate's PowerShell array wrapper was corrected before installation; all 14 final probe records were then validated.

The isolated profile's settings were restored byte-for-byte and the owned SSMS exited cleanly. No physical keyboard/F5 input, new complete server campaign, all-theme pass or public release is claimed. Raw receipts, captures and frozen binaries remain outside the repository.

Installed package SHA-256: `AA46980DF31DF8867028D6866054426476478F9C1752919F4FEF7D82ABD25DD9`.
