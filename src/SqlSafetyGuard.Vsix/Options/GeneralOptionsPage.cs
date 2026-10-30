using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace SqlSafetyGuard.Options
{
    [TypeConverter(typeof(GuardModeConverter))]
    public enum GuardMode
    {
        [Description("Off")]
        Off = 0,

        [Description("Warn on red only")]
        WarnOnRedOnly = 1,

        [Description("Warn on yellow and red")]
        WarnOnYellowAndRed = 2,
    }

    /// <summary>Tools &gt; Options &gt; SQL Safety Guard &gt; General.</summary>
    [ComVisible(true)]
    [Guid(PackageGuids.OptionsPageString)]
    public sealed class GeneralOptionsPage : DialogPage
    {
        [Category("F5 guard")]
        [DisplayName("F5 guard")]
        [Description("When executing a query: Off = never check; Warn on red only = prompt before real writes; " +
                     "Warn on yellow and red = also prompt when the effect cannot be seen (procedures, dynamic SQL, parse errors). " +
                     "Read-only, temp-only and allow-listed queries always run without a prompt. " +
                     "Use Tools > Export Settings... to share your configuration; Tools > Import Settings... validates and previews changes before applying them.")]
        [DefaultValue(GuardMode.WarnOnYellowAndRed)]
        [TypeConverter(typeof(GuardModeConverter))]
        public GuardMode GuardMode { get; set; } = GuardMode.WarnOnYellowAndRed;

        [Category("F5 guard")]
        [DisplayName("Show server and database in the dialog")]
        [Description("Shows the current server and database in the confirmation dialog. Uses undocumented SSMS internals; " +
                     "unknown connection details always remain visible and use global settings.")]
        [DefaultValue(true)]
        public bool ShowServerAndDatabase { get; set; } = true;

        [Category("Analysis")]
        [DisplayName("Safe tables allow-list")]
        [Description("One pattern per line, e.g. dbo.Staging_*. Writes to matching tables count as green. Case-insensitive; " +
                     "* and ? wildcards. No implicit dbo: schema-qualified patterns require a resolved schema. " +
                     "Omitted database matches the known current database only; omitted server never matches linked servers. " +
                     "Wildcard-only patterns are ignored with a warning. Lines starting with -- are comments. " +
                     "Tools > Import Settings... highlights additions that make writes green before you confirm.")]
        [DefaultValue("")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string AllowList { get; set; } = string.Empty;

        [Category("Analysis")]
        [DisplayName("Treat permanent tempdb objects as temporary")]
        [Description("Opt in to green writes to permanent tempdb tables. Off by default; #temp and ##temp remain green.")]
        [DefaultValue(false)]
        public bool TempdbIsTemp { get; set; }

        [Category("Analysis")]
        [DisplayName("Trusted read-only procedures")]
        [Description("One schema-qualified name per line; wildcards only in the last part. No implicit dbo. " +
                     "Example: dbo.sp_WhoIsActive or master.dbo.sp_Blitz*. Trust is user asserted, not verified.")]
        [DefaultValue("")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string TrustedProcedures { get; set; } = string.Empty;

        [Category("F5 guard")]
[Browsable(false)]
[Description("Legacy setting retained for compatibility; confirmation never requires typing.")]
        [DefaultValue(false)]
        public bool RequireTypedConfirmation { get; set; }

        [Category("F5 guard")]
        [DisplayName("Server / environment rules")]
        [Description("First match wins. One rule per line: server | database (blank = any) | label | Off/Red/YellowRed | legacy true/false (ignored) | allow-list | trusted procedures. " +
                     "Lists are semicolon-separated and ADD to global lists. Unknown server/database uses globals. See README for examples.")]
        [DefaultValue("")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string EnvironmentRules { get; set; } = string.Empty;
    }

    /// <summary>Shows and persists the enum by its description; also accepts the member name.</summary>
    internal sealed class GuardModeConverter : EnumConverter
    {
        public GuardModeConverter()
            : base(typeof(GuardMode))
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is GuardMode mode)
                return Describe(mode);
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string text)
            {
                foreach (GuardMode mode in Enum.GetValues(typeof(GuardMode)))
                {
                    if (string.Equals(Describe(mode), text, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(mode.ToString(), text, StringComparison.OrdinalIgnoreCase))
                        return mode;
                }

                return GuardMode.WarnOnYellowAndRed;
            }

            return base.ConvertFrom(context, culture, value);
        }

        private static string Describe(GuardMode mode)
        {
            var field = typeof(GuardMode).GetField(mode.ToString());
            var description = field?.GetCustomAttributes(typeof(DescriptionAttribute), false).OfType<DescriptionAttribute>().FirstOrDefault();
            return description?.Description ?? mode.ToString();
        }
    }
}
