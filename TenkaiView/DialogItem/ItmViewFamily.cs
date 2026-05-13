using Autodesk.Revit.DB;

namespace ADSK.ViewExtension.TenkaiView.DialogItem
{
    public class ItmViewFamily
    {
        private ViewFamilyType m_ViewFamilyType;

        public ViewFamilyType VfType
        {
            get => m_ViewFamilyType;
            set => m_ViewFamilyType = value;
        }

        public ItmViewFamily(ViewFamilyType vft)
        {
            m_ViewFamilyType = vft;
        }

        public override string ToString() => m_ViewFamilyType.Name;
    }
}
