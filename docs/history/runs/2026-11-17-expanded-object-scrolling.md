> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Expanded-object scrolling fix

On 2026-11-17 the owner reported that scrolling stalls inside a large expanded procedure and requested a local fix
and verification. The local candidate retains version 1.1.0; it is **unreleased** and differs from the published package.
The normal SSMS profile retains the [published GitHub 1.1.0 installation](2026-11-17-github-latest-1.1.0-install.md).

## Cause and change

The ListBox uses one item per object, including all expanded operation/body rows. With the default logical item
scrolling, an 80-operation object produced a viewport of one item and zero scrollable height, hiding later operations.
The shared list now sets `VirtualizingPanel.ScrollUnit` to `Pixel`. This permits movement inside oversized objects
while retaining virtualization, one scrollbar and the existing hierarchy, filters, gestures and colors.

## Verification

- The regression against the exact published analyzer/UI payload reproduces the failure: 80 operation headers,
  viewport 1, scrollable height 0. The same regression passes against the local fix.
- Debug and Release builds PASS. The existing two DialogChrome threading warnings remain separate from this change.
- All 1,913 .NET tests PASS. All 69 Release WPF pane checks PASS, including five new behavior checks: an oversized
  object exposes its content, routed wheel input over an operation moves less than a page, expansion/selection stay
  unchanged, the final operation is fully reachable and scrolling back restores the first operation.
- The frozen local Release VSIX installs into a fresh owned isolated SSMS 22.11 profile; all 24 installed payload files
  match the candidate. The three live UI Automation checks PASS on an undisplayed private desktop:

| Fixture | Operation headers including definitions | Small scroll | List viewport | Result |
|---|---:|---:|---:|---|
| Procedure with 80 DELETE operations | 81 | 20 DIP | 756 DIP | Last operation reached; returned to top |
| Scalar function with 80 DECLARE operations and RETURN | 82 | 20 DIP | 756 DIP | Last operation reached; returned to top |
| Decoded dynamic SQL with 80 DELETE operations | 80 | 20 DIP | 756 DIP | Last operation reached; returned to top |

The live driver targets the pane's GroupList explicitly, opens its rows through UI Automation, and drives its existing
scroll provider. Routed mouse-wheel behavior is verified separately by the Release WPF regression. No real mouse,
keyboard, clipboard, shared foreground focus, SQL connection or execution is used. Each owned SSMS session closes
cleanly and its pre-launch settings are restored byte for byte. The normal-profile runtime still matches all 24
published GitHub payload files. Raw logs, candidate VSIX, baseline failure, UI names and receipts stay outside the repository.

The changelog records this as Unreleased; the user guide and SSMS testing guide describe the behavior and regression.
This focused scrolling check adds no new F5 routing, server, full-theme or classification evidence.
