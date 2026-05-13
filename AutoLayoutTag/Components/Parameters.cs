using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Collections = System.Collections;
using RvtExtApp = ADSK.JExtRAC.AutoLayoutTag;

namespace ADSK.JExtRAC.AutoLayoutTag.Components
{
    public class Parameters
    {
        private RvtExtApp.Components.Attribute _CmpAttribute;
        private string _ShParamDefaultFileName;
        private string _ShParamFolderName;
        private string _ShParamFileName;
        private string _ShParamGroupName;
        private readonly UIDocument _rvtUIDoc;
        public Document RvtDBDoc { get; }

        public Parameters(RvtExtApp.Components.Attribute cmpAttribute, UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            RvtDBDoc = rvtUIDoc.Document;
            _CmpAttribute = cmpAttribute;

            _ShParamDefaultFileName = null;
            DefinitionFile defFile = GetSharedParameterFile();
            if (defFile != null)
            {
                _ShParamDefaultFileName = defFile.Filename;
            }

            _ShParamFolderName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _ShParamFileName = _CmpAttribute.ResourceText("IDS_SHPARAM_FILE");
            _ShParamGroupName = _CmpAttribute.ResourceText("IDS_SHPARAM_GROUP");

            if (_ShParamDefaultFileName == null)
            {
                _ShParamDefaultFileName = _ShParamFolderName + "\\" + _ShParamFileName;
            }
        }

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
            bool ret = false;
            DefinitionFile defFile = SetSharedParameterFile(null, _ShParamDefaultFileName);
            if (defFile != null)
            {
                ret = true;
            }
            return ret;
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
                                  _ShParamFolderName,
                                  _ShParamFileName,
                                  _ShParamGroupName,
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
    }
}
