using System;
using System.Windows.Forms;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Config
{
    /// ================================================================================
    /// <summary>画面 設定</summary>
    /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormConfig : Form
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル - レベル</summary>
        private RvtExtApp.Entities.DtLevel _EntDtLevel;

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="entDtLevel"  >データテーブル - レベル</param>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public FormConfig(RvtExtApp.Components.Attribute cmpAttribute,
                          RvtExtApp.Entities.DtLevel entDtLevel)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _EntDtLevel = entDtLevel;
            SetText();
            SetData();
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_CORRECTFRAMINGPLAN");

            this.lblLevelTitle.Text = _CmpAttribute.ResourceText("IDS_TXT_EXPLANELEM");
            this.lblLevelBase.Text = _CmpAttribute.ResourceText("IDS_TXT_EXPLANELEMBASE");
            this.lblLevelWork.Text = _CmpAttribute.ResourceText("IDS_TXT_EXPLANELEMWORK");
            this.lblLevelExplan.Text = _CmpAttribute.ResourceText("IDS_TXT_WORKHIDDEN");

            this.btnLevelRegist.Text = _CmpAttribute.ResourceText("IDS_TXT_REGISTLEVEL");
            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");

            var iconStream = typeof( FormConfig ).Assembly.GetManifestResourceStream( "RSTExtension.Resources.Images.IDI_SUBS_ICON.ico" ) ;
            if ( iconStream != null ) this.Icon = new System.Drawing.Icon( iconStream ) ;
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii
        ///          2021/10/28 Modified Applied Technology</history>
        /// ================================================================================
        private void SetData()
        {
            _EntDtLevel.GetData(this.cboLevelWork, true);
            _EntDtLevel.GetData(this.cboLevelBase, false);
        }

        /// ================================================================================
        /// <summary>フォームのデータ取得</summary>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii
        ///          021/10/28 Modified Applied Technology</history>
        ///
        /// ================================================================================
        private void GetData()
        {
            _EntDtLevel.GetWorkLevel(this.cboLevelWork);
            _EntDtLevel.GetBaseLevel(this.cboLevelBase);
        }

        #endregion Member Functions

        // プロパティ

        // イベント
        #region Events

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnOK_Click(object sender, EventArgs e)
        {
            // データ取得
            GetData();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnLevelRegist control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii
        ///         2021/10/28 Modified Applied Technology</history>
        /// ================================================================================
        private void btnLevelRegist_Click(object sender, EventArgs e)
        {
            // 画面 レベル登録
            RvtExtApp.Config.FormLevel form = new RvtExtApp.Config.FormLevel(_CmpAttribute, _EntDtLevel);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _EntDtLevel.GetData(this.cboLevelWork, true);
                _EntDtLevel.GetData(this.cboLevelBase, false);
            }
        }

        #endregion Events
    }
}
