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
    Write-Output 'Passed 4 settings editor checks (shell rendering excluded); no desktop input or SQL execution.'
}
finally {
    [AppDomain]::CurrentDomain.remove_AssemblyResolve($resolver)
}
