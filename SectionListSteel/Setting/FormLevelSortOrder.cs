using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Reflection;
namespace SectionListSteel.Setting
{
    /// ================================================================================
    /// <summary>フォーム 階記号ソート</summary>
    /// ================================================================================
    public partial class FormLevelSortOrder : Form
    {
        // メンバ変数
        #region Member Variables

        /// <summary>属性</summary>
        private SectionListSteel.Components.Attribute _CmpAttribute;

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
        /// <param name="cmpAttribute"      >属性</param>
        /// <param name="setLevelSortrOrder">ソート済み階記号</param>
        /// <param name="allLevelAry"       >全レベル名</param>
        ///
        /// <hisotry>2016/08/31 Created GSA,Inc. Ryo Kuroda</hisotry>
        /// ================================================================================
        public FormLevelSortOrder(SectionListSteel.Components.Attribute cmpAttribute,
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
        /// <summary>データ設定</summary>
        ///
        /// <hisotry>2016/08/31 Created GSA,Inc. Ryo Kuroda</hisotry>
        /// ================================================================================
        private
        void SetData()
        {
            this.listBoxAllLevel.Sorted = false;

            // ソート済み階記号を分割
            Collections.Generic.IList<string> sorted = new Collections.Generic.List<string>();

            while (_LevelSortOrder != "") {
                if (_LevelSortOrder.Contains("/SortOrder")) {
                    string str = _LevelSortOrder.Substring(0, _LevelSortOrder.IndexOf("/SortOrder"));
                    _LevelSortOrder = _LevelSortOrder.Substring(_LevelSortOrder.IndexOf("/SortOrder") + 10);

                    sorted.Add(str);
                }
                else {
                    sorted.Add(_LevelSortOrder);

                    break;
                }
            }

            // ソート済みを追加
            foreach (string str in sorted) {
                if (this.listBoxAllLevel.Items.Contains(str) == false) {
                    this.listBoxAllLevel.Items.Add(str);
                }
            }
            // 未追加分を追加
            foreach (string str in _AllLevels) {
                if (this.listBoxAllLevel.Items.Contains(str) == false) {
                    this.listBoxAllLevel.Items.Add(str);
                }
            }
        }

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <hisotry><p>2016/08/31 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2017/07/04 Modified CST,Co.Ltd. Ryo Kuroda</p></hisotry>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_BTN_LEVELSORTORDER_TEXT") ;
            this.lblAllLevel.Text = _CmpAttribute.ResourceText("IDS_TXT_LISTLEVELSORT");
            this.btnUp.Text = _CmpAttribute.ResourceText("IDS_TXT_UP");
            this.btnDown.Text = _CmpAttribute.ResourceText("IDS_TXT_DOWN");
            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>リストボックスのデータ</summary>
        ///
        /// <history>2016/08/31 Created GSA,Inc Ryo Kuroda</history>
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

        /// ================================================================================
        /// <summary>ロード</summary>
        /// ================================================================================
        private void FormLevelSortOrder_Load(object sender, EventArgs e)
        {
            SetText();
        }

        /// ================================================================================
        /// <summary>上へ</summary>
        /// ================================================================================
        private void btnUp_Click(object sender, EventArgs e)
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

        /// ================================================================================
        /// <summary>下へ</summary>
        /// ================================================================================
        private void btnDown_Click(object sender, EventArgs e)
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

        /// ================================================================================
        /// <summary>OK</summary>
        /// ================================================================================
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

            this.Close();
        }

        /// ================================================================================
        /// <summary>キャンセル</summary>
        /// ================================================================================
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.Close();
        }

        #endregion Events
    }
}