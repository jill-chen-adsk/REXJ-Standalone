using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FittingSchedule;
using ADSK.JExtRAC.FittingSchedule.Components;

namespace ADSK.JExtRAC.FittingSchedule.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - ビューシート</summary>
    /// ================================================================================
    public class DtViewSheet : RvtExtApp.Entities.DtBase
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>ビュータイプオプション</summary>
        private int _ViewTypeOpt;

        /// <summary>上の余白</summary>
        private int _BlankTop;

        /// <summary>下の余白</summary>
        private int _BlankBottom;

        /// <summary>左の余白</summary>
        private int _BlankLeft;

        /// <summary>右の余白</summary>
        private int _BlankRight;

        /// <summary>既存ビューデータ</summary>
        private System.Data.DataTable _DataViewExist;

        /// <summary>対象ビューデータ</summary>
        private System.Data.DataTable _DataViewTarget;

        /// <summary>余白の最小値</summary>
        private int _BlankMin;

        /// <summary>余白の最大値</summary>
        private int _BlankMax;

        /// <summary>窓のビュー名の接頭語 (all languages)</summary>
        private static readonly string[] _ViewNameWindows = { "姿図_窓", "Elevation_Window" };

        /// <summary>ドアのビュー名の接頭語 (all languages)</summary>
        private static readonly string[] _ViewNameDoors = { "姿図_ドア", "Elevation_Door" };

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
        /// <history>2011/08/03 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public DtViewSheet(RvtExtApp.Components.Attribute cmpAttribute,
                           RvtExtApp.Components.Elements cmpElements,
                           RvtExtApp.Components.Geometry cmpGeometry,
                           RvtExtApp.Components.Parameters cmpParameters,
                           RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            // 初期化
            _ViewTypeOpt = 0;

            _BlankTop = 10;
            _BlankBottom = 10;
            _BlankLeft = 10;
            _BlankRight = 10;

            _DataViewExist = base.CmpElements.ElementsTableViewSectionParts;
            base.CmpElements.CompareViewOfSheet(ref _DataViewExist);
            _DataViewExist.DefaultView.Sort = _DataViewExist.Columns[1].ColumnName + " " + "ASC";

            _BlankMin = 0;
            _BlankMax = 999;

            _DataViewTarget = new System.Data.DataTable();
            _DataViewTarget.Columns.Add("ID", typeof(int));
            _DataViewTarget.Columns.Add("NAME", typeof(string));
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>データ取得 - 建具姿図レイアウト</summary>
        ///
        /// <param name="viewTypeOpt" >ビュータイプオプション</param>
        /// <param name="blankTop"    >上の余白</param>
        /// <param name="blankBottom" >下の余白</param>
        /// <param name="blankLeft"   >左の余白</param>
        /// <param name="blankRight"  >右の余白</param>
        ///
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetDataLayout(string viewTypeOpt,
                           string blankTop, string blankBottom, string blankLeft, string blankRight)
        {
            if ((viewTypeOpt != null) && (viewTypeOpt != ""))
            {
                _ViewTypeOpt = int.Parse(viewTypeOpt);
            }

            if ((blankTop != null) && (blankTop != ""))
            {
                _BlankTop = int.Parse(blankTop);
            }

            if ((blankBottom != null) && (blankBottom != ""))
            {
                _BlankBottom = int.Parse(blankBottom);
            }

            if ((blankLeft != null) && (blankLeft != ""))
            {
                _BlankLeft = int.Parse(blankLeft);
            }

            if ((blankRight != null) && (blankRight != ""))
            {
                _BlankRight = int.Parse(blankRight);
            }
        }

        /// ================================================================================
        /// <summary>リストボックスのエラー設定</summary>
        ///
        /// <param name="listBox">リストボックス</param>
        ///
        /// <returns><p>エラー文字列</p>
        ///             <p>空白時はエラーなし</p></returns>
        ///
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string SetErrPvdListBox(System.Windows.Forms.ListBox listBox)
        {
            string errMsg = "";

            // 空白チェック
            if (listBox.Items.Count == 0)
            {
                errMsg = base.CmpAttribute.ResourceText("IDS_ERR_LSTNULL");
            }
            return errMsg;
        }

        /// ================================================================================
        /// <summary>余白値のエラー設定</summary>
        ///
        /// <param name="value">値</param>
        ///
        /// <returns><p>エラー文字列</p>
        ///             <p>空白時はエラーなし</p></returns>
        ///
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string SetErrPvdBlankText(string value)
        {
            string errMsg = "";

            // 空白チェック
            if (UtilValue.IsNull(value) == true)
            {
                errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALNULL");
            }

            // 整数チェック
            if (errMsg == "")
            {
                if (UtilValue.IsInteger(value) == false)
                {
                    errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALNUMBER");
                }
            }

            //　値の範囲チェック
            if (errMsg == "")
            {
                int iValue = int.Parse(value);
                if ((iValue < _BlankMin) || (iValue > _BlankMax))
                {
                    errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALRANGE");
                }
            }

            return errMsg;
        }

        /// ================================================================================
        /// <summary>テーブルデータ間の行移動</summary>
        ///
        /// <param name="selectIndex">選択されたインデックス</param>
        ///
        /// <returns>移動行データ</returns>
        ///
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Object MoveViewTable(int selectIndex)
        {
            return UtilData.MoveListDataTableRow(ref _DataViewExist, selectIndex, ref _DataViewTarget);
        }

        /// ================================================================================
        /// <summary>テーブルデータの行削除</summary>
        ///
        /// <param name="selectIndex">選択されたインデックス</param>
        ///
        /// <returns>削除行データ</returns>
        ///
        /// <history>2011/08/03 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Object DelViewTable(int selectIndex)
        {
            System.Object retObj = null;

            System.Data.DataRow row = _DataViewTarget.Rows[selectIndex];
            string idStr = row[0].ToString();
            int id = -1;
            if (UtilValue.IsInteger(idStr))
            {
                id = int.Parse(idStr);
            }

            if (id == 0)
            {
                _DataViewTarget.Rows[selectIndex].Delete();
            }
            else
            {
                retObj = UtilData.MoveListDataTableRow(ref _DataViewTarget, selectIndex, ref _DataViewExist);
            }

            return retObj;
        }

        /// ================================================================================
        /// <summary>テーブルデータ行の上下移動</summary>
        ///
        /// <param name="selectIndex" >選択されたインデックス</param>
        /// <param name="upFlag"      >上方向</param>
        ///
        /// <returns>移動行データ</returns>
        ///
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Object UpDnViewTable(int selectIndex, bool upFlag)
        {
            return UtilData.UpDnDataTableRow(ref _DataViewTarget, selectIndex, upFlag);
        }

        /// ================================================================================
        /// <summary>ビューテーブルのフィルター</summary>
        ///
        /// <param name="flag"><p>建具の種類</p>
        ///                       <p>0=窓</p>
        ///                       <p>1=ドア</p></param>
        ///
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void FilterViewTable(int flag)
        {
            string colName = _DataViewExist.Columns[1].ColumnName;
            string filterStr = "";

            string[] prefixes = null;
            if (flag == 0)
                prefixes = _ViewNameWindows;
            else if (flag == 1)
                prefixes = _ViewNameDoors;

            if (prefixes != null)
            {
                var parts = new System.Collections.Generic.List<string>();
                foreach (var prefix in prefixes)
                    parts.Add(colName + " LIKE '*" + prefix + "*'");
                filterStr = string.Join(" OR ", parts);
            }

            _DataViewExist.DefaultView.RowFilter = filterStr;
        }

        /// ================================================================================
        /// <summary>テーブルデータ行の改行追加</summary>
        ///
        /// <param name="selectIndex" >選択されたインデックス</param>
        ///
        /// <returns>追加行データ</returns>
        ///
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Object AddNewLineTable(int selectIndex)
        {
            int id = 0;
            string name = "<" + base.CmpAttribute.ResourceText("IDS_TXT_NEWLINE") + ">";
            return UtilData.AddDataTableRow(ref _DataViewTarget, selectIndex, id, name);
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>ビュータイプオプション</summary>
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int ViewTypeOpt
        {
            get
            {
                return _ViewTypeOpt;
            }
            set
            {
                _ViewTypeOpt = value;
            }
        }

        /// ================================================================================
        /// <summary>上の余白</summary>
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int BlankTop
        {
            get
            {
                return _BlankTop;
            }
            set
            {
                _BlankTop = value;
            }
        }

        /// ================================================================================
        /// <summary>下の余白</summary>
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int BlankBottom
        {
            get
            {
                return _BlankBottom;
            }
            set
            {
                _BlankBottom = value;
            }
        }

        /// ================================================================================
        /// <summary>左の余白</summary>
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int BlankLeft
        {
            get
            {
                return _BlankLeft;
            }
            set
            {
                _BlankLeft = value;
            }
        }

        /// ================================================================================
        /// <summary>右の余白</summary>
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int BlankRight
        {
            get
            {
                return _BlankRight;
            }
            set
            {
                _BlankRight = value;
            }
        }

        /// ================================================================================
        /// <summary>既存ビューデータ</summary>
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Data.DataTable DataViewExist
        {
            get
            {
                return _DataViewExist;
            }
        }

        /// ================================================================================
        /// <summary>対象ビューデータ</summary>
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Data.DataTable DataViewTarget
        {
            get
            {
                return _DataViewTarget;
            }
        }

        /// ================================================================================
        /// <summary>余白の最小値</summary>
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int BlankMin
        {
            get
            {
                return _BlankMin;
            }
        }

        /// ================================================================================
        /// <summary>余白の最大値</summary>
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int BlankMax
        {
            get
            {
                return _BlankMax;
            }
        }

        #endregion Properties
    }
}
