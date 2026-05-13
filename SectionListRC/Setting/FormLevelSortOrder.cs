using System;
using System.Text;
using System.Windows.Forms;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Reflection;
using UTILS = SectionListRC.Utils;

namespace SectionListRC.Setting
{
    /// ================================================================================
    /// <summary>レベルソート順序</summary>
    /// <history>2013/05/24 Created GSA,Inc Ryo Kuroda</history>
    /// ================================================================================
    public partial class FormLevelSortOrder : Form
    {
        // メンバ変数
        #region Member Variables

        /// <summary>属性</summary>
        private SectionListRC.Components.Attribute _CmpAttribute;

        /// <summary>レベルソート順序</summary>
        private string _LevelSortOrder;

        /// <summary>全レベル</summary>
        private Collections.Generic.IList<string> _AllLevels;

        #endregion Member Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        ///
        /// <history>2013/05/24 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public FormLevelSortOrder(SectionListRC.Components.Attribute cmpAttribute,
                                  string setLevelSortrOrder,
                                  Collections.Generic.IList<string> allLevelAry)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _LevelSortOrder = setLevelSortrOrder;
            _AllLevels = allLevelAry;

            SetData();
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        /// <history><p>2013/05/24 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2017/02/16 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_BTN_LEVELSORTORDER_TEXT") ;
            this.lblAllLevel.Text = _CmpAttribute.ResourceText("IDS_TXT_LISTLEVELSORT");
            this.btnUp.Text = _CmpAttribute.ResourceText("IDS_TXT_UP");
            this.btnDown.Text = _CmpAttribute.ResourceText("IDS_TXT_DOWN");
            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        /// <history>2013/05/24 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private
        void SetData()
        {
            this.listBoxAllLevel.Sorted = false;

            Collections.Generic.IList<string> sorted = new Collections.Generic.List<string>();

            while (_LevelSortOrder != "") {
                if (_LevelSortOrder.Contains("/SortOrder")) {
                    string str = _LevelSortOrder.Substring(0, _LevelSortOrder.IndexOf("/SortOrder"));
                    _LevelSortOrder = _LevelSortOrder.Substring(_LevelSortOrder.IndexOf("/SortOrder") + 10);

                    //if (_AllLevels.Contains(str))
                    //{
                    //  this.listBoxAllLevel.Items.Add(str);
                    //}

                    sorted.Add(str);
                }
                else {
                    //if (_AllLevels.Contains(_LevelSortOrder))
                    //{
                    //  this.listBoxAllLevel.Items.Add(_LevelSortOrder);
                    //}

                    sorted.Add(_LevelSortOrder);

                    break;
                }
            }

            foreach (string str in sorted) {
                if (!this.listBoxAllLevel.Items.Contains(str)) {
                    this.listBoxAllLevel.Items.Add(str);
                }
            }
            foreach (string str in _AllLevels) {
                if (!this.listBoxAllLevel.Items.Contains(str)) {
                    this.listBoxAllLevel.Items.Add(str);
                }
            }
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>リストボックスのデータ</summary>
        /// <history>2013/05/24 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public
        string SortedOrder
        {
            get
            {
                string ret = "";

                foreach (object obj in this.listBoxAllLevel.Items) {
                    ret += (string)obj + "/SortOrder";
                }

                return ret;
            }
        }

        #endregion Properties

        // イベント
        #region Events

        // ロード
        private
       void FormLevelSortOrder_Load(object sender, EventArgs e)
        {
            SetText();
        }

        // 上昇
        private
        void btnUp_Click(object sender, EventArgs e)
        {
            if (this.listBoxAllLevel.SelectedItem != null) {
                if (this.listBoxAllLevel.SelectedIndex != 0) {
                    // 選択項目
                    int selNum = this.listBoxAllLevel.SelectedIndex;
                    string selStr = this.listBoxAllLevel.SelectedItem.ToString();

                    // 除外
                    this.listBoxAllLevel.Items.RemoveAt(selNum);

                    // 再挿入(Insert != Add)
                    this.listBoxAllLevel.Items.Insert(selNum - 1, selStr);

                    // 挿入先を選択
                    this.listBoxAllLevel.SelectedIndex = selNum - 1;
                }
            }
        }

        // 下降
        private
        void btnDown_Click(object sender, EventArgs e)
        {
            if (this.listBoxAllLevel.SelectedItem != null) {
                if (this.listBoxAllLevel.SelectedIndex != this.listBoxAllLevel.Items.Count - 1) {
                    // 選択項目
                    int selNum = this.listBoxAllLevel.SelectedIndex;
                    string selStr = this.listBoxAllLevel.SelectedItem.ToString();

                    // 除外
                    this.listBoxAllLevel.Items.RemoveAt(selNum);

                    // 再挿入
                    this.listBoxAllLevel.Items.Insert(selNum + 1, selStr);

                    // 挿入先を選択
                    this.listBoxAllLevel.SelectedIndex = selNum + 1;
                }
            }
        }

        // OK
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

            this.Close();
        }

        // キャンセル
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.Close();
        }

        #endregion Events
    }
}