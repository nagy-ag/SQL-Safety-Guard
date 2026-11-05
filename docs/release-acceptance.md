# Release 1.0.3 acceptance

Updated **2026-11-05** for the owner-requested compact Options design C, repository format documentation,
main update, release 1.0.3 and changelog. Earlier releases are retained. GitHub packages remain unsigned and
support **SSMS 22.0 and later 22.x releases**, 64-bit, bounded to `[22.0,23.0)`.

## Changes

- Six settings in compact rows: explanations on the left, aligned controls on the right, actions below.
- Existing native storage, defaults, full migration, immediate persistence and editor/transfer/uninstall handlers.
- Native GEL presentation; SSMS supplies the Preview caption, but the settings are live. Keyboard focus is allowed
  within the editable form without changing other host content.
- Updated [user guide](user-guide.md#settings) with copyable list patterns, trusted procedure examples, rule fields
  and matching limits; updated [CHANGELOG.md](../CHANGELOG.md) and all version numbers.
- No analyzer verdict, F5 confirmation or SQL access boundary change.

## Local validation

| Check | Result |
|---|---|
| Locked offline suite | PASS: 1,635 .NET tests, 10 Python tests, 527 current rules |
| Debug rebuild and Release build | PASS |
| Release WPF checks | PASS: 26 pane, 15 confirmation, 14 settings |
| Installed SSMS registration schema | PASS |
| Isolated native settings assertions | PASS: 12; focus routing and native readback of all six controls |
| Light-theme capture | PASS: 13 surfaces, compact page inspected |
| Capture shutdown/settings restoration | PASS: clean exit and byte-for-byte restoration |
| Clean committed-source packaging gate | Pending |
| Published asset/download hashes | Pending |
| Original isolated extension restoration | Pending |

See the [focused UI record](history/runs/2026-11-05-compact-settings-1.0.3.md) for the exact Debug candidate identity,
initial discarded attempts and evidence limits. The final package is built from the committed release source.

## Scope and retained limits

No SQL execution, server campaign, exact Release-artifact live execution acceptance, complete theme matrix, all-version
assessment, fresh installer/uninstall acceptance, company signing or signed-copy verification was repeated. Prior
observations and open gaps remain in [verification/status.md](verification/status.md). A settings/UI pass is not
proof of new execution or server behavior. Final publication and restoration evidence is added after asset validation.
