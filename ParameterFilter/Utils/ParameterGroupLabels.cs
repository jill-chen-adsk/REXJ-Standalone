using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.ParameterFilter.Utils
{
    internal static class ParameterGroupLabels
    {
        public static string GetLabel(ForgeTypeId groupTypeId)
        {
            if (groupTypeId == null || string.IsNullOrEmpty(groupTypeId.TypeId))
                return string.Empty;

            try
            {
                return LabelUtils.GetLabelForGroup(groupTypeId);
            }
            catch
            {
                return groupTypeId.TypeId;
            }
        }
    }
}
