using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Entities.Exclusion
{
    /// ================================================================================
    /// <summary>データテーブル - 基礎タグ</summary>
    /// ================================================================================
    public class DtFoundationTag : RvtExtApp.Entities.Exclusion.DtBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>データ - 基礎</summary>
        private System.Data.DataTable _DataFoundation;

        /// <summary>基礎 - 符号 - 値</summary>
        private string _FoundationMarkVal;

        /// <summary>基礎 - レベル - 値</summary>
        private string _FoundationLevelVal;

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
        /// <history>2011/12/05 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public DtFoundationTag(RvtExtApp.Components.Attribute cmpAttribute,
                               RvtExtApp.Components.ESM_Elements cmpElements,
                               RvtExtApp.Components.ESM_Geometry cmpGeometry,
                               RvtExtApp.Components.ESM_Parameters cmpParameters,
                               RvtExtApp.Components.ESM_Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            _DataFoundation = new System.Data.DataTable();
            DefDataFormat(ref _DataFoundation);

            _FoundationMarkVal = "";

            _FoundationLevelVal = "";
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>データ書式定義</summary>
        ///
        /// <param name="data">データテーブル</param>
        ///
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void DefDataFormat(ref System.Data.DataTable data)
        {
            // ID
            data.Columns.Add(base.ColNameID, typeof(long));

            // 名称
            data.Columns.Add(base.ColNameName, typeof(string));
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="elemTagType">タグタイプ要素</param>
        /// <param name="mode"        ><p>モード</p>
        ///                               <p>1 = 基礎</p></param>
        ///
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetData(Element elemTagType, int mode)
        {
            // 初期化
            System.Data.DataRow row = null;

            // データ
            System.Data.DataTable dt = null;
            switch (mode)
            {
                case 1:
                    dt = _DataFoundation;
                    break;
            }

            if (dt == null)
            {
                return;
            }

            // 行設定
            if (elemTagType != null)
            {
                // 行データ
                row = dt.NewRow();

                // ID
                row[base.ColNameID] = (long)elemTagType.Id.Value;

                // 名称
                row[base.ColNameName] = base.GetFamilyTypeName(elemTagType);

                dt.Rows.Add(row);
            }
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetData()
        {
            // 初期化
            if (_DataFoundation == null)
            {
                _DataFoundation = new System.Data.DataTable();
                DefDataFormat(ref _DataFoundation);
            }

            // なし設定
            base.SetItemNothing(_DataFoundation);

            // 基礎要素
            IList<Element> foundationElemAry = new List<Element>();
            IList<string> foundationIdAry = new List<string>();
            base.GetFoundations(ref foundationElemAry, ref foundationIdAry);

            // 基礎タグ要素
            IList<Element> foundationTags = base.CmpElements.FoundationTag;

            // タグタイプ
            IList<Element> foundationTagTypeElemAry = new List<Element>();
            IList<string> foundationTagTypeIdAry = new List<string>();
            for (int i = 0; i < foundationTags.Count; ++i)
            {
                // タグ要素
                IndependentTag indpdtTag = foundationTags[i] as IndependentTag;
                if (indpdtTag == null)
                {
                    continue;
                }

                // 配置要素
                foreach (Element locElem in indpdtTag.GetTaggedLocalElements())
                {
                    if (locElem == null)
                    {
                        continue;
                    }
                    var idLocElem = locElem.Id.ToString();

                    // タグタイプ
                    Element indpdtTagType = CmpElements.GetElemType(indpdtTag);
                    if (indpdtTagType == null)
                    {
                        continue;
                    }
                    var idIndpdtTagType = indpdtTagType.Id.ToString();

                    // 基礎
                    if (foundationIdAry.Contains(idLocElem) == true)
                    {
                        if (foundationTagTypeIdAry.Contains(idIndpdtTagType) == false)
                        {
                            foundationTagTypeElemAry.Add(indpdtTagType);
                            foundationTagTypeIdAry.Add(idIndpdtTagType);
                        }
                    }
                }
            }

            // データ登録 - 基礎
            for (int i = 0; i < foundationTagTypeElemAry.Count; ++i)
            {
                GetData(foundationTagTypeElemAry[i], 1);
            }

            _DataFoundation.DefaultView.Sort = base.ColNameName + " " + "ASC";
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="data">データ</param>
        ///
        /// <history><p>2011/12/06 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/10/12 Modified Applied Technology<p></history>
        /// ================================================================================
        public
        void GetData(IList<string> data)
        {
            // ========== 基礎 ==========

            // 基礎 - 符号 - 値
            _FoundationMarkVal = data[0];

            // 基礎 - レベル - 値
            _FoundationLevelVal = data[1];
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>基礎 - 符号 - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string FoundationMarkVal
        {
            get
            {
                return _FoundationMarkVal;
            }
        }

        /// ================================================================================
        /// <summary>基礎 - レベル - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string FoundationLevelVal
        {
            get
            {
                return _FoundationLevelVal;
            }
        }

        #endregion Properties
    }
}



