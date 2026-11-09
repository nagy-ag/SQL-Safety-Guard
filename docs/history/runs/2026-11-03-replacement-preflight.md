> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Replacement 1.0.0 preflight, 2026-11-03

The owner requested publishing the uninstall-capable build to main and replacing the previous release while retaining
**1.0.0**, with no changelog file. This working tree is based on `5a35399`. The earlier provisional 1.1.0 record remains
historical; current package/assembly versions are 1.0.0. Release notes will live on GitHub.

## Review and checks

- Reviewed the uninstall adapter and pure targeting/confirmation helper, command IDs/table/registration, package inputs,
  offline tests, release tooling, current documentation and all four project skills. No analyzer implementation, verdict
  policy, server dependency, guard mode or settings change was added. Unknown analysis continues to fail closed.
- Removed the changelog and its current links/test dependency; the version test now checks both assembly and file versions
  plus analyzer package version against the VSIX manifest. Updated signing, deployment, same-version reinstall guidance,
  agent navigation and the identical extension skill copies in `.agents/` and `.claude/`.
- Locked offline gate passed: 1,621 .NET tests, 10 Python tests and 527 generated rules current. Debug solution rebuild passed.
  The no-server-access, documentation-index and skill-sync contracts are included in that suite.

## Live uninstall acceptance

A new owned isolated profile was installed with the current Debug VSIX and run on a private Windows desktop that was
never displayed or switched to. Only a disconnected scratch SQL file was opened; no Query.Execute or SQL connection was
requested. The existing Debug channel invoked the actual registered `SqlSafetyGuard.Uninstall` command.

- UI Automation read the real SSMS confirmation text, correct instance/profile and Yes/No buttons. No-default configuration
  is checked offline; keyboard default behavior was not synthesized on the private desktop.
- Choosing No started no installer and left the installation present.
- Choosing Yes started the adjacent interactive VSIXInstaller with the exact instance, isolated suffix and fixed extension ID.
- The owned SSMS session closed cleanly. The owned interactive installer completed removal, all owned processes exited,
  and no SQL Safety Guard DLL remained in that isolated profile.
- An independent check against the original pre-run baseline matched all 10 protected settings/extension-assembly hashes
  from existing profiles. The normal and Exp installations were not removed or replaced.

The first driver attempts could not identify the WPF dialog using native window text; SSMS's initial connection prompt
also needed cancellation. UI Automation resolved the dialog after dismissing that prompt without connecting. The final
driver incorrectly included its own installation in the preservation baseline on reuse and failed when that DLL was
successfully removed. An independent post-check excluded the owned profile, verified the original baseline and confirmed
removal/process exit. These were driver issues, not product failures. PrintWindow on the undisplayed desktop was blank;
no screenshot or fresh theme-rendering acceptance is claimed.

Receipts, the external driver, original release/tag backup and `uninstall-verified.json` are outside the repository under
`../release-out/replacement-1.0.0-20261103-131829/`. The original GitHub release's three assets matched GitHub's SHA-256 digests.
Committed-source Release build/package validation and publication/download/CI evidence follow this preflight.
No new server/F5 routing campaign, signing or full-theme pass was performed. Pending owner policy decisions are unchanged.
