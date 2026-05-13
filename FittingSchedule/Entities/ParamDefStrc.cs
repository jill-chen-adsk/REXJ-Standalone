using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.FittingSchedule.Entities
{
    public struct ParamDefStrc
    {
        public string DefName { get; }
        public ForgeTypeId ParamType { get; }
        public ForgeTypeId BltParamGroup { get; }

        public ParamDefStrc(string defName, ForgeTypeId paramType, ForgeTypeId bltParamGroup)
        {
            DefName = defName;
            ParamType = paramType;
            BltParamGroup = bltParamGroup;
        }
    }
}
