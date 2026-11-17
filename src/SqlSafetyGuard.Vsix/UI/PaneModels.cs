using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using SqlSafetyGuard.Analyzer;

namespace SqlSafetyGuard.UI
{
    /// <summary>One finding, tracked in the editor buffer so highlights follow edits until the next analysis.</summary>
    internal sealed class PaneFinding : INotifyPropertyChanged
    {
        private readonly ITrackingSpan _span;
        private readonly ITrackingSpan _headerSpan;
        private readonly ITrackingSpan[] _sourceSpans;
        private readonly int _operationIndex;
        private bool _isExpanded;
        private PaneCodeBody _body;
        private PaneFinding _parentOperation;
        private string _stateScope = string.Empty;
        private PaneVerdictFilter _filter = PaneVerdictFilter.All;

        public PaneFinding(Finding finding, ITextSnapshot snapshot, int baseOffset, string currentDatabase = null, int operationIndex = 0)
        {
            Finding = finding;
            _operationIndex = operationIndex;
            int start = Math.Min(snapshot.Length, baseOffset + finding.StartOffset);
            int length = Math.Min(snapshot.Length - start, finding.Length);
            var span = new Span(start, length);
            if (length == 0)
                span = snapshot.GetLineFromPosition(start).Extent.Span; // Parse errors etc.: the whole line.
            _span = snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);

            ExecutionLine = snapshot.GetLineNumberFromPosition(span.Start) + 1;
            _sourceSpans = finding.SourceRanges.Where(r => r.StartOffset >= 0 && r.Length > 0 &&
                    baseOffset + r.StartOffset <= snapshot.Length - r.Length)
                .Select(r => snapshot.CreateTrackingSpan(new Span(baseOffset + r.StartOffset, r.Length), SpanTrackingMode.EdgeExclusive)).ToArray();
            EditorLine = _sourceSpans.Length > 0 ? snapshot.GetLineNumberFromPosition(_sourceSpans[0].GetSpan(snapshot).Start) + 1 : ExecutionLine;
            SourceLocation = _sourceSpans.Length > 0 ? "SQL source: " + string.Join(", ", _sourceSpans.Select(s =>
                "L" + (snapshot.GetLineNumberFromPosition(s.GetSpan(snapshot).Start) + 1)).Distinct()) : "SQL source unavailable";
            var firstLine = snapshot.GetLineFromPosition(span.Start);
            _headerSpan = snapshot.CreateTrackingSpan(Span.FromBounds(span.Start, Math.Min(span.End, firstLine.End.Position)), SpanTrackingMode.EdgeExclusive);
            var snippet = snapshot.GetText(Span.FromBounds(span.Start, Math.Min(span.End, firstLine.End.Position))).Trim();
            bool more = span.End > firstLine.End.Position;
            Snippet = snippet.Length > 70 ? snippet.Substring(0, 70) + "…" : more ? snippet + " …" : snippet;

            var target = finding.ResolvedTarget ?? finding.Target;
            GroupKey = finding.IsContextChange ? "USE:" + target : string.IsNullOrEmpty(target) ? finding.Action : PaneGroup.Qualify(target, currentDatabase);
        }

        public IEnumerable<PaneFinding> DescendantsAndSelf()
        {
            yield return this;
            if (Body != null)
                foreach (var child in Body.Findings.SelectMany(f => f.DescendantsAndSelf())) yield return child;
        }

