using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning ;
using System.Text;
using System.Threading.Tasks;

using Autodesk;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
namespace MappingTable
{
    class LoadFamily
    {
        internal class ProjectFamily
        {
            internal string FamilyName;
            internal string FamilyTypeName;
        }

        internal  static List<ProjectFamily> FamilyNameList = new List<ProjectFamily>();

        internal static List<Family> ProFami = new List<Family>();

        internal class LevelList
        {
            internal ElementId id;
            internal string name;
            internal double elevation;
        }
        internal  List<Grid> GridX = new List<Grid>();
        internal  List<Grid> GridY = new List<Grid>();
        internal  List<LevelList> LevelNameList = new List<LevelList>();
        internal List<Material> RevitMatName = new List<Material>();
        internal List<Material> RevitConcName = new List<Material>();

        internal List<ViewPlan> VPlan = new List<ViewPlan>();


        internal static bool LoadFfamily_fromProject()
        {
            bool ret = true;
            FamilyNameList = new List<ProjectFamily>();
            ProFami = new List<Family>();

            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementFilter filter1 = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            ElementFilter filter2 = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            ElementFilter filter3 = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            ElementFilter filter4 = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
            //基礎・・・？基礎のファミリができたら確認
            ElementFilter filter5 = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation);
            //柱脚(センクシア)→構造接合
            ElementFilter filter6 = new ElementCategoryFilter(BuiltInCategory.OST_StructConnections);
            LogicalOrFilter filter = new LogicalOrFilter(new List<ElementFilter>() { filter1, filter2, filter3, filter4, filter5, filter6 });
            

            IList<Element> elements = collector.WherePasses(filter).WhereElementIsElementType().ToElements();

            try
            {
                if (elements == null || elements.Count() == 0)
                { ret = false; }
                else
                {
                    foreach (Element el in elements)
                    {

                        if (el is FamilySymbol familySymbol)
                        {
                            string familyName = familySymbol.Family.Name;
                            Family fami = familySymbol.Family;
                            bool addflg = true;
                            for (int i = 0; i < FamilyNameList.Count(); i++)
                            {
                                if (FamilyNameList[i].FamilyName == familyName)
                                {
                                    addflg = false;
                                }
                            }
                            if (addflg)
                            {
                                ProjectFamily fam = new ProjectFamily
                                {
                                    FamilyName = familyName,
                                    FamilyTypeName = familySymbol.Name
                                };
                                FamilyNameList.Add(fam);
                                ProFami.Add(fami);
                            }
                        }
                        else
                        {
                            ProjectFamily fam = new ProjectFamily
                            {
                                FamilyName = el.Name,
                                FamilyTypeName = el.Name
                            };
                            FamilyNameList.Add(fam);
                        }

                    }
                }
            }
            catch(Exception)
            { }
            
            return ret;
        }

        internal  bool LoadLevelfamily_fromProject()
        {
            bool ret = true;

            ElementCategoryFilter LVfilter = new ElementCategoryFilter(BuiltInCategory.OST_Levels);
            FilteredElementCollector LVcollector = new FilteredElementCollector(Commons.doc);
            IList<Element> LVlist = LVcollector.WherePasses(LVfilter).WhereElementIsNotElementType().ToElements();


            if (LVlist == null || LVlist.Count() == 0)
            {
                ret = false;
            }
            else
            {
                
                foreach (Element el in LVlist)
                {
                    bool addflg = false;
                    Level lv = el as Level;
                    LevelList lvst = new LevelList
                    {
                        id = lv.Id,
                        name = lv.Name,
                        elevation = Math.Round(Commons.ft2mm(lv.Elevation, 3), 3)
                    };

                    for (int i = 0; i < LevelNameList.Count(); i++)
                    {
                        if(LevelNameList[i].elevation > lvst.elevation)
                        {
                            LevelNameList.Insert(i, lvst);
                            addflg = true;
                            break;
                        }
                    }
                    if (addflg == false)
                    {
                        LevelNameList.Add(lvst);
                    }

                }
            }
                    return ret;
        }

