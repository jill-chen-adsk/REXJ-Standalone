using ADSK.ViewExtension.SheetLayout.Resources;
using Autodesk.Revit.DB;

namespace ADSK.ViewExtension.SheetLayout.DialogItem
{
    public class ItmViewType
    {
        private readonly int _typeCode;

        public ItmViewType(ViewType thisViewType)
        {
            _typeCode = (int)thisViewType;
        }

        public ItmViewType()
        {
            _typeCode = -1;
        }

        public ViewType ViewType => (ViewType)_typeCode;

        public string Name
        {
            get
            {
                switch (_typeCode)
                {
                    case -1:
                        return Text.TXT_VT_ALL;
                    case (int)ViewType.AreaPlan:
                        return Text.TXT_VT_AREA_PLAN;
                    case (int)ViewType.CeilingPlan:
                        return Text.TXT_VT_CEILING_PLAN;
                    case (int)ViewType.ColumnSchedule:
                        return Text.TXT_VT_COLUMN_SCHEDULE;
                    case (int)ViewType.CostReport:
                        return Text.TXT_VT_COST_REPORT;
                    case (int)ViewType.Detail:
                        return Text.TXT_VT_DETAIL;
                    case (int)ViewType.DraftingView:
                        return Text.TXT_VT_DRAFTING_VIEW;
                    case (int)ViewType.DrawingSheet:
                        return Text.TXT_VT_DRAWING_SHEET;
                    case (int)ViewType.Elevation:
                        return Text.TXT_VT_ELEVATION;
                    case (int)ViewType.EngineeringPlan:
                        return Text.TXT_VT_ENGINEERING_PLAN;
                    case (int)ViewType.FloorPlan:
                        return Text.TXT_VT_FLOOR_PLAN;
                    case (int)ViewType.Internal:
                        return Text.TXT_VT_INTERNAL;
                    case (int)ViewType.Legend:
                        return Text.TXT_VT_LEGEND;
                    case (int)ViewType.LoadsReport:
                        return Text.TXT_VT_LOADS_REPORT;
                    case (int)ViewType.PanelSchedule:
                        return Text.TXT_VT_PANEL_SCHEDULE;
                    case (int)ViewType.PressureLossReport:
                        return Text.TXT_VT_PRESSURE_LOSS_REPORT;
                    case (int)ViewType.ProjectBrowser:
                        return Text.TXT_VT_PROJECT_BROWSER;
                    case (int)ViewType.Rendering:
                        return Text.TXT_VT_RENDERING;
                    case (int)ViewType.Report:
                        return Text.TXT_VT_REPORT;
                    case (int)ViewType.Section:
                        return Text.TXT_VT_SECTION;
                    case (int)ViewType.Schedule:
                        return Text.TXT_VT_SCHEDULE;
                    case (int)ViewType.SystemBrowser:
                        return Text.TXT_VT_SYSTEM_BROWSER;
                    case (int)ViewType.ThreeD:
                        return Text.TXT_VT_THREE_D;
                    case (int)ViewType.Undefined:
                        return Text.TXT_VT_UNDEFINED;
                    case (int)ViewType.Walkthrough:
                        return Text.TXT_VT_WALKTHROUGH;
                    default:
                        return string.Empty;
                }
            }
        }

        public override string ToString() => Name;
    }
}
