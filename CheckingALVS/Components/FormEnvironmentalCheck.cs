
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;
using System.Reflection;

namespace ADSK.JExtRAC.CheckingALVS.Components
{
    /// ================================================================================
    /// <summary>画面 AVSLチェック</summary>
    /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormEnvironmentalCheck : Form
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル - 部屋</summary>
        private RvtExtApp.Entities.DtRoom _EntDtRoom;

        /// <summary>データテーブル - 建具</summary>
        private RvtExtApp.Entities.DtWinDoor _EntDtWinDoor;

        /// <summary>データテーブル - コマンド</summary>
        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        /// <summary>排煙壁長さの列番号</summary>
        private int _ColNoSmakeWallLength;

        /// <summary>数値チェックの列番号</summary>
        private Collections.Generic.IList<int> _ColNoCheckedNumeric;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="entDtRoom"   >データテーブル - 部屋</param>
        /// <param name="entDtWinDoor">データテーブル - 建具</param>
        /// <param name="entDtCmd"    >データテーブル - コマンド</param>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public FormEnvironmentalCheck(RvtExtApp.Components.Attribute cmpAttribute,
                                      RvtExtApp.Entities.DtRoom entDtRoom,
                                      RvtExtApp.Entities.DtWinDoor entDtWinDoor,
                                      RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();
            RevitFormTheme.Apply(this);

            _CmpAttribute = cmpAttribute;
            _EntDtRoom = entDtRoom;
            _EntDtWinDoor = entDtWinDoor;
            _EntDtCmd = entDtCmd;
            _ColNoSmakeWallLength = 0;
            _ColNoCheckedNumeric = new Collections.Generic.List<int>();

            SetText();
            SetData();
            CheckNumeric();
            CheckSmokeWallLength();
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>Set text of form<p>フォームの文字設定</p></summary>
        ///
        /// <history><p>2011/08/05 Created GSA,Inc. Shinichi Ishii</p>
        ///           <p>2015/09/07 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetText()
        {
            string sValue = CheckingCommandTitles.GetCommandTitle(_CmpAttribute, _EntDtRoom.CommandKind);

            // フォームタイトル
            this.Text = sValue;

            // コントロール文字
            this.lblSelectRoomGroup.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTEDROOMGROUP");
            this.lblRoomGroup.Text = _CmpAttribute.ResourceText("IDS_TXT_ROOMGROUP");
            this.btnAddGroup.Text = _CmpAttribute.ResourceText("IDS_TXT_ADD");
            this.btnDelGroup.Text = _CmpAttribute.ResourceText("IDS_TXT_DEL");
            this.btnEditRoomGroup.Text = _CmpAttribute.ResourceText("IDS_TXT_EDIT");

            this.lblSelectRoom.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTEDROOM");
            this.lblTotalRoomGroup.Text = _CmpAttribute.ResourceText("IDS_TXT_ROOMGROUPTOTAL");
            this.lblGroupNecessaryArea.Text = _CmpAttribute.ResourceText("IDS_TXT_NESAREA");
            this.lblGroupUsableArea.Text = _CmpAttribute.ResourceText("IDS_TXT_USABLEAREA");
            this.lblGroupJudgment.Text = _CmpAttribute.ResourceText("IDS_TXT_JUDGMENT");

            this.lblSelectParts.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTEDPARTS");

            this.gpbLighting.Text = _CmpAttribute.ResourceText("IDS_TXT_LIGHTING") +
                                    _CmpAttribute.ResourceText("IDS_TXT_CHANGEBULK");

            this.gpbVeranda.Text = _CmpAttribute.ResourceText("IDS_TXT_VERANDA");
            this.btnVeranda.Text = _CmpAttribute.ResourceText("IDS_TXT_UPDATE");

            this.gpbRoadSide.Text = _CmpAttribute.ResourceText("IDS_TXT_ROADSIDE");
            this.btnRoadSide.Text = _CmpAttribute.ResourceText("IDS_TXT_UPDATE");

            this.gpbHorizontalMeas.Text = _CmpAttribute.ResourceText("IDS_TXT_DISTHORIZONTAL_MEAS");
            this.btnHorizontalMeas.Text = _CmpAttribute.ResourceText("IDS_TXT_UPDATE");

            this.gpbHorizontalCorr.Text = _CmpAttribute.ResourceText("IDS_TXT_DISTHORIZONTAL_CORR");
            this.btnHorizontalCorr.Text = _CmpAttribute.ResourceText("IDS_TXT_UPDATE");

            this.gpbVerticalMeas.Text = _CmpAttribute.ResourceText("IDS_TXT_DISTVERTICAL_MEAS");
            this.btnVerticalMeas.Text = _CmpAttribute.ResourceText("IDS_TXT_UPDATE");

            this.gpbVerticalCorr.Text = _CmpAttribute.ResourceText("IDS_TXT_DISTVERTICAL_CORR");
            this.btnVerticalCorr.Text = _CmpAttribute.ResourceText("IDS_TXT_UPDATE");

            this.gpbSmoke.Text = _CmpAttribute.ResourceText("IDS_TXT_SMOKE") +
                                 _CmpAttribute.ResourceText("IDS_TXT_CHANGEBULK");

            this.gpbHeadHeight.Text = _CmpAttribute.ResourceText("IDS_TXT_UPPERMOSTSIDEHEIGHT");
            this.btnHeadHeight.Text = _CmpAttribute.ResourceText("IDS_TXT_UPDATE");

            this.gpbCeilingHeight.Text = _CmpAttribute.ResourceText("IDS_TXT_CEILINGHEIGHT");
            this.btnCeilingHeight.Text = _CmpAttribute.ResourceText("IDS_TXT_UPDATE");

            this.gpbSmokeWallLength.Text = _CmpAttribute.ResourceText("IDS_TXT_SMOKEWALLLENGTH");
            this.btnSmokeWallLength.Text = _CmpAttribute.ResourceText("IDS_TXT_UPDATE");

            this.gpbUsableHeightSmoke.Text = _CmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEHEIGHT");
            this.btnUsableHeightSmoke.Text = _CmpAttribute.ResourceText("IDS_TXT_UPDATE");

            this.btnSelectParts.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTPARTS");
            this.lblUseDistrict.Text = _CmpAttribute.ResourceText("IDS_TXT_USEDISTRICT");
            this.chkCreateHeader.Text = _CmpAttribute.ResourceText("IDS_TXT_CREATEHEADER");
            this.btnOutExcel.Text = _CmpAttribute.ResourceText("IDS_TXT_OUTEXCEL");
            this.btnClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOSE");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");

            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;

            // データグリッドビュー-部屋
            SetTextRoom(this.dgvSelectRoom);

