using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.ExportSchedule.Components
{
    public class Parameters
    {
        private readonly UIDocument _rvtUIDoc;
        private readonly Attribute _cmpAttribute;
        private readonly string _shParamFolderName;
        private readonly string _shParamFileName;
        private readonly string _shParamGroupName;

        public Parameters(Attribute cmpAttribute, UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            _cmpAttribute = cmpAttribute;
            _shParamFolderName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _shParamFileName = cmpAttribute.ResourceText("IDS_SHPARAM_FILE");
            _shParamGroupName = cmpAttribute.ResourceText("IDS_SHPARAM_GROUP");
        }

        public Parameter FindParameter(Element element, string parameterName)
        {
            if (element == null || string.IsNullOrEmpty(parameterName))
                return null;

            string searchName = parameterName;
            if (searchName.StartsWith("I:") || searchName.StartsWith("T:"))
                searchName = searchName.Substring(2);

            foreach (Parameter param in element.Parameters)
            {
                if (param.Definition.Name == searchName)
                    return param;
            }

            Element typeElement = GetTypeElement(element);
            if (typeElement != null)
            {
                foreach (Parameter param in typeElement.Parameters)
                {
                    if (param.Definition.Name == searchName)
                        return param;
                }
            }

            return null;
        }

        public Parameter GetParameter(Element element, BuiltInParameter builtIn)
        {
            if (element == null)
                return null;

            Parameter param = element.get_Parameter(builtIn);
            if (param != null)
                return param;

            Element typeElement = GetTypeElement(element);
            if (typeElement != null)
            {
                param = typeElement.get_Parameter(builtIn);
                if (param != null)
                    return param;
            }

            return null;
        }

        public Parameter GetParameter(Element element, string parameterName, object unused1, ForgeTypeId unused2)
        {
            if (element == null || string.IsNullOrEmpty(parameterName))
                return null;

            foreach (Parameter param in element.Parameters)
            {
                if (param.Definition.Name == parameterName)
                    return param;
            }

            return null;
        }

        private Element GetTypeElement(Element element)
        {
            if (element is FamilyInstance familyInstance)
            {
                if (familyInstance.Symbol != null)
                    return familyInstance.Symbol;
            }
            else if (element.CanHaveTypeAssigned())
            {
                ElementId typeId = element.GetTypeId();
                if (typeId != null && typeId != ElementId.InvalidElementId)
                    return element.Document.GetElement(typeId);
            }
            return null;
        }

        public bool SetDefinition(Element elem, IList<Category> categories, string defName, ForgeTypeId paramType, ForgeTypeId bltParamGroup, bool visible, int bindingMode)
        {
            return false;
        }

        public DefinitionFile GetSharedParameterFile()
        {
            try
            {
                return _rvtUIDoc.Application.Application.OpenSharedParameterFile();
            }
            catch
            {
                return null;
            }
        }
    }
}
