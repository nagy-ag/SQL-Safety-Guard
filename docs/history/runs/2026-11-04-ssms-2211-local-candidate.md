> **Snapshot ? not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# Local SSMS 22.11.2 candidate checks ? November 4, 2026

The owner requested runtime testing of the same unreleased SQLCMD compatibility candidate on the laptop after the cloud
campaigns. The installed host was **22.11.2 / 22.11.12324.101**, a different patch from the published package's earlier cloud
22.11.3 result. This run supplies partial candidate runtime evidence; it does not establish full host compatibility.

The frozen candidate Release VSIX SHA-256 was
`C09C435244C7DAA923ED2E13A78F48E9AE52758645EF30ABB9787E107A438ECC`, and the installed Release DLL matched
`B2FACFC2D1113194A1CD83D90676E767AF1610CA36DF92159181BF2424D01AF0`.
The frozen Debug VSIX was `1E1F7D2FA959B45888A98CE49A07FC6D7190C1E1553A930798562E09F88B5832`.
Source remained the isolated, uncommitted candidate based on `5e8c7f20c33c611b93846465602f1cd15669c4f2`, with the previously
recorded candidate patch `EEC3142B5A6B82C0A2F603523B439ECF5E3BFE7DA42998501AD0ED05A0699612`.
No product or assertion change, rebuild, full offline rerun, cloud provisioning or billing change was made for this run.
Prior candidate evidence remains 1,627 .NET / 10 Python / 527 rules and successful Debug/Release builds.

| Check | Outcome | Evidence and limits |
|---|---|---|
| Candidate Release install/load and green/yellow/red panes | PASS, 3/3 | Package load completed in Activity.xml; real pane UIA verdict controls; installed DLL hash matched. Native module enumeration alone did not list the managed assembly. |
| Candidate Release settings | PASS | Pane settings and integrated F5 warnings page exposed; no settings values changed. |
| Actual Execute toolbar, whole-window Cancel | PASS | Warning appeared with verified target and expected database context; zero owned query submissions; all counters unchanged. |
| Actual Execute toolbar, whole-window approved Run | PASS | Intended complete SQL submitted exactly once; raw XE matched owned PID/application/session; red, yellow and outside counters each advanced once. |
| Candidate Release injected F5 / selected execution | INCOMPLETE | Initial and paced-key attempts failed the unchanged scope precondition: expected selected line 3, observed whole window. No execution scenarios completed. A separate observation gate expired; all failures retained. |
| Candidate Release two-window execution | NOT TESTED | The selected-keyboard prerequisite was not established; no coverage inferred from cloud results or Debug probes. |
| Debug pane edits, selection and second document | PASS, 6 checks | Whole buffer, green selection, clear selection, red selection, yellow procedure and active B matched exact text hashes and pane counts. These are Debug-channel checks, not Release execution proof. |
| Focused Debug SQLCMD state | OBSERVED, 3 samples | A-off, A-on and B-off had correct document, focused editor ownership, adapter value and comparison state; both editor boolean properties agreed. No SQL executed. |
| Focused Debug return to A | INCOMPLETE | The hidden A document did not consume the probe command. The earlier no-focus helper captured B when A was requested; its raw FAIL rows are retained, not promoted to PASS. |
| Debug unverified target Cancel | PASS, limited observation | Guard displayed an explicit target warning; owned-database XE had zero completions for the exact owned PID across application names, with zero counter change. No general ambiguous-target matrix claim. |
| Debug native settings/list checks | PASS | Native settings receipt and list trace retained; separate from Release UI acceptance. |
| In-extension uninstall handoff | NOT TESTED | Cleanup used the external installer CLI; that is not evidence of the product menu handoff or confirmation. |

Initial testing used private desktops without mouse, keyboard, clipboard or shared foreground focus. The owner then explicitly
authorized input only to the owned SSMS test instance. External input was bound to exact PID, start time and executable,
and UIA remained scoped to that PID. No clipboard was used. The shared normal profile was not installed into or replaced.

The laptop did not demonstrate injected keyboard selection, even after focus and input-desktop checks and one paced-key
correction. Both computer-use interfaces also failed initialization with `failed to write kernel assets: The system cannot
find the path specified. (os error 3)`. The actual toolbar path was therefore tested as two distinct whole-window scenarios;
the selected execution assertions were not relaxed or relabeled. There is no human physical-key claim.

The initial no-focus Debug instance acknowledged Quit and closed its GUI but left a background process after the normal
cleanup wait. Its exact PID/start/suffix were reverified before termination and SQL cleanup. The cause was not triaged;
this is a shutdown qualification, not proof of an adapter defect. Later Release and focused Debug instances closed normally.

All five owned disposable databases and matching XE sessions were independently verified absent. Both owned profile
packages were removed by the exact isolated-profile installer commands, exit 0; profile folders and raw evidence were retained.
All **18 pre-existing protected settings/assembly files** retained their original hashes. No other SSMS profile or database
was adopted. No cloud resources were created and the previous cloud cleanup/billing unlink was left untouched.

Raw evidence remains outside the repository in the host candidate run: package/source receipts, UIA/activity records,
original failed helpers, focused probe TSVs, exact SQL/XE before/after observations, independent reviews and cleanup receipts.
The external final manifest records their hashes. Only documentation-index checks followed the documentation update;
no commit, push or publication occurred.