        internal  bool Axisfamily_fromProject()
        {
            bool ret = true;

            ElementCategoryFilter Grifilter2 = new ElementCategoryFilter(BuiltInCategory.OST_GridChains);
            FilteredElementCollector Gricollector2 = new FilteredElementCollector(Commons.doc);
            IList<Element> Grlist2 = Gricollector2.WherePasses(Grifilter2).WhereElementIsNotElementType().ToElements();

            ElementCategoryFilter Grifilter = new ElementCategoryFilter(BuiltInCategory.OST_Grids);
            FilteredElementCollector Gricollector = new FilteredElementCollector(Commons.doc);
            IList<Element> Grlist = Gricollector.WherePasses(Grifilter).WhereElementIsNotElementType().ToElements();

           

            if(Grlist2 != null && Grlist2.Count() != 0)
            { 
                foreach(Element el in Grlist2)
                {
                    MultiSegmentGrid mgr = el as MultiSegmentGrid;

                }
            }


            if (Grlist == null || Grlist.Count() == 0)
            {
                ret = false;
            }
            else
            {               
                foreach (Element el in Grlist)
                {
                    Grid gr = el as Grid;

                    XYZ s = gr.Curve.GetEndPoint(0);
                    XYZ e = gr.Curve.GetEndPoint(1);

                    bool sameflg = false;

                    //同じ名前は追加しない（マルチセグメントグリッドを使っているとグリッド名が同じものがいくつかできる）
                    for (int i = 0; i < GridX.Count(); i++)
                    {
                        if (GridX[i].Name == gr.Name)
                        {
                            sameflg = true;
                            break;
                        }
                    }
                    if(!sameflg)
                    {
                        for (int i = 0; i < GridY.Count(); i++)
                        {
                            if (GridY[i].Name == gr.Name)
                            {
                                sameflg = true;
                                break;
                            }
                        }
                    }

                    double angle = Math.Atan2((s.Y - e.Y), (s.X - e.X)) * 180 / Math.PI;
                   
                    if ((-45 < angle && angle < 45) || (135 < angle && angle < 225))
                    {
                        if(!sameflg)
                        { GridY.Add(gr); }
                    }
                    else
                    {
                        if (!sameflg)
                        { GridX.Add(gr); }
                    }
                    
                }
            }
            GridX.Sort(SortXGird);
            GridY.Sort(SortYGird);
            return ret;
        }

        internal bool Materialfamily_fromProject()
        {
            bool ret = true;
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Materials);
            IList<Element> elements = collector.WherePasses(filter).ToElements();

            if(elements == null || elements.Count() == 0)
            {
                ret = false;
                return ret;
            }
            foreach (Element el in elements)
            {
                Material mate = el as Material;
                if(mate.MaterialCategory == "メタル" || mate.MaterialCategory == "金属" ||
                   mate.MaterialClass    == "メタル" || mate.MaterialClass    == "金属")
                {
                    bool sameflg = false;
                    for(int i = 0; i < RevitMatName.Count(); i++)
                    {
                        if (RevitMatName[i].Name == mate.Name)
                        {
                            sameflg = true;
                            break;
                        }
                    }
                    if (!sameflg)
                    {
                        RevitMatName.Add(mate);
                    }
                }
            }
            return ret;
        }

        internal bool Concretefamily_fromProject()
        {
            bool ret = true;
            FilteredElementCollector collector = new FilteredElementCollector(Commons.doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Materials);
            IList<Element> elements = collector.WherePasses(filter).ToElements();

            if (elements == null || elements.Count() == 0)
            {
                ret = false;
                return ret;
            }
            foreach (Element el in elements)
            {
                Material mate = el as Material;
                if (mate.MaterialCategory == "コンクリート" ||
                    mate.MaterialClass    == "コンクリート")
                {
                    bool sameflg = false;
                    for (int i = 0; i < RevitConcName.Count(); i++)
                    {
                        if (RevitConcName[i].Name == mate.Name)
                        {
                            sameflg = true;
                            break;
                        }
                    }
                    if (!sameflg)
                    {
                        RevitConcName.Add(mate);
                    }
                }
            }
            return ret;
        }

        internal bool ViewPlanfamily_fromProject()
        {
            bool ret = true;

            ElementCategoryFilter VPlanfilter = new ElementCategoryFilter(BuiltInCategory.OST_Views);
            FilteredElementCollector LVcollector = new FilteredElementCollector(Commons.doc);
            IList<Element> VP = LVcollector.WherePasses(VPlanfilter).WhereElementIsNotElementType().ToElements();


            if (VP == null || VP.Count() == 0)
            {
                ret = false;
            }
            else
            {
                foreach (Element el in VP)
                {
                    if (el is ViewPlan vp)
                    {
                        VPlan.Add(vp);
                    }
                }
            }
            return ret;
        }


        internal static  int SortXGird(Grid a, Grid b) 
        {
            int ret = 0;
            XYZ p = a.Curve.GetEndPoint(0);
            XYZ q = b.Curve.GetEndPoint(0);
            if (p.X - q.X > 0)
            { ret = 1; }
            else if(p.X - q.X < 0)
            { ret = -1; }
            return ret;
        }
        internal static int SortYGird(Grid a, Grid b)
        {
            int ret = 0;
            XYZ p = a.Curve.GetEndPoint(0);
            XYZ q = b.Curve.GetEndPoint(0);
            if (p.Y - q.Y > 0)
            { ret = 1; }
            else if (p.Y - q.Y < 0)
            { ret = -1; }
            return ret;
        }
    }
}
