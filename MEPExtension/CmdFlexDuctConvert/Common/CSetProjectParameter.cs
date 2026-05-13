using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Revit関連のNamespace
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

//その他
using System.IO;

namespace nSetProjectParameter
{
    public class CSetProjectParameter
    {
        private static Definition GetDefiniton(BindingMap map, string name)
        {
            DefinitionBindingMapIterator it = map.ForwardIterator();
            it.Reset();
            while (it.MoveNext())
            {
                if (it.Key.Name == name)
                {
                    return it.Key;
                }
            }
            return null;
        }

        private static void RawCreateProjectParameter(
            Autodesk.Revit.ApplicationServices.Application app,
            string name,
            ForgeTypeId type,
            bool visible,
            CategorySet cats,
            Autodesk.Revit.DB.ForgeTypeId group, bool inst)
        {
            string oriFile = app.SharedParametersFilename;
            string tempFile = Path.GetTempFileName() + ".txt";
            using (File.Create(tempFile)) { }
            app.SharedParametersFilename = tempFile;

            var defOptions = new ExternalDefinitionCreationOptions(name, type) { Visible = visible };
            ExternalDefinition def = app.OpenSharedParameterFile().Groups.Create("TemporaryDefintionGroup").Definitions.Create(defOptions) as ExternalDefinition;

            app.SharedParametersFilename = oriFile;
            File.Delete(tempFile);

            Autodesk.Revit.DB.Binding binding = app.Create.NewTypeBinding(cats);
            if (inst)
            {
                binding = app.Create.NewInstanceBinding(cats);
            }

            BindingMap map = (new UIApplication(app)).ActiveUIDocument.Document.ParameterBindings;
            map.Insert(def, binding, group);
        }

        public static bool CreateProjectParameter(
            UIApplication uiapp,
            string name,
            ForgeTypeId type,
            bool visible,
            CategorySet cats,
            Autodesk.Revit.DB.ForgeTypeId group,
            bool inst)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            BindingMap map = doc.ParameterBindings;
            if (GetDefiniton(map, name) == null)
            {
                RawCreateProjectParameter(app, name, type, visible, cats, group, inst);
                return true;
            }
            return false;
        }
    }
}