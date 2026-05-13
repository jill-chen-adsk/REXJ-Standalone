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
    /// <summary>画面 建具姿図作成</summary>
    /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormCreatePartsDrawing : Form
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>縮尺フォームフラグ</summary>
        private bool _FlagformScale;

        /// <summary>データテーブル - ビュー</summary>
        private RvtExtApp.Entities.DtView _EntDtView;

        /// <summary>データテーブル - 建具タイプ</summary>
        private RvtExtApp.Entities.DtWinDoorType _EntDtWinDoorType;

        /// <summary>データテーブル - コマンド</summary>
        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"    >属性</param>
        /// <param name="entDtView"       >データテーブル - ビュー</param>
        /// <param name="entDtWinDoorType">データテーブル - 建具タイプ</param>
        /// <param name="entDtCmd"        >データテーブル - コマンド</param>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public FormCreatePartsDrawing(RvtExtApp.Components.Attribute cmpAttribute,
                                      RvtExtApp.Entities.DtView entDtView,
                                      RvtExtApp.Entities.DtWinDoorType entDtWinDoorType,
                                      RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _FlagformScale = false;
            _EntDtView = entDtView;
            _EntDtWinDoorType = entDtWinDoorType;
            _EntDtCmd = entDtCmd;
            this.errPvd.SetIconAlignment(this.cboDoorTag, ErrorIconAlignment.MiddleLeft);
            this.errPvd.SetIconAlignment(this.cboWindowTag, ErrorIconAlignment.MiddleLeft);
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        ///
        /// <history><p>2011/08/02 Created GSA,Inc. Shinichi Ishii</p>
        ///           <p>2015/09/07 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_CREATEPARTSDRAWING") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.lblDoorTag.Text = _CmpAttribute.ResourceText("IDS_TXT_DOORTAG") + "(D)";
            this.lblWindowTag.Text = _CmpAttribute.ResourceText("IDS_TXT_WINDOWTAG") + "(W)";
            this.gpbDuplicateView.Text = _CmpAttribute.ResourceText("IDS_TXT_HANDLINGDUPLICATEVIEW");
            this.rdbViewDelOld.Text = _CmpAttribute.ResourceText("IDS_TXT_DELOLDVIEW");
            this.rdbViewNotUndate.Text = _CmpAttribute.ResourceText("IDS_TXT_NOTUPDATE");
            this.rdbViewChangeOld.Text = _CmpAttribute.ResourceText("IDS_TXT_CHANGEOLDVIEW");
            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
            this.lblScale.Text = _CmpAttribute.ResourceText("IDS_TXT_SCALE") + "(S)";
            this.lblDetailLevel.Text = _CmpAttribute.ResourceText("IDS_TXT_DETAILLEVEL") + "(L)";

            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetData()
        {
            RDBDuplicateView = _EntDtView.DuplicateViewOpt;

            this.cboDoorTag.DataSource = _EntDtWinDoorType.DataDoorTags;
            this.cboDoorTag.DisplayMember = _EntDtWinDoorType.DataDoorTags.Columns[1].ColumnName;
            this.cboDoorTag.ValueMember = _EntDtWinDoorType.DataDoorTags.Columns[0].ColumnName;

            this.cboWindowTag.DataSource = _EntDtWinDoorType.DataWindowTags;
            this.cboWindowTag.DisplayMember = _EntDtWinDoorType.DataWindowTags.Columns[1].ColumnName;
            this.cboWindowTag.ValueMember = _EntDtWinDoorType.DataWindowTags.Columns[0].ColumnName;

            this.cboScale.DataSource = _EntDtView.DataScale;
            this.cboScale.DisplayMember = "Name";
            this.cboScale.ValueMember = "Value";

            this.cboDetailLevel.DataSource = _EntDtView.DetailLevel;
            this.cboDetailLevel.DisplayMember = "Name";
            this.cboDetailLevel.ValueMember = "Value";

            if (this.cboDoorTag.Items.Count > 0)
            {
                this.cboDoorTag.SelectedValue = _EntDtWinDoorType.IdDoorTag;
                if (this.cboDoorTag.SelectedIndex == -1)
                {
                    this.cboDoorTag.SelectedIndex = 0;
                }
            }

            if (this.cboWindowTag.Items.Count > 0)
            {
                this.cboWindowTag.SelectedValue = _EntDtWinDoorType.IdWindowTag;
                if (this.cboWindowTag.SelectedIndex == -1)
                {
                    this.cboWindowTag.SelectedIndex = 0;
                }
            }

            if (this.cboScale.Items.Count > 0)
            {
                this.cboScale.SelectedValue = _EntDtView.ViewScaleDefault;
                if (this.cboScale.SelectedIndex == -1)
                {
                    this.cboScale.SelectedIndex = 0;
                }
            }

            if (this.cboDetailLevel.Items.Count > 0)
            {
                this.cboDetailLevel.SelectedValue = _EntDtView.ViewDetailLevel;
                if (this.cboDetailLevel.SelectedIndex == -1)
                {
                    this.cboDetailLevel.SelectedIndex = 0;
                }
            }
        }

        /// ================================================================================
        /// <summary>フォームのデータ取得</summary>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void GetData()
        {
            _EntDtView.DuplicateViewOpt = RDBDuplicateView;

            _EntDtWinDoorType.IdDoorTag = -1;
            if (this.cboDoorTag.SelectedValue != null)
            {
                _EntDtWinDoorType.IdDoorTag = (int)this.cboDoorTag.SelectedValue;
            }

            _EntDtWinDoorType.IdWindowTag = -1;
            if (this.cboWindowTag.SelectedValue != null)
            {
                _EntDtWinDoorType.IdWindowTag = (int)this.cboWindowTag.SelectedValue;
            }

            _EntDtView.ViewScaleDefault = 0;
            if (this.cboScale.SelectedValue != null)
            {
                _EntDtView.ViewScaleDefault = (int)this.cboScale.SelectedValue;
            }

            _EntDtView.ViewDetailLevel = (int)Revit.DB.ViewDetailLevel.Medium;
            if (this.cboDetailLevel.SelectedValue != null) {
                _EntDtView.ViewDetailLevel = (int)this.cboDetailLevel.SelectedValue;
            }

            _EntDtCmd.Data[0] = _EntDtWinDoorType.IdDoorTag.ToString();
            _EntDtCmd.Data[1] = _EntDtWinDoorType.IdWindowTag.ToString();
            _EntDtCmd.Data[2] = _EntDtView.DuplicateViewOpt.ToString();
            _EntDtCmd.Data[3] = _EntDtView.ViewScaleDefault.ToString();
            _EntDtCmd.Data[4] = _EntDtView.ViewScaleCustom.ToString();
            _EntDtCmd.Data[5] = _EntDtView.ViewDetailLevel.ToString();
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

            if (this.errPvd.GetError(this.cboDoorTag) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.cboWindowTag) != "")
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
            this.errPvd.SetError(this.cboDoorTag, _EntDtView.SetErrPvdCboSelectedValue(this.cboDoorTag.SelectedValue));
            this.errPvd.SetError(this.cboWindowTag, _EntDtView.SetErrPvdCboSelectedValue(this.cboWindowTag.SelectedValue));
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>ビューが重複している時のオプション</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        int RDBDuplicateView
        {
            get
            {
                int ret = 0;

                if (this.rdbViewDelOld.Checked == true)
                {
                    ret = 0;
                }
                else if (this.rdbViewNotUndate.Checked == true)
                {
                    ret = 1;
                }
                else if (this.rdbViewChangeOld.Checked == true)
                {
                    ret = 2;
                }
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        this.rdbViewDelOld.Checked = true;
                        break;

                    case 1:
                        this.rdbViewNotUndate.Checked = true;
                        break;

                    case 2:
                        this.rdbViewChangeOld.Checked = true;
                        break;
                }
            }
        }

        #endregion Properties

        // イベント
        #region Events

        /// ================================================================================
        /// <summary>Handles the Load event of the FormCreatePartsDrawing control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void FormCreatePartsDrawing_Load(object sender, EventArgs e)
        {
            SetText();
            SetData();
            _FlagformScale = true;
        }

        /// ================================================================================
        /// <summary>Processes a command key</summary>
        ///
        /// <param name="msg"     >A <see cref="T:System.Windows.Forms.Message"/>, passed by reference, that represents the Win32 message to process.</param>
        /// <param name="keyData" >One of the <see cref="T:System.Windows.Forms.Keys"/> values that represents the key to process.</param>
        ///
        /// <returns>
        /// true if the keystroke was processed and consumed by the control; otherwise, false to allow further processing.
        /// </returns>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected override
        bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                switch (keyData)
                {
                    case Keys.Shift | Keys.D:
                        this.cboDoorTag.Focus();
                        break;

                    case Keys.Shift | Keys.W:
                        this.cboWindowTag.Focus();
                        break;

                    case Keys.Shift | Keys.S:
                        this.cboScale.Focus();
                        break;

                    case Keys.Shift | Keys.L:
                        this.cboDetailLevel.Focus();
                        break;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
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
        /// <summary>Handles the Validated event of the cboDoorTag control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void cboDoorTag_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.cboDoorTag, _EntDtView.SetErrPvdCboSelectedValue(this.cboDoorTag.SelectedValue));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the cboWindowTag control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>]
        /// ================================================================================
        private
        void cboWindowTag_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.cboWindowTag, _EntDtView.SetErrPvdCboSelectedValue(this.cboWindowTag.SelectedValue));
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the cboScale_ control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2010/07/15 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void cboScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_FlagformScale == true)
            {
                if ((int)this.cboScale.SelectedValue == 0)
                {
                    RvtExtApp.CreateAndEdit.FormScaleCustom form = new RvtExtApp.CreateAndEdit.FormScaleCustom(_CmpAttribute,
                                                                                                               _EntDtView,
                                                                                                               _EntDtCmd);
                    form.ShowDialog();
                }
            }
        }

        #endregion Events
    }
}
