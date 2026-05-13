using ADSK.ViewExtension.SheetLayout.Resources;
using Autodesk.Revit.DB;

namespace ADSK.ViewExtension.SheetLayout.DialogItem
{
    public class ItmViewFamilyType
    {
        private ViewFamilyType m_ViewfamilyType;

        public ItmViewFamilyType(ViewFamilyType myVft)
        {
            m_ViewfamilyType = myVft;
        }

        public ItmViewFamilyType()
        {
            m_ViewfamilyType = null;
        }

        public ViewFamilyType ViewfamilyType
        {
            get => m_ViewfamilyType;
            set => m_ViewfamilyType = value;
        }

        public override string ToString()
        {
            if (m_ViewfamilyType == null)
                return Text.TXT_ALL;
            return m_ViewfamilyType.Name;
        }

        public ElementId AppliedViewTemplateId
        {
            get
            {
                if (m_ViewfamilyType == null)
                    return ElementId.InvalidElementId;
                try
                {
                    Parameter prmDefVt = m_ViewfamilyType.get_Parameter(BuiltInParameter.DEFAULT_VIEW_TEMPLATE);
                    if (prmDefVt != null && prmDefVt.HasValue)
                        return prmDefVt.AsElementId();
                    return ElementId.InvalidElementId;
                }
                catch
                {
                    return ElementId.InvalidElementId;
                }
            }
        }
    }
}
