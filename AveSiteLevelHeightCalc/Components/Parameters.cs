using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Components
{
    public class Parameters
    {
        private readonly Attribute _CmpAttribute;
        private readonly UIDocument _rvtUIDoc;
        private string _ShParamDefaultFileName;
        private string _ShParamFolderName;
        private string _ShParamFileName;
        private string _ShParamGroupName;

        public Parameters(Attribute cmpAttribute, UIDocument rvtUIDoc)
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
                    ExternalDefinitionCreationOptions opts = new ExternalDefinitionCreationOptions(defName, paramType)
                    {
                        Visible = visible
                    };
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
                    Binding newBinding = bindingMode == 1
                        ? (Binding)_rvtUIDoc.Application.Application.Create.NewTypeBinding(catSet)
                        : (Binding)_rvtUIDoc.Application.Application.Create.NewInstanceBinding(catSet);
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

        public bool SetDefinition(Element elem, Category category, string defName,
            ForgeTypeId paramType, ForgeTypeId bltParamGroup, bool visible, int bindingMode)
        {
            IList<Category> categories = new List<Category> { category };
            return SetDefinition(elem, categories, defName, paramType, bltParamGroup, visible, bindingMode);
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref int value)
        {
            if (elem == null) return -2;
            Parameter param = elem.get_Parameter(bip);
            if (param == null) return -2;
            value = param.AsInteger();
            return 0;
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref string value)
        {
            if (elem == null) return -2;
            Parameter param = elem.get_Parameter(bip);
            if (param == null) return -2;
            if (param.StorageType == StorageType.String)
                value = param.AsString() ?? "";
            else
                value = param.AsValueString() ?? "";
            return 0;
        }

        public int GetValue(Element elem, string paramName, ForgeTypeId paramType, ForgeTypeId groupType, ref string value)
        {
            if (elem == null) return -2;
            foreach (Parameter param in elem.Parameters)
            {
                if (param.Definition.Name != paramName) continue;

                if (param.StorageType == StorageType.String)
                    value = param.AsString() ?? "";
                else
                    value = param.AsValueString() ?? "";
                return 0;
            }
            return -2;
        }

        public int GetValue(Element elem, string paramName, ForgeTypeId paramType, ForgeTypeId groupType, ref double value)
        {
            if (elem == null) return -2;
            foreach (Parameter param in elem.Parameters)
            {
                if (param.Definition.Name != paramName) continue;

                if (param.StorageType == StorageType.Double)
                {
                    value = param.AsDouble();
                    return 0;
                }

                if (double.TryParse(param.AsValueString(), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out double d))
                {
                    value = d;
                    return 0;
                }
            }
            return -2;
        }

        public int GetValue(Element elem, string paramName, ForgeTypeId paramType, ForgeTypeId groupType, ref int value)
        {
            if (elem == null) return -2;
            foreach (Parameter param in elem.Parameters)
            {
                if (param.Definition.Name != paramName) continue;

                if (param.StorageType == StorageType.Integer)
                {
                    value = param.AsInteger();
                    return 0;
                }

                if (int.TryParse(param.AsValueString(), out int i))
                {
                    value = i;
                    return 0;
                }
            }
            return -2;
        }

        public void SetValue(Element elem, BuiltInParameter bip, int value)
        {
            if (elem == null) return;
            Parameter param = elem.get_Parameter(bip);
            if (param != null && !param.IsReadOnly)
                param.Set(value);
        }

        public void SetValue(Element elem, BuiltInParameter bip, double value)
        {
            if (elem == null) return;
            Parameter param = elem.get_Parameter(bip);
            if (param != null && !param.IsReadOnly)
                param.Set(value);
        }

        public int SetValue(Element elem, string paramName, ForgeTypeId paramType, ForgeTypeId groupType, string value)
        {
            if (elem == null) return -2;
            foreach (Parameter param in elem.Parameters)
            {
                if (param.Definition.Name == paramName && !param.IsReadOnly)
                {
                    param.Set(value);
                    return 0;
                }
            }
            return -2;
        }

        public int SetValue(Element elem, string paramName, ForgeTypeId paramType, ForgeTypeId groupType, double value)
        {
            if (elem == null) return -2;
            foreach (Parameter param in elem.Parameters)
            {
                if (param.Definition.Name == paramName && !param.IsReadOnly)
                {
                    param.Set(value);
                    return 0;
                }
            }
            return -2;
        }

        public int SetValue(Element elem, string paramName, ForgeTypeId paramType, ForgeTypeId groupType, int value)
        {
            if (elem == null) return -2;
            foreach (Parameter param in elem.Parameters)
            {
                if (param.Definition.Name == paramName && !param.IsReadOnly)
                {
                    param.Set(value);
                    return 0;
                }
            }
            return -2;
        }

        public string StrZeroPadding(string sValue, int decimalType)
        {
            string ret = "";

            double d = 0;

            if (double.TryParse(sValue, out d))
            {
                string format = "0";

                if (decimalType > 0)
                {
                    format += ".";
                    for (int i = 0; i < decimalType; ++i)
                        format += "0";
                }

                ret = d.ToString(format);
            }

            return ret;
        }
    }
}
