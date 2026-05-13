using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using RvtExtApp   = ADSK.Ext.Fukashi;

namespace ADSK.Ext.Fukashi.Face.Components
{
  /// ================================================================================
  /// <summary>要素</summary>
  /// ================================================================================
  public class Elements
  {
    // メンバ変数
    #region Member Variables

    /// <summary>ドキュメント</summary>
    private readonly Revit.UI.UIDocument _rvtUiDoc;

    /// <summary>属性</summary>
    private readonly RvtExtApp.Face.Components.Attribute _CmpAttribute;

    /// <summary>要素カテゴリ</summary>
    Revit.DB.Category _ElemCategory;

    /// <summary>マテリアルID</summary>
    private Revit.DB.ElementId _MaterialId;

    /// <summary>グラフィックススタイルID</summary>
    private Revit.DB.ElementId _GraphicsStyleId;

    /// <summary>フカシファミリシンボル - 一般モデル_三角形</summary>
    private Revit.DB.FamilySymbol _FamSymTriangle_GenericModel;

    /// <summary>フカシファミリシンボル - 一般モデル_長方形 側面長方形</summary>
    private Revit.DB.FamilySymbol _FamSymRectRect_GenericModel;

    /// <summary>フカシファミリシンボル - 一般モデル_長方形 側面台形</summary>
    private Revit.DB.FamilySymbol _FamSymRectTorapezoid_GenericModel;

    /// <summary>フカシファミリシンボル - 一般モデル_長方形 側面三角形</summary>
    private Revit.DB.FamilySymbol _FamSymRectTriang_GenericModel;

    /// <summary>フカシファミリシンボル - 一般モデル_台形</summary>
    private Revit.DB.FamilySymbol _FamSymTorapezoid_GenericModel;

    /// <summary>フカシファミリシンボル - 一般モデル_平行四辺形</summary>
    private Revit.DB.FamilySymbol _FamSymParallelogram_GenericModel;

    /// <summary>フカシファミリシンボル - 一般モデル_L字形</summary>
    private Revit.DB.FamilySymbol _FamSymLshape_GenericModel;

    /// <summary>フカシファミリシンボル - 一般モデル_T字形</summary>
    private Revit.DB.FamilySymbol _FamSymTshape_GenericModel;

    /// <summary>フカシファミリシンボル - 構造柱_三角形</summary>
    private Revit.DB.FamilySymbol _FamSymTriangle_Column;

    /// <summary>フカシファミリシンボル - 構造柱_長方形 側面長方形</summary>
    private Revit.DB.FamilySymbol _FamSymRectRect_Column;

    /// <summary>フカシファミリシンボル - 構造柱_長方形 側面台形</summary>
    private Revit.DB.FamilySymbol _FamSymRectTorapezoid_Column;

    /// <summary>フカシファミリシンボル - 構造柱_長方形 側面三角形</summary>
    private Revit.DB.FamilySymbol _FamSymRectTriang_Column;

    /// <summary>フカシファミリシンボル - 構造柱_台形</summary>
    private Revit.DB.FamilySymbol _FamSymTorapezoid_Column;

    /// <summary>フカシファミリシンボル - 構造柱_平行四辺形</summary>
    private Revit.DB.FamilySymbol _FamSymParallelogram_Column;

    /// <summary>フカシファミリシンボル - 構造柱_L字形</summary>
    private Revit.DB.FamilySymbol _FamSymLshape_Column;

    /// <summary>フカシファミリシンボル - 構造柱_T字形</summary>
    private Revit.DB.FamilySymbol _FamSymTshape_Column;

    /// <summary>フカシファミリシンボル - 構造フレーム_三角形</summary>
    private Revit.DB.FamilySymbol _FamSymTriangle_Frame;

    /// <summary>フカシファミリシンボル - 構造フレーム_長方形 側面長方形</summary>
    private Revit.DB.FamilySymbol _FamSymRectRect_Frame;

    /// <summary>フカシファミリシンボル - 構造フレーム_長方形 側面台形</summary>
    private Revit.DB.FamilySymbol _FamSymRectTorapezoid_Frame;

    /// <summary>フカシファミリシンボル - 構造フレーム_長方形 側面三角形</summary>
    private Revit.DB.FamilySymbol _FamSymRectTriang_Frame;

    /// <summary>フカシファミリシンボル - 構造フレーム_台形</summary>
    private Revit.DB.FamilySymbol _FamSymTorapezoid_Frame;

    /// <summary>フカシファミリシンボル - 構造フレーム_平行四辺形</summary>
    private Revit.DB.FamilySymbol _FamSymParallelogram_Frame;

    /// <summary>フカシファミリシンボル - 構造フレーム_L字形</summary>
    private Revit.DB.FamilySymbol _FamSymLshape_Frame;

    /// <summary>フカシファミリシンボル - 構造フレーム_T字形</summary>
    private Revit.DB.FamilySymbol _FamSymTshape_Frame;

    /// <summary>フカシファミリシンボル - 構造基礎_三角形</summary>
    private Revit.DB.FamilySymbol _FamSymTriangle_Foundation;

    /// <summary>フカシファミリシンボル - 構造基礎_長方形 側面長方形</summary>
    private Revit.DB.FamilySymbol _FamSymRectRect_Foundation;

    /// <summary>フカシファミリシンボル - 構造基礎_長方形 側面台形</summary>
    private Revit.DB.FamilySymbol _FamSymRectTorapezoid_Foundation;

    /// <summary>フカシファミリシンボル - 構造基礎_長方形 側面三角形</summary>
    private Revit.DB.FamilySymbol _FamSymRectTriang_Foundation;

    /// <summary>フカシファミリシンボル - 構造基礎_台形</summary>
    private Revit.DB.FamilySymbol _FamSymTorapezoid_Foundation;

    /// <summary>フカシファミリシンボル - 構造基礎_平行四辺形</summary>
    private Revit.DB.FamilySymbol _FamSymParallelogram_Foundation;

    /// <summary>フカシファミリシンボル - 構造基礎_L字形</summary>
    private Revit.DB.FamilySymbol _FamSymLshape_Foundation;

    /// <summary>フカシファミリシンボル - 構造基礎_T字形</summary>
    private Revit.DB.FamilySymbol _FamSymTshape_Foundation;

