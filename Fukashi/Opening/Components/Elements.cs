
using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.Ext.Fukashi.Opening;

namespace ADSK.Ext.Fukashi.Opening.Components
{
    /// ================================================================================
    /// <summary>要素</summary>
    /// ================================================================================
    public class Elements
    {
        // メンバ変数
        #region Member Variables

        private readonly Revit.UI.UIDocument _rvtUidoc;

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>フカシファミリシンボル - 一般モデル_三角形</summary>
        private Revit.DB.FamilySymbol _FamSymTriangle_GenericModel;

        /// <summary>フカシファミリシンボル - 一般モデル_長方形 側面長方形</summary>
        private Revit.DB.FamilySymbol _FamSymRectRect_GenericModel;

        /// <summary>フカシファミリシンボル - 一般モデル_台形</summary>
        private Revit.DB.FamilySymbol _FamSymTorapezoid_GenericModel;

        /// <summary>フカシファミリシンボル - 一般モデル_平行四辺形</summary>
        private Revit.DB.FamilySymbol _FamSymParallelogram_GenericModel;

        /// <summary>フカシファミリシンボル - 一般モデル_L字形</summary>
        private Revit.DB.FamilySymbol _FamSymLshape_GenericModel;

        /// <summary>フカシファミリシンボル - 一般モデル_凸凹形</summary>
        private Revit.DB.FamilySymbol _FamSymUneven_GenericModel;

        /// <summary>ビューズーム領域</summary>
        private Collections.Generic.IList<Revit.DB.XYZ> _ZoomCorners;

        #endregion

        // コンストラクタ
        #region Constructor
        /// ================================================================================
        /// <summary>要素</summary>
        ///
        /// <param name="rvtUiDoc">UIドキュメント</param>
        ///
        /// <history>2016/11/17 Created  CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public Elements(Revit.UI.UIDocument rvtUiDoc,
                        RvtExtApp.Components.Attribute cmpAttribute)
        {
            _rvtUidoc = rvtUiDoc ?? throw new ArgumentNullException(nameof(rvtUiDoc));
            _CmpAttribute = cmpAttribute;
        }

        /// <summary>UIドキュメント</summary>
        public Revit.UI.UIDocument RvtUIDoc => _rvtUidoc;

        /// <summary>DBドキュメント</summary>
        public Revit.DB.Document RvtDBDoc => _rvtUidoc.Document;

        /// <summary>ゼロ相当寸法</summary>
        public double Approx0Len => 1.0e-9;

        /// <summary>プロジェクト情報</summary>
        public Revit.DB.ProjectInfo ProjectInfo => _rvtUidoc.Document.ProjectInformation;

        #endregion

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>マテリアル取得</summary>
        ///
        /// <history>2016/11/21 Created  CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Material> GetMaterials()
        {
            // 戻り値
            Collections.Generic.List<Revit.DB.Material> ret = new Collections.Generic.List<Revit.DB.Material>();

            Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(RvtDBDoc);
            fec.OfCategory(Autodesk.Revit.DB.BuiltInCategory.OST_Materials);

            foreach (Revit.DB.Material material in fec)
            {
                ret.Add(material);
            }

            ret.Sort(new MaterialNameComparer());

            return ret;
        }

        /// ================================================================================
        /// <summary>高さ順レベル取得</summary>
        ///
        /// <history>2016/12/05 Created  CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Level> GetElevationOrderLevels()
        {
            // 戻り値
            Collections.Generic.List<Revit.DB.Level> ret = new Collections.Generic.List<Revit.DB.Level>();

            Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(RvtDBDoc);
            fec.OfClass(typeof(Revit.DB.Level));

            foreach (Revit.DB.Level level in fec)
            {
                ret.Add(level);
            }

            ret.Sort(new LevelElevationComparer());

            return ret;
        }

