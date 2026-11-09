#if DEBUG
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.VisualStudio.Shell;
using SqlSafetyGuard.Analyzer;
using SqlSafetyGuard.Options;
using SqlSafetyGuard.Services;

namespace SqlSafetyGuard.UI
{
    // Entire file is absent from Release. These previews have no connection to ExecuteGuard's continuation.
    internal sealed partial class SafetyPaneMargin
    {
        private static readonly Dictionary<string, Window> CaptureDialogs = new Dictionary<string, Window>();

        private void RunCaptureCommand(string verb, string arg)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQLSAFETYGUARD_TEST_DIRECTORY")))
                throw new InvalidOperationException("Capture commands require a private test directory.");
            switch (verb)
            {
                case "showconfirm":
                    var package = TryGetPackage();
                    if (!RefreshSettings()) throw new InvalidOperationException("Pane settings are not ready.");
                    var buffer = _view.TextSnapshot;
                    var span = CurrentSelection() ?? new Microsoft.VisualStudio.Text.Span(0, buffer.Length);
                    var start = buffer.GetLineFromPosition(span.Start);
                    var snapshot = new QuerySnapshot(null, buffer.GetText(span), span.Length != buffer.Length,
                        start.LineNumber + 1, span.Start - start.Start.Position + 1);
                    // Capture the real target diagnostic without activating an editor or dispatching a command.
                    var target = ExecutionTarget.Capture(package, package.Dte);
                    var result = SqlSafetyAnalyzer.Analyze(snapshot.Text, _options);
                    ShowCaptureDialog("confirm", new ConfirmExecutionDialog(result, snapshot, _connection, _settings,
                        package.Options.ShowServerAndDatabase, target.Warning));
                    ExtensionCheckTrace.Write("preview-confirm", result.Overall.ToString(), ExtensionCheckTrace.Hash(snapshot.Text),
                        _connection.Description, target.Warning);
                    break;
                case "showfailure":
                    ShowCaptureDialog("failure", new GuardFailureDialog(new InvalidOperationException("Theme acceptance sample.")));
                    break;
                case "showlisteditor":
                    var current = Commands.SettingsCommands.Capture(TryGetPackage().Options);
                    string title, description, entries;
                    switch (arg)
                    {
                        case "allowList":
                            title = "Safe tables allow-list";
                            description = Commands.SettingsCommands.AllowListDescription;
                            entries = current.AllowList;
                            break;
                        case "trustedProcedures":
                            title = "Trusted procedures";
                            description = Commands.SettingsCommands.TrustedProceduresDescription;
                            entries = current.TrustedProcedures;
                            break;
                        case "environmentRules":
                            title = "Environment rules";
                            description = "";
                            entries = current.EnvironmentRules;
                            break;
                        default: throw new ArgumentException("Expected allowList, trustedProcedures or environmentRules.");
                    }
                    var editor = Commands.SettingsCommands.CreateEditor(arg, title, description, entries, text => { });
                    ShowCaptureDialog(arg, editor);
                    break;
                case "showimportpreview":
                    if (!arg.EndsWith(".ssgsettings.json", StringComparison.OrdinalIgnoreCase))
                        throw new ArgumentException("Expected a .ssgsettings.json file.");
                    if (new FileInfo(arg).Length > SettingsTransfer.MaximumFileCharacters * 4L)
                        throw new FormatException("Settings file exceeds the size limit.");
                    var import = SettingsTransfer.PrepareImport(File.ReadAllText(arg, new UTF8Encoding(false, true)),
                        Commands.SettingsCommands.Capture(TryGetPackage().Options));
                    var preview = new SettingsImportDialog(import);
                    // The preview retains the production validation/diff but cannot apply it.
                    ShowCaptureDialog("importpreview", preview);
                    ExtensionCheckTrace.Write("preview-import", import.Changes.Count.ToString(), import.Warnings.Count.ToString());
                    break;
                case "showoptions":
                    TryGetPackage().ShowOptionPage(typeof(GeneralOptionsPage));
                    WaitForCaptureOptions();
                    break;
                case "compactsettingscheck":
                    CheckCompactSettings();
                    break;
                case "compactsettingsscroll":
                    CheckCompactSettingsScroll();
                    break;
                case "capture":
                    int separator = arg.IndexOf(' ');
                    if (separator < 1) throw new ArgumentException("Expected surface and absolute PNG path.");
                    string surface = arg.Substring(0, separator), path = arg.Substring(separator + 1).Trim('"');
                    if (!Path.IsPathRooted(path) || !path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                        throw new ArgumentException("Expected an absolute PNG path.");
                    CaptureAtIdle(() => CaptureSurface(surface, path));
                    break;
                case "closedialogs":
                    foreach (var dialog in CaptureDialogs.Values.ToArray()) dialog.Close();
                    CaptureDialogs.Clear();
                    ExtensionCheckTrace.Write("closedialogs");
                    break;
                case "preparecaptureclose":
                    foreach (EnvDTE.Document document in TryGetPackage().Dte.Documents)
                        if (string.Equals(document.FullName, _documentPath, StringComparison.OrdinalIgnoreCase))
                            document.Saved = true; // This scratch buffer only; caller sends WM_CLOSE to its own PID.
                    break;
            }
        }

