# Pure result aggregation: dot-sourcing this file performs no UI or SQL operations.
function Get-CheckOutcome {
    param([object[]]$Results, [string[]]$ExpectedBlockedScenarios = @())
    $rows = @($Results)
    $failed = @($rows | Where-Object { $_.Status -eq 'FAIL' -or $_.Status -notin @('PASS','FAIL','BLOCKED') })
    $blocked = @($rows | Where-Object { $_.Status -eq 'BLOCKED' -and $_.Scenario -notin $ExpectedBlockedScenarios })
    $missing = @($ExpectedBlockedScenarios | Where-Object { $_ -notin @($rows.Scenario) })
    $duplicates = @($rows | Group-Object Scenario | Where-Object Count -gt 1)
    $code = if ($failed.Count -or $duplicates.Count) { 1 } elseif (!$rows.Count -or $blocked.Count -or $missing.Count) { 2 } else { 0 }
    [pscustomobject]@{
        SchemaVersion = 1
        Outcome = @('PASS','FAIL','INCOMPLETE')[$code]
        ExitCode = $code
        Scenarios = $rows.Count
        Failed = $failed.Count
        UnexpectedBlocked = @($blocked | ForEach-Object Scenario)
        MissingExpectedScenarios = $missing
        ExpectedBlockedScenarios = $ExpectedBlockedScenarios
    }
}
