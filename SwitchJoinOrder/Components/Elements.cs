using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.SwitchJoinOrder.Entities;

namespace ADSK.JExtRAC.SwitchJoinOrder.Components
{
    public class Elements
    {
        public UIDocument RvtUIDoc { get; }
        public Document RvtDBDoc => RvtUIDoc.Document;

        public Elements(UIDocument rvtUIDoc)
        {
            RvtUIDoc = rvtUIDoc;
        }

        public void GetElementFamilyNameAndTypeName(Element element, ref string familyName, ref string typeName)
        {
            familyName = "";
            typeName = "";
            if (element == null) return;

            ElementId typeId = element.GetTypeId();
            if (typeId != ElementId.InvalidElementId)
            {
                ElementType elemType = RvtDBDoc.GetElement(typeId) as ElementType;
                if (elemType != null)
                {
                    familyName = elemType.FamilyName ?? "";
                    typeName = elemType.Name ?? "";
                }
            }

            if (string.IsNullOrEmpty(familyName) && element is FamilyInstance fi)
            {
                familyName = fi.Symbol?.FamilyName ?? "";
                typeName = fi.Symbol?.Name ?? "";
            }

            if (string.IsNullOrEmpty(familyName))
            {
                familyName = element.Name ?? "";
            }
        }

        public List<Element> getAllElement(UIDocument rvtUIDoc, Document docDB)
        {
            Autodesk.Revit.UI.Selection.Selection selection = rvtUIDoc.Selection;
            ICollection<ElementId> collection = selection.GetElementIds();
            List<Element> listElement = new List<Element>();
            if (0 == collection.Count)
            {
                List<ElementId> listAllElement = (List<ElementId>)
                    new FilteredElementCollector(docDB).WherePasses(
                        (ElementFilter)new ElementMulticlassFilter((IList<Type>)new List<Type>()
                        {
                          typeof(HostObject),
                          typeof(FamilyInstance)
                        })).WhereElementIsViewIndependent().ToElementIds();

                foreach (ElementId elem in listAllElement)
                {
                    Element melements = docDB.GetElement(elem);
                    listElement.AddRange(GetElementGroup(docDB, melements));
                }
            }
            else
            {
                foreach (ElementId elem in collection)
                {
                    Element melements = docDB.GetElement(elem);
                    listElement.AddRange(GetElementGroup(docDB, melements));
                }
            }
            return listElement;
        }

        private List<Element> GetElementGroup(Document docDB, Element melements)
        {
            List<Element> listElement = new List<Element>();
            if (melements is Group)
            {
                Group gr = melements as Group;
                IList<ElementId> listele = gr.GetMemberIds();
                foreach (ElementId id in listele)
                {
                    Element mEle = docDB.GetElement(id);
                    if (mEle is Group)
                        listElement.AddRange(GetElementGroup(docDB, mEle));
                    else
                    {
                        if (!string.IsNullOrEmpty(mEle.Name) && mEle.Category != null)
                            listElement.Add(mEle);
                    }
                }
            }
            else
                listElement.Add(melements);
            return listElement;
        }

        public CategoryItems GroupingData(Document doc, List<Element> listElement)
        {
            CategoryItems showItem = new CategoryItems();
            List<CategoryItem> listCata = new List<CategoryItem>();
            int i = 0;
            foreach (Element ele in listElement)
            {
                if (ele == null || ele.Category == null)
                    continue;
                if (!showItem.Contains(ele.Category.Name))
                {
                    CategoryItem cata = new CategoryItem();
                    cata._name = ele.Category.Name;
                    cata.AddElementToList(ele);
                    cata.index = i;
                    listCata.Add(cata);
                    if (showItem._categoryShow == null)
                        showItem._categoryShow = new List<CategoryItem>();
                    showItem._categoryShow.AddRange(listCata);
                    listCata.Clear();
                    i++;
                }
                else
                {
                    foreach (var item in showItem._categoryShow)
                    {
                        if (item._name == ele.Category.Name)
                            showItem._categoryShow[item.index]._listElementId.Add(ele.Id);
                    }
                }
            }

            if (showItem._categoryShow != null)
            {
                foreach (var cate in showItem._categoryShow)
                    GetFamily(doc, cate);
            }
            return showItem;
        }

