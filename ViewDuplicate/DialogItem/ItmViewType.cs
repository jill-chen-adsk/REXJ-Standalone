using Autodesk.Revit.DB;
using R = ADSK.ViewExtension.ViewDuplicate.Resources;

namespace ADSK.ViewExtension.ViewDuplicate.DialogItem
{
    public class ItmViewType
    {
        private readonly bool _allTypes;
        private ViewType _viewType;

        public ItmViewType(ViewType thisViewType)
        {
            _viewType = thisViewType;
            _allTypes = false;
        }

        public ItmViewType()
        {
            _allTypes = true;
            _viewType = default;
        }

        public ViewType ViewType
        {
            get => _viewType;
            set => _viewType = value;
        }

        public string Name
        {
            get
            {
                if (_allTypes)
                    return R.Text.TXT_VIEWTYPE_ALL;

                switch (_viewType)
                {
                    case ViewType.AreaPlan:
                        return R.Text.TXT_VIEWTYPE_AREA_PLAN;
                    case ViewType.CeilingPlan:
                        return R.Text.TXT_VIEWTYPE_CEILING_PLAN;
                    case ViewType.ColumnSchedule:
                        return R.Text.TXT_VIEWTYPE_COLUMN_SCHEDULE;
                    case ViewType.CostReport:
                        return R.Text.TXT_VIEWTYPE_COST_REPORT;
                    case ViewType.Detail:
                        return R.Text.TXT_VIEWTYPE_DETAIL;
                    case ViewType.DraftingView:
                        return R.Text.TXT_VIEWTYPE_DRAFTING_VIEW;
                    case ViewType.DrawingSheet:
                        return R.Text.TXT_VIEWTYPE_DRAWING_SHEET;
                    case ViewType.Elevation:
                        return R.Text.TXT_VIEWTYPE_ELEVATION;
                    case ViewType.EngineeringPlan:
                        return R.Text.TXT_VIEWTYPE_ENGINEERING_PLAN;
                    case ViewType.FloorPlan:
                        return R.Text.TXT_VIEWTYPE_FLOOR_PLAN;
                    case ViewType.Internal:
                        return R.Text.TXT_VIEWTYPE_INTERNAL;
                    case ViewType.Legend:
                        return R.Text.TXT_VIEWTYPE_LEGEND;
                    case ViewType.LoadsReport:
                        return R.Text.TXT_VIEWTYPE_LOADS_REPORT;
                    case ViewType.PanelSchedule:
                        return R.Text.TXT_VIEWTYPE_PANEL_SCHEDULE;
                    case ViewType.PressureLossReport:
                        return R.Text.TXT_VIEWTYPE_PRESSURE_LOSS_REPORT;
                    case ViewType.ProjectBrowser:
                        return R.Text.TXT_VIEWTYPE_PROJECT_BROWSER;
                    case ViewType.Rendering:
                        return R.Text.TXT_VIEWTYPE_RENDERING;
                    case ViewType.Report:
                        return R.Text.TXT_VIEWTYPE_REPORT;
                    case ViewType.Section:
                        return R.Text.TXT_VIEWTYPE_SECTION;
                    case ViewType.Schedule:
                        return R.Text.TXT_VIEWTYPE_SCHEDULE;
                    case ViewType.SystemBrowser:
                        return R.Text.TXT_VIEWTYPE_SYSTEM_BROWSER;
                    case ViewType.ThreeD:
                        return R.Text.TXT_VIEWTYPE_THREE_D;
                    case ViewType.Undefined:
                        return R.Text.TXT_VIEWTYPE_UNDEFINED;
                    case ViewType.Walkthrough:
                        return R.Text.TXT_VIEWTYPE_WALKTHROUGH;
                    default:
                        return string.Empty;
                }
            }
        }

        public override string ToString() => Name;
    }
}
