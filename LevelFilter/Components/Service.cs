using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.LevelFilter.Components
{
    public class Service
    {
        private readonly Attribute _cmpAttribute;
        private readonly Elements _cmpElements;
        private readonly Parameters _cmpParameters;
        private readonly Settings _cmpSettings;

        public Service(Attribute cmpAttribute, Elements cmpElements, Parameters cmpParameters, Settings cmpSettings)
        {
            _cmpAttribute = cmpAttribute;
            _cmpElements = cmpElements;
            _cmpParameters = cmpParameters;
            _cmpSettings = cmpSettings;
        }

        public void GetFormData(Document doc,
            IList<ParameterFilterElement> lstRuleFilter,
            IList<Element> elemSet,
            IList<Material> materialSet,
            ref Dictionary<ElementId, IList<ElementId>> dicCat,
            ref Dictionary<string, IList<ElementId>> dicFam,
            ref Dictionary<string, IList<ElementId>> dicFamType,
            ref Dictionary<ElementId, IList<ElementId>> dicPart,
            ref Dictionary<ElementId, IList<ElementId>> dicFilter)
        {
            foreach (Element elem in elemSet)
            {
                ElementId key = ElementId.InvalidElementId;
                if (elem.Category != null)
                    key = elem.Category.Id;

                if (dicCat.ContainsKey(key))
                    dicCat[key].Add(elem.Id);
                else
                {
                    IList<ElementId> elementIdList = new List<ElementId>();
                    elementIdList.Add(elem.Id);
                    dicCat.Add(key, elementIdList);
                }

                if (elem.Category != null)
                {
                    var familyParameter = elem.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM);
                    if (familyParameter != null)
                    {
                        string cateName = string.Empty;
                        string familyName = familyParameter.AsValueString();
                        if (elem.Category != null)
                            cateName = elem.Category.Name;
                        if (!string.IsNullOrEmpty(familyName))
                        {
                            string keyStr = string.Format("[{0}]:{1}", cateName, familyName);
                            if (dicFam.ContainsKey(keyStr))
                                dicFam[keyStr].Add(elem.Id);
                            else
                            {
                                IList<ElementId> elementIdList = new List<ElementId>();
                                elementIdList.Add(elem.Id);
                                dicFam.Add(keyStr, elementIdList);
                            }
                        }
                    }
                }

                string s1 = elem.Name;
                if (s1 == "") s1 = "???";
                string s2;
                if (elem.Category != null)
                {
                    s2 = elem.Category.Name;
                    FamilyInstance familyInstance = elem as FamilyInstance;
                    if (familyInstance != null)
                        s1 = s1 + "(" + familyInstance.Symbol.Family.Name + ")";
                    else if (elem.GetTypeId() != ElementId.InvalidElementId)
                    {
                        var familyType = _cmpElements.GetElementDoc((int)elem.GetTypeId().Value);
                        if (familyType != null)
                            s1 = familyType.Name;
                    }

                    if (elem.Category.Id.Value == (long)(int)BuiltInCategory.OST_Lines)
                    {
                        CurveElement curveElement = elem as CurveElement;
                        if (curveElement != null)
                            s1 = s1 + "(" + curveElement.LineStyle.Name + ")";
                    }
                    if (elem.Category.Id.Value == (long)(int)BuiltInCategory.OST_Rooms)
                    {
                        int lastSpace = s1.LastIndexOf(" ");
                        if (lastSpace > 0)
                            s1 = s1.Substring(0, lastSpace);
                    }
                }
                else
                    s2 = _cmpAttribute.ResourceText("IDS_TXT_OTHER");

                string keyStrFamilyType = string.Format("{0}:{1}", s2, s1);

                bool exist = false;
                var find = dicFamType.Keys.ToList().Find(item => item == keyStrFamilyType);
                if (find != null) exist = true;

                if (exist)
                    dicFamType[find].Add(elem.Id);
                else
                {
                    IList<ElementId> elementIdList = new List<ElementId>();
                    elementIdList.Add(elem.Id);
                    dicFamType.Add(keyStrFamilyType, elementIdList);
                }
            }

            foreach (Material material in materialSet)
            {
                if (material != null)
                {
                    ElementId mkey = material.Id;
                    if (dicPart.ContainsKey(mkey))
                        dicPart[mkey].Add(material.Id);
                    else
                    {
                        IList<ElementId> elementIdList = new List<ElementId>();
                        elementIdList.Add(material.Id);
                        dicPart.Add(mkey, elementIdList);
                    }
                }
            }

            List<ElementId> elemSetId = new List<ElementId>();
            foreach (var ele in elemSet)
            {
                if (ele == null) continue;
                elemSetId.Add(ele.Id);
            }
            foreach (Element ele in lstRuleFilter)
            {
                if (ele == null) continue;
                ParameterFilterElement pfe = ele as ParameterFilterElement;
                if (pfe == null) continue;
                ElementFilter eleFilter = pfe.GetElementFilter();
                var categoryIds = pfe.GetCategories();
                if (categoryIds.Count == 0) continue;
                var lstCategory = new List<ElementFilter>();
                foreach (var cate in categoryIds)
                {
                    if (cate == null) continue;
                    var elementCategoryFilter = new ElementCategoryFilter(cate);
                    lstCategory.Add(elementCategoryFilter);
                }

                LogicalOrFilter category = new LogicalOrFilter(lstCategory);
                if (eleFilter == null) continue;

                LogicalAndFilter andFilter = new LogicalAndFilter(category, eleFilter);

                var eleIds = new FilteredElementCollector(doc, elemSetId)
                   .WherePasses(andFilter)
                   .ToElementIds().ToList();

                dicFilter.Add(pfe.Id, eleIds);
            }
        }
    }
}
