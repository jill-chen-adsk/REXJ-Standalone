using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Entities.Exclusion
{
    /// ================================================================================
    /// <summary>データテーブル - 柱タグ</summary>
    /// ================================================================================
    public class DtColumnTag : RvtExtApp.Entities.Exclusion.DtBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>データ - 柱</summary>
        private System.Data.DataTable _DataColumn;

        /// <summary>柱 - 符号 - 値</summary>
        private string _ColumnMarkVal;

        /// <summary>柱 - レベル - 値</summary>
        private string _ColumnLevelVal;

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
        public DtColumnTag(RvtExtApp.Components.Attribute cmpAttribute,
                           RvtExtApp.Components.ESM_Elements cmpElements,
                           RvtExtApp.Components.ESM_Geometry cmpGeometry,
                           RvtExtApp.Components.ESM_Parameters cmpParameters,
                           RvtExtApp.Components.ESM_Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            _DataColumn = new System.Data.DataTable();
            DefDataFormat(ref _DataColumn);

            _ColumnMarkVal = "";

            _ColumnLevelVal = "";
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
        ///                               <p>1 = 柱</p></param>
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
                    dt = _DataColumn;
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
            if (_DataColumn == null)
            {
                _DataColumn = new System.Data.DataTable();
                DefDataFormat(ref _DataColumn);
            }

            // なし設定
            base.SetItemNothing(_DataColumn);

            // 柱要素
            IList<Element> columnElemAry = new List<Element>();
            IList<string> columnIdAry = new List<string>();
            base.GetColumns(ref columnElemAry, ref columnIdAry);

            // 柱タグ要素
            IList<Element> columnTags = base.CmpElements.ColumnTag;

            // タグタイプ
            IList<Element> columnTagTypeElemAry = new List<Element>();
            IList<string> columnTagTypeIdAry = new List<string>();
            for (int i = 0; i < columnTags.Count; ++i)
            {
                // タグ要素
                IndependentTag indpdtTag = columnTags[i] as IndependentTag;
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

                    // 柱
                    if (columnIdAry.Contains(idLocElem) == true)
                    {
                        if (columnTagTypeIdAry.Contains(idIndpdtTagType) == false)
                        {
                            columnTagTypeElemAry.Add(indpdtTagType);
                            columnTagTypeIdAry.Add(idIndpdtTagType);
                        }
                    }
                }
            }

            // データ登録 - 柱
            for (int i = 0; i < columnTagTypeElemAry.Count; ++i)
            {
                GetData(columnTagTypeElemAry[i], 1);
            }

            _DataColumn.DefaultView.Sort = base.ColNameName + " " + "ASC";
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
            // ========== 柱 ==========

            // 柱 - 符号 - 値
            _ColumnMarkVal = data[0];

            // 柱 - レベル - 値
            _ColumnLevelVal = data[1];
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>柱 - 符号 - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string ColumnMarkVal
        {
            get
            {
                return _ColumnMarkVal;
            }
        }

        /// ================================================================================
        /// <summary>柱 - レベル - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string ColumnLevelVal
        {
            get
            {
                return _ColumnLevelVal;
            }
        }

        #endregion Properties
    }
}



