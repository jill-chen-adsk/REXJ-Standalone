using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FittingSchedule;
using ADSK.JExtRAC.FittingSchedule.Components;

namespace ADSK.JExtRAC.FittingSchedule.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - ビュー</summary>
    /// ================================================================================
    public class DtView : RvtExtApp.Entities.DtBase
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>共有パラメータ</summary>
        private RvtExtApp.Entities.SpView _EntSpView;

        /// <summary>ビューが重複している時のオプション</summary>
        private int _DuplicateViewOpt;

        /// <summary>ビュー縮尺デフォルト用</summary>
        private int _ViewScaleDefault;

        /// <summary>ビュー縮尺カスタム用</summary>
        private int _ViewScaleCustom;
        private int _ViewDetailLevel;

        /// <summary>縮尺データ</summary>
        private System.Data.DataTable _DataScale;
        private System.Data.DataTable _DetailLevel;

        /// <summary>縮尺の最小値</summary>
        private int _ScaleMin;

        /// <summary>縮尺の最大値</summary>
        private int _ScaleMax;

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
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public DtView(RvtExtApp.Components.Attribute cmpAttribute,
                      RvtExtApp.Components.Elements cmpElements,
                      RvtExtApp.Components.Geometry cmpGeometry,
                      RvtExtApp.Components.Parameters cmpParameters,
                      RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            // 共有パラメータ
            _EntSpView = new RvtExtApp.Entities.SpView(cmpAttribute, cmpParameters, cmpSettings);
            if (_EntSpView.DefSuccess == false)
            {
                string strCategory = cmpAttribute.ResourceText("IDS_TXT_CATEGORY");
                string strParam = cmpAttribute.ResourceText("IDS_TXT_PARAMETER");
                base.ErrMsg = cmpAttribute.ResourceText("IDS_ERR_PARAMDEF") + "\n" +
                                          strCategory + " = " + _EntSpView.DefCatName + "\n" +
                                          "    " + strParam + "[" + _EntSpView.ErrDefName + "]";
            }

            // 初期化
            _DuplicateViewOpt = 0;
            _ViewScaleDefault = 100;
            _ViewScaleCustom = 100;
            _ViewDetailLevel = (int)Revit.DB.ViewDetailLevel.Medium;

            _ScaleMin = 1;
            _ScaleMax = 9999;
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>データ取得 - 建具姿図作成・更新</summary>
        ///
        /// <param name="duplicateViewOpt">ビューが重複している時のオプション</param>
        /// <param name="viewScaleDefault">ビュー縮尺デフォルト用</param>
        /// <param name="viewScaleCustom" >ビュー縮尺カスタム用</param>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetDataCreateAndEdit(string duplicateViewOpt, string viewScaleDefault, string viewScaleCustom, string viewDetailLevel)
        {
            if ((duplicateViewOpt != null) && (duplicateViewOpt != ""))
            {
                _DuplicateViewOpt = int.Parse(duplicateViewOpt);
            }

            if ((viewScaleDefault != null) && (viewScaleDefault != ""))
            {
                _ViewScaleDefault = int.Parse(viewScaleDefault);
            }

            if ((viewScaleCustom != null) && (viewScaleCustom != ""))
            {
                _ViewScaleCustom = int.Parse(viewScaleCustom);
            }
            if ((viewDetailLevel != null) && (viewDetailLevel != ""))
            {
                _ViewDetailLevel = int.Parse(viewDetailLevel);
            }
        }

        /// ================================================================================
        /// <summary>コンボボックスの選択値のエラー設定</summary>
        ///
        /// <param name="value">選択値</param>
        ///
        /// <returns><p>エラー文字列</p>
        ///             <p>空白時はエラーなし</returns>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string SetErrPvdCboSelectedValue(Object value)
        {
            string errMsg = "";

            // 空白チェック
            if (value == null)
            {
                errMsg = base.CmpAttribute.ResourceText("IDS_ERR_CBONULL");
            }
            return errMsg;
        }

        /// ================================================================================
        /// <summary>小数点桁数のエラー設定</summary>
        ///
        /// <param name="value">値</param>
        ///
        /// <returns><p>エラー文字列</p>
        ///             <p>空白時はエラーなし</returns>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string SetErrPvdDecimalText(string value)
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
                if ((iValue < _ScaleMin) || (iValue > _ScaleMax))
                {
                    errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALRANGE");
                }
            }

            return errMsg;
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>共有パラメータ</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        RvtExtApp.Entities.SpView EntSpView
        {
            get
            {
                return _EntSpView;
            }
        }

        /// ================================================================================
        /// <summary>ビューが重複している時のオプション</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int DuplicateViewOpt
        {
            get
            {
                return _DuplicateViewOpt;
            }
            set
            {
                _DuplicateViewOpt = value;
            }
        }

        /// ================================================================================
        /// <summary>ビュー縮尺デフォルト用</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int ViewScaleDefault
        {
            get
            {
                return _ViewScaleDefault;
            }
            set
            {
                _ViewScaleDefault = value;
            }
        }

        /// ================================================================================
        /// <summary>ビュー縮尺カスタム用</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int ViewScaleCustom
        {
            get
            {
                return _ViewScaleCustom;
            }
            set
            {
                _ViewScaleCustom = value;
            }
        }

        /// ================================================================================
        /// <summary>ビュー詳細レベル</summary>
        /// <history>2022/02/02 Created OGI,Inc. Takemi Katoh</history>
        /// ================================================================================
        public
        int ViewDetailLevel
        {
            get
            {
                return _ViewDetailLevel;
            }
            set
            {
                _ViewDetailLevel = value;
            }
        }


        /// ================================================================================
        /// <summary>縮尺データ</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Data.DataTable DataScale
        {
            get
            {
                if (_DataScale == null)
                {
                    _DataScale = new System.Data.DataTable();
                    System.Data.DataRow row;
                    _DataScale.Columns.Add("Name", typeof(string));
                    _DataScale.Columns.Add("Value", typeof(int));

                    row = _DataScale.NewRow();
                    row["Name"] = "カスタム...";
                    row["Value"] = 0;
                    _DataScale.Rows.Add(row);

                    row = _DataScale.NewRow();
                    row["Name"] = "1:1";
                    row["Value"] = 1;
                    _DataScale.Rows.Add(row);

                    row = _DataScale.NewRow();
                    row["Name"] = "1:2";
                    row["Value"] = 2;
                    _DataScale.Rows.Add(row);

                    row = _DataScale.NewRow();
                    row["Name"] = "1:5";
                    row["Value"] = 5;
                    _DataScale.Rows.Add(row);

                    row = _DataScale.NewRow();
                    row["Name"] = "1:10";
                    row["Value"] = 10;
                    _DataScale.Rows.Add(row);

                    row = _DataScale.NewRow();
                    row["Name"] = "1:20";
                    row["Value"] = 20;
                    _DataScale.Rows.Add(row);

                    row = _DataScale.NewRow();
                    row["Name"] = "1:50";
                    row["Value"] = 50;
                    _DataScale.Rows.Add(row);

                    row = _DataScale.NewRow();
                    row["Name"] = "1:100";
                    row["Value"] = 100;
                    _DataScale.Rows.Add(row);

                    row = _DataScale.NewRow();
                    row["Name"] = "1:200";
                    row["Value"] = 200;
                    _DataScale.Rows.Add(row);

                    row = _DataScale.NewRow();
                    row["Name"] = "1:500";
                    row["Value"] = 500;
                    _DataScale.Rows.Add(row);

                    row = _DataScale.NewRow();
                    row["Name"] = "1:1000";
                    row["Value"] = 1000;
                    _DataScale.Rows.Add(row);

                    row = _DataScale.NewRow();
                    row["Name"] = "1:2000";
                    row["Value"] = 2000;
                    _DataScale.Rows.Add(row);

                    row = _DataScale.NewRow();
                    row["Name"] = "1:5000";
                    row["Value"] = 5000;
                    _DataScale.Rows.Add(row);
                }
                return _DataScale;
            }
        }

        public
        System.Data.DataTable DetailLevel
        {
            get
            {
                if (_DetailLevel == null)
                {
                    _DetailLevel = new System.Data.DataTable();
                    System.Data.DataRow row;
                    _DetailLevel.Columns.Add("Name", typeof(string));
                    _DetailLevel.Columns.Add("Value", typeof(int));

                    row = _DetailLevel.NewRow();
                    row["Name"] = CmpAttribute.ResourceText("IDS_TXT_DETAILLEVEL_COARSE");
                    row["Value"] = (int)Revit.DB.ViewDetailLevel.Coarse;
                    _DetailLevel.Rows.Add(row);

                    row = _DetailLevel.NewRow();
                    row["Name"] = CmpAttribute.ResourceText("IDS_TXT_DETAILLEVEL_MEDIUM");
                    row["Value"] = (int)Revit.DB.ViewDetailLevel.Medium;
                    _DetailLevel.Rows.Add(row);

                    row = _DetailLevel.NewRow();
                    row["Name"] = CmpAttribute.ResourceText("IDS_TXT_DETAILLEVEL_FINE");
                    row["Value"] = (int)Revit.DB.ViewDetailLevel.Fine;
                    _DetailLevel.Rows.Add(row);
                }
                return _DetailLevel;
            }
        }


        /// ================================================================================
        /// <summary>縮尺の最小値</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int ScaleMin
        {
            get
            {
                return _ScaleMin;
            }
        }

        /// ================================================================================
        /// <summary>縮尺の最大値</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int ScaleMax
        {
            get
            {
                return _ScaleMax;
            }
        }

        #endregion Properties
    }
}