    /// <summary>ファミリシンボルローカル</summary>
    private string famLoc;
    #endregion

    // コンストラクタ
        #region Constructor
        /// ================================================================================
        /// <summary>要素</summary>
        ///
        /// <param name="rvtUiDoc"    >UIドキュメント</param>
        ///<param name="cmpAttribute" >属性</param>
        /// 
        /// <history>2016/11/17 Created  CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
    public Elements(Revit.UI.UIDocument rvtUiDoc, RvtExtApp.Face.Components.Attribute cmpAttribute)
    {
      _rvtUiDoc = rvtUiDoc;
      _CmpAttribute = cmpAttribute;
    }

    /// <summary>UIドキュメント</summary>
    public Revit.UI.UIDocument RvtUIDoc => _rvtUiDoc;

    /// <summary>DBドキュメント</summary>
    public Revit.DB.Document RvtDBDoc => _rvtUiDoc.Document;

    /// <summary>許容値</summary>
    public double Approx0Len => 1.0e-9;

    /// <summary>プロジェクト情報要素</summary>
    public Revit.DB.ProjectInfo ProjectInfo => _rvtUiDoc.Document.ProjectInformation;

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

        // 一般モデル
        #region 一般モデル

        // 三角形
        public Revit.DB.FamilySymbol GetFamSymTriangle_GenericModel()
        {
            if (_FamSymTriangle_GenericModel != null)
                return _FamSymTriangle_GenericModel;

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
            return _FamSymTriangle_GenericModel;
        }


        // 長方形 側面長方形
        public Revit.DB.FamilySymbol GetFamSymRectRect_GenericModel()
        {
            if (_FamSymRectRect_GenericModel != null)
                return _FamSymRectRect_GenericModel;

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
            return _FamSymRectRect_GenericModel;
        }


