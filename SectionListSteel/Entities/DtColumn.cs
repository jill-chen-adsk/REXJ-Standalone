using System;
using System.Linq;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
namespace SectionListSteel.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - 柱</summary>
    /// ================================================================================
    public class DtColumn : SectionListSteel.Entities.DtBase
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private SectionListSteel.Components.Attribute _CmpAttribute;

        /// <summary>要素</summary>
        private SectionListSteel.Components.Elements _CmpElements;

        /// <summary>図形</summary>
        private SectionListSteel.Components.Geometry _CmpGeometry;

        /// <summary>パラメータ</summary>
        private SectionListSteel.Components.Parameters _CmpParameters;

        /// <summary>設定</summary>
        private SectionListSteel.Components.Settings _CmpSettings;

        /// <summary>データテーブル</summary>
        private System.Data.DataTable _Data;

        #endregion Member Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"></param>
        /// <param name="cmpElements"></param>
        /// <param name="cmpGeometry"></param>
        /// <param name="cmpParameters"></param>
        /// <param name="cmpSettings"></param>
        ///
        /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public DtColumn(SectionListSteel.Components.Attribute cmpAttribute,
                             SectionListSteel.Components.Elements cmpElements,
                             SectionListSteel.Components.Geometry cmpGeometry,
                             SectionListSteel.Components.Parameters cmpParameters,
                             SectionListSteel.Components.Settings cmpSettings)
          : base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;

            _Data = null;
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>データ書式定義</summary>
        ///
        /// <param name="data">データテーブル</param>
        ///
        /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        private
        void DefDataFormat(ref System.Data.DataTable data)
        {
            // 柱の種類
            // 1 = 鉄骨 H形鋼、2 = 鉄骨 角形鋼管、3 = 鉄骨 鋼管、4 = CFT 角形鋼管、5 = CFT 鋼管
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TYPE"), typeof(int));

            // 符号
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_FUGO"), typeof(string));

            // 階
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_KAI"), typeof(string));

            // 鉄骨 H形鋼
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELH_WEBMATERIAL"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELH_FLANGEMATERIAL"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELH_SYUBETSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELH_HABA"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELH_SEI"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELH_WEBATSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELH_FLANGEATSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELH_FILLET"), typeof(string));

            // 鉄骨 角形鋼管
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELRECT_STRUCTURALMATERIAL"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELRECT_SYUBETSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELRECT_HABA"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELRECT_SEI"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELRECT_ITAATSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELRECT_FILLET"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_COLUMN_T2"), typeof(string));

            // 鉄骨 鋼管
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELROUND_STRUCTURALMATERIAL"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELROUND_SYUBETSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELROUND_TYOKKEI"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_STEELROUND_ITAATSU"), typeof(string));

            // CFT 角形鋼管
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CFTRECT_STRUCTURALMATERIAL"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CFTRECT_CONCRETEMATERIAL"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CFTRECT_SYUBETSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CFTRECT_HABA"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CFTRECT_SEI"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CFTRECT_ITAATSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CFTRECT_FILLET"), typeof(string));

            // CFT 鋼管
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CFTROUND_STRUCTURALMATERIAL"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CFTROUND_CONCRETEMATERIAL"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CFTROUND_SYUBETSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CFTROUND_TYOKKEI"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CFTROUND_ITAATSU"), typeof(string));

            //Mark
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_MARK_H"), typeof(string));

            //L 
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_HABA"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_SEI"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_DIRTHICK"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_WTHICK"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_MATERIAL"), typeof(string));

            //U
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_HABA"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_SEI"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_WEBATSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_FLANGEATSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_MATERIAL"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_SYUBETSU"), typeof(string));

            //C
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_HABA"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_SEI"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_LIPLENGTH"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_BOARDTHICK"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_MATERIAL"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_SYUBETSU"), typeof(string));
            
            //FB
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_WIDTH"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_MATERIAL"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_SYUBETSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_BOARDTHICK"), typeof(string));

            //M
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_MCOLUMN_MATERIAL"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_MCOLUMN_SYUBETSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_MCOLUMN_DIAMETER"), typeof(string));

            //T
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_HABA"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_SEI"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_WEBMAT"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_FLANGEMAT"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_WEBATSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_FLANGEATSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_SYUBETSU"), typeof(string));


        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="famSymColumn">柱ファミリ</param>
        /// <param name="columnType"><p>柱種類</p>
        ///                           <p>1 = 鉄骨 H形鋼、2 = 鉄骨 角形鋼管、3 = 鉄骨 鋼管、4 = CFT 角形鋼管、5 = CFT 鋼管</p></param>
        ///
        /// <history><p>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2016/09/27 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        System.Data.DataRow GetData(Revit.DB.FamilySymbol famSymColumn,
                                    int columnType)
        {
            // 初期化
            System.Data.DataRow ret = null;

            if (_Data == null)
            {
                _Data = new System.Data.DataTable();
                DefDataFormat(ref _Data);
            }

            try
            {
                //Check mark
                var mark = GetMarkValue(famSymColumn);
                if (mark == string.Empty)
                    return null;

                ret = _Data.NewRow();

                //Mark-H
                ret[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] = mark;

                // 名前
                string name = famSymColumn.Name;

                // 柱種類
                ret[_CmpAttribute.ResourceText("IDS_CN_TYPE")] = columnType;

                // 鉄骨 H形鋼
                if (columnType == 1)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymColumn.LookupParameter(_CmpParameters.SColumnHFugo);

                    string fugo = parFugo.AsString();
                    ret[_CmpAttribute.ResourceText("IDS_CN_FUGO")] = fugo;

                    // 階
                    if (fugo == "")
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }
                    else if (name.Contains(fugo))
                    {
                        string kai = name.Substring(0, name.LastIndexOf(fugo));
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = kai;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }

                    // ウェブマテリアル
                    Revit.DB.Parameter parWebMat = famSymColumn.LookupParameter(_CmpParameters.SColumnHWebMaterial);
                    Revit.DB.ElementId webMatId = parWebMat.AsElementId();
                    Revit.DB.Element webMat = _CmpElements.RvtDBDoc.GetElement(webMatId);
                    if (webMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_STEELH_WEBMATERIAL")] = webMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_STEELH_WEBMATERIAL")] = "";
                    }

                    // フランジマテリアル
                    Revit.DB.Parameter parFlangeMat = famSymColumn.LookupParameter(_CmpParameters.SColumnHFlangeMaterial);
                    Revit.DB.ElementId flangeMatId = parFlangeMat.AsElementId();
                    Revit.DB.Element flangeMat = _CmpElements.RvtDBDoc.GetElement(flangeMatId);
                    if (flangeMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_STEELH_FLANGEMATERIAL")] = flangeMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_STEELH_FLANGEMATERIAL")] = "";
                    }

                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSymColumn.LookupParameter(_CmpParameters.SColumnHSyubetsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_STEELH_SYUBETSU")] = parSyubetsu.AsString();

                    // 柱幅
                    Revit.DB.Parameter parHaba = famSymColumn.LookupParameter(_CmpParameters.SColumnHHaba);
                    ret[_CmpAttribute.ResourceText("IDS_CN_STEELH_HABA")] = parHaba.AsValueString();

                    // 柱せい
                    Revit.DB.Parameter parSei = famSymColumn.LookupParameter(_CmpParameters.SColumnHSei);
                    ret[_CmpAttribute.ResourceText("IDS_CN_STEELH_SEI")] = parSei.AsValueString();

                    // ウェブ厚
                    Revit.DB.Parameter parWebAtsu = famSymColumn.LookupParameter(_CmpParameters.SColumnHWebAtsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_STEELH_WEBATSU")] = parWebAtsu.AsValueString();

                    // フランジ厚
                    Revit.DB.Parameter parFlangeAtsu = famSymColumn.LookupParameter(_CmpParameters.SColumnHFlangeAtsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_STEELH_FLANGEATSU")] = parFlangeAtsu.AsValueString();

                    //// フィレット
                    //Revit.DB.Parameter parFillet = famSymColumn.LookupParameter(_CmpParameters.SColumnHFillet);
                    //ret[_CmpAttribute.ResourceText("IDS_CN_STEELH_FILLET")] = parFillet.AsValueString();
                }
                // 鉄骨 角形鋼管
                else if (columnType == 2)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymColumn.LookupParameter(_CmpParameters.SColumnRectFugo);
                    string fugo = parFugo.AsString();
                    ret[_CmpAttribute.ResourceText("IDS_CN_FUGO")] = fugo;

                    // 階
                    if (fugo == "")
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }
                    else if (name.Contains(fugo))
                    {
                        string kai = name.Substring(0, name.LastIndexOf(fugo));
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = kai;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }

                    // 構造マテリアル
                    Revit.DB.Parameter parStrcMat = famSymColumn.LookupParameter(_CmpParameters.SColumnRectStructuralMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_STRUCTURALMATERIAL")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_STRUCTURALMATERIAL")] = "";
                    }

                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSymColumn.LookupParameter(_CmpParameters.SColumnRectSyubetsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_SYUBETSU")] = parSyubetsu.AsString();

                    // 柱幅
                    Revit.DB.Parameter parHaba = famSymColumn.LookupParameter(_CmpParameters.SColumnRectHaba);
                    ret[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_HABA")] = parHaba.AsValueString();

                    // 柱せい
                    Revit.DB.Parameter parSei = famSymColumn.LookupParameter(_CmpParameters.SColumnRectSei);
                    ret[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_SEI")] = parSei.AsValueString();

                    // 板厚
                    Revit.DB.Parameter parItaAtsu = famSymColumn.LookupParameter(_CmpParameters.SColumnRectItaAtsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_ITAATSU")] = parItaAtsu.AsValueString();

                    
                    Revit.DB.Parameter t2 = famSymColumn.LookupParameter(_CmpParameters.SColumnRectT2);
                    if (t2 != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_TXT_COLUMN_T2")] = t2.AsValueString();
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_TXT_COLUMN_T2")] = "";
                    }

                    // フィレット
                    Revit.DB.Parameter parFillet = famSymColumn.LookupParameter(_CmpParameters.SColumnRectFillet);
                    ret[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_FILLET")] = parFillet.AsValueString();
                }
                // 鉄骨 鋼管
                else if (columnType == 3)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymColumn.LookupParameter(_CmpParameters.SColumnRoundFugo);
                    string fugo = parFugo.AsString();
                    ret[_CmpAttribute.ResourceText("IDS_CN_FUGO")] = fugo;

                    // 階
                    if (fugo == "")
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }
                    else if (name.Contains(fugo))
                    {
                        string kai = name.Substring(0, name.LastIndexOf(fugo));
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = kai;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }

                    // 構造マテリアル
                    Revit.DB.Parameter parStrcMat = famSymColumn.LookupParameter(_CmpParameters.SColumnRoundStructuralMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_STEELROUND_STRUCTURALMATERIAL")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_STEELROUND_STRUCTURALMATERIAL")] = "";
                    }

                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSymColumn.LookupParameter(_CmpParameters.SColumnRoundSyubetsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_STEELROUND_SYUBETSU")] = parSyubetsu.AsString();

                    // 直径
                    Revit.DB.Parameter parTyokkei = famSymColumn.LookupParameter(_CmpParameters.SColumnRoundDiameter);
                    ret[_CmpAttribute.ResourceText("IDS_CN_STEELROUND_TYOKKEI")] = parTyokkei.AsValueString();

                    // 板厚
                    Revit.DB.Parameter parItaAtsu = famSymColumn.LookupParameter(_CmpParameters.SColumnRoundItaAtsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_STEELROUND_ITAATSU")] = parItaAtsu.AsValueString();
                }
                // CFT 角形鋼管
                else if (columnType == 4)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRectFugo);
                    string fugo = parFugo.AsString();
                    ret[_CmpAttribute.ResourceText("IDS_CN_FUGO")] = fugo;

                    // 階
                    if (fugo == "")
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }
                    else if (name.Contains(fugo))
                    {
                        string kai = name.Substring(0, name.LastIndexOf(fugo));
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = kai;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }

                    // 構造マテリアル
                    Revit.DB.Parameter parStrcMat = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRectStructuralMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_STRUCTURALMATERIAL")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_STRUCTURALMATERIAL")] = "";
                    }

                    // コンクリートマテリアル
                    Revit.DB.Parameter parConcMat = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRectConcreteMaterial);
                    Revit.DB.ElementId concMatId = parConcMat.AsElementId();
                    Revit.DB.Element concMat = _CmpElements.RvtDBDoc.GetElement(concMatId);
                    if (concMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_CONCRETEMATERIAL")] = concMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_CONCRETEMATERIAL")] = "";
                    }

                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRectSyubetsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_SYUBETSU")] = parSyubetsu.AsString();

                    // 柱幅
                    Revit.DB.Parameter parHaba = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRectHaba);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_HABA")] = parHaba.AsValueString();

                    // 柱せい
                    Revit.DB.Parameter parSei = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRectSei);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_SEI")] = parSei.AsValueString();

                    // 板厚
                    Revit.DB.Parameter parItaAtsu = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRectItaAtsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_ITAATSU")] = parItaAtsu.AsValueString();

                    //t2
                    Revit.DB.Parameter t2 = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRectT2);
                    if (t2 != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_TXT_COLUMN_T2")] = t2.AsValueString();
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_TXT_COLUMN_T2")] = "";
                    }
                    // フィレット
                    Revit.DB.Parameter parFillet = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRectFillet);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_FILLET")] = parFillet.AsValueString();

                    
                }
                // CFT 鋼管
                else if (columnType == 5)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRoundFugo);
                    string fugo = parFugo.AsString();
                    ret[_CmpAttribute.ResourceText("IDS_CN_FUGO")] = fugo;

                    // 階
                    if (fugo == "")
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }
                    else if (name.Contains(fugo))
                    {
                        string kai = name.Substring(0, name.LastIndexOf(fugo));
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = kai;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }

                    // 構造マテリアル
                    Revit.DB.Parameter parStrcMat = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRoundStructuralMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_STRUCTURALMATERIAL")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_STRUCTURALMATERIAL")] = "";
                    }

                    // コンクリートマテリアル
                    Revit.DB.Parameter parConcMat = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRoundConcreteMaterial);
                    Revit.DB.ElementId concMatId = parConcMat.AsElementId();
                    Revit.DB.Element concMat = _CmpElements.RvtDBDoc.GetElement(concMatId);
                    if (concMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_CONCRETEMATERIAL")] = concMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_CONCRETEMATERIAL")] = "";
                    }

                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRoundSyubetsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_SYUBETSU")] = parSyubetsu.AsString();

                    // 直径
                    Revit.DB.Parameter parTyokkei = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRoundDiameter);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_TYOKKEI")] = parTyokkei.AsValueString();

                    // 板厚
                    Revit.DB.Parameter parItaAtsu = famSymColumn.LookupParameter(_CmpParameters.CFTColumnRoundItaAtsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_ITAATSU")] = parItaAtsu.AsValueString();
                }
                // S柱山形鋼
                else if (columnType == 6)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymColumn.LookupParameter(_CmpParameters.LColumnFugo);

                    string fugo = parFugo.AsString();
                    ret[_CmpAttribute.ResourceText("IDS_CN_FUGO")] = fugo;

                    // 階
                    if (fugo == "")
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }
                    else if (name.Contains(fugo))
                    {
                        string kai = name.Substring(0, name.LastIndexOf(fugo));
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = kai;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }

                    // 構造マテリアル
                    Revit.DB.Parameter parStrcMat = famSymColumn.LookupParameter(_CmpParameters.LColumnStrcMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_MATERIAL")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_MATERIAL")] = "";
                    }

                    // 柱幅 
                    Revit.DB.Parameter parHaba = famSymColumn.LookupParameter(_CmpParameters.LColumnHaba);
                    ret[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_HABA")] = parHaba.AsValueString();

                    // 柱せい
                    Revit.DB.Parameter parSei = famSymColumn.LookupParameter(_CmpParameters.LColumnSei);
                    ret[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_SEI")] = parSei.AsValueString();

                    //Direction thickness
                    Revit.DB.Parameter directionThickness = famSymColumn.LookupParameter(_CmpParameters.LColumnDirThick);
                    ret[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_DIRTHICK")] = directionThickness.AsValueString();

                    //Width thickness 
                    Revit.DB.Parameter widthThickness = famSymColumn.LookupParameter(_CmpParameters.LColumnWidthThick);
                    ret[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_WTHICK")] = widthThickness.AsValueString();                    
                }
                // S柱溝形鋼            
                else if (columnType == 7)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymColumn.LookupParameter(_CmpParameters.UColumnFugo);

                    string fugo = parFugo.AsString();
                    ret[_CmpAttribute.ResourceText("IDS_CN_FUGO")] = fugo;

                    // 階
                    if (fugo == "")
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }
                    else if (name.Contains(fugo))
                    {
                        string kai = name.Substring(0, name.LastIndexOf(fugo));
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = kai;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }

                    // 構造マテリアル
                    Revit.DB.Parameter parStrcMat = famSymColumn.LookupParameter(_CmpParameters.UColumnStrcMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_MATERIAL")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_MATERIAL")] = "";
                    }

                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSymColumn.LookupParameter(_CmpParameters.UColumnSyubetsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_SYUBETSU")] = parSyubetsu.AsString();

                    // 柱幅
                    Revit.DB.Parameter parHaba = famSymColumn.LookupParameter(_CmpParameters.UColumnHaba);
                    ret[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_HABA")] = parHaba.AsValueString();

                    // 柱せい 
                    Revit.DB.Parameter parSei = famSymColumn.LookupParameter(_CmpParameters.UColumnSei);
                    ret[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_SEI")] = parSei.AsValueString();

                    // ウェブ厚
                    Revit.DB.Parameter parWebAtsu = famSymColumn.LookupParameter(_CmpParameters.UColumnWebAtsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_WEBATSU")] = parWebAtsu.AsValueString();

                    // フランジ厚
                    Revit.DB.Parameter parFlangeAtsu = famSymColumn.LookupParameter(_CmpParameters.UColumnFlangeAtsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_FLANGEATSU")] = parFlangeAtsu.AsValueString();
                                        
                    //// フィレット
                    //Revit.DB.Parameter parFillet = famSymColumn.LookupParameter(_CmpParameters.SColumnHFillet);
                    //ret[_CmpAttribute.ResourceText("IDS_CN_STEELH_FILLET")] = parFillet.AsValueString();
                }
                // S柱リップ鋼
                else if (columnType == 8)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymColumn.LookupParameter(_CmpParameters.CColumnFugo);

                    string fugo = parFugo.AsString();
                    ret[_CmpAttribute.ResourceText("IDS_CN_FUGO")] = fugo;

                    // 階
                    if (fugo == "")
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }
                    else if (name.Contains(fugo))
                    {
                        string kai = name.Substring(0, name.LastIndexOf(fugo));
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = kai;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }

                    // 構造マテリアル
                    Revit.DB.Parameter parStrcMat = famSymColumn.LookupParameter(_CmpParameters.CColumnStrcMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_MATERIAL")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_MATERIAL")] = "";
                    }

                    // 柱幅
                    Revit.DB.Parameter parHaba = famSymColumn.LookupParameter(_CmpParameters.CColumnHaba);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_HABA")] = parHaba.AsValueString();

                    // 柱せい
                    Revit.DB.Parameter parSei = famSymColumn.LookupParameter(_CmpParameters.CColumnSei);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_SEI")] = parSei.AsValueString();

                    //リップ長
                    Revit.DB.Parameter lipLength = famSymColumn.LookupParameter(_CmpParameters.CColumnLipLength);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_LIPLENGTH")] = lipLength.AsValueString();

                    //板厚
                    Revit.DB.Parameter boardThickness = famSymColumn.LookupParameter(_CmpParameters.CColumnBoardThick);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_BOARDTHICK")] = boardThickness.AsValueString();
                                        
                }
                // S柱フラット板
                else if (columnType == 9)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymColumn.LookupParameter(_CmpParameters.FBColumnFugo);

                    string fugo = parFugo.AsString();
                    ret[_CmpAttribute.ResourceText("IDS_CN_FUGO")] = fugo;

                    // 階
                    if (fugo == "")
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }
                    else if (name.Contains(fugo))
                    {
                        string kai = name.Substring(0, name.LastIndexOf(fugo));
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = kai;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }

                    // 構造マテリアル
                    Revit.DB.Parameter parStrcMat = famSymColumn.LookupParameter(_CmpParameters.FBColumnStrcMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_MATERIAL")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_MATERIAL")] = "";
                    }

                    Revit.DB.Parameter width = famSymColumn.LookupParameter(_CmpParameters.FBColumnWidth);
                    ret[_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_WIDTH")] = width.AsValueString();

                    Revit.DB.Parameter boardThickness = famSymColumn.LookupParameter(_CmpParameters.FBColumnBoardThick);
                    ret[_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_BOARDTHICK")] = boardThickness.AsValueString();

                }
                // S柱丸棒
                else if (columnType == 10)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymColumn.LookupParameter(_CmpParameters.MColumnFugo);

                    string fugo = parFugo.AsString();
                    ret[_CmpAttribute.ResourceText("IDS_CN_FUGO")] = fugo;

                    // 階
                    if (fugo == "")
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }
                    else if (name.Contains(fugo))
                    {
                        string kai = name.Substring(0, name.LastIndexOf(fugo));
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = kai;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }
                    // 構造マテリアル
                    Revit.DB.Parameter parStrcMat = famSymColumn.LookupParameter(_CmpParameters.MColumnStrcMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_MCOLUMN_MATERIAL")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_MCOLUMN_MATERIAL")] = "";
                    }

                    Revit.DB.Parameter boardThickness = famSymColumn.LookupParameter(_CmpParameters.MColumnDiameter);
                    ret[_CmpAttribute.ResourceText("IDS_CN_MCOLUMN_DIAMETER")] = boardThickness.AsValueString();
                                        
                }
                // S柱T形鋼
                if (columnType == 11)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymColumn.LookupParameter(_CmpParameters.TColumnFugo);

                    string fugo = parFugo.AsString();
                    ret[_CmpAttribute.ResourceText("IDS_CN_FUGO")] = fugo;

                    // 階
                    if (fugo == "")
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }
                    else if (name.Contains(fugo))
                    {
                        string kai = name.Substring(0, name.LastIndexOf(fugo));
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = kai;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_KAI")] = name;
                    }

                    // ウェブマテリアル
                    Revit.DB.Parameter parWebMat = famSymColumn.LookupParameter(_CmpParameters.TColumnWebMat);
                    Revit.DB.ElementId webMatId = parWebMat.AsElementId();
                    Revit.DB.Element webMat = _CmpElements.RvtDBDoc.GetElement(webMatId);
                    if (webMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_WEBMAT")] = webMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_WEBMAT")] = "";
                    }

                    // フランジマテリアル
                    Revit.DB.Parameter parFlangeMat = famSymColumn.LookupParameter(_CmpParameters.TColumnFlangeMat); 
                    Revit.DB.ElementId flangeMatId = parFlangeMat.AsElementId();
                    Revit.DB.Element flangeMat = _CmpElements.RvtDBDoc.GetElement(flangeMatId);
                    if (flangeMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_FLANGEMAT")] = flangeMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_FLANGEMAT")] = "";
                    }

                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSymColumn.LookupParameter(_CmpParameters.TColumnSyubetsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_SYUBETSU")] = parSyubetsu.AsString();

                    // 柱幅
                    Revit.DB.Parameter parHaba = famSymColumn.LookupParameter(_CmpParameters.TColumnHaba);
                    ret[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_HABA")] = parHaba.AsValueString();

                    // 柱せい
                    Revit.DB.Parameter parSei = famSymColumn.LookupParameter(_CmpParameters.TColumnSei);
                    ret[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_SEI")] = parSei.AsValueString();

                    // ウェブ厚
                    Revit.DB.Parameter parWebAtsu = famSymColumn.LookupParameter(_CmpParameters.TColumnWebAtsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_WEBATSU")] = parWebAtsu.AsValueString();

                    // フランジ厚
                    Revit.DB.Parameter parFlangeAtsu = famSymColumn.LookupParameter(_CmpParameters.TColumnFlangeAtsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_FLANGEATSU")] = parFlangeAtsu.AsValueString();

                    //// フィレット
                    //Revit.DB.Parameter parFillet = famSymColumn.LookupParameter(_CmpParameters.SColumnHFillet);
                    //ret[_CmpAttribute.ResourceText("IDS_CN_STEELH_FILLET")] = parFillet.AsValueString();
                }
            }
            catch
            {
                return null;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>Get mark value </summary>
        /// <param name="famSymColumn"></param>
        /// <returns></returns>
        /// ================================================================================
        private string GetMarkValue(Revit.DB.FamilySymbol famSymColumn)
        {
            string symbol = string.Empty;

            var para_mark = famSymColumn.LookupParameter(_CmpParameters.ColumnMark);

            if (para_mark != null && para_mark.AsString() != null && para_mark.AsString().Trim() != string.Empty)
                symbol = para_mark.AsString();
            else
            {
                //Get from default in family
                var para_shape = famSymColumn.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                {
                    if (para_shape.AsInteger() == 6)
                    {
                        var para = famSymColumn.LookupParameter(_CmpParameters.SColumnHFillet);
                        if (para != null && para.HasValue && para.AsDouble() == 0)
                        {
                            symbol += "B";
                        }
                        symbol += _CmpAttribute.ResourceText("IDS_TXT_SHAPE_SYMBOL_H");
                    }
                    else if (para_shape.AsInteger() == 11)
                        symbol = _CmpAttribute.ResourceText("IDS_TXT_SHAPE_SYMBOL_L");
                    else if (para_shape.AsInteger() == 10)
                        symbol = _CmpAttribute.ResourceText("IDS_TXT_SHAPE_SYMBOL_U");
                    else if (para_shape.AsInteger() == 21)
                        symbol = _CmpAttribute.ResourceText("IDS_TXT_SHAPE_SYMBOL_C");
                    else if (para_shape.AsInteger() == 31)
                        symbol = _CmpAttribute.ResourceText("IDS_TXT_SHAPE_SYMBOL_FB");
                    else if (para_shape.AsInteger() == 13)
                        symbol = _CmpAttribute.ResourceText("IDS_TXT_SHAPE_SYMBOL_M");
                    else if (para_shape.AsInteger() == 17)
                        symbol = _CmpAttribute.ResourceText("IDS_TXT_SHAPE_SYMBOL_T");
                    else if (para_shape.AsInteger() == 14)
                    {
                        var para = famSymColumn.LookupParameter(_CmpParameters.SColumnRectFillet);
                        if (para != null && para.HasValue && para.AsDouble() == 0)
                        {
                            symbol += "B";
                        }
                        symbol += _CmpAttribute.ResourceText("IDS_TXT_SHAPE_SYMBOL_BOX");
                    }
                    else if (para_shape.AsInteger() == 15)
                        symbol = _CmpAttribute.ResourceText("IDS_TXT_SHAPE_SYMBOL_P");
                }
            }

            if (symbol != string.Empty)
                symbol += "-";

            return symbol;
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="columnAry" >柱ファミリ</param>
        /// <param name="columnType"><p>柱種類</p>
        ///                           <p>1 = 鉄骨 H形鋼、2 = 鉄骨 角形鋼管、3 = 鉄骨 鋼管、4 = CFT 角形鋼管、5 = CFT 鋼管</p></param>
        ///
        /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetData(Collections.Generic.IList<Revit.DB.FamilySymbol> columnAry,
                     int columnType)
        {
            if (_Data == null)
            {
                _Data = new System.Data.DataTable();
                DefDataFormat(ref _Data);
            }

            foreach (Revit.DB.FamilySymbol famSym in columnAry)
            {
                System.Data.DataRow row = GetData(famSym, columnType);

                if (row != null)
                {
                    // 符号、階が同じ要素は除く
                    bool contain = false;
                    foreach (System.Data.DataRow r in _Data.Rows)
                    {
                        if ((string)r[_CmpAttribute.ResourceText("IDS_CN_FUGO")] == (string)row[_CmpAttribute.ResourceText("IDS_CN_FUGO")] &&
                            (string)r[_CmpAttribute.ResourceText("IDS_CN_KAI")] == (string)row[_CmpAttribute.ResourceText("IDS_CN_KAI")])
                        {
                            contain = true;
                        }
                    }

                    if (contain == false)
                    {
                        _Data.Rows.Add(row);
                    }
                }
            }
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>データ</summary>
        ///
        /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        System.Data.DataTable Data
        {
            get
            {
                return _Data;
            }
        }

        #endregion Properties
    }
}