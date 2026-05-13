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
using System.Linq;
using System.Reflection;

namespace ADSK.JExtRAC.GridDimension.UI
{
    /// ================================================================================
    /// <summary>画面 設定</summary>
    /// <history>2011/11/29 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormConfig : Form
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル - コマンド</summary>
        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        /// <summary>List all type of dimension</summary>
        private Collections.Generic.Dictionary<Revit.DB.DimensionType, string> _Dic_dimensionType;

        /// <summary>Select dimension type</summary>
        private Revit.DB.DimensionType _SelectedType;

        /// <summary>Direction of grid</summary>
        private int _optDirection;

        /// <summary>Curve of grid</summary>
        private bool _isCurveDim = true;

        /// <summary> Select left dimension draw</summary>
        private bool _SelectedLeft = false;

        /// <summary> Select right dimension draw</summary>
        private bool _SelectedRight = false;

        /// <summary> Select top dimension draw</summary>
        private bool _SelectedTop = false;

        /// <summary> Select left dimension draw</summary>
        private bool _SelectedBottom = false;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="entDtCmd">データテーブル - コマンド</param>
        ///<param name="optDirection">direction</param>
        ///
        /// <history>2011/11/29 Created GSA,Inc. Shinichi Ishii</history>
        /// <history>2021/12/20 Modified Applied Technology</history>
        /// ================================================================================
        public FormConfig(RvtExtApp.Components.Attribute cmpAttribute,
                          RvtExtApp.Entities.DtCmd entDtCmd,
                          Collections.Generic.IList<Revit.DB.DimensionType> list_dimensionType, bool isCurveDim, int optDirection)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _EntDtCmd = entDtCmd;
            _Dic_dimensionType = list_dimensionType.ToDictionary(x => x, x => x.Name);
            _isCurveDim = isCurveDim;
            _optDirection = optDirection;
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
        /// <history>2018/12/11 Modified Applied Technology</history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_GRIDDIMENSION") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.gpbDist.Text = _CmpAttribute.ResourceText("IDS_TXT_DIST");
            this.lblA.Text = _CmpAttribute.ResourceText("IDS_TXT_A");
            this.lblAUnit.Text = _CmpAttribute.ResourceText("IDS_UNIT_MM");
            this.lblB.Text = _CmpAttribute.ResourceText("IDS_TXT_B");
            this.lblBUnit.Text = _CmpAttribute.ResourceText("IDS_UNIT_MM");
            this.ckbMultiView.Text = _CmpAttribute.ResourceText("IDS_TXT_MULTIVIEW");
            this.lblType.Text = _CmpAttribute.ResourceText("IDS_TXT_TYPE");
            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
            if (_isCurveDim)
            {
                this.picDist.Image = _CmpAttribute.ResourceImage("IDI_PIC_GRID_CURVE") as System.Drawing.Image;
            }
            else
                this.picDist.Image = _CmpAttribute.ResourceImage("IDI_PIC_GRID") as System.Drawing.Image;

            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// <history>2018/12/11 Modified Applied Technology</history>
        /// ================================================================================
        private void SetData()
        {
            // 初期化
            double dValue = 0.0;
            string sValue = "";
            bool checkValue = false;

            // 既存値
            if (_EntDtCmd.Data.Count >= 4)
            {
                // A
                if (!_isCurveDim)
                {
                    sValue = _EntDtCmd.Data[0];
                    dValue = 0.0;
                    if (double.TryParse(sValue, out double da))
                        dValue = da;
                    this.txtA.Text = dValue.ToString();
                }
                else
                {
                    this.txtA.ResetText();
                    this.txtA.Enabled = false;
                }

                // B
                sValue = _EntDtCmd.Data[1];
                dValue = 0.0;
                if (_isCurveDim)
                    dValue = 1;
                if (double.TryParse(sValue, out double dvb))
                    dValue = dvb;

                // multi view
                sValue = _EntDtCmd.Data[2];

                if (bool.TryParse(sValue, out bool cb))
                    checkValue = cb;
                this.ckbMultiView.Checked = checkValue;

                cbDimensionType.DataSource = new BindingSource(_Dic_dimensionType, null);
                cbDimensionType.DisplayMember = "Value";
                cbDimensionType.ValueMember = "Key";

                // type
                sValue = _EntDtCmd.Data[3];
                if (!string.IsNullOrEmpty(sValue))
                {
                    if (_Dic_dimensionType.ContainsValue(sValue))
                        cbDimensionType.SelectedIndex = _Dic_dimensionType.Values.ToList().IndexOf(sValue);
                }

                // check box left
                sValue = _EntDtCmd.Data[4];
                if (sValue == "True")
                    ckbLeft.Checked = true;
                else
                    ckbLeft.Checked = false;

                //check box right
                sValue = _EntDtCmd.Data[5];
                if (sValue == "True")
                    ckbRight.Checked = true;
                else
                    ckbRight.Checked = false;

                //check box top
                sValue = _EntDtCmd.Data[6];
                if (sValue == "True")
                    ckbTop.Checked = true;
                else
                    ckbTop.Checked = false;

                //check box bottom
                sValue = _EntDtCmd.Data[7];
                if (sValue == "True")
                    ckbBottom.Checked = true;
                else
                    ckbBottom.Checked = false;
                if (!_isCurveDim)
                {
                    // on off check box direction
                    if (_optDirection == 0)
                    {
                        this.ckbBottom.Enabled = false;
                        this.ckbTop.Enabled = false;
                        this.ckbBottom.Checked = false;
                        this.ckbTop.Checked = false;
                    }
                    else if (_optDirection == 1)
                    {
                        this.ckbLeft.Enabled = false;
                        this.ckbRight.Enabled = false;
                        this.ckbLeft.Checked = false;
                        this.ckbRight.Checked = false;
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>フォームのデータ取得</summary>
        ///
        /// <history><p>2011/11/26 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/21/10 Modified Applied Technology<p></history>
        /// ================================================================================
        private void GetData()
        {
            // 初期化
            double dValue = 0.0;
            string sValue = "";

            // 既存値
            if (_EntDtCmd.Data.Count >= 6)
            {
                // A
                dValue = 0.0;
                sValue = this.txtA.Text;
                if (double.TryParse(sValue, out double dva))
                    dValue = dva;
                if (dValue < 0.0)
                {
                    System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_ANO") + _CmpAttribute.ResourceText("IDS_ERR_VALMORE0"), _CmpAttribute.ResourceText("IDS_TXT_ERROR"), MessageBoxButtons.OK);
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }

                _EntDtCmd.Data[0] = dValue.ToString();

                // B
                dValue = 0.0;
                if (_isCurveDim)
                    dValue = 1;
                sValue = this.txtB.Text;
                if (double.TryParse(sValue, out double dvb2))
                {
                    dValue = dvb2;
                    if (_isCurveDim && dValue < 1)
                        dValue = 1;
                }
                if (dValue < 0.0)
                {
                    System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_BNO") + _CmpAttribute.ResourceText("IDS_ERR_VALLARGE0"), _CmpAttribute.ResourceText("IDS_TXT_ERROR"), MessageBoxButtons.OK);
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }

                _EntDtCmd.Data[1] = dValue.ToString();

                _EntDtCmd.Data[2] = ckbMultiView.Checked.ToString();

                _SelectedType = (Revit.DB.DimensionType)cbDimensionType.SelectedValue;

                _EntDtCmd.Data[3] = _SelectedType.Name.ToString();

                // get data checkbox

                if (ckbLeft.Checked)
                    _SelectedLeft = true;
                if (ckbRight.Checked)
                    _SelectedRight = true;
                if (ckbTop.Checked)
                    _SelectedTop = true;
                if (ckbBottom.Checked)
                    _SelectedBottom = true;

                if (ckbLeft.Checked)
                    _EntDtCmd.Data[4] = "True";
                else
                    _EntDtCmd.Data[4] = "False";

                if (ckbRight.Checked)
                    _EntDtCmd.Data[5] = "True";
                else
                    _EntDtCmd.Data[5] = "False";

                if (ckbTop.Checked)
                    _EntDtCmd.Data[6] = "True";
                else
                    _EntDtCmd.Data[6] = "False";

                if (ckbBottom.Checked)
                    _EntDtCmd.Data[7] = "True";
                else
                    _EntDtCmd.Data[7] = "False";

                this.DialogResult = DialogResult.OK;
            }
        }

        // get selected dimension type
        /// ================================================================================
        /// <summary>get dimension type</summary>
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        public Revit.DB.DimensionType GetSelectDimensionType
        {
            get
            {
                return _SelectedType;
            }
        }

        /// ================================================================================
        /// <summary>get selected checkbox draw </summary>
        /// <param name="isCurveDim"> is grid arc or not</param>
        /// <returns></returns>
        /// <history>2021/10/21 Created Applied Technology</history>
        /// ================================================================================
        public bool GetSelectedCheckBox()
        {
            if (!_SelectedLeft && !_SelectedRight
                && !_SelectedTop && !_SelectedBottom)
                return true;
            return false;
        }

        /// ================================================================================
        /// <summary>get selected checkbox draw left</summary>
        /// <history>2021/10/21 Created Applied Technology</history>
        /// ================================================================================
        public bool GetSelectLeft
        {
            get
            {
                return _SelectedLeft;
            }
        }

        /// ================================================================================
        /// <summary>get selected checkbox draw right</summary>
        /// <history>2021/10/21 Created Applied Technology</history>
        /// ================================================================================
        public bool GetSelectRight
        {
            get
            {
                return _SelectedRight;
            }
        }

        /// ================================================================================
        /// <summary>get selected checkbox draw top</summary>
        /// <history>2021/10/21 Created Applied Technology</history>
        /// ================================================================================
        public bool GetSelectTop
        {
            get
            {
                return _SelectedTop;
            }
        }

        /// ================================================================================
        /// <summary>get selected checkbox draw bottom</summary>
        /// <history>2021/10/21 Created Applied Technology</history>
        /// ================================================================================
        public bool GetSelectBottom
        {
            get
            {
                return _SelectedBottom;
            }
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
        }

        /// ================================================================================
        /// <summary>Auto resize combobox</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2019/1/17 Created Applied Technology</history>
        /// ================================================================================
        private void cbDimensionType_DropDown(object sender, EventArgs e)
        {
            try
            {
                object[] items = new object[cbDimensionType.Items.Count];
                cbDimensionType.Items.CopyTo(items, 0);
                cbDimensionType.DropDownWidth = items.Select(obj => TextRenderer.MeasureText(cbDimensionType.GetItemText(obj), cbDimensionType.Font).Width).Max();
            }
            catch { }
        }

        #endregion Events
    }
}