        /// ================================================================================
        /// <summary>上レベル取得</summary>
        /// 
        /// <param name="level">レベル</param>
        /// 
        /// <history>2016/12/05 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Revit.DB.Level GetUpperLevel(Revit.DB.Level level)
        {
            // 戻り値
            Revit.DB.Level ret = null;

            Collections.Generic.IList<Revit.DB.Level> levels = GetElevationOrderLevels();

            for (int i = 0; i < levels.Count; ++i)
            {
                if (i < levels.Count - 1)
                {
                    Revit.DB.Level lvl = levels[i];

                    if (level.Id.ToString() == lvl.Id.ToString())
                    {
                        ret = levels[i + 1];
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>フカシファミリ取得 - 三角形</summary>
        ///
        /// <history>2017/01/11 Created  CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetFukashiFamily_Triangle()
        {
            Revit.DB.Transaction trans = new Revit.DB.Transaction(RvtDBDoc);
            trans.Start("フカシファミリ取得");

            // プロジェクトのファミリ取得
            Revit.DB.FilteredElementCollector filterElemColle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
            filterElemColle.OfClass(typeof(Revit.DB.Family));

            foreach (Revit.DB.Family family in filterElemColle)
            {
                // 三角形
                if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE"))
                {
                    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                    {
                        Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                        if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE"))
                        {
                            _FamSymTriangle_GenericModel = famSym;

                            if (_FamSymTriangle_GenericModel.IsActive == false)
                            {
                                _FamSymTriangle_GenericModel.Activate();
                            }
                        }
                    }
                }
            }

            // フォルダからロード
            string famLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

            if (_FamSymTriangle_GenericModel == null)
            {
                string famName = _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE") + ".rfa";

                if (System.IO.File.Exists(famLoc + famName))
                {
                    Revit.DB.Family family = null;

                    RvtDBDoc.LoadFamily(famLoc + famName, out family);

                    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                    {
                        Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                        if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE"))
                        {
                            _FamSymTriangle_GenericModel = famSym;

                            if (_FamSymTriangle_GenericModel.IsActive == false)
                            {
                                _FamSymTriangle_GenericModel.Activate();
                            }
                        }
                    }
                }
            }

            trans.Commit();
        }

        /// ================================================================================
        /// <summary>フカシファミリ取得 - 長方形</summary>
        ///
        /// <history>2017/01/11 Created  CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetFukashiFamily_Rectangle()
        {
            Revit.DB.Transaction trans = new Revit.DB.Transaction(RvtDBDoc);
            trans.Start("フカシファミリ取得");

            // プロジェクトのファミリ取得
            Revit.DB.FilteredElementCollector filterElemColle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
            filterElemColle.OfClass(typeof(Revit.DB.Family));

            foreach (Revit.DB.Family family in filterElemColle)
            {
                if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE"))
                {
                    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                    {
                        Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                        if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE"))
                        {
                            _FamSymRectRect_GenericModel = famSym;

                            if (_FamSymRectRect_GenericModel.IsActive == false)
                            {
                                _FamSymRectRect_GenericModel.Activate();
                            }
                        }
                    }
                }
            }

            // フォルダからロード
            string famLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

            if (_FamSymRectRect_GenericModel == null)
            {
                string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE") + ".rfa";

                if (System.IO.File.Exists(famLoc + famName))
                {
                    Revit.DB.Family family = null;

                    RvtDBDoc.LoadFamily(famLoc + famName, out family);

                    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                    {
                        Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                        if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE"))
                        {
                            _FamSymRectRect_GenericModel = famSym;

                            if (_FamSymRectRect_GenericModel.IsActive == false)
                            {
                                _FamSymRectRect_GenericModel.Activate();
                            }
                        }
                    }
                }
            }

            trans.Commit();
        }

        /// ================================================================================
        /// <summary>フカシファミリ取得 - 台形</summary>
        ///
        /// <history>2017/01/11 Created  CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetFukashiFamily_Torapezoid()
        {
            Revit.DB.Transaction trans = new Revit.DB.Transaction(RvtDBDoc);
            trans.Start("フカシファミリ取得");

            // プロジェクトのファミリ取得
            Revit.DB.FilteredElementCollector filterElemColle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
            filterElemColle.OfClass(typeof(Revit.DB.Family));

            foreach (Revit.DB.Family family in filterElemColle)
            {
                if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID"))
                {
                    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                    {
                        Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                        if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID"))
                        {
                            _FamSymTorapezoid_GenericModel = famSym;

                            if (_FamSymTorapezoid_GenericModel.IsActive == false)
                            {
                                _FamSymTorapezoid_GenericModel.Activate();
                            }
                        }
                    }
                }
            }

            // フォルダからロード
            string famLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

            if (_FamSymTorapezoid_GenericModel == null)
            {
                string famName = _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID") + ".rfa";

                if (System.IO.File.Exists(famLoc + famName))
                {
                    Revit.DB.Family family = null;

                    RvtDBDoc.LoadFamily(famLoc + famName, out family);

                    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                    {
                        Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                        if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID"))
                        {
                            _FamSymTorapezoid_GenericModel = famSym;

                            if (_FamSymTorapezoid_GenericModel.IsActive == false)
                            {
                                _FamSymTorapezoid_GenericModel.Activate();
                            }
                        }
                    }
                }
            }

            trans.Commit();
        }

        /// ================================================================================
        /// <summary>フカシファミリ取得 - 平行四辺形</summary>
        ///
        /// <history>2017/01/12 Created  CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetFukashiFamily_Parallelogram()
        {
            Revit.DB.Transaction trans = new Revit.DB.Transaction(RvtDBDoc);
            trans.Start("フカシファミリ取得");

            // プロジェクトのファミリ取得
            Revit.DB.FilteredElementCollector filterElemColle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
            filterElemColle.OfClass(typeof(Revit.DB.Family));

            foreach (Revit.DB.Family family in filterElemColle)
            {
                if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM"))
                {
                    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                    {
                        Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                        if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM"))
                        {
                            _FamSymParallelogram_GenericModel = famSym;

                            if (_FamSymParallelogram_GenericModel.IsActive == false)
                            {
                                _FamSymParallelogram_GenericModel.Activate();
                            }
                        }
                    }
                }
            }

            // フォルダからロード
            string famLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

            if (_FamSymParallelogram_GenericModel == null)
            {
                string famName = _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM") + ".rfa";

                if (System.IO.File.Exists(famLoc + famName))
                {
                    Revit.DB.Family family = null;

                    RvtDBDoc.LoadFamily(famLoc + famName, out family);

                    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                    {
                        Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                        if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM"))
                        {
                            _FamSymParallelogram_GenericModel = famSym;

                            if (_FamSymParallelogram_GenericModel.IsActive == false)
                            {
                                _FamSymParallelogram_GenericModel.Activate();
                            }
                        }
                    }
                }
            }

            trans.Commit();
        }

        /// ================================================================================
        /// <summary>フカシファミリ取得 - L形</summary>
        ///
        /// <history>2017/01/12 Created  CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetFukashiFamily_Lshape()
        {
            Revit.DB.Transaction trans = new Revit.DB.Transaction(RvtDBDoc);
            trans.Start("フカシファミリ取得");

            // プロジェクトのファミリ取得
            Revit.DB.FilteredElementCollector filterElemColle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
            filterElemColle.OfClass(typeof(Revit.DB.Family));

            foreach (Revit.DB.Family family in filterElemColle)
            {
                if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE"))
                {
                    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                    {
                        Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                        if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE"))
                        {
                            _FamSymLshape_GenericModel = famSym;

                            if (_FamSymLshape_GenericModel.IsActive == false)
                            {
                                _FamSymLshape_GenericModel.Activate();
                            }
                        }
                    }
                }
            }

            // フォルダからロード
            string famLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

            if (_FamSymLshape_GenericModel == null)
            {
                string famName = _CmpAttribute.ResourceText("IDS_FAM_LSHAPE") + ".rfa";

                if (System.IO.File.Exists(famLoc + famName))
                {
                    Revit.DB.Family family = null;

                    RvtDBDoc.LoadFamily(famLoc + famName, out family);

                    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                    {
                        Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                        if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE"))
                        {
                            _FamSymLshape_GenericModel = famSym;

                            if (_FamSymLshape_GenericModel.IsActive == false)
                            {
                                _FamSymLshape_GenericModel.Activate();
                            }
                        }
                    }
                }
            }

            trans.Commit();
        }

        /// ================================================================================
        /// <summary>フカシファミリ取得 - 凸凹形</summary>
        ///
        /// <history>2017/01/12 Created  CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetFukashiFamily_Uneven()
        {
            Revit.DB.Transaction trans = new Revit.DB.Transaction(RvtDBDoc);
            trans.Start("フカシファミリ取得");

            // プロジェクトのファミリ取得
            Revit.DB.FilteredElementCollector filterElemColle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
            filterElemColle.OfClass(typeof(Revit.DB.Family));

            foreach (Revit.DB.Family family in filterElemColle)
            {
                if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN"))
                {
                    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                    {
                        Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                        if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN"))
                        {
                            _FamSymUneven_GenericModel = famSym;

                            if (_FamSymUneven_GenericModel.IsActive == false)
                            {
                                _FamSymUneven_GenericModel.Activate();
                            }
                        }
                    }
                }
            }

            // フォルダからロード
            string famLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

            if (_FamSymUneven_GenericModel == null)
            {
                string famName = _CmpAttribute.ResourceText("IDS_FAM_UNEVEN") + ".rfa";

                if (System.IO.File.Exists(famLoc + famName))
                {
                    Revit.DB.Family family = null;

                    RvtDBDoc.LoadFamily(famLoc + famName, out family);

                    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                    {
                        Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                        if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN"))
                        {
                            _FamSymUneven_GenericModel = famSym;

                            if (_FamSymUneven_GenericModel.IsActive == false)
                            {
                                _FamSymUneven_GenericModel.Activate();
                            }
                        }
                    }
                }
            }

            trans.Commit();
        }

        /// ================================================================================
        /// <summary>現在ビューにフィット</summary>
        /// 
        /// <param name="activeView">現在ビュー</param>
        /// 
        /// <history>2016/12/27 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        void FitActiveView(Revit.DB.View activeView)
        {
            Collections.Generic.IList<Revit.UI.UIView> openViews = RvtUIDoc.GetOpenUIViews();

            foreach (Revit.UI.UIView uiView in openViews)
            {
                if (uiView.ViewId.ToString() == activeView.Id.ToString())
                {
                    _ZoomCorners = uiView.GetZoomCorners();

                    uiView.ZoomToFit();
                    break;
                }
            }
        }

        /// ================================================================================
        /// <summary>現在ビューにズーム</summary>
        /// 
        /// <param name="activeView">現在ビュー</param>
        /// 
        /// <history>2016/12/27 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        void ZoomActiveView(Revit.DB.View activeView)
        {
            Collections.Generic.IList<Revit.UI.UIView> openViews = RvtUIDoc.GetOpenUIViews();

            foreach (Revit.UI.UIView uiView in openViews)
            {
                if (uiView.ViewId.ToString() == activeView.Id.ToString())
                {
                    uiView.ZoomAndCenterRectangle(_ZoomCorners[0], _ZoomCorners[1]);
                    break;
                }
            }
        }

        #endregion

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>フカシファミリシンボル - 三角形</summary>
        ///
        /// <history><p>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/01/11 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        Revit.DB.FamilySymbol FamSymTriangle
        {
            get
            {
                if (_FamSymTriangle_GenericModel == null)
                {
                    GetFukashiFamily_Triangle();
                }

                return _FamSymTriangle_GenericModel;
            }
        }

        /// ================================================================================
        /// <summary>フカシファミリシンボル - 長方形 側面長方形</summary>
        ///
        /// <history><p>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/01/11 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        Revit.DB.FamilySymbol FamSymRectRect
        {
            get
            {
                if (_FamSymRectRect_GenericModel == null)
                {
                    GetFukashiFamily_Rectangle();
                }

                return _FamSymRectRect_GenericModel;
            }
        }

        /// ================================================================================
        /// <summary>フカシファミリシンボル - 台形</summary>
        ///
        /// <history><p>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/01/11 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        Revit.DB.FamilySymbol FamSymTorapezoid
        {
            get
            {
                if (_FamSymTorapezoid_GenericModel == null)
                {
                    GetFukashiFamily_Torapezoid();
                }

                return _FamSymTorapezoid_GenericModel;
            }
        }

        /// ================================================================================
        /// <summary>フカシファミリシンボル - 平行四辺形</summary>
        ///
        /// <history><p>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/01/12 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        Revit.DB.FamilySymbol FamSymParallelogram
        {
            get
            {
                if (_FamSymParallelogram_GenericModel == null)
                {
                    GetFukashiFamily_Parallelogram();
                }

                return _FamSymParallelogram_GenericModel;
            }
        }

        /// ================================================================================
        /// <summary>フカシファミリシンボル - L字形</summary>
        ///
        /// <history><p>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/01/12 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        Revit.DB.FamilySymbol FamSymLshape
        {
            get
            {
                if (_FamSymLshape_GenericModel == null)
                {
                    GetFukashiFamily_Lshape();
                }

                return _FamSymLshape_GenericModel;
            }
        }

        /// ================================================================================
        /// <summary>フカシファミリシンボル - 凸凹形</summary>
        ///
        /// <history><p>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/01/12 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        Revit.DB.FamilySymbol FamSymUneven
        {
            get
            {
                if (_FamSymUneven_GenericModel == null)
                {
                    GetFukashiFamily_Uneven();
                }

                return _FamSymUneven_GenericModel;
            }
        }

        /// ================================================================================
        /// <summary>ビューズーム領域</summary>
        ///
        /// <history>2017/01/10 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.XYZ> ViewZoomCorners
        {
            get
            {
                return _ZoomCorners;
            }
        }

        #endregion
    }

    /// ================================================================================
    /// <summary>マテリアル名の並び替え</summary>
    /// 
    /// <history>2016/11/22 Created  CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public class MaterialNameComparer : System.Collections.Generic.IComparer<Revit.DB.Material>
    {
        public int Compare(Revit.DB.Material materialA, Revit.DB.Material materialB)
        {
            // 戻り値
            int ret = 0;

            string nameA = materialA.Name;
            string nameB = materialB.Name;

            ret = string.Compare(nameA, nameB);

            return ret;
        }
    }

    /// ================================================================================
    /// <summary>レベル高さの並び替え</summary>
    /// 
    /// <history>2016/12/05 Created  CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public class LevelElevationComparer : System.Collections.Generic.IComparer<Revit.DB.Level>
    {
        public int Compare(Revit.DB.Level levelA, Revit.DB.Level levelB)
        {
            // 戻り値
            int ret = 0;

            double elevA = levelA.Elevation;
            double elevB = levelB.Elevation;

            if (elevA == elevB)
            {
                ret = string.Compare(levelA.Name, levelB.Name);
            }
            else if (elevA < elevB)
            {
                ret = -1;
            }
            else if (elevA > elevB)
            {
                ret = 1;
            }

            return ret;
        }
    }
}
