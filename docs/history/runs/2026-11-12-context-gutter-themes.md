> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Context counts, shared gutter and themed settings

Local, unpublished update on 2026-11-12. Published release 1.0.5 is unchanged.

Toggle hover now shows the name and count followed by the color meaning shared with the settings dictionary.
The former submitted-operation, on/off and label-mode help is removed from these tooltips. The six independent
adaptive controls retain their existing layout and label modes.

USE has a primary blue Context marker and a numbered object count. It is excluded from green display totals,
and Context works independently of Safe. Leaf operations use their left marker and line number without a repeated
right-side blue accent. A decoded USE/SELECT block shows blue 1 and green 1. The analyzer verdict remains ReadOnly;
execution analysis, destination-policy escalation and the F5 guard are unchanged.

Object, operation and preview headers share one line-number gutter, measured from every analyzed operation's
source/execution line, including collapsed descendants. Right-aligned tabular digits and spacing keep counts aligned.
Filtering and expansion preserve the width; a new shorter analysis can release it. No divider or fixed five-digit
reservation was introduced.

Settings use six circle/name/description rows grouped into Effects and Code and context, with SQL terms expanded
separately. Checkboxes and the F5 warning ComboBox use supported shell styles; labels and expanders use theme
foregrounds. The dark dropdown popup has dark fill and readable light text.

## Verification

| Check | Result |
|---|---|
| Locked offline gate | 1,913 .NET tests and 10 Python tests PASS; generated references and 548-rule inventory fresh |
| Builds | Debug and Release PASS; existing DialogChrome threading-analyzer warnings remain |
| Release WPF | 55 pane checks and 14 settings checks PASS |
| Isolated SSMS 22.11, verified Dark | 31 nonblank captures; 15 context/gutter/hover, 19 automatic-pane, 175 typed-body, 21 nested-body, 12 compact-settings, 11 scroll, four settings-list and two appearance checks PASS |
| Visual inspection | Dark pane, six-row legend, Enabled labels and separate warning-dropdown popup checked; an initial light-palette run also passed its UI checks |
| Restoration | Isolated settings file restored byte-for-byte; owned SSMS exited cleanly |
| Local installation | All 25 installed payload files match the final frozen Release package; normal-profile settings unchanged and extension cache refreshed |

The context tests cover all 64 masks for ordinary and decoded USE/SELECT while preserving analyzer verdicts.
Live checks compare the gutter edge across object, operation and body headers with one- through four-digit lines.
The WPF checks verify stable width after filtering and reclaimed space after new results. Existing function forms,
nested previews, navigation, persistent highlights and circular scope markers remain covered.

The first offline attempt hit three existing analyzer time budgets while builds and SSMS setup were running.
The standalone rerun passed all 1,913 tests without changing budgets. Initial startup JSON requested Dark but the
shell migrated to a light palette; those images are not claimed as dark coverage. The driver now sets the theme
through the native settings manager in the owned profile and verifies actual Dark/Light shell resource brightness.
The final Dark run passed with a live background of `#FF282828`. It captures the ComboBox popup separately because
that popup is a separate presentation surface. A final group-count tooltip copy cleanup followed the captures;
both builds passed afterward and the installed package includes that cleanup.

No SQL execution, new server campaign, physical input simulation, all-theme matrix, commit, push or release publication
was performed. Raw receipts, screenshots and packages remain outside the repository. Earlier real-server evidence is
in the [qualified-trust report](2026-11-12-qualified-trust.md).

Installed package SHA-256: `0F183708D092930CE62FAFE485177BEE9E53663A1EC733826D2836B09E3DBBE9`.
