using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.Internal.VisualStudio.Services.UnifiedSettings;
using Microsoft.VisualStudio.Utilities.UnifiedSettings;

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
        private GuardMode _guardMode = GuardMode.WarnOnYellowAndRed;
        private bool _showServerAndDatabase = true;
        private string _allowList = string.Empty;
        private bool _tempdbIsTemp;
        private string _trustedProcedures = string.Empty;
        private string _environmentRules = string.Empty;

        internal event EventHandler Changed;

        // Native settings have already been committed before DialogPage delivers their
        // property notifications. Its legacy auto-save writes every property, including
        // stale values from notifications later in the same batch. Only explicit local
        // edits should save back. The native UI context is not active until the
        // Options tab opens, although settings.json can change at any time.
        public override void SaveSettingsToStorage() { }

        protected override void OnApply(PageApplyEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            PersistChanges();
        }

        [Guid("e3684f31-344e-42ea-9047-b620fdc7ac25")]
        private sealed class NativeSettingsService { }

        internal void PersistChanges()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var manager = Package.GetGlobalService(typeof(NativeSettingsService)) as ISettingsManager;
            if (manager == null || manager.GetReader().GetValue<bool>("sqlSafetyGuard.showConnection").Outcome == SettingRetrievalOutcome.NotSupportedInClassicMode)
            {
                base.SaveSettingsToStorage();
                return;
            }

            // Commit the entire explicit edit as one native batch. DialogPage's
            // per-property persistence cache can be stale when Options first opens.
            var writer = manager.GetWriter("SQL Safety Guard");
            Queue("guardMode", GuardMode == GuardMode.Off ? "Off" : GuardMode == GuardMode.WarnOnRedOnly ? "Red" : "YellowRed");
            Queue("showConnection", ShowServerAndDatabase);
            Queue("allowList", AllowList);
            Queue("tempdbIsTemp", TempdbIsTemp);
            Queue("trustedProcedures", TrustedProcedures);
            Queue("environmentRules", EnvironmentRules);
            var commit = writer.RequestCommit("Save SQL Safety Guard settings");
            if (commit.Outcome != SettingCommitOutcome.Success && commit.Outcome != SettingCommitOutcome.NoChangesQueued)
                throw new InvalidOperationException("SSMS could not save the settings: " + commit.Outcome);

            void Queue<T>(string name, T value)
            {
                var result = writer.EnqueueChange("sqlSafetyGuard." + name, value);
                if (result.Outcome != SettingChangeOutcome.PendingCommit)
                    throw new InvalidOperationException("SSMS could not save " + name + ": " + result.Outcome);
            }
        }

        private void Set<T>(ref T field, T value)
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        [Category("F5 guard")]
        [DisplayName("F5 guard")]
        [Description("When executing a query: Off = never check; Warn on red only = prompt before real writes; " +
                     "Warn on yellow and red = also prompt when the effect cannot be seen (procedures, dynamic SQL, parse errors). " +
                     "Read-only, temp-only and allow-listed queries always run without a prompt. " +
                     "Import and export are available on the SQL Safety Guard settings page.")]
        [DefaultValue(GuardMode.WarnOnYellowAndRed)]
        [TypeConverter(typeof(GuardModeConverter))]
        [UnifiedSettingsMoniker("SqlSafetyGuard.Options.GeneralOptionsPage.GuardMode")]
        public GuardMode GuardMode { get => _guardMode; set => Set(ref _guardMode, value); }

        [Category("F5 guard")]
        [DisplayName("Show connection in confirmation")]
        [Description("Shows the current server and database in the confirmation dialog. Uses undocumented SSMS internals; " +
                     "unknown connection details always remain visible and use global settings.")]
        [DefaultValue(true)]
        [UnifiedSettingsMoniker("SqlSafetyGuard.Options.GeneralOptionsPage.ShowServerAndDatabase")]
        public bool ShowServerAndDatabase { get => _showServerAndDatabase; set => Set(ref _showServerAndDatabase, value); }

        [Category("Analysis")]
        [DisplayName("Safe tables allow-list")]
        [Description("One pattern per line, e.g. dbo.Staging_*. Writes to matching tables count as green. Case-insensitive; " +
                     "* and ? wildcards. No implicit dbo: schema-qualified patterns require a resolved schema. " +
                     "Omitted database matches the known current database only; omitted server never matches linked servers. " +
                     "Wildcard-only patterns are ignored with a warning. Lines starting with -- are comments. " +
                     "Import settings highlights additions that make writes green before you confirm.")]
        [DefaultValue("")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [UnifiedSettingsMoniker("SqlSafetyGuard.Options.GeneralOptionsPage.AllowList")]
        public string AllowList { get => _allowList; set => Set(ref _allowList, value ?? string.Empty); }

        [Category("Analysis")]
        [DisplayName("Permanent tempdb objects are temporary")]
        [Description("Opt in to green writes to permanent tempdb tables. Off by default; #temp and ##temp remain green.")]
        [DefaultValue(false)]
        [UnifiedSettingsMoniker("SqlSafetyGuard.Options.GeneralOptionsPage.TempdbIsTemp")]
        public bool TempdbIsTemp { get => _tempdbIsTemp; set => Set(ref _tempdbIsTemp, value); }

        [Category("Analysis")]
        [DisplayName("Trusted procedures")]
        [Description("One schema-qualified name per line; wildcards only in the last part. No implicit dbo. " +
                     "Example: dbo.sp_WhoIsActive or master.dbo.sp_Blitz*. Trust is user asserted, not verified.")]
        [DefaultValue("")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [UnifiedSettingsMoniker("SqlSafetyGuard.Options.GeneralOptionsPage.TrustedProcedures")]
        public string TrustedProcedures { get => _trustedProcedures; set => Set(ref _trustedProcedures, value ?? string.Empty); }

        [Category("F5 guard")]
        [Browsable(false)]
        [Description("Legacy setting retained for compatibility; confirmation never requires typing.")]
        [DefaultValue(false)]
        public bool RequireTypedConfirmation { get; set; }

        [Category("F5 guard")]
        [DisplayName("Environment rules")]
        [Description("First match wins. One rule per line: server | database (blank = any) | label | Off/Red/YellowRed | legacy true/false (ignored) | allow-list | trusted procedures. " +
                     "Lists are semicolon-separated and ADD to global lists. Unknown server/database uses globals. See README for examples.")]
        [DefaultValue("")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [UnifiedSettingsMoniker("SqlSafetyGuard.Options.GeneralOptionsPage.EnvironmentRules")]
        public string EnvironmentRules { get => _environmentRules; set => Set(ref _environmentRules, value ?? string.Empty); }
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
