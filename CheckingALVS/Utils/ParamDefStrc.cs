using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
    /// <summary>Shared-parameter definition metadata (replaces JExtCom Rvt.ParamDefStrc).</summary>
    public struct ParamDefStrc
    {
        public Category Category { get; }
        public IList<Category> Categories { get; }
        public string DefName { get; }
        public ForgeTypeId ParamType { get; }
        public ForgeTypeId BltParamGroup { get; }
        public bool Visible { get; }
        public int BindingMode { get; }

        public ParamDefStrc(IList<Category> categories, string defName, ForgeTypeId paramType, ForgeTypeId bltParamGroup, bool visible, int bindingMode)
        {
            Category = null;
            Categories = categories;
            DefName = defName;
            ParamType = paramType;
            BltParamGroup = bltParamGroup;
            Visible = visible;
            BindingMode = bindingMode;
        }

        public ParamDefStrc(Category category, string defName, ForgeTypeId paramType, ForgeTypeId bltParamGroup, bool visible, int bindingMode)
        {
            Category = category;
            Categories = null;
            DefName = defName;
            ParamType = paramType;
            BltParamGroup = bltParamGroup;
            Visible = visible;
            BindingMode = bindingMode;
        }

        /// <summary>Stub ctor for Mark/Number defs (no shared-param registration in original).</summary>
        public ParamDefStrc(string defName, ForgeTypeId paramType, ForgeTypeId bltParamGroup)
        {
            Category = null;
            Categories = null;
            DefName = defName;
            ParamType = paramType;
            BltParamGroup = bltParamGroup;
            Visible = true;
            BindingMode = 0;
        }
    }
}
