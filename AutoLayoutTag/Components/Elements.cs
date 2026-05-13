using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Revit = Autodesk.Revit;
using Collections = System.Collections;

namespace ADSK.JExtRAC.AutoLayoutTag.Components
{
    public class Elements
    {
        private List<BuiltInCategory> ListCategoryDefault = new List<BuiltInCategory>()
        {
            BuiltInCategory.OST_Doors,
            BuiltInCategory.OST_Windows,
            BuiltInCategory.OST_Furniture,
            BuiltInCategory.OST_Floors,
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_Ceilings,
            BuiltInCategory.OST_Columns,
            BuiltInCategory.OST_GenericModel,
            BuiltInCategory.OST_StructuralColumns,
            BuiltInCategory.OST_StructuralFraming,
            BuiltInCategory.OST_StructuralFoundation,
            BuiltInCategory.OST_DuctCurves,
            BuiltInCategory.OST_DuctFitting,
            BuiltInCategory.OST_FlexDuctCurves,
            BuiltInCategory.OST_DuctAccessory,
            BuiltInCategory.OST_DuctTerminal,
            BuiltInCategory.OST_PipeCurves,
            BuiltInCategory.OST_PipeFitting,
            BuiltInCategory.OST_PipeAccessory,
            BuiltInCategory.OST_MechanicalEquipment,
            BuiltInCategory.OST_LightingFixtures,
            BuiltInCategory.OST_LightingDevices,
            BuiltInCategory.OST_SpecialityEquipment,
            BuiltInCategory.OST_PlumbingFixtures,
            BuiltInCategory.OST_ElectricalFixtures,
            BuiltInCategory.OST_ElectricalEquipment,
            BuiltInCategory.OST_Conduit,
            BuiltInCategory.OST_ConduitFitting,
            BuiltInCategory.OST_CableTray,
            BuiltInCategory.OST_CableTrayFitting
        };

        private Document _Doc;
        private const double distance = 1000;
        private readonly UIDocument _rvtUIDoc;

        public Document RvtDBDoc { get; }

        public Elements(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            _Doc = rvtUIDoc.Document;
            RvtDBDoc = rvtUIDoc.Document;
        }

        public Category GetCategory(BuiltInCategory builtIn)
        {
            return Category.GetCategory(RvtDBDoc, builtIn);
        }

        public string GetCategoryName(BuiltInCategory builtIn)
        {
            Category cat = GetCategory(builtIn);
            return cat?.Name ?? builtIn.ToString();
        }

        public Element GetElementDoc(int elementId)
        {
            return RvtDBDoc.GetElement(new ElementId((long)elementId));
        }

        public Category GetCategoryTag(Category cateEle)
        {
            if (cateEle == null)
                return null;

            Category retval = null;

            switch (Int32.Parse(cateEle.Id.ToString()))
            {
                case (int)(BuiltInCategory.OST_Doors):
                    retval = GetCategory(BuiltInCategory.OST_DoorTags); break;
                case (int)(BuiltInCategory.OST_Windows):
                    retval = GetCategory(BuiltInCategory.OST_WindowTags); break;
                case (int)(BuiltInCategory.OST_Furniture):
                    retval = GetCategory(BuiltInCategory.OST_FurnitureTags); break;
                case (int)(BuiltInCategory.OST_Floors):
                    retval = GetCategory(BuiltInCategory.OST_FloorTags); break;
                case (int)(BuiltInCategory.OST_Walls):
                    retval = GetCategory(BuiltInCategory.OST_WallTags); break;
                case (int)(BuiltInCategory.OST_Ceilings):
                    retval = GetCategory(BuiltInCategory.OST_CeilingTags); break;
                case (int)(BuiltInCategory.OST_Columns):
                    retval = GetCategory(BuiltInCategory.OST_StructuralColumnTags); break;
                case (int)(BuiltInCategory.OST_GenericModel):
                    retval = GetCategory(BuiltInCategory.OST_GenericModelTags); break;
                case (int)(BuiltInCategory.OST_StructuralColumns):
                    retval = GetCategory(BuiltInCategory.OST_StructuralColumnTags); break;
                case (int)(BuiltInCategory.OST_StructuralFraming):
                    retval = GetCategory(BuiltInCategory.OST_StructuralFramingTags); break;
                case (int)(BuiltInCategory.OST_StructuralFoundation):
                    retval = GetCategory(BuiltInCategory.OST_StructuralFoundationTags); break;
                case (int)(BuiltInCategory.OST_DuctCurves):
                    retval = GetCategory(BuiltInCategory.OST_DuctTags); break;
                case (int)(BuiltInCategory.OST_DuctFitting):
                    retval = GetCategory(BuiltInCategory.OST_DuctFittingTags); break;
                case (int)(BuiltInCategory.OST_FlexDuctCurves):
                    retval = GetCategory(BuiltInCategory.OST_FlexDuctTags); break;
                case (int)(BuiltInCategory.OST_DuctAccessory):
                    retval = GetCategory(BuiltInCategory.OST_DuctAccessoryTags); break;
                case (int)(BuiltInCategory.OST_DuctTerminal):
                    retval = GetCategory(BuiltInCategory.OST_DuctTerminalTags); break;
                case (int)(BuiltInCategory.OST_PipeCurves):
                    retval = GetCategory(BuiltInCategory.OST_PipeTags); break;
                case (int)(BuiltInCategory.OST_PipeFitting):
                    retval = GetCategory(BuiltInCategory.OST_PipeFittingTags); break;
                case (int)(BuiltInCategory.OST_PipeAccessory):
                    retval = GetCategory(BuiltInCategory.OST_PipeAccessoryTags); break;
                case (int)(BuiltInCategory.OST_MechanicalEquipment):
                    retval = GetCategory(BuiltInCategory.OST_MechanicalEquipmentTags); break;
                case (int)(BuiltInCategory.OST_LightingFixtures):
                    retval = GetCategory(BuiltInCategory.OST_LightingFixtureTags); break;
                case (int)(BuiltInCategory.OST_LightingDevices):
                    retval = GetCategory(BuiltInCategory.OST_LightingDeviceTags); break;
                case (int)(BuiltInCategory.OST_SpecialityEquipment):
                    retval = GetCategory(BuiltInCategory.OST_SpecialityEquipmentTags); break;
                case (int)(BuiltInCategory.OST_PlumbingFixtures):
                    retval = GetCategory(BuiltInCategory.OST_PlumbingFixtureTags); break;
                case (int)(BuiltInCategory.OST_ElectricalFixtures):
                    retval = GetCategory(BuiltInCategory.OST_ElectricalFixtureTags); break;
                case (int)(BuiltInCategory.OST_ElectricalEquipment):
                    retval = GetCategory(BuiltInCategory.OST_ElectricalEquipmentTags); break;
                case (int)(BuiltInCategory.OST_Conduit):
                    retval = GetCategory(BuiltInCategory.OST_ConduitTags); break;
                case (int)(BuiltInCategory.OST_ConduitFitting):
                    retval = GetCategory(BuiltInCategory.OST_ConduitFittingTags); break;
                case (int)(BuiltInCategory.OST_CableTray):
                    retval = GetCategory(BuiltInCategory.OST_CableTrayTags); break;
                case (int)(BuiltInCategory.OST_CableTrayFitting):
                    retval = GetCategory(BuiltInCategory.OST_CableTrayFittingTags); break;
            }

            return retval;
        }

