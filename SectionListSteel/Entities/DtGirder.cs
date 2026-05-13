using System;
using System.Linq;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Data;

namespace SectionListSteel.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - 梁</summary>
    /// ================================================================================
    public class DtGirder : SectionListSteel.Entities.DtBase
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
        /// <history>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public DtGirder(SectionListSteel.Components.Attribute cmpAttribute,
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
        /// <history>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        private
        void DefDataFormat(ref System.Data.DataTable data)
        {
            // 梁の種類
            // 1 = 梁、2 = 片持ち梁
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TYPE"), typeof(int));

            // 符号
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_FUGO"), typeof(string));

            // 階
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_KAI"), typeof(string));

            // 梁
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_SYUBETSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_E"), typeof(string));

            // 片持ち梁
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SYUBETSU"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FILLET_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FILLET_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_BH_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_BH_E"), typeof(string));

            //Mark
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_GIRDER_E"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_S"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_E"), typeof(string));

            // S梁・ブレース山形鋼

            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_LGIRDERMAT"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_LGirderHashiyubetsu"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_LGirderSei_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_LGirderHaba_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_LGirderDirThick_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_LGirderWidthThick_C"), typeof(string));

            //  S梁・ブレース溝形鋼

            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_UGIRDERMAT"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_UGirderHashiyubetsu"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_UGirderSei_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_UGirderHaba_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_UGirderWebAtsu_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_UGirderFlangeAtsu_C"), typeof(string));

            //  S梁・ブレースリップ溝形鋼

            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CGIRDERMAT"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CGirderHashiyubetsu"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CGirderSei_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CGirderHaba_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CGirderLipLength_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_CGirderThick_C"), typeof(string));

            //  ブレースフラットバー

            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_FBGIRDERMAT"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_FBGirderBraceType"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_FBGIRDER_WIDTH"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_FBGIRDER_THICK"), typeof(string));

            //  ブレース丸鋼

            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_MGIRDERMAT"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_MGirderBraceType"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_MGIRDER_DIAMETER"), typeof(string));

            //  ブレース角形鋼管

            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_RECTGIRDERMAT"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_RECTGirderBraceType"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_HABA"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_SEI"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_DIRTHICK"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_DIRWIDTH"), typeof(string));

            //  ブレース円形鋼管

            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_PGIRDERMAT"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_PGirderBraceType"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_PGIRDER_DIAMETER"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_PGIRDER_ITAATSU"), typeof(string));

            //
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TGIRDERMAT"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TGirderHashiyubetsu"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TGirderSei_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TGirderHaba_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TGirderWebAtsu_C"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_CN_TGirderFlangeAtsu_C"), typeof(string));







            //構造マテリアル
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_GIRDER_MATERIAL"), typeof(string));
            //種別
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_GIRDER_TYPE"), typeof(string));
            //
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACE_HEIGHT"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACE_WIDTH"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACE_WEBTHICK"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACE_FLANGETHICK"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACE_LIPLENGTH"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACE_PLATETHICK"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACE_B"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACE_T"), typeof(string)); 
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACE_D"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACE_H"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACE_T1"), typeof(string));
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACE_T2"), typeof(string)); 
            data.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACE_R"), typeof(string));





        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="famSymGirder">梁ファミリ</param>
        /// <param name="girderType"  ><p>梁種類</p>
        ///                             <p>1 = 梁、2 = 片持ち梁</p></param>
        ///
        /// <history><p>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/08/01 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        System.Data.DataRow GetData(Revit.DB.FamilySymbol famSymGirder,
                                    int girderType)
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
                //////////////////////////////////////////////////////////////////////////
                if (girderType == 1)
                {
                    string girderMark_S = GetMarkValue(famSymGirder, _CmpParameters.GirderMark_S);
                    string girderMark_C = GetMarkValue(famSymGirder, _CmpParameters.GirderMark_C);
                    string girderMark_E = GetMarkValue(famSymGirder, _CmpParameters.GirderMark_E);

                    if (girderMark_S == string.Empty && girderMark_C == string.Empty && girderMark_E == string.Empty)
                        return null;
                    else
                    {
                        ret = _Data.NewRow();
                        //Mark
                        ret[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")] = girderMark_S;
                        ret[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] = girderMark_C;
                        ret[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_E")] = girderMark_E;
                    }
                }
                else if (girderType == 2)
                {
                    string cantiGirderMark_S = GetMarkValue(famSymGirder, _CmpParameters.CantiGirderMark_S);
                    string cantiGirderMark_E = GetMarkValue(famSymGirder, _CmpParameters.CantiGirderMark_E);

                    if (cantiGirderMark_S == string.Empty && cantiGirderMark_E == string.Empty)
                        return null;
                    else
                    {
                        ret = _Data.NewRow();
                        //Mark
                        ret[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_S")] = cantiGirderMark_S;
                        ret[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_E")] = cantiGirderMark_E;
                    }
                }
                else if (girderType == 3 || girderType == 4 || girderType == 5 || girderType == 8)
                {

                    string girderMark_C = GetMarkValue(famSymGirder, _CmpParameters.GirderMark_C);
                    if (girderMark_C == string.Empty)
                        return null;
                    else
                    {
                        ret = _Data.NewRow();
                        //Mark
                        ret[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] = girderMark_C;
                    }
                }
                else
                {
                    string girderMark_C = GetMarkValue(famSymGirder, _CmpParameters.GirderMark);
                    if (girderMark_C == string.Empty)
                        return null;
                    else
                    {
                        ret = _Data.NewRow();
                        //Mark
                        ret[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] = girderMark_C;
                    }
                }
                //////////////////////////////////////////////////////////////////////////

                // 名前
                string name = famSymGirder.Name;

                // 梁種類
                ret[_CmpAttribute.ResourceText("IDS_CN_TYPE")] = girderType;

                // 梁
                if (girderType == 1)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymGirder.LookupParameter(_CmpParameters.GirderFugo);

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

                    // 始端 ウェブマテリアル
                    Revit.DB.Parameter parWebMat = famSymGirder.LookupParameter(_CmpParameters.GirderWebMaterial_S);
                    Revit.DB.ElementId webMatId = parWebMat.AsElementId();
                    Revit.DB.Element webMat = _CmpElements.RvtDBDoc.GetElement(webMatId);
                    if (webMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_S")] = webMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_S")] = "";
                    }

                    // 始端 フランジマテリアル
                    Revit.DB.Parameter parFlangeMat = famSymGirder.LookupParameter(_CmpParameters.GirderFlangeMaterial_S);
                    Revit.DB.ElementId flangeMatId = parFlangeMat.AsElementId();
                    Revit.DB.Element flangeMat = _CmpElements.RvtDBDoc.GetElement(flangeMatId);
                    if (flangeMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S")] = flangeMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S")] = "";
                    }

                    // 中央 ウェブマテリアル
                    parWebMat = famSymGirder.LookupParameter(_CmpParameters.GirderWebMaterial_C);
                    webMatId = parWebMat.AsElementId();
                    webMat = _CmpElements.RvtDBDoc.GetElement(webMatId);
                    if (webMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_C")] = webMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_C")] = "";
                    }

                    // 中央 フランジマテリアル
                    parFlangeMat = famSymGirder.LookupParameter(_CmpParameters.GirderFlangeMaterial_C);
                    flangeMatId = parFlangeMat.AsElementId();
                    flangeMat = _CmpElements.RvtDBDoc.GetElement(flangeMatId);
                    if (flangeMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")] = flangeMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")] = "";
                    }

                    // 終端 ウェブマテリアル
                    parWebMat = famSymGirder.LookupParameter(_CmpParameters.GirderWebMaterial_E);
                    webMatId = parWebMat.AsElementId();
                    webMat = _CmpElements.RvtDBDoc.GetElement(webMatId);
                    if (webMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_E")] = webMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_E")] = "";
                    }

                    // 終端 フランジマテリアル
                    parFlangeMat = famSymGirder.LookupParameter(_CmpParameters.GirderFlangeMaterial_E);
                    flangeMatId = parFlangeMat.AsElementId();
                    flangeMat = _CmpElements.RvtDBDoc.GetElement(flangeMatId);
                    if (flangeMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_E")] = flangeMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_E")] = "";
                    }

                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSymGirder.LookupParameter(_CmpParameters.GirderSyubetsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SYUBETSU")] = parSyubetsu.AsString();

                    // 始端 梁せい
                    Revit.DB.Parameter parSei = famSymGirder.LookupParameter(_CmpParameters.GirderSei_S);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_S")] = parSei.AsValueString();

                    // 始端 梁幅
                    Revit.DB.Parameter parHaba = famSymGirder.LookupParameter(_CmpParameters.GirderHaba_S);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_S")] = parHaba.AsValueString();

                    // 始端 ウェブ厚
                    Revit.DB.Parameter parWebAtsu = famSymGirder.LookupParameter(_CmpParameters.GirderWebAtsu_S);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_S")] = parWebAtsu.AsValueString();

                    // 始端 フランジ厚
                    Revit.DB.Parameter parFlangeAtsu = famSymGirder.LookupParameter(_CmpParameters.GirderFlangeAtsu_S);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_S")] = parFlangeAtsu.AsValueString();

                    // 始端 フィレット
                    Revit.DB.Parameter parFillet = famSymGirder.LookupParameter(_CmpParameters.GirderFillet_S);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_S")] = parFillet.AsValueString();

                    // 中央 梁せい
                    parSei = famSymGirder.LookupParameter(_CmpParameters.GirderSei_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_C")] = parSei.AsValueString();

                    // 中央 梁幅
                    parHaba = famSymGirder.LookupParameter(_CmpParameters.GirderHaba_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_C")] = parHaba.AsValueString();

                    // 中央 ウェブ厚
                    parWebAtsu = famSymGirder.LookupParameter(_CmpParameters.GirderWebAtsu_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_C")] = parWebAtsu.AsValueString();

                    // 中央 フランジ厚
                    parFlangeAtsu = famSymGirder.LookupParameter(_CmpParameters.GirderFlangeAtsu_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_C")] = parFlangeAtsu.AsValueString();

                    // 中央 フィレット
                    parFillet = famSymGirder.LookupParameter(_CmpParameters.GirderFillet_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_C")] = parFillet.AsValueString();

                    // 終端 梁せい
                    parSei = famSymGirder.LookupParameter(_CmpParameters.GirderSei_E);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_E")] = parSei.AsValueString();

                    // 終端 梁幅
                    parHaba = famSymGirder.LookupParameter(_CmpParameters.GirderHaba_E);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_E")] = parHaba.AsValueString();

                    // 終端 ウェブ厚
                    parWebAtsu = famSymGirder.LookupParameter(_CmpParameters.GirderWebAtsu_E);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_E")] = parWebAtsu.AsValueString();

                    // 終端 フランジ厚
                    parFlangeAtsu = famSymGirder.LookupParameter(_CmpParameters.GirderFlangeAtsu_E);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_E")] = parFlangeAtsu.AsValueString();

                    // 終端 フィレット
                    parFillet = famSymGirder.LookupParameter(_CmpParameters.GirderFillet_E);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_E")] = parFillet.AsValueString();

                    // 始端 ハンチ長さ
                    Revit.DB.Parameter parHaunchNagasa = famSymGirder.LookupParameter(_CmpParameters.GirderHaunchNagasa_S);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_S")] = parHaunchNagasa.AsValueString();

                    // 終端 ハンチ長さ
                    parHaunchNagasa = famSymGirder.LookupParameter(_CmpParameters.GirderHaunchNagasa_E);
                    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_E")] = parHaunchNagasa.AsValueString();

                    //// 始端 BH
                    //// パラメータマッピングファイルにはないパラメータのため、パラメータがある場合だけ処理
                    //Revit.DB.Parameter parBH = famSymGirder.LookupParameter(_CmpParameters.GirderBH_S);
                    //if (parBH == null)
                    //{
                    //  ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_S")] = "0";
                    //}
                    //else
                    //{
                    //  if (parBH.StorageType == Autodesk.Revit.DB.StorageType.Integer)
                    //  {
                    //    // オフ
                    //    if (parBH.AsInteger() == 0)
                    //    {
                    //      ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_S")] = "1";
                    //    }
                    //    // オン
                    //    if (parBH.AsInteger() == 1)
                    //    {
                    //      ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_S")] = "2";
                    //    }
                    //  }
                    //  else
                    //  {
                    //    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_S")] = "0";
                    //  }
                    //}

                    //// 終端 BH
                    //parBH = famSymGirder.LookupParameter(_CmpParameters.GirderBH_E);
                    //if (parBH == null)
                    //{
                    //  ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_E")] = "0";
                    //}
                    //else
                    //{
                    //  if (parBH.StorageType == Autodesk.Revit.DB.StorageType.Integer)
                    //  {
                    //    // オフ
                    //    if (parBH.AsInteger() == 0)
                    //    {
                    //      ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_E")] = "1";
                    //    }
                    //    // オン
                    //    if (parBH.AsInteger() == 1)
                    //    {
                    //      ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_E")] = "2";
                    //    }
                    //  }
                    //  else
                    //  {
                    //    ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_E")] = "0";
                    //  }
                    //}

                    //// BHが一方しかない場合は両方なし
                    //if ((string)ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_S")] == "0" ||
                    //    (string)ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_E")] == "0")
                    //{
                    //  ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_S")] = "0";
                    //  ret[_CmpAttribute.ResourceText("IDS_CN_GIRDER_BH_E")] = "0";
                    //}
                }
                // 片持ち梁
                else if (girderType == 2)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymGirder.LookupParameter(_CmpParameters.CantiGirderFugo);

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

                    // 元端 ウェブマテリアル
                    Revit.DB.Parameter parWebMat = famSymGirder.LookupParameter(_CmpParameters.CantiGirderWebMaterial_S);
                    Revit.DB.ElementId webMatId = parWebMat.AsElementId();
                    Revit.DB.Element webMat = _CmpElements.RvtDBDoc.GetElement(webMatId);
                    if (webMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_S")] = webMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_S")] = "";
                    }

                    // 元端 フランジマテリアル
                    Revit.DB.Parameter parFlangeMat = famSymGirder.LookupParameter(_CmpParameters.CantiGirderFlangeMaterial_S);
                    Revit.DB.ElementId flangeMatId = parFlangeMat.AsElementId();
                    Revit.DB.Element flangeMat = _CmpElements.RvtDBDoc.GetElement(flangeMatId);
                    if (flangeMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")] = flangeMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")] = "";
                    }

                    // 先端 ウェブマテリアル
                    parWebMat = famSymGirder.LookupParameter(_CmpParameters.CantiGirderWebMaterial_E);
                    webMatId = parWebMat.AsElementId();
                    webMat = _CmpElements.RvtDBDoc.GetElement(webMatId);
                    if (webMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_E")] = webMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_E")] = "";
                    }

                    // 先端 フランジマテリアル
                    parFlangeMat = famSymGirder.LookupParameter(_CmpParameters.CantiGirderFlangeMaterial_E);
                    flangeMatId = parFlangeMat.AsElementId();
                    flangeMat = _CmpElements.RvtDBDoc.GetElement(flangeMatId);
                    if (flangeMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_E")] = flangeMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_E")] = "";
                    }

                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSymGirder.LookupParameter(_CmpParameters.CantiGirderSyubetsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SYUBETSU")] = parSyubetsu.AsString();

                    // 元端 梁せい
                    Revit.DB.Parameter parSei = famSymGirder.LookupParameter(_CmpParameters.CantiGirderSei_S);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_S")] = parSei.AsValueString();

                    // 元端 梁幅
                    Revit.DB.Parameter parHaba = famSymGirder.LookupParameter(_CmpParameters.CantiGirderHaba_S);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_S")] = parHaba.AsValueString();

                    // 元端 ウェブ厚
                    Revit.DB.Parameter parWebAtsu = famSymGirder.LookupParameter(_CmpParameters.CantiGirderWebAtsu_S);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_S")] = parWebAtsu.AsValueString();

                    // 元端 フランジ厚
                    Revit.DB.Parameter parFlangeAtsu = famSymGirder.LookupParameter(_CmpParameters.CantiGirderFlangeAtsu_S);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_S")] = parFlangeAtsu.AsValueString();

                    // 元端 フィレット
                    Revit.DB.Parameter parFillet = famSymGirder.LookupParameter(_CmpParameters.CantiGirderFillet_S);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FILLET_S")] = parFillet.AsValueString();

                    // 先端 梁せい
                    parSei = famSymGirder.LookupParameter(_CmpParameters.CantiGirderSei_E);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_E")] = parSei.AsValueString();

                    // 先端 梁幅
                    parHaba = famSymGirder.LookupParameter(_CmpParameters.CantiGirderHaba_E);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_E")] = parHaba.AsValueString();

                    // 先端 ウェブ厚
                    parWebAtsu = famSymGirder.LookupParameter(_CmpParameters.CantiGirderWebAtsu_E);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_E")] = parWebAtsu.AsValueString();

                    // 先端 フランジ厚
                    parFlangeAtsu = famSymGirder.LookupParameter(_CmpParameters.CantiGirderFlangeAtsu_E);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_E")] = parFlangeAtsu.AsValueString();

                    // 先端 フィレット
                    parFillet = famSymGirder.LookupParameter(_CmpParameters.CantiGirderFillet_E);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FILLET_E")] = parFillet.AsValueString();
                }
                // S梁・ブレース山形鋼
                else if (girderType == 3)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymGirder.LookupParameter(_CmpParameters.LGirderFugo); 

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
                    Revit.DB.Parameter parStrcMat = famSymGirder.LookupParameter(_CmpParameters.LGirderMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_LGIRDERMAT")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_LGIRDERMAT")] = "";
                    }

                    // 中央 梁せい
                    var parSei = famSymGirder.LookupParameter(_CmpParameters.LGirderSei_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_LGirderSei_C")] = parSei.AsValueString();

                    // 中央 梁幅
                    var parHaba = famSymGirder.LookupParameter(_CmpParameters.LGirderHaba_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_LGirderHaba_C")] = parHaba.AsValueString();

                    var directionThickness = famSymGirder.LookupParameter(_CmpParameters.LGirderDirThick_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_LGirderDirThick_C")] = directionThickness.AsValueString();

                    //Width thickness
                    Revit.DB.Parameter widthThickness = famSymGirder.LookupParameter(_CmpParameters.LGirderWidthThick_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_LGirderWidthThick_C")] = widthThickness.AsValueString();
                }
                //  S梁・ブレース溝形鋼
                else if (girderType == 4)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymGirder.LookupParameter(_CmpParameters.UGirderFugo);

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
                    Revit.DB.Parameter parStrcMat = famSymGirder.LookupParameter(_CmpParameters.UGirderMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_UGIRDERMAT")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_UGIRDERMAT")] = "";
                    }
                    // 中央 梁せい
                    var parSei = famSymGirder.LookupParameter(_CmpParameters.UGirderSei_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_UGirderSei_C")] = parSei.AsValueString();

                    // 中央 梁幅
                    var parHaba = famSymGirder.LookupParameter(_CmpParameters.UGirderHaba_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_UGirderHaba_C")] = parHaba.AsValueString();

                    // 中央 ウェブ厚
                    var parWebAtsu = famSymGirder.LookupParameter(_CmpParameters.UGirderWebAtsu_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_UGirderWebAtsu_C")] = parWebAtsu.AsValueString();

                    // 中央 フランジ厚
                    var parFlangeAtsu = famSymGirder.LookupParameter(_CmpParameters.UGirderFlangeAtsu_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_UGirderFlangeAtsu_C")] = parFlangeAtsu.AsValueString();
                }
                // S梁・ブレースリップ溝形鋼
                else if (girderType == 5)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymGirder.LookupParameter(_CmpParameters.CGirderFugo);

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
                    Revit.DB.Parameter parStrcMat = famSymGirder.LookupParameter(_CmpParameters.CGirderMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CGIRDERMAT")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_CGIRDERMAT")] = "";
                    }

                    // 中央 梁せい
                    var parSei = famSymGirder.LookupParameter(_CmpParameters.CGirderSei_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CGirderSei_C")] = parSei.AsValueString();

                    // 中央 梁幅
                    var parHaba = famSymGirder.LookupParameter(_CmpParameters.CGirderHaba_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CGirderHaba_C")] = parHaba.AsValueString();

                    // 中央 リップ長
                    var parLipLength = famSymGirder.LookupParameter(_CmpParameters.CGirderLipLength_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CGirderLipLength_C")] = parLipLength.AsValueString();

                    // 中央 板厚
                    var parThickness = famSymGirder.LookupParameter(_CmpParameters.CGirderThick_C);
                    ret[_CmpAttribute.ResourceText("IDS_CN_CGirderThick_C")] = parThickness.AsValueString();
                }
                // ブレースフラットバー
                else if (girderType == 6)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymGirder.LookupParameter(_CmpParameters.FBGirderFugo);

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
                    Revit.DB.Parameter parStrcMat = famSymGirder.LookupParameter(_CmpParameters.FBGirderMaterial); 
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_FBGIRDERMAT")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_FBGIRDERMAT")] = "";
                    }
                    // 幅
                    var parWidth = famSymGirder.LookupParameter(_CmpParameters.FBGirderWidth);
                    ret[_CmpAttribute.ResourceText("IDS_CN_FBGIRDER_WIDTH")] = parWidth.AsValueString();

                    // 板厚
                    var parBoardThickness = famSymGirder.LookupParameter(_CmpParameters.FBGirderBoardThick);
                    ret[_CmpAttribute.ResourceText("IDS_CN_FBGIRDER_THICK")] = parBoardThickness.AsValueString();
                }
                // ブレース丸鋼
                else if (girderType == 7)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymGirder.LookupParameter(_CmpParameters.MGirderFugo);

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
                    Revit.DB.Parameter parStrcMat = famSymGirder.LookupParameter(_CmpParameters.MGirderMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_MGIRDERMAT")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_MGIRDERMAT")] = "";
                    }
                    // 直径
                    var parDiameter = famSymGirder.LookupParameter(_CmpParameters.MGirderDiameter);
                    ret[_CmpAttribute.ResourceText("IDS_CN_MGIRDER_DIAMETER")] = parDiameter.AsValueString();
                }
                //ブレース円形鋼管
                else if (girderType == 8)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymGirder.LookupParameter(_CmpParameters.TGirderFugo);

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
                    Revit.DB.Parameter parStrcMat = famSymGirder.LookupParameter(_CmpParameters.TGirderMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_TGIRDERMAT")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_TGIRDERMAT")] = "";
                    }
                    
                    var parSei = famSymGirder.LookupParameter(_CmpParameters.TGirderSei);
                    ret[_CmpAttribute.ResourceText("IDS_CN_TGirderSei_C")] = parSei.AsValueString();

                    
                    var parHaba = famSymGirder.LookupParameter(_CmpParameters.TGirderHaba);
                    ret[_CmpAttribute.ResourceText("IDS_CN_TGirderHaba_C")] = parHaba.AsValueString();

                    
                    var parWebAtsu = famSymGirder.LookupParameter(_CmpParameters.TGirderWebAtsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_TGirderWebAtsu_C")] = parWebAtsu.AsValueString();

                    
                    var parFlangeAtsu = famSymGirder.LookupParameter(_CmpParameters.TGirderFlangeAtsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_TGirderFlangeAtsu_C")] = parFlangeAtsu.AsValueString();                    
                }
                // ブレース角形鋼管
                else if (girderType == 9)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymGirder.LookupParameter(_CmpParameters.RectGirderFugo);

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
                    Revit.DB.Parameter parStrcMat = famSymGirder.LookupParameter(_CmpParameters.RectGirderMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDERMAT")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDERMAT")] = "";
                    }
                    // 鉄骨せい
                    var SteelFrame = famSymGirder.LookupParameter(_CmpParameters.RectGirderSei);
                    ret[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_SEI")] = SteelFrame.AsValueString();

                    // 鉄骨幅
                    var SteelWFrame = famSymGirder.LookupParameter(_CmpParameters.RectGirderHaba);
                    ret[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_HABA")] = SteelWFrame.AsValueString();

                    // せい方向の板厚
                    var ThicknessDirect = famSymGirder.LookupParameter(_CmpParameters.RectGirderDirThick);
                    ret[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_DIRTHICK")] = ThicknessDirect.AsValueString();

                    // 幅方向の板厚
                    var WidthDirect = famSymGirder.LookupParameter(_CmpParameters.RectGirderDirWidth);
                    ret[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_DIRWIDTH")] = WidthDirect.AsValueString();
                }
                // S梁カットティー
                else if (girderType == 10)
                {
                    // 符号
                    Revit.DB.Parameter parFugo = famSymGirder.LookupParameter(_CmpParameters.PGirderFugo);

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
                    Revit.DB.Parameter parStrcMat = famSymGirder.LookupParameter(_CmpParameters.PGirderMaterial);
                    Revit.DB.ElementId strcMatId = parStrcMat.AsElementId();
                    Revit.DB.Element strcMat = _CmpElements.RvtDBDoc.GetElement(strcMatId);
                    if (strcMat != null)
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_PGIRDERMAT")] = strcMat.Name;
                    }
                    else
                    {
                        ret[_CmpAttribute.ResourceText("IDS_CN_PGIRDERMAT")] = "";
                    }
                    // 直径
                    var parDiameter = famSymGirder.LookupParameter(_CmpParameters.PGirderDiameter);
                    ret[_CmpAttribute.ResourceText("IDS_CN_PGIRDER_DIAMETER")] = parDiameter.AsValueString();

                    // 板厚
                    var parBoardThickness = famSymGirder.LookupParameter(_CmpParameters.PGirderItaatsu);
                    ret[_CmpAttribute.ResourceText("IDS_CN_PGIRDER_ITAATSU")] = parBoardThickness.AsValueString();
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
        /// <param name="famSymGirder">FamilySymbol</param>
        /// <param name="parameterName">ParameterName</param>
        /// <returns></returns>
        /// ================================================================================
        private string GetMarkValue(Revit.DB.FamilySymbol famSymGirder, string parameterName)
        {
            string symbol = string.Empty;
            Revit.DB.Parameter para_mark = famSymGirder.LookupParameter(parameterName);

            if (para_mark != null && para_mark.AsString() != null && para_mark.AsString().Trim() != string.Empty)
                symbol = para_mark.AsString();
            else
            {
                //Get from default in family
                var para_shape = famSymGirder.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_SECTION_SHAPE);
                if (para_shape != null && para_shape.AsValueString().Trim() != string.Empty)
                {
                    if (para_shape.AsInteger() == 6)
                    {
                        if (CheckFillet(famSymGirder))
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
                        if (CheckFillet(famSymGirder))
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

        private bool CheckFillet(Revit.DB.FamilySymbol famSymGirder)
        {
            var para1 = famSymGirder.LookupParameter(_CmpParameters.CantiGirderFillet_S);
            var para2 = famSymGirder.LookupParameter(_CmpParameters.CantiGirderFillet_E);
            

            if (para1 != null && para1.HasValue && para1.AsDouble() == 0)
            {
                if (para2 != null && para2.HasValue && para2.AsDouble() == 0)
                {
                    return true;
                }
            }           

            para1 = famSymGirder.LookupParameter(_CmpParameters.GirderFillet_S);
            para2 = famSymGirder.LookupParameter(_CmpParameters.GirderFillet_C);
            var para3 = famSymGirder.LookupParameter(_CmpParameters.GirderFillet_E);

            if (para1 != null && para1.HasValue && para1.AsDouble() == 0)
            {
                if (para2 != null && para2.HasValue && para2.AsDouble() == 0)
                {
                    if (para3 != null && para3.HasValue && para3.AsDouble() == 0)
                    {
                        return true;
                    }
                }
            }

            para1 = famSymGirder.LookupParameter(_CmpParameters.RectGirderFillet);
            if (para1 != null && para1.HasValue && para1.AsDouble() == 0)
            {
                return true;
            }

            return false;
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="girderAry" >梁ファミリ</param>
        /// <param name="girderType"><p>梁種類</p>
        ///                           <p>1 = 梁、2 = 片持ち梁</p></param>
        ///
        /// <history>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetData(Collections.Generic.IList<Revit.DB.FamilySymbol> girderAry,
                     int girderType)
        {
            if (_Data == null)
            {
                _Data = new System.Data.DataTable();
                DefDataFormat(ref _Data);
            }

            foreach (Revit.DB.FamilySymbol famSym in girderAry)
            {
                System.Data.DataRow row = GetData(famSym, girderType);

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
        /// <history>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</history>
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