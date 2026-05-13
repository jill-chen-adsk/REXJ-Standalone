using Autodesk.Revit.DB;

namespace ADSK.ViewExtension.SheetLayout.DialogItem
{
    public class ItmViewPort
    {
        private Viewport m_ViewPort;

        public ItmViewPort(Viewport viewPort1)
        {
            m_ViewPort = viewPort1;
        }

        public Viewport ViewPort
        {
            get => m_ViewPort;
            set => m_ViewPort = value;
        }

        public string ViewPortNumber
        {
            get
            {
                Parameter prmNum = m_ViewPort.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER);
                return prmNum.AsString();
            }
            set
            {
                Parameter prmNum = m_ViewPort.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER);
                prmNum.Set(value);
            }
        }

        public string SheetTitle
        {
            get
            {
                Parameter prmSheetTitle = m_ViewPort.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION);
                string strSheetTitle = prmSheetTitle.AsString();
                if (string.IsNullOrEmpty(strSheetTitle))
                {
                    Parameter prmViewName = m_ViewPort.get_Parameter(BuiltInParameter.VIEWPORT_VIEW_NAME);
                    strSheetTitle = prmViewName.AsString();
                }
                return strSheetTitle;
            }
        }

        public override string ToString() => ViewPortNumber + ":" + SheetTitle;
    }
}
