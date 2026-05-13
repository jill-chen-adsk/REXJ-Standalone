using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Collections = System.Collections;

namespace ADSK.JExtRAC.GridDimension.Components
{
    public class Parameters
    {
        private readonly Attribute _cmpAttribute;
        private string _shParamDefaultFileName;
        private string _shParamFolderName;
        private string _shParamFileName;
        private string _shParamGroupName;
        private readonly UIDocument _rvtUIDoc;

        public Parameters(Attribute cmpAttribute, UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            RvtDBDoc = rvtUIDoc.Document;
            _cmpAttribute = cmpAttribute;

            _shParamDefaultFileName = null;
            DefinitionFile defFile = GetSharedParameterFile();
            if (defFile != null)
                _shParamDefaultFileName = defFile.Filename;

            _shParamFolderName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _shParamFileName = _cmpAttribute.ResourceText("IDS_SHPARAM_FILE");
            _shParamGroupName = _cmpAttribute.ResourceText("IDS_SHPARAM_GROUP");

            if (_shParamDefaultFileName == null)
                _shParamDefaultFileName = _shParamFolderName + "\\" + _shParamFileName;
        }

        public Document RvtDBDoc { get; }

        private DefinitionFile GetSharedParameterFile()
        {
            try
            {
                return RvtDBDoc.Application.OpenSharedParameterFile();
            }
            catch
            {
                return null;
            }
        }

        private DefinitionFile SetSharedParameterFile(object unused, string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    using (System.IO.File.Create(filePath)) { }
                }
                RvtDBDoc.Application.SharedParametersFilename = filePath;
                return RvtDBDoc.Application.OpenSharedParameterFile();
            }
            catch
            {
                return null;
            }
        }

        public bool SetSharedParamDefault()
        {
            DefinitionFile defFile = SetSharedParameterFile(null, _shParamDefaultFileName);
            return defFile != null;
        }

        public bool SetDefinition(Element elem,
                           Collections.Generic.IList<Category> categories,
                           string defName,
                           ForgeTypeId paramType,
                           ForgeTypeId bltParamGroup,
                           bool visible,
                           int bindingMode)
        {
            return SetDefinitionInternal(elem,
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

        public bool SetDefinition(Element elem,
                           Category category,
                           string defName,
                           ForgeTypeId paramType,
                           ForgeTypeId bltParamGroup,
                           bool visible,
                           int bindingMode)
        {
            Collections.Generic.IList<Category> categories = new Collections.Generic.List<Category>();
            categories.Add(category);
            return SetDefinition(elem, categories, defName, paramType, bltParamGroup, visible, bindingMode);
        }

        private bool SetDefinitionInternal(Element elem,
                                    string folderName,
                                    string fileName,
                                    string groupName,
                                    Collections.Generic.IList<Category> categories,
                                    string defName,
                                    ForgeTypeId paramType,
                                    ForgeTypeId bltParamGroup,
                                    bool visible,
                                    int bindingMode)
        {
            try
            {
                string filePath = System.IO.Path.Combine(folderName, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    using (System.IO.File.Create(filePath)) { }
                }

                RvtDBDoc.Application.SharedParametersFilename = filePath;
                DefinitionFile defFile = RvtDBDoc.Application.OpenSharedParameterFile();
                if (defFile == null) return false;

                DefinitionGroup defGroup = defFile.Groups.get_Item(groupName);
                if (defGroup == null)
                    defGroup = defFile.Groups.Create(groupName);

                Definition def = defGroup.Definitions.get_Item(defName);
                if (def == null)
                {
                    ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions(defName, paramType);
                    options.Visible = visible;
                    def = defGroup.Definitions.Create(options);
                }

                CategorySet catSet = RvtDBDoc.Application.Create.NewCategorySet();
                foreach (var cat in categories)
                    catSet.Insert(cat);

                Binding binding;
                if (bindingMode == 0)
                    binding = RvtDBDoc.Application.Create.NewInstanceBinding(catSet);
                else
                    binding = RvtDBDoc.Application.Create.NewTypeBinding(catSet);

                BindingMap bindingMap = RvtDBDoc.ParameterBindings;
                if (bindingMap.Contains(def))
                    bindingMap.ReInsert(def, binding, bltParamGroup);
                else
                    bindingMap.Insert(def, binding, bltParamGroup);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int GetValue(Element elem, BuiltInParameter bltParam, ref bool value)
        {
            value = false;
            try
            {
                Parameter param = elem?.get_Parameter(bltParam);
                if (param == null)
                    return -2;
                if (param.StorageType != StorageType.Integer)
                    return -2;
                value = Convert.ToBoolean(param.AsInteger());
                return 0;
            }
            catch
            {
                return -3;
            }
        }

        public int GetValue(Element elem, string defName, ForgeTypeId dataType, ForgeTypeId bltParamGroup, ref string value)
        {
            value = "";
            try
            {
                if (elem == null)
                    return -2;
                foreach (Parameter p in elem.Parameters)
                {
                    if (p?.Definition?.Name != defName)
                        continue;
                    value = p.AsString() ?? "";
                    return 0;
                }
                return -2;
            }
            catch
            {
                return -3;
            }
        }

        public void SetValue(Element elem, string defName, ForgeTypeId dataType, ForgeTypeId bltParamGroup, string val)
        {
            if (elem == null)
                return;
            foreach (Parameter p in elem.Parameters)
            {
                if (p?.Definition?.Name == defName)
                {
                    p.Set(val);
                    return;
                }
            }
        }
    }
}
