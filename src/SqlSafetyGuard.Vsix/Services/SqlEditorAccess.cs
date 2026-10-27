using System;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace SqlSafetyGuard.Services
{
    /// <summary>Reads the query text from, and navigates within, the active SSMS SQL editor via DTE.</summary>
    internal static class SqlEditorAccess
    {
        /// <summary>
        /// Captures exactly what Query.Execute would run: the selection when there is one, otherwise the whole window.
        /// Returns null when the active document is not a T-SQL editor.
        /// </summary>
        public static QuerySnapshot TryCapture(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var document = dte?.ActiveDocument;
            if (document == null || !IsSqlDocument(document))
                return null;

            if (!(document.Object("TextDocument") is TextDocument textDocument))
                return null;

            var selection = textDocument.Selection;
            if (selection != null && !selection.IsEmpty)
            {
                var top = selection.TopPoint;
                return new QuerySnapshot(document, selection.Text, true, top.Line, top.LineCharOffset);
            }

            var start = textDocument.StartPoint.CreateEditPoint();
            return new QuerySnapshot(document, start.GetText(textDocument.EndPoint), false, 1, 1);
        }

        public static bool IsSqlDocument(Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var extension = Path.GetExtension(document.FullName ?? document.Name ?? string.Empty);
                if (string.Equals(extension, ".sql", StringComparison.OrdinalIgnoreCase))
                    return true;

                var language = document.Language ?? string.Empty;
                return language.IndexOf("SQL", StringComparison.OrdinalIgnoreCase) >= 0 &&
                       language.IndexOf("MDX", StringComparison.OrdinalIgnoreCase) < 0 &&
                       language.IndexOf("DMX", StringComparison.OrdinalIgnoreCase) < 0 &&
                       language.IndexOf("XMLA", StringComparison.OrdinalIgnoreCase) < 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Moves the caret to a position in the analyzed text (does not select, so F5 still runs the same text).</summary>
        public static void Navigate(QuerySnapshot snapshot, int analyzedOffset)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            snapshot.ToEditorPosition(analyzedOffset, out var line, out var column);

            try
            {
                snapshot.Document.Activate();
                if (!(snapshot.Document.Object("TextDocument") is TextDocument textDocument))
                    return;

                var selection = textDocument.Selection;
                try
                {
                    selection.MoveToLineAndOffset(line, column, false);
                }
                catch (ArgumentException)
                {
                    // The text changed since the analysis; go to the line at least.
                    selection.GotoLine(Math.Max(1, Math.Min(line, textDocument.EndPoint.Line)), false);
                }

                selection.ActivePoint.TryToShow(vsPaneShowHow.vsPaneShowCentered, null);
            }
            catch (Exception ex)
            {
                OutputLog.Warning($"Could not navigate to line {line}: {ex.Message}");
            }
        }
    }
}
