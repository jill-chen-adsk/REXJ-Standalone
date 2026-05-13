using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace SectionListRC.JExtComCompat
{
    public class ParamDefStrc
    {
        public ParamDefStrc(
            IList<Category> categories,
            string defName,
            ForgeTypeId paramType,
            ForgeTypeId paramGroup,
            bool visible,
            int bindingMode)
        {
            Categories = categories;
            DefName = defName;
            ParamType = paramType;
            BltParamGroup = paramGroup;
            Visible = visible;
            BindingMode = bindingMode;
        }

        public IList<Category> Categories { get; }
        public string DefName { get; }
        public ForgeTypeId ParamType { get; }
        public ForgeTypeId BltParamGroup { get; }
        public bool Visible { get; }
        public int BindingMode { get; }
    }
}
