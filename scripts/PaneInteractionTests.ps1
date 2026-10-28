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
    $groupType = $assembly.GetType('SqlSafetyGuard.UI.PaneGroup', $true)
    $findingType = $assembly.GetType('SqlSafetyGuard.UI.PaneFinding', $true)
    $control = [Activator]::CreateInstance($assembly.GetType('SqlSafetyGuard.UI.SafetyPaneControl', $true))
    $script:checks = 0

    function Invoke-Control([string] $Name, [object[]] $Arguments) {
        $control.GetType().GetMethod($Name, [Reflection.BindingFlags]'Instance,Public,NonPublic').Invoke($control, $Arguments)
    }

    function Assert-State([bool] $Condition, [string] $Message) {
        if (-not $Condition) { throw $Message }
        $script:checks++
    }

    function New-Group([string] $Key, [SqlSafetyGuard.Analyzer.Verdict] $Verdict) {
        $finding = [Runtime.Serialization.FormatterServices]::GetUninitializedObject($findingType)
        $value = [SqlSafetyGuard.Analyzer.Finding]::new(1, 1, 0, 8, 'UPDATE', $Key, $null, $Verdict, 'Test finding')
        $findingType.GetField('<Finding>k__BackingField', [Reflection.BindingFlags]'Instance,NonPublic').SetValue($finding, $value)
        $findings = [Array]::CreateInstance($findingType, 1)
        $findings.SetValue($finding, 0)
        return [Activator]::CreateInstance($groupType, [object[]]@($Key, $findings))
    }

    $first = New-Group 'dbo.First' RealWrite
    $second = New-Group 'dbo.Second' RealWrite
    $safe = New-Group '#Temporary' TempOnly
    $groups = [Array]::CreateInstance($groupType, 3)
    $groups.SetValue($first, 0)
    $groups.SetValue($second, 1)
    $groups.SetValue($safe, 2)
    Invoke-Control ShowResult @('Test', 'Nothing executes', $groups)
    Invoke-Control ClickGroup @($first)
    Assert-State $first.IsExpanded 'First click should expand its row.'
    Invoke-Control ClickGroup @($second)
    Assert-State (-not $first.IsExpanded -and $second.IsExpanded) 'Opening another row should close the first.'
    Invoke-Control ClickGroup @($safe)
    Assert-State (-not $second.IsExpanded -and $safe.IsExpanded) 'Expansion must be exclusive across both lists.'
    $control.FindName('GroupList').SelectedItem = $first
    Assert-State (-not $safe.IsExpanded -and -not $first.IsExpanded) 'Keyboard-style selection should close old details without opening new ones.'
    Invoke-Control ClickGroup @($first)
    Invoke-Control ClickGroup @($first)
    Assert-State (-not $first.IsExpanded) 'Clicking an open row again should collapse it.'
    Invoke-Control ClickGroup @($safe)
    Invoke-Control ShowResult @('Test', 'Nothing executes', $groups)
    Assert-State $safe.IsExpanded 'Reanalysis should preserve the one expanded object.'
    Invoke-Control Select @($second)
    Assert-State (-not $safe.IsExpanded) 'Caret-driven selection should collapse previous details.'
    Invoke-Control ClickGroup @($second)
    foreach ($source in @([Windows.Controls.ListBoxItem]::new(), [Windows.Controls.Primitives.ScrollBar]::new(), [Windows.Controls.Primitives.ToggleButton]::new())) {
        $eventArgs = [Windows.Input.MouseButtonEventArgs]::new([Windows.Input.Mouse]::PrimaryDevice, 0, [Windows.Input.MouseButton]::Left)
        $eventArgs.RoutedEvent = [Windows.UIElement]::PreviewMouseLeftButtonDownEvent
        $eventArgs.Source = $source
        Invoke-Control OnAnalysisBackgroundMouseDown @($control, $eventArgs)
        Assert-State $second.IsExpanded 'Row content, scrollbars, and section toggles must not count as background.'
    }
    $analysis = $control.FindName('AnalysisPage')
    $backgroundClick = [Windows.Input.MouseButtonEventArgs]::new([Windows.Input.Mouse]::PrimaryDevice, 0, [Windows.Input.MouseButton]::Left)
    $backgroundClick.RoutedEvent = [Windows.UIElement]::PreviewMouseLeftButtonDownEvent
    $analysis.RaiseEvent($backgroundClick)
    Assert-State (-not $second.IsExpanded) 'A routed pane-background click should collapse details.'
    Assert-State ($null -eq $control.FindName('GroupList').SelectedItem -and $null -eq $control.FindName('SafeList').SelectedItem) 'A pane-background click should clear both list selections.'
    Invoke-Control ClickGroup @($safe)
    Invoke-Control Select @($null)
    Assert-State (-not $safe.IsExpanded -and $null -eq $control.FindName('SafeList').SelectedItem) 'Editor deselection should close safe details and clear the selected item.'
    Write-Output "Passed $script:checks pane interaction checks; no SSMS interaction or SQL execution."
}
finally {
    [AppDomain]::CurrentDomain.remove_AssemblyResolve($resolver)
}
