using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.GridDimension;
using System.Reflection;

namespace ADSK.JExtRAC.GridDimension.UI
{
    /// ================================================================================
    /// <summary>画面 設定</summary>
    /// <history>2018/12/11 Created Applied Technology</history>
    /// ================================================================================
    public partial class FormSelectView : Form
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル - コマンド</summary>
        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        /// <summary>All view</summary>
        private List<Autodesk.Revit.DB.View> _ViewList;

        /// <summary>current view</summary>
        private Revit.DB.View _CurrentView;

        /// <summary>Last selected index</summary>
        private int _LastIndex = 0;

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="entDtCmd">データテーブル - コマンド</param>
        /// <param name="viewList">all view</param>
        /// <param name="currentView">current view</param>
        ///
        /// <history>2018/12/11 Created Applied Technology</history>
        /// ================================================================================
        public FormSelectView(RvtExtApp.Components.Attribute cmpAttribute,
                        RvtExtApp.Entities.DtCmd entDtCmd, List<Autodesk.Revit.DB.View> viewList, Revit.DB.View currentView)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;

            _EntDtCmd = entDtCmd;

            _ViewList = viewList;

            _CurrentView = currentView;

            SetText();
            SetData();
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        ///
        /// <history>2018/12/11 Created Applied Technology</history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTVIEWS") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.btnOk.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
            cbkSelecAll.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTALLVIEW");

            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2018/12/11 Created Applied Technology</history>
        /// ================================================================================
        private
        void SetData()
        {
            if (_ViewList == null || _ViewList.Count == 0)
                return;

            int index = 0;
            foreach (Revit.DB.View view in _ViewList)
            {
                this.cklView.Items.Add(new RvtExtApp.Entities.ViewItem()
                {
                    Tag = view,
                    Text = view.Title
                });

                // if current view, check = true
                if (view.Id == _CurrentView.Id)
                {
                    cklView.SetItemChecked(index, true);
                }
                index++;
            }

            // set default checkbox
            if (_ViewList.Count == 1)
                cbkSelecAll.CheckState = CheckState.Checked;
            else
                cbkSelecAll.CheckState = CheckState.Indeterminate;
        }

        /// ================================================================================
        /// <summary>フォームのデータ取得</summary>
        ///
        /// <history>2018/12/11 Created Applied Technology</history>
        /// ================================================================================
        private
        void GetData()
        {
            _ViewList.Clear();

            foreach (RvtExtApp.Entities.ViewItem item in this.cklView.CheckedItems)
            {
                if (item.Tag.Id == _CurrentView.Id)
                    _ViewList.Insert(0, item.Tag);
                else
                    _ViewList.Add(item.Tag);
            }
        }

        #endregion Member Functions

        // イベント
        #region Events

        /// ================================================================================
        /// <summary>Click Ok</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2018/12/11 Created Applied Technology</history>
        /// ================================================================================
        private
        void btnOk_Click(object sender, EventArgs e)
        {
            GetData();

            if (_ViewList.Count == 0)
            {
                this.DialogResult = DialogResult.None;
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_NOVIEWSELECT"), _CmpAttribute.ResourceText("IDS_TXT_ERROR"), MessageBoxButtons.OK);
            }
            else
                this.DialogResult = DialogResult.OK;
        }

        /// ================================================================================
        /// <summary>Click Cancel</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2018/12/11 Created Applied Technology</history>
        /// ================================================================================
        private
        void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// ================================================================================
        /// <summary>Update checkbox cbkSelecAll status</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2019/1/17 Created Applied Technology</history>
        /// ================================================================================
        private
        void cklView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // use shift to select
            if (Control.ModifierKeys == Keys.Shift)
            {
                int lastindex = _LastIndex;
                int currentindex = cklView.SelectedIndex;
                int upper = Math.Max(lastindex, currentindex);
                int lower = Math.Min(lastindex, currentindex);
                bool isChecked = cklView.GetItemChecked(currentindex);
                for (int i = lower; i <= upper; i++)
                {
                    cklView.SetItemCheckState(i, isChecked == true ? CheckState.Checked : CheckState.Unchecked);
                }
            }
            _LastIndex = cklView.SelectedIndex;

            int countItem = cklView.Items.Count;
            int checkedItem = cklView.CheckedItems.Count;
            // set control checkbox
            if (checkedItem != 0 && checkedItem != countItem)
                cbkSelecAll.CheckState = CheckState.Indeterminate;
            else
            {
                if (checkedItem == countItem)
                    cbkSelecAll.CheckState = CheckState.Checked;
                else
                    cbkSelecAll.CheckState = CheckState.Unchecked;
            }
        }

        /// ================================================================================
        /// <summary>Select all view</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2019/1/17 Created Applied Technology</history>
        /// ================================================================================
        private
        void cbkSelecAll_CheckedChanged(object sender, EventArgs e)
        {
            if (cbkSelecAll.CheckState != CheckState.Indeterminate)
            {
                for (int i = 0; i < cklView.Items.Count; i++)
                {
                    cklView.SetItemCheckState(i, cbkSelecAll.Checked == true ? CheckState.Checked : CheckState.Unchecked);
                }
            }
        }

        #endregion Events

    }
}