            // データグリッドビュー-建具
            SetTextParts(this.dgvSelectParts);
        }

        /// ================================================================================
        /// <summary>フォームの部屋文字設定</summary>
        ///
        /// <param name="dgv">データグリッドビュー</param>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetTextRoom(System.Windows.Forms.DataGridView dgv)
        {
            // 列の文字位置
            System.Windows.Forms.DataGridViewContentAlignment alignMidRight = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            System.Windows.Forms.DataGridViewContentAlignment alignMidCenter = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            System.Windows.Forms.DataGridViewContentAlignment alignMidLeft = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;

            // DataGridView 列初期化
            int colsCount = 0;
            System.Windows.Forms.DataGridViewColumnCollection cols;
            System.Windows.Forms.DataGridViewColumn col;
            bool visible = false;
            string header = "";

            // 列数
            colsCount = _EntDtRoom.Data.Columns.Count;
            if (colsCount == 0)
            {
                return;
            }
            cols = dgv.Columns;
            string colName = "";

            // ID
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtRoom.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtRoom.ColNameID)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);
                    UtilForm.SetDataGridViewColumnProperty(col, 0, alignMidLeft, true, false, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_ROOMELEMENTID"));
                    break;
                }
            }

            // レベル名
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtRoom.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtRoom.ColNameLevelName)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);
                    UtilForm.SetDataGridViewColumnProperty(col, 100, alignMidLeft, true, true, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_LEVEL"));
                    break;
                }
            }

            // グループ名
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtRoom.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtRoom.ColNameGroupName)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);
                    UtilForm.SetDataGridViewColumnProperty(col, 0, alignMidLeft, false, false, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_GROUPNAME"));
                    break;
                }
            }

            // 計算グループ名
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtRoom.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtRoom.ColNameCalcGroupName)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);
                    UtilForm.SetDataGridViewColumnProperty(col, 0, alignMidLeft, false, false, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_CALCGROUP_N"));
                    break;
                }
            }

            // 部屋名
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtRoom.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtRoom.ColNameRoomName)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);
                    UtilForm.SetDataGridViewColumnProperty(col, 100, alignMidLeft, true, true, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_ROOMNAME"));
                    break;
                }
            }

            // 部屋番号
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtRoom.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtRoom.ColNameRoomNo)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);
                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, true, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_ROOMNO"));
                    break;
                }
            }

            // 面積
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtRoom.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtRoom.ColNameArea)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);
                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, true, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_LEGALAREA"));
                    break;
                }
            }

            // 部屋種類
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtRoom.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtRoom.ColNameRoomKind)
                {
                    col = UtilForm.SetDataGridViewComboBoxColumn(dgv, _EntDtRoom.EntDtItems.RoomKind, "Name", "Name");
                    visible = true;

                    if (_EntDtRoom.CommandKind != 0)
                    {
                        visible = false;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 220, alignMidLeft, false, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_ROOMKIND"));
                    break;
                }
            }

            // 必要係数
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtRoom.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtRoom.ColNameNecessaryCoefficient)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    header = "";
                    switch (_EntDtRoom.CommandKind)
                    {
                        case 0:
                            header = _CmpAttribute.ResourceText("IDS_TXT_LIGHTINGNESCOEFF_N");
                            break;

                        case 1:
                            header = _CmpAttribute.ResourceText("IDS_TXT_SMOKENESCOEFF_N");
                            break;

                        case 2:
                            header = _CmpAttribute.ResourceText("IDS_TXT_VENTILATIONNESCOEFF_N");
                            break;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, true, colName, header);
                    break;
                }
            }

            // 平均天井高
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtRoom.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtRoom.ColNameAverageCeilingHeight)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;

                    if (_EntDtRoom.CommandKind != 1)
                    {
                        visible = false;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_AVERAGECEILINGHEIGHT_N"));
                    break;
                }
            }

            // 必要面積
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtRoom.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtRoom.ColNameNecessaryArea)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    header = "";
                    switch (_EntDtRoom.CommandKind)
                    {
                        case 0:
                            header = _CmpAttribute.ResourceText("IDS_TXT_LIGHTINGNESAREA_N");
                            break;

                        case 1:
                            header = _CmpAttribute.ResourceText("IDS_TXT_SMOKENESAREA_N");
                            break;

                        case 2:
                            header = _CmpAttribute.ResourceText("IDS_TXT_VENTILATIONNESAREA_N");
                            break;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, true, colName, header);
                    break;
                }
            }

            // 合計有効面積
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtRoom.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtRoom.ColNameTotalUsableArea)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    header = _CmpAttribute.ResourceText("IDS_TXT_TOTAL") + "\n";
                    switch (_EntDtRoom.CommandKind)
                    {
                        case 0:
                            header += _CmpAttribute.ResourceText("IDS_TXT_LIGHTINGUSABLEAREA_N");
                            break;

                        case 1:
                            header += _CmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEAREA_N");
                            break;

                        case 2:
                            header += _CmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEAREA_N");
                            break;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, true, colName, header);
                    break;
                }
            }

            // 判定
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtRoom.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtRoom.ColNameJudgment)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidLeft, true, true, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_JUDGMENT"));

                    if (_EntDtRoom.CommandKind == 0)
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    break;
                }
            }

            // ヘッダー
            dgv.ColumnHeadersHeight = 50;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = alignMidCenter;
            dgv.AutoGenerateColumns = false;
        }

        /// ================================================================================
        /// <summary>フォームの建具文字設定</summary>
        ///
        /// <param name="dgv">データグリッドビュー</param>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetTextParts(System.Windows.Forms.DataGridView dgv)
        {
            // 列の文字位置
            System.Windows.Forms.DataGridViewContentAlignment alignMidRight = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            System.Windows.Forms.DataGridViewContentAlignment alignMidCenter = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            System.Windows.Forms.DataGridViewContentAlignment alignMidLeft = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;

            // DataGridView 列初期化
            int colsCount = 0;
            System.Windows.Forms.DataGridViewColumnCollection cols;
            System.Windows.Forms.DataGridViewColumn col;
            bool visible = false;
            string header = "";

            // 列数
            colsCount = _EntDtWinDoor.Data.Columns.Count;
            if (colsCount == 0)
            {
                return;
            }
            cols = dgv.Columns;
            string colName = "";

            // ID
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameID)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);
                    UtilForm.SetDataGridViewColumnProperty(col, 0, alignMidLeft, true, false, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_PARTSELEMENTID"));
                    break;
                }
            }

            // カテゴリ
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameCategory)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);
                    UtilForm.SetDataGridViewColumnProperty(col, 0, alignMidLeft, true, false, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_PARTSELEMENTTYPE"));
                    break;
                }
            }

            // 所属部屋
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameAffiliationRoom)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);
                    UtilForm.SetDataGridViewColumnProperty(col, 0, alignMidLeft, true, false, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_PARTSAFFILIATIONROOM"));
                    break;
                }
            }

            // 建具幅
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameWidth)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);
                    UtilForm.SetDataGridViewColumnProperty(col, 0, alignMidLeft, true, false, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_WIDTH"));
                    break;
                }
            }

            // 建具高さ
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameHeight)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);
                    UtilForm.SetDataGridViewColumnProperty(col, 0, alignMidLeft, true, false, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_HEIGHT"));
                    break;
                }
            }

            // 建具符号
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameSign)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);
                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidLeft, true, true, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_PARTSSIGN_N"));
                    break;
                }
            }

            // 建具-縁側
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameVeranda)
                {
                    col = UtilForm.SetDataGridViewCheckBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 50, alignMidCenter, false, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_VERANDA"));
                    break;
                }
            }

            // 建具-道路
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameRoadSide)
                {
                    col = UtilForm.SetDataGridViewCheckBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 50, alignMidCenter, false, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_ROADSIDE"));
                    break;
                }
            }

            // 水平測定距離
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameDistHorizontalMeas)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }
                    else
                    {
                        _ColNoCheckedNumeric.Add(col.Index);
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_DISTHORIZONTAL_MEAS"));
                    break;
                }
            }

            // 水平補正距離
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameDistHorizontalCorr)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }
                    else
                    {
                        _ColNoCheckedNumeric.Add(col.Index);
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_DISTHORIZONTAL_CORR"));
                    break;
                }
            }

            // 水平距離
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameHorizontalDist)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }
                    else
                    {
                        _ColNoCheckedNumeric.Add(col.Index);
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_DISTHORIZONTAL_N"));
                    break;
                }
            }

            // 垂直測定距離
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameDistVerticalMeas)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }
                    else
                    {
                        _ColNoCheckedNumeric.Add(col.Index);
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_DISTVERTICAL_MEAS"));
                    break;
                }
            }

            // 垂直補正距離
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameDistVerticalCorr)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }
                    else
                    {
                        _ColNoCheckedNumeric.Add(col.Index);
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_DISTVERTICAL_CORR"));
                    break;
                }
            }

            // 垂直距離
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameVerticalDist)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }
                    else
                    {
                        _ColNoCheckedNumeric.Add(col.Index);
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_DISTVERTICAL_N"));
                    break;
                }
            }

            // d/h
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameDsH)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_DSH"));
                    break;
                }
            }

            // α
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameA)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_ALPHA"));
                    break;
                }
            }

            // β
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameB)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_BETA"));
                    break;
                }
            }

            // D
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameD)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_D"));
                    break;
                }
            }

            // A(仮)
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameATemp)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_A_TEMP_N"));
                    break;
                }
            }

            // A(補正値)
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameACorr)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_A_CORR_N"));
                    break;
                }
            }

            // 排煙窓幅
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameSmokeWinWidth)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    header = "";
                    visible = true;
                    switch (_EntDtWinDoor.CommandKind)
                    {
                        case 0:
                            header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEWINWIDTH_N");
                            visible = false;
                            break;

                        case 1:
                            header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEWINWIDTH_N");
                            visible = true;
                            break;

                        case 2:
                            header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEWINWIDTH_N");
                            visible = false;
                            break;
                    }
                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName, header);
                    break;
                }
            }

            // 排煙窓高さ
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameSmokeWinHeight)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    header = "";
                    visible = true;
                    switch (_EntDtWinDoor.CommandKind)
                    {
                        case 0:
                            header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEWINHEIGHT_N");
                            visible = false;
                            break;

                        case 1:
                            header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEWINHEIGHT_N");
                            visible = true;
                            break;

                        case 2:
                            header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEWINHEIGHT_N");
                            visible = false;
                            break;
                    }
                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName, header);
                    break;
                }
            }

            // 開口係数
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameOpenCoefficient)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind == 0)
                    {
                        visible = false;
                    }
                    else
                    {
                        _ColNoCheckedNumeric.Add(col.Index);
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_OPENCOEFF"));
                    break;
                }
            }

            // 天端高さ
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameHeadHeight)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 1)
                    {
                        visible = false;
                    }
                    else
                    {
                        _ColNoCheckedNumeric.Add(col.Index);
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_UPPERMOSTSIDEHEIGHT"));
                    break;
                }
            }

            // 天井高さ
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameCeilingHeight)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 1)
                    {
                        visible = false;
                    }
                    else
                    {
                        _ColNoCheckedNumeric.Add(col.Index);
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_CEILINGHEIGHT"));
                    break;
                }
            }

            // 防煙壁長さ
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameSmokeWallLength)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 1)
                    {
                        visible = false;
                    }
                    else
                    {
                        _ColNoCheckedNumeric.Add(col.Index);
                    }
                    _ColNoSmakeWallLength = col.Index;

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_SMOKEWALLLENGTH_N"));
                    break;
                }
            }

            // 有効幅
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameUsableWidth)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    header = "";
                    visible = true;
                    switch (_EntDtWinDoor.CommandKind)
                    {
                        case 0:
                            header = _CmpAttribute.ResourceText("IDS_TXT_OPENINGUSABLEWIDTH_N");
                            visible = true;
                            break;

                        case 1:
                            header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEWIDTH_N");
                            visible = false;
                            break;

                        case 2:
                            header = _CmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEWIDTH_N");
                            visible = true;
                            break;
                    }

                    _ColNoCheckedNumeric.Add(col.Index);

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName, header);
                    break;
                }
            }

            // 有効高さ
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameUsableHeight)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    header = "";
                    visible = true;

                    switch (_EntDtWinDoor.CommandKind)
                    {
                        case 0:
                            header = _CmpAttribute.ResourceText("IDS_TXT_OPENINGUSABLEHEIGHT_N");
                            visible = true;
                            break;

                        case 1:
                            header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEHEIGHT_N");
                            visible = false;
                            break;

                        case 2:
                            header = _CmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEHEIGHT_N");
                            visible = true;
                            break;
                    }

                    _ColNoCheckedNumeric.Add(col.Index);

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName, header);
                    break;
                }
            }

            // 排煙有効高さ
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameUsableHeightSmoke)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    header = "";
                    visible = true;

                    switch (_EntDtWinDoor.CommandKind)
                    {
                        case 0:
                            header = _CmpAttribute.ResourceText("IDS_TXT_OPENINGUSABLEHEIGHT_N");
                            visible = false;
                            break;

                        case 1:
                            header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEHEIGHT_N");
                            visible = true;
                            break;

                        case 2:
                            header = _CmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEHEIGHT_N");
                            visible = false;
                            break;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, false, visible, colName, header);
                    break;
                }
            }

            // 有効開口面積
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameUsableOpenArea)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    visible = true;
                    if (_EntDtWinDoor.CommandKind != 0)
                    {
                        visible = false;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, visible, colName,
                                                                 _CmpAttribute.ResourceText("IDS_TXT_OPENINGUSABLEAREA_N"));
                    break;
                }
            }

            // 有効面積
            for (int i = 0; i < colsCount; ++i)
            {
                colName = _EntDtWinDoor.Data.Columns[i].ColumnName;
                col = null;
                if (colName == _EntDtWinDoor.ColNameUsableArea)
                {
                    col = UtilForm.SetDataGridViewTextBoxColumn(dgv);

                    header = "";
                    switch (_EntDtWinDoor.CommandKind)
                    {
                        case 0:
                            header = _CmpAttribute.ResourceText("IDS_TXT_LIGHTINGUSABLEAREA_N");
                            break;

                        case 1:
                            header = _CmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEAREA_N");
                            break;

                        case 2:
                            header = _CmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEAREA_N");
                            break;
                    }

                    UtilForm.SetDataGridViewColumnProperty(col, 60, alignMidRight, true, true, colName, header);

                    if (_EntDtWinDoor.CommandKind == 0)
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    break;
                }
            }

            // ヘッダー
            dgv.ColumnHeadersHeight = 50;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = alignMidCenter;
            dgv.AutoGenerateColumns = false;
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetData()
        {
            // 用途地域
            if (_EntDtRoom.CommandKind == 0)
            {
                this.cboUseDistrict.DataSource = _EntDtRoom.EntDtItems.UseDistrict;
                this.cboUseDistrict.DisplayMember = "Name";
                if ( cboUseDistrict.Items.Count > 0 ) cboUseDistrict.SelectedIndex = _EntDtCmd.CvUseDistrictOpt ;
            }
            else
            {
                this.cboUseDistrict.Visible = false;
                this.lblUseDistrict.Visible = false;
            }

            // 見出し作成チェック
            this.chkCreateHeader.Checked = _EntDtCmd.CvChkCreateHeader;

            // コマンド値
            this.chkVeranda.Checked = _EntDtCmd.CvVeranda;
            this.chkRoadSide.Checked = _EntDtCmd.CvRoadSide;
            this.txtHorizontalMeas.Text = _EntDtCmd.CvHorizontalMeas;
            this.txtHorizontalCorr.Text = _EntDtCmd.CvHorizontalCorr;
            this.txtVerticalMeas.Text = _EntDtCmd.CvVerticalMeas;
            this.txtVerticalCorr.Text = _EntDtCmd.CvVerticalCorr;
            this.txtHeadHeight.Text = _EntDtCmd.CvHeadHeight;
            this.txtCeilingHeight.Text = _EntDtCmd.CvCeilingHeight;
            this.txtSmokeWallLength.Text = _EntDtCmd.CvSmokeWallLength;
            this.txtUsableHeightSmoke.Text = _EntDtCmd.CvUsableHeightSmoke;

            // ツリービュー-部屋グループ
            _EntDtRoom.GetRoomGroup(ref this.trvRoomGroup);

            // データグリッドビュー-部屋
            this.dgvSelectRoom.DataSource = _EntDtRoom.Data;

            // データグリッドビュー-建具
            this.dgvSelectParts.DataSource = _EntDtWinDoor.Data;

            // 選択した部屋の選択状態
            if (this.dgvSelectRoom.RowCount > 0)
            {
                this.dgvSelectRoom.CurrentCell = this.dgvSelectRoom[6, 0];
                SetPartsData();
            }

            // 一括変更
            switch (_EntDtRoom.CommandKind)
            {
                case 0:
                    this.gpbLighting.Enabled = true;
                    this.gpbSmoke.Enabled = false;
                    break;

                case 1:
                    this.gpbLighting.Enabled = false;
                    this.gpbSmoke.Enabled = true;
                    break;

                case 2:
                    this.gpbLighting.Enabled = false;
                    this.gpbSmoke.Enabled = false;
                    break;
            }
        }

        /// ================================================================================
        /// <summary>建具のデータ設定</summary>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetPartsData()
        {
            // 部屋データの選択位置
            this.dgvSelectParts.CurrentCell = null;
            int roomRowIndex = this.dgvSelectRoom.CurrentCellAddress.Y;
            if (roomRowIndex > -1)
            {
                System.Object cellValue = this.dgvSelectRoom[0, roomRowIndex].Value;
                if (cellValue != null)
                {
                    int id = int.Parse(cellValue.ToString());
                    _EntDtWinDoor.SetVisbleWinDoor(_EntDtWinDoor.Data, id);

                    if (this.dgvSelectParts.RowCount > 0)
                    {
                        if (_EntDtRoom.CommandKind == 0)
                        {
                            // 用途地域確認
                            int index = this.cboUseDistrict.SelectedIndex;
                            if (index > -1)
                            {
                                _EntDtWinDoor.SetUseDistrictValue(index, this.dgvSelectParts);
                            }
                        }

                        // 内窓除外チェック
                        this.dgvSelectParts.CurrentCell = null;
                        for (int i = 0; i < this.dgvSelectParts.RowCount; ++i)
                        {
                            if (this.dgvSelectParts[_EntDtWinDoor.ColNameAffiliationRoom, i].Value.ToString() == "-1")
                            {
                                this.dgvSelectParts.Rows[i].Visible = false;
                            }
                        }
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>フォームのデータ取得</summary>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void GetData()
        {
            _EntDtCmd.CvUseDistrictOpt = this.cboUseDistrict.SelectedIndex;
            _EntDtCmd.CvChkCreateHeader = this.chkCreateHeader.Checked;
            _EntDtCmd.CvVeranda = this.chkVeranda.Checked;
            _EntDtCmd.CvRoadSide = this.chkRoadSide.Checked;
            _EntDtCmd.CvHorizontalMeas = this.txtHorizontalMeas.Text;
            _EntDtCmd.CvHorizontalCorr = this.txtHorizontalCorr.Text;
            _EntDtCmd.CvVerticalMeas = this.txtVerticalMeas.Text;
            _EntDtCmd.CvVerticalCorr = this.txtVerticalCorr.Text;
            _EntDtCmd.CvHeadHeight = this.txtHeadHeight.Text;
            _EntDtCmd.CvCeilingHeight = this.txtCeilingHeight.Text;
            _EntDtCmd.CvSmokeWallLength = this.txtSmokeWallLength.Text;
            _EntDtCmd.CvUsableHeightSmoke = this.txtUsableHeightSmoke.Text;
        }

        /// ================================================================================
        /// <summary>防煙壁長さチェック</summary>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void CheckSmokeWallLength()
        {
            if (_EntDtRoom.CommandKind == 1)
            {
                if ((this.dgvSelectParts.ColumnCount > _ColNoSmakeWallLength) &&
                    (this.dgvSelectParts.RowCount > 0))
                {
                    for (int i = 0; i < this.dgvSelectParts.RowCount; ++i)
                    {
                        System.Windows.Forms.DataGridViewCell cell = this.dgvSelectParts[_ColNoSmakeWallLength, i];
                        string value = cell.Value.ToString();
                        cell.ErrorText = _EntDtWinDoor.SetErrPvdSmokeWallLength(value);
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>数値チェック</summary>
        ///
        /// <param name="colNo">列番号</param>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool CheckNumeric(int colNo)
        {
            if (_ColNoCheckedNumeric.Contains(colNo) == true)
            {
                int countCol = this.dgvSelectParts.ColumnCount;
                int countRow = this.dgvSelectParts.RowCount;

                if ((countCol > 0) && (countRow > 0))
                {
                    if ((colNo >= 0) && (colNo < countCol))
                    {
                        for (int i = 0; i < countRow; ++i)
                        {
                            System.Windows.Forms.DataGridViewCell cell = this.dgvSelectParts[colNo, i];
                            string value = cell.Value.ToString();
                            cell.ErrorText = _EntDtWinDoor.SetErrPvdNumeric(value, false);
                            if (string.IsNullOrEmpty(cell.ErrorText) == false)
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        /// ================================================================================
        /// <summary>数値チェック(オーバーロード)</summary>
        ///
        /// <param name="colNo">列番号</param>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void CheckNumeric()
        {
            if (_ColNoCheckedNumeric.Count > 0)
            {
                for (int i = 0; i < _ColNoCheckedNumeric.Count; ++i)
                {
                    CheckNumeric(_ColNoCheckedNumeric[i]);
                }
            }
        }

        /// ================================================================================
        /// <summary>数値チェック(オーバーロード)</summary>
        ///
        /// <param name="value">値</param>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool CheckNumeric(string value)
        {
            bool ret = true;
            if (_EntDtRoom.SetErrPvdNumeric(value, false) != "")
            {
                ret = false;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>エラープロバイダチェック</summary>
        ///
        /// <param name="objDataGridView">データグリッドビューオブジェクト</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = エラーなし</p>
        ///             <p>False = エラーあり</p></returns>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool CheckErrPvd(System.Windows.Forms.DataGridView objDataGridView)
        {
            bool ret = true;
            int countCol = objDataGridView.ColumnCount;
            int countRow = objDataGridView.RowCount;

            if ((countCol > 0) && (countRow > 0))
            {
                for (int i = 0; i < countCol; ++i)
                {
                    for (int j = 0; j < countRow; ++j)
                    {
                        System.Windows.Forms.DataGridViewCell cell = objDataGridView[i, j];
                        if (cell.ErrorText != "")
                        {
                            ret = false;
                            break;
                        }
                    }
                    if (ret == false)
                    {
                        break;
                    }
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>Textコントロールのエラープロバイダチェック</summary>
        ///
        /// <history>2011/09/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void ChkErrPvdTxtCtrl()
        {
            this.errPvd.SetError(this.txtHorizontalMeas, _EntDtWinDoor.SetErrPvdNumeric(this.txtHorizontalMeas.Text.Trim(), false));
            this.errPvd.SetError(this.txtHorizontalCorr, _EntDtWinDoor.SetErrPvdNumeric(this.txtHorizontalCorr.Text.Trim(), false));
            this.errPvd.SetError(this.txtVerticalMeas, _EntDtWinDoor.SetErrPvdNumeric(this.txtVerticalMeas.Text.Trim(), false));
            this.errPvd.SetError(this.txtVerticalCorr, _EntDtWinDoor.SetErrPvdNumeric(this.txtVerticalCorr.Text.Trim(), false));
            this.errPvd.SetError(this.txtHeadHeight, _EntDtWinDoor.SetErrPvdNumeric(this.txtHeadHeight.Text.Trim(), false));
            this.errPvd.SetError(this.txtCeilingHeight, _EntDtWinDoor.SetErrPvdNumeric(this.txtCeilingHeight.Text.Trim(), false));
            this.errPvd.SetError(this.txtSmokeWallLength, _EntDtWinDoor.SetErrPvdNumeric(this.txtSmokeWallLength.Text.Trim(), false));
            this.errPvd.SetError(this.txtUsableHeightSmoke, _EntDtWinDoor.SetErrPvdNumeric(this.txtUsableHeightSmoke.Text.Trim(), false));
        }

        /// ================================================================================
        /// <summary>Textコントロールのエラープロバイダ取得</summary>
        ///
        /// <returns>Result<p>結果</p>
        ///           <p>True  = エラーなし</p>
        ///           <p>False = エラーあり</p></returns>
        ///
        /// <history>2011/09/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool GetErrPvdTxtCtrl()
        {
            bool ret = false;

            if (this.errPvd.GetError(this.txtHorizontalMeas) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtHorizontalCorr) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtVerticalMeas) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtVerticalCorr) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtHeadHeight) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtCeilingHeight) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtSmokeWallLength) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtUsableHeightSmoke) != "")
            {
                return ret;
            }
            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>エラーチェック</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = エラーなし</p>
        ///             <p>False = エラーあり</p></returns>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool CheckError()
        {
            bool ret = true;

            CheckNumeric();
            CheckSmokeWallLength();

            if (CheckErrPvd(this.dgvSelectParts) == false)
            {
                ret = false;
            }

            if (ret == true)
            {
                GetData();
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>有効寸法変更チェック</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 変更する</p>
        ///             <p>False = 変更しない</p></returns>
        ///
        /// <history>2011/08/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool CheckChangedUsableDim()
        {
            bool ret = true;

            System.Data.DataTable changeData = _EntDtWinDoor.GetPartsChangedUsableDim();

            if (changeData.Rows.Count > 0)
            {
                RvtExtApp.Components.FormChangedUsableDim form = new RvtExtApp.Components.FormChangedUsableDim(_CmpAttribute,
                                                                                                               _EntDtWinDoor,
                                                                                                               changeData);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    ret = false;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>部屋グループ集計設定</summary>
        ///
        /// <param name="groupName">グループ名</param>
        ///
        /// <history>2011/09/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetTotalRoomGroup(string groupName)
        {
            string strGroupNecessaryArea = "";
            string strGroupUsableArea = "";
            string strGroupJudgment = "";

            _EntDtRoom.TotalRoomGroup(groupName,
                                      ref strGroupNecessaryArea,
                                      ref strGroupUsableArea,
                                      ref strGroupJudgment);

            this.txtGroupNecessaryArea.Text = strGroupNecessaryArea;
            this.txtGroupUsableArea.Text = strGroupUsableArea;
            this.txtGroupJudgment.Text = strGroupJudgment;
        }

        /// ================================================================================
        /// <summary>選択部屋更新</summary>
        ///
        /// <param name="rowIndex">行インデックス</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void UpdateSelectRoom(int rowIndex)
        {
            string sValue = "";

            if (rowIndex > -1)
            {
                // 部屋ID
                string roomID = this.dgvSelectRoom[0, rowIndex].Value.ToString();

                // 必要係数
                sValue = this.dgvSelectRoom[_EntDtRoom.ColNameRoomKind, rowIndex].Value.ToString();
                this.dgvSelectRoom[_EntDtRoom.ColNameNecessaryCoefficient, rowIndex].Value = _EntDtRoom.GetNesCoeff(sValue);

                // 必要面積
                this.dgvSelectRoom[_EntDtRoom.ColNameNecessaryArea, rowIndex].Value = _EntDtRoom.GetNesArea(roomID);

                this.dgvSelectRoom[_EntDtRoom.ColNameJudgment, rowIndex].Value = _EntDtRoom.GetJudgment(roomID);
                string groupName = this.dgvSelectRoom[_EntDtRoom.ColNameGroupName, rowIndex].Value.ToString();
                SetTotalRoomGroup(groupName);

                this.dgvSelectRoom.Refresh();
            }
        }

        /// ================================================================================
        /// <summary>選択建具更新</summary>
        ///
        /// <param name="rowIndex">行インデックス</param>
        /// <param name="colName" >列名</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void UpdateSelectParts(int rowIndex, string colName)
        {
            if (rowIndex > -1)
            {
                // 建具ID
                string partsID = this.dgvSelectParts[0, rowIndex].Value.ToString();

                // 水平距離
                this.dgvSelectParts[_EntDtWinDoor.ColNameHorizontalDist, rowIndex].Value = _EntDtWinDoor.GetDistHoriOrVert(partsID, 0);

                // 垂直距離
                this.dgvSelectParts[_EntDtWinDoor.ColNameVerticalDist, rowIndex].Value = _EntDtWinDoor.GetDistHoriOrVert(partsID, 1);

                // d/h
                this.dgvSelectParts[_EntDtWinDoor.ColNameDsH, rowIndex].Value = _EntDtWinDoor.GetDsH(partsID);

                // A(仮)
                this.dgvSelectParts[_EntDtWinDoor.ColNameATemp, rowIndex].Value = _EntDtWinDoor.GetAtempValue(partsID);

                // A(補正値)
                this.dgvSelectParts[_EntDtWinDoor.ColNameACorr, rowIndex].Value = _EntDtWinDoor.GetACorrValue(partsID);

                // 排煙有効高さ
                if ((colName == _EntDtWinDoor.ColNameHeadHeight) || (colName == _EntDtWinDoor.ColNameCeilingHeight) ||
                    (colName == _EntDtWinDoor.ColNameSmokeWinHeight) || (colName == _EntDtWinDoor.ColNameSmokeWallLength))
                {
                    //this.dgvSelectParts[_EntDtWinDoor.ColNameUsableHeightSmoke, rowIndex].Value = _EntDtWinDoor.GetUsableHeightSmoke(partsID);
                    _EntDtWinDoor.SetUsableHeightSmoke(partsID);
                }

                // 有効面積
                switch (_EntDtWinDoor.CommandKind)
                {
                    case 0:
                        this.dgvSelectParts[_EntDtWinDoor.ColNameUsableOpenArea, rowIndex].Value = _EntDtWinDoor.GetUsableOpenArea(partsID);
                        this.dgvSelectParts[_EntDtWinDoor.ColNameUsableArea, rowIndex].Value = _EntDtWinDoor.GetUsableArea(partsID);
                        break;

                    case 1:
                        this.dgvSelectParts[_EntDtWinDoor.ColNameUsableArea, rowIndex].Value = _EntDtWinDoor.GetUsableArea(partsID);
                        break;

                    case 2:
                        this.dgvSelectParts[_EntDtWinDoor.ColNameUsableArea, rowIndex].Value = _EntDtWinDoor.GetUsableArea(partsID);
                        break;
                }
                _EntDtWinDoor.SetUsableArea(partsID);

                _EntDtWinDoor.SetTotalUsableArea(_EntDtRoom.Data, _EntDtWinDoor.Data);
                _EntDtWinDoor.SetJudgment(_EntDtRoom.Data, _EntDtWinDoor.Data);
                string roomID = this.dgvSelectParts[_EntDtWinDoor.ColNameAffiliationRoom, rowIndex].Value.ToString();
                string groupName = _EntDtRoom.GetGroupName(roomID);
                SetTotalRoomGroup(groupName);

                this.dgvSelectParts.Refresh();
            }
        }

        #endregion Member Functions

        // イベント

        #region Events

        /// ================================================================================
        /// <summary>Handles the Shown event of the  FormEnvironmentalCheck control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void FormEnvironmentalCheck_Shown(object sender, EventArgs e)
        {
            // 選択状態
            if (_EntDtWinDoor.WinDoorFromDraw.Count > 0)
            {
                if (this.dgvSelectParts.RowCount > 0)
                {
                    for (int i = 0; i < this.dgvSelectParts.RowCount; ++i)
                    {
                        int id = int.Parse(this.dgvSelectParts[_EntDtWinDoor.ColNameID, i].Value.ToString());
                        if (_EntDtWinDoor.WinDoorFromDraw.Contains(id) == true)
                        {
                            this.dgvSelectParts.Rows[i].Selected = true;
                        }
                        else
                        {
                            this.dgvSelectParts.Rows[i].Selected = false;
                        }
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the SelectedIndexChanged event of the cboUseDistrict control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void cboUseDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = this.cboUseDistrict.SelectedIndex;
            if (index > -1)
            {
                int countRoom = this.dgvSelectRoom.RowCount;
                if (countRoom > 0)
                {
                    int indexRoom = -1;
                    foreach (System.Windows.Forms.DataGridViewCell cell in this.dgvSelectRoom.SelectedCells)
                    {
                        indexRoom = cell.RowIndex;
                    }

                    for (int i = 0; i < countRoom; ++i)
                    {
                        this.dgvSelectRoom.CurrentCell = this.dgvSelectRoom[_EntDtRoom.ColNameLevelName, i];
                        SetPartsData();
                    }

                    if (indexRoom > -1)
                    {
                        this.dgvSelectRoom.CurrentCell = this.dgvSelectRoom[_EntDtRoom.ColNameLevelName, indexRoom];
                        SetPartsData();
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOutExcel control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnOutExcel_Click(object sender, EventArgs e)
        {
            if (CheckError() == false)
            {
                return;
            }

            ChkErrPvdTxtCtrl();
            if (GetErrPvdTxtCtrl() == false)
            {
                return;
            }

            if (CheckChangedUsableDim() == false)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            }

            this.Close();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnClose control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnClose_Click(object sender, EventArgs e)
        {
            if (CheckError() == false)
            {
                return;
            }

            ChkErrPvdTxtCtrl();
            if (GetErrPvdTxtCtrl() == false)
            {
                return;
            }

            if (CheckChangedUsableDim() == false)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.No;
            }

            this.Close();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnSelectParts control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnSelectParts_Click(object sender, EventArgs e)
        {
            if (CheckError() == false)
            {
                return;
            }

            ChkErrPvdTxtCtrl();
            if (GetErrPvdTxtCtrl() == false)
            {
                return;
            }

            if (CheckChangedUsableDim() == false)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            }

            this.Close();
        }

        /// ================================================================================
        /// <summary>Handles the SelectionChanged event of the dgvSelectRoom control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void dgvSelectRoom_SelectionChanged(object sender, EventArgs e)
        {
            SetPartsData();
            CheckNumeric();
            CheckSmokeWallLength();
            _EntDtWinDoor.SetTotalUsableArea(_EntDtRoom.Data, _EntDtWinDoor.Data);
            _EntDtRoom.SetJudgment(_EntDtRoom.Data, _EntDtWinDoor.Data);

            if (this.dgvSelectParts.RowCount > 0)
            {
                this.btnSelectParts.Enabled = true;
            }
            else
            {
                this.btnSelectParts.Enabled = false;
            }
        }

        /// ================================================================================
        /// <summary>Handles the CellEndEdit event of the dgvSelectRoom control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void dgvSelectRoom_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateSelectRoom(e.RowIndex);
        }

        /// ================================================================================
        /// <summary>Handles the CellEndEdit event of the dgvSelectParts control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void dgvSelectParts_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // 数値チェック
            if (CheckNumeric(e.ColumnIndex) == false)
                return;

            UpdateSelectParts(e.RowIndex, this.dgvSelectParts.Columns[e.ColumnIndex].Name);
        }

        /// ================================================================================
        /// <summary>Handles the CellValidated event of the dgvSelectParts control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void dgvSelectParts_CellValidated(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            // 数値チェック
            CheckNumeric(e.ColumnIndex);

            // 防煙壁長さチェック
            if (_EntDtWinDoor.CommandKind == 1)
            {
                if (e.ColumnIndex == _ColNoSmakeWallLength)
                {
                    CheckSmokeWallLength();
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtHorizontalMeas control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void txtHorizontalMeas_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtHorizontalMeas, _EntDtWinDoor.SetErrPvdNumeric(this.txtHorizontalMeas.Text.Trim(), false));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtHorizontalCorr control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void txtHorizontalCorr_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtHorizontalCorr, _EntDtWinDoor.SetErrPvdNumeric(this.txtHorizontalCorr.Text.Trim(), false));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtVerticalMeas control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void txtVerticalMeas_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtVerticalMeas, _EntDtWinDoor.SetErrPvdNumeric(this.txtVerticalMeas.Text.Trim(), false));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtVerticalCorr control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void txtVerticalCorr_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtVerticalCorr, _EntDtWinDoor.SetErrPvdNumeric(this.txtVerticalCorr.Text.Trim(), false));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtHeadHeight control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void txtHeadHeight_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtHeadHeight, _EntDtWinDoor.SetErrPvdNumeric(this.txtHeadHeight.Text.Trim(), false));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtCeilingHeight control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void txtCeilingHeight_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtCeilingHeight, _EntDtWinDoor.SetErrPvdNumeric(this.txtCeilingHeight.Text.Trim(), false));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtSmokeWallLength control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void txtSmokeWallLength_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtSmokeWallLength, _EntDtWinDoor.SetErrPvdNumeric(this.txtSmokeWallLength.Text.Trim(), false));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtUsableHeightSmoke control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void txtUsableHeightSmoke_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtUsableHeightSmoke, _EntDtWinDoor.SetErrPvdNumeric(this.txtUsableHeightSmoke.Text.Trim(), false));
        }

        /// ================================================================================
        /// <summary>Handles the AfterSelect event of the trvRoomGroup control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.TreeViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void trvRoomGroup_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // グループ名
            string groupName = "";

            // ルートノード
            if (e.Node.Parent != null)
            {
                // 親ノード名
                groupName = this.trvRoomGroup.SelectedNode.Parent.Text;
            }
            else
            {
                // ノード名
                groupName = e.Node.Text;
            }
            _EntDtRoom.SetVisbleRowsRoomsOfRoomGroup(groupName, ref this.dgvSelectRoom);
            SetTotalRoomGroup(groupName);
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnAddGroup control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnAddGroup_Click(object sender, EventArgs e)
        {
            // ルートノード
            if (this.trvRoomGroup.SelectedNode.Parent == null)
            {
                // グループ名ダイアログ
                RvtExtApp.Components.FormGroupName form = new RvtExtApp.Components.FormGroupName(_CmpAttribute);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //グループ追加
                    if (form.GroupName != null)
                    {
                        if (form.GroupName != "")
                        {
                            _EntDtRoom.AddRoomGroup(ref this.trvRoomGroup, form.GroupName);
                        }
                    }
                }
            }
            this.trvRoomGroup.Focus();
        }

        // ================================================================================
        /// <summary>Handles the Click event of the btnDelGroup control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnDelGroup_Click(object sender, EventArgs e)
        {
            // ルートノード
            if (this.trvRoomGroup.SelectedNode.Parent == null)
            {
                // グループ削除
                if (this.trvRoomGroup.SelectedNode.Text != _CmpAttribute.ResourceText("IDS_TXT_NOTHING"))
                {
                    _EntDtRoom.DeleteRoomGroup(ref this.trvRoomGroup,
                                               this.trvRoomGroup.SelectedNode,
                                               ref this.dgvSelectRoom);
                }
            }
            this.trvRoomGroup.Focus();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnEditRoomGroup control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnEditRoomGroup_Click(object sender, EventArgs e)
        {
            // ルートノード以外
            if (this.trvRoomGroup.SelectedNode.Parent != null)
            {
                // グループダイアログ
                RvtExtApp.Components.FormGroup form = new RvtExtApp.Components.FormGroup(_CmpAttribute, this.trvRoomGroup.Nodes);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //グループ移動
                    _EntDtRoom.MoveRoomGroup(ref this.trvRoomGroup,
                                             this.trvRoomGroup.SelectedNode,
                                             form.Group,
                                             ref this.dgvSelectRoom);
                }
            }
            this.trvRoomGroup.Focus();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnVeranda control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnVeranda_Click(object sender, EventArgs e)
        {
            bool value = this.chkVeranda.Checked;
            System.Windows.Forms.DataGridViewSelectedRowCollection selRows = this.dgvSelectParts.SelectedRows;
            foreach (System.Windows.Forms.DataGridViewRow row in selRows)
            {
                row.Cells[_EntDtWinDoor.ColNameVeranda].Value = value;
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnRoadSide control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnRoadSide_Click(object sender, EventArgs e)
        {
            bool value = this.chkRoadSide.Checked;
            System.Windows.Forms.DataGridViewSelectedRowCollection selRows = this.dgvSelectParts.SelectedRows;
            foreach (System.Windows.Forms.DataGridViewRow row in selRows)
            {
                row.Cells[_EntDtWinDoor.ColNameRoadSide].Value = value;
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnHorizontalMeas control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnHorizontalMeas_Click(object sender, EventArgs e)
        {
            if (this.errPvd.GetError(this.txtHorizontalMeas) == "")
            {
                string value = this.txtHorizontalMeas.Text;
                System.Windows.Forms.DataGridViewSelectedRowCollection selRows = this.dgvSelectParts.SelectedRows;
                foreach (System.Windows.Forms.DataGridViewRow row in selRows)
                {
                    row.Cells[_EntDtWinDoor.ColNameDistHorizontalMeas].Value = value;
                    UpdateSelectParts(row.Index, _EntDtWinDoor.ColNameDistHorizontalMeas);
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnHorizontalCorr control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnHorizontalCorr_Click(object sender, EventArgs e)
        {
            if (this.errPvd.GetError(this.txtHorizontalCorr) == "")
            {
                string value = this.txtHorizontalCorr.Text;
                System.Windows.Forms.DataGridViewSelectedRowCollection selRows = this.dgvSelectParts.SelectedRows;
                foreach (System.Windows.Forms.DataGridViewRow row in selRows)
                {
                    row.Cells[_EntDtWinDoor.ColNameDistHorizontalCorr].Value = value;
                    UpdateSelectParts(row.Index, _EntDtWinDoor.ColNameDistHorizontalCorr);
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnVerticalMeas control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnVerticalMeas_Click(object sender, EventArgs e)
        {
            if (this.errPvd.GetError(this.txtVerticalMeas) == "")
            {
                string value = this.txtVerticalMeas.Text;
                System.Windows.Forms.DataGridViewSelectedRowCollection selRows = this.dgvSelectParts.SelectedRows;
                foreach (System.Windows.Forms.DataGridViewRow row in selRows)
                {
                    row.Cells[_EntDtWinDoor.ColNameDistVerticalMeas].Value = value;
                    UpdateSelectParts(row.Index, _EntDtWinDoor.ColNameDistVerticalMeas);
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnVerticalCorr control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnVerticalCorr_Click(object sender, EventArgs e)
        {
            if (this.errPvd.GetError(this.txtVerticalCorr) == "")
            {
                string value = this.txtVerticalCorr.Text;
                System.Windows.Forms.DataGridViewSelectedRowCollection selRows = this.dgvSelectParts.SelectedRows;
                foreach (System.Windows.Forms.DataGridViewRow row in selRows)
                {
                    row.Cells[_EntDtWinDoor.ColNameDistVerticalCorr].Value = value;
                    UpdateSelectParts(row.Index, _EntDtWinDoor.ColNameDistVerticalCorr);
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnHeadHeight control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnHeadHeight_Click(object sender, EventArgs e)
        {
            if (this.errPvd.GetError(this.txtHeadHeight) == "")
            {
                string value = this.txtHeadHeight.Text;
                System.Windows.Forms.DataGridViewSelectedRowCollection selRows = this.dgvSelectParts.SelectedRows;
                foreach (System.Windows.Forms.DataGridViewRow row in selRows)
                {
                    row.Cells[_EntDtWinDoor.ColNameHeadHeight].Value = value;
                    UpdateSelectParts(row.Index, _EntDtWinDoor.ColNameHeadHeight);
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnCeilingHeight control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnCeilingHeight_Click(object sender, EventArgs e)
        {
            if (this.errPvd.GetError(this.txtCeilingHeight) == "")
            {
                string value = this.txtCeilingHeight.Text;
                System.Windows.Forms.DataGridViewSelectedRowCollection selRows = this.dgvSelectParts.SelectedRows;
                foreach (System.Windows.Forms.DataGridViewRow row in selRows)
                {
                    row.Cells[_EntDtWinDoor.ColNameCeilingHeight].Value = value;
                    UpdateSelectParts(row.Index, _EntDtWinDoor.ColNameCeilingHeight);
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnSmokeWallLength control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnSmokeWallLength_Click(object sender, EventArgs e)
        {
            if (this.errPvd.GetError(this.txtSmokeWallLength) == "")
            {
                string value = this.txtSmokeWallLength.Text;
                System.Windows.Forms.DataGridViewSelectedRowCollection selRows = this.dgvSelectParts.SelectedRows;
                foreach (System.Windows.Forms.DataGridViewRow row in selRows)
                {
                    row.Cells[_EntDtWinDoor.ColNameSmokeWallLength].Value = value;
                    UpdateSelectParts(row.Index, _EntDtWinDoor.ColNameSmokeWallLength);
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnUsableHeightSmoke control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnUsableHeightSmoke_Click(object sender, EventArgs e)
        {
            if (this.errPvd.GetError(this.txtUsableHeightSmoke) == "")
            {
                string value = this.txtUsableHeightSmoke.Text;
                System.Windows.Forms.DataGridViewSelectedRowCollection selRows = this.dgvSelectParts.SelectedRows;
                foreach (System.Windows.Forms.DataGridViewRow row in selRows)
                {
                    row.Cells[_EntDtWinDoor.ColNameUsableHeightSmoke].Value = value;
                    UpdateSelectParts(row.Index, _EntDtWinDoor.ColNameUsableHeightSmoke);
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the CurrentCellDirtyStateChanged event of the dgvSelectParts control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void dgvSelectParts_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dgvSelectParts.CurrentCell.GetType() == typeof(System.Windows.Forms.DataGridViewCheckBoxCell))
            {
                if (this.dgvSelectParts.IsCurrentCellDirty)
                {
                    this.dgvSelectParts.CommitEdit(System.Windows.Forms.DataGridViewDataErrorContexts.Commit);
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the CellValueChanged event of the dgvSelectParts control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void dgvSelectParts_CellValueChanged(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            string colName = this.dgvSelectParts.Columns[e.ColumnIndex].Name;

            if (rowIndex > -1)
            {
                // Veranda || Road side
                // 建具-縁側 || 建具-道路
                if ((colName == _EntDtWinDoor.ColNameVeranda) || (colName == _EntDtWinDoor.ColNameRoadSide))
                {
                    UpdateSelectParts(rowIndex, "");
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the CurrentCellDirtyStateChanged event of the dgvSelectRoom control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.Windows.Forms.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void dgvSelectRoom_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dgvSelectRoom.CurrentCell.GetType() == typeof(System.Windows.Forms.DataGridViewComboBoxCell))
            {
                if (this.dgvSelectRoom.IsCurrentCellDirty)
                {
                    this.dgvSelectRoom.CommitEdit(System.Windows.Forms.DataGridViewDataErrorContexts.Commit);
                }
            }
        }

        /// ================================================================================
        /// <summary>Handles the CellValueChanged event of the dgvSelectRoom control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void dgvSelectRoom_CellValueChanged(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            string colName = this.dgvSelectRoom.Columns[e.ColumnIndex].Name;

            if (rowIndex > -1)
            {
                // 部屋種類
                if (colName == _EntDtRoom.ColNameRoomKind)
                {
                    UpdateSelectRoom(rowIndex);
                }
            }
        }

        #endregion Events
    }
}