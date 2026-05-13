using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Document = Autodesk.Revit.DB.Document;
using Parameter = Autodesk.Revit.DB.Parameter;

namespace ADSK.JExtRAC.ExportExcel.Utils
{
    public class Common
    {
        public static List<Element> GetElements(Document document, ElementId viewId, bool includeType)
        {
            List<Element> alls = new List<Element>();

            try
            {
                FilteredElementCollector coll_instances = null;

                if (viewId != ElementId.InvalidElementId)
                    coll_instances = new FilteredElementCollector(document, viewId);
                else
                    coll_instances = new FilteredElementCollector(document);

                coll_instances = coll_instances.WhereElementIsNotElementType();

                var list_Instances = ((IEnumerable)((IEnumerable<Element>)coll_instances)
                        .Where<Element>((Func<Element, bool>)(obj0 => obj0 is Element && (obj0 as Element).Category != null)))
                        .Cast<Element>().ToList<Element>();

                alls.AddRange(list_Instances);

                if (includeType)
                {
                    FilteredElementCollector coll_types = null;

                    if (viewId != ElementId.InvalidElementId)
                        coll_types = new FilteredElementCollector(document, viewId);
                    else
                        coll_types = new FilteredElementCollector(document);

                    coll_types = coll_types.WhereElementIsElementType();

                    var list_types = ((IEnumerable)((IEnumerable<Element>)coll_types)
                        .Where<Element>((Func<Element, bool>)(obj0 => obj0 is Element && (obj0 as Element).Category != null)))
                        .Cast<Element>().ToList<Element>();

                    var categories = (from Element element in list_Instances
                                      where element.Category != null
                                      select element.Category.Id).ToList();

                    var _list_types = (from Element element in list_types
                                       where element.Category != null && categories.Contains(element.Category.Id) == false
                                       select element).ToList();

                    alls.AddRange(_list_types);
                }
                return alls;
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
            }
            return null;
        }

        public static double AsProjectUnitTypeDouble(Parameter param)
        {
            return UnitUtils.ConvertFromInternalUnits(param.AsDouble(), param.Element.Document.GetUnits().GetFormatOptions(param.Definition.GetDataType()).GetUnitTypeId());
        }

        private static string GetElementFamilyName(Document doc, ElementType elementType)
        {
            return elementType.FamilyName;
        }

        public static object GetParameterValue(Document doc, Parameter parameter)
        {
            object obj = null;
            switch (parameter.StorageType)
            {
                case StorageType.Integer:
                    obj = parameter.AsInteger();
                    if (parameter.Definition.GetDataType() == SpecTypeId.Boolean.YesNo)
                    {
                        obj = (int)obj == 0 ? "False" : "True";
                        break;
                    }
                    break;

                case StorageType.Double:
                    double num = AsProjectUnitTypeDouble(parameter);
                    try
                    {
                        obj = parameter.GetUnitTypeId() != UnitTypeId.Percentage ? num : (num / 100.0);
                        break;
                    }
                    catch (Exception ex)
                    {
                        string errMsg = ex.Message;
                        obj = num;
                        break;
                    }
                case StorageType.String:
                    obj = parameter.AsString();
                    break;

                case StorageType.ElementId:
                    ElementId elementId = parameter.AsElementId();
                    if (Int32.Parse(elementId.ToString()) < 0)
                    {
                        Category category = doc.Settings.Categories.get_Item((BuiltInCategory)Int32.Parse(elementId.ToString()));
                        if (category != null)
                        {
                            obj = category.Name;
                            break;
                        }
                        break;
                    }
                    if (Int32.Parse(parameter.Id.ToString()) == (int)BuiltInParameter.ELEM_FAMILY_PARAM ||
                        Int32.Parse(parameter.Id.ToString()) == (int)BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM)
                    {
                        ElementType elementType = doc.GetElement(elementId) as ElementType;
                        string elementFamilyName = Common.GetElementFamilyName(doc, elementType);
                        obj = Int32.Parse(parameter.Id.ToString()) != (int)BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM ?
                            elementFamilyName : (elementFamilyName + ": " + ((Element)elementType).Name);
                        break;
                    }
                    Element element = doc.GetElement(elementId);
                    if (element != null)
                    {
                        obj = element.Name;
                        break;
                    }
                    break;
            }
            return obj;
        }

        public static ElementId GetFamilyType(Element element)
        {
            if (element is FamilyInstance)
            {
                FamilyInstance inst = element as FamilyInstance;
                if (null != inst.Symbol)
                {
                    return inst.Symbol.Id;
                }
            }
            else if (element.CanHaveTypeAssigned())
            {
                ElementId typeId = element.GetTypeId();
                if (null != typeId && ElementId.InvalidElementId != typeId)
                {
                    return typeId;
                }
            }

            return ElementId.InvalidElementId;
        }
    }

    public enum eType_Instance_Parameter
    {
        Type = 0,
        Instance
    }

    public enum eSelectMode
    {
        Invalid = -1,
        All,
        CurrentView,
        Selection
    }

    public enum sDataType
    {
        General = 0,
        Text,
    }
}
