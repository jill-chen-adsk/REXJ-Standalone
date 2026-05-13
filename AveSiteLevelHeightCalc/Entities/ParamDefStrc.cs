using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Entities
{
    public struct ParamDefStrc
    {
        public string DefName;
        public ForgeTypeId ParamType;
        public ForgeTypeId BltParamGroup;

        public ParamDefStrc(string defName, ForgeTypeId paramType, ForgeTypeId bltParamGroup)
        {
            DefName = defName;
            ParamType = paramType;
            BltParamGroup = bltParamGroup;
        }
    }
}
