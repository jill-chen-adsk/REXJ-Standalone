
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AveSiteLevelHeightCalc;
using System.Reflection;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Create
{
    /// ================================================================================
    /// <summary>画面 算定図</summary>
    /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormCalcDraw : Form
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル - 注釈</summary>
        private RvtExtApp.Entities.DtAnnotation _EntDtAnnotation;

        /// <summary>データテーブル - コマンド</summary>
        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        /// <param name="entDtAnnotation" >データテーブル - 注釈</param>
        /// <param name="entDtCmd"        >データテーブル - コマンド</param>
        ///
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public FormCalcDraw(RvtExtApp.Components.Attribute cmpAttribute,
                            RvtExtApp.Entities.DtAnnotation entDtAnnotation,
                            RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;
            _EntDtAnnotation = entDtAnnotation;
            _EntDtCmd = entDtCmd;
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        ///
        /// <history><p>2011/08/07 Created GSA,Inc. Shinichi Ishii</p>
        ///           <p>2015/09/07 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_CREATEAVEGLLEVELDRAW") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.gpbCalcPoint.Text = _CmpAttribute.ResourceText("IDS_TXT_CALCPOINT");
            this.btnUp.Text = _CmpAttribute.ResourceText("IDS_TXT_UPSIGN");
            this.btnDn.Text = _CmpAttribute.ResourceText("IDS_TXT_DNSIGN");
            this.btnDel.Text = _CmpAttribute.ResourceText("IDS_TXT_DEL");
            this.btnUpdateNumber.Text = _CmpAttribute.ResourceText("IDS_TXT_UPDATENUMBER");
            this.lblBMHeight.Text = _CmpAttribute.ResourceText("IDS_TXT_BMHEIGHT");
            this.btnUpdateLevel.Text = _CmpAttribute.ResourceText("IDS_TXT_UPDATELEVEL");
            this.gpbCalcDraw.Text = _CmpAttribute.ResourceText("IDS_TXT_CALCDRAW");
            this.lblScale.Text = _CmpAttribute.ResourceText("IDS_TXT_SCALE");
            this.lbl1Slash.Text = _CmpAttribute.ResourceText("IDS_TXT_1SLASH");
            this.lblRate.Text = _CmpAttribute.ResourceText("IDS_TXT_VHRATE");
            this.lblHorizontal.Text = _CmpAttribute.ResourceText("IDS_TXT_HORIZONTAL");
            this.lblVertical.Text = _CmpAttribute.ResourceText("IDS_TXT_VERTICAL");
            this.gpbLengthUnit.Text = _CmpAttribute.ResourceText("IDS_TXT_UNITLENGTH");
            this.rdbLengthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.rdbLengthM.Text = _CmpAttribute.ResourceText("IDS_TXT_M");
            this.gpbAreaDecimal.Text = _CmpAttribute.ResourceText("IDS_TXT_AREADECIMAL");
            this.lblAreaDecimal.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
            this.lblAreaOrder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
            this.gpbAreaRounding.Text = "";
            this.rdbAreaCut.Text = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF");
            this.rdbAreaClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOSE1");
            this.rdbAreaRounding.Text = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");
            this.btnCreate.Text = _CmpAttribute.ResourceText("IDS_TXT_CREATECALCDRAW");
            this.btnClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOSE");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");

            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;

            int colsCount = _EntDtAnnotation.TableAveGlLvlCalcPos.Columns.Count;

            // 列の文字位置
            System.Windows.Forms.DataGridViewContentAlignment alignMidRight = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            System.Windows.Forms.DataGridViewContentAlignment alignMidCenter = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            System.Windows.Forms.DataGridViewContentAlignment alignMidLeft = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;

            // ソートモード
            System.Windows.Forms.DataGridViewColumnSortMode sortModeNon = DataGridViewColumnSortMode.NotSortable;

            for (int i = 0; i < colsCount; ++i)
            {
                string colName = _EntDtAnnotation.TableAveGlLvlCalcPos.Columns[i].ColumnName;
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.SortMode = sortModeNon;
                switch (colName)
                {
                    case "IDCircle":
                        ConfigureCalcPointColumn(col, alignMidLeft, colName, _CmpAttribute.ResourceText("IDS_SYMTAG"), visible: false, width: 0, fillHoriz: false);
                        break;
                    case "IDTag":
                        ConfigureCalcPointColumn(col, alignMidLeft, colName, _CmpAttribute.ResourceText("IDS_SYMTAG"), visible: false, width: 0, fillHoriz: false);
                        break;
                    case "Number":
                        ConfigureCalcPointColumn(col, alignMidLeft, colName, _CmpAttribute.ResourceText("IDS_TXT_NUMBER"), visible: true, width: 50, fillHoriz: false);
                        break;
                    case "Level":
                        ConfigureCalcPointColumn(col, alignMidRight, colName, _CmpAttribute.ResourceText("IDS_TXT_LEVELFROMBM"), visible: true, width: 80, fillHoriz: true);
                        break;
                    default:
                        ConfigureCalcPointColumn(col, alignMidLeft, colName, colName, visible: true, width: 60, fillHoriz: false);
                        break;
                }

                dgvCalcPoint.Columns.Add(col);
            }

            // ヘッダー
            this.dgvCalcPoint.ColumnHeadersHeight = 25;
            this.dgvCalcPoint.ColumnHeadersDefaultCellStyle.Alignment = alignMidCenter;
            this.dgvCalcPoint.AutoGenerateColumns = false;
        }

        private static void ConfigureCalcPointColumn(DataGridViewTextBoxColumn col, DataGridViewContentAlignment alignment,
            string columnName, string headerText, bool visible, int width, bool fillHoriz)
        {
            col.Name = columnName;
            col.DataPropertyName = columnName;
            col.HeaderText = headerText;
            col.Visible = visible;
            col.DefaultCellStyle.Alignment = alignment;
            if (visible && width > 0 && !fillHoriz)
                col.Width = width;
            if (visible && fillHoriz)
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void SetData()
        {
            // BM高さ
            this.txtBMHeight.Text = _EntDtAnnotation.BMHeight.ToString();

            // 古いBM高さ
            this.txtBMHeightOld.Text = this.txtBMHeight.Text;

            // 縮尺
            this.txtScale.Text = _EntDtAnnotation.Scale.ToString();

            // 横比
            this.txtHorizontal.Text = _EntDtAnnotation.RaiteHorizontal.ToString();

            // 縦比
            this.txtVertical.Text = _EntDtAnnotation.RaiteVertical.ToString();

            // データグリッドビュー-平均地盤面算定ポイント
            this.dgvCalcPoint.DataSource = _EntDtAnnotation.TableAveGlLvlCalcPos;

            this.txtAreaDecimal.Text = _EntDtAnnotation.AreaDecimal.ToString();
            RdbAreaRounding = _EntDtAnnotation.AreaRoundingOpt;
            RdbLengthUnit = _EntDtAnnotation.LengthUnit;
        }

        /// ================================================================================
        /// <summary>フォームのデータ取得</summary>
        ///
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void GetData()
        {
            _EntDtAnnotation.BMHeight = double.Parse(this.txtBMHeight.Text);
            _EntDtAnnotation.Scale = int.Parse(this.txtScale.Text);
            _EntDtAnnotation.RaiteHorizontal = int.Parse(this.txtHorizontal.Text);
            _EntDtAnnotation.RaiteVertical = int.Parse(this.txtVertical.Text);
            _EntDtAnnotation.AreaDecimal = int.Parse(this.txtAreaDecimal.Text);
            _EntDtAnnotation.AreaRoundingOpt = RdbAreaRounding;
            _EntDtAnnotation.LengthUnit = RdbLengthUnit;

            _EntDtCmd.Data[0] = _EntDtAnnotation.BMHeight.ToString();
            _EntDtCmd.Data[1] = _EntDtAnnotation.Scale.ToString();
            _EntDtCmd.Data[2] = _EntDtAnnotation.RaiteHorizontal.ToString();
            _EntDtCmd.Data[3] = _EntDtAnnotation.RaiteVertical.ToString();
            _EntDtCmd.Data[4] = _EntDtAnnotation.AreaDecimal.ToString();
            _EntDtCmd.Data[5] = _EntDtAnnotation.AreaRoundingOpt.ToString();
            _EntDtCmd.Data[6] = _EntDtAnnotation.LengthUnit.ToString();
        }

        /// ================================================================================
        /// <summary>フォームのエラープロバイダ取得</summary>
        ///
        /// <param name="mode ">モード<p>Mode</p>
        ///
        /// <returns>Result<p>結果</p>
        ///           <p>True  = There is no error</p><p>        エラーなし</p>
        ///           <p>False = There is an error</p><p>        エラーあり</p></returns>
        ///
        /// <history>2009/12/10 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private bool GetErrPvd(int mode)
        {
            bool ret = false;

            if (this.errPvd.GetError(this.txtBMHeight) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtScale) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtHorizontal) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtVertical) != "")
            {
                return ret;
            }

            if (this.errPvd.GetError(this.txtAreaDecimal) != "")
            {
                return ret;
            }

            if (mode < 1)
            {
                if (GetErrPvdLevel() == false)
                {
                    return ret;
                }
            }
            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>フォームのエラープロバイダチェック</summary>
        ///
        /// <param name="mode ">モード</param>
        ///
        /// <history>2011/08/08 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void ChkErrPvd(int mode)
        {
            CheckBMHeight();
            this.errPvd.SetError(this.txtScale, _EntDtAnnotation.SetErrPvdValue(this.txtScale.Text.Trim(), 0, 1));
            this.errPvd.SetError(this.txtHorizontal, _EntDtAnnotation.SetErrPvdValue(this.txtHorizontal.Text.Trim(), 0, 1));
            this.errPvd.SetError(this.txtVertical, _EntDtAnnotation.SetErrPvdValue(this.txtVertical.Text.Trim(), 0, 1));
            this.errPvd.SetError(this.txtAreaDecimal, _EntDtAnnotation.SetErrPvdDecimalText(this.txtAreaDecimal.Text.Trim()));

            if (mode < 1)
            {
                ChkErrPvdLevel();
            }
        }

        /// ================================================================================
        /// <summary>BM高さ更新チェック</summary>
        ///
        /// <history>2011/08/08 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void ChkUpdateBMHeight()
        {
            if (this.txtBMHeight.Text != this.txtBMHeightOld.Text)
            {
                System.Windows.Forms.DialogResult dlgRet = System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_WAR_UPDATELEVEL"), "",
                                                                                                System.Windows.Forms.MessageBoxButtons.OKCancel);
                if (dlgRet == System.Windows.Forms.DialogResult.OK)
                {
                    UpdateBMHeight();
                }
                else
                {
                    this.txtBMHeight.Text = this.txtBMHeightOld.Text;
                }
            }
        }

        /// ================================================================================
        /// <summary>BM高さ更新</summary>
        ///
        /// <history>2011/08/08 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private bool UpdateBMHeight()
        {
            string bmLevelNew = this.txtBMHeight.Text;

            double iBmLevelNew = double.Parse(bmLevelNew);

            string bmLevelOld = this.txtBMHeightOld.Text;
            _EntDtAnnotation.UpdateLevelViewTable(bmLevelNew, bmLevelOld);
            this.txtBMHeightOld.Text = this.txtBMHeight.Text;

            return true;
        }

        /// ================================================================================
        /// <summary>レベルのエラープロバイダチェック</summary>
        ///
        /// <history>2011/08/08 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void ChkErrPvdLevel()
        {
            int colNo = 3;
            int countCol = this.dgvCalcPoint.ColumnCount;
            int countRow = this.dgvCalcPoint.RowCount;

            if ((countCol > 0) && (countRow > 0))
            {
                if ((colNo >= 0) && (colNo < countCol))
                {
                    for (int i = 0; i < countRow; ++i)
                    {
                        System.Windows.Forms.DataGridViewCell cell = this.dgvCalcPoint[colNo, i];
                        string value = cell.Value.ToString();
                        cell.ErrorText = _EntDtAnnotation.SetErrPvdValue(value, 1, 0);
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>レベルのエラープロバイダ取得</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = エラーなし</p>
        ///             <p>False = エラーあり</p></returns>
        ///
        /// <history>2011/08/08 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private bool GetErrPvdLevel()
        {
            bool ret = true;
            int colNo = 3;
            int countCol = this.dgvCalcPoint.ColumnCount;
            int countRow = this.dgvCalcPoint.RowCount;

            if ((countCol > 0) && (countRow > 0))
            {
                for (int i = 0; i < countRow; ++i)
                {
                    System.Windows.Forms.DataGridViewCell cell = this.dgvCalcPoint[colNo, i];
                    if (cell.ErrorText != "")
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>面積の端数タイプ</summary>
        /// <history>2011/08/08 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbAreaRounding
        {
            get
            {
                int ret = 0;

                if (this.rdbAreaCut.Checked == true)
                {
                    ret = 0;
                }
                else if (this.rdbAreaClose.Checked == true)
                {
                    ret = 1;
                }
                else if (this.rdbAreaRounding.Checked == true)
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
                        this.rdbAreaCut.Checked = true;
                        break;

                    case 1:
                        this.rdbAreaClose.Checked = true;
                        break;

                    case 2:
                        this.rdbAreaRounding.Checked = true;
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary>長さの単位</summary>
        /// <history>2011/08/08 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbLengthUnit
        {
            get
            {
                int ret = 0;

                if (this.rdbLengthMM.Checked == true)
                {
                    ret = 0;
                }
                else if (this.rdbLengthM.Checked == true)
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
                        this.rdbLengthMM.Checked = true;
                        break;

                    case 1:
                        this.rdbLengthM.Checked = true;
                        break;
                }
            }
        }

        #endregion Properties

        // イベント

        #region Events

        /// ================================================================================
        /// <summary>Handles the Load event of the FormCalcDraw control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void FormCalcDraw_Load(object sender, EventArgs e)
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
        /// <history>2009/12/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnCreate_Click(object sender, EventArgs e)
        {
            ChkErrPvd(0);
            if (GetErrPvd(0) == true)
            {
                ChkUpdateBMHeight();
                GetData();
                this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                this.Close();
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/12/10 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnClose_Click(object sender, EventArgs e)
        {
            ChkErrPvd(0);
            if (GetErrPvd(0) == true)
            {
                ChkUpdateBMHeight();
                GetData();
                this.DialogResult = System.Windows.Forms.DialogResult.No;
                this.Close();
            }
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtBMHeight control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/12/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void txtBMHeight_Validated(object sender, EventArgs e)
        {
            CheckBMHeight();
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtScale control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/12/10 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void txtScale_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtScale, _EntDtAnnotation.SetErrPvdValue(this.txtScale.Text.Trim(), 0, 1));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtHorizontal control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/12/10 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void txtHorizontal_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtHorizontal, _EntDtAnnotation.SetErrPvdValue(this.txtHorizontal.Text.Trim(), 0, 1));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtVertical control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/12/10 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void txtVertical_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtVertical, _EntDtAnnotation.SetErrPvdValue(this.txtVertical.Text.Trim(), 0, 1));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtAreaDecimal control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void txtAreaDecimal_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtAreaDecimal, _EntDtAnnotation.SetErrPvdDecimalText(this.txtAreaDecimal.Text.Trim()));
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnUp control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/12/23 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnUp_Click(object sender, EventArgs e)
        {
            int indexY = this.dgvCalcPoint.CurrentCellAddress.Y;
            int indexX = this.dgvCalcPoint.CurrentCellAddress.X;
            if (indexY > -1)
            {
                System.Object selectVal = _EntDtAnnotation.UpDnViewTable(indexY, true);
                if ((indexY - 1) > -1)
                {
                    System.Windows.Forms.DataGridViewCell cell = this.dgvCalcPoint.Rows[indexY - 1].Cells[indexX];
                    this.dgvCalcPoint.CurrentCell = cell;
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnDn control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/12/23 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnDn_Click(object sender, EventArgs e)
        {
            int indexY = this.dgvCalcPoint.CurrentCellAddress.Y;
            int indexX = this.dgvCalcPoint.CurrentCellAddress.X;
            if (indexY > -1)
            {
                System.Object selectVal = _EntDtAnnotation.UpDnViewTable(indexY, false);
                if ((indexY + 1) < this.dgvCalcPoint.Rows.Count)
                {
                    System.Windows.Forms.DataGridViewCell cell = this.dgvCalcPoint.Rows[indexY + 1].Cells[indexX];
                    this.dgvCalcPoint.CurrentCell = cell;
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnDel control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/12/23 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnDel_Click(object sender, EventArgs e)
        {
            int indexY = this.dgvCalcPoint.CurrentCellAddress.Y;
            int indexX = this.dgvCalcPoint.CurrentCellAddress.X;
            if (indexY > -1)
            {
                System.Object delVal = _EntDtAnnotation.DelViewTable(indexY);
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnUpdate control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/12/23 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnUpdateNumber_Click(object sender, EventArgs e)
        {
            _EntDtAnnotation.UpdateNumberViewTable();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnUpdateLevel control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/12/28 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnUpdateLevel_Click(object sender, EventArgs e)
        {
            ChkErrPvd(1);
            if (GetErrPvd(1) == true)
            {
                if (UpdateBMHeight() == false)
                    return;

                ChkErrPvdLevel();
            }
        }

        /// ================================================================================
        /// <summary>Handles the CellValidated event of the dgvCalcPoint control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2010/02/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void dgvCalcPoint_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            int colIndex = e.ColumnIndex;
            if (colIndex == 2)
            {
                ChkErrPvdLevel();
            }
        }

        /// ================================================================================
        /// <summary>Check value for BMHeight control</summary>
        /// <history>2021/12/28 Created AT</history>
        /// ================================================================================
        private void CheckBMHeight()
        {
            this.errPvd.SetError(this.txtBMHeight, _EntDtAnnotation.SetErrPvdValue(this.txtBMHeight.Text.Trim(), 1, 0));

            //Check exist error
            string existError = this.errPvd.GetError(this.txtBMHeight);
            if (existError == null || existError == string.Empty)
            {
                string bmLevelNew = txtBMHeight.Text.Trim();

                //Check value
                bool error = false;
                decimal value = 0;
                if (decimal.TryParse(bmLevelNew, out value) == false)
                {
                    error = true;
                }
                else
                {
                    if (value < decimal.MinValue || value > decimal.MaxValue)
                        error = true;
                }
                if (error)
                {
                    this.errPvd.SetError(this.txtBMHeight, _CmpAttribute.ResourceText("IDS_ERROR_INVALID_LEVEL_VALUE"));
                }
            }
        }

        #endregion Events
    }
}