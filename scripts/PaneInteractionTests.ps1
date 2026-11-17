param([string] $Configuration = 'Debug')

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName PresentationFramework
$repository = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$output = Join-Path $repository "src\SqlSafetyGuard.Vsix\bin\$Configuration"
$ssms = 'C:\Program Files\Microsoft SQL Server Management Studio 22\Release\Common7\IDE'
$script:probePaths = @($output, (Join-Path $ssms 'PublicAssemblies'), (Join-Path $ssms 'PrivateAssemblies'), (Join-Path $ssms 'CommonExtensions/Microsoft/Editor'), $ssms)
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
        $value = [SqlSafetyGuard.Analyzer.Finding]::new(1, 1, 0, 8, 'UPDATE', $Key, $null, $Verdict, 'Test finding', $true, $null, $true)
        $findingType.GetField('<Finding>k__BackingField', [Reflection.BindingFlags]'Instance,NonPublic').SetValue($finding, $value)
        $findings = [Array]::CreateInstance($findingType, 1)
        $findings.SetValue($finding, 0)
        return [Activator]::CreateInstance($groupType, [object[]]@($Key, $findings))
    }

    $context = New-Group 'Sandbox' ReadOnly
    $contextFinding = [SqlSafetyGuard.Analyzer.Finding]::new(1,1,0,12,'USE','Sandbox',$null,[SqlSafetyGuard.Analyzer.Verdict]::ReadOnly,'Context')
    $findingType.GetField('<Finding>k__BackingField', [Reflection.BindingFlags]'Instance,NonPublic').SetValue($context.Findings[0], $contextFinding)
    Assert-State ($context.Findings[0].ContextVisibility -eq [Windows.Visibility]::Visible) 'USE must expose a context accent without changing its green verdict.'
    Assert-State ($context.ContextVisibility -eq [Windows.Visibility]::Visible) 'The object row must preserve the context accent.'
    $first = New-Group 'dbo.First' RealWrite
    $second = New-Group 'dbo.Second' RealWrite
    $safe = New-Group '#Temporary' TempOnly
    Assert-State (-not $safe.Findings[0].IsPermanentWholeTarget) 'Temporary-only operations must not carry a permanent-write risk label.'
    Assert-State ($first.Findings[0].Reason -like 'No WHERE clause:*') 'A permanent whole-target operation must retain its warning.'
    $union = $assembly.GetType('SqlSafetyGuard.UI.HighlightGeometry', $true).GetMethod('Union')
    $rectangles = [Windows.Rect[]]@([Windows.Rect]::new(0,0,100,20), [Windows.Rect]::new(0,20,80,20))
    $geometry = $union.Invoke($null, [object[]]@(,$rectangles))
    $outline = $geometry.GetWidenedPathGeometry([Windows.Media.Pen]::new([Windows.Media.Brushes]::Red, 1))
    Assert-State ([Math]::Abs($geometry.GetArea() - 3600) -lt 0.1) "Joined rows must retain the selected area ($($geometry.GetArea()))."
    Assert-State (-not $outline.FillContains([Windows.Point]::new(50,20))) 'Adjacent rows must have no internal outline.'
    Assert-State ($outline.FillContains([Windows.Point]::new(0,20))) 'The continuous outer edge must remain outlined.'
    $separated = [Windows.Rect[]]@([Windows.Rect]::new(0,0,100,20), [Windows.Rect]::new(0,40,100,20))
    $separateGeometry = $union.Invoke($null, [object[]]@(,$separated))
    Assert-State (-not $separateGeometry.FillContains([Windows.Point]::new(50,30))) 'Unselected rows between ranges must stay clear.'
    $groups = [Array]::CreateInstance($groupType, 3)
    $groups.SetValue($first, 0)
    $groups.SetValue($second, 1)
    $groups.SetValue($safe, 2)
    Invoke-Control ShowResult @('Test', 'Nothing executes', $groups)
    $list = $control.FindName('GroupList')
    Assert-State ($list.Items.Count -eq 3 -and $null -eq $control.FindName('SafeList')) 'All verdicts must share one object list.'
    Assert-State ($null -eq $control.FindName('FilterAll')) 'The pane must have no All button.'
    Assert-State ($null -eq $control.FindName('HeaderCounts')) 'The expanded header must have no color badges.'
    $badges = $control.FindName('CollapsedCounts')
    Assert-State ($badges.Children.Count -eq 2) 'Only present categories must appear in the collapsed strip.'
    $badgeNumbers = @($badges.Children | ForEach-Object { $_.Children[1].Child.Text })
    Assert-State ($badgeNumbers[0] -eq '2' -and $badgeNumbers[1] -eq '1') 'Collapsed red and green circles must show operation counts.'
    Assert-State ($null -eq $control.FindName('CollapsedGlyph')) 'The collapsed pane must not use a green checkmark.'
    # Six independent toggles. Actual OnClick toggles IsChecked before the routed handler.
    foreach ($choice in @(@('FilterWrites', 1, $false), @('FilterSafe', 0, $false), @('FilterWrites', 2, $true), @('FilterSafe', 3, $true))) {
        $toggle = $control.FindName($choice[0])
        $toggle.GetType().GetMethod('OnClick', [Reflection.BindingFlags]'Instance,NonPublic').Invoke($toggle, @())
        Assert-State ($list.Items.Count -eq $choice[1]) 'Independent risk toggles must combine without resetting the other choices.'
        Assert-State ([bool]$toggle.IsChecked -eq $choice[2]) 'A second click must turn the same filter back on.'
        Assert-State ((Invoke-Control FilterCount @([Enum]::Parse($assembly.GetType('SqlSafetyGuard.UI.PaneVerdictFilter'), 'Writes'))) -eq 2) 'Filters must preserve the overall warning count.'
        Assert-State (($control.FindName('EmptyText').Visibility -eq [Windows.Visibility]::Visible) -eq ($choice[1] -eq 0)) 'An empty filter must have a visible explanation.'
    }
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
    $control.FindName('FilterWrites').GetType().GetMethod('OnClick', [Reflection.BindingFlags]'Instance,NonPublic').Invoke($control.FindName('FilterWrites'), @())
    $control.FindName('FilterWrites').GetType().GetMethod('OnClick', [Reflection.BindingFlags]'Instance,NonPublic').Invoke($control.FindName('FilterWrites'), @())
    Assert-State ($first.IsExpanded -and $second.IsExpanded -and $safe.IsExpanded) 'Filtering away and back must preserve every expanded object.'
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
    Assert-State ($null -eq $list.SelectedItem) 'A pane-background click should clear the object selection.'
    Invoke-Control ClickGroup @($safe)
    Invoke-Control Select @($null)
    Assert-State ($safe.IsExpanded -and $null -eq $list.SelectedItem) 'Editor deselection must clear selection while keeping details open.'
    Invoke-Control ShowPlaceholder @('Test', 'Layout only')
    $many = [Array]::CreateInstance($findingType, 12345)
    for ($i = 0; $i -lt $many.Length; $i++) { $many.SetValue($first.Findings[0], $i) }
    $large = [Activator]::CreateInstance($groupType, [object[]]@('dbo.Large', $many))
    $largeGroups = [Array]::CreateInstance($groupType, 1); $largeGroups.SetValue($large, 0)
    Invoke-Control ShowResult @('Test', 'Nothing executes', $largeGroups)
    Assert-State ($badges.Children.Count -eq 1 -and $badges.Children[0].Children[1].Child.Text -eq '12345') 'Large counts must remain intact inside a circle.'
    $minimum = $control.GetType().GetProperty('MinimumExpandedWidth', [Reflection.BindingFlags]'Instance,NonPublic').GetValue($control)
    $control.Width = $minimum
    $control.Measure([Windows.Size]::new($minimum, 1000))
    $control.Arrange([Windows.Rect]::new(0, 0, $minimum, 1000))
    $control.UpdateLayout()
    $filterButtons = @('FilterWrites','FilterUnknown','FilterSafe','FilterInside','FilterContext','FilterRemote') | ForEach-Object { $control.FindName($_) }
    $origins = @($filterButtons | ForEach-Object { $_.TranslatePoint([Windows.Point]::new(0, 0), $control) })
    Assert-State (($origins | Where-Object { [Math]::Abs($_.Y - $origins[0].Y) -ge 1 }).Count -eq 0) 'Large counts must retain one filter row at minimum width.'
    Assert-State ($origins[5].X + $filterButtons[5].ActualWidth -le $minimum) "Minimum width must include the entire last filter (minimum=$minimum; lastX=$($origins[5].X); lastWidth=$($filterButtons[5].ActualWidth))."
    $compact = $control.GetType().GetProperty('CompactFilterLabels', [Reflection.BindingFlags]'Instance,NonPublic')
    Assert-State $compact.GetValue($control) 'Automatic labels must use circles at the compact minimum width.'
    $control.Width = 1000
    $control.Measure([Windows.Size]::new(1000,1000)); $control.Arrange([Windows.Rect]::new(0,0,1000,1000)); $control.UpdateLayout()
    Assert-State (-not $compact.GetValue($control)) 'Automatic labels must restore text when enough space is available.'
    $labelsType = $assembly.GetType('SqlSafetyGuard.UI.PaneFilterLabels', $true)
    Invoke-Control SetFilterLabels @([Enum]::Parse($labelsType,'Circles'))
    Assert-State $compact.GetValue($control) 'Forced circles must stay compact at a wide width.'
    Invoke-Control SetFilterLabels @([Enum]::Parse($labelsType,'Text'))
    Assert-State (-not $compact.GetValue($control)) 'Forced text must stay labelled.'
    Invoke-Control SetFilterLabels @([Enum]::Parse($labelsType,'Automatic'))
    Assert-State ($control.FindName('FilterWrites').IsChecked -and $control.FindName('FilterInside').IsChecked) 'Changing labels must preserve selected filters.'
    Assert-State ($null -eq $control.FindName('FilterSummary')) 'Filters must have no explanatory text row beneath them.'
    $lineField = $findingType.GetField('<EditorLine>k__BackingField', [Reflection.BindingFlags]'Instance,NonPublic')
    $lineField.SetValue($first.Findings[0], 1000)
    $smallGroups = [Array]::CreateInstance($groupType, 1); $smallGroups.SetValue($first, 0)
    Invoke-Control ShowResult @('Test', 'Nothing executes', $smallGroups)
    $wideGutter = $control.LineGutterWidth.Value
    Assert-State ($wideGutter -gt 30 -and $wideGutter -lt 50) 'Four-digit lines must use their measured width plus spacing.'
    $filterType = $assembly.GetType('SqlSafetyGuard.UI.PaneVerdictFilter', $true)
    Invoke-Control SetFilter @([Enum]::Parse($filterType,'Safe'))
    Assert-State ($control.LineGutterWidth.Value -eq $wideGutter) 'Hiding the longest row must not move the gutter.'
    $lineField.SetValue($first.Findings[0], 1)
    # Synthetic rows have no tracking spans; reset before replacing the same key.
    Invoke-Control ShowPlaceholder @('Test', 'Layout only')
    Invoke-Control ShowResult @('Test', 'Nothing executes', $smallGroups)
    Assert-State ($control.LineGutterWidth.Value -lt $wideGutter - 10) 'A new short analysis must release unused gutter width.'
    $contextGroups = [Array]::CreateInstance($groupType, 1); $contextGroups.SetValue($context, 0)
    Invoke-Control ShowResult @('Test', 'Nothing executes', $contextGroups)
    Invoke-Control SetFilter @([Enum]::Parse($filterType,'Context'))
    Assert-State ($list.Items.Count -eq 1 -and $context.DisplayCounts.Context -eq 1 -and $context.DisplayCounts.Safe -eq 0) 'Context must be independently visible and counted once.'
    Invoke-Control SetFilter @([Enum]::Parse($filterType,'Safe'))
    Assert-State ($list.Items.Count -eq 0) 'Safe alone must hide context operations.'
    Invoke-Control ShowAnalysisPending @()
    Assert-State ($badges.Children.Count -eq 0 -and $badges.Visibility -eq [Windows.Visibility]::Collapsed) 'Pending analysis must remove stale colored totals.'
    Invoke-Control ShowPlaceholder @('Waiting', 'No analyzed SQL')
    Invoke-Control SetFilter @([Enum]::Parse($filterType,'All'))
    Assert-State ($badges.Children.Count -eq 0) 'Changing filters must not restore stale counts during a placeholder.'
    $safeGroups = [Array]::CreateInstance($groupType, 1); $safeGroups.SetValue($safe, 0)
    Invoke-Control ShowResult @('Safe', 'One temporary operation', $safeGroups)
    Assert-State ($badges.Children.Count -eq 1 -and $badges.Children[0].Tag.ToString() -eq 'Safe' -and $badges.Children[0].Children[1].Child.Text -eq '1') 'A safe-only result must show a numbered green circle.'
    $strip = $control.FindName('CollapsedStrip')
    $collapsedEvent = [Windows.Input.MouseButtonEventArgs]::new([Windows.Input.Mouse]::PrimaryDevice, 0, [Windows.Input.MouseButton]::Left)
    $collapsedEvent.RoutedEvent = [Windows.UIElement]::MouseLeftButtonUpEvent
    $script:collapseEvents = 0
    $collapseHandler = [Action] { $script:collapseEvents++ }
    $control.GetType().GetEvent('CollapseToggled', [Reflection.BindingFlags]'Instance,NonPublic').GetAddMethod($true).Invoke($control, @($collapseHandler))
    $strip.RaiseEvent($collapsedEvent)
    Assert-State ($script:collapseEvents -eq 1) 'The entire collapsed strip must remain one expand target.'
    # A single expanded object can be taller than the viewport. Wheel input over its
    # children must move within it, rather than treating the whole object as one step.
    Invoke-Control ShowPlaceholder @('Test', 'Scrolling regression')
    $scrollFindings = [Array]::CreateInstance($findingType, 80)
    for ($i = 0; $i -lt $scrollFindings.Length; $i++) {
        $scrollFindings.SetValue((New-Group ("dbo.Scroll$i") RealWrite).Findings[0], $i)
    }
    $scrollGroup = [Activator]::CreateInstance($groupType, [object[]]@('dbo.TallObject', $scrollFindings))
    $scrollGroup.IsExpanded = $true
    $scrollGroups = [Array]::CreateInstance($groupType, 1); $scrollGroups.SetValue($scrollGroup, 0)
    Invoke-Control ShowResult @('Test', 'Nothing executes', $scrollGroups)
    $control.Width = 400; $control.Height = 300
    $control.Measure([Windows.Size]::new(400,300)); $control.Arrange([Windows.Rect]::new(0,0,400,300)); $control.UpdateLayout()
    $visuals = [Collections.Generic.List[Windows.DependencyObject]]::new()
    $pending = [Collections.Generic.Queue[Windows.DependencyObject]]::new(); $pending.Enqueue($list)
    while ($pending.Count) {
        $visual = $pending.Dequeue(); $visuals.Add($visual)
        for ($i = 0; $i -lt [Windows.Media.VisualTreeHelper]::GetChildrenCount($visual); $i++) {
            $pending.Enqueue([Windows.Media.VisualTreeHelper]::GetChild($visual,$i))
        }
    }
    $scroller = @($visuals | Where-Object { $_ -is [Windows.Controls.ScrollViewer] })[0]
    $headers = @($visuals | Where-Object { $_ -is [Windows.Controls.Button] -and $_.Name -eq 'RowHeader' -and $_.DataContext -is $findingType })
    Assert-State ($headers.Count -eq 80 -and $scroller.ScrollableHeight -gt 0) 'A tall expanded object must expose its complete content to the pane scroller.'
    $header = $headers[0]; $initialY = $header.TranslatePoint([Windows.Point]::new(0,0),$list).Y
    $wheel = [Windows.Input.MouseWheelEventArgs]::new([Windows.Input.Mouse]::PrimaryDevice,0,-120)
    $wheel.RoutedEvent = [Windows.UIElement]::PreviewMouseWheelEvent; $header.RaiseEvent($wheel)
    $wheel.RoutedEvent = [Windows.UIElement]::MouseWheelEvent; $header.RaiseEvent($wheel)
    $control.UpdateLayout()
    $distance = $initialY - $header.TranslatePoint([Windows.Point]::new(0,0),$list).Y
    Assert-State ($wheel.Handled -and $distance -gt 0 -and $distance -lt $list.ActualHeight) 'Wheel input over a nested operation must move less than a page within its object.'
    Assert-State ($null -eq $list.SelectedItem -and $scrollGroup.IsExpanded) 'Scrolling must preserve expansion without selecting or navigating a row.'
    $scroller.ScrollToEnd(); $control.UpdateLayout()
    $last = $headers[-1]; $lastY = $last.TranslatePoint([Windows.Point]::new(0,0),$list).Y
    Assert-State ($lastY -ge 0 -and $lastY + $last.ActualHeight -le $list.ActualHeight + 1) 'The final operation of an oversized object must be fully reachable.'
    $scroller.ScrollToHome(); $control.UpdateLayout()
    Assert-State ([Math]::Abs($header.TranslatePoint([Windows.Point]::new(0,0),$list).Y - $initialY) -lt 1) 'Scrolling back must restore the first operation.'
    Write-Output "Passed $script:checks pane interaction checks; no SSMS interaction or SQL execution."
}
finally {
    [AppDomain]::CurrentDomain.remove_AssemblyResolve($resolver)
}