        private static void WaitForCaptureOptions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var deadline = DateTime.UtcNow.AddSeconds(30);
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            timer.Tick += (sender, args) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                bool ready = PresentationSource.CurrentSources.OfType<HwndSource>()
                    .Where(source => source.RootVisual != null)
                    .SelectMany(source => CapturePreview.VisualDescendants(source.RootVisual))
                    .OfType<TextBlock>().Any(text => text.IsVisible && text.Text == "F5 warnings");
                if (ready)
                {
                    timer.Stop();
                    CaptureAtIdle(() => ExtensionCheckTrace.Write("shown", "options", "F5 warnings"));
                }
                else if (DateTime.UtcNow > deadline)
                {
                    timer.Stop();
                    ExtensionCheckTrace.Write("capture-error", "The integrated SQL Safety Guard options page did not finish loading.");
                }
            };
            timer.Start();
        }

        private void CheckCompactSettings()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var control = PresentationSource.CurrentSources.OfType<HwndSource>()
                .Where(source => source.RootVisual != null)
                .SelectMany(source => CapturePreview.VisualDescendants(source.RootVisual))
                .OfType<CompactSettingsControl>().First(value => value.IsVisible);
            var page = TryGetPackage().Options;
            var original = Commands.SettingsCommands.Capture(page);
            int checks = 0;
            try
            {
                // Route focus events through the real host without desktop input.
                foreach (string name in new[] { "WarningMode", "ShowConnection", "EditAllowList", "EditTrustedProcedures", "EditEnvironmentRules", "TempdbIsTemp" })
                {
                    var input = (UIElement)control.FindName(name);
                    var focus = new System.Windows.Input.KeyboardFocusChangedEventArgs(System.Windows.Input.Keyboard.PrimaryDevice,
                        Environment.TickCount, null, input) { RoutedEvent = System.Windows.Input.Keyboard.PreviewGotKeyboardFocusEvent };
                    input.RaiseEvent(focus);
                    if (focus.Handled) throw new InvalidOperationException("The settings host blocked keyboard focus for " + name);
                    checks++;
                }
                ((ComboBox)control.FindName("WarningMode")).SelectedIndex = original.GuardMode == ExecutionMode.Red ? 2 : 1;
                ((CheckBox)control.FindName("ShowConnection")).IsChecked = !original.ShowServerAndDatabase;
                ((CheckBox)control.FindName("TempdbIsTemp")).IsChecked = !original.TempdbIsTemp;
                // Lists change only through their editors, which store through the same option page.
                page.AllowList = "dbo.CompactSettingsProbe";
                page.TrustedProcedures = "dbo.CompactSettingsReader";
                page.EnvironmentRules = "PROBE-* | Sandbox | Probe | YellowRed | false";
                page.PersistChanges();
                string Summary(string name) => ((TextBlock)control.FindName(name)).Text;
                var manager = (Microsoft.VisualStudio.Utilities.UnifiedSettings.ISettingsManager)
                    Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(NativeSettingsService));
                var reader = manager.GetReader();
                Check(reader.GetValue<string>("sqlSafetyGuard.guardMode").Value == (original.GuardMode == ExecutionMode.Red ? "YellowRed" : "Red"), "mode persistence");
                Check(reader.GetValue<bool>("sqlSafetyGuard.showConnection").Value == !original.ShowServerAndDatabase, "connection persistence");
                Check(reader.GetValue<string>("sqlSafetyGuard.allowList").Value == "dbo.CompactSettingsProbe" &&
                    Summary("AllowListSummary") == "1 pattern", "allow-list persistence and summary");
                Check(reader.GetValue<string>("sqlSafetyGuard.trustedProcedures").Value == "dbo.CompactSettingsReader" &&
                    Summary("TrustedSummary") == "1 procedure", "trusted persistence and summary");
                Check(reader.GetValue<string>("sqlSafetyGuard.environmentRules").Value == "PROBE-* | Sandbox | Probe | YellowRed | false" &&
                    Summary("RulesSummary") == "1 rule", "rules persistence and summary");
                Check(reader.GetValue<bool>("sqlSafetyGuard.tempdbIsTemp").Value == !original.TempdbIsTemp, "tempdb persistence");
                ExtensionCheckTrace.Write("compact-settings", checks.ToString(), "PASS");
            }
            finally
            {
                page.GuardMode = (GuardMode)original.GuardMode;
                page.ShowServerAndDatabase = original.ShowServerAndDatabase;
                page.AllowList = original.AllowList;
                page.TrustedProcedures = original.TrustedProcedures;
                page.EnvironmentRules = original.EnvironmentRules;
                page.TempdbIsTemp = original.TempdbIsTemp;
                page.PersistChanges();
            }

            void Check(bool condition, string label)
            {
                if (!condition) throw new InvalidOperationException("Compact settings failed: " + label);
                checks++;
            }
        }

        /// <summary>
        /// Proves the integrated settings page scrolls under SQL Safety Guard's custom content, without the desktop
        /// mouse. Shrinks the page viewport to force overflow, raises in-process wheel input over each control,
        /// then waits for SSMS's sticky category header and hit-tests the viewport: only the scrolled content may
        /// be under the pointer. Restores the page size and offset afterwards.
        /// </summary>
        private static void CheckCompactSettingsScroll()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var invariant = System.Globalization.CultureInfo.InvariantCulture;
            var control = AllCompactSettings().First(value => value.IsVisible && Ancestor<ScrollViewer>(value) != null);
            var host = Ancestor<ScrollViewer>(control);
            var original = host.VerticalOffset;
            int checks = 0;
            try
            {
                // Simulate a short Options window so the page overflows, as on a small screen.
                host.Height = Math.Max(120, control.ActualHeight / 2);
                host.UpdateLayout();
                if (host.ScrollableHeight <= 0) throw new InvalidOperationException("The settings page did not overflow.");
                foreach (string name in new[] { "SettingsContent", "WarningMode", "ShowConnection", "AllowListBox", "TrustedBox", "RulesBox", "TempdbIsTemp" })
                {
                    var target = (UIElement)control.FindName(name);
                    host.ScrollToVerticalOffset(0);
                    host.UpdateLayout();
                    Wheel(target, -120);
                    host.UpdateLayout();
                    var down = host.VerticalOffset;
                    Wheel(target, 120);
                    host.UpdateLayout();
                    if (down <= 0 || host.VerticalOffset >= down) throw new InvalidOperationException("The settings page did not scroll over " + name);
                    checks++;
                }
                host.ScrollToVerticalOffset(host.ScrollableHeight / 2);
                host.UpdateLayout();
            }
            catch
            {
                Restore();
                throw;
            }

            // SSMS updates its sticky category header at Input priority after a scroll; let it settle first.
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(800) };
            timer.Tick += (sender, args) =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                timer.Stop();
                try
                {
                    foreach (var instance in AllCompactSettings())
                        ExtensionCheckTrace.Write("compact-scroll-instance", instance.IsVisible.ToString(), instance.ActualHeight.ToString("0", invariant),
                            ReferenceEquals(instance, control) ? "content" : instance.IsDescendantOf(host) ? "scrolled" : "overlay",
                            CompactSettingsControl.IsStickyHeaderCopy(instance) ? "sticky" : "normal");
                    var overlay = AllCompactSettings().FirstOrDefault(value => !value.IsDescendantOf(host) && value.IsVisible && value.ActualHeight > 0);
                    if (overlay != null) throw new InvalidOperationException("A sticky copy of the settings covers the scrolling page.");
                    checks++;
                    var root = (UIElement)PresentationSource.FromVisual(host).RootVisual;
                    foreach (double fraction in new[] { 0.25, 0.5, 0.75 })
                    {
                        var point = host.TranslatePoint(new System.Windows.Point(host.ViewportWidth / 2, host.ViewportHeight * fraction), root);
                        var hit = root.InputHitTest(point) as DependencyObject;
                        ExtensionCheckTrace.Write("compact-scroll-hit", fraction.ToString(invariant), hit?.GetType().Name ?? "(none)",
                            hit != null && hit is Visual visual && visual.IsDescendantOf(host) ? "content" : "outside");
                        if (!(hit is Visual found) || !found.IsDescendantOf(host))
                            throw new InvalidOperationException("Pointer input over the settings page does not reach its scrolling content.");
                        var before = host.VerticalOffset;
                        Wheel((UIElement)(hit as UIElement ?? Ancestor<UIElement>(hit)), 120);
                        host.UpdateLayout();
                        if (host.VerticalOffset >= before) throw new InvalidOperationException("The wheel did not scroll at the pointer position.");
                        Wheel((UIElement)(hit as UIElement ?? Ancestor<UIElement>(hit)), -120);
                        host.UpdateLayout();
                        checks++;
                    }
                    ExtensionCheckTrace.Write("compact-settings-scroll", checks.ToString(invariant), "PASS");
                }
                catch (Exception exception)
                {
                    ExtensionCheckTrace.Write("error", "compactsettingsscroll", exception.ToString());
                }
                finally { Restore(); }
            };
            timer.Start();

            void Restore()
            {
                host.ClearValue(FrameworkElement.HeightProperty);
                host.ScrollToVerticalOffset(original);
            }

            void Wheel(UIElement target, int delta)
            {
                // Same pairing the WPF input manager uses: tunnel, then bubble with shared Handled state.
                var args = new System.Windows.Input.MouseWheelEventArgs(System.Windows.Input.Mouse.PrimaryDevice, Environment.TickCount, delta)
                    { RoutedEvent = UIElement.PreviewMouseWheelEvent };
                target.RaiseEvent(args);
                args.RoutedEvent = UIElement.MouseWheelEvent;
                target.RaiseEvent(args);
            }
        }

        private static IEnumerable<CompactSettingsControl> AllCompactSettings() =>
            PresentationSource.CurrentSources.OfType<HwndSource>()
                .Where(source => source.RootVisual != null)
                .SelectMany(source => CapturePreview.VisualDescendants(source.RootVisual))
                .OfType<CompactSettingsControl>().ToList();

        private static T Ancestor<T>(DependencyObject node) where T : DependencyObject
        {
            for (node = node == null ? null : VisualTreeHelper.GetParent(node); node != null; node = VisualTreeHelper.GetParent(node))
                if (node is T match) return match;
            return null;
        }

        private static void ShowCaptureDialog(string surface, Window dialog)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (CaptureDialogs.TryGetValue(surface, out var previous)) previous.Close();
            CapturePreview.Prepare(dialog);
            new WindowInteropHelper(dialog).Owner = CaptureNative.MainWindow;
            CaptureDialogs[surface] = dialog;
            dialog.Closed += (sender, args) =>
            {
                if (CaptureDialogs.TryGetValue(surface, out var value) && ReferenceEquals(value, dialog)) CaptureDialogs.Remove(surface);
            };
            dialog.Show(); // Never ShowModal: no paused Execute command and no continuation to resume.
            CaptureAtIdle(() =>
            {
                ExtensionCheckTrace.Write("chrome", surface, DialogChrome.LastResult);
                ExtensionCheckTrace.Write("shown", surface, dialog.ActualWidth.ToString(), dialog.ActualHeight.ToString());
            });
        }

        private static void CaptureAtIdle(Action action)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            // Two dispatcher passes let Loaded, bindings, layout and render run before the receipt/image.
