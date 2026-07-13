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

        public bool IsSharedParameterBound(string defName, Category category)
        {
            if (category == null)
                return false;

            return IsDefinitionBoundToCategories(defName, new List<Category> { category });
        }

        public bool IsSharedParameterAvailable(Element elem, string defName, Category category)
        {
            if (FindParameter(elem, defName) != null)
                return true;

            return IsSharedParameterBound(defName, category);
        }

        public bool SetDefinition(Element elem, string folderName, string fileName, string groupName,
            IList<Category> categories, string defName, ForgeTypeId paramType, ForgeTypeId bltParamGroup,
            bool visible, int bindingMode)
        {
            try
            {
                if (IsDefinitionBoundToCategories(defName, categories))
                    return true;

                var app = _rvtUIDoc.Application.Application;
                string addInFilePath = Path.Combine(folderName, fileName);
                string filePath = ResolveSharedParameterFilePath(addInFilePath);

                if (!File.Exists(filePath))
                    using (File.Create(filePath)) { }

                string origFile = null;
                try { origFile = app.SharedParametersFilename; } catch { }

                app.SharedParametersFilename = filePath;
                var defFile = app.OpenSharedParameterFile();
                if (defFile == null)
                    return false;

                var group = defFile.Groups.get_Item(groupName) ?? defFile.Groups.Create(groupName);

                var def = group.Definitions.get_Item(defName);
                if (def == null)
                {
                    var opts = new ExternalDefinitionCreationOptions(defName, paramType) { Visible = visible };
                    def = group.Definitions.Create(opts);
                }

                if (def == null)
                    return false;

                var catSet = app.Create.NewCategorySet();
                foreach (var cat in categories)
                {
                    if (cat != null && CanBindToCategory(cat))
                        catSet.Insert(cat);
                }

                if (catSet.IsEmpty)
                    return IsDefinitionBoundToCategories(defName, categories);

                var bindingMap = RvtDBDoc.ParameterBindings;
                Autodesk.Revit.DB.Binding binding = bindingMode == 1
                    ? app.Create.NewTypeBinding(catSet)
                    : app.Create.NewInstanceBinding(catSet);

                ForgeTypeId paramGroup = ResolveParameterGroup(bltParamGroup);

                if (bindingMap.Contains(def))
                    bindingMap.ReInsert(def, binding, paramGroup);
                else
                    bindingMap.Insert(def, binding, paramGroup);

                try
                {
                    if (!string.IsNullOrEmpty(origFile))
                        app.SharedParametersFilename = origFile;
                }
                catch { }

                return true;
            }
            catch
            {
                return IsDefinitionBoundToCategories(defName, categories);
            }
        }

        string ResolveSharedParameterFilePath(string addInFilePath)
        {
            if (!string.IsNullOrEmpty(_shParamDefaultFileName) &&
                File.Exists(_shParamDefaultFileName) &&
                !string.Equals(_shParamDefaultFileName, addInFilePath, StringComparison.OrdinalIgnoreCase))
            {
                return _shParamDefaultFileName;
            }

            return addInFilePath;
        }

        static bool CanBindToCategory(Category category)
        {
            if (category.AllowsBoundParameters)
                return true;

            try
            {
                return category.BuiltInCategory == BuiltInCategory.OST_ProjectInformation;
            }
            catch
            {
                return false;
            }
        }

        static ForgeTypeId ResolveParameterGroup(ForgeTypeId bltParamGroup)
        {
            if (bltParamGroup == null || string.IsNullOrEmpty(bltParamGroup.TypeId))
                return GroupTypeId.IdentityData;

            return bltParamGroup;
        }

        bool IsDefinitionBoundToCategories(string defName, IList<Category> categories)
        {
            if (categories == null || categories.Count == 0)
                return false;

            var targetCategoryIds = new HashSet<ElementId>();
            foreach (var category in categories)
            {
                if (category != null)
                    targetCategoryIds.Add(category.Id);
            }

            if (targetCategoryIds.Count == 0)
                return false;

            var bindings = RvtDBDoc.ParameterBindings;
            var iterator = bindings.ForwardIterator();
            while (iterator.MoveNext())
            {
                if (iterator.Key is not Definition definition)
                    continue;

                if (!string.Equals(definition.Name, defName, StringComparison.Ordinal))
                    continue;

                if (iterator.Current is not ElementBinding elementBinding)
                    continue;

                foreach (Category boundCategory in elementBinding.Categories)
                {
                    if (targetCategoryIds.Contains(boundCategory.Id))
                        return true;
                }
            }

            return false;
        }

        static Parameter FindParameter(Element elem, string paramName)
        {
            if (elem == null || string.IsNullOrEmpty(paramName))
                return null;

            var param = elem.LookupParameter(paramName);
            if (param != null)
                return param;

            foreach (Parameter candidate in elem.Parameters)
            {
                if (string.Equals(candidate?.Definition?.Name, paramName, StringComparison.Ordinal))
                    return candidate;
            }

            return null;
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
                var param = FindParameter(elem, paramName);
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
                var param = FindParameter(elem, paramName);
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
                var param = FindParameter(elem, paramName);
                if (param != null && param.HasValue)
                {
                    value = param.AsInteger() != 0;
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
                var param = FindParameter(elem, paramName);
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
                var param = FindParameter(elem, paramName);
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
                var param = FindParameter(elem, paramName);
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
