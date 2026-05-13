using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Entities.Exclusion
{
    /// ================================================================================
    /// <summary>データテーブル - 壁タグ</summary>
    /// ================================================================================
    public class DtWallTag : RvtExtApp.Entities.Exclusion.DtBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>データ - 外壁</summary>
        private System.Data.DataTable _DataWallExt;

        /// <summary>データ - 内壁</summary>
        private System.Data.DataTable _DataWallInt;

        /// <summary>外壁 - 符号 - 値</summary>
        private string _WallExtMarkVal;

        /// <summary>外壁 - レベル - 値</summary>
        private string _WallExtLevelVal;

        /// <summary>内壁 - 符号 - 値</summary>
        private string _WallIntMarkVal;

        /// <summary>内壁 - レベル - 値</summary>
        private string _WallIntLevelVal;

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
        public DtWallTag(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.ESM_Elements cmpElements,
                         RvtExtApp.Components.ESM_Geometry cmpGeometry,
                         RvtExtApp.Components.ESM_Parameters cmpParameters,
                         RvtExtApp.Components.ESM_Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            _DataWallExt = new System.Data.DataTable();
            DefDataFormat(ref _DataWallExt);

            _DataWallInt = new System.Data.DataTable();
            DefDataFormat(ref _DataWallInt);

            _WallExtMarkVal = "";

            _WallExtLevelVal = "";

            _WallIntMarkVal = "";

            _WallIntLevelVal = "";
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
        /// <param name="elemTagType" >タグタイプ要素</param>
        /// <param name="mode"        ><p>モード</p>
        ///                               <p>1 = 外壁</p>
        ///                               <p>2 = 内壁</p></param>
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
                    dt = _DataWallExt;
                    break;

                case 2:
                    dt = _DataWallInt;
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
            if (_DataWallExt == null)
            {
                _DataWallExt = new System.Data.DataTable();
                DefDataFormat(ref _DataWallExt);
            }
            if (_DataWallInt == null)
            {
                _DataWallInt = new System.Data.DataTable();
                DefDataFormat(ref _DataWallInt);
            }

            // なし設定
            base.SetItemNothing(_DataWallExt);
            base.SetItemNothing(_DataWallInt);

            // 壁要素
            IList<Element> wallExtElemAry = new List<Element>();
            IList<Element> wallIntElemAry = new List<Element>();
            IList<string> wallExtIdAry = new List<string>();
            IList<string> wallIntIdAry = new List<string>();
            base.GetWalls(ref wallExtElemAry, ref wallIntElemAry, ref wallExtIdAry, ref wallIntIdAry);

            // 壁タグ要素
            IList<Element> wallTags = base.CmpElements.WallTag;

            // タグタイプ
            IList<Element> wallExtTagTypeElemAry = new List<Element>();
            IList<Element> wallIntTagTypeElemAry = new List<Element>();
            IList<string> wallExtTagTypeIdAry = new List<string>();
            IList<string> wallIntTagTypeIdAry = new List<string>();
            for (int i = 0; i < wallTags.Count; ++i)
            {
                // タグ要素
                IndependentTag indpdtTag = wallTags[i] as IndependentTag;
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

                    // 外壁
                    if (wallExtIdAry.Contains(idLocElem) == true)
                    {
                        if (wallExtTagTypeIdAry.Contains(idIndpdtTagType) == false)
                        {
                            wallExtTagTypeElemAry.Add(indpdtTagType);
                            wallExtTagTypeIdAry.Add(idIndpdtTagType);
                        }
                    }
                    // 内壁
                    else if (wallIntIdAry.Contains(idLocElem) == true)
                    {
                        if (wallIntTagTypeIdAry.Contains(idIndpdtTagType) == false)
                        {
                            wallIntTagTypeElemAry.Add(indpdtTagType);
                            wallIntTagTypeIdAry.Add(idIndpdtTagType);
                        }
                    }
                }
            }

            // データ登録 - 外壁
            for (int i = 0; i < wallExtTagTypeElemAry.Count; ++i)
            {
                GetData(wallExtTagTypeElemAry[i], 1);
            }

            // データ登録 - 内壁
            for (int i = 0; i < wallIntTagTypeElemAry.Count; ++i)
            {
                GetData(wallIntTagTypeElemAry[i], 2);
            }

            _DataWallExt.DefaultView.Sort = base.ColNameName + " " + "ASC";
            _DataWallInt.DefaultView.Sort = base.ColNameName + " " + "ASC";
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
            // ========== 外壁 ==========

            // 外壁 - 符号 - 値
            _WallExtMarkVal = data[0];

            // 外壁 - レベル - 値
            _WallExtLevelVal = data[1];

            // ========== 内壁 ==========

            // 内壁 - 符号 - 値
            _WallIntMarkVal = data[2];

            // 内壁 - レベル - 値
            _WallIntLevelVal = data[3];
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>外壁 - 符号 - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string WallExtMarkVal
        {
            get
            {
                return _WallExtMarkVal;
            }
        }

        /// ================================================================================
        /// <summary>外壁 - レベル - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string WallExtLevelVal
        {
            get
            {
                return _WallExtLevelVal;
            }
        }

        /// ================================================================================
        /// <summary>内壁 - 符号 - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string WallIntMarkVal
        {
            get
            {
                return _WallIntMarkVal;
            }
        }

        /// ================================================================================
        /// <summary>内壁 - レベル - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string WallIntLevelVal
        {
            get
            {
                return _WallIntLevelVal;
            }
        }

        #endregion Properties
    }
}



