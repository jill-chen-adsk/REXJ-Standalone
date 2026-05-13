using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AreaSchedule.Components
{
    public class Parameters
    {
        private readonly Attribute _cmpAttribute;
        private readonly UIDocument _rvtUIDoc;
        private string _shParamDefaultFileName;
        private readonly string _shParamFolderName;
        private readonly string _shParamFileName;
        private readonly string _shParamGroupName;

        public Parameters(Attribute cmpAttribute, UIDocument rvtUIDoc)
        {
            _cmpAttribute = cmpAttribute;
            _rvtUIDoc = rvtUIDoc;

            _shParamDefaultFileName = null;
            DefinitionFile defFile = GetSharedParameterFile();
            if (defFile != null)
                _shParamDefaultFileName = defFile.Filename;

            _shParamFolderName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _shParamFileName = _cmpAttribute.ResourceText("IDS_SHPARAM_FILE");
            _shParamGroupName = _cmpAttribute.ResourceText("IDS_SHPARAM_GROUP");

            if (_shParamDefaultFileName == null)
                _shParamDefaultFileName = Path.Combine(_shParamFolderName, _shParamFileName);
        }

        private Document Doc => _rvtUIDoc.Document;

        public DefinitionFile GetSharedParameterFile()
        {
            try { return _rvtUIDoc.Application.Application.OpenSharedParameterFile(); }
            catch { return null; }
        }

        public DefinitionFile SetSharedParameterFile(object _, string fileName)
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
            DefinitionFile defFile = SetSharedParameterFile(null, _shParamDefaultFileName);
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
                string shParamFilePath = Path.Combine(_shParamFolderName, _shParamFileName);
                if (!File.Exists(shParamFilePath))
                    File.Create(shParamFilePath).Dispose();

                string prevFile = null;
                try { prevFile = _rvtUIDoc.Application.Application.SharedParametersFilename; } catch { }

                _rvtUIDoc.Application.Application.SharedParametersFilename = shParamFilePath;
                DefinitionFile defFile = _rvtUIDoc.Application.Application.OpenSharedParameterFile();
                if (defFile == null) return false;

                DefinitionGroup group = defFile.Groups.get_Item(_shParamGroupName)
                    ?? defFile.Groups.Create(_shParamGroupName);

                Definition def = group.Definitions.get_Item(defName);
                if (def == null)
                {
                    ExternalDefinitionCreationOptions opts = new ExternalDefinitionCreationOptions(defName, paramType) { Visible = visible };
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
                        ? _rvtUIDoc.Application.Application.Create.NewTypeBinding(catSet)
                        : _rvtUIDoc.Application.Application.Create.NewInstanceBinding(catSet);

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

        public int GetValue(Element elem, string paramName, ForgeTypeId _, ForgeTypeId __, ref string value)
        {
            if (elem == null) return -2;
            Parameter param = elem.LookupParameter(paramName);
            if (param == null)
            {
                foreach (Parameter p in elem.Parameters)
                {
                    if (p.Definition.Name == paramName)
                    {
                        param = p;
                        break;
                    }
                }
            }
            if (param == null) return -2;
            if (param.StorageType == StorageType.Double)
                value = param.AsValueString() ?? "";
            else if (param.StorageType == StorageType.Integer)
                value = param.AsInteger().ToString();
            else if (param.StorageType == StorageType.String)
                value = param.AsString() ?? "";
            else
                value = param.AsValueString() ?? "";
            return 0;
        }

        public int GetValue(Element elem, string paramName, ForgeTypeId _, ForgeTypeId __, ref double value)
        {
            if (elem == null) return -2;
            Parameter param = elem.LookupParameter(paramName);
            if (param == null) return -2;
            if (param.StorageType == StorageType.Double)
                value = param.AsDouble();
            else if (param.HasValue)
                double.TryParse(param.AsValueString(), out value);
            else
                return -2;
            return 0;
        }

        public int SetValue(Element elem, BuiltInParameter bip, string value)
        {
            if (elem == null) return -2;
            Parameter param = elem.get_Parameter(bip);
            if (param != null && !param.IsReadOnly)
            {
                param.Set(value);
                return 0;
            }
            return -2;
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref double value)
        {
            if (elem == null) return -2;
            Parameter param = elem.get_Parameter(bip);
            if (param == null) return -2;
            if (param.StorageType == StorageType.Double)
            {
                value = param.AsDouble();
                return 0;
            }
            return -2;
        }

        public int SetValue(Element elem, BuiltInParameter bip, double value)
        {
            if (elem == null) return -2;
            Parameter param = elem.get_Parameter(bip);
            if (param != null && !param.IsReadOnly)
            {
                param.Set(value);
                return 0;
            }
            return -2;
        }

        public int SetValue(Element elem, string paramName, ForgeTypeId _, ForgeTypeId __, double value)
        {
            if (elem == null) return -2;
            Parameter param = elem.LookupParameter(paramName);
            if (param != null && !param.IsReadOnly)
            {
                param.Set(value);
                return 0;
            }
            foreach (Parameter p in elem.Parameters)
            {
                if (p.Definition.Name == paramName && !p.IsReadOnly)
                {
                    p.Set(value);
                    return 0;
                }
            }
            return -2;
        }

        public int SetValue(Element elem, string paramName, ForgeTypeId _, ForgeTypeId __, string value)
        {
            if (elem == null) return -2;
            Parameter param = elem.LookupParameter(paramName);
            if (param != null && !param.IsReadOnly)
            {
                param.Set(value);
                return 0;
            }
            foreach (Parameter p in elem.Parameters)
            {
                if (p.Definition.Name == paramName && !p.IsReadOnly)
                {
                    p.Set(value);
                    return 0;
                }
            }
            return -2;
        }
    }
}
