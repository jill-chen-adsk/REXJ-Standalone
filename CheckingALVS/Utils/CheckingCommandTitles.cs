using RvtExtApp = ADSK.JExtRAC.CheckingALVS;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
    public static class CheckingCommandTitles
    {
        public static string GetCommandTitle(RvtExtApp.Components.Attribute attribute, int commandKind)
        {
            switch (commandKind)
            {
                case 0:
                    return attribute.ResourceText("IDS_TXT_CMD_DAYLIGHTCHECK");
                case 1:
                    return attribute.ResourceText("IDS_TXT_CMD_SMOKEEXHAUST");
                case 2:
                    return attribute.ResourceText("IDS_TXT_CMD_VENTILATIONCHECK");
                default:
                    return string.Empty;
            }
        }
    }
}
