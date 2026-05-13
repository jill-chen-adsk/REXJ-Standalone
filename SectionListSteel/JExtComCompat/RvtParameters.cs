using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace SectionListSteel.JExtComCompat
{
    public class RvtParameters
    {
        private readonly UIDocument _uidoc;
        private readonly Document _document;
        private readonly Autodesk.Revit.ApplicationServices.Application _app;

        public RvtParameters(UIDocument uidoc)
        {
            _uidoc = uidoc;
            _document = uidoc.Document;
            _app = uidoc.Application.Application;
        }

        public Document Document => _document;

        public DefinitionFile? GetSharedParameterFile()
        {
            try
            {
                return _app.OpenSharedParameterFile();
            }
            catch
            {
                return null;
            }
        }

        public DefinitionFile? SetSharedParameterFileFromPath(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath).Dispose();
            }

            try
            {
                _app.SharedParametersFilename = fullPath;
                return _app.OpenSharedParameterFile();
            }
            catch
            {
                return null;
            }
        }

        public DefinitionFile? SetSharedParameterFile(string? folderName, string fileName)
        {
            string fullPath;
            if (!string.IsNullOrEmpty(folderName))
            {
                fullPath = Path.Combine(folderName, fileName);
            }
            else
            {
                fullPath = fileName;
            }
            return SetSharedParameterFileFromPath(fullPath);
        }

        public bool SetDefinition(
            Element? elem,
            string folder,
            string file,
            string group,
            IList<Category> categories,
            string defName,
            ForgeTypeId paramType,
            ForgeTypeId paramGroup,
            bool visible,
            int bindingMode)
        {
            Definition? existing = null;
            if (elem != null)
            {
                existing = GetDefinitionFromElement(elem, defName, paramType, paramGroup);
            }

            if (existing != null)
            {
                return InsertBinding(existing, categories, paramGroup, bindingMode);
            }

            existing = GetDefinitionFromBindingMap(defName, paramType);
            if (existing != null)
            {
                return InsertBinding(existing, categories, paramGroup, bindingMode);
            }

            var defFile = SetSharedParameterFile(folder, file);
            if (defFile == null)
            {
                return false;
            }

            DefinitionGroup? defGroup = null;
            foreach (DefinitionGroup g in defFile.Groups)
            {
                if (g.Name == group)
                {
                    defGroup = g;
                    break;
                }
            }
            defGroup ??= defFile.Groups.Create(group);

            ExternalDefinition? extDef = GetExternalDefinition(defGroup, defName, paramType);
            if (extDef == null)
            {
                var options = new ExternalDefinitionCreationOptions(defName, paramType)
                {
                    Visible = visible,
                };
                extDef = defGroup.Definitions.Create(options) as ExternalDefinition;
            }

            if (extDef == null)
            {
                return false;
            }

            return InsertBinding(extDef, categories, paramGroup, bindingMode);
        }

        private Definition? GetDefinitionFromElement(Element elem, string defName, ForgeTypeId paramType, ForgeTypeId paramGroup)
        {
            foreach (Parameter p in elem.Parameters)
            {
                if (p.Definition?.Name != defName)
                {
                    continue;
                }
                if (p.Definition.GetDataType() != paramType)
                {
                    continue;
                }
                if (paramGroup.TypeId != string.Empty
                    && p.Definition.GetGroupTypeId() != paramGroup)
                {
                    continue;
                }
                return p.Definition;
            }
            return null;
        }

        private Definition? GetDefinitionFromBindingMap(string defName, ForgeTypeId dataType)
        {
            var it = _document.ParameterBindings.ForwardIterator();
            while (it.MoveNext())
            {
                var d = it.Key as Definition;
                if (d == null || d.Name != defName)
                {
                    continue;
                }
                if (d.GetDataType() == dataType)
                {
                    return d;
                }
            }
            return null;
        }

        private ExternalDefinition? GetExternalDefinition(DefinitionGroup defGroup, string defName, ForgeTypeId dataType)
        {
            foreach (Definition d in defGroup.Definitions)
            {
                if (d.Name == defName && d.GetDataType() == dataType)
                {
                    return d as ExternalDefinition;
                }
            }
            return null;
        }

        private bool InsertBinding(
            Definition definition,
            IList<Category> categories,
            ForgeTypeId paramGroup,
            int bindingMode)
        {
            var set = _app.Create.NewCategorySet();
            var seen = new HashSet<long>();
            foreach (var c in categories)
            {
                long id = c.Id.Value;
                if (seen.Add(id))
                {
                    set.Insert(c);
                }
            }

            Binding binding = bindingMode == 1
                ? _app.Create.NewTypeBinding(set)
                : _app.Create.NewInstanceBinding(set);

            if (paramGroup.TypeId == string.Empty)
            {
                return _document.ParameterBindings.Insert(definition, binding);
            }
            return _document.ParameterBindings.Insert(definition, binding, paramGroup);
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref int value)
        {
            Parameter? p = elem.get_Parameter(bip);
            if (p == null || p.StorageType != StorageType.Integer)
            {
                return -2;
            }
            value = p.AsInteger();
            return 0;
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref bool value)
        {
            Parameter? p = elem.get_Parameter(bip);
            if (p == null || p.StorageType != StorageType.Integer)
            {
                return -2;
            }
            value = p.AsInteger() != 0;
            return 0;
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref string value)
        {
            Parameter? p = elem.get_Parameter(bip);
            if (p == null)
            {
                return -2;
            }
            if (p.StorageType == StorageType.String)
            {
                value = p.AsString() ?? "";
                return string.IsNullOrEmpty(value) ? -1 : 0;
            }
            value = p.AsValueString() ?? "";
            return string.IsNullOrEmpty(value) ? -1 : 0;
        }

        public int GetValue(Element elem, BuiltInParameter bip, ref ElementId value)
        {
            Parameter? p = elem.get_Parameter(bip);
            if (p == null || p.StorageType != StorageType.ElementId)
            {
                return -2;
            }
            value = p.AsElementId();
            return 0;
        }

        public int GetValue(Element elem, string defName, ForgeTypeId paramType, ForgeTypeId paramGroup, ref int value)
        {
            Parameter? p = FindParameter(elem, defName, paramType, paramGroup);
            if (p == null)
            {
                return -2;
            }
            if (!string.IsNullOrEmpty(p.AsValueString()) && int.TryParse(p.AsValueString(), out int v))
            {
                value = v;
                return 0;
            }
            return -1;
        }

        public int GetValue(Element elem, string defName, ForgeTypeId paramType, ForgeTypeId paramGroup, ref bool value)
        {
            Parameter? p = FindParameter(elem, defName, paramType, paramGroup);
            if (p == null)
            {
                return -2;
            }
            value = p.AsInteger() != 0;
            return 0;
        }

        public int GetValue(Element elem, string defName, ForgeTypeId paramType, ForgeTypeId paramGroup, ref string value)
        {
            Parameter? p = FindParameter(elem, defName, paramType, paramGroup);
            if (p == null)
            {
                return -2;
            }
            value = p.AsString() ?? "";
            return string.IsNullOrEmpty(value) ? -1 : 0;
        }

        public int SetValue(Element elem, BuiltInParameter bip, int value)
        {
            Parameter? p = elem.get_Parameter(bip);
            if (p == null || p.IsReadOnly)
            {
                return -2;
            }
            if (p.StorageType == StorageType.Integer)
            {
                p.Set(value);
                return 0;
            }
            return -2;
        }

        public int SetValue(Element elem, BuiltInParameter bip, bool value)
        {
            Parameter? p = elem.get_Parameter(bip);
            if (p == null)
            {
                return -2;
            }
            if (p.StorageType == StorageType.Integer)
            {
                p.Set(value ? 1 : 0);
                return 0;
            }
            return -2;
        }

        public int SetValue(Element elem, BuiltInParameter bip, string value)
        {
            Parameter? p = elem.get_Parameter(bip);
            if (p == null)
            {
                return -2;
            }
            if (p.StorageType == StorageType.String)
            {
                p.Set(value);
                return 0;
            }
            return -2;
        }

        public int SetValue(Element elem, BuiltInParameter bip, ElementId value)
        {
            Parameter? p = elem.get_Parameter(bip);
            if (p == null)
            {
                return -2;
            }
            if (p.StorageType == StorageType.ElementId)
            {
                p.Set(value);
                return 0;
            }
            return -2;
        }

        public int SetValue(Element elem, string defName, ForgeTypeId paramType, ForgeTypeId paramGroup, int value)
        {
            Parameter? p = FindParameter(elem, defName, paramType, paramGroup);
            if (p == null || p.IsReadOnly)
            {
                return -2;
            }
            p.Set(value);
            return 0;
        }

        public int SetValue(Element elem, string defName, ForgeTypeId paramType, ForgeTypeId paramGroup, bool value)
        {
            Parameter? p = FindParameter(elem, defName, paramType, paramGroup);
            if (p == null || p.IsReadOnly)
            {
                return -2;
            }
            p.Set(value ? 1 : 0);
            return 0;
        }

        public int SetValue(Element elem, string defName, ForgeTypeId paramType, ForgeTypeId paramGroup, string value)
        {
            Parameter? p = FindParameter(elem, defName, paramType, paramGroup);
            if (p == null || p.IsReadOnly)
            {
                return -2;
            }
            p.Set(value);
            return 0;
        }

        private Parameter? FindParameter(Element elem, string defName, ForgeTypeId paramType, ForgeTypeId paramGroup)
        {
            foreach (Parameter p in elem.Parameters)
            {
                if (p.Definition?.Name != defName)
                {
                    continue;
                }
                if (p.Definition.GetDataType() != paramType)
                {
                    continue;
                }
                if (paramGroup.TypeId != string.Empty
                    && p.Definition.GetGroupTypeId() != paramGroup)
                {
                    continue;
                }
                return p;
            }
            return null;
        }
    }
}
