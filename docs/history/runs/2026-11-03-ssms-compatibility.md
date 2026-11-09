> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Focused SSMS 22.x compatibility assessment, 2026-11-03

## Outcome and scope

**Incomplete: this run does not establish compatibility across the SSMS 22.x minor lines.** All 12 selected Microsoft
bootstrappers were available and signed. The only installed SSMS 22 build discovered was **22.11.2, full build 22.11.12324.101**. That extra
host sample supplied released-package load/pane/settings evidence and limited Debug evidence. It was not substituted
for the selected newest patch, 22.11.3. No other host version was installed or replaced.

The owner clarified that the target is one available build from every released **22.x minor**, rather than every patch;
the separate company-build requirement and priority were removed. The intended order is newest, oldest, then remaining
minor lines. The approximately four-hour target was a planning allowance. Work stopped substantially earlier because
the VM prerequisites and supported automation blocked the remaining runtime coverage, rather than expanding into
alternative virtualization systems or a server/analyzer campaign. Exact start/end timestamps are in external provenance.

## Source and frozen packages

- Tested source: `5e8c7f20c33c611b93846465602f1cd15669c4f2`, initially clean. Its product, test, script, tooling and skill
  trees matched the released `v1.0.0` source, `0cd57d004988cb7eb8080c89b1ee2b3136a5c0c2`.
- Fresh GitHub release download, **1.0.0 VSIX SHA-256**:
  `c4068832e698f6faa9998fa7042267d115c1ca3810feed7f69c1cafae64a4ec9`.
- The two local Release assembly hashes matched those in the published VSIX. Reused those binaries; no per-version
  rebuild. The archive entry inventory contains runtime assets, not the external test helpers.
- One Debug solution build succeeded and was frozen before testing. **Debug VSIX SHA-256**:
  `b168ec36e44325abf2e075dc6824dc5f69d04a13b98c11adaf2804e1a9224385`.
  Debug extension assembly SHA-256: `ed9b86b8083d19010cb306ac3a8b9785adff786ebedd28ef2b44c69d1b7fde2e`.
- The offline verification gate ran once: **1,621 .NET tests, 10 Python tests passed; 527 generated rules current**.
  Source/tool/package identities, SDK information, file hashes and logs are retained externally.
- No production code, classifier behavior, assertions or release package was changed. Helper adaptations stayed outside
  the repository. The maintained broad scenario runner's SHA-256 matched its repository original.

## VM and Docker feasibility

Hyper-V management tools and the running VM services were present on Windows 11 Pro. Firmware virtualization and an
active hypervisor were reported. The session had a medium-integrity token, Administrators membership marked deny-only,
and no enabled Hyper-V Administrators membership. **Both Get-VM and Get-VMSwitch returned "You do not have the required
permission to complete this task."** No existing VM could be adopted or inspected through those commands. No VM or
checkpoint was created or booted.

The initial sample had about **13.85 GiB physical RAM, 2.09 GiB free RAM, 7.53 GiB free on C: and 3.14 GiB free on D:**.
Available capacity was inadequate for an additional useful Windows/SSMS test VM while preserving the running laptop.
The CPU's other virtualization capability fields were masked/false under the active hypervisor; they were not treated
as proof of unsupported hardware. No other installed VM provider was discovered through command and uninstall-registry
inspection. No matching Windows ISO/VHD/VM files were found in the accessible searched user/development/secondary-drive
paths. The media search had an access denial in a protected secondary-drive directory, so it is not an exhaustive absence
claim. Usable Windows installation media and VM-management permission remain unproved prerequisites.

Docker **Desktop 4.77.0 / Engine 29.5.3** was accessible through the Linux context despite the helper service being stopped.
There were no running containers. A Linux/amd64 SQL Server 2025 image was cached, with its digest retained. Docker cannot
supply the Windows SSMS desktop. No container was started because available host RAM was already tight; existing Docker
configuration and workloads were left alone. The small execution observation used only the run's newly created, allowed
extension-check database and identically named XE session on the approved local playground instance. No server-wide
configuration was changed, and no product SQL connection/server dependency was introduced.

