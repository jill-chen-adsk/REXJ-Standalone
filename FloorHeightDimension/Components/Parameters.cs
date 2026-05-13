using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RvtExtApp = ADSK.JExtRAC.FloorHeightDimension;

namespace ADSK.JExtRAC.FloorHeightDimension.Components
{
    public class Parameters
    {
        private readonly RvtExtApp.Components.Attribute _CmpAttribute;
        private readonly UIDocument _rvtUIDoc;
        private string _ShParamDefaultFileName;
        private string _ShParamFolderName;
        private string _ShParamFileName;
        private string _ShParamGroupName;

        public Parameters(RvtExtApp.Components.Attribute cmpAttribute, UIDocument rvtUIDoc)
        {
            _CmpAttribute = cmpAttribute;
            _rvtUIDoc = rvtUIDoc;

            _ShParamDefaultFileName = null;
            DefinitionFile defFile = GetSharedParameterFile();
            if (defFile != null)
                _ShParamDefaultFileName = defFile.Filename;

            _ShParamFolderName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _ShParamFileName = _CmpAttribute.ResourceText("IDS_SHPARAM_FILE");
            _ShParamGroupName = _CmpAttribute.ResourceText("IDS_SHPARAM_GROUP");

            if (_ShParamDefaultFileName == null)
                _ShParamDefaultFileName = Path.Combine(_ShParamFolderName, _ShParamFileName);
        }

        private Document Doc => _rvtUIDoc.Document;

        public DefinitionFile GetSharedParameterFile()
        {
            try { return _rvtUIDoc.Application.Application.OpenSharedParameterFile(); }
            catch { return null; }
        }

        public DefinitionFile SetSharedParameterFile(object unused, string fileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName) && !File.Exists(fileName))
                    File.Create(fileName).Dispose();
                _rvtUIDoc.Application.Application.SharedParametersFilename = fileName;
                return _rvtUIDoc.Application.Application.OpenSharedParameterFile();
            }
            catch { return null; }
        }

        public bool SetSharedParamDefault()
        {
            DefinitionFile defFile = SetSharedParameterFile(null, _ShParamDefaultFileName);
            return defFile != null;
        }

        public bool SetDefinition(Element elem, Category category, string defName,
            ForgeTypeId paramType, ForgeTypeId bltParamGroup, bool visible, int bindingMode)
        {
            IList<Category> categories = new List<Category> { category };
            return SetDefinition(elem, categories, defName, paramType, bltParamGroup, visible, bindingMode);
        }

        public bool SetDefinition(Element elem, IList<Category> categories, string defName,
            ForgeTypeId paramType, ForgeTypeId bltParamGroup, bool visible, int bindingMode)
        {
            try
            {
                string shParamFilePath = Path.Combine(_ShParamFolderName, _ShParamFileName);
                if (!File.Exists(shParamFilePath))
                    File.Create(shParamFilePath).Dispose();

                string prevFile = null;
                try { prevFile = _rvtUIDoc.Application.Application.SharedParametersFilename; } catch { }

                _rvtUIDoc.Application.Application.SharedParametersFilename = shParamFilePath;
                DefinitionFile defFile = _rvtUIDoc.Application.Application.OpenSharedParameterFile();
                if (defFile == null) return false;

                DefinitionGroup group = defFile.Groups.get_Item(_ShParamGroupName);
                if (group == null)
                    group = defFile.Groups.Create(_ShParamGroupName);

                Definition def = group.Definitions.get_Item(defName);
                if (def == null)
                {
                    ExternalDefinitionCreationOptions opts = new ExternalDefinitionCreationOptions(defName, paramType);
                    opts.Visible = visible;
                    def = group.Definitions.Create(opts);
                }

                CategorySet catSet = _rvtUIDoc.Application.Application.Create.NewCategorySet();
                foreach (Category cat in categories)
                {
                    if (cat != null && cat.AllowsBoundParameters)
                        catSet.Insert(cat);
                }

                BindingMap bindingMap = Doc.ParameterBindings;
                Binding existingBinding = bindingMap.get_Item(def);

                if (existingBinding == null)
                {
                    Binding newBinding;
                    if (bindingMode == 1)
                        newBinding = _rvtUIDoc.Application.Application.Create.NewTypeBinding(catSet);
                    else
                        newBinding = _rvtUIDoc.Application.Application.Create.NewInstanceBinding(catSet);

                    bindingMap.Insert(def, newBinding, bltParamGroup);
                }

                if (!string.IsNullOrEmpty(prevFile))
                {
                    try { _rvtUIDoc.Application.Application.SharedParametersFilename = prevFile; } catch { }
                }

                return true;
            }
            catch { return false; }
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref int value)
        {
            if (elem == null) return -2;
            Parameter param = elem.get_Parameter(bip);
            if (param == null) return -2;
            value = param.AsInteger();
            return 0;
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref string textValue)
        {
            if (elem == null) return -2;
            Parameter param = elem.get_Parameter(bip);
            if (param == null) return -2;
            if (param.StorageType == StorageType.String)
                textValue = param.AsString() ?? "";
            else
                textValue = param.AsValueString() ?? "";
            return 0;
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref bool value)
        {
            if (elem == null) return -2;
            Parameter param = elem.get_Parameter(bip);
            if (param == null) return -2;
            value = param.AsInteger() != 0;
            return 0;
        }

        public int GetValue(Element elem, string paramName, ForgeTypeId paramType, ForgeTypeId groupType, ref string value)
        {
            if (elem == null) return -2;
            foreach (Parameter param in elem.Parameters)
            {
                if (param.Definition?.Name == paramName)
                {
                    if (param.StorageType == StorageType.String)
                        value = param.AsString() ?? "";
                    else
                        value = param.AsValueString() ?? "";
                    return 0;
                }
            }
            return -2;
        }

        public void SetValue(Element elem, BuiltInParameter bip, int intValue)
        {
            if (elem == null) return;
            Parameter param = elem.get_Parameter(bip);
            if (param != null && !param.IsReadOnly)
                param.Set(intValue);
        }

        public void SetValue(Element elem, BuiltInParameter bip, double doubleValue)
        {
            if (elem == null) return;
            Parameter param = elem.get_Parameter(bip);
            if (param != null && !param.IsReadOnly)
                param.Set(doubleValue);
        }

        public void SetValue(Element elem, string paramName, ForgeTypeId paramType, ForgeTypeId groupType, string value)
        {
            if (elem == null) return;
            foreach (Parameter param in elem.Parameters)
            {
                if (param.Definition?.Name == paramName && !param.IsReadOnly)
                {
                    param.Set(value);
                    return;
                }
            }
        }
    }
}
