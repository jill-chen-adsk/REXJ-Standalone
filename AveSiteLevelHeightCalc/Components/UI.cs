using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Components
{
    /// <summary>Placeholder for parity with legacy Rvt.Com UI wrapper.</summary>
    public class UI
    {
        public UI(UIApplication rvtUiApp)
        {
            UiApp = rvtUiApp;
        }

        public UIApplication UiApp { get; }
    }
}
