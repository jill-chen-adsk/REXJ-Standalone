
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule;
using System.Reflection;

namespace ADSK.JExtRAC.AreaSchedule.RoomConvertedToArea
{
    /// ================================================================================
    /// <summary>画面 作業の選択</summary>
    /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormChoiceWork : Form
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル - 部屋</summary>
        private RvtExtApp.Entities.DtRoom _EntDtRoom;

        /// <summary>データテーブル - エリア</summary>
        private RvtExtApp.Entities.DtArea _EntDtArea;

        /// <summary>データテーブル - コマンド</summary>
        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="entDtRoom"   >データテーブル - 部屋</param>
        /// <param name="entDtArea"   >データテーブル - エリア</param>
        /// <param name="entDtCmd"    >データテーブル - コマンド</param>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public FormChoiceWork(RvtExtApp.Components.Attribute cmpAttribute,
                              RvtExtApp.Entities.DtRoom entDtRoom,
                              RvtExtApp.Entities.DtArea entDtArea,
                              RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;
            _EntDtRoom = entDtRoom;
            _EntDtArea = entDtArea;
            _EntDtCmd = entDtCmd;
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        ///
        /// <history><p>2011/08/01 Created GSA,Inc. Shinichi Ishii</p>
        ///           <p>2015/09/07 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_CHOICEWORK") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.chkConvertArea.Text = _CmpAttribute.ResourceText("IDS_TXT_CONVERTAREABOUNDARY") + "(&A)";
            this.gpbAreaCalc.Text = _CmpAttribute.ResourceText("IDS_TXT_AREACALC");
            this.rdbWallFinish.Text = _CmpAttribute.ResourceText("IDS_TXT_WALLFINISH") + "(&F)";
            this.rdbWallCenter.Text = _CmpAttribute.ResourceText("IDS_TXT_WALLCENTER") + "(&N)";
            this.rdbWallCoreLayer.Text = _CmpAttribute.ResourceText("IDS_TXT_WALLCORELAYER") + "(&L)";
            this.rdbWallCoreCenter.Text = _CmpAttribute.ResourceText("IDS_TXT_WALLCORECENTER") + "(&C)";
            this.chkAddAreaTag.Text = _CmpAttribute.ResourceText("IDS_TXT_ADDAREATAG") + "(&T)";
            this.lblTag.Text = _CmpAttribute.ResourceText("IDS_TXT_TAG");
            this.gpbTagName.Text = _CmpAttribute.ResourceText("IDS_TXT_NAME");
            this.rdbUseRoomName.Text = _CmpAttribute.ResourceText("IDS_TXT_USEROOMNAME");
            this.rdbUseRoomNo.Text = _CmpAttribute.ResourceText("IDS_TXT_USEROOMNO");
            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");

            //this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
        }

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetData()
        {
            this.chkConvertArea.Checked = _EntDtRoom.ChkConvertArea;
            this.chkAddAreaTag.Checked = _EntDtArea.ChkAddAreaTag;
            RdbAreaCalc = _EntDtRoom.GetRoomBndLocTypeNo();
            RdbTagName = _EntDtArea.TagNameOpt;
            this.cboTag.DataSource = _EntDtArea.DataAreaTags;
            this.cboTag.DisplayMember = _EntDtArea.DataAreaTags.Columns[1].ColumnName;
            this.cboTag.ValueMember = _EntDtArea.DataAreaTags.Columns[0].ColumnName;

            if (this.cboTag.Items.Count > 0)
            {
                this.cboTag.SelectedValue = _EntDtArea.TagID;
                if (this.cboTag.SelectedIndex == -1)
                {
                    this.cboTag.SelectedIndex = 0;
                }
            }

            ChangeGpdTag(this.chkConvertArea.Checked);
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void GetData()
        {
            _EntDtRoom.ChkConvertArea = this.chkConvertArea.Checked;
            _EntDtArea.ChkAddAreaTag = this.chkAddAreaTag.Checked;
            _EntDtRoom.SetRoomBndLocType(RdbAreaCalc);
            _EntDtArea.TagNameOpt = RdbTagName;
            _EntDtArea.TagID = -1;
            if (this.cboTag.SelectedValue != null)
            {
                _EntDtArea.TagID = (int)this.cboTag.SelectedValue;
            }

            _EntDtCmd.Data[0] = Convert.ToByte(_EntDtRoom.ChkConvertArea).ToString();
            _EntDtCmd.Data[1] = Convert.ToByte(_EntDtArea.ChkAddAreaTag).ToString();
            _EntDtCmd.Data[2] = _EntDtArea.TagID.ToString();
            _EntDtCmd.Data[3] = _EntDtArea.TagNameOpt.ToString();
        }

        /// ================================================================================
        /// <summary>タグ名オプションの値の変更</summary>
        ///
        /// <param name="type">タグ名オプションの値</param>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void ChangeGpdTag(bool type)
        {
            bool flag = type;

            if (this.cboTag.Items.Count == 0)
            {
                flag = false;
            }

            if (flag == false)
            {
                this.chkAddAreaTag.Checked = flag;
            }
            this.chkAddAreaTag.Enabled = flag;
            this.gpbTag.Enabled = flag;
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>部屋の境界線配置タイプ</summary>
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        int RdbAreaCalc
        {
            get
            {
                int ret = 0;

                if (this.rdbWallFinish.Checked == true)
                {
                    ret = 0;
                }
                else if (this.rdbWallCenter.Checked == true)
                {
                    ret = 1;
                }
                else if (this.rdbWallCoreLayer.Checked == true)
                {
                    ret = 2;
                }
                else if (this.rdbWallCoreCenter.Checked == true)
                {
                    ret = 3;
                }
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        this.rdbWallFinish.Checked = true;
                        break;

                    case 1:
                        this.rdbWallCenter.Checked = true;
                        break;

                    case 2:
                        this.rdbWallCoreLayer.Checked = true;
                        break;

                    case 3:
                        this.rdbWallCoreCenter.Checked = true;
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary>タグ名オプション</summary>
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        int RdbTagName
        {
            get
            {
                int ret = 0;

                if (this.rdbUseRoomName.Checked == true)
                {
                    ret = 0;
                }
                else if (this.rdbUseRoomNo.Checked == true)
                {
                    ret = 1;
                }
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        this.rdbUseRoomName.Checked = true;
                        break;

                    case 1:
                        this.rdbUseRoomNo.Checked = true;
                        break;
                }
            }
        }

        #endregion Properties

        // イベント
        #region Events

        /// ================================================================================
        /// <summary>Handles the Load event of the FormChoiceWork control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void FormChoiceWork_Load(object sender, EventArgs e)
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
            GetData();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        /// ================================================================================
        /// <summary>Handles the CheckedChanged event of the chkConvertArea control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void chkConvertArea_CheckedChanged(object sender, EventArgs e)
        {
            ChangeGpdTag(this.chkConvertArea.Checked);
        }

        #endregion Events
    }
}