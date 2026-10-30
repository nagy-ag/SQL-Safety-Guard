> **Snapshot — not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Light/Dark capture tooling trial — 2026-10-30

The Debug VSIX was installed in the isolated Exp profile by uninstalling, installing and refreshing configuration.
The capture script ran Light and Dark, creating 13 PNGs per theme. No Execute command or SQL was dispatched.

The captured surfaces were the green/yellow/red pane, yellow/red confirmation previews, guard-failure preview,
all three list editors, import preview, pane settings, integrated options and main-window toolbar. The sample import
added one allow-list pattern and one trusted procedure; its real widening warnings and old/new diff were visible.
Save and Import were disabled and confirmation Run anyway was inert. No settings import was applied.

Several PNGs were opened, including both options pages, confirmation previews, the import preview and pane content.
The images showed real controls with the respective theme colours. The Dark safe-findings text has low contrast;
that existing visual issue remains for a separate UI change. The other 11 themes and execution behaviour were not tested.

SSMS startup can activate itself before an extension loads. The final script therefore creates a private Windows desktop
for each process and never displays or switches to it. A Debug UI-thread activation veto also protects channel operations.
No real keyboard, mouse or clipboard was used. Only the launched processes/windows were addressed.

WPF surfaces were rendered in-process at their layout size and DPI after ContextIdle/ApplicationIdle. Dialog captures
use the WPF client root, excluding native chrome. Whole-window captures use PrintWindow(PW_RENDERFULLCONTENT) plus
the live WPF HWND roots at their actual positions: PrintWindow alone omitted WPF content on an undisplayed desktop.
Options readiness was confirmed using its visible F5 warnings control before capture. Pixel variance alone was insufficient
to catch PrintWindow's missing content, so visual inspection remains required.

Each run backed up the settings bytes and SHA-256 before modifying the theme. Successful completion and failed-run cleanup
both restored the original hash. The owned SSMS processes closed using WM_CLOSE; no process was killed. The final restoration
receipt records matching hashes and SsmsExited=true. Exp retains the Debug build. PNGs, manifest, trace receipts and settings
backup/restoration evidence remain outside the repository.

Validation: all 1,571 unit tests pass, including DocsIndexTests and AgentSkillsSyncTests. Visual Studio MSBuild Debug and
Release builds each have zero warnings/errors. Release pane/confirmation/settings interaction checks pass (13/15/4).
Release IL contains none of the capture verb literals or capture helper types. This is tooling verification, not full release
visual acceptance, and does not add server-verification evidence or change analyzer verdicts.
