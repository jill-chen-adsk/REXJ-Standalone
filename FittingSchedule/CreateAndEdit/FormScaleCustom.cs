using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FittingSchedule;
using System.Reflection;

namespace ADSK.JExtRAC.FittingSchedule.CreateAndEdit
{
    /// ================================================================================
    /// <summary>画面 縮尺</summary>
    /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormScaleCustom : Form
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル - ビュー</summary>
        private RvtExtApp.Entities.DtView _EntDtView;

        /// <summary>データテーブル - コマンド</summary>
        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="entDtView"   >データテーブル - ビュー</param>
        /// <param name="entDtCmd"    >データテーブル - コマンド</param>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public FormScaleCustom(RvtExtApp.Components.Attribute cmpAttribute,
                               RvtExtApp.Entities.DtView entDtView,
                               RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;
            _EntDtView = entDtView;
            _EntDtCmd = entDtCmd;
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_SCALE") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.lblScale.Text = _CmpAttribute.ResourceText("IDS_TXT_SCALE");
            this.lblScaleOrder.Text = _CmpAttribute.ResourceText("IDS_TXT_COLON1");
            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");

            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2010/07/15 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetData()
        {
            this.txtScale.Text = _EntDtView.ViewScaleCustom.ToString();
        }

        /// ================================================================================
        /// <summary>フォームのデータ取得</summary>
        ///
        /// <history>2010/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void GetData()
        {
            _EntDtView.ViewScaleCustom = int.Parse(this.txtScale.Text);
            _EntDtCmd.Data[4] = _EntDtView.ViewScaleCustom.ToString();
        }

        /// ================================================================================
        /// <summary>フォームのエラープロバイダ取得</summary>
        ///
        /// <returns><p>結果</p>
        ///           <p>True  = エラーなし</p>
        ///           <p>False = エラーあり</p></returns>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool GetErrPvd()
        {
            bool ret = false;

            if (this.errPvd.GetError(this.txtScale) != "")
            {
                return ret;
            }

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>フォームのエラープロバイダチェック</summary>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void ChkErrPvd()
        {
            this.errPvd.SetError(this.txtScale, _EntDtView.SetErrPvdDecimalText(this.txtScale.Text));
        }

        #endregion Member Functions

        // イベント
        #region Events

        /// ================================================================================
        /// <summary>Handles the Load event of the FormScaleCustom control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2010/07/15 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void FormScaleCustom_Load(object sender, EventArgs e)
        {
            SetText();
            SetData();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2010/07/15 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnOK_Click(object sender, EventArgs e)
        {
            ChkErrPvd();
            if (GetErrPvd() == true)
            {
                GetData();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtScale control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2010/07/15 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void txtScale_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtScale, _EntDtView.SetErrPvdDecimalText(this.txtScale.Text.Trim()));
        }

        #endregion Events
    }
}
