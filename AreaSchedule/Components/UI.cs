using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AreaSchedule.Components
{
    public class UI
    {
        public UIApplication RvtUIApp { get; }

        public UI(UIApplication rvtUIApp)
        {
            RvtUIApp = rvtUIApp;
        }
    }
}
