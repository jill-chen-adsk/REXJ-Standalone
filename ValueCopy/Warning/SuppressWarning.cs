using ADSK.JExtRAC.ValueCopy.Commands;
using Autodesk.Revit.DB;
using System.Linq;
using System.Text;

namespace ADSK.JExtRAC.ValueCopy.Warning
{
    /// ================================================================================
    /// <summary>Class SuppressWarning</summary>
    /// ================================================================================
    internal class SuppressWarning : IFailuresPreprocessor
    {
        // Member Variables

        #region Member Variables

        /// <summary>Copy has error or not</summary>
        public static bool _IsHasError = false;

        #endregion Member Variables

        // Member Functions

        #region Member Functions

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="failuresAccessor">FailuresAccessor</param>
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        FailureProcessingResult IFailuresPreprocessor.PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            var failureMessageAccessors = failuresAccessor.GetFailureMessages();

            if (failureMessageAccessors.Count == 0)
                return FailureProcessingResult.Continue;

            var currentDoc = failuresAccessor.GetDocument();
            if (currentDoc == null)
                return FailureProcessingResult.ProceedWithRollBack;

            string categoryName = "なし";
            string familyName = "なし";
            string typeName = "なし";

            foreach (FailureMessageAccessor failureMessageAccessor in failureMessageAccessors)
            {
                // Get description of text mess error
                CmdCopyParameter.errorMess.AppendLine("-----------------------");
                CmdCopyParameter.errorMess.AppendLine(failureMessageAccessor.GetDescriptionText());

                var lstElementId = failureMessageAccessor.GetFailingElementIds();
                foreach (var id in lstElementId)
                {
                    var element = currentDoc.GetElement(id);
                    if (element == null)
                        continue;

                    if (element.Category != null)
                        categoryName = element.Category.Name;

                    var typeElement = currentDoc.GetElement(element.GetTypeId()) as ElementType;
                    if (typeElement != null)
                    {
                        familyName = typeElement.FamilyName;
                        typeName = typeElement.Name;
                    }

                    CmdCopyParameter.errorMess.AppendLine("\t" + categoryName + ": " + familyName + ": " + typeName + ": " + element.Name + " [ID: " + id.ToString() + "]");
                }

                CmdCopyParameter.errorMess.AppendLine("-----------------------");
            }

            _IsHasError = true;

            return FailureProcessingResult.ProceedWithRollBack;
        }

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="doc">Current document</param>
        /// <param name="CurrentElementError">Current Element</param>
        /// <param name="errorMess">Error mess</param>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public static bool AppendError(Document doc, Element CurrentElementError, string errorMess)
        {
            string categoryName = "なし";
            string familyName = "なし";
            string typeName = "なし";

            _IsHasError = true;
            if (CurrentElementError == null)
                return false;

            if (CurrentElementError.Category != null)
                categoryName = CurrentElementError.Category.Name;

            var typeElement = doc.GetElement(CurrentElementError.GetTypeId()) as ElementType;
            if (typeElement != null)
            {
                familyName = typeElement.FamilyName;
                typeName = typeElement.Name;
            }

            // Get description of text mess error
            CmdCopyParameter.errorMess.AppendLine("-----------------------");
            CmdCopyParameter.errorMess.AppendLine(errorMess);
            CmdCopyParameter.errorMess.AppendLine("\t" + categoryName + ": " + familyName + ": " + typeName + ": " + CurrentElementError.Name
                                                    + " [ID: " + CurrentElementError.Id.ToString() + "]");
            CmdCopyParameter.errorMess.AppendLine("-----------------------");

            return true;
        }

        #endregion Member Functions
    }
}