        // 長方形 側面台形
        public Revit.DB.FamilySymbol GetFamSymRectTorapezoid_GenericModel()
        {
            if (_FamSymRectTorapezoid_GenericModel != null)
                return _FamSymRectTorapezoid_GenericModel;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID"))
                    {
                        _FamSymRectTorapezoid_GenericModel = famSym;

                        if (_FamSymRectTorapezoid_GenericModel.IsActive == false)
                        {
                            _FamSymRectTorapezoid_GenericModel.Activate();
                        }
                    }
                }
            }
            return _FamSymRectTorapezoid_GenericModel;
        }


        // 長方形 側面三角形
        public Revit.DB.FamilySymbol GetFamSymRectTriang_GenericModel()
        {
            if (_FamSymRectTriang_GenericModel != null)
                return _FamSymRectTriang_GenericModel;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE"))
                    {
                        _FamSymRectTriang_GenericModel = famSym;

                        if (_FamSymRectTriang_GenericModel.IsActive == false)
                        {
                            _FamSymRectTriang_GenericModel.Activate();
                        }
                    }
                }
            }
            return _FamSymRectTriang_GenericModel;
        }


        // 台形
        public Revit.DB.FamilySymbol GetFamSymTorapezoid_GenericModel()
        {
            if (_FamSymTorapezoid_GenericModel != null)
                return _FamSymTorapezoid_GenericModel;

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
            return _FamSymTorapezoid_GenericModel;
        }


        // 平行四辺形
        public Revit.DB.FamilySymbol GetFamSymParallelogram_GenericModel()
        {
            if (_FamSymParallelogram_GenericModel != null)
                return _FamSymParallelogram_GenericModel;

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
            return _FamSymParallelogram_GenericModel;
        }


        // L字形
        public Revit.DB.FamilySymbol GetFamSymLshape_GenericModel()
        {
            if (_FamSymLshape_GenericModel != null)
                return _FamSymLshape_GenericModel;

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
            return _FamSymLshape_GenericModel;
        }


        // T字形
        public Revit.DB.FamilySymbol GetFamSymTshape_GenericModel()
        {
            if (_FamSymTshape_GenericModel != null)
                return _FamSymTshape_GenericModel;

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
                        _FamSymTshape_GenericModel = famSym;

                        if (_FamSymTshape_GenericModel.IsActive == false)
                        {
                            _FamSymTshape_GenericModel.Activate();
                        }
                    }
                }
            }
            return _FamSymTshape_GenericModel;
        }


        #endregion

        // 構造柱
        #region 構造柱

        // 三角形
        public Revit.DB.FamilySymbol GetFamSymTriangle_Column()
        {
            if (_FamSymTriangle_Column != null)
                return _FamSymTriangle_Column;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_COLUMN") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_COLUMN"))
                    {
                        _FamSymTriangle_Column = famSym;

                        if (_FamSymTriangle_Column.IsActive == false)
                        {
                            _FamSymTriangle_Column.Activate();
                        }
                    }
                }
            }
            return _FamSymTriangle_Column;
        }


        // 長方形 側面長方形
        public Revit.DB.FamilySymbol GetFamSymRectRect_Column()
        {
            if (_FamSymRectRect_Column != null)
                return _FamSymRectRect_Column;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_COLUMN") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_COLUMN"))
                    {
                        _FamSymRectRect_Column = famSym;

                        if (_FamSymRectRect_Column.IsActive == false)
                        {
                            _FamSymRectRect_Column.Activate();
                        }
                    }
                }
            }
            return _FamSymRectRect_Column;
        }


        // 長方形 側面台形
        public Revit.DB.FamilySymbol GetFamSymRectTorapezoid_Column()
        {
            if (_FamSymRectTorapezoid_Column != null)
                return _FamSymRectTorapezoid_Column;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_COLUMN") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_COLUMN"))
                    {
                        _FamSymRectTorapezoid_Column = famSym;

                        if (_FamSymRectTorapezoid_Column.IsActive == false)
                        {
                            _FamSymRectTorapezoid_Column.Activate();
                        }
                    }
                }
            }
            return _FamSymRectTorapezoid_Column;
        }


        // 長方形 側面三角形
        public Revit.DB.FamilySymbol GetFamSymRectTriang_Column()
        {
            if (_FamSymRectTriang_Column != null)
                return _FamSymRectTriang_Column;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_COLUMN") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_COLUMN"))
                    {
                        _FamSymRectTriang_Column = famSym;

                        if (_FamSymRectTriang_Column.IsActive == false)
                        {
                            _FamSymRectTriang_Column.Activate();
                        }
                    }
                }
            }
            return _FamSymRectTriang_Column;
        }


        // 台形
        public Revit.DB.FamilySymbol GetFamSymTorapezoid_Column()
        {
            if (_FamSymTorapezoid_Column != null)
                return _FamSymTorapezoid_Column;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_COLUMN") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_COLUMN"))
                    {
                        _FamSymTorapezoid_Column = famSym;

                        if (_FamSymTorapezoid_Column.IsActive == false)
                        {
                            _FamSymTorapezoid_Column.Activate();
                        }
                    }
                }
            }
            return _FamSymTorapezoid_Column;
        }


        // 平行四辺形
        public Revit.DB.FamilySymbol GetFamSymParallelogram_Column()
        {
            if (_FamSymParallelogram_Column != null)
                return _FamSymParallelogram_Column;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_COLUMN") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_COLUMN"))
                    {
                        _FamSymParallelogram_Column = famSym;

                        if (_FamSymParallelogram_Column.IsActive == false)
                        {
                            _FamSymParallelogram_Column.Activate();
                        }
                    }
                }
            }
            return _FamSymParallelogram_Column;
        }


        // L字形
        public Revit.DB.FamilySymbol GetFamSymLshape_Column()
        {
            if (_FamSymLshape_Column != null)
                return _FamSymLshape_Column;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_COLUMN") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_COLUMN"))
                    {
                        _FamSymLshape_Column = famSym;

                        if (_FamSymLshape_Column.IsActive == false)
                        {
                            _FamSymLshape_Column.Activate();
                        }
                    }
                }
            }
            return _FamSymLshape_Column;
        }


        // T字形
        public Revit.DB.FamilySymbol GetFamSymTshape_Column()
        {
            if (_FamSymTshape_Column != null)
                return _FamSymTshape_Column;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_COLUMN") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_COLUMN"))
                    {
                        _FamSymTshape_Column = famSym;

                        if (_FamSymTshape_Column.IsActive == false)
                        {
                            _FamSymTshape_Column.Activate();
                        }
                    }
                }
            }
            return _FamSymTshape_Column;
        }


        #endregion

        // 構造フレーム
        #region 構造フレーム

        // 三角形
        public Revit.DB.FamilySymbol GetFamSymTriangle_Frame()
        {
            if (_FamSymTriangle_Frame != null)
                return _FamSymTriangle_Frame;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_FRAME") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_FRAME"))
                    {
                        _FamSymTriangle_Frame = famSym;

                        if (_FamSymTriangle_Frame.IsActive == false)
                        {
                            _FamSymTriangle_Frame.Activate();
                        }
                    }
                }
            }
            return _FamSymTriangle_Frame;
        }


        // 長方形 側面長方形
        public Revit.DB.FamilySymbol GetFamSymRectRect_Frame()
        {
            if (_FamSymRectRect_Frame != null)
                return _FamSymRectRect_Frame;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_FRAME") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_FRAME"))
                    {
                        _FamSymRectRect_Frame = famSym;

                        if (_FamSymRectRect_Frame.IsActive == false)
                        {
                            _FamSymRectRect_Frame.Activate();
                        }
                    }
                }
            }
            return _FamSymRectRect_Frame;
        }


        // 長方形 側面台形
        public Revit.DB.FamilySymbol GetFamSymRectTorapezoid_Frame()
        {
            if (_FamSymRectTorapezoid_Frame != null)
                return _FamSymRectTorapezoid_Frame;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_FRAME") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_FRAME"))
                    {
                        _FamSymRectTorapezoid_Frame = famSym;

                        if (_FamSymRectTorapezoid_Frame.IsActive == false)
                        {
                            _FamSymRectTorapezoid_Frame.Activate();
                        }
                    }
                }
            }
            return _FamSymRectTorapezoid_Frame;
        }


        // 長方形 側面三角形
        public Revit.DB.FamilySymbol GetFamSymRectTriang_Frame()
        {
            if (_FamSymRectTriang_Frame != null)
                return _FamSymRectTriang_Frame;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_FRAME") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_FRAME"))
                    {
                        _FamSymRectTriang_Frame = famSym;

                        if (_FamSymRectTriang_Frame.IsActive == false)
                        {
                            _FamSymRectTriang_Frame.Activate();
                        }
                    }
                }
            }
            return _FamSymRectTriang_Frame;
        }


        // 台形
        public Revit.DB.FamilySymbol GetFamSymTorapezoid_Frame()
        {
            if (_FamSymTorapezoid_Frame != null)
                return _FamSymTorapezoid_Frame;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_FRAME") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_FRAME"))
                    {
                        _FamSymTorapezoid_Frame = famSym;

                        if (_FamSymTorapezoid_Frame.IsActive == false)
                        {
                            _FamSymTorapezoid_Frame.Activate();
                        }
                    }
                }
            }
            return _FamSymTorapezoid_Frame;
        }


        // 平行四辺形
        public Revit.DB.FamilySymbol GetFamSymParallelogram_Frame()
        {
            if (_FamSymParallelogram_Frame != null)
                return _FamSymParallelogram_Frame;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_FRAME") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_FRAME"))
                    {
                        _FamSymParallelogram_Frame = famSym;

                        if (_FamSymParallelogram_Frame.IsActive == false)
                        {
                            _FamSymParallelogram_Frame.Activate();
                        }
                    }
                }
            }
            return _FamSymParallelogram_Frame;
        }


        // L字形
        public Revit.DB.FamilySymbol GetFamSymLshape_Frame()
        {
            if (_FamSymLshape_Frame != null)
                return _FamSymLshape_Frame;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_FRAME") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_FRAME"))
                    {
                        _FamSymLshape_Frame = famSym;

                        if (_FamSymLshape_Frame.IsActive == false)
                        {
                            _FamSymLshape_Frame.Activate();
                        }
                    }
                }
            }
            return _FamSymLshape_Frame;
        }


        // T字形
        public Revit.DB.FamilySymbol GetFamSymTshape_Frame()
        {
            if (_FamSymTshape_Frame != null)
                return _FamSymTshape_Frame;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_FRAME") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_FRAME"))
                    {
                        _FamSymTshape_Frame = famSym;

                        if (_FamSymTshape_Frame.IsActive == false)
                        {
                            _FamSymTshape_Frame.Activate();
                        }
                    }
                }
            }
            return _FamSymTshape_Frame;
        }


        #endregion

        // 構造基礎
        #region 構造基礎

        // 三角形
        public Revit.DB.FamilySymbol GetFamSymTriangle_Foundation()
        {
            if (_FamSymTriangle_Foundation != null)
                return _FamSymTriangle_Foundation;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_FOUNDATION") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_FOUNDATION"))
                    {
                        _FamSymTriangle_Foundation = famSym;

                        if (_FamSymTriangle_Foundation.IsActive == false)
                        {
                            _FamSymTriangle_Foundation.Activate();
                        }
                    }
                }
            }
            return _FamSymTriangle_Foundation;
        }


        // 長方形 側面長方形
        public Revit.DB.FamilySymbol GetFamSymRectRect_Foundation()
        {
            if (_FamSymRectRect_Foundation != null)
                return _FamSymRectRect_Foundation;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_FOUNDATION") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_FOUNDATION"))
                    {
                        _FamSymRectRect_Foundation = famSym;

                        if (_FamSymRectRect_Foundation.IsActive == false)
                        {
                            _FamSymRectRect_Foundation.Activate();
                        }
                    }
                }
            }
            return _FamSymRectRect_Foundation;
        }


        // 長方形 側面台形
        public Revit.DB.FamilySymbol GetFamSymRectTorapezoid_Foundation()
        {
            if (_FamSymRectTorapezoid_Foundation != null)
                return _FamSymRectTorapezoid_Foundation;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_FOUNDATION") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_FOUNDATION"))
                    {
                        _FamSymRectTorapezoid_Foundation = famSym;

                        if (_FamSymRectTorapezoid_Foundation.IsActive == false)
                        {
                            _FamSymRectTorapezoid_Foundation.Activate();
                        }
                    }
                }
            }
            return _FamSymRectTorapezoid_Foundation;
        }


        // 長方形 側面三角形
        public Revit.DB.FamilySymbol GetFamSymRectTriang_Foundation()
        {
            if (_FamSymRectTriang_Foundation != null)
                return _FamSymRectTriang_Foundation;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_FOUNDATION") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_FOUNDATION"))
                    {
                        _FamSymRectTriang_Foundation = famSym;

                        if (_FamSymRectTriang_Foundation.IsActive == false)
                        {
                            _FamSymRectTriang_Foundation.Activate();
                        }
                    }
                }
            }
            return _FamSymRectTriang_Foundation;
        }


        // 台形
        public Revit.DB.FamilySymbol GetFamSymTorapezoid_Foundation()
        {
            if (_FamSymTorapezoid_Foundation != null)
                return _FamSymTorapezoid_Foundation;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_FOUNDATION") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_FOUNDATION"))
                    {
                        _FamSymTorapezoid_Foundation = famSym;

                        if (_FamSymTorapezoid_Foundation.IsActive == false)
                        {
                            _FamSymTorapezoid_Foundation.Activate();
                        }
                    }
                }
            }
            return _FamSymTorapezoid_Foundation;
        }


        // 平行四辺形
        public Revit.DB.FamilySymbol GetFamSymParallelogram_Foundation()
        {
            if (_FamSymParallelogram_Foundation != null)
                return _FamSymParallelogram_Foundation;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_FOUNDATION") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_FOUNDATION"))
                    {
                        _FamSymParallelogram_Foundation = famSym;

                        if (_FamSymParallelogram_Foundation.IsActive == false)
                        {
                            _FamSymParallelogram_Foundation.Activate();
                        }
                    }
                }
            }
            return _FamSymParallelogram_Foundation;
        }


        // L字形
        public Revit.DB.FamilySymbol GetFamSymLshape_Foundation()
        {
            if (_FamSymLshape_Foundation != null)
                return _FamSymLshape_Foundation;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_FOUNDATION") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_FOUNDATION"))
                    {
                        _FamSymLshape_Foundation = famSym;

                        if (_FamSymLshape_Foundation.IsActive == false)
                        {
                            _FamSymLshape_Foundation.Activate();
                        }
                    }
                }
            }
            return _FamSymLshape_Foundation;
        }


        // T字形
        public Revit.DB.FamilySymbol GetFamSymTshape_Foundation()
        {
            if (_FamSymTshape_Foundation != null)
                return _FamSymTshape_Foundation;

            string famName = _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_FOUNDATION") + ".rfa";

            if (System.IO.File.Exists(famLoc + famName))
            {
                Revit.DB.Family family = null;

                RvtDBDoc.LoadFamily(famLoc + famName, out family);

                foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

                    if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_FOUNDATION"))
                    {
                        _FamSymTshape_Foundation = famSym;

                        if (_FamSymTshape_Foundation.IsActive == false)
                        {
                            _FamSymTshape_Foundation.Activate();
                        }
                    }
                }
            }
            return _FamSymTshape_Foundation;
        }


        #endregion



        /// ================================================================================
        /// <summary>フカシファミリ取得</summary>
        ///
        /// <history><p>2016/12/06 Created  CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2016/12/15 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void GetFukashiFamily()
    {
      famLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
      #region ドキュメント内のファミリから取得

            Revit.DB.FilteredElementCollector filterElemColle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
      filterElemColle.OfClass(typeof(Revit.DB.Family));

      foreach (Revit.DB.Family family in filterElemColle)
      {
        // 一般モデル
        #region 一般モデル

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
        // 長方形 側面長方形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE"))
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
        // 長方形 側面台形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID"))
            {
              _FamSymRectTorapezoid_GenericModel = famSym;

              if (_FamSymRectTorapezoid_GenericModel.IsActive == false)
              {
                _FamSymRectTorapezoid_GenericModel.Activate();
              }
            }
          }
        }
        // 長方形 側面三角形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE"))
            {
              _FamSymRectTriang_GenericModel = famSym;

              if (_FamSymRectTriang_GenericModel.IsActive == false)
              {
                _FamSymRectTriang_GenericModel.Activate();
              }
            }
          }
        }
        // 台形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID"))
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
        // 平行四辺形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM"))
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
        // L字形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE"))
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
        // T字形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN"))
            {
              _FamSymTshape_GenericModel = famSym;

              if (_FamSymTshape_GenericModel.IsActive == false)
              {
                _FamSymTshape_GenericModel.Activate();
              }
            }
          }
        }

        #endregion

        // 構造柱
        #region 構造柱

        // 三角形
        if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_COLUMN"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_COLUMN"))
            {
              _FamSymTriangle_Column = famSym;

              if (_FamSymTriangle_Column.IsActive == false)
              {
                _FamSymTriangle_Column.Activate();
              }
            }
          }
        }
        // 長方形 側面長方形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_COLUMN"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_COLUMN"))
            {
              _FamSymRectRect_Column = famSym;

              if (_FamSymRectRect_Column.IsActive == false)
              {
                _FamSymRectRect_Column.Activate();
              }
            }
          }
        }
        // 長方形 側面台形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_COLUMN"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_COLUMN"))
            {
              _FamSymRectTorapezoid_Column = famSym;

              if (_FamSymRectTorapezoid_Column.IsActive == false)
              {
                _FamSymRectTorapezoid_Column.Activate();
              }
            }
          }
        }
        // 長方形 側面三角形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_COLUMN"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_COLUMN"))
            {
              _FamSymRectTriang_Column = famSym;

              if (_FamSymRectTriang_Column.IsActive == false)
              {
                _FamSymRectTriang_Column.Activate();
              }
            }
          }
        }
        // 台形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_COLUMN"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_COLUMN"))
            {
              _FamSymTorapezoid_Column = famSym;

              if (_FamSymTorapezoid_Column.IsActive == false)
              {
                _FamSymTorapezoid_Column.Activate();
              }
            }
          }
        }
        // 平行四辺形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_COLUMN"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_COLUMN"))
            {
              _FamSymParallelogram_Column = famSym;

              if (_FamSymParallelogram_Column.IsActive == false)
              {
                _FamSymParallelogram_Column.Activate();
              }
            }
          }
        }
        // L字形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_COLUMN"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_COLUMN"))
            {
              _FamSymLshape_Column = famSym;

              if (_FamSymLshape_Column.IsActive == false)
              {
                _FamSymLshape_Column.Activate();
              }
            }
          }
        }
        // T字形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_COLUMN"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_COLUMN"))
            {
              _FamSymTshape_Column = famSym;

              if (_FamSymTshape_Column.IsActive == false)
              {
                _FamSymTshape_Column.Activate();
              }
            }
          }
        }

        #endregion

        // 構造フレーム
        #region 構造フレーム

        // 三角形
        if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_FRAME"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_FRAME"))
            {
              _FamSymTriangle_Frame = famSym;

              if (_FamSymTriangle_Frame.IsActive == false)
              {
                _FamSymTriangle_Frame.Activate();
              }
            }
          }
        }
        // 長方形 側面長方形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_FRAME"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_FRAME"))
            {
              _FamSymRectRect_Frame = famSym;

              if (_FamSymRectRect_Frame.IsActive == false)
              {
                _FamSymRectRect_Frame.Activate();
              }
            }
          }
        }
        // 長方形 側面台形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_FRAME"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_FRAME"))
            {
              _FamSymRectTorapezoid_Frame = famSym;

              if (_FamSymRectTorapezoid_Frame.IsActive == false)
              {
                _FamSymRectTorapezoid_Frame.Activate();
              }
            }
          }
        }
        // 長方形 側面三角形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_FRAME"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_FRAME"))
            {
              _FamSymRectTriang_Frame = famSym;

              if (_FamSymRectTriang_Frame.IsActive == false)
              {
                _FamSymRectTriang_Frame.Activate();
              }
            }
          }
        }
        // 台形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_FRAME"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_FRAME"))
            {
              _FamSymTorapezoid_Frame = famSym;

              if (_FamSymTorapezoid_Frame.IsActive == false)
              {
                _FamSymTorapezoid_Frame.Activate();
              }
            }
          }
        }
        // 平行四辺形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_FRAME"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_FRAME"))
            {
              _FamSymParallelogram_Frame = famSym;

              if (_FamSymParallelogram_Frame.IsActive == false)
              {
                _FamSymParallelogram_Frame.Activate();
              }
            }
          }
        }
        // L字形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_FRAME"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_FRAME"))
            {
              _FamSymLshape_Frame = famSym;

              if (_FamSymLshape_Frame.IsActive == false)
              {
                _FamSymLshape_Frame.Activate();
              }
            }
          }
        }
        // T字形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_FRAME"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_FRAME"))
            {
              _FamSymTshape_Frame = famSym;

              if (_FamSymTshape_Frame.IsActive == false)
              {
                _FamSymTshape_Frame.Activate();
              }
            }
          }
        }

        #endregion

        // 構造基礎
        #region 構造基礎

        // 三角形
        if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_FOUNDATION"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_FOUNDATION"))
            {
              _FamSymTriangle_Foundation = famSym;

              if (_FamSymTriangle_Foundation.IsActive == false)
              {
                _FamSymTriangle_Foundation.Activate();
              }
            }
          }
        }
        // 長方形 側面長方形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_FOUNDATION"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_FOUNDATION"))
            {
              _FamSymRectRect_Foundation = famSym;

              if (_FamSymRectRect_Foundation.IsActive == false)
              {
                _FamSymRectRect_Foundation.Activate();
              }
            }
          }
        }
        // 長方形 側面台形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_FOUNDATION"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_FOUNDATION"))
            {
              _FamSymRectTorapezoid_Foundation = famSym;

              if (_FamSymRectTorapezoid_Foundation.IsActive == false)
              {
                _FamSymRectTorapezoid_Foundation.Activate();
              }
            }
          }
        }
        // 長方形 側面三角形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_FOUNDATION"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_FOUNDATION"))
            {
              _FamSymRectTriang_Foundation = famSym;

              if (_FamSymRectTriang_Foundation.IsActive == false)
              {
                _FamSymRectTriang_Foundation.Activate();
              }
            }
          }
        }
        // 台形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_FOUNDATION"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_FOUNDATION"))
            {
              _FamSymTorapezoid_Foundation = famSym;

              if (_FamSymTorapezoid_Foundation.IsActive == false)
              {
                _FamSymTorapezoid_Foundation.Activate();
              }
            }
          }
        }
        // 平行四辺形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_FOUNDATION"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_FOUNDATION"))
            {
              _FamSymParallelogram_Foundation = famSym;

              if (_FamSymParallelogram_Foundation.IsActive == false)
              {
                _FamSymParallelogram_Foundation.Activate();
              }
            }
          }
        }
        // L字形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_FOUNDATION"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_FOUNDATION"))
            {
              _FamSymLshape_Foundation = famSym;

              if (_FamSymLshape_Foundation.IsActive == false)
              {
                _FamSymLshape_Foundation.Activate();
              }
            }
          }
        }
        // T字形
        else if (family.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_FOUNDATION"))
        {
          foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
          {
            Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

            if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_FOUNDATION"))
            {
              _FamSymTshape_Foundation = famSym;

              if (_FamSymTshape_Foundation.IsActive == false)
              {
                _FamSymTshape_Foundation.Activate();
              }
            }
          }
        }

        #endregion
      }

      #endregion

      #region ロードして取得

      //// フォルダ
      //string famLoc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

      //int loadCnt = 0;
      //DnfCom.ProgressBarThread thread = new DnfCom.ProgressBarThread(false, false);
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.ShowDialog();

      //// 一般モデル
      //#region 一般モデル

      //// 三角形
      //if (_FamSymTriangle_GenericModel == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE"))
      //      {
      //        _FamSymTriangle_GenericModel = famSym;

      //        if (_FamSymTriangle_GenericModel.IsActive == false)
      //        {
      //          _FamSymTriangle_GenericModel.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 長方形 側面長方形
      //if (_FamSymRectRect_GenericModel == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE"))
      //      {
      //        _FamSymRectRect_GenericModel = famSym;

      //        if (_FamSymRectRect_GenericModel.IsActive == false)
      //        {
      //          _FamSymRectRect_GenericModel.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 長方形 側面台形
      //if (_FamSymRectTorapezoid_GenericModel == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID"))
      //      {
      //        _FamSymRectTorapezoid_GenericModel = famSym;

      //        if (_FamSymRectTorapezoid_GenericModel.IsActive == false)
      //        {
      //          _FamSymRectTorapezoid_GenericModel.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 長方形 側面三角形
      //if (_FamSymRectTriang_GenericModel == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE"))
      //      {
      //        _FamSymRectTriang_GenericModel = famSym;

      //        if (_FamSymRectTriang_GenericModel.IsActive == false)
      //        {
      //          _FamSymRectTriang_GenericModel.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 台形
      //if (_FamSymTorapezoid_GenericModel == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID"))
      //      {
      //        _FamSymTorapezoid_GenericModel = famSym;

      //        if (_FamSymTorapezoid_GenericModel.IsActive == false)
      //        {
      //          _FamSymTorapezoid_GenericModel.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 平行四辺形
      //if (_FamSymParallelogram_GenericModel == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM"))
      //      {
      //        _FamSymParallelogram_GenericModel = famSym;

      //        if (_FamSymParallelogram_GenericModel.IsActive == false)
      //        {
      //          _FamSymParallelogram_GenericModel.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// L字形
      //if (_FamSymLshape_GenericModel == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_LSHAPE") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE"))
      //      {
      //        _FamSymLshape_GenericModel = famSym;

      //        if (_FamSymLshape_GenericModel.IsActive == false)
      //        {
      //          _FamSymLshape_GenericModel.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// T字形
      //if (_FamSymTshape_GenericModel == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_UNEVEN") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN"))
      //      {
      //        _FamSymTshape_GenericModel = famSym;

      //        if (_FamSymTshape_GenericModel.IsActive == false)
      //        {
      //          _FamSymTshape_GenericModel.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //#endregion

      //// 構造柱
      //#region 構造柱

      //// 三角形
      //if (_FamSymTriangle_Column == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_COLUMN") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_COLUMN"))
      //      {
      //        _FamSymTriangle_Column = famSym;

      //        if (_FamSymTriangle_Column.IsActive == false)
      //        {
      //          _FamSymTriangle_Column.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 長方形 側面長方形
      //if (_FamSymRectRect_Column == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_COLUMN") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_COLUMN"))
      //      {
      //        _FamSymRectRect_Column = famSym;

      //        if (_FamSymRectRect_Column.IsActive == false)
      //        {
      //          _FamSymRectRect_Column.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 長方形 側面台形
      //if (_FamSymRectTorapezoid_Column == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_COLUMN") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_COLUMN"))
      //      {
      //        _FamSymRectTorapezoid_Column = famSym;

      //        if (_FamSymRectTorapezoid_Column.IsActive == false)
      //        {
      //          _FamSymRectTorapezoid_Column.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 長方形 側面三角形
      //if (_FamSymRectTriang_Column == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_COLUMN") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_COLUMN"))
      //      {
      //        _FamSymRectTriang_Column = famSym;

      //        if (_FamSymRectTriang_Column.IsActive == false)
      //        {
      //          _FamSymRectTriang_Column.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 台形
      //if (_FamSymTorapezoid_Column == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_COLUMN") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_COLUMN"))
      //      {
      //        _FamSymTorapezoid_Column = famSym;

      //        if (_FamSymTorapezoid_Column.IsActive == false)
      //        {
      //          _FamSymTorapezoid_Column.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 平行四辺形
      //if (_FamSymParallelogram_Column == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_COLUMN") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_COLUMN"))
      //      {
      //        _FamSymParallelogram_Column = famSym;

      //        if (_FamSymParallelogram_Column.IsActive == false)
      //        {
      //          _FamSymParallelogram_Column.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// L字形
      //if (_FamSymLshape_Column == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_COLUMN") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_COLUMN"))
      //      {
      //        _FamSymLshape_Column = famSym;

      //        if (_FamSymLshape_Column.IsActive == false)
      //        {
      //          _FamSymLshape_Column.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// T字形
      //if (_FamSymTshape_Column == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_COLUMN") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_COLUMN"))
      //      {
      //        _FamSymTshape_Column = famSym;

      //        if (_FamSymTshape_Column.IsActive == false)
      //        {
      //          _FamSymTshape_Column.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //#endregion

      //// 構造フレーム
      //#region 構造フレーム

      //// 三角形
      //if (_FamSymTriangle_Frame == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_FRAME") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_FRAME"))
      //      {
      //        _FamSymTriangle_Frame = famSym;

      //        if (_FamSymTriangle_Frame.IsActive == false)
      //        {
      //          _FamSymTriangle_Frame.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 長方形 側面長方形
      //if (_FamSymRectRect_Frame == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_FRAME") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_FRAME"))
      //      {
      //        _FamSymRectRect_Frame = famSym;

      //        if (_FamSymRectRect_Frame.IsActive == false)
      //        {
      //          _FamSymRectRect_Frame.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 長方形 側面台形
      //if (_FamSymRectTorapezoid_Frame == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_FRAME") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_FRAME"))
      //      {
      //        _FamSymRectTorapezoid_Frame = famSym;

      //        if (_FamSymRectTorapezoid_Frame.IsActive == false)
      //        {
      //          _FamSymRectTorapezoid_Frame.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 長方形 側面三角形
      //if (_FamSymRectTriang_Frame == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_FRAME") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_FRAME"))
      //      {
      //        _FamSymRectTriang_Frame = famSym;

      //        if (_FamSymRectTriang_Frame.IsActive == false)
      //        {
      //          _FamSymRectTriang_Frame.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 台形
      //if (_FamSymTorapezoid_Frame == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_FRAME") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_FRAME"))
      //      {
      //        _FamSymTorapezoid_Frame = famSym;

      //        if (_FamSymTorapezoid_Frame.IsActive == false)
      //        {
      //          _FamSymTorapezoid_Frame.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 平行四辺形
      //if (_FamSymParallelogram_Frame == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_FRAME") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_FRAME"))
      //      {
      //        _FamSymParallelogram_Frame = famSym;

      //        if (_FamSymParallelogram_Frame.IsActive == false)
      //        {
      //          _FamSymParallelogram_Frame.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// L字形
      //if (_FamSymLshape_Frame == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_FRAME") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_FRAME"))
      //      {
      //        _FamSymLshape_Frame = famSym;

      //        if (_FamSymLshape_Frame.IsActive == false)
      //        {
      //          _FamSymLshape_Frame.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// T字形
      //if (_FamSymTshape_Frame == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_FRAME") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_FRAME"))
      //      {
      //        _FamSymTshape_Frame = famSym;

      //        if (_FamSymTshape_Frame.IsActive == false)
      //        {
      //          _FamSymTshape_Frame.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //#endregion

      //// 構造基礎
      //#region 構造基礎

      //// 三角形
      //if (_FamSymTriangle_Foundation == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_FOUNDATION") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TRIANGLE_FOUNDATION"))
      //      {
      //        _FamSymTriangle_Foundation = famSym;

      //        if (_FamSymTriangle_Foundation.IsActive == false)
      //        {
      //          _FamSymTriangle_Foundation.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 長方形 側面長方形
      //if (_FamSymRectRect_Foundation == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_FOUNDATION") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_RECTANGLE_FOUNDATION"))
      //      {
      //        _FamSymRectRect_Foundation = famSym;

      //        if (_FamSymRectRect_Foundation.IsActive == false)
      //        {
      //          _FamSymRectRect_Foundation.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 長方形 側面台形
      //if (_FamSymRectTorapezoid_Foundation == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_FOUNDATION") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TORAPEZOID_FOUNDATION"))
      //      {
      //        _FamSymRectTorapezoid_Foundation = famSym;

      //        if (_FamSymRectTorapezoid_Foundation.IsActive == false)
      //        {
      //          _FamSymRectTorapezoid_Foundation.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 長方形 側面三角形
      //if (_FamSymRectTriang_Foundation == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_FOUNDATION") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_RECT_TRIANGLE_FOUNDATION"))
      //      {
      //        _FamSymRectTriang_Foundation = famSym;

      //        if (_FamSymRectTriang_Foundation.IsActive == false)
      //        {
      //          _FamSymRectTriang_Foundation.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 台形
      //if (_FamSymTorapezoid_Foundation == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_FOUNDATION") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_TORAPEZOID_FOUNDATION"))
      //      {
      //        _FamSymTorapezoid_Foundation = famSym;

      //        if (_FamSymTorapezoid_Foundation.IsActive == false)
      //        {
      //          _FamSymTorapezoid_Foundation.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// 平行四辺形
      //if (_FamSymParallelogram_Foundation == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_FOUNDATION") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_PARALLELOGRAM_FOUNDATION"))
      //      {
      //        _FamSymParallelogram_Foundation = famSym;

      //        if (_FamSymParallelogram_Foundation.IsActive == false)
      //        {
      //          _FamSymParallelogram_Foundation.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// L字形
      //if (_FamSymLshape_Foundation == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_FOUNDATION") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_LSHAPE_FOUNDATION"))
      //      {
      //        _FamSymLshape_Foundation = famSym;

      //        if (_FamSymLshape_Foundation.IsActive == false)
      //        {
      //          _FamSymLshape_Foundation.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //// T字形
      //if (_FamSymTshape_Foundation == null)
      //{
      //  string famName = _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_FOUNDATION") + ".rfa";

      //  if (System.IO.File.Exists(famLoc + famName))
      //  {
      //    Revit.DB.Family family = null;

      //    RvtDBDoc.LoadFamily(famLoc + famName, out family);

      //    foreach (Revit.DB.ElementId famSymId in family.GetFamilySymbolIds())
      //    {
      //      Revit.DB.FamilySymbol famSym = RvtDBDoc.GetElement(famSymId) as Revit.DB.FamilySymbol;

      //      if (famSym.Name == _CmpAttribute.ResourceText("IDS_FAM_UNEVEN_FOUNDATION"))
      //      {
      //        _FamSymTshape_Foundation = famSym;

      //        if (_FamSymTshape_Foundation.IsActive == false)
      //        {
      //          _FamSymTshape_Foundation.Activate();
      //        }
      //      }
      //    }
      //  }
      //}

      //loadCnt += 1;
      //thread.SetData(_CmpAttribute.ResourceText("IDS_TXT_FAMILYLOADING"), 32, loadCnt);
      //thread.Active();

      //#endregion

      //thread.Close();

      #endregion
    }

        /// ================================================================================
        /// <summary>対象カテゴリ化検査</summary>
        /// 
        /// <history>2017/01/19 Created  CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public bool CheckTargetCategory()
        {
            bool ret = false;
            int id = Int32.Parse(ElemCategory.Id.ToString());

            switch (id)
            {
                case (int)Revit.DB.BuiltInCategory.OST_StructuralColumns:       //構造柱
                    ret = true;
                    break;
                case (int)Revit.DB.BuiltInCategory.OST_StructuralFraming:       //構造フレーム
                    ret = true;
                    break;
                case (int)Revit.DB.BuiltInCategory.OST_StructuralFoundation:    //構造基礎
                    ret = true;
                    break;
                case (int)Revit.DB.BuiltInCategory.OST_Walls:                   //壁
                    ret = true;
                    break;
                case (int)Revit.DB.BuiltInCategory.OST_Floors:                  //床スラブ
                    ret = true;
                    break;
            }
            return ret;
    }


    #endregion

    // プロパティ
    #region Properties

    /// ================================================================================
    /// <summary>要素カテゴリ</summary>
    ///
    /// <history>2016/12/14 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Category ElemCategory
    {
      get
      {
        return _ElemCategory;
      }
      set
      {
        _ElemCategory = value;
      }
    }

    /// ================================================================================
    /// <summary>マテリアルID</summary>
    ///
    /// <history>2016/12/14 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.ElementId MaterialId
    {
      get
      {
        return _MaterialId;
      }
      set
      {
        _MaterialId = value;
      }
    }

    /// ================================================================================
    /// <summary>グラフィックススタイルID</summary>
    ///
    /// <history>2016/12/14 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.ElementId GraphicsStyleId
    {
      get
      {
        return _GraphicsStyleId;
      }
      set
      {
        _GraphicsStyleId = value;
      }
    }

    /// ================================================================================
    /// <summary>フカシファミリシンボル - 三角形</summary>
    ///
    /// <history>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.FamilySymbol FamSymTriangle
    {
      get
      {
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralColumns).ToString()))
        {
          return GetFamSymTriangle_Column();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFraming).ToString()))
        {
          return GetFamSymTriangle_Frame();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFoundation).ToString()))
        {
          return GetFamSymTriangle_Foundation();
        }

        return GetFamSymTriangle_GenericModel();
      }
    }

    /// ================================================================================
    /// <summary>フカシファミリシンボル - 長方形 側面長方形</summary>
    ///
    /// <history>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.FamilySymbol FamSymRectRect
    {
      get
      {
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralColumns).ToString()))
        {
          return GetFamSymRectRect_Column();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFraming).ToString()))
        {
          return GetFamSymRectRect_Frame();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFoundation).ToString()))
        {
          return GetFamSymRectRect_Foundation();
        }

        return GetFamSymRectRect_GenericModel();
      }
    }

    /// ================================================================================
    /// <summary>フカシファミリシンボル - 長方形 側面台形</summary>
    ///
    /// <history>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.FamilySymbol FamSymRectTorapezoid
    {
      get
      {
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralColumns).ToString()))
        {
          return GetFamSymRectTorapezoid_Column();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFraming).ToString()))
        {
          return GetFamSymRectTorapezoid_Frame();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFoundation).ToString()))
        {
          return GetFamSymRectTorapezoid_Foundation();
        }

        return GetFamSymRectTorapezoid_GenericModel();
      }
    }

    /// ================================================================================
    /// <summary>フカシファミリシンボル - 長方形 側面三角形</summary>
    ///
    /// <history>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.FamilySymbol FamSymRectTriang
    {
      get
      {
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralColumns).ToString()))
        {
          return GetFamSymRectTriang_Column();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFraming).ToString()))
        {
          return GetFamSymRectTriang_Frame();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFoundation).ToString()))
        {
          return GetFamSymRectTriang_Foundation();
        }

        return GetFamSymRectTriang_GenericModel();
      }
    }

    /// ================================================================================
    /// <summary>フカシファミリシンボル - 台形</summary>
    ///
    /// <history>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.FamilySymbol FamSymTorapezoid
    {
      get
      {
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralColumns).ToString()))
        {
          return GetFamSymTorapezoid_Column();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFraming).ToString()))
        {
          return GetFamSymTorapezoid_Frame();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFoundation).ToString()))
        {
          return GetFamSymTorapezoid_Foundation();
        }

        return GetFamSymTorapezoid_GenericModel();
      }
    }

    /// ================================================================================
    /// <summary>フカシファミリシンボル - 平行四辺形</summary>
    ///
    /// <history>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.FamilySymbol FamSymParallelogram
    {
      get
      {
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralColumns).ToString()))
        {
          return GetFamSymParallelogram_Column();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFraming).ToString()))
        {
          return GetFamSymParallelogram_Frame();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFoundation).ToString()))
        {
          return GetFamSymParallelogram_Foundation();
        }

        return GetFamSymParallelogram_GenericModel();
      }
    }

    /// ================================================================================
    /// <summary>フカシファミリシンボル - L字形</summary>
    ///
    /// <history>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.FamilySymbol FamSymLshape
    {
      get
      {
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralColumns).ToString()))
        {
          return GetFamSymLshape_Column();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFraming).ToString()))
        {
          return GetFamSymLshape_Frame();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFoundation).ToString()))
        {
          return GetFamSymLshape_Foundation();
        }

        return GetFamSymLshape_GenericModel();
      }
    }

    /// ================================================================================
    /// <summary>フカシファミリシンボル - T字形</summary>
    ///
    /// <history>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.FamilySymbol FamSymTshape
    {
      get
      {
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralColumns).ToString()))
        {
          return GetFamSymTshape_Column();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFraming).ToString()))
        {
          return GetFamSymTshape_Frame();
        }
        if (ElemCategory.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_StructuralFoundation).ToString()))
        {
          return GetFamSymTshape_Foundation();
        }

        return GetFamSymTshape_GenericModel();
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

}
