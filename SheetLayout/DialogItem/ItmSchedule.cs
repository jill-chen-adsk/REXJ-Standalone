using Autodesk.Revit.DB;

namespace ADSK.ViewExtension.SheetLayout.DialogItem
{
    public class ItmSchedule
    {
        private ViewSchedule m_Sch;

        public ItmSchedule(ScheduleSheetInstance schInstance)
        {
            Element elm = schInstance.Document.GetElement(schInstance.ScheduleId);
            m_Sch = (ViewSchedule)elm;
        }

        public ItmSchedule(ViewSchedule sch)
        {
            m_Sch = sch;
        }

        public ViewSchedule Sch
        {
            get => m_Sch;
            set => m_Sch = value;
        }

        public string Name => m_Sch.Name;

        public ElementId Id => m_Sch.Id;

        public override string ToString() => m_Sch.Name;
    }
}
