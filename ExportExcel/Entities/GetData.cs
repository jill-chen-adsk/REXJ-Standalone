using ADSK.JExtRAC.ExportExcel.Utils;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace ADSK.JExtRAC.ExportExcel.Entities
{
    public class GetData
    {
        public static List<CategoryItem> GetCategories(List<Element> elementList)
        {
            if (elementList == null || elementList.Count == 0)
                return null;

            List<CategoryItem> categories = new List<CategoryItem>();
            foreach (Element element in elementList)
            {
                if (element.Category == null)
                    continue;

                var exist = categories.Find(item => item.ElementId == element.Category.Id);

                if (exist == null)
                {
                    var cateItem = new CategoryItem(element.Category.Name, element.Category.Id);
                    categories.Add(cateItem);
                }
            }

            categories.Sort(delegate (CategoryItem item1, CategoryItem item2)
            {
                return item1.ToString().CompareTo(item2.ToString());
            });

            return categories;
        }

        public static void GetParameters(Document doc, CategoryItem categoryItem, List<Element> elementList)
        {
            if (categoryItem._Parameters == null || categoryItem._Parameters.Count == 0)
            {
                var elements = (from Element element in elementList
                                where element.Category != null && element.Category.Id == categoryItem.ElementId
                                select element).ToList();

                categoryItem._Parameters = GetAllParameters(doc, elements);
            }
        }

        private static void AddItems(List<string> parameterList, List<string> parameterAllList)
        {
            var others = (from string type in parameterList
                          where parameterAllList.Contains(type) == false
                          select type);

            parameterAllList.AddRange(others);
        }

        public static List<ParameterData> GetAllParameters(Document doc, List<Element> elements)
        {
            List<string> typeParameters = new List<string>();
            List<string> instanceParameters = new List<string>();

            bool hasInstance = false;
            foreach (Element element in elements)
            {
                if (Common.GetFamilyType(element) != ElementId.InvalidElementId)
                {
                    var parameters = GetNameParameters(doc, element, eType_Instance_Parameter.Type);
                    AddItems(parameters, typeParameters);

                    parameters = GetNameParameters(doc, element, eType_Instance_Parameter.Instance);
                    AddItems(parameters, instanceParameters);

                    hasInstance = true;
                }
                else
                {
                    var parameters = GetNameParameters(doc, element, eType_Instance_Parameter.Instance);
                    AddItems(parameters, instanceParameters);
                }
            }

            typeParameters.Sort(delegate (string str1, string str2)
            {
                return str1.CompareTo(str2);
            });

            instanceParameters.Sort(delegate (string str1, string str2)
            {
                return str1.CompareTo(str2);
            });

            List<ParameterData> all_paras = new List<ParameterData>();

            foreach (string orignalName in instanceParameters)
            {
                string name = string.Format("{0}{1}", Setting._Prefix_Instance, orignalName);

                var find = all_paras.Find(item => item.OrignalName == orignalName);
                if (find == null)
                {
                    var parameterData = new ParameterData(orignalName, name);
                    all_paras.Add(parameterData);
                }
            }

            foreach (string orignalName in typeParameters)
            {
                string name = string.Format("{0}{1}", Setting._Prefix_Type, orignalName);

                var find = all_paras.Find(item => item.OrignalName == orignalName);
                if (find == null)
                {
                    var parameterData = new ParameterData(orignalName, name);
                    all_paras.Add(parameterData);
                }
            }

            if (hasInstance)
            {
                List<string> addition = new List<string>()
                {
                    "ID",
                    "Room",
                    "ToRoom",
                    "FromRoom",
                    "Host",
                    "Space",
                    "SpaceName",
                    Resources.Text.IDS_PSEUDO_TYPE_GUID,
                };
                foreach (string parameterName in addition)
                {
                    var find = all_paras.Find(item => item.OrignalName == parameterName);
                    if (find == null)
                    {
                        var parameterDataId = new ParameterData(parameterName, "I:" + parameterName);
                        all_paras.Add(parameterDataId);
                    }
                }
            }

            string count_name = Resources.Text.IDS_PSEUDO_COUNT;
            var find_exist = all_paras.Find(item => item.OrignalName == count_name);
            if (find_exist == null)
            {
                var parameterCount = new ParameterData(count_name, count_name);
                all_paras.Add(parameterCount);
            }

            return all_paras;
        }

        private static List<string> GetNameParameters(Document doc, Element element, eType_Instance_Parameter eParameter)
        {
            List<string> parameterList = new List<string>();

            if (eParameter == eType_Instance_Parameter.Type)
            {
                if (element is FamilyInstance)
                {
                    FamilyInstance inst = element as FamilyInstance;
                    if (null != inst.Symbol)
                    {
                        element = inst.Symbol;
                    }
                }
                else if (element.CanHaveTypeAssigned())
                {
                    ElementId typeId = element.GetTypeId();
                    if (null != typeId && ElementId.InvalidElementId != typeId)
                    {
                        Element type = doc.GetElement(typeId);

                        if (null != type)
                        {
                            element = type;
                        }
                    }
                }
            }

            if (element != null)
            {
                foreach (Parameter p in element.Parameters)
                {
                    if (p.Definition is null) continue;
                    if (parameterList.Contains(p.Definition.Name) == false)
                    {
                        parameterList.Add(p.Definition.Name);
                    }
                }
            }

            return parameterList;
        }
    }
}
