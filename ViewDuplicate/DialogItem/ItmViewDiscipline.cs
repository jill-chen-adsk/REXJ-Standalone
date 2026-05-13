using Autodesk.Revit.DB;
using R = ADSK.ViewExtension.ViewDuplicate.Resources;

namespace ADSK.ViewExtension.ViewDuplicate.DialogItem
{
    public class ItmViewDiscipline
    {
        private ViewDiscipline _discipline;

        public ItmViewDiscipline(ViewDiscipline thisDiscipline)
        {
            _discipline = thisDiscipline;
        }

        public string Name
        {
            get
            {
                switch (_discipline)
                {
                    case ViewDiscipline.Architectural:
                        return R.Text.TXT_VIEWDISCIPLINE_ARCHITECTURAL;
                    case ViewDiscipline.Coordination:
                        return R.Text.TXT_VIEWDISCIPLINE_COORDINATION;
                    case ViewDiscipline.Mechanical:
                        return R.Text.TXT_VIEWDISCIPLINE_MECHANICAL;
                    case ViewDiscipline.Plumbing:
                        return R.Text.TXT_VIEWDISCIPLINE_PLUMBING;
                    case ViewDiscipline.Electrical:
                        return R.Text.TXT_VIEWDISCIPLINE_ELECTRICAL;
                    case ViewDiscipline.Structural:
                        return R.Text.TXT_VIEWDISCIPLINE_STRUCTURAL;
                    default:
                        return string.Empty;
                }
            }
        }

        public ViewDiscipline Discipline
        {
            get => _discipline;
            set => _discipline = value;
        }

        public override string ToString() => Name;
    }
}
