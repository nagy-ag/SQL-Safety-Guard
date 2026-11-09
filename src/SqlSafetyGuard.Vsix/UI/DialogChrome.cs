using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.VisualStudio.PlatformUI;

namespace SqlSafetyGuard.UI
{
    /// <summary>
    /// Windows draws dialog title bars light unless the window opts in to dark mode. Match the title bar to the
    /// SSMS theme: dark mode for dark themes, and on Windows 11 the dialog's own background and text colours.
    /// Purely cosmetic: every call is best effort and never affects the dialog's behaviour.
    /// </summary>
    internal static class DialogChrome
    {
        private const int ImmersiveDarkMode = 20, ImmersiveDarkModeBefore20H1 = 19, CaptionColor = 35, TextColor = 36;

        /// <summary>Last outcome, for the Debug capture receipts: dark flag and the HRESULT of each attribute.</summary>
        internal static string LastResult { get; private set; } = "not applied";

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        internal static void Apply(Window window)
        {
            window.SourceInitialized += (sender, args) => Update(window);
            ThemeChangedEventHandler changed = args => window.Dispatcher.BeginInvoke(new Action(() => Update(window)));
            try { VSColorTheme.ThemeChanged += changed; } catch (Exception) { return; }
            window.Closed += (sender, args) => { try { VSColorTheme.ThemeChanged -= changed; } catch (Exception) { } };
        }

        private static void Update(Window window)
        {
            try
            {
                var hwnd = new WindowInteropHelper(window).Handle;
                if (hwnd == IntPtr.Zero) return;
                var background = ToColor(VSColorTheme.GetThemedColor(ThemedDialogColors.WindowPanelColorKey));
                var text = ToColor(VSColorTheme.GetThemedColor(ThemedDialogColors.WindowPanelTextColorKey));
                int dark = 0.2126 * background.R + 0.7152 * background.G + 0.0722 * background.B < 128 ? 1 : 0;
                int mode = DwmSetWindowAttribute(hwnd, ImmersiveDarkMode, ref dark, sizeof(int));
                if (mode != 0) mode = DwmSetWindowAttribute(hwnd, ImmersiveDarkModeBefore20H1, ref dark, sizeof(int));
                // Windows 11 only; older versions reject these attributes and keep the dark/light frame above.
                int caption = ToColorRef(background), captionText = ToColorRef(text);
                int captionResult = DwmSetWindowAttribute(hwnd, CaptionColor, ref caption, sizeof(int));
                int textResult = DwmSetWindowAttribute(hwnd, TextColor, ref captionText, sizeof(int));
                LastResult = string.Format("dark={0} mode=0x{1:X} caption=#{2:X2}{3:X2}{4:X2}:0x{5:X} text=0x{6:X}",
                    dark, mode, background.R, background.G, background.B, captionResult, textResult);
            }
            catch (Exception exception) { LastResult = "failed: " + exception.Message; /* Cosmetic only. */ }
        }

        private static Color ToColor(System.Drawing.Color color) => Color.FromRgb(color.R, color.G, color.B);
        private static int ToColorRef(Color color) => color.R | (color.G << 8) | (color.B << 16);
    }
}