## Selected versions and actual installation

Selection comes from Microsoft's live [release history](https://learn.microsoft.com/en-us/ssms/release-history) and
[SSMS 22 release notes](https://learn.microsoft.com/en-us/ssms/release-notes-22), saved during this run. The live pages listed
22.11.3 dated October 29; cached search results still listed 22.11.2, so the live documents governed selection.

| Minor line | Selected release | Bootstrapper | Installed full build / runtime result |
|---|---|---|---|
| 22.0 | 22.0 | Downloaded, signature valid | Not installed — VM blocked |
| 22.1 | 22.1 | Downloaded, signature valid | Not installed — VM blocked |
| 22.2 | 22.2.1 | Downloaded, signature valid | Not installed — VM blocked |
| 22.3 | 22.3.3 | Downloaded, signature valid | Not installed — VM blocked |
| 22.4 | 22.4.1 | Downloaded, signature valid | Not installed — VM blocked |
| 22.5 | 22.5.2 | Downloaded, signature valid | Not installed — VM blocked |
| 22.6 | 22.6.0 | Downloaded, signature valid | Not installed — VM blocked |
| 22.7 | 22.7.2 | Downloaded, signature valid | Not installed — VM blocked |
| 22.8 | 22.8.2 | Downloaded, signature valid | Not installed — VM blocked |
| 22.9 | 22.9.2 | Downloaded, signature valid | Not installed — VM blocked |
| 22.10 | 22.10.2 | Downloaded, signature valid | Not installed — VM blocked |
| 22.11 | 22.11.3 | Downloaded, signature valid | Not installed — VM blocked |
| Extra installed sample | 22.11.2 | Existing host installation | **22.11.12324.101 — partial evidence below** |

Bootstrapper HTTP success, embedded version, SHA-256, resolved URL and Microsoft signer were recorded for each selection.
This does not prove payload availability or that an old bootstrapper would install that exact servicing patch. No
unavailable bootstrapper was silently replaced. Only the host's actual installation was verified through both vswhere
catalog data and SSMS.exe product/file version.

## Scenarios on the extra installed 22.11.2 sample

All application sessions used owned isolated profiles and undisplayed private desktops. No real keyboard, mouse,
clipboard, shared foreground activation, physical shortcuts or other agent's process/profile was used.

| Scenario | Published Release 1.0.0 | Frozen Debug build | What was actually demonstrated |
|---|---|---|---|
| Install and package load | PASS | PASS | Profile-bound VSIX installation; Release Activity.xml package-load completion and real pane UIA content |
| Representative green/yellow/red pane | PASS, static file loads | PASS, live buffer replacements | SELECT safe summary; unknown procedure yellow count; UPDATE red count and no-WHERE finding |
| Selection / whole window | Incomplete | 4 PASS | Whole buffer, selected green, cleared selection, selected red: exact snapshot hashes and pane counts; no execution |
| Multiple windows and execution binding | Incomplete | Incomplete | Second editor analysis appeared in traces, but the text-only open timed out and verified execution binding was not exercised |
| Cancel submits nothing | Incomplete | Narrow observation only | Unverified-target green dispatch warned; Cancel left zero observed owned-DB SSMS Query batches and counters at 0/0/0 |
| Approved Run submits intended SQL exactly once | Incomplete | Incomplete | No approved Run observation completed |
| Unknown/ambiguous targets warn | Incomplete | One observed warning | Missing focused owner/connection caused a visible yellow target warning even for green SQL; other modes/ambiguities unmeasured |
| Basic settings | PASS, read/display | PASS, native read and 4 list-editor receipts | Release pane and integrated F5 warnings page loaded; Debug exercised allow-list saving and invalid-trust/cancellation checks |
| In-extension uninstall handoff | Incomplete | Not attempted in this run | Release submenu command was not reachable in private-desktop UIA; CLI cleanup is not handoff acceptance |
| Light/dark on selected newest | Not run | Not run | Selected 22.11.3 was unavailable as a runtime; no theme substitution or visual acceptance claim |
| Broader extension checks | Not applicable to Debug channel | Setup aborted, 0 broad scenarios completed | Runner never reached its maintained scenario campaign; no aggregate PASS |

The Release pane samples came from three disconnected scratch-file sessions, not editing as the user types. Debug supplied
the live-update/selection evidence. Raw Release collection intentionally left its generic rows incomplete; reviewed UIA
and package-load receipts support only the specific passing subchecks above. Managed-module enumeration returned no
matching module entries and was not used as package-load proof.

The broad Debug setup left a connection-confirmation dialog pending and could not verify a focused SQL command-target
owner on the undisplayed desktop. Its initial Query.Execute dispatch therefore warned on green text. The guard was not
bypassed, focus was not forced, and assertions were not weakened. An external PID/start-time-validated recovery invoked
the real Cancel button, sampled the owned XE ring buffer before/after, then closed only the owned session. The observation
is scoped to sql_batch_completed events for that owned database, exact PID and SSMS Query application; it does not prove
the absence of every possible server submission outside that scope. Physical F5/Ctrl+E/Alt+X and toolbar execution are
not covered by this run. Historical command-dispatch results remain historical.

## Failures, limits and next useful scope

No product defect was demonstrated by the completed subchecks. This is not evidence that the untested hosts or execution
paths work. Blocking/failed automation receipts remain retained: MSBuild autodiscovery (corrected by an explicit path),
an ambiguous Settings label (corrected by control type), unavailable uninstall submenu, broad-run target/connection setup,
and the second-window open timeout. The four passing selection checks are not a six-check or multiwindow PASS. Host
Activity.xml also contains unrelated MEF/Copilot/settings startup errors; no clean-host-log claim is made.

Resume the 12-line matrix on one Windows VM with sufficient RAM/disk, permitted Hyper-V management, usable Windows media,
a clean checkpoint and an allowed way to establish actual editor focus. Install versions sequentially and verify the
installed build each time. Start with **22.11.3 and 22.0**, then 22.1 through 22.10; run the broader suite only on the two
anchors, and light/dark only on newest. There is no separate company-build lane after the owner's clarification. If time
must be narrowed further, those two anchors plus the existing installed-host evidence are the smallest useful next scope;
the remaining minor lines must stay explicitly incomplete.

## Evidence and cleanup

External bundle: **`compat-20261103-1408`**, under the sibling `harness-out` directory. Machine-specific paths are recorded
there, not embedded in repository documentation. The bundle retains Microsoft source HTML and signed bootstrappers,
`selected-installers.json`, `bootstrapper-signatures.json`, `provenance.json`, package/assembly hashes, the released VSIX,
the frozen Debug output, build/offline logs, isolated profile/PID ownership records, UIA snapshots, channel events and
snapshot hashes, native settings/list receipts, before/after XE XML and counters, recovery/cleanup receipts and external
helper scripts. Raw evidence and frozen binaries remain outside the repository and release assets.

Both owned test VSIX installations were removed through the explicitly profile-bound CLI, with installer exit 0 and
absence of their extension DLLs verified. This proves cleanup, not in-extension handoff acceptance. Both owned SSMS
sessions are gone. The owned database and XE session are independently confirmed absent. All **16** pre-existing
protected settings/assembly hashes match the initial baseline, and the host remains build **22.11.12324.101**. No Docker
containers are running, as at the initial sample. Two owned isolated profile caches/settings remain for inspection;
external packages, bootstrappers and raw evidence are intentionally retained.

Initial cleanup attempts correctly retained the database while a modal dialog blocked exit. Recovery used only the
owned guard Cancel, the observed native shutdown prompt, and Don't Save for the owned scratch document. The raw failed
cleanup receipts are retained beside the later successful recovery and independent final checks.

Only this snapshot, its history index entry and current verification status were updated in the repository. No commit,
push, publication, host SSMS replacement, reboot, host network change, other-worktree edit or Docker workload change was
performed. Targeted documentation checks are recorded with the final external assessment.