        public System.Data.DataTable DataCategory()
        {
            System.Data.DataTable retVal = new System.Data.DataTable();
            System.Data.DataRow row;
            retVal.Columns.Add("Name", typeof(string));
            retVal.Columns.Add("Value", typeof(BuiltInCategory));
            foreach (var built in ListCategoryDefault)
            {
                row = retVal.NewRow();
                row["Name"] = GetCategoryName(built);
                row["Value"] = built;
                retVal.Rows.Add(row);
            }
            return retVal;
        }

        public Collections.Generic.List<ElementId> GetAllTagOfElement(Document doc, Element ele)
        {
            if (ele.Category == null)
                return null;

            Category cateTag = GetCategoryTag(ele.Category);
            if (cateTag == null)
                return null;

            return new FilteredElementCollector(doc, doc.ActiveView.Id)
                    .OfClass(typeof(IndependentTag))
                    .WhereElementIsNotElementType()
                    .Cast<IndependentTag>()
                    .Where(sym => sym.Category.Id == cateTag.Id && sym.GetTaggedLocalElementIds().FirstOrDefault() == ele.Id)
                    .Select(x => x.Id)
                    .ToList();
        }

        public Collections.Generic.List<FamilySymbol> GetAllType(Document doc, Category cateEle)
        {
            Category cateTag = GetCategoryTag(cateEle);
            if (cateTag == null)
                return null;

            return new FilteredElementCollector(doc)
                       .OfClass(typeof(FamilySymbol))
                       .WhereElementIsElementType()
                       .Cast<FamilySymbol>()
                       .Where(sym => sym.Category.Id == cateTag.Id).ToList();
        }

        public Collections.Generic.List<Element> PickElements(UIDocument uiDoc, ISelectionFilter filter, string message)
        {
            Collections.Generic.List<Element> elements = new Collections.Generic.List<Element>();
            try
            {
                Collections.Generic.IList<Reference> references = uiDoc.Selection.PickObjects(ObjectType.Element, filter, message);
                if (references?.Count() > 0)
                {
                    foreach (Reference reference in references)
                        elements.Add(uiDoc.Document.GetElement(reference));
                }
            }
            catch (System.Exception ex)
            {
                var mes = ex.Message;
            }
            return elements;
        }

        public static Outline CreateOutline(UIDocument uidoc, string masagge)
        {
            PickedBox pickedBox = uidoc.Selection.PickBox(PickBoxStyle.Directional, masagge);
            if (pickedBox == null)
                return null;

            double minX = pickedBox.Min.X;
            double minY = pickedBox.Min.Y;
            double maxX = pickedBox.Max.X;
            double maxY = pickedBox.Max.Y;

            double sminX = System.Math.Min(minX, maxX);
            double sminY = System.Math.Min(minY, maxY);
            double smaxX = System.Math.Max(minX, maxX);
            double smaxY = System.Math.Max(minY, maxY);

            Outline outline = new Outline(new XYZ(sminX, sminY, -distance), new XYZ(smaxX, smaxY, distance));
            return outline;
        }

        public View ActiveView
        {
            get
            {
                View view = null;
                View activeView = RvtDBDoc.ActiveView;
                if (activeView != null)
                {
                    if (activeView is ViewPlan)
                        view = activeView as ViewPlan;
                }
                return view;
            }
        }
    }
}
