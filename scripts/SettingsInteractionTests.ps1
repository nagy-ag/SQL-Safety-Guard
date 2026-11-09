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
    $dialogType = $assembly.GetType('SqlSafetyGuard.UI.SettingsListDialog', $true)
    # Build the real layout without shell-only theme resources. The live SSMS
    # check separately covers the themed dialog and command integration.
    $xaml = Get-Content -Raw -Encoding UTF8 (Join-Path $repository 'src/SqlSafetyGuard.Vsix/UI/SettingsListDialog.xaml')
    $xaml = $xaml.Replace('platformUi:DialogWindow', 'UserControl') -replace ' x:Class="[^"]+"', ''
    $xaml = $xaml -replace ' Click="[^"]+"', '' -replace ' [A-Za-z]+="{DynamicResource[^"\r\n]+}"', ''
    [xml] $markup = $xaml
    foreach ($attribute in @($markup.DocumentElement.Attributes)) {
        if ($attribute.Prefix -ne 'xmlns' -and $attribute.Name -ne 'xmlns') { [void] $markup.DocumentElement.RemoveAttributeNode($attribute) }
    }
    $layout = [Windows.Markup.XamlReader]::Parse($markup.OuterXml)
    $dialog = [Runtime.Serialization.FormatterServices]::GetUninitializedObject($dialogType)
    foreach ($name in @('Entries', 'Error')) {
        $dialogType.GetField($name, [Reflection.BindingFlags]'Instance,NonPublic').SetValue($dialog, $layout.FindName($name))
    }
    $entries = $layout.FindName('Entries')
    $entries.Text = "dbo.First`r`ndbo.Second`r`ndbo.Third"
    if (-not $entries.AcceptsReturn -or $entries.AcceptsTab) { throw 'Enter must insert a line; Tab must navigate.' }
    $buttons = $layout.Content.Children[3].Children
    if (-not $buttons[1].IsDefault -or -not $buttons[1].IsCancel) { throw 'Cancel must remain the default and escape action.' }
    $dialogType.GetProperty('Save', [Reflection.BindingFlags]'Instance,NonPublic').SetValue($dialog,
        [Action[string]] { param($text) throw 'Invalid test entries' })
    $dialogType.GetMethod('OnSave', [Reflection.BindingFlags]'Instance,NonPublic').Invoke($dialog,
        [object[]]@($null, [Windows.RoutedEventArgs]::new()))
    if ($layout.FindName('Error').Visibility -ne 'Visible' -or $layout.FindName('Error').Text -notlike '*Invalid test entries*') { throw 'A validation error must stay visible without dismissing the editor.' }
    if ($entries.Text -notlike '*dbo.Third*') { throw 'Invalid input must remain available for correction.' }

    # Exercise the actual compact control with the real option model and injected
    # storage/command callbacks. No shell storage or installer is invoked here.
    if (-not [Windows.Application]::Current) { $application = [Windows.Application]::new() }
    $shell = [Reflection.Assembly]::Load('Microsoft.VisualStudio.Shell.15.0')
    $keys = $shell.GetType('Microsoft.VisualStudio.Shell.VsResourceKeys', $true)
    $buttonKey = $keys.GetProperty('ThemedDialogButtonStyleKey').GetValue($null)
    [Windows.Application]::Current.Resources[$buttonKey] = [Windows.Style]::new([Windows.Controls.Button])
    $pageType = $assembly.GetType('SqlSafetyGuard.Options.GeneralOptionsPage', $true)
    # DialogPage's constructor requires a running VS shell. Its six ordinary
    # backing properties can be exercised without constructing that shell.
    $page = [Runtime.Serialization.FormatterServices]::GetUninitializedObject($pageType)
    function Set-Option($name, $value) { $pageType.GetProperty($name).SetValue($page, $value) }
    function Get-Option($name) { $pageType.GetProperty($name).GetValue($page) }
    Set-Option 'GuardMode' ([Enum]::ToObject($assembly.GetType('SqlSafetyGuard.Options.GuardMode'), 2))
    Set-Option 'ShowServerAndDatabase' $true
    Set-Option 'AllowList' ''
    Set-Option 'TrustedProcedures' ''
    Set-Option 'EnvironmentRules' ''
    $controlType = $assembly.GetType('SqlSafetyGuard.UI.CompactSettingsControl', $true)
    $script:commits = 0
    $script:commands = [Collections.Generic.List[int]]::new()
    $script:failCommit = $false
    $control = [Activator]::CreateInstance($controlType, [Reflection.BindingFlags]'Instance,NonPublic', $null,
        [object[]]@($page, [Action] { if ($script:failCommit) { throw 'Storage unavailable' }; $script:commits++ },
            [Action[int]] { param($id) $script:commands.Add($id) }), $null)
    $control.RaiseEvent([Windows.RoutedEventArgs]::new([Windows.FrameworkElement]::LoadedEvent))
    $hostPanel = [Windows.Controls.StackPanel]::new()
    [void]$hostPanel.Children.Add($control)
    $hostPanel.AddHandler([Windows.Input.Keyboard]::PreviewGotKeyboardFocusEvent,
        [Windows.Input.KeyboardFocusChangedEventHandler] { param($sender, $eventArgs) $eventArgs.Handled = $true })
    $focus = [Windows.Input.KeyboardFocusChangedEventArgs]::new([Windows.Input.Keyboard]::PrimaryDevice, 0, $null, $control.FindName('EditAllowList'))
    $focus.RoutedEvent = [Windows.Input.Keyboard]::PreviewGotKeyboardFocusEvent
    $control.FindName('EditAllowList').RaiseEvent($focus)
    if ($focus.Handled) { throw 'The editable form must allow keyboard focus inside the native content host.' }
    if ($script:commits -ne 0) { throw 'Opening/refreshing settings must not persist anything.' }
    if ($control.FindName('WarningMode').SelectedIndex -ne 2 -or -not $control.FindName('ShowConnection').IsChecked -or $control.FindName('TempdbIsTemp').IsChecked) { throw 'Existing defaults changed.' }
    function Summary($name) { $control.FindName($name).Text }
    if ((Summary 'AllowListSummary') -ne 'None' -or (Summary 'TrustedSummary') -ne 'None' -or (Summary 'RulesSummary') -ne 'None') { throw 'Empty lists must read None.' }
    $control.FindName('WarningMode').SelectedIndex = 1
    $control.FindName('ShowConnection').IsChecked = $false
    $control.FindName('TempdbIsTemp').IsChecked = $true
    if ($script:commits -ne 3 -or [int](Get-Option 'GuardMode') -ne 1 -or (Get-Option 'ShowServerAndDatabase') -or -not (Get-Option 'TempdbIsTemp')) {
        throw 'Every control must immediately update and persist its corresponding existing setting.' }
    Set-Option 'AllowList' "-- note`r`ndbo.First`r`ndbo.Second"
    Set-Option 'TrustedProcedures' 'usp_NoSchema'
    Set-Option 'EnvironmentRules' "TEST-* | Sandbox | Test | YellowRed | false`r`nnot a rule"
    if ((Summary 'AllowListSummary') -ne '2 patterns' -or $control.FindName('AllowListProblem').Visibility -ne 'Collapsed' -or $script:commits -ne 3) {
        throw 'A list update must refresh its count without saving it again.' }
    if ((Summary 'TrustedSummary') -ne '1 procedure' -or $control.FindName('TrustedProblem').Visibility -ne 'Visible') { throw 'An invalid procedure must be counted and flagged.' }
    if ((Summary 'RulesSummary') -ne '2 rules' -or $control.FindName('RulesProblem').Text -notlike '*1 rule has a problem*') { throw 'An unreadable rule must be counted and flagged.' }
    $script:failCommit = $true
    $control.FindName('WarningMode').SelectedIndex = 0
    if ([int](Get-Option 'GuardMode') -ne 1 -or $control.FindName('WarningMode').SelectedIndex -ne 1 -or
        $control.FindName('SaveError').Visibility -ne 'Visible') { throw 'A failed save must restore the previous value and expose the error.' }
    $script:failCommit = $false
    $control.FindName('ShowConnection').IsChecked = $true
    if ($control.FindName('SaveError').Visibility -ne 'Collapsed') { throw 'A successful edit must clear a previous save error.' }
    $markup = [xml](Get-Content -Raw -Encoding UTF8 (Join-Path $repository 'src/SqlSafetyGuard.Vsix/UI/CompactSettingsControl.xaml'))
    $ns = [Xml.XmlNamespaceManager]::new($markup.NameTable)
    $ns.AddNamespace('p', 'http://schemas.microsoft.com/winfx/2006/xaml/presentation')
    foreach ($button in $markup.SelectNodes('//p:Button[@Tag]', $ns)) {
        $source = [Windows.Controls.Button]::new()
        $source.Tag = [string]$button.Tag
        $controlType.GetMethod('OnCommand', [Reflection.BindingFlags]'Instance,NonPublic').Invoke($control,
            [object[]]@($source, [Windows.RoutedEventArgs]::new()))
    }
    if (($script:commands -join ',') -ne '261,262,263,258,257,264') { throw 'The compact page must invoke the six existing editor/transfer/uninstall commands.' }
    $control.RaiseEvent([Windows.RoutedEventArgs]::new([Windows.FrameworkElement]::UnloadedEvent))
    Set-Option 'AllowList' 'dbo.WhileClosed'
    if ((Summary 'AllowListSummary') -ne '2 patterns') { throw 'An unloaded page must release its settings subscription.' }
    $control.RaiseEvent([Windows.RoutedEventArgs]::new([Windows.FrameworkElement]::LoadedEvent))
    if ((Summary 'AllowListSummary') -ne '1 pattern') { throw 'Reopening must load the current settings.' }
    $control.RaiseEvent([Windows.RoutedEventArgs]::new([Windows.FrameworkElement]::UnloadedEvent))

    # The structured editors need SSMS's theme service; ThemeAcceptanceCapture -CheckCompactSettings drives them in SSMS.
    Write-Output 'Passed 14 settings checks (4 text editor, 10 compact page; shell rendering excluded); no desktop input or SQL execution.'
}
finally {
    [AppDomain]::CurrentDomain.remove_AssemblyResolve($resolver)
}
