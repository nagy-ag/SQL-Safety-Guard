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
    $section = $control.FindName('SafeSection')
    [void]$section.ApplyTemplate()
    $toggle = $section.Template.FindName('SafeSectionToggle', $section)
    [void]$toggle.ApplyTemplate()
    $chevron = $toggle.Template.FindName('SafeSectionChevron', $toggle)
    $details = $section.Template.FindName('SafeSectionDetails', $section)
    Assert-State ($null -ne $toggle -and $null -ne $chevron) 'Safe-section arrow must have an explicit template.'
    Assert-State ($toggle.Content -eq $section.Header) 'Safe-section template must preserve its header.'
    foreach ($expanded in @($false, $true, $false)) {
        $section.IsExpanded = $expanded
        $geometry = $chevron.Data.GetFlattenedPathGeometry().Figures[0]
        $tip = $geometry.Segments[0].Points[0]
        Assert-State ($geometry.StartPoint.Y -eq $(if ($expanded) { 4 } else { 0 }) -and $tip.Y -eq $(if ($expanded) { 0 } else { 4 })) 'Collapsed arrow must point down; expanded arrow must point up.'
        Assert-State ($details.Visibility -eq $(if ($expanded) { [Windows.Visibility]::Visible } else { [Windows.Visibility]::Collapsed })) 'Safe-section content visibility must follow its expanded state.'
        Assert-State ($toggle.IsChecked -eq $expanded) 'Safe-section toggle must track its expanded state.'
    }
    # Use the actual ToggleButton command path; a routed Click alone does not toggle IsChecked.
    $toggle.GetType().GetMethod('OnClick', [Reflection.BindingFlags]'Instance,NonPublic').Invoke($toggle, @())
    Assert-State $section.IsExpanded 'Safe-section button must expand the section.'
    $toggle.GetType().GetMethod('OnClick', [Reflection.BindingFlags]'Instance,NonPublic').Invoke($toggle, @())
    Assert-State (-not $section.IsExpanded) 'Safe-section button must collapse the section.'
    Invoke-Control ClickGroup @($first)
    Assert-State (-not $first.IsExpanded) 'Single click must leave a collapsed row collapsed.'
    Invoke-Control ToggleGroup @($first)
    Assert-State $first.IsExpanded 'Double click must expand its row.'
    Invoke-Control ClickGroup @($second)
    Assert-State ($first.IsExpanded -and -not $second.IsExpanded) 'Selecting another row must not change expansion.'
    Invoke-Control ToggleGroup @($second)
    Assert-State ($first.IsExpanded -and $second.IsExpanded) 'Double click must toggle only the chosen row.'
    Invoke-Control ClickGroup @($safe)
    Assert-State ($second.IsExpanded -and -not $safe.IsExpanded) 'Safe row clicks must also leave expansion unchanged.'
    Invoke-Control ToggleGroup @($safe)
    $control.FindName('GroupList').SelectedItem = $first
    Assert-State ($safe.IsExpanded -and $first.IsExpanded) 'Keyboard-style selection must preserve open details.'
    Invoke-Control ClickGroup @($first)
    Invoke-Control ClickGroup @($first)
    Assert-State $first.IsExpanded 'Repeated single clicks must leave an open row open.'
    Invoke-Control ToggleGroup @($first)
    Assert-State (-not $first.IsExpanded) 'Double clicking an open row must collapse it.'
    Invoke-Control ToggleGroup @($first)
    Invoke-Control ClickGroup @($safe)
    Invoke-Control ShowResult @('Test', 'Nothing executes', $groups)
    Assert-State ($first.IsExpanded -and $second.IsExpanded -and $safe.IsExpanded) 'Reanalysis must preserve every expanded object.'
    Invoke-Control Select @($second)
    Assert-State $safe.IsExpanded 'Programmatic selection must preserve previous details.'
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
    Assert-State $second.IsExpanded 'A routed pane-background click must preserve expanded details.'
    Assert-State ($null -eq $control.FindName('GroupList').SelectedItem -and $null -eq $control.FindName('SafeList').SelectedItem) 'A pane-background click should clear both list selections.'
    Invoke-Control ClickGroup @($safe)
    Invoke-Control Select @($null)
    Assert-State ($safe.IsExpanded -and $null -eq $control.FindName('SafeList').SelectedItem) 'Editor deselection must clear selection while keeping details open.'
    Write-Output "Passed $script:checks pane interaction checks; no SSMS interaction or SQL execution."
}
finally {
    [AppDomain]::CurrentDomain.remove_AssemblyResolve($resolver)
}