#pragma warning disable VSTHRD001 // Deliberate render-priority barrier, already on the UI thread; never blocks.
            _ = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
            {
                _ = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    try { action(); }
                    catch (Exception exception) { ExtensionCheckTrace.Write("capture-error", exception.ToString()); }
                }));
            }));
#pragma warning restore VSTHRD001
        }

        private void CaptureSurface(string surface, string path)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (surface == "main")
            {
                CaptureNative.CaptureMain(path);
                return;
            }
            FrameworkElement element = surface == "pane" ? _control :
                CaptureDialogs.TryGetValue(surface, out var dialog) ? dialog : null;
            if (element == null) throw new ArgumentException("Surface is not open: " + surface);
            element.UpdateLayout();
            // Window.ActualHeight includes native chrome that WPF cannot render. Capture its client root
            // at that root's actual size instead, avoiding an unpainted/transparent title-bar-sized strip.
            if (element is Window && VisualTreeHelper.GetChildrenCount(element) > 0)
                element = (FrameworkElement)VisualTreeHelper.GetChild(element, 0);
            if (element.ActualWidth <= 0 || element.ActualHeight <= 0) throw new InvalidOperationException("Surface has no layout.");
            var dpi = VisualTreeHelper.GetDpi(element);
            int width = (int)Math.Ceiling(element.ActualWidth * dpi.DpiScaleX);
            int height = (int)Math.Ceiling(element.ActualHeight * dpi.DpiScaleY);
            BitmapSource bitmap;
            if (surface == "pane")
            {
                // Rendering a margin in isolation preserves parent clipping/offsets. Render its actual
                // WPF host, then crop to the pane's transformed bounds so both edges remain intact.
                var root = (FrameworkElement)PresentationSource.FromVisual(element).RootVisual;
                root.UpdateLayout();
                var bounds = element.TransformToAncestor(root).TransformBounds(new System.Windows.Rect(element.RenderSize));
                var host = new RenderTargetBitmap((int)Math.Ceiling(root.ActualWidth * dpi.DpiScaleX),
                    (int)Math.Ceiling(root.ActualHeight * dpi.DpiScaleY), dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
                host.Render(root);
                bitmap = new CroppedBitmap(host, new Int32Rect((int)Math.Round(bounds.X * dpi.DpiScaleX),
                    (int)Math.Round(bounds.Y * dpi.DpiScaleY), width, height));
            }
            else
            {
                var rendered = new RenderTargetBitmap(width, height, dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
                rendered.Render(element);
                bitmap = rendered;
            }
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var stream = File.Create(path)) encoder.Save(stream);
            ExtensionCheckTrace.Write("captured", surface, path, width.ToString(), height.ToString(),
                dpi.PixelsPerInchX.ToString(), dpi.PixelsPerInchY.ToString());
        }
    }

    internal static class CapturePreview
    {
        private static readonly DependencyProperty PreviewProperty = DependencyProperty.RegisterAttached(
            "Preview", typeof(bool), typeof(CapturePreview), new PropertyMetadata(false));

        internal static bool IsPreview(Window window) => (bool)window.GetValue(PreviewProperty);

        internal static IEnumerable<DependencyObject> Descendants(DependencyObject element)
        {
            // Logical tree also works before Show/Loaded, when templates have not been materialized.
            foreach (var child in LogicalTreeHelper.GetChildren(element).OfType<DependencyObject>())
            {
                yield return child;
                foreach (var descendant in Descendants(child)) yield return descendant;
            }
        }

        internal static IEnumerable<DependencyObject> VisualDescendants(DependencyObject element)
        {
            yield return element;
            for (int index = 0; index < VisualTreeHelper.GetChildrenCount(element); index++)
                foreach (var descendant in VisualDescendants(VisualTreeHelper.GetChild(element, index))) yield return descendant;
        }

        internal static void Prepare(Window window)
        {
            window.SetValue(PreviewProperty, true);
            window.ShowActivated = false;
            // Even an accidental click/Enter cannot set DialogResult on a modeless preview or apply settings.
            foreach (var cancel in Descendants(window).OfType<Button>().Where(button => button.IsCancel))
            {
                cancel.IsCancel = false;
                cancel.Click += (sender, args) => window.Close();
            }
        }
    }

    internal static class CaptureNative
    {
        private delegate IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam);
        private static readonly HookCallback ActivationHook = OnCbt;
        private static IntPtr _activationHook;
        [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
        [DllImport("user32.dll", SetLastError = true)] private static extern IntPtr SetWindowsHookEx(int hook, HookCallback callback, IntPtr module, uint thread);
        [DllImport("user32.dll")] private static extern IntPtr CallNextHookEx(IntPtr hook, int code, IntPtr wParam, IntPtr lParam);

        internal static void PreventActivation()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_activationHook != IntPtr.Zero) return;
            // Thread-local CBT hook, only in an explicitly enabled Debug test process. Some shell APIs
            // (including options navigation) can try to activate windows even when the caller does not.
            _activationHook = SetWindowsHookEx(5 /* WH_CBT */, ActivationHook, IntPtr.Zero, GetCurrentThreadId());
            if (_activationHook == IntPtr.Zero) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }

        private static IntPtr OnCbt(int code, IntPtr wParam, IntPtr lParam) =>
            code == 5 /* HCBT_ACTIVATE: veto activation, never touch another process */ ? new IntPtr(1) :
                CallNextHookEx(_activationHook, code, wParam, lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect { public int Left, Top, Right, Bottom; }
        [StructLayout(LayoutKind.Sequential)]
        private struct NativePoint { public int X, Y; }
        [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr window, out Rect rect);
        [DllImport("user32.dll")] private static extern bool PrintWindow(IntPtr window, IntPtr dc, uint flags);
        [DllImport("user32.dll")] private static extern bool IsChild(IntPtr parent, IntPtr child);
        [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr window);
        [DllImport("user32.dll")] private static extern bool ClientToScreen(IntPtr window, ref NativePoint point);
        [DllImport("user32.dll")] private static extern IntPtr GetParent(IntPtr window);

        internal static IntPtr MainWindow
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var shell = (Microsoft.VisualStudio.Shell.Interop.IVsUIShell)Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SVsUIShell));
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(shell.GetDialogOwnerHwnd(out var handle));
                return handle;
            }
        }

        internal static void CaptureMain(string path)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var handle = MainWindow;
            if (!GetWindowRect(handle, out var rect)) throw new InvalidOperationException("Cannot read SSMS bounds.");
            int width = rect.Right - rect.Left, height = rect.Bottom - rect.Top;
            if (width <= 0 || height <= 0) throw new InvalidOperationException("SSMS has no window bounds.");
            using (var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var dc = graphics.GetHdc();
                try
                {
                    if (!PrintWindow(handle, dc, 2 /* PW_RENDERFULLCONTENT */))
                        throw new InvalidOperationException("PrintWindow failed.");
                }
                finally { graphics.ReleaseHdc(dc); }
                // A private, never-displayed desktop has no DWM composition for PrintWindow to copy.
                // Preserve its native pixels, and supply the live WPF HWNDs from their real visual trees.
                // No screen pixels, focus, re-created UI, or synthetic text are involved.
                int roots = 0;
                var sources = PresentationSource.CurrentSources.OfType<HwndSource>()
                    .Where(source => source.RootVisual is FrameworkElement && IsWindowVisible(source.Handle) &&
                        (source.Handle == handle || IsChild(handle, source.Handle)))
                    .OrderBy(source => Depth(source.Handle)).ToArray();
                foreach (var source in sources)
                {
                    var root = (FrameworkElement)source.RootVisual;
                    root.UpdateLayout();
                    if (root is Window && VisualTreeHelper.GetChildrenCount(root) > 0)
                        root = (FrameworkElement)VisualTreeHelper.GetChild(root, 0);
                    if (root.ActualWidth <= 0 || root.ActualHeight <= 0) continue;
                    var dpi = VisualTreeHelper.GetDpi(root);
                    var rendered = new RenderTargetBitmap((int)Math.Ceiling(root.ActualWidth * dpi.DpiScaleX),
                        (int)Math.Ceiling(root.ActualHeight * dpi.DpiScaleY), dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
                    rendered.Render(root);
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rendered));
                    var origin = new NativePoint();
                    if (!ClientToScreen(source.Handle, ref origin)) throw new InvalidOperationException("Cannot locate WPF HWND.");
                    using (var stream = new MemoryStream())
                    {
                        encoder.Save(stream);
                        stream.Position = 0;
                        using (var overlay = System.Drawing.Image.FromStream(stream))
                            graphics.DrawImageUnscaled(overlay, origin.X - rect.Left, origin.Y - rect.Top);
                    }
                    roots++;
                }
                bitmap.Save(path, ImageFormat.Png);
                ExtensionCheckTrace.Write("main-wpf-roots", roots.ToString());
            }
            ExtensionCheckTrace.Write("captured", "main", path, width.ToString(), height.ToString(), "PrintWindow+WPF");

            int Depth(IntPtr window)
            {
                int depth = 0;
                for (var parent = GetParent(window); parent != IntPtr.Zero && depth < 100; parent = GetParent(parent)) depth++;
                return depth;
            }
        }
    }
}
#endif
