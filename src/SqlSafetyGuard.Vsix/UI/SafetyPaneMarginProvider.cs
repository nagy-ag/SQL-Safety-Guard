using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace SqlSafetyGuard.UI
{
    /// <summary>
    /// Puts the safety pane inside each SQL query window, to the right of the vertical scroll bar.
    /// Attached to every document text view; <see cref="CreateMargin"/> returns null for anything that isn't T-SQL.
    /// </summary>
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(SafetyPaneMargin.MarginName)]
    [MarginContainer(PredefinedMarginNames.Right)]
    [Order(After = PredefinedMarginNames.VerticalScrollBar)]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class SafetyPaneMarginProvider : IWpfTextViewMarginProvider
    {
        /// <summary>Statement highlights are drawn under the text.</summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name(SafetyPaneMargin.HighlightLayerName)]
        [Order(Before = PredefinedAdornmentLayers.Text)]
        internal AdornmentLayerDefinition HighlightLayer = null;

        [Import]
        internal ITextDocumentFactoryService DocumentFactory = null;

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            var view = wpfTextViewHost.TextView;
            string path = null;
            if (DocumentFactory != null && DocumentFactory.TryGetTextDocument(view.TextDataModel.DocumentBuffer, out var document))
                path = document.FilePath;

            var contentType = view.TextBuffer.ContentType;
            bool isSql = (path != null && path.EndsWith(".sql", StringComparison.OrdinalIgnoreCase)) ||
                         contentType.TypeName.IndexOf("SQL", StringComparison.OrdinalIgnoreCase) >= 0;

            return isSql ? new SafetyPaneMargin(view, path) : null;
        }
    }
}
