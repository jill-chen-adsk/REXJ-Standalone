using Autodesk.Revit.DB;

namespace ADSK.ViewExtension.TenkaiView.DialogItem
{
    public class ItmDimStyle
    {
        private readonly DimensionType m_DimStyle;

        public ItmDimStyle(DimensionType dType)
        {
            m_DimStyle = dType;
        }

        public string DimTypeName => m_DimStyle.Name;

        public ElementId Id => m_DimStyle.Id;

        public override string ToString() => m_DimStyle.Name;
    }
}
