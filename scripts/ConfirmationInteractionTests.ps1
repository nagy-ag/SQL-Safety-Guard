param([string] $Configuration = 'Debug')

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName PresentationFramework
$repository = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$output = Join-Path $repository "src\SqlSafetyGuard.Vsix\bin\$Configuration"
$ssms = 'C:\Program Files\Microsoft SQL Server Management Studio 22\Release\Common7\IDE'
$script:probePaths = @($output, (Join-Path $ssms 'PublicAssemblies'), (Join-Path $ssms 'PrivateAssemblies'), $ssms)
$resolver = [ResolveEventHandler] {
    param($sender, $eventArgs)
    $name = ([Reflection.AssemblyName]::new($eventArgs.Name)).Name + '.dll'
    foreach ($directory in $script:probePaths) {
        $candidate = Join-Path $directory $name
        if (Test-Path -LiteralPath $candidate) { return [Reflection.Assembly]::LoadFrom($candidate) }
    }
    return $null
}
[AppDomain]::CurrentDomain.add_AssemblyResolve($resolver)
try {
    $assembly = [Reflection.Assembly]::LoadFrom((Join-Path $output 'SqlSafetyGuard.dll'))
    [void][Reflection.Assembly]::LoadFrom((Join-Path $output 'SqlSafetyGuard.Analyzer.dll'))
    $snapshotType = $assembly.GetType('SqlSafetyGuard.Services.QuerySnapshot', $true)
    $script:checks = 0

    function Assert-State([bool] $Condition, [string] $Message) {
        if (-not $Condition) { throw $Message }
        $script:checks++
    }

    function New-Dialog([string] $Sql, [string] $Database = 'Orders', [object] $Result = $null) {
        $snapshot = [Activator]::CreateInstance($snapshotType, [object[]]@($null, $Sql, $true, 20, 1))
        if ($null -eq $Result) { $Result = [SqlSafetyGuard.Analyzer.SqlSafetyAnalyzer]::Analyze($Sql, $null) }
        $ledgerType = $assembly.GetType('SqlSafetyGuard.UI.ConfirmationLedgerRow', $true)
        $rows = $ledgerType.GetMethod('Create', [Reflection.BindingFlags]'Static,NonPublic').Invoke($null, [object[]]@($result, $snapshot, $Database))
        $xaml = Get-Content -Raw -Encoding UTF8 (Join-Path $repository 'src/SqlSafetyGuard.Vsix/UI/ConfirmExecutionDialog.xaml')
        $xaml = $xaml.Replace('platformUi:DialogWindow', 'UserControl')
        $xaml = $xaml -replace ' x:Class="[^"]+"', ''
        $xaml = $xaml -replace ' Click="[^"]+"', ''
        $xaml = $xaml -replace ' [A-Za-z]+="{DynamicResource[^"
]+}"', ''
        $xaml = $xaml -replace '<Setter Property="Foreground"[^>]+/>', ''
        [xml] $markup = $xaml
        foreach ($attribute in @($markup.DocumentElement.Attributes)) {
            if ($attribute.Prefix -ne 'xmlns' -and $attribute.Name -ne 'xmlns') { [void] $markup.DocumentElement.RemoveAttributeNode($attribute) }
        }
        $dialog = [Windows.Markup.XamlReader]::Parse($markup.OuterXml)
        $redRows = @($rows | Where-Object { $_.Verdict -eq [SqlSafetyGuard.Analyzer.Verdict]::RealWrite })
        $yellowRows = @($rows | Where-Object { $_.Verdict -eq [SqlSafetyGuard.Analyzer.Verdict]::Unknown })
        $dialog.FindName('RedList').ItemsSource = $redRows
        $dialog.FindName('YellowList').ItemsSource = $yellowRows
        $dialog.FindName('RedCount').Text = $redRows.Count.ToString()
        $dialog.FindName('YellowCount').Text = $yellowRows.Count.ToString()
        if ($redRows.Count -eq 0) { $dialog.FindName('RedSection').Visibility = 'Collapsed' }
        if ($yellowRows.Count -eq 0) { $dialog.FindName('YellowSection').Visibility = 'Collapsed' }
        if (@($result.Warnings | Where-Object { -not $_.WillRun }).Count -gt 0) { $dialog.FindName('BlockedText').Visibility = 'Visible' }
        return $dialog
    }


    $dialog = New-Dialog "UPDATE dbo.First SET Id=1;
