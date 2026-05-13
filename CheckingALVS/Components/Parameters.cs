using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.CheckingALVS.Components
{
    public class Parameters
    {
        private readonly UIDocument _rvtUIDoc;
        private readonly string _shParamDefaultFileName;
        private readonly string _shParamFolderName;
        private readonly string _shParamFileName;
        private readonly string _shParamGroupName;

        public Parameters(Attribute cmpAttribute, UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;

            _shParamDefaultFileName = null;
            var defFile = GetSharedParameterFile();
            if (defFile != null)
                _shParamDefaultFileName = defFile.Filename;

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
            var categories = new List<Category>();
            if (category != null) categories.Add(category);
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
                    using (File.Create(filePath)) { }

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

                if (def != null)
                {
                    var catSet = new CategorySet();
                    foreach (var cat in categories)
                    {
                        if (cat != null) catSet.Insert(cat);
                    }

                    var existingBinding = RvtDBDoc.ParameterBindings.get_Item(def);
                    if (existingBinding == null)
                    {
                        ElementBinding binding;
                        if (bindingMode == 1)
                            binding = app.Create.NewTypeBinding(catSet);
                        else
                            binding = app.Create.NewInstanceBinding(catSet);
                        RvtDBDoc.ParameterBindings.Insert(def, binding, bltParamGroup);
                    }
                }

                try { app.SharedParametersFilename = origFile; } catch { }
                return true;
            }
            catch { return false; }
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref int value)
        {
            try
            {
                var param = elem.get_Parameter(bip);
                if (param != null)
                {
                    value = param.AsInteger();
                    return 0;
                }
                return -2;
            }
            catch { return -2; }
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref double value)
        {
            try
            {
                var param = elem.get_Parameter(bip);
                if (param != null)
                {
                    value = param.AsDouble();
                    return 0;
                }
                return -2;
            }
            catch { return -2; }
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref string value)
        {
            try
            {
                var param = elem.get_Parameter(bip);
                if (param != null)
                {
                    value = param.AsString() ?? param.AsValueString() ?? "";
                    return 0;
                }
                return -2;
            }
            catch { return -2; }
        }

        public int GetValue(Element elem, string paramName, ForgeTypeId paramType,
            ForgeTypeId paramGroup, ref string value)
        {
            try
            {
                var param = elem.LookupParameter(paramName);
                if (param != null)
                {
                    value = param.AsString() ?? param.AsValueString() ?? "";
                    return 0;
                }
                return -2;
            }
            catch { return -2; }
        }

        public int GetValue(Element elem, string paramName, ForgeTypeId paramType,
            ForgeTypeId paramGroup, ref double value)
        {
            try
            {
                var param = elem.LookupParameter(paramName);
                if (param != null && param.HasValue)
                {
                    value = param.AsDouble();
                    return 0;
                }
                return -2;
            }
            catch { return -2; }
        }

        public int SetValue(Element elem, BuiltInParameter bip, double value)
        {
            try
            {
                var param = elem.get_Parameter(bip);
                if (param != null)
                {
                    param.Set(value);
                    return 0;
                }
                return -2;
            }
            catch { return -2; }
        }

        public int SetValue(Element elem, BuiltInParameter bip, string value)
        {
            try
            {
                var param = elem.get_Parameter(bip);
                if (param != null)
                {
                    param.Set(value);
                    return 0;
                }
                return -2;
            }
            catch { return -2; }
        }

        public int GetValue(Element elem, string paramName, ForgeTypeId paramType,
            ForgeTypeId paramGroup, ref bool value)
        {
            try
            {
                var param = elem.LookupParameter(paramName);
                if (param != null && param.HasValue)
                {
                    bool b = param.AsInteger() != 0;
                    value = b;
                    return 0;
                }
                return -2;
            }
            catch { return -2; }
        }

        public int SetValue(Element elem, string paramName, ForgeTypeId paramType,
            ForgeTypeId paramGroup, string value)
        {
            try
            {
                var param = elem.LookupParameter(paramName);
                if (param != null)
                {
                    param.Set(value);
                    return 0;
                }
                return -2;
            }
            catch { return -2; }
        }

        public int SetValue(Element elem, string paramName, ForgeTypeId paramType,
            ForgeTypeId paramGroup, bool value)
        {
            try
            {
                var param = elem.LookupParameter(paramName);
                if (param != null)
                {
                    param.Set(value ? 1 : 0);
                    return 0;
                }
                return -2;
            }
            catch { return -2; }
        }

        public int SetValue(Element elem, string paramName, ForgeTypeId paramType,
            ForgeTypeId paramGroup, double value)
        {
            try
            {
                var param = elem.LookupParameter(paramName);
                if (param != null)
                {
                    param.Set(value);
                    return 0;
                }
                return -2;
            }
            catch { return -2; }
        }

    }
}
