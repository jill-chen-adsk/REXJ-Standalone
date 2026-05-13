using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Components
{
    /// ================================================================================
    /// <summary>サービス</summary>
    /// ================================================================================
    public class ESM_Service
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>要素</summary>
        private RvtExtApp.Components.ESM_Elements _CmpElements;

        /// <summary>図形</summary>
        private RvtExtApp.Components.ESM_Geometry _CmpGeometry;

        /// <summary>パラメータ</summary>
        private RvtExtApp.Components.ESM_Parameters _CmpParameters;

        /// <summary>設定</summary>
        private RvtExtApp.Components.ESM_Settings _CmpSettings;

        /// <summary>エラーメッセージ</summary>
        private string _ErrMsg;

        /// <summary>Document</summary>
        private Document _Doc;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        /// <param name="cmpElements"   >要素</param>
        /// <param name="cmpGeometry"   >図形</param>
        /// <param name="cmpParameters" >パラメータ</param>
        /// <param name="cmpSettings"   >設定</param>
        ///
        /// <history><p>2011/12/01 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2011/12/01 Modified Applied Technology</p><history>
        /// ================================================================================
        public ESM_Service(Document doc, RvtExtApp.Components.Attribute cmpAttribute,
                       RvtExtApp.Components.ESM_Elements cmpElements,
                       RvtExtApp.Components.ESM_Geometry cmpGeometry,
                       RvtExtApp.Components.ESM_Parameters cmpParameters,
                       RvtExtApp.Components.ESM_Settings cmpSettings)
        {
            // 初期化
            _Doc = doc;
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;

            _ErrMsg = "";
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>タグ要素判定</summary>
        ///
        /// <param name="elemTag" >タグ要素</param>
        /// <param name="mode"    ><p>モード</p>
        ///                               <p>11 = 外壁</p>
        ///                               <p>12 = 内壁</p>
        ///                               <p>21 = 柱</p>
        ///                               <p>31 = 大梁</p>
        ///                               <p>32 = 小梁</p>
        ///                               <p>33 = 鉛直ブレース</p>
        ///                               <p>34 = 水平ブレース</p>
        ///                               <p>35 = その他</p>
        ///                               <p>41 = スラブ</p>
        ///                               <p>51 = 基礎</p></param>
        ///
        /// <returns>結果</returns>
        ///
        /// <history><p>2011/12/07 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        bool BoolElemTag(Element elemTag, int mode)
        {
            // 戻り値
            bool ret = false;

            // 初期化
            Element elemType = null;
            Category elemCat = null;
            IList<string> idCatAry = null;

            // タグ要素
            IndependentTag indpdntTag = elemTag as IndependentTag;
            if (indpdntTag == null)
            {
                return ret;
            }

            // 配置要素
            foreach (Element locElem in indpdntTag.GetTaggedLocalElements())
            {
                if (locElem == null)
                {
                    return ret;
                }

                // 壁
                if ((mode > 10) && (mode < 20))
                {
                    // 要素タイプ
                    elemType = _CmpElements.GetElementType(locElem);
                    if (elemType != null)
                    {
                        // 構成
                        int func = 0;
                        if (_CmpParameters.GetValue(elemType,
                                                    BuiltInParameter.FUNCTION_PARAM,
                                                    ref func) < -1)
                        {
                        }

                        switch (func)
                        {
                            // 内壁
                            case 0:
                                if (mode == 12)
                                {
                                    ret = true;
                                }
                                break;

                            //外壁
                            case 1:
                                if (mode == 11)
                                {
                                    ret = true;
                                }
                                break;
                        }
                    }
                }

                // 柱
                if ((mode > 20) && (mode < 30))
                {
                    // カテゴリ
                    elemCat = locElem.Category;
                    if (elemCat != null)
                    {
                        // カテゴリ
                        var idElemCat = elemCat.Id.ToString();

                        idCatAry = new List<string>();
                        idCatAry.Add(_CmpElements.GetCategory(BuiltInCategory.OST_Columns).Id.ToString());
                        idCatAry.Add(_CmpElements.GetCategory(BuiltInCategory.OST_StructuralColumns).Id.ToString());

                        if (idCatAry.Contains(idElemCat) == true)
                        {
                            ret = true;
                        }
                    }
                }

                // 梁
                if ((mode > 30) && (mode < 40))
                {
                    // 構造用途
                    int usage = 0;
                    if (_CmpParameters.GetValue(locElem,
                                                BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM,
                                                ref usage) < -1)
                    {
                    }

                    switch (usage)
                    {
                        // 大梁
                        case 3:
                            if (mode == 31)
                            {
                                ret = true;
                            }
                            break;

                        // 小梁
                        case 4:
                            if (mode == 32)
                            {
                                ret = true;
                            }
                            break;

                        // その他
                        case 6:
                            if (mode == 35)
                            {
                                ret = true;
                            }
                            break;

                        // 鉛直ブレース
                        case 7:
                            if (mode == 33)
                            {
                                ret = true;
                            }
                            break;

                        // 水平ブレース
                        case 8:
                            if (mode == 34)
                            {
                                ret = true;
                            }
                            break;
                    }
                }

                // スラブ
                if ((mode > 40) && (mode < 50))
                {
                    // カテゴリ
                    elemCat = locElem.Category;
                    if (elemCat != null)
                    {
                        // カテゴリ
                        var idElemCat = elemCat.Id.ToString();

                        idCatAry = new List<string>();
                        idCatAry.Add(_CmpElements.GetCategory(BuiltInCategory.OST_Floors).Id.ToString());

                        if (idCatAry.Contains(idElemCat) == true)
                        {
                            ret = true;
                        }
                    }
                }

                // 基礎
                if ((mode > 50) && (mode < 60))
                {
                    // カテゴリ
                    elemCat = locElem.Category;
                    if (elemCat != null)
                    {
                        // カテゴリ
                        var idElemCat = elemCat.Id.ToString();

                        idCatAry = new List<string>();
                        idCatAry.Add(_CmpElements.GetCategory(BuiltInCategory.OST_StructuralFoundation).Id.ToString());

                        if (idCatAry.Contains(idElemCat) == true)
                        {
                            ret = true;
                        }
                    }
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>タグ要素取得</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="builtInCategory">BuiltInCategory</param>
        /// <param name="markVal"     >符号 - 値</param>
        /// <param name="levelVale"   >レベル - 値</param>
        /// <param name="modeMark"    ><p>モード - 符号</p>
        ///                               <p>1 = 符号のみ</p>
        ///                               <p>2 = 符号、レベル</p></param>
        /// <param name="modeElem"    ><p>モード - 要素</p>
        ///                               <p>11 = 外壁</p>
        ///                               <p>12 = 内壁</p>
        ///                               <p>21 = 柱</p>
        ///                               <p>31 = 大梁</p>
        ///                               <p>32 = 小梁</p>
        ///                               <p>33 = 鉛直ブレース</p>
        ///                               <p>34 = 水平ブレース</p>
        ///                               <p>35 = その他</p>
        ///                               <p>41 = スラブ</p>
        ///                               <p>51 = 基礎</p></param>
        /// <param name="elemIdsWork" >要素ID - 対象</param>
        /// <param name="elemIdsExcl" >要素ID - 除外</param>
        ///
        /// <history><p>2011/12/07 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2021/10/12 Modified Applied Technology</p></history>
        /// ================================================================================
        public
        void GetElemTag(Document doc, BuiltInCategory builtInCategory,
                      string markVal,
                      string levelVale,
                      int modeMark,
                      int modeElem,
                      ref IList<ElementId> elemIdsWork,
                      ref IList<ElementId> elemIdsExcl)
        {
            // 初期化
            string sValue = "";
            double dValue1 = 0.0;
            double dValue2 = 0.0;

            IList<Element> elems = null;
            IndependentTag idpdntTag = null;

            // 符号
            IList<Element> markElems = new List<Element>();
            IList<int> idMarkLocElems = new List<int>();

            if ((modeMark == 1) || (modeMark == 2))
            {
                if (!string.IsNullOrEmpty(markVal))
                {
                    // タグ要素取得
                    elems = _CmpElements.GetElemTag(doc, builtInCategory);
                    foreach (Element elem in elems)
                    {
                        idpdntTag = elem as IndependentTag;
                        if (idpdntTag != null)
                        {
                            // Check element tag by modeElem
                            if (BoolElemTag(elem, modeElem) == true)
                            {
                                sValue = idpdntTag.TagText;
                                if (sValue.Trim().ToLower() == markVal.Trim().ToLower())
                                {
                                    elemIdsWork.Add(elem.Id);
                                }
                                else
                                {
                                    elemIdsExcl.Add(elem.Id);
                                }
                            }
                        }
                    }
                }
            }

            // レベル
            if (modeMark == 2)
            {
                if (!string.IsNullOrEmpty(levelVale))
                {
                    // タグ要素取得
                    elems = _CmpElements.GetElemTag(doc, builtInCategory);
                    foreach (Element elem in elems)
                    {
                        idpdntTag = elem as IndependentTag;
                        if (idpdntTag != null)
                        {
                            if (BoolElemTag(elem, modeElem) == true)
                            {
                                int iFlag = 0;
                                dValue1 = 0.0;
                                sValue = idpdntTag.TagText;
                                if (ConvNumTagText(sValue, ref dValue1) == true)
                                {
                                    iFlag++;
                                }

                                dValue2 = 0.0;
                                if (JExtComCompat.UtilValue.IsNumber(levelVale) == true)
                                {
                                    dValue2 = double.Parse(levelVale);
                                    iFlag++;
                                }

                                bool bFlag = false;
                                if (iFlag == 2)
                                {
                                    if (dValue1 == dValue2)
                                    {
                                        bFlag = true;
                                    }
                                }
                                if (bFlag == true)
                                {
                                    elemIdsWork.Add(elem.Id);
                                }
                                else
                                {
                                    elemIdsExcl.Add(elem.Id);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>タグデータ取得</summary>
        ///
        /// <param name="markVal"     >符号 - 値</param>
        /// <param name="levelVale"   >レベル - 値</param>
        /// <param name="mode"        ><p>モード</p>
        ///                               <p>1 = 符号のみ</p>
        ///                               <p>2 = 符号、レベル</p></param>
        /// <param name="strMark"     >文字列 - 符号</param>
        /// <param name="strLevel"    >文字列 - レベル</param>
        ///
        /// <returns>タグ要素</returns>
        ///
        /// <history><p>2011/12/07 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2021/10/12 Modified  Applied Technology</p></history>
        /// ================================================================================
        public
        void GetElemTagData(string markVal,
                            string levelVale,
                            int mode,
                            ref string strMark,
                            ref string strLevel)
        {
            // 戻り値
            strMark = null;
            strLevel = null;

            // 初期化
            string sValue = "";

            // 符号
            if ((mode == 1) || (mode == 2))
            {
                sValue = markVal;
                if (sValue == null)
                {
                    sValue = "";
                }
                if (sValue != "")
                {
                    strMark = sValue;
                }
            }

            // レベル
            if (mode == 2)
            {
                if (strMark != null)
                {
                    if (JExtComCompat.UtilValue.IsNumber(levelVale) == true)
                    {
                        string sign = "";
                        double dValue = double.Parse(levelVale);
                        if (dValue == 0.0)
                        {
                            sign = "±";
                        }
                        else if (dValue > 0.0)
                        {
                            sign = "+";
                        }

                        strLevel = sign + levelVale.ToString();
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>タグ文字の数値変換</summary>
        ///
        /// <param name="tagText"   >タグ文字</param>
        /// <param name="tagNumber" >タグ数値</param>
        ///
        /// <returns>結果</returns>
        ///
        /// <history>2011/12/07 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool ConvNumTagText(string tagText, ref double tagNumber)
        {
            // 戻り値
            bool ret = false;

            // 初期化
            string sValue = "";

            // タグ文字確認
            if (tagText == null)
            {
                return ret;
            }
            if (tagText == "")
            {
                return ret;
            }

            // 数値部開始位置
            int idxS = -1;
            for (int i = 0; i < tagText.Length; ++i)
            {
                sValue = tagText.Substring(i, 1);
                if (sValue == "-")
                {
                    idxS = i;
                    break;
                }
                if (sValue == ".")
                {
                    idxS = i;
                    break;
                }
                if (JExtComCompat.UtilValue.IsInteger(sValue) == true)
                {
                    idxS = i;
                    break;
                }
            }
            if (idxS == -1)
            {
                return ret;
            }
            string strF = tagText.Substring(idxS, 1);
            idxS++;

            // 数値部
            string strB = "";
            if (idxS < tagText.Length)
            {
                for (int i = idxS; i < tagText.Length; ++i)
                {
                    sValue = tagText.Substring(i, 1);
                    if (sValue == ".")
                    {
                        strB += sValue;
                    }
                    else if (JExtComCompat.UtilValue.IsInteger(sValue) == true)
                    {
                        strB += sValue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            sValue = strF + strB;
            if (JExtComCompat.UtilValue.IsNumber(sValue) == true)
            {
                tagNumber = double.Parse(sValue);
            }

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>対象タグ要素取得</summary>
        ///
        /// <param name="entDtWallTag"      >データテーブル - 壁タグ</param>
        /// <param name="entDtColumnTag"    >データテーブル - 柱タグ</param>
        /// <param name="entDtBeamTag"      >データテーブル - 梁タグ</param>
        /// <param name="entDtSlabTag"      >データテーブル - スラブタグ</param>
        /// <param name="entDtFoundationTag">データテーブル - 基礎タグ</param>
        /// <param name="elemIdsWork"       >要素ID - 対象</param>
        /// <param name="elemIdsExcl"       >要素ID - 除外</param>
        ///
        /// <history><p>2011/12/07 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p>
        ///          <p>2021/10/12 Modified Applied Technology</p></history>
        /// ================================================================================
        public
        void GetWorkTag(RvtExtApp.Entities.Exclusion.DtWallTag entDtWallTag,
                        RvtExtApp.Entities.Exclusion.DtColumnTag entDtColumnTag,
                        RvtExtApp.Entities.Exclusion.DtBeamTag entDtBeamTag,
                        RvtExtApp.Entities.Exclusion.DtSlabTag entDtSlabTag,
                        RvtExtApp.Entities.Exclusion.DtFoundationTag entDtFoundationTag,
                        ref IList<ElementId> elemIdsWork,
                        ref IList<ElementId> elemIdsExcl)
        {
            // 戻り値
            if (elemIdsWork == null)
            {
                elemIdsWork = new List<ElementId>();
            }
            if (elemIdsExcl == null)
            {
                elemIdsExcl = new List<ElementId>();
            }

            _ErrMsg = "";

            // 初期化

            string markVal = "";

            string levelVal = "";

            // 外壁

            markVal = entDtWallTag.WallExtMarkVal;

            levelVal = entDtWallTag.WallExtLevelVal;

            GetElemTag(_Doc, BuiltInCategory.OST_WallTags,
                       markVal,
                       levelVal,
                       1,
                       11,
                       ref elemIdsWork,
                       ref elemIdsExcl);

            // 内壁

            markVal = entDtWallTag.WallIntMarkVal;

            levelVal = entDtWallTag.WallIntLevelVal;

            GetElemTag(_Doc, BuiltInCategory.OST_WallTags,
                       markVal,
                       levelVal,
                       1,
                       12,
                       ref elemIdsWork,
                       ref elemIdsExcl);

            // 柱

            markVal = entDtColumnTag.ColumnMarkVal;

            levelVal = entDtColumnTag.ColumnLevelVal;

            GetElemTag(_Doc, BuiltInCategory.OST_StructuralColumnTags,
                       markVal,
                       levelVal,
                       1,
                       21,
                       ref elemIdsWork,
                       ref elemIdsExcl);

            // 大梁

            markVal = entDtBeamTag.GirderMarkVal;

            levelVal = entDtBeamTag.GirderLevelVal;

            GetElemTag(_Doc, BuiltInCategory.OST_StructuralFramingTags,
                       markVal,
                       levelVal,
                       2,
                       31,
                       ref elemIdsWork,
                       ref elemIdsExcl);

            // 小梁

            markVal = entDtBeamTag.BeamMarkVal;

            levelVal = entDtBeamTag.BeamLevelVal;

            GetElemTag(_Doc, BuiltInCategory.OST_StructuralFramingTags,
                       markVal,
                       levelVal,
                       2,
                       32,
                       ref elemIdsWork,
                       ref elemIdsExcl);

            // 鉛直ブレース

            markVal = entDtBeamTag.VbraceMarkVal;

            levelVal = entDtBeamTag.VbraceLevelVal;

            GetElemTag(_Doc, BuiltInCategory.OST_StructuralFramingTags,
                       markVal,
                       levelVal,
                       1,
                       33,
                       ref elemIdsWork,
                       ref elemIdsExcl);

            // 水平ブレース

            markVal = entDtBeamTag.HbraceMarkVal;

            levelVal = entDtBeamTag.HbraceLevelVal;

            GetElemTag(_Doc, BuiltInCategory.OST_StructuralFramingTags,
                       markVal,
                       levelVal,
                       2,
                       34,
                       ref elemIdsWork,
                       ref elemIdsExcl);

            // その他

            markVal = entDtBeamTag.OtherMarkVal;

            levelVal = entDtBeamTag.OtherLevelVal;

            GetElemTag(_Doc, BuiltInCategory.OST_StructuralFramingTags,
                       markVal,
                       levelVal,
                       2,
                       35,
                       ref elemIdsWork,
                       ref elemIdsExcl);

            // スラブ

            markVal = entDtSlabTag.SlabMarkVal;

            levelVal = entDtSlabTag.SlabLevelVal;

            GetElemTag(_Doc, BuiltInCategory.OST_FloorTags,
                       markVal,
                       levelVal,
                       2,
                       41,
                       ref elemIdsWork,
                       ref elemIdsExcl);

            // 基礎

            markVal = entDtFoundationTag.FoundationMarkVal;

            levelVal = entDtFoundationTag.FoundationLevelVal;

            GetElemTag(_Doc, BuiltInCategory.OST_StructuralFoundationTags,
                       markVal,
                       levelVal,
                       2,
                       51,
                       ref elemIdsWork,
                       ref elemIdsExcl);
        }

        /// ================================================================================
        /// <summary>ビュー処理</summary>
        ///
        /// <param name="mode"              ><p>モード</p>
        ///                                     <p>1 = 表示</p>
        ///                                     <p>2 = 非表示</p></param>
        /// <param name="activeView"        >アクティブビュー</param>
        /// <param name="entDtWallTag"      >データテーブル - 壁タグ</param>
        /// <param name="entDtColumnTag"    >データテーブル - 柱タグ</param>
        /// <param name="entDtBeamTag"      >データテーブル - 梁タグ</param>
        /// <param name="entDtSlabTag"      >データテーブル - スラブタグ</param>
        /// <param name="entDtFoundationTag">データテーブル - 基礎タグ</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/12/07 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        bool WorkView(int mode,
                      View activeView,
                      RvtExtApp.Entities.Exclusion.DtWallTag entDtWallTag,
                      RvtExtApp.Entities.Exclusion.DtColumnTag entDtColumnTag,
                      RvtExtApp.Entities.Exclusion.DtBeamTag entDtBeamTag,
                      RvtExtApp.Entities.Exclusion.DtSlabTag entDtSlabTag,
                      RvtExtApp.Entities.Exclusion.DtFoundationTag entDtFoundationTag)
        {
            // 戻り値
            bool ret = false;
            _ErrMsg = "";

            // 要素
            IList<ElementId> elemIdsWork = new List<ElementId>();
            IList<ElementId> elemIdsExcl = new List<ElementId>();
            GetWorkTag(entDtWallTag,
                       entDtColumnTag,
                       entDtBeamTag,
                       entDtSlabTag,
                       entDtFoundationTag,
                       ref elemIdsWork,
                       ref elemIdsExcl);

            // ビュー設定
            switch (mode)
            {
                // 表示
                case 1:
                    if (elemIdsExcl.Count > 0)
                    {
                        activeView.UnhideElements(elemIdsExcl);
                    }
                    if (elemIdsWork.Count > 0)
                    {
                        activeView.UnhideElements(elemIdsWork);
                    }
                    break;

                // 非表示
                case 2:
                    if (elemIdsExcl.Count > 0)
                    {
                        activeView.UnhideElements(elemIdsExcl);
                    }
                    if (elemIdsWork.Count > 0)
                    {
                        activeView.HideElements(elemIdsWork);
                    }
                    break;
            }

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>書出し処理</summary>
        ///
        /// <param name="activeView"        >アクティブビュー</param>
        /// <param name="entDtWallTag"      >データテーブル - 壁タグ</param>
        /// <param name="entDtColumnTag"    >データテーブル - 柱タグ</param>
        /// <param name="entDtBeamTag"      >データテーブル - 梁タグ</param>
        /// <param name="entDtSlabTag"      >データテーブル - スラブタグ</param>
        /// <param name="entDtFoundationTag">データテーブル - 基礎タグ</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/12/07 Created  GSA,Inc. Shinichi Ishii</p>
        ///           <p>2014/07/23 Modified GSA,Inc. Ryo Kuroda</p>
        ///           <p>2015/05/13 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        bool WorkOutput(View activeView,
                        RvtExtApp.Entities.Exclusion.DtWallTag entDtWallTag,
                        RvtExtApp.Entities.Exclusion.DtColumnTag entDtColumnTag,
                        RvtExtApp.Entities.Exclusion.DtBeamTag entDtBeamTag,
                        RvtExtApp.Entities.Exclusion.DtSlabTag entDtSlabTag,
                        RvtExtApp.Entities.Exclusion.DtFoundationTag entDtFoundationTag)
        {
            // 戻り値
            bool ret = false;
            _ErrMsg = "";

            // 初期化
            string sValue = "";
            string strMark = null;
            string strLevel = null;
            int scale = 1;

            // データ
            IList<string> itemAry = new List<string>();
            IList<string> markAry = new List<string>();
            IList<string> levelAry = new List<string>();

            // 大梁 - タグデータ
            strMark = null;
            strLevel = null;
            GetElemTagData(entDtBeamTag.GirderMarkVal,
                           entDtBeamTag.GirderLevelVal,
                           2,
                           ref strMark,
                           ref strLevel);

            if (strMark != null)
            {
                itemAry.Add(_CmpAttribute.ResourceText("IDS_TXT_GIRDER"));
                markAry.Add(strMark);
                levelAry.Add(strLevel);
            }

            // 小梁 - タグデータ
            strMark = null;
            strLevel = null;
            GetElemTagData(entDtBeamTag.BeamMarkVal,
                           entDtBeamTag.BeamLevelVal,
                           2,
                           ref strMark,
                           ref strLevel);

            if (strMark != null)
            {
                itemAry.Add(_CmpAttribute.ResourceText("IDS_TXT_BEAM"));
                markAry.Add(strMark);
                levelAry.Add(strLevel);
            }

            // 水平ブレース - タグデータ
            strMark = null;
            strLevel = null;
            GetElemTagData(entDtBeamTag.HbraceMarkVal,
                           entDtBeamTag.HbraceLevelVal,
                           2,
                           ref strMark,
                           ref strLevel);

            if (strMark != null)
            {
                itemAry.Add(_CmpAttribute.ResourceText("IDS_TXT_HBRACE"));
                markAry.Add(strMark);
                levelAry.Add(strLevel);
            }

            // 鉛直ブレース - タグデータ
            strMark = null;
            strLevel = null;
            GetElemTagData(entDtBeamTag.VbraceMarkVal,
                           entDtBeamTag.VbraceLevelVal,
                           1,
                           ref strMark,
                           ref strLevel);

            if (strMark != null)
            {
                itemAry.Add(_CmpAttribute.ResourceText("IDS_TXT_VBRACE"));
                markAry.Add(strMark);
                levelAry.Add(strLevel);
            }

            // その他 - タグデータ
            strMark = null;
            strLevel = null;
            GetElemTagData(entDtBeamTag.OtherMarkVal,
                           entDtBeamTag.OtherLevelVal,
                           2,
                           ref strMark,
                           ref strLevel);

            if (strMark != null)
            {
                itemAry.Add(_CmpAttribute.ResourceText("IDS_TXT_OTHER"));
                markAry.Add(strMark);
                levelAry.Add(strLevel);
            }

            // 外壁 - タグデータ
            strMark = null;
            strLevel = null;
            GetElemTagData(entDtWallTag.WallExtMarkVal,
                           entDtWallTag.WallExtLevelVal,
                           1,
                           ref strMark,
                           ref strLevel);

            if (strMark != null)
            {
                itemAry.Add(_CmpAttribute.ResourceText("IDS_TXT_EXTWALL"));
                markAry.Add(strMark);
                levelAry.Add(strLevel);
            }

            // 内壁 - タグデータ
            strMark = null;
            strLevel = null;
            GetElemTagData(entDtWallTag.WallIntMarkVal,
                           entDtWallTag.WallIntLevelVal,
                           1,
                           ref strMark,
                           ref strLevel);

            if (strMark != null)
            {
                itemAry.Add(_CmpAttribute.ResourceText("IDS_TXT_INTWALL"));
                markAry.Add(strMark);
                levelAry.Add(strLevel);
            }

            // スラブ - タグデータ
            strMark = null;
            strLevel = null;
            GetElemTagData(entDtSlabTag.SlabMarkVal,
                           entDtSlabTag.SlabLevelVal,
                           2,
                           ref strMark,
                           ref strLevel);

            if (strMark != null)
            {
                itemAry.Add(_CmpAttribute.ResourceText("IDS_TXT_SLAB"));
                markAry.Add(strMark);
                levelAry.Add(strLevel);
            }

            // 柱 - タグデータ
            strMark = null;
            strLevel = null;
            GetElemTagData(entDtColumnTag.ColumnMarkVal,
                           entDtColumnTag.ColumnLevelVal,
                           1,
                           ref strMark,
                           ref strLevel);

            if (strMark != null)
            {
                itemAry.Add(_CmpAttribute.ResourceText("IDS_TXT_COLUMN"));
                markAry.Add(strMark);
                levelAry.Add(strLevel);
            }

            // 基礎 - タグデータ
            strMark = null;
            strLevel = null;
            GetElemTagData(entDtFoundationTag.FoundationMarkVal,
                           entDtFoundationTag.FoundationLevelVal,
                           2,
                           ref strMark,
                           ref strLevel);

            if (strMark != null)
            {
                itemAry.Add(_CmpAttribute.ResourceText("IDS_TXT_FOUNDATION"));
                markAry.Add(strMark);
                levelAry.Add(strLevel);
            }

            // タグデータ数
            if (itemAry.Count == 0)
            {
                _ErrMsg = _CmpAttribute.ResourceText("IDS_ERR_NOTTAGDATA");
                return ret;
            }

            // 製図ビュー名
            string viewDraftName = _CmpAttribute.ResourceText("IDS_VIEW_EXCLUSIONSPECIALMENTIONITEM") + "_" +
                                   activeView.Name;

            ViewDrafting viewDraft = _CmpElements.GetViewDrafting(viewDraftName);
            if (viewDraft == null)
            {
                // 製図ビュー作成
                ViewFamilyType viewFamilytype = null;

                ElementId typeId = activeView.GetTypeId();
                if (typeId != null)
                {
                    viewFamilytype = _CmpElements.GetElementDoc(Int32.Parse(typeId.ToString())) as ViewFamilyType;
                }

                viewDraft = _CmpElements.CreateViewDrafting(viewDraftName, viewFamilytype, scale);
            }
            else
            {
                // 製図ビューの要素削除
                viewDraft.Scale = scale;
                _CmpElements.DelElemsView(viewDraft);
            }

            // 最大文字数
            int maxLenItem = 0;
            int maxLenMark = 0;
            int maxLenLevel = 0;

            for (int i = 0; i < itemAry.Count; ++i)
            {
                sValue = itemAry[i];
                if (sValue != null)
                {
                    int lenItem = JExtComCompat.UtilValue.GetByteCountString(itemAry[i]);
                    if (lenItem > maxLenItem)
                    {
                        maxLenItem = lenItem;
                    }
                }

                sValue = markAry[i];
                if (sValue != null)
                {
                    int lenMark = JExtComCompat.UtilValue.GetByteCountString(sValue);
                    if (lenMark > maxLenMark)
                    {
                        maxLenMark = lenMark;
                    }
                }

                sValue = levelAry[i];
                if (sValue != null)
                {
                    int lenLevel = JExtComCompat.UtilValue.GetByteCountString(sValue);
                    if (lenLevel > maxLenLevel)
                    {
                        maxLenLevel = lenLevel;
                    }
                }
            }

            // 文字列初期化
            HorizontalTextAlignment alignType = HorizontalTextAlignment.Left;

            TextNote elemText;

            Curve crv;
            CurveElement crvElem;

            double nX = 0.0;
            double nY = 0.0;
            double nyD = 5.0 / _CmpGeometry.UnitCoe;
            double kX1 = 0.0;
            double kY1 = 0.0;
            double kX2 = 100.0 / _CmpGeometry.UnitCoe;
            double kY2 = 0.0;
            double kyD = 2.0 / _CmpGeometry.UnitCoe;
            XYZ pos1;
            XYZ pos2;

            // タイトル
            nY = 0.0;
            pos1 = new XYZ(nX, nY, 0.0);
            string title = _CmpAttribute.ResourceText("IDS_TXT_TITLE_EXCLUSIONSPECIALMENTION");
            elemText = _CmpElements.CreateTextNotePosRotate(viewDraft, pos1, alignType, title, 0, 1);

            // 罫線
            kY1 = nY - kyD;
            kY2 = kY1;
            pos1 = new XYZ(kX1, kY1, 0.0);
            pos2 = new XYZ(kX2, kY1, 0.0);
            crv = Line.CreateBound(pos1, pos2);
            crvElem = _CmpElements.CreateDetailCurve(viewDraft, crv);

            for (int i = 0; i < itemAry.Count; ++i)
            {
                string srtItem = itemAry[i];
                strMark = markAry[i];
                strLevel = levelAry[i];

                int blkItem = 0;
                if (srtItem != null)
                {
                    int lenItem = JExtComCompat.UtilValue.GetByteCountString(srtItem);
                    blkItem = maxLenItem - lenItem;
                }

                int blkMark = 0;
                if (strMark != null)
                {
                    int lenMark = JExtComCompat.UtilValue.GetByteCountString(strMark);
                    blkMark = maxLenMark - lenMark;
                }

                int blkLevel = 0;
                if (strLevel != null)
                {
                    int lenLevel = JExtComCompat.UtilValue.GetByteCountString(strLevel);
                    blkLevel = maxLenLevel - lenLevel;
                }
                string blankStrItem = JExtComCompat.UtilValue.CreateBlankString(blkItem);
                string blankStrMark = JExtComCompat.UtilValue.CreateBlankString(blkMark);
                string blankStrLevel = JExtComCompat.UtilValue.CreateBlankString(blkLevel);

                nY -= nyD;
                pos1 = new XYZ(nX, nY, 0.0);

                // 文字
                sValue = "・" + srtItem + blankStrItem;
                if (strMark != null)
                {
                    sValue += " " + strMark + blankStrMark;
                }
                if (strLevel != null)
                {
                    sValue += " " + strLevel;
                }

                elemText = _CmpElements.CreateTextNotePosRotate(viewDraft, pos1, alignType, sValue, 0, 1);

                // 罫線
                kY1 = nY - kyD;
                kY2 = kY1;
                pos1 = new XYZ(kX1, kY1, 0.0);
                pos2 = new XYZ(kX2, kY1, 0.0);
                crv = Line.CreateBound(pos1, pos2);
                crvElem = _CmpElements.CreateDetailCurve(viewDraft, crv);
            }

            ret = true;
            return ret;
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>エラーメッセージ</summary>
        /// <history>2011/12/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ErrMsg
        {
            get
            {
                return _ErrMsg;
            }
        }

        #endregion Properties
    }
}
