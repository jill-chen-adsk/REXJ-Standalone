using System;
using System.Linq;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using SectionListRC.Utils;

namespace SectionListRC.Components
{
    /// ================================================================================
    /// <summary>要素</summary>
    /// ================================================================================
    public class Elements : SectionListRC.JExtComCompat.RvtElements
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>属性</summary>
        private Attribute _CmpAttribute;

        /// <summary>枠線スタイル</summary>
        private GraphicsStyle _FrameLineStyle;

        /// <summary>躯体線スタイル</summary>
        private GraphicsStyle _BodyLineStyle;

        /// <summary>幅止筋線スタイル</summary>
        private GraphicsStyle _SpacerLineStyle;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="rvtUIDoc"    >Revit UIドキュメント</param>
        ///
        /// <history>2013/02/04 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public Elements(Attribute cmpAttribute,
                        Revit.UI.UIDocument rvtUIDoc) : base(rvtUIDoc)
        {
            _CmpAttribute = cmpAttribute;
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>プロジェクト内のレベル</summary>
        ///
        /// <history>2014/06/10 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        List<string> ProjLevelNames()
        {
            // 戻り値
            List<string> ret = new List<string>();

            FilteredElementCollector fec = new FilteredElementCollector(RvtDBDoc);
            fec.OfClass(typeof(Level));

            List<double> height = new List<double>();
            foreach (Level l in fec)
            {
                height.Add(l.Elevation);
            }

            // 高い順ソート
            height.Sort();
            if (height[0] < height[height.Count - 1])
            {
                height.Reverse();
            }

            foreach (double d in height)
            {
                foreach (Level l in fec)
                {
                    if (ToHalfAdjust(d, -9) == ToHalfAdjust(l.Elevation, -9))
                    {
                        ret.Add(l.Name);
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>同名、同レベルファミリの単一化</summary>
        ///
        /// <history>2013/03/01 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void IsHaveSame(ref IList<FamilySymbol> famInsAry, string paramHugoName)
        {
            IList<FamilySymbol> ret = new List<FamilySymbol>();

            IList<string> name_level = new List<string>();

            foreach (FamilySymbol famSym in famInsAry)
            {
                string name = "";
                string level = "0";

                GetTypeMarkLevel(famSym, ref name, ref level, paramHugoName);

                string str = name + "_" + level;

                if (!name_level.Contains(str))
                {
                    name_level.Add(str);
                    ret.Add(famSym);
                }
            }

            famInsAry = ret;
        }

        /// ================================================================================
        /// <summary>異なる柱ファミリに同じ符号名</summary>
        ///
        /// <history>2013/07/04 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<string> SameColumnHugoInDifferentFamily(IList<FamilySymbol> famSymAry,
                                                                          string kakuHugo,
                                                                          string enHugo,
                                                                          string dx,
                                                                          string dy,
                                                                          string tyokkei)
        {
            IList<string> ret = new List<string>();

            foreach (FamilySymbol famSym in famSymAry)
            {
                Family fam = famSym.Family;

                Parameter x = famSym.LookupParameter(dx);
                Parameter y = famSym.LookupParameter(dy);
                Parameter d = null;

                string hugoName = "";
                string famLevel = "";

                if (x != null && y != null)
                {
                    GetTypeMarkLevel(famSym, ref hugoName, ref famLevel, kakuHugo);
                }
                else
                {
                    d = famSym.LookupParameter(tyokkei);

                    if (d != null)
                    {
                        GetTypeMarkLevel(famSym, ref hugoName, ref famLevel, enHugo);
                    }
                    else
                    {
                        continue;
                    }
                }

                foreach (FamilySymbol fS in famSymAry)
                {
                    Family f = fS.Family;

                    x = fS.LookupParameter(dx);
                    y = fS.LookupParameter(dy);
                    d = null;

                    string hn = "";
                    string fl = "";

                    if (x != null && y != null)
                    {
                        GetTypeMarkLevel(fS, ref hn, ref fl, kakuHugo);
                    }
                    else
                    {
                        d = fS.LookupParameter(tyokkei);

                        if (d != null)
                        {
                            GetTypeMarkLevel(fS, ref hn, ref fl, enHugo);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    // 符号名比較
                    if (hugoName == hn)
                    {
                        if (fam.Id.Value != f.Id.Value)
                        {
                            if (!ret.Contains(hugoName))
                            {
                                ret.Add(hugoName);
                                break;
                            }
                        }
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>異なる梁ファミリに同じ符号名</summary>
        ///
        /// <history>2013/07/04 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<string> SameGirderHugoInDifferentFamily(IList<FamilySymbol> famSymAry,
                                                                          string hugo,
                                                                          string hugo_katamoti)
        {
            IList<string> ret = new List<string>();

            foreach (FamilySymbol famSym in famSymAry)
            {
                if (famSym == null)
                {
                    continue;
                }

                Family fam = famSym.Family;
                if (fam == null)
                {
                    continue;
                }

                Parameter param = famSym.LookupParameter("梁種別");

                if (param == null)
                {
                    continue;
                }

                string paramHugoName = "";

                string strParam = param.AsString();
                if (strParam == "Girder" || strParam == "Beam" || strParam == "Foundation-Girder" || strParam == "Foundation-Beam")
                {
                    paramHugoName = hugo;
                }
                else if (strParam == "Cantilever-Girder" || strParam == "Cantilever-Beam" || strParam == "Cantilever-Foundation-Girder" || strParam == "Cantilever-Foundation-Beam")
                {
                    paramHugoName = hugo_katamoti;
                }

                string hugoName = "";
                string famLevel = "";

                GetTypeMarkLevel(famSym, ref hugoName, ref famLevel, paramHugoName);

                foreach (FamilySymbol fS in famSymAry)
                {
                    if (fS == null)
                    {
                        continue;
                    }

                    Family f = fS.Family;
                    if (f == null)
                    {
                        continue;
                    }

                    Parameter par = famSym.LookupParameter("梁種別");

                    if (par == null)
                    {
                        continue;
                    }

                    paramHugoName = "";

                    strParam = par.AsString();
                    if (strParam == "Girder" || strParam == "Beam" || strParam == "Foundation-Girder" || strParam == "Foundation-Beam")
                    {
                        paramHugoName = hugo;
                    }
                    else if (strParam == "Cantilever-Girder" || strParam == "Cantilever-Beam" || strParam == "Cantilever-Foundation-Girder" || strParam == "Cantilever-Foundation-Beam")
                    {
                        paramHugoName = hugo_katamoti;
                    }

                    string hn = "";
                    string fl = "";

                    GetTypeMarkLevel(fS, ref hn, ref fl, paramHugoName);

                    // 符号名比較
                    if (hugoName == hn)
                    {
                        // ファミリ比較
                        if (fam.Id.Value != f.Id.Value)
                        {
                            if (!ret.Contains(hugoName))
                            {
                                ret.Add(hugoName);
                                break;
                            }
                        }
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>違うファミリのタイプ名重複確認</summary>
        ///
        /// <history>2016/10/13 Created CST,Co.Ltd Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<string> OverlapTypeName(IList<FamilySymbol> famSymAry)
        {
            // 戻り値
            IList<string> ret = new List<string>();

            IDictionary<long, IList<string>> dicIdName = new Dictionary<long, IList<string>>();

            // 梁
            foreach (FamilySymbol famSym in famSymAry)
            {
                string name = famSym.Name;

                Family fam = famSym.Family;

                foreach (var id in dicIdName.Keys)
                {
                    // 違うファミリ
                    if (fam.Id.Value != id)
                    {
                        IList<string> value = dicIdName[id];

                        // 同じタイプ名
                        if (value.Contains(name))
                        {
                            if (ret.Contains(name) == false)
                            {
                                ret.Add(name);
                            }
                        }
                    }
                }

                if (dicIdName.ContainsKey(fam.Id.Value))
                {
                    if (dicIdName[fam.Id.Value].Contains(name) == false)
                    {
                        dicIdName[fam.Id.Value].Add(name);
                    }
                }
                else
                {
                    IList<string> value = new List<string>();
                    value.Add(name);

                    dicIdName.Add(fam.Id.Value, value);
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>タイプ符号階取得</summary>
        ///
        /// <history>2013/04/15 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetTypeMarkLevel(FamilySymbol famSym,
                              ref string famName,
                              ref string famLevel,
                              string paramHugoName)
        {
            if (famSym != null)
            {
                Parameter paramMark = famSym.LookupParameter(paramHugoName);

                if (paramMark != null)
                {
                    famName = paramMark.AsString();

                    if (famName != null && famName != "")
                    {
                        if (famSym.Name.Contains(famName))
                        {
                            famLevel = famSym.Name.Substring(0, famSym.Name.LastIndexOf(famName));
                        }
                    }
                    else if (famName == null)
                    {
                        famName = "";
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>柱タイプ階取得</summary>
        ///
        /// <history><p>2013/04/23 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2015/06/26 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string GetColumnTypeLevel(FamilySymbol column,
                                  string paramHugo)
        {
            string ret = "0";

            if (column != null)
            {
                FamilySymbol symbol = column;
                if (symbol != null)
                {
                    try
                    {
                        Parameter paramColumnMark = symbol.LookupParameter(paramHugo);
                        string markName = paramColumnMark.AsString();

                        if (markName != "")
                        {
                            ret = column.Name.Substring(0, column.Name.LastIndexOf(markName));
                        }
                        else
                        {
                            ret = column.Name;
                        }
                    }
                    catch
                    {
                        ret = column.Name;
                        return ret;
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>梁タイプ階取得</summary>
        ///
        /// <history><p>2013/04/25 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2015/06/26 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string GetBeamTypeLevel(FamilySymbol beam,
                                string paramHugo)
        {
            string ret = "";

            if (beam != null)
            {
                FamilySymbol famSym = beam;

                if (famSym != null)
                {
                    try
                    {
                        Parameter param = famSym.LookupParameter(paramHugo);
                        string name = param.AsString();

                        if (name != "")
                        {
                            ret = beam.Name.Substring(0, beam.Name.LastIndexOf(name));
                        }
                        else
                        {
                            ret = beam.Name;
                        }
                    }
                    catch
                    {
                        ret = beam.Name;
                        return ret;
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>作図行(レベル)数</summary>
        /// <history>2013/03/05 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        int DrawRowsCount(IList<FamilyInstance> famInsAry)
        {
            IList<string> strs = new List<string>();

            foreach (FamilyInstance famIns in famInsAry)
            {
                Level lvl = null;

                if (famIns.StructuralType == Revit.DB.Structure.StructuralType.Beam)
                {
                    lvl = RvtDBDoc.GetElement(famIns.Host.Id) as Level;
                }
                else if (famIns.StructuralType == Revit.DB.Structure.StructuralType.Column)
                {
                    lvl = RvtDBDoc.GetElement(famIns.LevelId) as Level;
                }

                if (!strs.Contains(lvl.Name))
                {
                    strs.Add(lvl.Name);
                }
            }

            return strs.Count;
        }

        /// ================================================================================
        /// <summary>作図列(タイプ)数</summary>
        /// <history>2013/03/05 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        int DrawColumnCount(IList<FamilyInstance> famInsAry)
        {
            IList<string> strs = new List<string>();

            foreach (FamilyInstance famIns in famInsAry)
            {
                if (!strs.Contains(famIns.Name))
                {
                    strs.Add(famIns.Name);
                }
            }

            return strs.Count;
        }

        /// ================================================================================
        /// <summary>有限線分作成</summary>
        /// <history>2013/03/25 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        Line CreateBoundLine(XYZ p1, XYZ p2)
        {
            Line l = null;

            try
            {
                l = Line.CreateBound(p1, p2);
            }
            catch
            {
            }

            return l;
        }

        /// ================================================================================
        /// <summary>非Nullカーブをセット</summary>
        /// <history>2013/03/25 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void NotNullCurveSet(ref CurveArray crvAry, Curve crv)
        {
            if (crv != null)
            {
                crvAry.Append(crv);
            }
        }

        /// ================================================================================
        /// <summary>線分追加</summary>
        /// <history>2013/03/26 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void AddCrvByAry(ref CurveArray crvAry, IList<Curve> crvs)
        {
            foreach (Curve c in crvs)
            {
                crvAry.Append(c);
            }
        }

        /// ================================================================================
        /// <summary>すべての平面ビュー名</summary>
        /// <history>2013/05/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<string> AllViewPlanName()
        {
            IList<string> ret = new List<string>();

            FilteredElementCollector colle = new FilteredElementCollector(RvtDBDoc);
            colle.OfClass(typeof(ViewPlan));

            foreach (ViewPlan vp in colle)
            {
                ret.Add(vp.Name);
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>構造平面ビュータイプ取得</summary>
        /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IEnumerable<ViewFamilyType> StructuralPlanFamilyTypes()
        {
            IEnumerable<ViewFamilyType> ret = null;

            try
            {
                ret = from elem in new FilteredElementCollector(RvtDBDoc)
                           .OfClass(typeof(ViewFamilyType))
                      let type = elem as ViewFamilyType
                      where type.ViewFamily == ViewFamily.StructuralPlan
                      select type;
            }
            catch
            {
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>構造平面取得判定</summary>
        /// <history>2013/06/13 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool IsStrPlaneGet()
        {
            bool ret = false;
            IEnumerable<ViewFamilyType> strPlanFamType = StructuralPlanFamilyTypes();

            if (strPlanFamType != null)
            {
                ret = true;
            }
            else if (strPlanFamType == null)
            {
                ret = false;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>平面ビュータイプ取得</summary>
        /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IEnumerable<ViewFamilyType> ViewPlanFamilyTypes()
        {
            return from elem in new FilteredElementCollector(RvtDBDoc)
                     .OfClass(typeof(ViewFamilyType))
                   let type = elem as ViewFamilyType
                   where type.ViewFamily == ViewFamily.FloorPlan
                   select type;
        }

        /// ================================================================================
        /// <summary>0高さレベル</summary>
        /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        Level ZeroLevel()
        {
            Level ret = null;

            FilteredElementCollector filterElemColle = new FilteredElementCollector(RvtDBDoc);
            filterElemColle.OfCategory(BuiltInCategory.OST_Levels);

            foreach (Element elem in filterElemColle)
            {
                Level l = elem as Level;
                if (l == null)
                {
                    continue;
                }

                if (ret == null)
                {
                    ret = l;
                }
                else
                {
                    if (Math.Abs(ret.Elevation) > Math.Abs(l.Elevation))
                    {
                        ret = l;
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>作図ビュー作成</summary>
        ///
        /// <param name="viewScale">ビュー尺度</param>
        /// <param name="mode">mode = 0 柱
        ///                         = 1 間柱
        ///                         = 2 大梁
        ///                         = 3 小梁
        ///                         = 4 基礎大梁
        ///                         = 5 基礎小梁</param>
        ///
        /// <history>2013/04/19 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void SetCreateListView(int viewScale, int mode)
        {
            ViewPlan vp = null;

            ElementId eid = ZeroLevel().Id;

            // ビューの作成
            Transaction trans = new Transaction(RvtDBDoc);
            trans.Start("ビューの作成");

            if (StructuralPlanFamilyTypes() != null && StructuralPlanFamilyTypes().Count() > 0)
            {
                foreach (ViewFamilyType vft in StructuralPlanFamilyTypes())
                {
                    if (vft.Name == _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST"))
                    {
                        vp = ViewPlan.Create(RvtDBDoc, vft.Id, eid);
                    }
                }

                if (vp == null)
                {
                    ICollection<ElementId> copyElem = ElementTransformUtils.CopyElement(RvtDBDoc, StructuralPlanFamilyTypes().First().Id, new XYZ());

                    ViewFamilyType viewFamType = RvtDBDoc.GetElement(copyElem.First()) as ViewFamilyType;
                    viewFamType.Name = _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST");
                    vp = ViewPlan.Create(RvtDBDoc, viewFamType.Id, eid);
                }

                if (vp == null)
                {
                    foreach (ViewFamilyType vft in StructuralPlanFamilyTypes())
                    {
                        if (vft.Name == _CmpAttribute.ResourceText("IDS_TXT_LIST"))
                        {
                            vp = ViewPlan.Create(RvtDBDoc, vft.Id, eid);
                        }
                    }

                    if (vp == null)
                    {
                        vp = ViewPlan.Create(RvtDBDoc, StructuralPlanFamilyTypes().First().Id, eid);
                    }
                }
            }
            else if (ViewPlanFamilyTypes().Count() > 0)
            {
                vp = ViewPlan.Create(RvtDBDoc, ViewPlanFamilyTypes().First().Id, eid);
            }

            if (vp == null)
            {
                trans.Commit();
                return;
            }

            if (viewScale > 0)
            {
                vp.Scale = viewScale;
            }

            trans.Commit();

            // 作成したビューの要素
            FilteredElementCollector filterElemColle = new FilteredElementCollector(RvtDBDoc, vp.Id);
            ICollection<ElementId> hideElemIds = new List<ElementId>();
            foreach (Element elem in filterElemColle)
            {
                if (elem.CanBeHidden(vp))
                {
                    hideElemIds.Add(elem.Id);
                }
            }

            if (hideElemIds.Count > 0)
            {
                trans.Start("既存要素の非表示");
                vp.HideElements(hideElemIds);
                trans.Commit();
            }

            UiDocument.ActiveView = vp;

            if (trans.GetStatus() == TransactionStatus.Started)
            {
                trans.Commit();
            }
        }

        /// ================================================================================
        /// <summary>Set name for view plans</summary>
        /// <param name="dic_views">Dictionary contains the view plan</param>
        /// ================================================================================
        public void SetViewPlanName(Dictionary<int, List<ViewPlan>> dic_views, bool byRange)
        {
            string prefixName = string.Empty;

            foreach (KeyValuePair<int, List<ViewPlan>> keyPair in dic_views)
            {
                var aryNum = keyPair.Key;

                bool isCreateMultipleView = keyPair.Value.ToList().Count > 1 ? true : false;

                // 名前
                if (aryNum == (int)EnumType.Column)
                {
                    prefixName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_COLUMN");
                }
                else if (aryNum == (int)EnumType.Post)
                {
                    prefixName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_POST");
                }
                else if (aryNum == (int)EnumType.Girder)
                {
                    prefixName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_GIRDER");
                }
                else if (aryNum == (int)EnumType.CantiGirder)
                {
                    prefixName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_CANTIGIRDER");
                }
                else if (aryNum == (int)EnumType.Beam)
                {
                    prefixName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_BEAM");
                }
                else if (aryNum == (int)EnumType.CantiBeam)
                {
                    prefixName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_CANTIBEAM");
                }
                else if (aryNum == (int)EnumType.FoundationGirder)
                {
                    prefixName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_GIRDER_FOND");
                }
                else if (aryNum == (int)EnumType.CantiFoundationGirder)
                {
                    prefixName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_CANTIGIRDER_FOND");
                }
                else if (aryNum == (int)EnumType.FoundationBeam)
                {
                    prefixName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_BEAM_FOND");
                }
                else if (aryNum == (int)EnumType.CantiFoundationBeam)
                {
                    prefixName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_CANTIBEAM_FOND");
                }

                if (prefixName == string.Empty)
                    return;
                               
                var allViewNames = AllViewPlanName();

                bool isFirstbyRange = true;
                foreach (string name in allViewNames)
                {
                    var name1 = string.Format("{0}_1", prefixName);

                    if (name.Contains(prefixName))
                    {
                        isFirstbyRange = false;
                        break;
                    }
                    else if(name.Contains(name1))
                    {
                        isFirstbyRange = false;
                        break;
                    }
                }


                if (byRange && isCreateMultipleView == false)
                {
                    int number1 = 0;
                    var name1 = string.Format("{0}_1", prefixName);
                    var name2 = string.Format("{0}({1})", prefixName, ++number1);
                    var name3 = string.Format("{0}({1})_1", prefixName, number1);

                    while (true)
                    {
                        if (isFirstbyRange)
                        {
                            keyPair.Value[0].Name = name1;

                            break;

                        }
                        else if (AllViewPlanName().Contains(name2) == false)
                        {
                            if (AllViewPlanName().Contains(name3) == false)
                            {
                                keyPair.Value[0].Name = name3;

                                break;
                            }
                            name2 = string.Format("{0}({1})", prefixName, ++number1);
                            name3 = string.Format("{0}({1})_1", prefixName, number1);
                        }                        
                        else
                        {
                            name2 = string.Format("{0}({1})", prefixName, ++number1);
                            name3 = string.Format("{0}({1})_1", prefixName, number1);
                        }
                    }
                    continue;
                }

                //var allViewNames = AllViewPlanName();

                bool isFirst = true;
                foreach (string name in allViewNames)
                {
                    if (name.Contains(prefixName))
                    {
                        isFirst = false;
                        break;
                    }
                }

                if (isCreateMultipleView == false)
                {
                    if (allViewNames.Contains(prefixName) == false && isFirst)
                    {
                        keyPair.Value[0].Name = prefixName;
                        continue;
                    }
                }

                string numberStr = string.Empty;
                int number = -1;
                while (true)
                {
                    number++;
                    string name = prefixName;

                    if (isCreateMultipleView == true && isFirst)
                    {
                        name = prefixName + "_";
                    }
                    else
                    {
                        if (number == 0)
                            number = 1;
                        name = string.Format("{0}({1})", prefixName, number.ToString());
                    }
                    numberStr = name;

                    bool exist = false;
                    foreach (string exitName in allViewNames)
                    {
                        if (exitName.Contains(numberStr) == true)
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (exist == false)
                        break;
                }

                foreach (View vp in keyPair.Value)
                {
                    if (isCreateMultipleView == false)
                    {
                        if (AllViewPlanName().Contains(numberStr) == false)
                        {
                            vp.Name = numberStr;
                        }
                    }

                    if (vp.Name != numberStr)
                    {
                        int nameNum = 0;

                        while (true)
                        {
                            nameNum += 1;

                            var vpName = numberStr + (number != 0 ? "_" : "") + nameNum;

                            if (AllViewPlanName().Contains(vpName) == false)
                            {
                                vp.Name = vpName;
                                break;
                            }
                        }
                    }

                    ////////////////////////////////////////////////////////////////////////

                    //if (vp.Name != prefixName)
                    //{
                    //    string vpName = prefixName;
                    //    int nameNum = 0;

                    //    bool isVPName = false;

                    //    while (isVPName == false)
                    //    {
                    //        nameNum += 1;

                    //        vpName = prefixName + "(" + nameNum + ")";

                    //        if (AllViewPlanName().Contains(vpName) == false)
                    //        {
                    //            vp.Name = vpName;
                    //            isVPName = true;
                    //        }
                    //    }
                    //}
                }
            }
        }

        /// ================================================================================
        /// <summary>作図ビュー作成</summary>
        ///
        /// <param name="viewScale">ビュー尺度</param>
        ///
        /// <history>2014/06/17 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        ViewPlan SetCreateListView(int viewScale)
        {
            // 戻り値
            View ret = RvtDBDoc.ActiveView;

            ViewPlan vp = null;

            ElementId eid = ZeroLevel().Id;

            // ビューの作成
            Transaction trans = new Transaction(RvtDBDoc);
            trans.Start("ビューの作成");

            if (StructuralPlanFamilyTypes() != null && StructuralPlanFamilyTypes().Count() > 0)
            {
                foreach (ViewFamilyType vft in StructuralPlanFamilyTypes())
                {
                    if (vft.Name == _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST"))
                    {
                        vp = ViewPlan.Create(RvtDBDoc, vft.Id, eid);
                    }
                }

                if (vp == null)
                {
                    ICollection<ElementId> copyElem = ElementTransformUtils.CopyElement(RvtDBDoc, StructuralPlanFamilyTypes().First().Id, new XYZ());

                    ViewFamilyType viewFamType = RvtDBDoc.GetElement(copyElem.First()) as ViewFamilyType;
                    viewFamType.Name = _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST");
                    vp = ViewPlan.Create(RvtDBDoc, viewFamType.Id, eid);
                }

                if (vp == null)
                {
                    foreach (ViewFamilyType vft in StructuralPlanFamilyTypes())
                    {
                        if (vft.Name == "リスト")
                        {
                            vp = ViewPlan.Create(RvtDBDoc, vft.Id, eid);
                        }
                    }

                    if (vp == null)
                    {
                        vp = ViewPlan.Create(RvtDBDoc, StructuralPlanFamilyTypes().First().Id, eid);
                    }
                }
            }
            else if (ViewPlanFamilyTypes().Count() > 0)
            {
                vp = ViewPlan.Create(RvtDBDoc, ViewPlanFamilyTypes().First().Id, eid);
            }

            if (vp == null)
            {
                trans.Commit();
                return vp;
            }

            if (viewScale > 0)
            {
                vp.Scale = viewScale;
            }

            trans.Commit();

            // 作成したビューの要素
            FilteredElementCollector filterElemColle = new FilteredElementCollector(RvtDBDoc, vp.Id);
            ICollection<ElementId> hideElemIds = new List<ElementId>();
            foreach (Element elem in filterElemColle)
            {
                if (elem.CanBeHidden(vp))
                {
                    hideElemIds.Add(elem.Id);
                }
            }

            if (hideElemIds.Count > 0)
            {
                trans.Start("既存要素の非表示");
                vp.HideElements(hideElemIds);
                trans.Commit();
            }

            UiDocument.ActiveView = vp;

            if (trans.GetStatus() == TransactionStatus.Started)
            {
                trans.Commit();
            }

            return vp;
        }

        /// ================================================================================
        /// <summary>作図ビュー作成</summary>
        ///
        /// <param name="viewScale" >ビュー尺度</param>
        /// <param name="current"   >元のビュー</param>
        ///
        /// <history><p>2014/06/17 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2018/05/28 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string SetCreateListView(int viewScale, ref View current)
        {
            // 戻り値
            string ret = "";

            current = RvtDBDoc.ActiveView;

            ViewPlan vp = null;

            ElementId eid = ZeroLevel().Id;

            // ビューの作成
            Transaction trans = new Transaction(RvtDBDoc);

            try
            {
                trans.Start("ビューの作成");

                if (StructuralPlanFamilyTypes() != null && StructuralPlanFamilyTypes().Count() > 0)
                {
                    foreach (ViewFamilyType vft in StructuralPlanFamilyTypes())
                    {
                        if (vft.Name == _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST"))
                        {
                            vp = ViewPlan.Create(RvtDBDoc, vft.Id, eid);
                        }
                    }

                    if (vp == null)
                    {
                        ICollection<ElementId> copyElem = ElementTransformUtils.CopyElement(RvtDBDoc, StructuralPlanFamilyTypes().First().Id, new XYZ());

                        ViewFamilyType viewFamType = RvtDBDoc.GetElement(copyElem.First()) as ViewFamilyType;
                        viewFamType.Name = _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST");
                        vp = ViewPlan.Create(RvtDBDoc, viewFamType.Id, eid);
                    }

                    if (vp == null)
                    {
                        foreach (ViewFamilyType vft in StructuralPlanFamilyTypes())
                        {
                            if (vft.Name == "リスト")
                            {
                                vp = ViewPlan.Create(RvtDBDoc, vft.Id, eid);
                            }
                        }

                        if (vp == null)
                        {
                            vp = ViewPlan.Create(RvtDBDoc, StructuralPlanFamilyTypes().First().Id, eid);
                        }
                    }
                }
                else if (ViewPlanFamilyTypes().Count() > 0)
                {
                    vp = ViewPlan.Create(RvtDBDoc, ViewPlanFamilyTypes().First().Id, eid);
                }

                if (vp == null)
                {
                    trans.Commit();

                    ret = "作図用ビューの作成に失敗しました";
                    return ret;
                }

                if (viewScale > 0)
                {
                    vp.Scale = viewScale;
                }

                trans.Commit();

                // 作成したビューの要素
                FilteredElementCollector filterElemColle = new FilteredElementCollector(RvtDBDoc, vp.Id);
                ICollection<ElementId> hideElemIds = new List<ElementId>();
                foreach (Element elem in filterElemColle)
                {
                    if (elem.CanBeHidden(vp))
                    {
                        hideElemIds.Add(elem.Id);
                    }
                }

                if (hideElemIds.Count > 0)
                {
                    trans.Start("既存要素の非表示");
                    vp.HideElements(hideElemIds);
                    trans.Commit();
                }

                UiDocument.ActiveView = vp;
            }
            catch
            {
                ret = "作図用ビューの設定に失敗しました";
            }

            if (trans.GetStatus() != TransactionStatus.Committed)
            {
                trans.RollBack();
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>作図ビュー削除</summary>
        ///
        /// <param name="view">作図ビュー</param>
        ///
        /// <history>2018/05/28 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string DeleteListVIew(View view)
        {
            string ret = "";

            try
            {
                RvtDBDoc.Delete(view.Id);
            }
            catch
            {
                ret = "作図用ビューの削除に失敗しました";
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>名前指定文字タイプ取得</summary>
        ///
        /// <param name="typeName">タイプ名</param>
        ///
        /// <history><p>2013/04/11 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2014/09/10 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        TextNoteType TxtNoteTypeByName(string typeName)
        {
            TextNoteType ret = null;

            IList<TextNoteType> txtNoteTypes = TxtNoteTypes;

            foreach (TextNoteType tnt in txtNoteTypes)
            {
                if (tnt.Name == typeName)
                {
                    ret = tnt;
                    break;
                }
            }

            if (ret == null)
            {
                List<string> names = new List<string>();

                foreach (TextNoteType tnt in txtNoteTypes)
                {
                    names.Add(tnt.Name);
                }

                // 名前ソート
                names.Sort();

                foreach (TextNoteType tnt in txtNoteTypes)
                {
                    if (names[0] == tnt.Name)
                    {
                        return tnt;
                    }
                }

                if (ret == null)
                {
                    ret = txtNoteTypes[0];
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>名前指定寸法線タイプ取得</summary>
        ///
        /// <param name="typeName">タイプ名</param>
        ///
        /// <history><p>2013/04/11 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2014/09/10 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        DimensionType DimTypeByName(string typeName)
        {
            DimensionType ret = null;

            IList<DimensionType> dimTypes = DimTypes;

            foreach (DimensionType dt in dimTypes)
            {
                if (dt.Name == typeName)
                {
                    ret = dt;
                }
            }

            if (ret == null && dimTypes.Count > 0)
            {
                List<string> names = new List<string>();

                foreach (DimensionType dt in dimTypes)
                {
                    names.Add(dt.Name);
                }

                // 名前ソート
                names.Sort();

                foreach (DimensionType dt in dimTypes)
                {
                    if (names[0] == dt.Name)
                    {
                        return dt;
                    }
                }

                if (ret == null)
                {
                    ret = dimTypes[0];
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>名前指定詳細線分線種タイプ取得</summary>
        ///
        /// <param name="styleName">スタイル名</param>
        ///
        /// <history><p>2013/04/11 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2014/09/10 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        GraphicsStyle GraStyleByName(string styleName)
        {
            GraphicsStyle ret = null;

            IList<GraphicsStyle> graStyles = DetailGraStyles;

            foreach (GraphicsStyle gs in graStyles)
            {
                if (gs.Name == styleName)
                {
                    ret = gs;
                }
            }

            if (ret == null && graStyles.Count > 0)
            {
                List<string> names = new List<string>();

                foreach (GraphicsStyle gs in graStyles)
                {
                    names.Add(gs.Name);
                }

                // 名前ソート
                names.Sort();

                foreach (GraphicsStyle gs in graStyles)
                {
                    if (names[0] == gs.Name)
                    {
                        return gs;
                    }
                }

                if (ret == null)
                {
                    ret = graStyles[0];
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>枠線スタイル取得</summary>
        ///
        /// <param name="styleName">スタイル名</param>
        ///
        /// <history><p>2013/06/02 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2014/09/10 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        GraphicsStyle FrameLineGraStyleByName(string styleName)
        {
            GraphicsStyle ret = GraStyleByName(styleName);

            //foreach (Revit.DB.GraphicsStyle gs in DetailGraStyles)
            //{
            //  if (gs.Name == styleName)
            //  {
            //    ret = gs;
            //  }
            //}

            //if (ret == null)
            //{
            //  if (DetailGraStyles.Count > 0)
            //  {
            //    ret = DetailGraStyles[0];
            //  }
            //}

            _FrameLineStyle = ret;

            return ret;
        }

        /// ================================================================================
        /// <summary>躯体線スタイル取得</summary>
        ///
        /// <param name="styleName">スタイル名</param>
        ///
        /// <history><p>2013/06/02 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2014/09/10 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        GraphicsStyle BodyLineGraStyleByName(string styleName)
        {
            GraphicsStyle ret = GraStyleByName(styleName);

            //foreach (Revit.DB.GraphicsStyle gs in DetailGraStyles)
            //{
            //  if (gs.Name == styleName)
            //  {
            //    ret = gs;
            //  }
            //}

            //if (ret == null)
            //{
            //  if (DetailGraStyles.Count > 0)
            //  {
            //    ret = DetailGraStyles[0];
            //  }
            //}

            _BodyLineStyle = ret;

            return ret;
        }

        /// ================================================================================
        /// <summary>幅止筋線スタイル取得</summary>
        ///
        /// <param name="styleName">スタイル名</param>
        ///
        /// <history><p>2013/06/02 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2014/09/10 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        GraphicsStyle SpacerLineGraStyleByName(string styleName)
        {
            GraphicsStyle ret = GraStyleByName(styleName);

            //foreach (Revit.DB.GraphicsStyle gs in DetailGraStyles)
            //{
            //  if (gs.Name == styleName)
            //  {
            //    ret = gs;
            //  }
            //}

            //if (ret == null)
            //{
            //  if (DetailGraStyles.Count > 0)
            //  {
            //    ret = DetailGraStyles[0];
            //  }
            //}

            _SpacerLineStyle = ret;

            return ret;
        }

        /// ================================================================================
        /// <summary>角柱と円柱の分割</summary>
        /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void ColumnDivision(IList<FamilySymbol> allColumnAry,
                            string dx,
                            string dy,
                            string tyokkei,
                            ref IList<FamilySymbol> kakuAry,
                            ref IList<FamilySymbol> enAry)
        {
            foreach (FamilySymbol column in allColumnAry)
            {
                Parameter parX = column.LookupParameter(dx);
                Parameter parY = column.LookupParameter(dy);
                Parameter parDiameter = column.LookupParameter(tyokkei);

                if (parX != null && parY != null)
                {
                    if (parX.AsDouble() > 0 && parY.AsDouble() > 0)
                    {
                        kakuAry.Add(column);

                        continue;
                    }
                    if (parX.AsInteger() > 0 && parY.AsInteger() > 0)
                    {
                        kakuAry.Add(column);

                        continue;
                    }
                }

                if (parDiameter != null)
                {
                    if (parDiameter.AsDouble() > 0)
                    {
                        enAry.Add(column);

                        continue;
                    }
                    if (parDiameter.AsInteger() > 0)
                    {
                        enAry.Add(column);
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>柱と間柱の分割</summary>
        /// <history>2013/04/19 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void ColumnDivision(IList<FamilySymbol> allColumnAry,
                            string column_Category_Kaku,
                            string column_Category_En,
                            string dx,
                            string dy,
                            string tyokkei,
                            ref IList<FamilySymbol> columnAry,
                            ref IList<FamilySymbol> postAry)
        {
            foreach (FamilySymbol column in allColumnAry)
            {
                string ccKaku = "";
                string ccEn = "";

                try
                {
                    ccKaku = column.LookupParameter(column_Category_Kaku).AsString();
                }
                catch
                {
                    ccKaku = "";
                }
                try
                {
                    ccEn = column.LookupParameter(column_Category_En).AsString();
                }
                catch
                {
                    ccEn = "";
                }

                Parameter parX = column.LookupParameter(dx);
                Parameter parY = column.LookupParameter(dy);
                Parameter parDiameter = column.LookupParameter(tyokkei);

                if (parX != null && parY != null)
                {
                    //if (parX.AsDouble() > 0 && parY.AsDouble() > 0)
                    {
                        if (ccKaku == "Column")
                        {
                            columnAry.Add(column);
                        }
                        else if (ccKaku == "Post")
                        {
                            postAry.Add(column);
                        }
                    }
                }
                else if (parDiameter != null)
                {
                    //if (parDiameter.AsDouble() > 0)
                    {
                        if (ccEn == "Column")
                        {
                            columnAry.Add(column);
                        }
                        else if (ccEn == "Post")
                        {
                            postAry.Add(column);
                        }
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>角柱、円柱の柱と間柱の分割</summary>
        /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void ColumnDivision(IList<FamilyInstance> allColumnAry,
                            string column_Category_Kaku,
                            string column_Category_En,
                            string dx,
                            string dy,
                            string tyokkei,
                            ref IList<FamilyInstance> kakuColumnAry,
                            ref IList<FamilyInstance> kakuPostAry,
                            ref IList<FamilyInstance> enColumnAry,
                            ref IList<FamilyInstance> enPostAry)
        {
            foreach (FamilyInstance column in allColumnAry)
            {
                Parameter parX = column.Symbol.LookupParameter(dx);
                Parameter parY = column.Symbol.LookupParameter(dy);
                Parameter parDiameter = column.Symbol.LookupParameter(tyokkei);

                string ccKaku = column.Symbol.LookupParameter(column_Category_Kaku).AsString();
                string ccEn = column.Symbol.LookupParameter(column_Category_En).AsString();

                if (parX != null && parY != null)
                {
                    if (parX.AsDouble() > 0 && parY.AsDouble() > 0)
                    {
                        if (ccKaku == "Column")
                        {
                            kakuColumnAry.Add(column);
                        }
                        else if (ccKaku == "Post")
                        {
                            kakuPostAry.Add(column);
                        }
                    }
                }
                else if (parDiameter != null)
                {
                    if (parDiameter.AsDouble() > 0)
                    {
                        if (ccEn == "Column")
                        {
                            enColumnAry.Add(column);
                        }
                        else if (ccEn == "Post")
                        {
                            enPostAry.Add(column);
                        }
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>大梁、小梁、片持ち大梁、片持ち小梁、
        ///          基礎大梁、基礎小梁、基礎片持ち大梁、基礎片持ち小梁の分割</summary>
        /// <history>2013/05/20 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GirderDivision(IList<FamilySymbol> allGirderAry,
                            string girder_Category,
                            string girder_Category_Canti,
                            ref IList<FamilySymbol> girderAry,
                            ref IList<FamilySymbol> beamAry,
                            ref IList<FamilySymbol> cantiGirderAry,
                            ref IList<FamilySymbol> cantiBeamAry,
                            ref IList<FamilySymbol> foundationGirderAry,
                            ref IList<FamilySymbol> foundationBeamAry,
                            ref IList<FamilySymbol> cantiFoundationGirderAry,
                            ref IList<FamilySymbol> cantiFoundationBeamAry)
        {
            foreach (FamilySymbol famSym in allGirderAry)
            {
                Parameter paramCategory = famSym.LookupParameter(girder_Category);
                Parameter paramCategory_Canti = famSym.LookupParameter(girder_Category_Canti);

                if (paramCategory == null && paramCategory_Canti == null)
                {
                    continue;
                }
                // 梁符号パラメータ
                else if (paramCategory != null && paramCategory_Canti == null)
                {
                    string strCategory = paramCategory.AsString();

                    if (strCategory == "Girder")
                    {
                        girderAry.Add(famSym);
                    }
                    else if (strCategory == "Beam")
                    {
                        beamAry.Add(famSym);
                    }
                    else if (strCategory == "Foundation-Girder")
                    {
                        foundationGirderAry.Add(famSym);
                    }
                    else if (strCategory == "Foundation-Beam")
                    {
                        foundationBeamAry.Add(famSym);
                    }
                }
                // 片持ち梁符号パラメータ
                else if (paramCategory == null && paramCategory_Canti != null)
                {
                    string strCategory = paramCategory_Canti.AsString();

                    if (strCategory == "Cantilever-Girder")
                    {
                        cantiGirderAry.Add(famSym);
                    }
                    else if (strCategory == "Cantilever-Beam")
                    {
                        cantiBeamAry.Add(famSym);
                    }
                    else if (strCategory == "Cantilever-Foundation-Girder")
                    {
                        cantiFoundationGirderAry.Add(famSym);
                    }
                    else if (strCategory == "Cantilever-Foundation-Beam")
                    {
                        cantiFoundationBeamAry.Add(famSym);
                    }
                }
                // 両方持っている場合
                else
                {
                    if (paramCategory.AsString() == paramCategory_Canti.AsString())
                    {
                        string strCategory = paramCategory.AsString();

                        if (strCategory == "Girder")
                        {
                            girderAry.Add(famSym);
                        }
                        else if (strCategory == "Beam")
                        {
                            beamAry.Add(famSym);
                        }
                        else if (strCategory == "Foundation-Girder")
                        {
                            foundationGirderAry.Add(famSym);
                        }
                        else if (strCategory == "Foundation-Beam")
                        {
                            foundationBeamAry.Add(famSym);
                        }
                        else if (strCategory == "Cantilever-Girder")
                        {
                            cantiGirderAry.Add(famSym);
                        }
                        else if (strCategory == "Cantilever-Beam")
                        {
                            cantiBeamAry.Add(famSym);
                        }
                        else if (strCategory == "Cantilever-Foundation-Girder")
                        {
                            cantiFoundationGirderAry.Add(famSym);
                        }
                        else if (strCategory == "Cantilever-Foundation-Beam")
                        {
                            cantiFoundationBeamAry.Add(famSym);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>RC大梁タイプ</summary>
        /// <history>2013/04/25 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<FamilyInstance> GetGirderFamInsAry(string girderCategory)
        {
            IList<FamilyInstance> retFamInsAry = new List<FamilyInstance>();

            foreach (FamilyInstance famIns in GetRCBeamFamInsAry)
            {
                if (famIns.Symbol.LookupParameter(girderCategory).AsString() == _CmpAttribute.ResourceText("IDS_TXT_GIRDER"))
                {
                    retFamInsAry.Add(famIns);
                }
            }

            return retFamInsAry;
        }

        /// ================================================================================
        /// <summary>RC小梁タイプ</summary>
        /// <history>2013/04/25 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<FamilyInstance> GetBeamFamInsAry(string girderCategory)
        {
            IList<FamilyInstance> retFamInsAry = new List<FamilyInstance>();

            foreach (FamilyInstance famIns in GetRCBeamFamInsAry)
            {
                if (famIns.Symbol.LookupParameter(girderCategory).AsString() == _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                {
                    retFamInsAry.Add(famIns);
                }
            }

            return retFamInsAry;
        }

        /// ================================================================================
        /// <summary>片持ち大梁タイプ</summary>
        /// <history>2013/04/25 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<FamilyInstance> GetCantilever_Girder(string girderCategory)
        {
            IList<FamilyInstance> ret = new List<FamilyInstance>();

            foreach (FamilyInstance famIns in GetRCBeamFamInsAry)
            {
                if (famIns.Symbol.LookupParameter(girderCategory).AsString() == _CmpAttribute.ResourceText("IDS_TXT_CANTILEVER_GIRDER"))
                {
                    ret.Add(famIns);
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>片持ち小梁タイプ</summary>
        /// <history>2013/04/25 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<FamilyInstance> GetCantilever_Beam(string girderCategory)
        {
            IList<FamilyInstance> ret = new List<FamilyInstance>();

            foreach (FamilyInstance famIns in GetRCBeamFamInsAry)
            {
                if (famIns.Symbol.LookupParameter(girderCategory).AsString() == _CmpAttribute.ResourceText("IDS_TXT_CANTILEVER_BEAM"))
                {
                    ret.Add(famIns);
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>RC基礎大梁タイプ</summary>
        /// <history>2013/04/25 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<FamilyInstance> GetFoundationGirderFamInsAry(string girderCategory)
        {
            IList<FamilyInstance> ret = new List<FamilyInstance>();

            foreach (FamilyInstance famIns in GetRCBeamFamInsAry)
            {
                if (famIns.Symbol.LookupParameter(girderCategory).AsString() == _CmpAttribute.ResourceText("IDS_TXT_FOUNDATION_GIRDER"))
                {
                    ret.Add(famIns);
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>RC基礎小梁タイプ</summary>
        /// <history>2013/04/25 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<FamilyInstance> GetFoundationBeamFamInsAry(string girderCategory)
        {
            IList<FamilyInstance> ret = new List<FamilyInstance>();

            foreach (FamilyInstance famIns in GetRCBeamFamInsAry)
            {
                if (famIns.Symbol.LookupParameter(girderCategory).AsString() == _CmpAttribute.ResourceText("IDS_TXT_FOUNDATION_BEAM"))
                {
                    ret.Add(famIns);
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>RC基礎片持ち大梁タイプ</summary>
        /// <history>2013/04/25 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<FamilyInstance> GetCantileverFoundationGirderFamInsAry(string girderCategory)
        {
            IList<FamilyInstance> ret = new List<FamilyInstance>();

            foreach (FamilyInstance famIns in GetRCBeamFamInsAry)
            {
                if (famIns.Symbol.LookupParameter(girderCategory).AsString() == _CmpAttribute.ResourceText("IDS_TXT_CANTILEVER_FOUNDATION_GIRDER"))
                {
                    ret.Add(famIns);
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>RC基礎片持ち小梁タイプ</summary>
        /// <history>2013/04/25 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<FamilyInstance> GetCantileverFoundationBeamFamInsAry(string girderCategory)
        {
            IList<FamilyInstance> ret = new List<FamilyInstance>();

            foreach (FamilyInstance famIns in GetRCBeamFamInsAry)
            {
                if (famIns.Symbol.LookupParameter(girderCategory).AsString() == _CmpAttribute.ResourceText("IDS_TXT_CANTILEVER_FOUNDATION_BEAM"))
                {
                    ret.Add(famIns);
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>鉄筋マーク取得</summary>
        /// <history><p>2013/04/17 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2013/12/00 Modified GSA,Inc. Ryo Kuroda</p>
        ///           <p>2015/04/06 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        bool GetRebarFamily(ref Family rebarFam)
        {
            bool ret = false;

            rebarFam = null;

            // ドキュメント内のファミリから取得
            FilteredElementCollector filterElemColle = new FilteredElementCollector(RvtDBDoc);
            filterElemColle.OfClass(typeof(Family));

            foreach (Family fam in filterElemColle)
            {
                if (fam.Name == _CmpAttribute.ResourceText("IDS_TXT_TEKKINMARK"))
                {
                    rebarFam = fam;
                }
            }

            if (rebarFam == null)
            {
                // データフォルダからロード
                string famLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
                string famName = _CmpAttribute.ResourceText("IDS_TXT_TEKKINMARK") + ".rfa";
                //string famName = _CmpAttribute.ResourceText("IDS_TXT_EIZENKYOKAITEKKINMARK") + ".rfa";

                // ファイルの確認
                if (!System.IO.File.Exists(famLoc + famName))
                {
                    return ret;
                }

                Transaction trans = new Transaction(RvtDBDoc);
                trans.Start("鉄筋ファミリロード");

                RvtDBDoc.LoadFamily(famLoc + famName, out rebarFam);

                foreach (ElementId id in rebarFam.GetFamilySymbolIds())
                {
                    FamilySymbol famSym = RvtDBDoc.GetElement(id) as FamilySymbol;

                    if (famSym.IsActive == false)
                    {
                        famSym.Activate();
                    }
                }

                trans.Commit();
            }

            if (rebarFam != null)
            {
                ret = true;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>寄せ筋マーク取得</summary>
        /// <history><p>2013/06/03 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2015/04/06 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        bool GetRebarYose(ref Family yoseFam)
        {
            bool ret = false;

            yoseFam = null;
            string famLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
            string famName = _CmpAttribute.ResourceText("IDS_TXT_YOSEMARK") + ".rfa";

            // ドキュメント内のファミリから取得
            FilteredElementCollector filterElemColle = new FilteredElementCollector(RvtDBDoc);
            filterElemColle.OfClass(typeof(Family));

            yoseFam = (Family)filterElemColle.FirstOrDefault( o => o.Name == _CmpAttribute.ResourceText( "IDS_TXT_YOSEMARK" ) ) ;

            if (yoseFam == null)
            {
                // データフォルダからロード
                if (!System.IO.File.Exists(famLoc + famName))
                {
                    return ret;
                }

                var trans = new Transaction(RvtDBDoc);
                trans.Start("鉄筋ファミリロード");

                RvtDBDoc.LoadFamily(famLoc + famName, new FamilyLoadOption(), out yoseFam);

                foreach (ElementId id in yoseFam.GetFamilySymbolIds())
                {
                    FamilySymbol famSym = RvtDBDoc.GetElement(id) as FamilySymbol;

                    if (famSym.IsActive == false)
                    {
                        famSym.Activate();
                    }
                }

                trans.Commit();
            }

            if (yoseFam != null)
            {
                ret = true;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>ファミリインスタンスの並び替え規則</summary>
        /// <history>2013/03/05 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        int CompareToFamIns_Name_Level_Id(FamilyInstance fi1, FamilyInstance fi2)
        {
            int ret = 0;

            if (string.Compare(fi1.Name, fi2.Name, false) != 0)
            {
                ret = string.Compare(fi1.Name, fi2.Name, true);
            }
            else
            {
                Level lvl1 = null;
                Level lvl2 = null;

                if (fi1.StructuralType == Revit.DB.Structure.StructuralType.Beam)
                {
                    lvl1 = RvtDBDoc.GetElement(fi1.Host.Id) as Level;
                    lvl2 = RvtDBDoc.GetElement(fi2.Host.Id) as Level;
                }
                else if (fi1.StructuralType == Revit.DB.Structure.StructuralType.Column)
                {
                    lvl1 = RvtDBDoc.GetElement(fi1.LevelId) as Level;
                    lvl2 = RvtDBDoc.GetElement(fi2.LevelId) as Level;
                }

                if (string.Compare(lvl1.Elevation.ToString(), lvl2.Elevation.ToString(), false) != 0)
                {
                    ret = string.Compare((lvl1.Elevation * -1).ToString(), (lvl2.Elevation * -1).ToString(), false);
                }
                else
                {
                    ret = string.Compare(fi1.Id.Value.ToString(), fi2.Id.Value.ToString(), false);
                }
            }

            return -ret;
        }

        /// ================================================================================
        /// <summary>n位(10^digits)に四捨五入</summary>
        /// ================================================================================
        public
        double ToHalfAdjust(double value, int digits)
        {
            digits = digits * -1;

            // 10のべき乗
            double dCoef = Math.Pow(10, digits);

            // 指定位を1の位にする
            // 値が0より大きい場合
            // 0.5を足した値以下の最大の整数を10のべき乗で割る
            // 値が0以下の場合
            // 0.5を引いた値以上の最小の整数を10のべき乗で割る
            return value > 0 ? Math.Floor((value * dCoef) + 0.5) / dCoef :
                               Math.Ceiling((value * dCoef) - 0.5) / dCoef;

            // 例 1
            // value = 10.56, digits = 0
            // 10^digits = 10^0 = 1
            // value > 0 == true
            // Floor(10.56 * 1 + 0.5) = Floor(10.56 + 0.5) = Floor(11.06) = 11
            // return 11

            // 例 2
            // value = 0.014, digits = -2
            // digits = -2 * -1
            // 10^digits = 10^2 = 100
            // value > 0 == false
            // Ceiling(0.014 * 100 - 0.5) = Ceiling(1.4 -0.5) = Ceiling(0.9) = 1
            // 1 / 100 = 0.01
            // return 0.01
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>文字タイプ</summary>
        /// <history>2013/02/04 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<TextNoteType> TxtNoteTypes
        {
            get
            {
                // 戻り値
                IList<TextNoteType> rets = new List<TextNoteType>();

                FilteredElementCollector collector = new FilteredElementCollector(RvtDBDoc);
                collector.OfClass(typeof(TextNoteType));

                foreach (TextNoteType tnp in collector)
                {
                    rets.Add(tnp);
                }

                return rets;
            }
        }

        /// ================================================================================
        /// <summary>寸法タイプ</summary>
        /// <history>2013/02/21 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<DimensionType> DimTypes
        {
            get
            {
                // 戻り値
                IList<DimensionType> rets = new List<DimensionType>();

                FilteredElementCollector collector = new FilteredElementCollector(RvtDBDoc);
                collector.OfClass(typeof(DimensionType));

                foreach (DimensionType dt in collector)
                {
                    if (dt.StyleType == DimensionStyleType.Linear && dt.Parameters.Size > 0 && dt.Name != dt.FamilyName)
                    {
                        rets.Add(dt);
                    }
                }

                return rets;
            }
        }

        /// ================================================================================
        /// <summary>詳細線分タイプ</summary>
        /// <history>2013/02/21 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<GraphicsStyle> DetailGraStyles
        {
            get
            {
                // 戻り値
                // 線種からグラフィックスタイルを取得
                IList<GraphicsStyle> graStyleColle = new List<GraphicsStyle>();

                // 詳細線分が使っていない線種もあるので
                // ダミーの線分を作って詳細線分で使う線種だけを取得
                XYZ siten = new XYZ(0, 0, 0);
                XYZ syuten = new XYZ(1, 1, 0);

                View actiview = RvtDBDoc.ActiveView;
                ViewSection viewSec = actiview as ViewSection;
                if (viewSec != null)
                {
                    syuten = new XYZ(0, 0, 1);
                }

                Line line = Line.CreateBound(siten, syuten);
                DetailLine dl = null;

                Transaction trans = new Transaction(RvtDBDoc);

                // 3Dビュー
                View3D view3d = actiview as View3D;
                if (view3d != null)
                {
                    FilteredElementCollector filterElemColle = new FilteredElementCollector(RvtDBDoc);
                    filterElemColle.OfCategory(BuiltInCategory.OST_Lines);

                    IList<DetailCurve> DCs = new List<DetailCurve>();
                    foreach (Element element in filterElemColle)
                    {
                        DetailCurve dc = element as DetailCurve;
                        if (dc != null)
                        {
                            DCs.Add(dc);
                        }
                    }

                    // 詳細線分がプロジェクトにある場合
                    if (DCs.Count() > 0)
                    {
                        foreach (Element element in filterElemColle)
                        {
                            DetailLine dLine = element as DetailLine;
                            if (dLine != null)
                            {
                                foreach (ElementId eId in dLine.GetLineStyleIds())
                                {
                                    GraphicsStyle graStyle = RvtDBDoc.GetElement(eId) as GraphicsStyle;
                                    graStyleColle.Add(graStyle);
                                }
                                if (graStyleColle.Count > 0)
                                {
                                    return graStyleColle;
                                }
                            }
                            else
                            {
                                DetailArc dArc = element as DetailArc;
                                if (dArc != null)
                                {
                                    foreach (ElementId eId in dArc.GetLineStyleIds())
                                    {
                                        GraphicsStyle graStyle = RvtDBDoc.GetElement(eId) as GraphicsStyle;
                                        graStyleColle.Add(graStyle);
                                    }
                                    if (graStyleColle.Count > 0)
                                    {
                                        return graStyleColle;
                                    }
                                }
                            }
                        }
                    }
                    // 詳細線分がプロジェクトにない場合
                    else
                    {
                        // 平面ビューがあれば、
                        // 平面ビューに詳細線分を作成して削除
                        FilteredElementCollector colle = new FilteredElementCollector(RvtDBDoc);
                        colle.OfClass(typeof(ViewPlan));
                        IList<ViewPlan> vps = new List<ViewPlan>();
                        foreach (ViewPlan vp in colle)
                        {
                            vps.Add(vp);
                        }

                        if (vps.Count > 0)
                        {
                            foreach (ViewPlan vp in vps)
                            {
                                line = Line.CreateBound(siten, syuten);
                                dl = null;

                                trans.Start("詳細線分線種取得");

                                try
                                {
                                    dl = RvtDBDoc.Create.NewDetailCurve(vp, line) as DetailLine;
                                    trans.Commit();
                                }
                                catch (Revit.Exceptions.InvalidOperationException)
                                {
                                    trans.Commit();
                                    continue;
                                }
                                catch (Revit.Exceptions.ArgumentException)
                                {
                                    trans.Commit();
                                    continue;
                                }
                                catch
                                {
                                    trans.Commit();
                                    continue;
                                }

                                IList<Element> elementAry = new List<Element>();

                                if (dl != null)
                                {
                                    foreach (ElementId eId in dl.GetLineStyleIds())
                                    {
                                        Element e = RvtDBDoc.GetElement(eId);
                                        elementAry.Add(e);
                                    }

                                    // ダミーを削除
                                    trans.Start("ダミー削除");
                                    RvtDBDoc.Delete(dl.Id);
                                    trans.Commit();

                                    foreach (Element elem in elementAry)
                                    {
                                        GraphicsStyle graStyle = elem as GraphicsStyle;
                                        if (graStyle != null)
                                        {
                                            graStyleColle.Add(graStyle);
                                        }
                                    }

                                    if (trans.GetStatus() == TransactionStatus.Started)
                                    {
                                        trans.Commit();
                                    }

                                    if (graStyleColle.Count > 0)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            if (graStyleColle.Count < 1)
                            {
                                filterElemColle = new FilteredElementCollector(RvtDBDoc);
                                filterElemColle.OfClass(typeof(GraphicsStyle));

                                if (filterElemColle.Count() > 0)
                                {
                                    foreach (GraphicsStyle gs in filterElemColle)
                                    {
                                        try
                                        {
                                            if (gs.GraphicsStyleCategory.Parent.Id.Value.Equals((long)BuiltInCategory.OST_Lines))
                                            {
                                                graStyleColle.Add(gs);
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    return graStyleColle;
                                }
                                else
                                {
                                    return graStyleColle;
                                }
                            }

                            return graStyleColle;
                        }
                        else
                        {
                            filterElemColle = new FilteredElementCollector(RvtDBDoc);
                            filterElemColle.OfClass(typeof(GraphicsStyle));

                            if (filterElemColle.Count() > 0)
                            {
                                foreach (GraphicsStyle gs in filterElemColle)
                                {
                                    try
                                    {
                                        if (gs.GraphicsStyleCategory.Parent.Id.Value.Equals((long)BuiltInCategory.OST_Lines))
                                        {
                                            graStyleColle.Add(gs);
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                                return graStyleColle;
                            }
                            else
                            {
                                return graStyleColle;
                            }
                        }
                    }
                }
                // 3D以外
                else
                {
                    FilteredElementCollector filterElemColle = new FilteredElementCollector(RvtDBDoc);
                    filterElemColle.OfCategory(BuiltInCategory.OST_Lines);

                    IList<DetailCurve> DCs = new List<DetailCurve>();
                    foreach (Element element in filterElemColle)
                    {
                        DetailCurve dc = element as DetailCurve;
                        if (dc != null)
                        {
                            DCs.Add(dc);
                        }
                    }

                    // 詳細線分がプロジェクトにある場合
                    if (DCs.Count() > 0)
                    {
                        foreach (Element element in filterElemColle)
                        {
                            DetailLine dLine = element as DetailLine;
                            if (dLine != null)
                            {
                                foreach (ElementId eId in dLine.GetLineStyleIds())
                                {
                                    GraphicsStyle graStyle = RvtDBDoc.GetElement(eId) as GraphicsStyle;
                                    graStyleColle.Add(graStyle);
                                }
                                if (graStyleColle.Count > 0)
                                {
                                    return graStyleColle;
                                }
                            }
                            else
                            {
                                DetailArc dArc = element as DetailArc;
                                if (dArc != null)
                                {
                                    foreach (ElementId eId in dArc.GetLineStyleIds())
                                    {
                                        GraphicsStyle graStyle = RvtDBDoc.GetElement(eId) as GraphicsStyle;
                                        graStyleColle.Add(graStyle);
                                    }
                                    if (graStyleColle.Count > 0)
                                    {
                                        return graStyleColle;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        trans.Start("詳細線分線種取得");

                        try
                        {
                            dl = RvtDBDoc.Create.NewDetailCurve(RvtDBDoc.ActiveView, line) as DetailLine;
                            trans.Commit();
                        }
                        catch (Revit.Exceptions.InvalidOperationException)
                        {
                            trans.Commit();
                            return graStyleColle;
                        }
                        catch (Revit.Exceptions.ArgumentException)
                        {
                            trans.Commit();
                            return graStyleColle;
                        }
                        catch
                        {
                            trans.Commit();
                            return graStyleColle;
                        }

                        IList<Element> elemAry = new List<Element>();

                        foreach (ElementId eId in dl.GetLineStyleIds())
                        {
                            Element e = RvtDBDoc.GetElement(eId);
                            elemAry.Add(e);
                        }

                        // ダミーを削除
                        trans.Start("ダミー削除");
                        RvtDBDoc.Delete(dl.Id);
                        trans.Commit();

                        foreach (Element elem in elemAry)
                        {
                            GraphicsStyle graStyle = elem as GraphicsStyle;
                            if (graStyle != null)
                            {
                                graStyleColle.Add(graStyle);
                            }
                        }

                        if (trans.GetStatus() == TransactionStatus.Started)
                        {
                            trans.Commit();
                        }
                    }
                }

                return graStyleColle;
            }
        }

        /// ================================================================================
        /// <summary>RC柱インスタンス(厳密にはコンクリート柱)</summary>
        /// <history><p>2013/03/01 Created GSA,Inc. Ryo Kuroda</p>
        ///          <p>2013/05/20 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        IList<FamilyInstance> GetRCColumnFamInsAry
        {
            get
            {
                IList<FamilyInstance> retFamInsAry = new List<FamilyInstance>();

                FilteredElementCollector collector = new FilteredElementCollector(RvtDBDoc);
                collector.OfClass(typeof(FamilyInstance));

                foreach (Element elem in collector)
                {
                    FamilyInstance famIns = elem as FamilyInstance;

                    if (famIns.StructuralType == Revit.DB.Structure.StructuralType.Column &&
                       (famIns.StructuralMaterialType == Revit.DB.Structure.StructuralMaterialType.Concrete ||
                        famIns.StructuralMaterialType == Revit.DB.Structure.StructuralMaterialType.PrecastConcrete))
                    {
                        retFamInsAry.Add(famIns);
                    }
                }

                return retFamInsAry;
            }
        }

        /// ================================================================================
        /// <summary>RC柱タイプ(厳密にはコンクリート柱)</summary>
        /// <history>2013/10/02 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<FamilySymbol> GetRCColumnFamSymAry
        {
            get
            {
                IList<FamilySymbol> ret = new List<FamilySymbol>();

                FilteredElementCollector coll = new FilteredElementCollector(RvtDBDoc);
                coll.OfCategory(BuiltInCategory.OST_StructuralColumns).OfClass(typeof(FamilySymbol));

                foreach (FamilySymbol fs in coll)
                {
                    if (fs.Family.StructuralMaterialType == Revit.DB.Structure.StructuralMaterialType.Concrete)
                    {
                        ret.Add(fs);
                    }
                }

                return ret;
            }
        }

        /// ================================================================================
        /// <summary>RC梁インスタンス(厳密にはコンクリート梁)</summary>
        /// <history>2013/03/01 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<FamilyInstance> GetRCBeamFamInsAry
        {
            get
            {
                IList<FamilyInstance> retFamInsAry = new List<FamilyInstance>();

                FilteredElementCollector collector = new FilteredElementCollector(RvtDBDoc);
                collector.OfClass(typeof(FamilyInstance));

                foreach (Element elem in collector)
                {
                    FamilyInstance famIns = elem as FamilyInstance;

                    if (famIns.StructuralType == Revit.DB.Structure.StructuralType.Beam &&
                       (famIns.StructuralMaterialType == Revit.DB.Structure.StructuralMaterialType.Concrete ||
                        famIns.StructuralMaterialType == Revit.DB.Structure.StructuralMaterialType.PrecastConcrete))
                    {
                        retFamInsAry.Add(famIns);
                    }
                }

                return retFamInsAry;
            }
        }

        /// ================================================================================
        /// <summary>RC梁タイプ(厳密にはコンクリート梁)</summary>
        /// <history>2013/10/02 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<FamilySymbol> GetRCBeamFamSymAry
        {
            get
            {
                IList<FamilySymbol> retFamInsAry = new List<FamilySymbol>();

                FilteredElementCollector collector = new FilteredElementCollector(RvtDBDoc);
                collector.OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilySymbol));

                foreach (FamilySymbol fs in collector)
                {
                    if (fs.Family.StructuralMaterialType == Revit.DB.Structure.StructuralMaterialType.Concrete)
                    {
                        retFamInsAry.Add(fs);
                    }
                }

                return retFamInsAry;
            }
        }

        /// ================================================================================
        /// <summary>大梁か小梁か</summary>
        /// <history>2013/03/18 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        int IsUsageGrider(FamilyInstance famIns)
        {
            int ret = 0;

            BuiltInParameter bltInParam = BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM;

            ret = famIns.get_Parameter(bltInParam).AsInteger();

            return ret;
        }

        /// ================================================================================
        /// <summary>全レベル</summary>
        /// <history>2013/05/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        IList<Level> AllLevelAry
        {
            get
            {
                IList<Level> ret = new List<Level>();

                FilteredElementCollector colle = new FilteredElementCollector(RvtDBDoc);
                //colle.OfCategory(Revit.DB.BuiltInCategory.OST_Levels);
                colle.OfClass(typeof(Level));

                foreach (Level l in colle)
                {
                    ret.Add(l);
                }

                return ret;
            }
        }

        /// ================================================================================
        /// <summary>枠線スタイル</summary>
        /// <history>2013/07/02 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        GraphicsStyle FrameLineStyle
        {
            get
            {
                return _FrameLineStyle;
            }
        }

        /// ================================================================================
        /// <summary>躯体線スタイル</summary>
        /// <history>2013/07/02 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        GraphicsStyle BodyLineStyle
        {
            get
            {
                return _BodyLineStyle;
            }
        }

        /// ================================================================================
        /// <summary>幅止筋線スタイル</summary>
        /// <history>2013/07/02 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        GraphicsStyle SpacerLineStyle
        {
            get
            {
                return _SpacerLineStyle;
            }
        }

        #endregion Properties
    }

    /// ================================================================================
    /// <summary>ファミリーロードオプション</summary>
    /// ================================================================================
    public
    class FamilyLoadOption : IFamilyLoadOptions
    {
        public bool OnFamilyFound(bool familyInUse,
                                  out bool overwriteParameterValues)
        {
            overwriteParameterValues = familyInUse;
            return familyInUse;
        }

        public bool OnSharedFamilyFound(Family sharedFamily,
                                        bool familyInUse,
                                        out FamilySource source,
                                        out bool overwriteParameterValues)
        {
            source = FamilySource.Family;
            overwriteParameterValues = true;
            return true;
        }
    }
}