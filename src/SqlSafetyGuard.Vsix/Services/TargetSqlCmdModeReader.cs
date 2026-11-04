using System;
using System.Reflection;

namespace SqlSafetyGuard.Services
{
    /// <summary>Reads only SQLCMD state belonging to the already captured editor. Unknown or conflicting state stays unknown.</summary>
    internal static class TargetSqlCmdModeReader
    {
        public static bool? TryRead(object editor)
        {
            if (editor == null) return null;
            bool? result = null;
            try
            {
                // SSMS 22.4 exposes IsSqlCmdMode on ScriptAndResultsEditorControl;
                // newer hosts expose IsSqlCmdModeEnabled. Never substitute global active-window state.
                for (var type = editor.GetType(); type != null; type = type.BaseType)
                foreach (var name in new[] { "IsSqlCmdModeEnabled", "IsSqlCmdMode" })
                {
                    var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public |
                        BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                    if (property == null) continue;
                    if (property.PropertyType != typeof(bool) || !property.CanRead || property.GetIndexParameters().Length != 0)
                        return null;
                    var value = property.GetValue(editor);
                    if (!(value is bool mode) || (result.HasValue && result.Value != mode)) return null;
                    result = mode;
                }
                return result;
            }
            catch (Exception) { return null; }
        }
    }
}