        private void GetFamily(Document doc, CategoryItem categoryItem)
        {
            if (categoryItem == null || categoryItem._listElementId == null || categoryItem._listElementId.Count == 0)
                return;

            int i = 0;
            List<FamilyItem> listFami = new List<FamilyItem>();
            foreach (ElementId eleId in categoryItem._listElementId)
            {
                Element ele = doc.GetElement(eleId);
                if (ele == null || ele.Category == null)
                    continue;

                string familname = string.Empty;
                string type = string.Empty;
                GetElementFamilyNameAndTypeName(ele, ref familname, ref type);

                if (string.IsNullOrEmpty(familname))
                    continue;

                if (!categoryItem.ContainsFml(familname))
                {
                    FamilyItem fami = new FamilyItem();
                    fami._nameFami = familname;
                    fami._nameCate = ele.Category.Name;
                    fami._indexFami = i;
                    fami._indexCate = categoryItem.index;
                    fami.AddElementToList(ele);
                    listFami.Add(fami);
                    if (categoryItem._listFamilyItem == null)
                        categoryItem._listFamilyItem = new List<FamilyItem>();
                    categoryItem._listFamilyItem.AddRange(listFami);
                    listFami.Clear();
                    i++;
                }
                else
                {
                    foreach (var item in categoryItem._listFamilyItem)
                    {
                        if (item._nameFami == familname)
                            categoryItem._listFamilyItem[item._indexFami]._listElementIdOfFamily.Add(ele.Id);
                    }
                }
            }
        }

        public ICollection<ElementId> GetElementIntersectsFilter(Document rvtDBDoc, ICollection<ElementId> listFilter, Element aElement)
        {
            try
            {
                if (ElementIntersectsFilter.IsCategorySupported(aElement) == false)
                    return null;
                if (ElementIntersectsFilter.IsElementSupported(aElement) == false)
                    return null;

                var coll = new FilteredElementCollector(rvtDBDoc,
                    (listFilter)).WherePasses((ElementFilter)new ElementIntersectsElementFilter(aElement));

                if (coll.GetElementCount() == 0)
                    return null;

                ICollection<ElementId> collectorEle = ((List<ElementId>)coll.ToElementIds());
                return collectorEle;
            }
            catch
            {
                return null;
            }
        }

        private void GetSolid(GeometryObject geObject, List<Solid> listSolid, bool getSymbol = false)
        {
            if (geObject is Solid)
            {
                listSolid.Add(geObject as Solid);
            }
            if (geObject as GeometryElement != null)
            {
                GeometryElement geo = geObject as GeometryElement;
                IEnumerator<GeometryObject> Objects = geo.GetEnumerator();
                while (Objects.MoveNext())
                {
                    GeometryObject geObject1 = Objects.Current;
                    GetSolid(geObject1, listSolid, getSymbol);
                }
            }
            if (geObject as GeometryInstance != null)
            {
                GeometryInstance geometryInstance = geObject as GeometryInstance;
                GeometryElement geo = null;
                if (getSymbol == true)
                    geo = geometryInstance.GetSymbolGeometry();
                else
                    geo = geometryInstance.GetInstanceGeometry();
                IEnumerator<GeometryObject> Objects = geo.GetEnumerator();
                while (Objects.MoveNext())
                {
                    GeometryObject geObject1 = Objects.Current;
                    GetSolid(geObject1, listSolid, getSymbol);
                }
            }
        }

        public bool IsOverlappedBySolid(Autodesk.Revit.ApplicationServices.Application rvtApp,
            ref Dictionary<ElementId, List<Solid>> dicSolidsElement,
            Element aElement, Element xElement)
        {
            try
            {
                Options geOptions = rvtApp.Create.NewGeometryOptions();
                List<Solid> listSolidA = null;
                if (dicSolidsElement.TryGetValue(aElement.Id, out listSolidA) == false)
                {
                    GeometryElement geoA = aElement.get_Geometry(geOptions);
                    listSolidA = new List<Solid>();
                    GetSolid(geoA, listSolidA);
                    dicSolidsElement.Add(aElement.Id, listSolidA);
                }

                List<Solid> listSolidB = null;
                if (dicSolidsElement.TryGetValue(xElement.Id, out listSolidB) == false)
                {
                    GeometryElement geoB = xElement.get_Geometry(geOptions);
                    listSolidB = new List<Solid>();
                    GetSolid(geoB, listSolidB);
                    dicSolidsElement.Add(xElement.Id, listSolidB);
                }

                listSolidA = listSolidA.Where(s => s.Volume != 0).ToList();
                listSolidB = listSolidB.Where(s => s.Volume != 0).ToList();
                foreach (Solid solidA in listSolidA)
                {
                    foreach (Solid solidB in listSolidB)
                    {
                        try
                        {
                            Solid intersection = BooleanOperationsUtils.ExecuteBooleanOperation(solidA, solidB, BooleanOperationsType.Intersect);
                            Solid intersection1 = BooleanOperationsUtils.ExecuteBooleanOperation(solidB, solidA, BooleanOperationsType.Intersect);
                            if (intersection.Volume == 0.0 && intersection1.Volume == 0.0)
                                continue;
                            if (intersection.Edges.Size == 0 && intersection.Faces.Size == 0)
                            {
                            }
                            else
                            {
                                return true;
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return false;
        }
    }
}
