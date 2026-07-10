using System;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.AreaSchedule.Utils
{
    /// <summary>
    /// Suppresses benign area-scheme overlap warnings when copying room boundaries
    /// in bulk. Shared walls and fragmented room segments often trigger Revit's
    /// "Highlighted lines overlap" dialog even though the boundaries are acceptable.
    /// </summary>
    internal sealed class AreaBoundaryFailuresPreprocessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            foreach (FailureMessageAccessor failure in failuresAccessor.GetFailureMessages())
            {
                if (failure.GetSeverity() != FailureSeverity.Warning)
                    continue;

                string description = failure.GetDescriptionText() ?? string.Empty;
                if (IsBenignAreaBoundaryOverlapWarning(description))
                    failuresAccessor.DeleteWarning(failure);
            }

            return FailureProcessingResult.Continue;
        }

        private static bool IsBenignAreaBoundaryOverlapWarning(string description)
        {
            if (string.IsNullOrEmpty(description))
                return false;

            return description.IndexOf("overlap", StringComparison.OrdinalIgnoreCase) >= 0
                || description.IndexOf("closed loop", StringComparison.OrdinalIgnoreCase) >= 0
                || description.IndexOf("重複", StringComparison.OrdinalIgnoreCase) >= 0
                || description.IndexOf("閉じ", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
