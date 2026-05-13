using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - レベル</summary>
    /// ================================================================================
    public class DtLevel : RvtExtApp.Entities.DtBase
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>共有パラメータ</summary>
        private RvtExtApp.Entities.SpLevel _EntSpLevel;

        /// <summary>データ</summary>
        private System.Data.DataTable _Data;

        /// <summary>アクティブレベル</summary>
        private Level _ActiveLevel;

        /// <summary>処理レベル</summary>
        private Level _WorkLevel;

        /// <summary>Base level</summary>
        private Level _BaseLevel;

        /// <summary>列名 スラブレベル</summary>
        private string _ColNameSlabLevel;

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
        /// <history>2011/11/27 Created  GSA,Inc. Shinichi Ishii
        ///          2021/10/28 Modified Applied Technology</history>
        /// ================================================================================
        public DtLevel(RvtExtApp.Components.Attribute cmpAttribute,
                       RvtExtApp.Components.CFP_Elements cmpElements,
                       RvtExtApp.Components.CFP_Geometry cmpGeometry,
                       RvtExtApp.Components.CFP_Parameters cmpParameters,
                       RvtExtApp.Components.CFP_Settings cmpSettings) :
          base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            // 共有パラメータ
            _EntSpLevel = new RvtExtApp.Entities.SpLevel(cmpAttribute, cmpParameters, cmpSettings);
            if (_EntSpLevel.DefSuccess == false)
            {
                string strCategory = cmpAttribute.ResourceText("IDS_TXT_CATEGORY");
                string strParam = cmpAttribute.ResourceText("IDS_TXT_PARAMETER");
                base.ErrMsg = cmpAttribute.ResourceText("IDS_ERR_PARAMDEF") + "\n" +
                                          strCategory + " = " + _EntSpLevel.DefCatName + "\n" +
                                          "    " + strParam + "[" + _EntSpLevel.ErrDefName + "]";
            }
            _Data = new System.Data.DataTable();
            DefDataFormat(ref _Data);
            _ActiveLevel = null;
            _WorkLevel = null;
            _BaseLevel = null;
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>データ書式定義</summary>
        ///
        /// <param name="data">データテーブル</param>
        ///
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void DefDataFormat(ref System.Data.DataTable data)
        {
            // ID
            data.Columns.Add(base.ColNameID, typeof(int));

            // 名称
            data.Columns.Add(base.ColNameName, typeof(string));

            // 高さ
            data.Columns.Add(base.ColNameHeight, typeof(double));

            // スラブレベル
            data.Columns.Add(ColNameSlabLevel, typeof(bool));
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="elemLevel">レベル要素</param>
        ///
        /// <returns>データ行</returns>
        ///
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii
        ///          2021/10/28 Modified Applied Technology</history>
        /// ================================================================================
        public System.Data.DataRow GetData(Level elemLevel)
        {
            // 初期化
            System.Data.DataRow row = null;

            // データ
            if (_Data == null)
            {
                _Data = new System.Data.DataTable();
                DefDataFormat(ref _Data);
            }

            // 行データ
            row = _Data.NewRow();

            // 行設定
            if (elemLevel != null)
            {
                // 要素
                _EntSpLevel.CurrentElem = elemLevel;

                // ID
row[base.ColNameID] = elemLevel.Id.Value;
// 名称
                row[base.ColNameName] = elemLevel.Name;

                // 高さ
                double height = elemLevel.Elevation * base.CmpSettings.UnitCoe;
                row[base.ColNameHeight] = double.Parse(JExtComCompat.UtilValue.Rounding(height, 4, 2));

                // スラブレベル
                row[ColNameSlabLevel] = _EntSpLevel.SlabLevel;
            }
            return row;
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="elemLevels">レベル要素</param>
        ///
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void GetData(IList<Level> elemLevels)
        {
            // 行設定
            if (elemLevels != null)
            {
                foreach (Level elemLevel in elemLevels)
                {
                    // 行データ
                    System.Data.DataRow row = GetData(elemLevel);
                    if (row != null)
                    {
                        _Data.Rows.Add(row);
                    }
                }
            }
            _Data.DefaultView.Sort = ColNameHeight + " " + "ASC" + "," +
                                     base.ColNameName + " " + "ASC";
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="cbo">ComboBox</param>
        /// <param name="isShowAll">Is show all level or not</param>
        ///
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        ///          2021/11/17 Modified Applied Technology</history>
        /// ================================================================================
        public void GetData(System.Windows.Forms.ComboBox cbo, bool isShowAll)
        {
            bool bValue = false;

            // データテーブル設定
            System.Data.DataTable dt = cbo.DataSource as System.Data.DataTable;
            if (dt != null)
            {
                dt.Rows.Clear();
            }
            else
            {
                dt = new System.Data.DataTable();
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                cbo.DataSource = dt;
                cbo.DisplayMember = "Name";
                cbo.ValueMember = "ID";
            }

            if (ActiveLevel == null)
            {
                return;
            }

            if (_Data.Rows.Count == 0)
            {
                return;
            }

            // アクティブレベルID
var idActiveLevel = ActiveLevel.Id.Value;
// 該当レベル
            IList<int> idxAry = new List<int>();
            for (int i = _Data.Rows.Count - 1; i >= 0; i--)
            {
                if (isShowAll)
                {
                    if (bool.Parse(_Data.Rows[i][ColNameSlabLevel].ToString()))
                        idxAry.Add(i);
                }
                else
                {
                    var idLevel = long.Parse(_Data.Rows[i][base.ColNameID].ToString());
                    if (idActiveLevel == idLevel)
                    {
                        bValue = bool.Parse(_Data.Rows[i][ColNameSlabLevel].ToString());
                        if (bValue == false)
                        {
                            if (i > 0)
                            {
                                bValue = bool.Parse(_Data.Rows[i - 1][ColNameSlabLevel].ToString());
                                if (bValue)
                                    idxAry.Add(i - 1);
                            }
                            break;
                        }
                        else
                            idxAry.Add(i);
                    }
                    else
                    {
                        if (idxAry.Count == 1)
                        {
                            bValue = bool.Parse(_Data.Rows[i][ColNameSlabLevel].ToString());
                            if (bValue == true)
                                idxAry.Add(i);
                            else
                                break;
                        }
                    }
                }
            }

            // 値設定
            if (idxAry.Count > 0)
            {
                for (int i = 0; i < idxAry.Count; ++i)
                {
                    System.Data.DataRow row = dt.NewRow();
                    var id = int.Parse(_Data.Rows[idxAry[i]][base.ColNameID].ToString());
                    row["ID"] = id;
                    row["Name"] = _Data.Rows[idxAry[i]][base.ColNameName].ToString();
                    dt.Rows.Add(row);

                    if (id == idActiveLevel)
                        cbo.SelectedIndex = i;
                }
            }
        }

        /// ================================================================================
        /// <summary>処理レベル取得</summary>
        ///
        /// <param name="cbo">ComboBox</param>
        ///
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void GetWorkLevel(System.Windows.Forms.ComboBox cbo)
        {
            _WorkLevel = null;

            if (cbo.SelectedIndex > -1)
            {
                int id = int.Parse(cbo.SelectedValue.ToString());
                _WorkLevel = base.CmpElements.GetElementLevel(id);
            }
        }

        /// ================================================================================
        /// <summary>処理レベル取得</summary>
        ///
        /// <param name="cbo">ComboBox</param>
        ///
        /// <history>2021/10/28 Created Applied Technology</history>
        /// ================================================================================
        public void GetBaseLevel(System.Windows.Forms.ComboBox cbo)
        {
            _BaseLevel = null;

            if (cbo.SelectedIndex > -1)
            {
                int id = int.Parse(cbo.SelectedValue.ToString());
                _BaseLevel = base.CmpElements.GetElementLevel(id);
            }
        }

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <param name="elemLevels">レベル要素</param>
        ///
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii
        ///          2021/10/28 Modified Applied Technology</history>
        /// ================================================================================
        public void SetData(IList<Level> elemLevels)
        {
            // 初期化
            bool bValue = false;
            string keyColName = base.ColNameID;

            if (_Data != null)
            {
                if (elemLevels != null)
                {
                    foreach (Level elemLevel in elemLevels)
                    {
                        // 要素
                        _EntSpLevel.CurrentElem = elemLevel;

var strId = elemLevel.Id.Value.ToString();
// スラブレベル
                        bValue = bool.Parse(JExtComCompat.UtilData.GetValueTableData(_Data, keyColName, strId, ColNameSlabLevel));
                        _EntSpLevel.SlabLevel = bValue;
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>デフォルト設定</summary>
        ///
        /// <history>2011/12/11 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SetDefault()
        {
            if (_Data == null)
            {
                return;
            }

            // スラブレベル確認
            bool flag = false;
            for (int i = 0; i < _Data.Rows.Count; ++i)
            {
                bool bValue = bool.Parse(_Data.Rows[i][ColNameSlabLevel].ToString());
                if (bValue == true)
                {
                    flag = true;
                }
            }

            // スラブレベル設定
            if (flag == false)
            {
                for (int i = 0; i < _Data.Rows.Count; ++i)
                {
                    _Data.Rows[i][ColNameSlabLevel] = true;
                }
            }
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>データ</summary>
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public System.Data.DataTable Data
        {
            get
            {
                return _Data;
            }
        }

        /// ================================================================================
        /// <summary>アクティブレベル</summary>
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Level ActiveLevel
        {
            get
            {
                return _ActiveLevel;
            }
            set
            {
                _ActiveLevel = value;
            }
        }

        /// ================================================================================
        /// <summary>処理レベル</summary>
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Level WorkLevel
        {
            get
            {
                return _WorkLevel;
            }
        }

        /// ================================================================================
        /// <summary>Base level</summary>
        /// <history>2021/10/26 Created Applied Technology</history>
        /// ================================================================================
        public Level BaseLevel
        {
            get
            {
                return _BaseLevel;
            }
        }

        /// ================================================================================
        /// <summary>列名 スラブレベル</summary>
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameSlabLevel
        {
            get
            {
                if (_ColNameSlabLevel == null)
                {
                    _ColNameSlabLevel = base.CmpAttribute.ResourceText("IDS_COLNAME_SLABLEVEL");
                }
                return _ColNameSlabLevel;
            }
        }

        #endregion Properties
    }
}