        public Finding Finding { get; }
        public PaneCodeBody Body
        {
            get => _body;
            set
            {
                _body = value;
                if (value != null)
                {
                    value.Parent = this;
                    foreach (var child in value.Findings)
                    {
                        child._parentOperation = this;
                        child.SetStateScope(StateKey);
                    }
                }
            }
        }
        private void SetStateScope(string scope)
        {
            _stateScope = scope;
            if (Body != null)
                foreach (var child in Body.Findings) child.SetStateScope(StateKey);
        }
        public bool IsProcedureCall { get; set; }
        public bool IsPreviewOperation { get; set; }
        public Visibility ReasonVisibility => Body == null || Body.IsEmbedded ? Visibility.Visible : Visibility.Collapsed;
        public Visibility BodyVisibility => Body != null && Body.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        public bool MatchesFilter => PaneFilterRules.Includes(_filter, Finding) &&
            (Body?.Kind != CodeBodyKind.RemoteQuery || (_filter & PaneVerdictFilter.Remote) != 0);
        public bool IsVisible => MatchesFilter || Body != null && Body.IsVisible;
        public IEnumerable<PaneFinding> MatchingOperations => (MatchesFilter ? new[] { this } : Enumerable.Empty<PaneFinding>())
            .Concat(Body != null && Body.IsVisible ? Body.VisibleFindings.SelectMany(f => f.MatchingOperations) : Enumerable.Empty<PaneFinding>());
        public void ApplyFilter(PaneVerdictFilter filter)
        {
            _filter = filter;
            Body?.ApplyFilter(filter);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BodyVisibility)));
        }

        public int EditorLine { get; }
        public int ExecutionLine { get; }
        public string SourceLocation { get; }
        public bool HasSource => _sourceSpans != null && _sourceSpans.Length > 0;
        public string ExecutionLocationLabel { get; set; } = "Executed at";
        public string ExecutionLocation => ExecutionLocationLabel + ": L" + ExecutionLine;
        public Visibility SourceLocationVisibility => PaneOperationPresentation.IsDecodedDynamic(Finding) ? Visibility.Visible : Visibility.Collapsed;

        public string GroupKey { get; }

        public string Snippet { get; }

        public string Header => $"Line {EditorLine} · {Finding.Action}";
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value) return;
                _isExpanded = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Chevron)));
            }
        }
        public string Chevron => IsExpanded ? "⌃" : "⌄";
        public string Tooltip => SafeText + Environment.NewLine + Reason + (Body == null ? string.Empty :
            Environment.NewLine + Body.ContextDescription + Environment.NewLine + Body.Summary);
        public string StateKey
        {
            get
            {
                var span = _span.GetSpan(_span.TextBuffer.CurrentSnapshot);
                return _stateScope + "/" + span.Start.Position + ":" + span.Length + ":" + Finding.Action + ":" + Finding.Target + ":" + _operationIndex;
            }
        }
        public string Reason => !string.IsNullOrEmpty(Note) ? Note : Finding.Verdict == Verdict.AllowListed ? "Allowed by your settings." :
            Finding.Verdict == Verdict.TempOnly ? "Temporary-object operations only." :
            Finding.Verdict.IsRed() ? "Changes a permanent object." : Finding.Verdict.IsYellow() ? "The analyzer cannot see what runs." : "No permanent changes detected.";
        public string CompactAction => Finding.Action.Replace("CREATE OR ALTER ", "CREATE/ALTER ");
        public string InlineLabel => Body != null || IsProcedureCall ? CompactAction :
            IsPreviewOperation || PaneOperationPresentation.IsDynamic(Finding)
                ? (Finding.Action + " " + Finding.DisplayTarget).TrimEnd() : Snippet;

        /// <summary>The whole-target warning (same wording as the F5 dialog) followed by the analyzer's note.</summary>
        public string Note
        {
            get
            {
                string note = Finding.Note.Replace("inside dynamic SQL, ", string.Empty).Replace("inside dynamic SQL", string.Empty);
                if (!Finding.AffectsAllRows) return note;
                bool noteRepeatsSummary = string.IsNullOrEmpty(note) || note == "no WHERE: affects every row" ||
                    note == "removes every row" || note.Contains(Finding.CteOrDerivedTableLimitNote);
                return noteRepeatsSummary ? Finding.AllRowsSummary : Finding.AllRowsSummary + " · " + note;
            }
        }

        public Visibility NoteVisibility => string.IsNullOrEmpty(Note) ? Visibility.Collapsed : Visibility.Visible;
        public bool IsPermanentWholeTarget => Finding.Verdict.IsRed() && Finding.AffectsAllRows;

        public Brush Marker => Finding.IsContextChange ? VerdictBrushes.Context : VerdictBrushes.For(Finding.Verdict);
        public Brush ContextMarker => VerdictBrushes.Context;
        public Visibility ContextVisibility => Finding.IsContextChange ? Visibility.Visible : Visibility.Collapsed;
        public string ContextTooltip => SafetyDictionary.Context;

        public string SafeText => $"Line {EditorLine} · {Finding.Action} {Finding.DisplayTarget}".TrimEnd();

        public SnapshotSpan ExecutionSpanIn(ITextSnapshot snapshot) => (Body == null || Body.IsEmbedded ? _span : _headerSpan).GetSpan(snapshot);
        // The enclosing definition is context, never an execution selection or a verdict fill.
        public SnapshotSpan? DefinitionContextIn(ITextSnapshot snapshot)
        {
            for (var operation = this; operation != null; operation = operation._parentOperation)
                if (operation.Body != null && !operation.Body.IsEmbedded) return operation._span.GetSpan(snapshot);
            return null;
        }
        public IReadOnlyList<SnapshotSpan> SourceSpansIn(ITextSnapshot snapshot) => _sourceSpans.Select(s => s.GetSpan(snapshot)).ToArray();
        public SnapshotSpan SpanIn(ITextSnapshot snapshot) => _sourceSpans.Length > 0 ? _sourceSpans[0].GetSpan(snapshot) : ExecutionSpanIn(snapshot);
        public IReadOnlyList<SnapshotSpan> PrimarySpansIn(ITextSnapshot snapshot) => _sourceSpans.Length > 0 ? SourceSpansIn(snapshot) : new[] { ExecutionSpanIn(snapshot) };

        public override string ToString() => SafeText; // Name for UI Automation.
    }

    internal sealed class PaneGroup : INotifyPropertyChanged
    {
        private bool _isExpanded;
        private PaneVerdictFilter _filter = PaneVerdictFilter.All;

        public PaneGroup(string key, IReadOnlyList<PaneFinding> findings) : this(key, findings, false, false) { }

        internal PaneGroup(string key, IReadOnlyList<PaneFinding> findings, bool scopeObject, bool nested)
        {
            Key = key;
            IsScopeObject = scopeObject;
            IsNested = nested;
            Findings = findings.OrderBy(finding => finding.Finding.StartOffset).ToList();
            Verdict = findings.Max(finding => finding.Finding.Verdict);
            Counts = new PaneVerdictCounts(findings.Select(finding => finding.Finding.Verdict));
            FirstOffset = Findings[0].Finding.StartOffset;
            var parts = SplitName(key).Select(Unquote).ToList();
            int prefix = Math.Max(0, parts.Count - 2);
            DatabasePrefix = Findings.All(f => f.Finding.IsContextChange) || prefix == 0 ? string.Empty : string.Join(".", parts.Take(prefix)) + ".";
            ObjectName = Findings.All(f => f.Finding.IsContextChange) ? "USE " + Findings[0].Finding.Target : IsDynamicContainer
                ? Findings.Any(f => PaneOperationPresentation.IsDecodedDynamic(f.Finding)) || Findings.All(f => f.Finding.Verdict.IsGreen()) ? "Dynamic SQL" : "Runtime SQL"
                : Findings.All(finding => string.IsNullOrEmpty(finding.Finding.ResolvedTarget ?? finding.Finding.Target))
                ? Findings.All(finding => finding.Finding.Action == "BEGIN TRAN" || finding.Finding.Action == "COMMIT" ||
                    finding.Finding.Action == "ROLLBACK" || finding.Finding.Action == "SAVE TRAN") ? "Transaction" : Findings[0].Finding.Action
                : string.Join(".", parts.Skip(prefix));
            if (IsDynamicContainer && Findings.Any(f => PaneOperationPresentation.IsDecodedDynamic(f.Finding)))
                ScopeRows = new PaneScopeRows(Findings, includeDynamicContainers: false);
            RefreshBodyBadge();
        }

        public string Key { get; }
        public bool IsScopeObject { get; }
        public bool IsNested { get; }
        public bool IsDynamicContainer => !IsScopeObject && Findings.All(f => PaneOperationPresentation.IsDynamic(f.Finding));
        public string StateKey => (IsNested ? Findings[0].StateKey : string.Empty) + "|" + Key;
        public PaneScopeRows ScopeRows { get; }
        public IReadOnlyList<object> VisibleRows => ScopeRows?.VisibleRows ?? VisibleFindings.Cast<object>().ToList();
        internal IEnumerable<PaneGroup> DescendantsAndSelf()
        {
            yield return this;
            foreach (var group in ScopeRows != null ? ScopeRows.AllGroups : Findings.Where(f => f.Body != null).SelectMany(f => f.Body.ScopeRows.AllGroups))
                yield return group;
        }
        private void RefreshBodyBadge()
        {
            _bodies = Findings.Where(f => f.Body != null).Select(f => f.Body).ToList();
            BodyBadge = _bodies.Count == 0 ? null : new PaneBodyBadge(_bodies);
        }
        private IReadOnlyList<PaneCodeBody> _bodies = new PaneCodeBody[0];
        public IReadOnlyList<PaneCodeBody> Bodies
        {
            get => _bodies;
            set
            {
                _bodies = value;
                foreach (var finding in Findings)
                {
                    finding.Body = value.FirstOrDefault(body => ReferenceEquals(body.Definition, finding.Finding));
                    finding.IsProcedureCall = finding.Finding.Action == "EXEC" && value.Any(b => b.Kind == CodeBodyKind.Procedure) ||
                        finding.Finding.Action == "FUNCTION" && value.Any(b => b.Kind == CodeBodyKind.ScalarFunction ||
                            b.Kind == CodeBodyKind.InlineTableFunction || b.Kind == CodeBodyKind.TableFunction);
                }
                RefreshBodyBadge();
                foreach (var group in ScopeRows?.AllGroups ?? Enumerable.Empty<PaneGroup>()) group.RefreshBodyBadge();
            }
        }
        public bool HasBodies => Bodies.Count > 0;
        public PaneBodyBadge BodyBadge { get; private set; }
        public Visibility BodyBadgeVisibility => HasBodies ? Visibility.Visible : Visibility.Collapsed;
        public bool IsCodeContainer => Bodies.Any(body => body.IsDeferred);
        public Visibility DynamicScopeVisibility => ScopeRows != null ? Visibility.Visible : Visibility.Collapsed;
        public IReadOnlyList<PaneFinding> Findings { get; }
        public Verdict Verdict { get; }
        public int FirstOffset { get; }
        public string DatabasePrefix { get; }
        public string ObjectName { get; }
        public string DisplayName => IsDynamicContainer ? ObjectName : DatabasePrefix + ObjectName;
        public PaneVerdictCounts Counts { get; }
        // Submission and preview effects have separate, explicitly labelled badges.
        public PaneDisplayCounts DisplayCounts => new PaneDisplayCounts(Findings.Select(f => f.Finding));
        public Brush RedMarker => VerdictBrushes.Red;
        public Brush YellowMarker => VerdictBrushes.Yellow;
        public Brush GreenMarker => VerdictBrushes.Green;
        public Visibility RedVisibility => DisplayCounts.Writes > 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility YellowVisibility => DisplayCounts.Unknown > 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility GreenVisibility => DisplayCounts.Safe > 0 ? Visibility.Visible : Visibility.Collapsed;
        public string CountTooltip => DisplayCounts.Description +
            ". Includes hidden rows. Inside and Remote previews are counted separately.";
        public PaneVerdictFilter Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                if (ScopeRows != null) ScopeRows.ApplyFilter(value);
                else foreach (var finding in Findings) finding.ApplyFilter(value);
                foreach (string property in new[] { nameof(VisibleFindings), nameof(VisibleRows), nameof(VisibleLine), nameof(VisibleSummary) })
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }
        public IReadOnlyList<PaneFinding> VisibleFindings => Findings.Where(f => f.IsVisible).ToList();
        // Prefer submitted matches on object selection. Descendants are used only when the
        // containing operation is present solely to reach a matching preview.
        public IEnumerable<PaneFinding> MatchingOperations => VisibleFindings.SelectMany(f =>
            f.MatchesFilter ? new[] { f } : f.MatchingOperations);
        public int VisibleLine => IsNested ? MatchingOperations.FirstOrDefault()?.EditorLine ?? Findings[0].EditorLine
            : VisibleFindings.FirstOrDefault()?.ExecutionLine ?? Line;
        public string VisibleSummary => VisibleFindings.Count + " of " + Findings.Count + " operations";
        public string Chevron => IsExpanded ? "⌃" : "⌄";
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value) return;
                _isExpanded = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Chevron)));
            }
        }
        public Brush Marker => Findings.All(f => f.Finding.IsContextChange) ? VerdictBrushes.Context : VerdictBrushes.For(Verdict);
        public Brush ContextMarker => VerdictBrushes.Context;
        public Visibility ContextVisibility => DisplayCounts.Context > 0 ? Visibility.Visible : Visibility.Collapsed;
        public string ContextCount => DisplayCounts.Context.ToString();
        public string ContextTooltip => SafetyDictionary.Context;
        public bool IsAllowed => Verdict.IsGreen() && Findings.Any(finding =>
            finding.Finding.Verdict == Verdict.AllowListed ||
            (finding.Finding.Verdict == Verdict.ReadOnly && (finding.Finding.Note ?? string.Empty).IndexOf("trusted procedure", StringComparison.OrdinalIgnoreCase) >= 0));
        public string Section => Verdict.IsRed() ? "CHANGES TO PERMANENT OBJECTS" : Verdict.IsYellow() ? "CAN'T SEE INSIDE" :
            IsAllowed ? "ALLOWED BY YOUR SETTINGS" : "Safe";
        public bool IsWholeTarget => Findings.Any(finding => finding.Finding.Verdict.IsRed() && (finding.Finding.AffectsAllRows ||
            finding.Finding.Action.StartsWith("TRUNCATE", StringComparison.OrdinalIgnoreCase) ||
            finding.Finding.Action.StartsWith("DROP ", StringComparison.OrdinalIgnoreCase)));
        public string Reason => Verdict == Verdict.TempOnly ? "Temporary-object operations only. No permanent-object changes detected." :
            Findings.OrderByDescending(finding => finding.Finding.Verdict)
            .ThenByDescending(finding => finding.Finding.AffectsAllRows).Select(finding => finding.Note).FirstOrDefault(note => !string.IsNullOrEmpty(note)) ??
            (Verdict.IsRed() ? "Changes a permanent object." : Verdict.IsYellow() ? "The analyzer cannot see what runs." : "No permanent changes detected.");
        public string Subtitle
        {
            get
            {
                var actions = Findings.Select(finding => finding.Finding.Action).Distinct().ToList();
                return actions[0] + (actions.Count > 1 ? " +" + (actions.Count - 1) :
                    Findings.Count > 1 ? " ×" + Findings.Count : string.Empty);
            }
        }
        public int Line => Findings[0].ExecutionLine;
        public string OperationSummary => string.Join(" · ", Findings.GroupBy(finding => finding.Finding.Action)
            .Select(action => action.Key + (action.Count() > 1 ? " ×" + action.Count() : string.Empty)));
        public string LineSummary => "Lines " + string.Join(", ", Findings.Select(finding => finding.EditorLine).Distinct());
        public Visibility MultipleFindingsVisibility => Findings.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility SingleFindingVisibility => Findings.Count == 1 ? Visibility.Visible : Visibility.Collapsed;
        public string Tooltip => DisplayName + (IsCodeContainer ? Environment.NewLine +
            "Contains code for later execution. Separate preview counts describe its inner operations; hover those counts for context." : string.Empty) +
            Environment.NewLine + CountTooltip + Environment.NewLine + string.Join(Environment.NewLine,
            Findings.Select(finding => finding.Header + (string.IsNullOrEmpty(finding.Note) ? string.Empty : " · " + finding.Note)));
        public override string ToString() => Key;

        internal static IReadOnlyList<PaneGroup> Create(IEnumerable<PaneFinding> findings) => findings
            .GroupBy(f => PaneOperationPresentation.IsDynamic(f.Finding) ? PaneOperationPresentation.DynamicKey(f.Finding) : f.GroupKey,
                StringComparer.OrdinalIgnoreCase)
            .Select(g => new PaneGroup(g.Key, g.ToList()))
            .OrderByDescending(g => g.Verdict).ThenBy(g => g.FirstOffset).ToList();

        /// <summary>Normalizes a target to [database].[schema].[name] so the same object groups together however it was written.
        /// #temp tables and @table variables are left as written: they don't belong to the current database.</summary>
        internal static string Qualify(string target, string database)
        {
            var parts = SplitName(target);
            string last = Unquote(parts[parts.Count - 1]);
            if (last.StartsWith("#", StringComparison.Ordinal) || last.StartsWith("@", StringComparison.Ordinal))
                return target;
            if (parts.Count < 3 && !string.IsNullOrEmpty(database))
            {
                if (parts.Count == 1) parts.Insert(0, string.Empty);
                parts.Insert(0, "[" + database.Replace("]", "]]") + "]");
            }
            return string.Join(".", parts.Select(part =>
            {
                string value = Unquote(part);
                return value.Length == 0 ? string.Empty : "[" + value.Replace("]", "]]") + "]";
            }));
        }

        /// <summary>One name part without [brackets] or "quotes", for display.</summary>
        private static string Unquote(string part)
        {
            string value = part.Trim();
            if (value.StartsWith("[", StringComparison.Ordinal) && value.EndsWith("]", StringComparison.Ordinal))
                return value.Substring(1, value.Length - 2).Replace("]]", "]");
            if (value.Length > 1 && value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal))
                return value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
            return value;
        }

        internal static string ObjectDisplayName(string target) =>
            string.Join(".", SplitName(target).Select(Unquote).Reverse().Take(2).Reverse());

        private static List<string> SplitName(string name)
        {
            var parts = new List<string>();
            int start = 0;
            char quote = '\0';
            for (int index = 0; index < name.Length; index++)
            {
                char character = name[index];
                if (quote != '\0')
                {
                    if (character != quote) continue;
                    if (index + 1 < name.Length && name[index + 1] == quote) { index++; continue; }
                    quote = '\0';
                }
                else if (character == '[' || character == '"') quote = character == '[' ? ']' : '"';
                else if (character == '.') { parts.Add(name.Substring(start, index - start)); start = index + 1; }
            }
            parts.Add(name.Substring(start));
            return parts;
        }
    }

    /// <summary>Display-only rows within one body or decoded EXEC. Keeps the original findings intact.</summary>
    internal sealed class PaneScopeRows
    {
        public PaneScopeRows(IReadOnlyList<PaneFinding> findings, bool includeDynamicContainers = true)
        {
            var buckets = new Dictionary<string, List<PaneFinding>>(StringComparer.OrdinalIgnoreCase);
            var rows = new List<object>();
            foreach (var finding in findings.OrderBy(f => f.Finding.StartOffset))
            {
                bool dynamic = includeDynamicContainers && PaneOperationPresentation.IsDynamic(finding.Finding);
                string target = finding.Finding.ResolvedTarget ?? finding.Finding.Target;
                if (!dynamic && (finding.Finding.IsContextChange || string.IsNullOrEmpty(target) || finding.Body?.Kind == CodeBodyKind.RemoteQuery))
                {
                    rows.Add(finding);
                    continue;
                }
                string key = dynamic ? PaneOperationPresentation.DynamicKey(finding.Finding) : ObjectKey(finding);
                string bucketKey = (dynamic ? "dynamic|" : "object|") + key;
                if (!buckets.TryGetValue(bucketKey, out var bucket))
                {
                    bucket = new List<PaneFinding>();
                    buckets.Add(bucketKey, bucket);
                    rows.Add(bucket);
                }
                bucket.Add(finding);
            }
            Rows = rows.Select(row => row is List<PaneFinding> bucket
                ? (object)new PaneGroup(includeDynamicContainers && PaneOperationPresentation.IsDynamic(bucket[0].Finding)
                    ? PaneOperationPresentation.DynamicKey(bucket[0].Finding) : ObjectKey(bucket[0]), bucket,
                    scopeObject: !(includeDynamicContainers && PaneOperationPresentation.IsDynamic(bucket[0].Finding)), nested: true)
                : row).ToList();
        }
        public IReadOnlyList<object> Rows { get; }
        // A visible body or remote query does not establish the caller's database/default schema.
        // Normalize the analyzer's identified target without borrowing the editor connection's database.
        private static string ObjectKey(PaneFinding finding) => PaneGroup.Qualify(finding.Finding.ResolvedTarget ?? finding.Finding.Target, null);
        public IReadOnlyList<PaneGroup> Groups => Rows.OfType<PaneGroup>().ToList();
        public IReadOnlyList<object> VisibleRows => Rows.Where(row => row is PaneGroup group
            ? group.VisibleFindings.Count > 0 : ((PaneFinding)row).IsVisible).ToList();
        public IEnumerable<PaneGroup> AllGroups => Rows.SelectMany(row => row is PaneGroup group
            ? group.DescendantsAndSelf() : ((PaneFinding)row).Body?.ScopeRows.AllGroups ?? Enumerable.Empty<PaneGroup>());
        public void ApplyFilter(PaneVerdictFilter filter)
        {
            foreach (var row in Rows)
                if (row is PaneGroup group) group.Filter = filter;
                else ((PaneFinding)row).ApplyFilter(filter);
        }
    }

    internal sealed class PaneCodeBody : INotifyPropertyChanged
    {
        private PaneVerdictFilter _filter = PaneVerdictFilter.All;
        private bool _isExpanded = true;
        public event PropertyChangedEventHandler PropertyChanged;
        public PaneFinding Parent { get; set; }
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded))); PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Chevron))); }
        }
        public string Chevron => IsExpanded ? "\u2303" : "\u2304";
        public string PreviewLabel => IsDeferred ? "Inside" : "Remote";
        public Brush Marker => IsDeferred ? VerdictBrushes.CodeContainer : VerdictBrushes.Remote;
        public Brush ContextMarker => VerdictBrushes.Context;
        public Visibility ContextVisibility => DisplayCounts.Context > 0 ? Visibility.Visible : Visibility.Collapsed;
        public string Tooltip => ContextDescription + Environment.NewLine + Summary;
        public bool IsVisible => (_filter & (IsDeferred ? PaneVerdictFilter.Inside : PaneVerdictFilter.Remote)) != 0 &&
            (VisibleFindings.Count > 0 || Findings.Count == 0 && PaneFilterRules.Includes(_filter, Verdict.Unknown));
        public IReadOnlyList<PaneFinding> VisibleFindings => Findings.Where(f => f.IsVisible).ToList();
        public PaneScopeRows ScopeRows { get; }
        public IReadOnlyList<object> VisibleRows => ScopeRows.VisibleRows;
        public void ApplyFilter(PaneVerdictFilter filter)
        {
            _filter = filter;
            ScopeRows.ApplyFilter(filter);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibleFindings)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibleRows)));
        }
        public PaneCodeBody(CodeBodyAnalysis body, ITextSnapshot snapshot, int baseOffset, string database)
        {
            Definition = body.Definition;
            Kind = body.Kind;
            IsEmbedded = body.IsEmbedded;
            OperationLabel = body.OperationLabel;
            ContextLabel = body.ContextLabel + " \u00b7 preview";
            ContextDescription = body.ContextDescription;
            IsComplete = body.IsComplete;
            Findings = body.Analysis.Findings.Select((f, index) => new PaneFinding(f, snapshot, baseOffset, database, index) { IsPreviewOperation = true }).ToList();
            foreach (var finding in Findings)
            {
                var nested = body.Analysis.CodeBodies.FirstOrDefault(b => ReferenceEquals(b.Definition, finding.Finding));
                if (nested != null) finding.Body = new PaneCodeBody(nested, snapshot, baseOffset, database);
                if (body.Kind == CodeBodyKind.JobCommand) finding.ExecutionLocationLabel = "Stored at";
                else if (body.Kind == CodeBodyKind.RemoteQuery) finding.ExecutionLocationLabel = "Sent at";
            }
            // Embedded previews already represent the decoded string's execution scope.
            // Their provenance notes also say "inside dynamic SQL"; do not add a second container.
            ScopeRows = new PaneScopeRows(Findings, includeDynamicContainers: !IsEmbedded);
            Counts = new PaneVerdictCounts(Findings.Select(f => f.Finding.Verdict));
            Summary = (IsComplete ? string.Empty : "Body analysis incomplete. ") +
                DisplayCounts.Description + ". Body preview, including nested operations; separate from execution totals.";
            Badge = new PaneBodyBadge(new[] { this });
        }

        public CodeBodyKind Kind { get; }
        public bool IsEmbedded { get; }
        public bool IsDeferred => Kind != CodeBodyKind.RemoteQuery;
        public PaneBodyBadge Badge { get; }
        public string OperationLabel { get; }
        public string ContextLabel { get; }
        public string ContextDescription { get; }
        public Finding Definition { get; }
        public IReadOnlyList<PaneFinding> Findings { get; }
        public PaneVerdictCounts Counts { get; }
        public PaneDisplayCounts DisplayCounts => new PaneDisplayCounts(Findings
            .SelectMany(f => f.DescendantsAndSelf()).Select(f => f.Finding));
        public bool IsComplete { get; }
        public string Summary { get; }
        public Brush RedMarker => VerdictBrushes.Red;
        public Brush YellowMarker => VerdictBrushes.Yellow;
        public Brush GreenMarker => VerdictBrushes.Green;
        public Visibility RedVisibility => DisplayCounts.Writes > 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility YellowVisibility => DisplayCounts.Unknown > 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility GreenVisibility => DisplayCounts.Safe > 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IncompleteVisibility => IsComplete ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <summary>Explicitly scoped preview counts; never used by the guard or execution filters.</summary>
    internal sealed class PaneBodyBadge
    {
        public PaneBodyBadge(IEnumerable<PaneCodeBody> bodies)
        {
            var values = bodies.ToArray();
            bool deferred = values.Any(body => body.IsDeferred);
            Context = deferred ? "Body" : "Remote";
            Counts = new PaneDisplayCounts(values.SelectMany(body => body.Findings)
                .SelectMany(f => f.DescendantsAndSelf()).Select(f => f.Finding));
            InsideTotal = values.Where(b => b.IsDeferred).Sum(b => b.DisplayCounts.Total);
            RemoteTotal = values.Where(b => !b.IsDeferred).Sum(b => b.DisplayCounts.Total);
            Tooltip = Context + " preview: " + string.Join(Environment.NewLine, values.Select(body => body.ContextDescription).Distinct()) +
                Environment.NewLine + Counts.Description + ". Preview only; separate from submitted-operation counts.";
        }
        public string Context { get; }
        public int InsideTotal { get; }
        public int RemoteTotal { get; }
        public Brush InsideMarker => VerdictBrushes.CodeContainer;
        public Brush RemoteMarker => VerdictBrushes.Remote;
        public Visibility InsideVisibility => InsideTotal > 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility RemoteVisibility => RemoteTotal > 0 ? Visibility.Visible : Visibility.Collapsed;
        public PaneDisplayCounts Counts { get; }
        public string Tooltip { get; }
        public Brush RedMarker => VerdictBrushes.Red;
        public Brush YellowMarker => VerdictBrushes.Yellow;
        public Brush GreenMarker => VerdictBrushes.Green;
        public Visibility RedVisibility => Counts.Writes > 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility YellowVisibility => Counts.Unknown > 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility GreenVisibility => Counts.Safe > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    internal sealed class PanePattern
    {
        public PanePattern(string pattern, string source) { Pattern = pattern; Source = source; }
        public string Pattern { get; }
        public string Source { get; }
    }

    internal sealed class PaneSettings
    {
        public PaneSettings(string server, string database, GuardSettings settings, AnalyzerOptions options,
            int globalTables, int globalProcedures)
        {
            Server = server ?? "Unknown";
            Database = database ?? "Unknown";
            EnvironmentLabel = settings.MatchedRule == null ? null : settings.Environment;
            EnvironmentDescription = EnvironmentLabel == null ? "global settings" : EnvironmentLabel + " rule · " + settings.MatchedRule;
            WhenToAsk = settings.Mode == ExecutionMode.Off ? "Off" : settings.Mode == ExecutionMode.Red ? "Red only" : "Red and yellow";
            TypedConfirmation = "No typing required";
            Tempdb = options.TempdbIsTemp ? "Treated as temp" : "Not treated as temp";
            SafeTables = options.AllowList.Patterns.Select((pattern, index) => new PanePattern(pattern,
                index < globalTables ? "global" : EnvironmentLabel + " rule")).ToList();
            TrustedProcedures = options.TrustedProcedures.Patterns.Select((pattern, index) => new PanePattern(pattern,
                index < globalProcedures ? "global" : EnvironmentLabel + " rule")).ToList();
        }
        public string Server { get; }
        public string Database { get; }
        public string EnvironmentLabel { get; }
        public string EnvironmentDescription { get; }
        public string WhenToAsk { get; }
        public string TypedConfirmation { get; }
        public string Tempdb { get; }
        public IReadOnlyList<PanePattern> SafeTables { get; }
        public IReadOnlyList<PanePattern> TrustedProcedures { get; }
        public string SafeTablesHeading => "Safe tables " + SafeTables.Count;
        public string TrustedProceduresHeading => "Trusted routines " + TrustedProcedures.Count;
    }
}
