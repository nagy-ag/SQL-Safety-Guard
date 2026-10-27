using System.Windows.Media;
using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.UI
{
    internal static class VerdictBrushes
    {
        public static readonly Brush Green = Frozen(0x2E, 0x7D, 0x32);
        public static readonly Brush Yellow = Frozen(0xF9, 0xA8, 0x25);
        public static readonly Brush Red = Frozen(0xC6, 0x28, 0x28);

        public static Brush For(Verdict verdict) =>
            verdict.IsGreen() ? Green : verdict == Verdict.Unknown ? Yellow : Red;

        /// <summary>Readable text colour on top of <see cref="For"/>.</summary>
        public static Brush TextOn(Verdict verdict) =>
            verdict == Verdict.Unknown ? Brushes.Black : Brushes.White;

        private static Brush Frozen(byte r, byte g, byte b)
        {
            var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            brush.Freeze();
            return brush;
        }
    }
}
