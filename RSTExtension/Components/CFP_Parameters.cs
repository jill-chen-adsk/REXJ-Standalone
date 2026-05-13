using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using JExtComCompat;

namespace RSTExtension.Components
{
    public class CFP_Parameters : RvtParameters
    {
        private readonly Attribute _attribute;
        private readonly string _shParamDefaultFileName;
        private readonly string _shParamFolderName;
        private readonly string _shParamFileName;
        private readonly string _shParamGroupName;

        public CFP_Parameters(Attribute attribute, UIDocument uidoc) : base(uidoc)
        {
            _attribute = attribute;

            _shParamDefaultFileName = "";
            DefinitionFile? defFile = GetSharedParameterFile();
            if (defFile != null)
            {
                _shParamDefaultFileName = defFile.Filename;
            }

            _shParamFolderName = _attribute.DataFolder;
            if (!System.IO.Directory.Exists(_shParamFolderName))
            {
                _shParamFolderName = System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
            }

            _shParamFileName = _attribute.ResourceText("IDS_SHPARAM_FILE");
            _shParamGroupName = _attribute.ResourceText("IDS_SHPARAM_GROUP");

            if (string.IsNullOrEmpty(_shParamDefaultFileName))
            {
                _shParamDefaultFileName = System.IO.Path.Combine(_shParamFolderName, _shParamFileName);
            }
        }

        public bool SetSharedParamDefault()
        {
            DefinitionFile? defFile = SetSharedParameterFile(null, _shParamDefaultFileName);
            return defFile != null;
        }

        public bool SetDefinition(
            Element? elem,
            IList<Category> categories,
            string defName,
            ForgeTypeId paramType,
            ForgeTypeId bltParamGroup,
            bool visible,
            int bindingMode)
        {
            return SetDefinition(
                elem,
                _shParamFolderName,
                _shParamFileName,
                _shParamGroupName,
                categories,
                defName,
                paramType,
                bltParamGroup,
                visible,
                bindingMode);
        }

        public bool SetDefinition(
            Element? elem,
            Category category,
            string defName,
            ForgeTypeId paramType,
            ForgeTypeId bltParamGroup,
            bool visible,
            int bindingMode)
        {
            var categories = new List<Category> { category };
            return SetDefinition(
                elem,
                categories,
                defName,
                paramType,
                bltParamGroup,
                visible,
                bindingMode);
        }
    }
}
