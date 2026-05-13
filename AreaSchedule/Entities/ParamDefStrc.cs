using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.AreaSchedule.Entities
{
    public sealed class ParamDefStrc
    {
        public Category Category { get; }
        public string DefName { get; }
        public ForgeTypeId ParamType { get; }
        public ForgeTypeId BltParamGroup { get; }
        public bool Visible { get; }
        public int BindingMode { get; }

        public ParamDefStrc(Category category, string defName, ForgeTypeId paramType, ForgeTypeId bltParamGroup, bool visible, int bindingMode)
        {
            Category = category;
            DefName = defName;
            ParamType = paramType;
            BltParamGroup = bltParamGroup;
            Visible = visible;
            BindingMode = bindingMode;
        }
    }
}
