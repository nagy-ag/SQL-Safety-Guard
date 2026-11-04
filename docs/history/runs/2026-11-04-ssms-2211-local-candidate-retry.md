> **Snapshot ? not maintained.** Current state: [verification/status.md](../../verification/status.md). All current docs: [docs/README.md](../../README.md).

# Local candidate retry and numbered badge change ? November 4, 2026

The owner requested another local candidate attempt, explicitly permitted input in the owned test instance, then authorized
using the normal SSMS profile temporarily and restoring its original extension afterward. The installed host remained
**22.11.2 / 22.11.12324.101**. This retry added **zero completed strict execution cases**. It does not close the
[previous local run's](2026-11-04-ssms-2211-local-candidate.md) keyboard, selection, two-window or Debug return-A gaps.

## Frozen-package attempts

The unchanged unreleased Release VSIX SHA-256 was
`C09C435244C7DAA923ED2E13A78F48E9AE52758645EF30ABB9787E107A438ECC`; its DLL was
`B2FACFC2D1113194A1CD83D90676E767AF1610CA36DF92159181BF2424D01AF0`.
The original source base remained `5e8c7f20c33c611b93846465602f1cd15669c4f2` plus the previously recorded SQLCMD candidate.

| Attempt | Actual outcome | Qualification |
|---|---|---|
| Fresh isolated Release installation | Installer and configuration refresh completed; installed DLL matched the frozen candidate | Not new execution acceptance |
| Connected execution driver | INCOMPLETE: editor runtime binding prerequisite unavailable | No case ran; owned SQL resources cleaned |
| Public editor COM diagnostic | INCOMPLETE: `RPC_E_CALL_REJECTED` when reading selection | Disconnected diagnostic; no SQL execution |
| SSMS Select All menu diagnostic | INCOMPLETE: duplicate menu observations prevented unique selection | No keyboard or selection PASS inferred |
| Authorized normal-profile attempt | INCOMPLETE: experimental-profile-only client guard refused the new launch | No SQL case ran; later cleanup verified independently |
| Explicit normal-profile launch support and scan-code retry | INCOMPLETE: query oracle rejected a tab without the owned database label | Supplied SQL file was disconnected; a separate scratch query was connected |
| Corrected connected-tab startup retry | NOT RUN: overlap guard refused while the user had reopened normal SSMS | Correction is not live-proven; no ownership or SQL assertion was relaxed |
| Computer Use initialization | BLOCKED: kernel assets could not be written, Windows path error 3 | Both JavaScript runtimes failed before controlling SSMS; resets did not repair it |

Helper changes were external only: exact PID/start/path/database ownership for the explicitly authorized normal-profile
launch; initial binding to a unique focused runtime identity when other views exist; scan-code input; and requiring the
startup tab to name the disposable database. Existing SQL, count, counter, context and target checks remained intact.
No original failure was overwritten or converted to PASS. These observations do not establish a new product defect.

The temporary normal-profile deployment copied the frozen VSIX payload into a byte-backed-up per-user installation.
It was not a fresh normal-profile VSIXInstaller acceptance result. The existing unsaved user query was saved through
SSMS and additionally copied into external evidence, without executing it.

## Requested visual change

The owner requested removal of the black outline around numbered colored circles. Four named count ellipses now have
`StrokeThickness="0"`: expanded red/yellow and collapsed red/yellow. Other markers, focus outlines, colors, counts,
SQLCMD handling and execution policy were unchanged. The four-attribute XAML edit was applied in the main checkout and
the candidate checkout; both previously dirty documentation sets and all older snapshots were preserved.

The candidate checkout's **Debug build, Release build and all 1,627 .NET tests passed** after this change. The prior
Python/rule gate was not repeated for the decorative XAML change. The SQLCMD adapter file retained SHA-256
`809C907B483118B68E129794CED65A8235CC252CB93A07097366BC8292CD63BC`.

New, separately frozen, unreleased visual-candidate packages:

| Package | SHA-256 |
|---|---|
| Debug | `3AA2B1920B0CA6A497FD3DE22DC2A0AC701EB36392725FDDF74B5692857D8D65` |
| Release | `D59FF032B2940C70967C860A3944751EFC9E423B4AB8824CA26D81705BA81B85` |

**These rebuilt packages were not installed or visually accepted in SSMS.** Original frozen-package execution results
cannot be relabeled as acceptance of these new binaries. Direct Computer Use remains blocked by its runtime initialization.

## Cleanup and retained evidence

The original normal installation was restored and all **25 original extension files** matched their backup hashes.
Its original DLL SHA-256 was `86C88A2C31F848E4748CF2213E0BC0A4F95EAB5C0F4B69D81B960FB6F24DDFD9`.
Both settings files matched the post-user-save, pre-swap backup. All **20 protected baseline files** retained their hashes.
The fresh isolated candidate package was uninstalled with exit zero. SSMS and VSIXInstaller processes were absent at the
final audit. All three owned database/event-session pairs were independently verified absent. No cloud resource was
started, billing was not enabled, and no commit, push or publication occurred.

External evidence bundle `host-candidate-retry-20261104-1515` retains the controller identities, raw failures, UIA captures,
public-COM observations, original installation/query backup, SQL cleanup audit, protected hashes, source patches, build/test
logs, new package hashes and provenance. `cleanup-final.json`, `normal-restoration.json`, `source-provenance.json` and
`badge-verification.json` are its principal receipts. Machine-specific paths remain outside the repository.
