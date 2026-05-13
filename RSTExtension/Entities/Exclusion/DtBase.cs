using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Entities.Exclusion
{
    /// ================================================================================
    /// <summary>データテーブル - 基底</summary>
    /// ================================================================================
    public abstract class DtBase
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

        /// <summary>列名 ID</summary>
        private string _ColNameID;

        /// <summary>列名 名称</summary>
        private string _ColNameName;

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
        /// <history>2011/12/01 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected DtBase(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.ESM_Elements cmpElements,
                         RvtExtApp.Components.ESM_Geometry cmpGeometry,
                         RvtExtApp.Components.ESM_Parameters cmpParameters,
                         RvtExtApp.Components.ESM_Settings cmpSettings)
        {
            // 初期化
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
        /// <summary>壁取得</summary>
        ///
        /// <param name="wallExtElemAry">外壁</param>
        /// <param name="wallIntElemAry">内壁</param>
        /// <param name="wallExtIdAry"  >外壁ID</param>
        /// <param name="wallIntIdAry"  >内壁ID</param>
        ///
        /// <history><p>2011/12/05 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        void GetWalls(ref IList<Element> wallExtElemAry,
                      ref IList<Element> wallIntElemAry,
                      ref IList<string> wallExtIdAry,
                      ref IList<string> wallIntIdAry)
        {
            // 戻り値
            if (wallExtElemAry == null)
            {
                wallExtElemAry = new List<Element>();
            }
            if (wallIntElemAry == null)
            {
                wallIntElemAry = new List<Element>();
            }

            // 要素
            IList<System.Type> sysTypes = new List<System.Type>();
            sysTypes.Add(typeof(Wall));

            IList<Element> elems = _CmpElements.GetElementsDoc(null,
                                                                                            sysTypes,
                                                                                            null,
                                                                                            null,
                                                                                            null);

            foreach (Element elem in elems)
            {
                // 要素タイプ
                Element elemType = _CmpElements.GetElementType(elem);
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
                            wallIntElemAry.Add(elem);
                            if (wallIntIdAry != null)
                            {
                                wallIntIdAry.Add(elem.Id.ToString());
                            }
                            break;

                        //外壁
                        case 1:
                            wallExtElemAry.Add(elem);
                            if (wallExtIdAry != null)
                            {
                                wallExtIdAry.Add(elem.Id.ToString());
                            }
                            break;
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>柱取得</summary>
        ///
        /// <param name="columnElemAry" >柱</param>
        /// <param name="columnIdAry"   >柱ID</param>
        ///
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetColumns(ref IList<Element> columnElemAry,
                        ref IList<string> columnIdAry)
        {
            // 戻り値
            if (columnElemAry == null)
            {
                columnElemAry = new List<Element>();
            }

            // 要素
            IList<System.Type> sysTypes = new List<System.Type>();
            sysTypes.Add(typeof(FamilyInstance));

            IList<Category> categories = new List<Category>();
            categories.Add(_CmpElements.GetCategory(BuiltInCategory.OST_Columns));
            categories.Add(_CmpElements.GetCategory(BuiltInCategory.OST_StructuralColumns));

            IList<Element> elems = _CmpElements.GetElementsDoc(null,
                                                                                            sysTypes,
                                                                                            categories,
                                                                                            null,
                                                                                            null);
            foreach (Element elem in elems)
            {
                columnElemAry.Add(elem);
                if (columnIdAry != null)
                {
                    columnIdAry.Add(elem.Id.ToString());
                }
            }
        }

        /// ================================================================================
        /// <summary>梁取得</summary>
        ///
        /// <param name="girderElemAry" >大梁</param>
        /// <param name="beamElemAry"   >小梁</param>
        /// <param name="vbraceElemAry" >鉛直ブレース</param>
        /// <param name="hbraceElemAry" >水平ブレース</param>
        /// <param name="otherElemAry"  >その他</param>
        /// <param name="girderIdAry"   >大梁ID</param>
        /// <param name="beamIdAry"     >小梁ID</param>
        /// <param name="vbraceIdAry"   >鉛直ブレースID</param>
        /// <param name="hbraceIdAry"   >水平ブレースID</param>
        /// <param name="otherIdAry"    >その他ID</param>
        ///
        /// <history><p>2011/12/05 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        void GetBeams(ref IList<Element> girderElemAry,
                      ref IList<Element> beamElemAry,
                      ref IList<Element> vbraceElemAry,
                      ref IList<Element> hbraceElemAry,
                      ref IList<Element> otherElemAry,
                      ref IList<string> girderIdAry,
                      ref IList<string> beamIdAry,
                      ref IList<string> vbraceIdAry,
                      ref IList<string> hbraceIdAry,
                      ref IList<string> otherIdAry)
        {
            // 戻り値
            if (girderElemAry == null)
            {
                girderElemAry = new List<Element>();
            }

            if (beamElemAry == null)
            {
                beamElemAry = new List<Element>();
            }

            if (vbraceElemAry == null)
            {
                vbraceElemAry = new List<Element>();
            }

            if (hbraceElemAry == null)
            {
                hbraceElemAry = new List<Element>();
            }

            if (otherElemAry == null)
            {
                otherElemAry = new List<Element>();
            }

            // 要素
            IList<System.Type> sysTypes = new List<System.Type>();
            sysTypes.Add(typeof(FamilyInstance));

            IList<Category> categories = new List<Category>();
            categories.Add(_CmpElements.GetCategory(BuiltInCategory.OST_StructuralFraming));

            IList<Element> elems = _CmpElements.GetElementsDoc(null,
                                                                                            sysTypes,
                                                                                            categories,
                                                                                            null,
                                                                                            null);

            foreach (Element elem in elems)
            {
                // 構造用途
                int usage = 0;
                if (_CmpParameters.GetValue(elem,
                                            BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM,
                                            ref usage) < -1)
                {
                }

                switch (usage)
                {
                    // 大梁
                    case 3:
                        girderElemAry.Add(elem);
                        if (girderIdAry != null)
                        {
                            girderIdAry.Add(elem.Id.ToString());
                        }
                        break;

                    // 小梁
                    case 4:
                        beamElemAry.Add(elem);
                        if (beamIdAry != null)
                        {
                            beamIdAry.Add(elem.Id.ToString());
                        }
                        break;

                    // その他
                    case 6:
                        otherElemAry.Add(elem);
                        if (otherIdAry != null)
                        {
                            otherIdAry.Add(elem.Id.ToString());
                        }
                        break;

                    // 鉛直ブレース
                    case 7:
                        vbraceElemAry.Add(elem);
                        if (vbraceIdAry != null)
                        {
                            vbraceIdAry.Add(elem.Id.ToString());
                        }
                        break;

                    // 水平ブレース
                    case 8:
                        hbraceElemAry.Add(elem);
                        if (hbraceIdAry != null)
                        {
                            hbraceIdAry.Add(elem.Id.ToString());
                        }
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary>スラブ取得</summary>
        ///
        /// <param name="slabElemAry" >スラブ</param>
        /// <param name="slabIdAry"   >スラブID</param>
        ///
        /// <history><p>2011/12/05 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        void GetSlabs(ref IList<Element> slabElemAry,
                      ref IList<string> slabIdAry)
        {
            // 戻り値
            if (slabElemAry == null)
            {
                slabElemAry = new List<Element>();
            }

            // 要素
            IList<System.Type> sysTypes = new List<System.Type>();
            sysTypes.Add(typeof(Floor));

            IList<Category> categories = new List<Category>();
            categories.Add(_CmpElements.GetCategory(BuiltInCategory.OST_Floors));

            IList<Element> elems = _CmpElements.GetElementsDoc(null,
                                                                                            sysTypes,
                                                                                            categories,
                                                                                            null,
                                                                                            null);

            foreach (Element elem in elems)
            {
                // 構造用途
                bool isStruct = false;
                if (_CmpParameters.GetValue(elem,
                                            BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL,
                                            ref isStruct) < -1)
                {
                }
                if (isStruct == true)
                {
                    slabElemAry.Add(elem);
                    if (slabIdAry != null)
                    {
                        slabIdAry.Add(elem.Id.ToString());
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>基礎取得</summary>
        ///
        /// <param name="foundationElemAry" >基礎</param>
        /// <param name="foundationIdAry"   >基礎ID</param>
        ///
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetFoundations(ref IList<Element> foundationElemAry,
                            ref IList<string> foundationIdAry)
        {
            // 戻り値
            if (foundationElemAry == null)
            {
                foundationElemAry = new List<Element>();
            }

            IList<Category> categories = new List<Category>();
            categories.Add(_CmpElements.GetCategory(BuiltInCategory.OST_StructuralFoundation));

            IList<Element> elems = _CmpElements.GetElementsDoc(null,
                                                                                            null,
                                                                                            categories,
                                                                                            null,
                                                                                            null);

            foreach (Element elem in elems)
            {
                foundationElemAry.Add(elem);
                if (foundationIdAry != null)
                {
                    foundationIdAry.Add(elem.Id.ToString());
                }
            }
        }

        /// ================================================================================
        /// <summary>ファミリタイプ名取得</summary>
        ///
        /// <param name="elemType">要素タイプ</param>
        ///
        /// <returns>ファミリタイプ名</returns>
        ///
        /// <history><p>2011/12/05 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string GetFamilyTypeName(Element elemType)
        {
            // 戻り値
            string ret = "";
            string sValue = "";

            if (_CmpParameters.GetValue(elemType,
                                        BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM,
                                        ref sValue) < -1)
            {
            }
            if (sValue != null)
            {
                ret = sValue;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>なし項目設定</summary>
        ///
        /// <param name="dt">データテーブル</param>
        ///
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void SetItemNothing(System.Data.DataTable dt)
        {
            // 行データ
            System.Data.DataRow row = dt.NewRow();

            // ID
            row[ColNameID] = 0;

            // 名称
            row[ColNameName] = _CmpAttribute.ResourceText("IDS_TXT_NOTHING");

            dt.Rows.Add(row);
        }

        /// ================================================================================
        /// <summary>コンボボックス幅調整</summary>
        ///
        /// <param name="cbo">コンボボックス</param>
        ///
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void SetCboWidth(System.Windows.Forms.ComboBox cbo)
        {
            // 初期化
            int maxSize = 0;

            // コンボボックス文字サイズ
            foreach (System.Object item in cbo.Items)
            {
                string sItem = item as string;
                if (sItem != null)
                {
                    maxSize = System.Math.Max(maxSize, System.Windows.Forms.TextRenderer.MeasureText(sItem, cbo.Font).Width);
                }
            }

            // スクロールバー幅
            maxSize += 15;

            // 比較
            if (cbo.DropDownWidth < maxSize)
            {
                cbo.DropDownWidth = maxSize;
            }
        }

        /// ================================================================================
        /// <summary>データ取得 - 文字列</summary>
        ///
        /// <param name="dataStr"   >文字列データ</param>
        /// <param name="itemNum"   >項目数</param>
        /// <param name="dataStrAry">文字列データ配列</param>
        ///
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetDataString(string dataStr,
                           int itemNum,
                           ref IList<string> dataStrAry)
        {
            // 初期化
            IList<string> valueSplit;

            // データ取得
            dataStrAry = new List<string>();

            // データ分割
            valueSplit = JExtComCompat.UtilValue.SplitString(dataStr, ",");

            bool flag = false;
            if (itemNum == valueSplit.Count)
            {
                flag = true;
            }

            // 値取得
            if (itemNum > 0)
            {
                for (int i = 0; i < itemNum; ++i)
                {
                    if (flag == true)
                    {
                        dataStrAry.Add(valueSplit[i]);
                    }
                    else
                    {
                        dataStrAry.Add("");
                    }
                }
            }
            else
            {
                if (valueSplit.Count > 0)
                {
                    for (int i = 0; i < valueSplit.Count; ++i)
                    {
                        dataStrAry.Add(valueSplit[i]);
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>データ取得 - 文字列</summary>
        ///
        /// <param name="dataStrAry">文字列データ配列</param>
        /// <param name="dataStr"   >文字列データ</param>
        ///
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetDataString(IList<string> dataStrAry,
                           ref string dataStr)
        {
            dataStr = null;
            string separator = ",";

            // 値取得
            if (dataStrAry != null)
            {
                foreach (string str in dataStrAry)
                {
                    dataStr += str + separator;
                }
            }

            if (dataStr != null)
            {
                dataStr = dataStr.Substring(0, dataStr.Length - 1);
            }
        }

        /// ================================================================================
        /// <summary>要素取得</summary>
        ///
        /// <param name="idStr" >ID文字列</param>
        /// <param name="elem"  >要素</param>
        ///
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetElem(string idStr, ref Element elem)
        {
            elem = null;
            int iValue = int.Parse(idStr);
            if (iValue != 0)
            {
                elem = CmpElements.GetElementDoc(iValue);
            }
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>属性</summary>
        /// <history>2015/12/14 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected
        RvtExtApp.Components.Attribute CmpAttribute
        {
            get
            {
                return _CmpAttribute;
            }
        }

        /// ================================================================================
        /// <summary>要素</summary>
        /// <history>2011/12/01 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected
        RvtExtApp.Components.ESM_Elements CmpElements
        {
            get
            {
                return _CmpElements;
            }
        }

        /// ================================================================================
        /// <summary>図形</summary>
        /// <history>2011/12/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected
        RvtExtApp.Components.ESM_Geometry CmpGeometry
        {
            get
            {
                return _CmpGeometry;
            }
        }

        /// ================================================================================
        /// <summary>パラメーター</summary>
        /// <history>2011/12/01 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected
        RvtExtApp.Components.ESM_Parameters CmpParameters
        {
            get
            {
                return _CmpParameters;
            }
        }

        /// ================================================================================
        /// <summary>設定</summary>
        /// <history>2011/12/01 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected
        RvtExtApp.Components.ESM_Settings CmpSettings
        {
            get
            {
                return _CmpSettings;
            }
        }

        /// ================================================================================
        /// <summary>エラーメッセージ</summary>
        /// <history>2011/12/01 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string ErrMsg
        {
            get
            {
                return _ErrMsg;
            }
            set
            {
                _ErrMsg = value;
            }
        }

        /// ================================================================================
        /// <summary>列名 ID</summary>
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string ColNameID
        {
            get
            {
                if (_ColNameID == null)
                {
                    _ColNameID = _CmpAttribute.ResourceText("IDS_COLNAME_ID");
                }
                return _ColNameID;
            }
        }

        /// ================================================================================
        /// <summary>列名 名称</summary>
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string ColNameName
        {
            get
            {
                if (_ColNameName == null)
                {
                    _ColNameName = _CmpAttribute.ResourceText("IDS_COLNAME_NAME");
                }
                return _ColNameName;
            }
        }

        #endregion Properties
    }
}

