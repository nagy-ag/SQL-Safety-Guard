using System;

namespace SqlSafetyGuard.Analyzer
{
    public static class GuardDecision
    {
        public static bool CancelAfterFailure(GuardSettings settings, AnalysisResult result, bool executionTargetVerified = true) =>
            !executionTargetVerified || (result == null ? settings?.Mode != ExecutionMode.Off :
                (settings?.ShouldWarn(result) ?? !result.IsGreen));

        public static bool ShouldCancelAfterFailure(GuardSettings settings, AnalysisResult result, bool executionTargetVerified,
            Func<bool> confirmFailure, Action<Exception> reportFailure)
        {
            // A computed warning cannot become permission to run because later host work failed.
            if (result != null) return CancelAfterFailure(settings, result, executionTargetVerified);
            if (!CancelAfterFailure(settings, null, executionTargetVerified)) return false;
            try { return !confirmFailure(); }
            catch (Exception exception) { Report(reportFailure, exception); return true; }
        }

        public static bool ShouldCancel(GuardSettings settings, AnalysisResult result, Action present, Func<bool> confirm,
            Action<Exception> reportFailure, bool executionTargetVerified = true)
        {
            bool warn = !executionTargetVerified || settings.ShouldWarn(result);
            try { present(); }
            catch (Exception exception) { Report(reportFailure, exception); }
            if (!warn) return false;
            try { return !confirm(); }
            catch (Exception exception) { Report(reportFailure, exception); return true; }
        }

        private static void Report(Action<Exception> reportFailure, Exception exception)
        {
            try { reportFailure?.Invoke(exception); }
            catch { }
        }
    }
}
