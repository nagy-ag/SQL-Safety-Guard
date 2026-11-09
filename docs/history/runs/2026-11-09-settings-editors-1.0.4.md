> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Settings editors, Options scrolling and title bars: 1.0.4 verification

## Scope

Three owner-reported settings issues, fixed for release 1.0.4. No analyzer verdict, F5 policy, stored settings format or
SQL access boundary changed.

- **Options scrolling.** When the integrated Options page is taller than its window, SSMS repeats the scrolled-past
  category as a sticky header above the scrolling list, including the category's preview content. With no following
  category its height is unlimited, so a second full copy of the compact settings form covered the page and received
  the mouse wheel. The copy now collapses itself; it is recognized by its `CategoryView` having a finite
  `MaxVerticalExtent`, and an unrecognized host keeps the form visible.
- **List settings.** The page shows only counts and a problem line. Safe tables and trusted procedures open a list with
  Server / Database / Schema / Name fields, a note, a description and a Try-a-name check (owner-chosen design S2).
  Environment rules open an ordered list with a form per rule (design D2). Edit as text keeps the stored text format.
  The editor model parses and composes the stored text, keeps unedited entries byte-for-byte, and validates and matches
  only through the analyzer's `AllowList` and `EnvironmentRules`. Save uses the existing import validation.
- **Title bars.** Dialogs request a dark title bar for dark themes and, on Windows 11, the dialog's own colours.

## Results

| Check | Result |
|---|---|
| Reproduction before the fix | FAIL as reported: after forced overflow and a scroll, a second 531 px copy of the form overlaid the page |
| Offline suite | PASS: 1,663 .NET tests (28 new editor-model tests, including export/import round trips), 10 Python tests |
| Debug and Release builds | PASS |
| Release WPF settings checks | PASS: 14 (text editor and compact page with count summaries and problem lines) |
| Isolated SSMS, Light and Dark | PASS: native settings 12, scrolling 11 (forced overflow, wheel over every control, sticky copy hidden, hit-tests reach content), editors 4 (driven through their real controls) |
| Title bar receipts, Dark | PASS: all seven dialog previews report dark mode and caption colour #242424 accepted (HRESULT 0) |
| Capture shutdown and settings restoration | PASS: clean exit; original settings bytes restored in both runs |

The editor checks add entries through the form, use Try a name, save, confirm that an entry with a problem blocks Save,
reorder, confirm that unchanged text survives exactly, and confirm that Cancel stores nothing.

## Limits

The isolated SSMS runs on a private desktop without window composition, so the title bar itself was not seen there; the
receipts show only that Windows accepted the attributes. Edit as text inside SSMS, and a theme change while a dialog
is open, were not driven. Wheel input was raised in-process, not with the physical mouse. The structured dialogs need
SSMS's theme service and are therefore checked inside SSMS rather than by the standalone WPF script.
