using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Entities.Exclusion
{
    /// ================================================================================
    /// <summary>データテーブル - スラブタグ</summary>
    /// ================================================================================
    public class DtSlabTag : RvtExtApp.Entities.Exclusion.DtBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>データ - スラブ</summary>
        private System.Data.DataTable _DataSlab;

        /// <summary>スラブ - 符号 - 値</summary>
        private string _SlabMarkVal;

        /// <summary>スラブ - レベル - 値</summary>
        private string _SlabLevelVal;

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
        public DtSlabTag(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.ESM_Elements cmpElements,
                         RvtExtApp.Components.ESM_Geometry cmpGeometry,
                         RvtExtApp.Components.ESM_Parameters cmpParameters,
                         RvtExtApp.Components.ESM_Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            _DataSlab = new System.Data.DataTable();
            DefDataFormat(ref _DataSlab);

            _SlabMarkVal = "";

            _SlabLevelVal = "";
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
        ///                               <p>1 = スラブ</p></param>
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
                    dt = _DataSlab;
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
            if (_DataSlab == null)
            {
                _DataSlab = new System.Data.DataTable();
                DefDataFormat(ref _DataSlab);
            }

            // なし設定
            base.SetItemNothing(_DataSlab);

            // スラブ要素
            IList<Element> slabElemAry = new List<Element>();
            IList<string> slabIdAry = new List<string>();
            base.GetSlabs(ref slabElemAry, ref slabIdAry);

            // スラブタグ要素
            IList<Element> slabTags = base.CmpElements.FloorTag;

            // タグタイプ
            IList<Element> slabTagTypeElemAry = new List<Element>();
            IList<string> slabTagTypeIdAry = new List<string>();
            for (int i = 0; i < slabTags.Count; ++i)
            {
                // タグ要素
                IndependentTag indpdtTag = slabTags[i] as IndependentTag;
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

                    // スラブ
                    if (slabIdAry.Contains(idLocElem) == true)
                    {
                        if (slabTagTypeIdAry.Contains(idIndpdtTagType) == false)
                        {
                            slabTagTypeElemAry.Add(indpdtTagType);
                            slabTagTypeIdAry.Add(idIndpdtTagType);
                        }
                    }
                }
            }

            // データ登録 - スラブ
            for (int i = 0; i < slabTagTypeElemAry.Count; ++i)
            {
                GetData(slabTagTypeElemAry[i], 1);
            }

            _DataSlab.DefaultView.Sort = base.ColNameName + " " + "ASC";
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
            // ========== スラブ ==========

            // スラブ - 符号 - 値
            _SlabMarkVal = data[0];

            // スラブ - レベル - 値
            _SlabLevelVal = data[1];
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>スラブ - 符号 - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string SlabMarkVal
        {
            get
            {
                return _SlabMarkVal;
            }
        }

        /// ================================================================================
        /// <summary>スラブ - レベル - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string SlabLevelVal
        {
            get
            {
                return _SlabLevelVal;
            }
        }

        #endregion Properties
    }
}



