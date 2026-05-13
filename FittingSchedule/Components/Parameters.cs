using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.FittingSchedule.Components
{
    public class Parameters
    {
        private UIDocument _rvtUIDoc;
        private string _shParamDefaultFileName;
        private string _shParamFolderName;
        private string _shParamFileName;
        private string _shParamGroupName;

        public Parameters(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;

            _shParamDefaultFileName = null;
            var defFile = GetSharedParameterFile();
            if (defFile != null)
                _shParamDefaultFileName = defFile.Filename;

            var cmpAttribute = new Attribute();
            _shParamFolderName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _shParamFileName = cmpAttribute.ResourceText("IDS_SHPARAM_FILE");
            _shParamGroupName = cmpAttribute.ResourceText("IDS_SHPARAM_GROUP");

            if (_shParamDefaultFileName == null)
                _shParamDefaultFileName = Path.Combine(_shParamFolderName, _shParamFileName);
        }

        public Document RvtDBDoc => _rvtUIDoc.Document;

        public DefinitionFile GetSharedParameterFile()
        {
            try { return _rvtUIDoc.Application.Application.OpenSharedParameterFile(); }
            catch { return null; }
        }

        public DefinitionFile SetSharedParameterFile(object unused, string filePath)
        {
            try
            {
                _rvtUIDoc.Application.Application.SharedParametersFilename = filePath;
                return _rvtUIDoc.Application.Application.OpenSharedParameterFile();
            }
            catch { return null; }
        }

        public bool SetSharedParamDefault()
        {
            var defFile = SetSharedParameterFile(null, _shParamDefaultFileName);
            return defFile != null;
        }

        public bool SetDefinition(Element elem, IList<Category> categories, string defName,
            ForgeTypeId paramType, ForgeTypeId bltParamGroup, bool visible, int bindingMode)
        {
            return SetDefinition(elem, _shParamFolderName, _shParamFileName, _shParamGroupName,
                categories, defName, paramType, bltParamGroup, visible, bindingMode);
        }

        public bool SetDefinition(Element elem, Category category, string defName,
            ForgeTypeId paramType, ForgeTypeId bltParamGroup, bool visible, int bindingMode)
        {
            var categories = new List<Category> { category };
            return SetDefinition(elem, categories, defName, paramType, bltParamGroup, visible, bindingMode);
        }

        public bool SetDefinition(Element elem, string folderName, string fileName, string groupName,
            IList<Category> categories, string defName, ForgeTypeId paramType, ForgeTypeId bltParamGroup,
            bool visible, int bindingMode)
        {
            try
            {
                var app = _rvtUIDoc.Application.Application;
                string filePath = Path.Combine(folderName, fileName);

                if (!File.Exists(filePath))
                {
                    using (File.Create(filePath)) { }
                }

                string origFile = app.SharedParametersFilename;
                app.SharedParametersFilename = filePath;
                var defFile = app.OpenSharedParameterFile();
                if (defFile == null) return false;

                var group = defFile.Groups.get_Item(groupName);
                if (group == null)
                    group = defFile.Groups.Create(groupName);

                var def = group.Definitions.get_Item(defName);
                if (def == null)
                {
                    var opts = new ExternalDefinitionCreationOptions(defName, paramType);
                    opts.Visible = visible;
                    def = group.Definitions.Create(opts);
                }

                if (categories != null && categories.Count > 0)
                {
                    var catSet = new CategorySet();
                    foreach (var cat in categories)
                        if (cat != null) catSet.Insert(cat);

                    if (catSet.Size > 0)
                    {
                        ElementBinding binding;
                        if (bindingMode == 1)
                            binding = RvtDBDoc.Application.Create.NewTypeBinding(catSet);
                        else
                            binding = RvtDBDoc.Application.Create.NewInstanceBinding(catSet);

                        RvtDBDoc.ParameterBindings.Insert(def, binding, bltParamGroup);
                    }
                }

                app.SharedParametersFilename = origFile ?? "";
                return true;
            }
            catch { return false; }
        }

        public Category CategoryProjInfo => Category.GetCategory(RvtDBDoc, BuiltInCategory.OST_ProjectInformation);

        public int GetValue(Element elem, string paramName, ForgeTypeId paramType, ForgeTypeId bltParamGroup, ref string value)
        {
            if (elem == null) return -2;
            var param = elem.LookupParameter(paramName);
            if (param == null) return -2;
            value = param.AsString() ?? "";
            return 0;
        }

        public int SetValue(Element elem, string paramName, ForgeTypeId paramType, ForgeTypeId bltParamGroup, string value)
        {
            if (elem == null) return -2;
            var param = elem.LookupParameter(paramName);
            if (param == null) return -2;
            param.Set(value);
            return 0;
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref int value)
        {
            if (elem == null) return -2;
            var param = elem.get_Parameter(bip);
            if (param == null) return -2;
            value = param.AsInteger();
            return 0;
        }

        public int SetValue(Element elem, BuiltInParameter bip, int value)
        {
            if (elem == null) return -2;
            var param = elem.get_Parameter(bip);
            if (param == null) return -2;
            param.Set(value);
            return 0;
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref string value)
        {
            if (elem == null) return -2;
            var param = elem.get_Parameter(bip);
            if (param == null) return -2;
            value = param.AsString() ?? "";
            return 0;
        }

        public int SetValue(Element elem, BuiltInParameter bip, string value)
        {
            if (elem == null) return -2;
            var param = elem.get_Parameter(bip);
            if (param == null) return -2;
            param.Set(value);
            return 0;
        }
    }
}
