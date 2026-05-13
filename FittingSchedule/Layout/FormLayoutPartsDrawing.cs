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

namespace ADSK.JExtRAC.FittingSchedule.Layout
{
    /// ================================================================================
    /// <summary>画面 建具姿図レイアウト</summary>
    /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormLayoutPartsDrawing : Form
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル - ビューシート</summary>
        private RvtExtApp.Entities.DtViewSheet _EntDtViewSheet;

        /// <summary>データテーブル - コマンド</summary>
        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        /// <param name="entDtViewSheet">データテーブル - ビューシート</param>
        /// <param name="entDtCmd"      >データテーブル - コマンド</param>
        ///
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public FormLayoutPartsDrawing(RvtExtApp.Components.Attribute cmpAttribute,
                                      RvtExtApp.Entities.DtViewSheet entDtViewSheet,
                                      RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;
            _EntDtViewSheet = entDtViewSheet;
            _EntDtCmd = entDtCmd;
            this.errPvd.SetIconAlignment(this.lstPlacementSolidPicture, ErrorIconAlignment.BottomLeft);
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        ///
        /// <history><p>2009/05/25 Created  GSA,Inc. Shinichi Ishii</p>
        ///           <p>2015/09/07 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_LAYOUTPARTSDRAWING") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.lblSolidPicture.Text = _CmpAttribute.ResourceText("IDS_TXT_SOLIDPICTURE");
            this.lblPlacementSolidPicture.Text = _CmpAttribute.ResourceText("IDS_TXT_PLACEMENTSOLIDPICTURE");
            this.btnMove.Text = _CmpAttribute.ResourceText("IDS_TXT_SIGNMOVE");
            this.btnDel.Text = _CmpAttribute.ResourceText("IDS_TXT_SIGNDEL");
            this.btnUp.Text = _CmpAttribute.ResourceText("IDS_TXT_SIGNUP");
            this.btnDn.Text = _CmpAttribute.ResourceText("IDS_TXT_SIGNDN");
            this.rdbWindow.Text = _CmpAttribute.ResourceText("IDS_TXT_WINDOW");
            this.rdbDoor.Text = _CmpAttribute.ResourceText("IDS_TXT_DOOR");
            this.rdbBoth.Text = _CmpAttribute.ResourceText("IDS_TXT_BOTH");
            this.lblBlank.Text = _CmpAttribute.ResourceText("IDS_TXT_BLANK");
            this.lblTop.Text = _CmpAttribute.ResourceText("IDS_TXT_TOP") + ":";
            this.lblBottom.Text = _CmpAttribute.ResourceText("IDS_TXT_BOTTOM") + ":";
            this.lblLeft.Text = _CmpAttribute.ResourceText("IDS_TXT_LEFT") + ":";
            this.lblRight.Text = _CmpAttribute.ResourceText("IDS_TXT_RIGHT") + ":";
            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");

            this.btnNewLine.Text = _CmpAttribute.ResourceText("IDS_TXT_NEWLINE");

            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetData()
        {
            RdbViewType = _EntDtViewSheet.ViewTypeOpt;

            this.txtTop.Text = _EntDtViewSheet.BlankTop.ToString();
            this.txtBottom.Text = _EntDtViewSheet.BlankBottom.ToString();
            this.txtLeft.Text = _EntDtViewSheet.BlankLeft.ToString();
            this.txtRight.Text = _EntDtViewSheet.BlankRight.ToString();

            this.lstSolidPicture.DataSource = _EntDtViewSheet.DataViewExist;
            this.lstSolidPicture.DisplayMember = _EntDtViewSheet.DataViewExist.Columns[1].ColumnName;
            this.lstSolidPicture.ValueMember = _EntDtViewSheet.DataViewExist.Columns[0].ColumnName;

            this.lstPlacementSolidPicture.DataSource = _EntDtViewSheet.DataViewTarget;
            this.lstPlacementSolidPicture.DisplayMember = _EntDtViewSheet.DataViewTarget.Columns[1].ColumnName;
            this.lstPlacementSolidPicture.ValueMember = _EntDtViewSheet.DataViewTarget.Columns[0].ColumnName;

            ChangeBtnEnabled();
        }

        /// ================================================================================
        /// <summary>フォームのデータ取得</summary>
        ///
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void GetData()
        {
            _EntDtViewSheet.ViewTypeOpt = RdbViewType;
            _EntDtViewSheet.BlankTop = int.Parse(this.txtTop.Text);
            _EntDtViewSheet.BlankBottom = int.Parse(this.txtBottom.Text);
            _EntDtViewSheet.BlankLeft = int.Parse(this.txtLeft.Text);
            _EntDtViewSheet.BlankRight = int.Parse(this.txtRight.Text);

            _EntDtCmd.Data[0] = _EntDtViewSheet.ViewTypeOpt.ToString();
            _EntDtCmd.Data[1] = _EntDtViewSheet.BlankTop.ToString();
            _EntDtCmd.Data[2] = _EntDtViewSheet.BlankBottom.ToString();
            _EntDtCmd.Data[3] = _EntDtViewSheet.BlankLeft.ToString();
            _EntDtCmd.Data[4] = _EntDtViewSheet.BlankRight.ToString();
        }

        /// ================================================================================
        /// <summary>ボタンを有効に変更</summary>
        ///
        /// <history>2011/08/03 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void ChangeBtnEnabled()
        {
            bool flagMove = false;
            bool flagDel = false;
            bool flagUpDn = false;

            bool flagNewLine = false;

            if (this.lstSolidPicture.Items.Count > 0)
            {
                flagMove = true;
            }

            if (this.lstPlacementSolidPicture.Items.Count >= 0)
            {
                flagNewLine = true;
            }

            if (this.lstPlacementSolidPicture.Items.Count > 0)
            {
                flagDel = true;
            }

            if (this.lstPlacementSolidPicture.Items.Count > 1)
            {
                flagUpDn = true;
            }

            this.btnMove.Enabled = flagMove;
            this.btnDel.Enabled = flagDel;
            this.btnUp.Enabled = flagUpDn;
            this.btnDn.Enabled = flagUpDn;

            this.btnNewLine.Enabled = flagNewLine;
        }

        /// ================================================================================
        /// <summary>リストボックスのフィルター</summary>
        ///
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void FilterListBox()
        {
            _EntDtViewSheet.FilterViewTable(RdbViewType);

            ChangeBtnEnabled();
        }

        /// ================================================================================
        /// <summary>フォームのエラープロバイダ取得</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = エラーなし</p>
        ///             <p>False = エラーあり</p></returns>
        ///
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool GetErrPvd()
        {
            bool ret = false;

            if (this.errPvd.GetError(this.lstPlacementSolidPicture) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtTop) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtBottom) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtLeft) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtRight) != "")
            {
                return ret;
            }
            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>フォームのエラープロバイダチェック</summary>
        ///
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void ChkErrPvd()
        {
            this.errPvd.SetError(this.lstPlacementSolidPicture, _EntDtViewSheet.SetErrPvdListBox(this.lstPlacementSolidPicture));
            this.errPvd.SetError(this.txtTop, _EntDtViewSheet.SetErrPvdBlankText(this.txtTop.Text.Trim()));
            this.errPvd.SetError(this.txtBottom, _EntDtViewSheet.SetErrPvdBlankText(this.txtBottom.Text.Trim()));
            this.errPvd.SetError(this.txtLeft, _EntDtViewSheet.SetErrPvdBlankText(this.txtLeft.Text.Trim()));
            this.errPvd.SetError(this.txtRight, _EntDtViewSheet.SetErrPvdBlankText(this.txtRight.Text.Trim()));
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>ビュータイプオプション</summary>
        /// <history>2011/08/03 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        int RdbViewType
        {
            get
            {
                int ret = 0;

                if (this.rdbWindow.Checked == true)
                {
                    ret = 0;
                }
                else if (this.rdbDoor.Checked == true)
                {
                    ret = 1;
                }
                else if (this.rdbBoth.Checked == true)
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
                        this.rdbWindow.Checked = true;
                        break;

                    case 1:
                        this.rdbDoor.Checked = true;
                        break;

                    case 2:
                        this.rdbBoth.Checked = true;
                        break;
                }
            }
        }

        #endregion Properties

        // イベント
        #region Events

        /// ================================================================================
        /// <summary>Handles the Load event of the FormLayoutPartsDrawing control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void FormLayoutPartsDrawing_Load(object sender, EventArgs e)
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
        /// <summary>Handles the Validated event of the lstPlacementSolidPicture control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void lstPlacementSolidPicture_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.lstPlacementSolidPicture, _EntDtViewSheet.SetErrPvdListBox(this.lstPlacementSolidPicture));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtTop control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void txtTop_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtTop, _EntDtViewSheet.SetErrPvdBlankText(this.txtTop.Text.Trim()));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtBottom control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void txtBottom_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtBottom, _EntDtViewSheet.SetErrPvdBlankText(this.txtBottom.Text.Trim()));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtLeft control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void txtLeft_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtLeft, _EntDtViewSheet.SetErrPvdBlankText(this.txtLeft.Text.Trim()));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtRight control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void txtRight_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtRight, _EntDtViewSheet.SetErrPvdBlankText(this.txtRight.Text.Trim()));
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnMove control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnMove_Click(object sender, EventArgs e)
        {
            int index = this.lstSolidPicture.SelectedIndex;
            if (index > -1)
            {
                System.Object selectVal = _EntDtViewSheet.MoveViewTable(index);
                if (selectVal != null)
                {
                    this.lstPlacementSolidPicture.SelectedValue = selectVal;
                }
                ChangeBtnEnabled();
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnDel control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnDel_Click(object sender, EventArgs e)
        {
            int index = this.lstPlacementSolidPicture.SelectedIndex;
            if (index > -1)
            {
                System.Object selectVal = _EntDtViewSheet.DelViewTable(index);
                if (selectVal != null)
                {
                    this.lstSolidPicture.SelectedValue = selectVal;
                }
                ChangeBtnEnabled();
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnUp control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/06/22 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnUp_Click(object sender, EventArgs e)
        {
            int index = this.lstPlacementSolidPicture.SelectedIndex;
            if (index > -1)
            {
                System.Object selectVal = _EntDtViewSheet.UpDnViewTable(index, true);

                if ((index - 1) > -1)
                {
                    this.lstPlacementSolidPicture.SelectedIndex = index - 1;
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnDn control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/06/22 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnDn_Click(object sender, EventArgs e)
        {
            int index = this.lstPlacementSolidPicture.SelectedIndex;
            if (index > -1)
            {
                System.Object selectVal = _EntDtViewSheet.UpDnViewTable(index, false);

                if ((index + 1) < this.lstPlacementSolidPicture.Items.Count)
                {
                    this.lstPlacementSolidPicture.SelectedIndex = index + 1;
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the CheckedChanged event of the rdbWindow control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void rdbWindow_CheckedChanged(object sender, EventArgs e)
        {
            FilterListBox();
        }

        /// ================================================================================
        /// <summary>Handles the CheckedChanged event of the rdbDoor control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void rdbDoor_CheckedChanged(object sender, EventArgs e)
        {
            FilterListBox();
        }

        /// ================================================================================
        /// <summary>Handles the CheckedChanged event of the rdbBoth control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void rdbBoth_CheckedChanged(object sender, EventArgs e)
        {
            FilterListBox();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnNewLine control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/06/20 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnNewLine_Click(object sender, EventArgs e)
        {
            int index = this.lstPlacementSolidPicture.SelectedIndex;

            bool flag = false;
            if (this.lstPlacementSolidPicture.Items.Count > 0)
            {
                if (index > -1)
                {
                    flag = true;
                }
            }
            else
            {
                flag = true;
                index = -1;
            }

            if (flag == true)
            {
                System.Object selectVal = _EntDtViewSheet.AddNewLineTable(index + 1);
                if (selectVal != null)
                {
                    this.lstPlacementSolidPicture.SelectedIndex = index + 1;
                }
                ChangeBtnEnabled();
            }
        }

        #endregion Events
    }
}
