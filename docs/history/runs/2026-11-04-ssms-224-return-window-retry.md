> **Snapshot — not maintained.** Current state: [status.md](../../status.md). All current docs: [docs/README.md](../../README.md).

# SSMS 22.4 return-window retry — November 4, 2026

The formerly incomplete return-to-window-A case **PASSed** on **SSMS 22.4.1 / 22.4.11612.150** using the same frozen,
unreleased candidate Release VSIX. Its prerequisite window-B case also passed. This closes that Release evidence gap;
the [November 3 pilot](2026-11-03-ssms-sqlcmd-compatibility-pilot.md) remains unchanged as a historical 10/11 run.
Together the two runs contain passes for all eleven distinct focused candidate Release cases. This was a two-case retry,
not a new eleven-case run, published-package requalification or candidate 22.11 runtime test.

## Scope and results

The user authorized a retry of 22.4. Only external test-helper tab navigation changed. When the owned A tab was hidden,
the helper allowed at most two guest Ctrl+Tab steps, then required the exact document, owned database and previously
captured editor runtime identity. The successful retry needed one step. SQL text, count, counter, context and verified-target
assertions remained unchanged; their preservation receipt is retained externally. Product code and packages were not changed.

| Case | Input and scope | Submission evidence | Result |
|---|---|---|---|
| Window B prerequisite | Guest SendInput F5, whole window | Exactly one intended batch; B session 62, owned SSMS process 5088; counter delta `[0,0,1]`; verified target, matching context, no target warning | PASS |
| Return to window A | Guest SendInput F5, red selection on line 3 | Exactly one intended selected batch; original A session 56, same owned process; counter delta `[1,0,0]`; verified target, matching context, no target warning | PASS |

Independent review compared before/after batch prefixes, exact SQL, successful completion, session/process/application
identity in raw Extended Events and counter changes. Both cases displayed confirmation. The raw events normalize line
endings to LF; the externally recorded expected editor-text hash uses CRLF. This is injected guest F5 evidence, not human
physical-key coverage. No toolbar, Cancel, themes, unknown-target, uninstall or Debug-channel checks were repeated.
No UIA TextPattern crash retry was attempted. Release execution was checked with the external SQL oracle, without a Debug channel.

The prior return-A helper failed to find the hidden tab; its zero-match condition shared a generic uniqueness error.
This retry restored the cached A editor identity after bounded navigation. It does not establish a product routing defect
or justify weakening ambiguity checks. The earlier failures and incomplete evidence are retained.

## Identity and retained evidence

| Artifact | Identity |
|---|---|
| Source base | `5e8c7f20c33c611b93846465602f1cd15669c4f2` plus the same isolated, uncommitted SQLCMD candidate described in the pilot |
| Candidate source patch SHA-256 | `EEC3142B5A6B82C0A2F603523B439ECF5E3BFE7DA42998501AD0ED05A0699612` |
| Candidate Release VSIX SHA-256 | `C09C435244C7DAA923ED2E13A78F48E9AE52758645EF30ABB9787E107A438ECC` |
| Candidate Release DLL SHA-256 | `B2FACFC2D1113194A1CD83D90676E767AF1610CA36DF92159181BF2424D01AF0` |
| Requested / actual SSMS | `22.4.1` / `22.4.11612.150`; installer exit 3010 followed by normal guest restart |
| Bootstrap SHA-256 | `FA33ED7913B30E6C73F160E9F71445E2F51E4F63F9555CBA44E8F25B610AA204` |
| Export `retry-final.zip` SHA-256 | `D405936D9521F93083F2CF9CCA9C055F11789D16B7508492AA0C63BEA2C88E87` |

Raw evidence is outside the repository in the retained `ssms224-retry-20261104-0700` workspace: `retry-final.zip`, its
expanded `retry-final` tree, installation/export receipts, `independent-evidence-review.json`, ownership/binding/navigation
records, before/after oracles, raw Extended Events, strict-assertion preservation, controller logs and cloud cleanup receipts.
The archive hash matched both guest and host receipts. Prior snapshots and evidence were not overwritten.

The original retry clock is **07:05:08.8109188 UTC**; controller finalization completed **07:40:33 UTC**. A T3 restart
cancelled an auxiliary read-only status shell, but the owned controller, installer and tunnel survived and were not duplicated.
One earlier IAP lookup failure was recovered by replacing only its exited tunnel after verifying the new VM identity.
The historical campaign clocks were retained. No product build or full offline suite was repeated: the prior candidate
gate remains 1,627 .NET tests, 10 Python tests, 527 rules and Debug/Release builds PASS. Four documentation index tests
were run for this documentation update; their result is retained in the external finalization receipt.

## Cleanup and remaining limits

The owned database and Extended Events session were cleaned and SSMS exited. Mandatory cloud cleanup verified empty
VMs, disks, snapshots, addresses, owned network, subnet and firewall at **07:40:26 UTC**, and project billing was unlinked.
The exact owned IAP process tree was verified and closed; its listener is absent. This retry's private/public SSH keys were
removed. Previously blocked older key sets remain for manual cleanup; their deletion was not retried. The same monitor was
disabled with no next run. The billing account and cumulative $5 budget/$4.50 alert remain for delayed charges; actual
billed cost is still pending, and the budget is not a spending cap. No owned paid cloud resources remain.

Candidate 22.11 runtime regression, the separate Debug return-A channel gap, broader unknown-target/uninstall checks,
the 22.5 VSIX installer blocker and remaining minor-line coverage are still incomplete. The
[22.11 metadata supplement](2026-11-03-ssms-sqlcmd-2211-metadata.md) supports one adapter statically, without runtime PASS.
The original published-VSIX results remain separate. No product SQL access, commit, push or publication occurred.
