using RvtExtApp = ADSK.JExtRAC.AreaSchedule;

namespace ADSK.JExtRAC.AreaSchedule.Utils
{
    public static class WeaveCommandTitles
    {
        public static string RoomToArea(RvtExtApp.Components.Attribute attribute) =>
            attribute.ResourceText("IDS_TXT_CMD_ROOMTOAREA");

        public static string GroundsExpression(RvtExtApp.Components.Attribute attribute) =>
            attribute.ResourceText("IDS_TXT_CMD_GROUNDSEXPRESSION");

        public static string LegalArea(RvtExtApp.Components.Attribute attribute) =>
            attribute.ResourceText("IDS_TXT_CMD_LEGALAREA");
    }
}
