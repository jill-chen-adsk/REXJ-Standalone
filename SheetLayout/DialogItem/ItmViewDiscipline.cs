using ADSK.ViewExtension.SheetLayout.Resources;
using Autodesk.Revit.DB;

namespace ADSK.ViewExtension.SheetLayout.DialogItem
{
    public class ItmViewDiscipline
    {
        private ViewDiscipline m_Discipline;

        public ItmViewDiscipline(ViewDiscipline thisDiscipline)
        {
            m_Discipline = thisDiscipline;
        }

        public string Name
        {
            get
            {
                switch (m_Discipline)
                {
                    case ViewDiscipline.Architectural:
                        return Text.TXT_VIEWDISCIPLINE_ARCHITECTURAL;
                    case ViewDiscipline.Coordination:
                        return Text.TXT_VIEWDISCIPLINE_COORDINATION;
                    case ViewDiscipline.Mechanical:
                        return Text.TXT_VIEWDISCIPLINE_MECHANICAL;
                    case ViewDiscipline.Plumbing:
                        return Text.TXT_VIEWDISCIPLINE_PLUMBING;
                    case ViewDiscipline.Electrical:
                        return Text.TXT_VIEWDISCIPLINE_ELECTRICAL;
                    case ViewDiscipline.Structural:
                        return Text.TXT_VIEWDISCIPLINE_STRUCTURAL;
                    default:
                        return string.Empty;
                }
            }
        }

        public ViewDiscipline Discipline
        {
            get => m_Discipline;
            set => m_Discipline = value;
        }

        public override string ToString() => Name;
    }
}