DELETE dbo.First WHERE Id=2;
EXEC dbo.Opaque;
SELECT 1;"
    $red = $dialog.FindName('RedSection')
    $yellow = $dialog.FindName('YellowSection')
    Assert-State (-not $red.IsExpanded -and -not $yellow.IsExpanded) 'Both sections must start collapsed, including a single row.'
    Assert-State ($dialog.FindName('RedCount').Text -eq '1' -and $dialog.FindName('YellowCount').Text -eq '1') 'Counts must use grouped warnings only, excluding green reads.'
    $row = @($dialog.FindName('RedList').ItemsSource)[0]
    Assert-State ($row.ObjectName -eq 'dbo.First' -and $row.Statements -eq ('UPDATE ' + [char]0xB7 + ' DELETE') -and $row.Lines -eq '20, 21') 'Ledger must group actions and map selection lines.'
    $red.IsExpanded = $true
    $yellow.IsExpanded = $true
    Assert-State ($red.IsExpanded -and $yellow.IsExpanded) 'Category dropdowns should expand independently.'
    Assert-State $dialog.FindName('RunButton').IsEnabled 'Legacy typed policy must not disable Run anyway.'
    Assert-State ($dialog.FindName('CancelButton').IsDefault -and $dialog.FindName('CancelButton').IsCancel) 'Enter and Escape must cancel.'

    $dialog = New-Dialog 'DELETE [Other].[dbo].[First];' $null
    Assert-State (@($dialog.FindName('RedList').ItemsSource)[0].ObjectName -eq 'dbo.First') 'Database prefix must not appear in ledger object labels.'
    Assert-State $dialog.FindName('RunButton').IsEnabled 'Unknown database must never require typing.'
    Assert-State ($dialog.FindName('YellowSection').Visibility -eq 'Collapsed') 'Empty categories should be hidden.'

    $sql = (1..45 | ForEach-Object { "DELETE dbo.Table$_;" }) -join "
"
    $dialog = New-Dialog $sql
    $section = $dialog.FindName('RedSection')
    $section.IsExpanded = $true
    $scroll = $section.Content
    Assert-State ($dialog.FindName('RedCount').Text -eq '45') 'All overflow objects must remain represented.'
    Assert-State ($scroll.MaxHeight -eq 190 -and $scroll.VerticalScrollBarVisibility -eq 'Auto') 'Overflow region must retain its height cap and scrollbar.'
    $dialog.Measure([Windows.Size]::new(660, 720))
    $dialog.Arrange([Windows.Rect]::new(0, 0, 660, $dialog.DesiredSize.Height))
    $dialog.UpdateLayout()
    Assert-State ($scroll.ScrollableHeight -gt 0) 'Overflow entries must produce a scrollable layout.'
    $scroll.ScrollToEnd()
    $dialog.UpdateLayout()
    Assert-State ($scroll.VerticalOffset -gt 0) 'The overflow list must scroll to later entries.'

    $blocked = [SqlSafetyGuard.Analyzer.Finding]::new(2, 1, 10, 12, 'DELETE', 'dbo.Blocked', $null,
        [SqlSafetyGuard.Analyzer.Verdict]::RealWrite, 'Blocked batch', $false, 2, $true)
    $result = [SqlSafetyGuard.Analyzer.AnalysisResult]::new([SqlSafetyGuard.Analyzer.Finding[]]@($blocked), [SqlSafetyGuard.Analyzer.SqlParseError[]]@())
    $dialog = New-Dialog -Sql 'Blocked batch fixture' -Result $result
    Assert-State ($dialog.FindName('BlockedText').Visibility -eq 'Visible') 'Syntax-error batches need an explicit caveat.'
    Assert-State (@($dialog.FindName('RedList').ItemsSource | Where-Object { $_.Opacity -lt 1 }).Count -gt 0) 'Blocked entries must be dimmed.'
    Write-Output "Passed $script:checks ledger/dropdown checks (rendering and shell theming excluded); no SSMS interaction or SQL execution."
}
finally {
    [AppDomain]::CurrentDomain.remove_AssemblyResolve($resolver)
}
