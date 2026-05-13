
using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;

namespace ADSK.JExtRAC.CheckingALVS.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - エクセル</summary>
    /// ================================================================================
    public class DtExcel : RvtExtApp.Entities.DtBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>データ</summary>
        private System.Data.DataTable _Data;

        /// <summary>列整列フラグ</summary>
        private Collections.Generic.IList<int> _ColAlignmentAry;

        /// <summary>列幅</summary>
        private Collections.Generic.IList<double> _ColWidthAry;

        /// <summary>メートル変換フラグ</summary>
        private Collections.Generic.IList<bool> _ColConvMAry;

        /// <summary>システムタイプ</summary>
        private Collections.Generic.IList<System.Type> _ColSysType;

        /// <summary>行罫線フラグ</summary>
        private Collections.Generic.IList<Collections.Generic.IList<int>> _RowBordersAryAry;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        /// <param name="cmpElements"   >要素</param>
        /// <param name="cmpGeometry"   >図形</param>
        /// <param name="cmpParameters" >パラメータ</param>
        /// <param name="cmpSettings"   >設定</param>
        ///
        /// <history>2011/07/29 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public DtExcel(RvtExtApp.Components.Attribute cmpAttribute,
                       RvtExtApp.Components.Elements cmpElements,
                       RvtExtApp.Components.Geometry cmpGeometry,
                       RvtExtApp.Components.Parameters cmpParameters,
                       RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>データ書式定義</summary>
        ///
        /// <param name="data">データテーブル</param>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void DefDataFormat(ref System.Data.DataTable data)
        {
            _ColAlignmentAry = new Collections.Generic.List<int>();
            _ColWidthAry = new Collections.Generic.List<double>();
            _ColConvMAry = new Collections.Generic.List<bool>();
            _ColSysType = new Collections.Generic.List<System.Type>();

            // レベル名
            data.Columns.Add(base.ColNameLevelName, typeof(string));
            _ColAlignmentAry.Add(0);
            _ColWidthAry.Add(-1);
            _ColConvMAry.Add(false);
            _ColSysType.Add(typeof(string));

            // グループ名
            data.Columns.Add(base.ColNameGroupName, typeof(string));
            _ColAlignmentAry.Add(0);
            _ColWidthAry.Add(-1);
            _ColConvMAry.Add(false);
            _ColSysType.Add(typeof(string));

            // 部屋名
            data.Columns.Add(base.ColNameRoomName, typeof(string));
            _ColAlignmentAry.Add(0);
            _ColWidthAry.Add(-1);
            _ColConvMAry.Add(false);
            _ColSysType.Add(typeof(string));

            // 部屋番号
            data.Columns.Add(base.ColNameRoomNo, typeof(string));
            _ColAlignmentAry.Add(0);
            _ColWidthAry.Add(-1);
            _ColConvMAry.Add(false);
            _ColSysType.Add(typeof(string));

            // 面積
            data.Columns.Add(base.ColNameArea, typeof(string));
            _ColAlignmentAry.Add(2);
            _ColWidthAry.Add(-1);
            _ColConvMAry.Add(false);
            _ColSysType.Add(typeof(double));

            // 必要係数
            data.Columns.Add(base.ColNameNecessaryCoefficient, typeof(string));
            _ColAlignmentAry.Add(2);
            _ColWidthAry.Add(-1);
            _ColConvMAry.Add(false);
            _ColSysType.Add(typeof(double));

            // 必要面積
            data.Columns.Add(base.ColNameNecessaryArea, typeof(string));
            _ColAlignmentAry.Add(2);
            _ColWidthAry.Add(-1);
            _ColConvMAry.Add(false);
            _ColSysType.Add(typeof(double));

            // 符号
            data.Columns.Add(base.ColNameSign, typeof(string));
            _ColAlignmentAry.Add(0);
            _ColWidthAry.Add(-1);
            _ColConvMAry.Add(false);
            _ColSysType.Add(typeof(string));

            // 水平距離
            if (base.CommandKind == 0)
            {
                data.Columns.Add(base.ColNameHorizontalDist, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(true);
                _ColSysType.Add(typeof(double));
            }

            // 垂直距離
            if (base.CommandKind == 0)
            {
                data.Columns.Add(base.ColNameVerticalDist, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(true);
                _ColSysType.Add(typeof(double));
            }

            // d/h
            if (base.CommandKind == 0)
            {
                data.Columns.Add(base.ColNameDsH, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(false);
                _ColSysType.Add(typeof(double));
            }

            // α
            if (base.CommandKind == 0)
            {
                data.Columns.Add(base.ColNameA, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(false);
                _ColSysType.Add(typeof(double));
            }

            // β
            if (base.CommandKind == 0)
            {
                data.Columns.Add(base.ColNameB, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(false);
                _ColSysType.Add(typeof(double));
            }

            // D
            if (base.CommandKind == 0)
            {
                data.Columns.Add(base.ColNameD, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(false);
                _ColSysType.Add(typeof(double));
            }

            // A(仮)
            if (base.CommandKind == 0)
            {
                data.Columns.Add(base.ColNameATemp, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(false);
                _ColSysType.Add(typeof(double));
            }

            // A(補正値)
            if (base.CommandKind == 0)
            {
                data.Columns.Add(base.ColNameACorr, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(false);
                _ColSysType.Add(typeof(double));
            }

            // 開口係数
            if ((base.CommandKind == 1) || (base.CommandKind == 2))
            {
                data.Columns.Add(base.ColNameOpenCoefficient, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(false);
                _ColSysType.Add(typeof(double));
            }

            if ((base.CommandKind == 0) || (base.CommandKind == 2))
            {
                // 有効幅
                data.Columns.Add(base.ColNameUsableWidth, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(true);
                _ColSysType.Add(typeof(double));
            }
            else
            {
                // 排煙窓幅
                data.Columns.Add(base.ColNameSmokeWinWidth, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(true);
                _ColSysType.Add(typeof(double));
            }

            if ((base.CommandKind == 0) || (base.CommandKind == 2))
            {
                // 有効高さ
                data.Columns.Add(base.ColNameUsableHeight, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(true);
                _ColSysType.Add(typeof(double));
            }
            else
            {
                // 排煙有効高さ
                data.Columns.Add(base.ColNameUsableHeightSmoke, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(true);
                _ColSysType.Add(typeof(double));
            }

            // 有効開口面積
            if (base.CommandKind == 0)
            {
                data.Columns.Add(base.ColNameUsableOpenArea, typeof(string));
                _ColAlignmentAry.Add(2);
                _ColWidthAry.Add(-1);
                _ColConvMAry.Add(false);
                _ColSysType.Add(typeof(double));
            }

            // 有効面積
            data.Columns.Add(base.ColNameUsableArea, typeof(string));
            _ColAlignmentAry.Add(2);
            _ColWidthAry.Add(-1);
            _ColConvMAry.Add(false);
            _ColSysType.Add(typeof(double));

            // 合計有効面積
            data.Columns.Add(ColNameTotalUsableArea, typeof(string));
            _ColAlignmentAry.Add(2);
            _ColWidthAry.Add(-1);
            _ColConvMAry.Add(false);
            _ColSysType.Add(typeof(double));

            // 判定
            data.Columns.Add(ColNameJudgment, typeof(string));
            _ColAlignmentAry.Add(0);
            _ColWidthAry.Add(-1);
            _ColConvMAry.Add(false);
            _ColSysType.Add(typeof(string));
        }

        /// ================================================================================
        /// <summary>テーブルデータ見出し設定</p></summary>
        ///
        /// <param name="data">データテーブル</param>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetHeader(ref System.Data.DataTable data)
        {
            if (data == null)
            {
                return;
            }

            string colName = "";
            string sValue = "";

            data.Rows.Add(data.NewRow());
            data.Rows.Add(data.NewRow());

            for (int i = 0; i < data.Columns.Count; ++i)
            {
                colName = data.Columns[i].ColumnName;

                // レベル名
                if (colName == base.ColNameLevelName)
                {
                    data.Rows[0][colName] = base.CmpAttribute.ResourceText("IDS_TXT_FLOOR");
                    data.Rows[1][colName] = "";
                    continue;
                }

                // グループ名
                if (colName == base.ColNameGroupName)
                {
                    data.Rows[0][colName] = base.CmpAttribute.ResourceText("IDS_TXT_SITSUGROUP");
                    data.Rows[1][colName] = "";
                    continue;
                }

                // 部屋名
                if (colName == base.ColNameRoomName)
                {
                    data.Rows[0][colName] = base.CmpAttribute.ResourceText("IDS_TXT_SITSUMEI");
                    data.Rows[1][colName] = "";
                    continue;
                }

                // 部屋番号
                if (colName == base.ColNameRoomNo)
                {
                    data.Rows[0][colName] = base.CmpAttribute.ResourceText("IDS_TXT_SITSUBANGO");
                    data.Rows[1][colName] = "";
                    continue;
                }

                // 面積
                if (colName == base.ColNameArea)
                {
                    data.Rows[0][colName] = base.CmpAttribute.ResourceText("IDS_TXT_LEGALAREA");
                    data.Rows[1][colName] = "(" + base.CmpAttribute.ResourceText("IDS_TXT_M2") + ")";
                    continue;
                }

                // 必要係数
                if (colName == base.ColNameNecessaryCoefficient)
                {
                    sValue = "";
                    switch (base.CommandKind)
                    {
                        case 0:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_LIGHTINGNESCOEFF");
                            break;

                        case 1:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_SMOKENESCOEFF");
                            break;

                        case 2:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_VENTILATIONNESCOEFF");
                            break;
                    }
                    data.Rows[0][colName] = sValue;
                    data.Rows[1][colName] = "(" + base.CmpAttribute.ResourceText("IDS_TXT_1SX") + ")";
                    continue;
                }

                // 必要面積
                if (colName == base.ColNameNecessaryArea)
                {
                    sValue = "";
                    switch (base.CommandKind)
                    {
                        case 0:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_LIGHTINGNESAREA");
                            break;

                        case 1:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_SMOKENESAREA");
                            break;

                        case 2:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_VENTILATIONNESAREA");
                            break;
                    }
                    data.Rows[0][colName] = sValue;
                    data.Rows[1][colName] = "(" + base.CmpAttribute.ResourceText("IDS_TXT_M2") + ")";
                    continue;
                }

                // 建具符号
                if (colName == base.ColNameSign)
                {
                    data.Rows[0][colName] = base.CmpAttribute.ResourceText("IDS_TXT_PARTSSIGN");
                    data.Rows[1][colName] = "";
                    continue;
                }

                // 水平距離
                if (colName == base.ColNameHorizontalDist)
                {
                    data.Rows[0][colName] = "d";
                    data.Rows[1][colName] = "";
                    continue;
                }

                // 垂直距離
                if (colName == base.ColNameVerticalDist)
                {
                    data.Rows[0][colName] = "h";
                    data.Rows[1][colName] = "";
                    continue;
                }

                // d/h
                if (colName == base.ColNameDsH)
                {
                    data.Rows[0][colName] = "d/h";
                    data.Rows[1][colName] = "";
                    continue;
                }

                // α
                if (colName == base.ColNameA)
                {
                    data.Rows[0][colName] = "α";
                    data.Rows[1][colName] = "";
                    continue;
                }

                // β
                if (colName == base.ColNameB)
                {
                    data.Rows[0][colName] = "β";
                    data.Rows[1][colName] = "";
                    continue;
                }

                // D
                if (colName == base.ColNameD)
                {
                    data.Rows[0][colName] = "D";
                    data.Rows[1][colName] = "";
                    continue;
                }

                // A(仮)
                if (colName == base.ColNameATemp)
                {
                    data.Rows[0][colName] = "A";
                    data.Rows[1][colName] = "(" + base.CmpAttribute.ResourceText("IDS_TXT_TEMP") + ")";
                    continue;
                }

                // A(補正値)
                if (colName == base.ColNameACorr)
                {
                    data.Rows[0][colName] = "A";
                    data.Rows[1][colName] = "(" + base.CmpAttribute.ResourceText("IDS_TXT_CORRECTIONVALUE") + ")";
                    continue;
                }

                // 開口係数
                if (colName == base.ColNameOpenCoefficient)
                {
                    data.Rows[0][colName] = base.CmpAttribute.ResourceText("IDS_TXT_OPENCOEFF");
                    data.Rows[1][colName] = "";
                    continue;
                }

                // 有効幅
                if ((colName == base.ColNameUsableWidth) || (colName == base.ColNameSmokeWinWidth))
                {
                    sValue = "";
                    switch (base.CommandKind)
                    {
                        case 0:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_OPENINGUSABLEWIDTH");
                            break;

                        case 1:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEWIDTH");
                            break;

                        case 2:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEWIDTH");
                            break;
                    }
                    data.Rows[0][colName] = sValue;
                    data.Rows[1][colName] = "";
                    continue;
                }

                // 有効高さ
                if ((colName == base.ColNameUsableHeight) || (colName == base.ColNameUsableHeightSmoke))
                {
                    sValue = "";
                    switch (base.CommandKind)
                    {
                        case 0:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_OPENINGUSABLEHEIGHT");
                            break;

                        case 1:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEHEIGHT");
                            break;

                        case 2:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEHEIGHT");
                            break;
                    }
                    data.Rows[0][colName] = sValue;
                    data.Rows[1][colName] = "";
                    continue;
                }

                // 有効開口面積
                if (colName == base.ColNameUsableOpenArea)
                {
                    data.Rows[0][colName] = base.CmpAttribute.ResourceText("IDS_TXT_OPENINGUSABLEAREA");
                    data.Rows[1][colName] = "";
                    continue;
                }

                // 有効面積
                if (colName == base.ColNameUsableArea)
                {
                    sValue = "";
                    switch (base.CommandKind)
                    {
                        case 0:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_LIGHTINGUSABLEAREA");
                            break;

                        case 1:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEAREA");
                            break;

                        case 2:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEAREA");
                            break;
                    }
                    data.Rows[0][colName] = sValue;
                    data.Rows[1][colName] = "";
                    continue;
                }

                // 合計有効面積
                if (colName == ColNameTotalUsableArea)
                {
                    sValue = "";
                    switch (base.CommandKind)
                    {
                        case 0:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_LIGHTINGUSABLEAREA");
                            break;

                        case 1:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEAREA");
                            break;

                        case 2:
                            sValue = base.CmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEAREA");
                            break;
                    }
                    data.Rows[0][colName] = base.CmpAttribute.ResourceText("IDS_TXT_TOTAL");
                    data.Rows[1][colName] = sValue;
                    continue;
                }

                // 判定
                if (colName == ColNameJudgment)
                {
                    data.Rows[0][colName] = base.CmpAttribute.ResourceText("IDS_TXT_JUDGMENT");
                    data.Rows[1][colName] = "";
                    continue;
                }
            }
        }

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <param name="entDtRoom"    >データテーブル - 部屋</param>
        /// <param name="entDtWinDoor" >データテーブル - 建具</param>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void SetData(RvtExtApp.Entities.DtRoom entDtRoom,
                     RvtExtApp.Entities.DtWinDoor entDtWinDoor)
        {
            if (_Data == null)
            {
                _Data = new System.Data.DataTable();
                DefDataFormat(ref _Data);
                SetHeader(ref _Data);
            }

            System.Data.DataTable dataRoom = entDtRoom.Data;
            System.Data.DataTable dataWinDoor = entDtWinDoor.Data;

            if (dataRoom == null)
            {
                return;
            }

            base.SetVisbleRooms(dataRoom);
            if (dataRoom.DefaultView.Count == 0)
            {
                return;
            }

            // 階/ グループ名種類
            Collections.Generic.IList<string> kindFloors = new Collections.Generic.List<string>();
            Collections.Generic.IList<string> kindGroups = new Collections.Generic.List<string>();
            foreach (System.Data.DataRowView rowViewRoom in dataRoom.DefaultView)
            {
                string levelName = rowViewRoom[base.ColNameLevelName].ToString();
                if (kindFloors.Contains(levelName) == false)
                {
                    kindFloors.Add(levelName);
                }

                string groupName = rowViewRoom[base.ColNameGroupName].ToString();
                if (groupName != base.CmpAttribute.ResourceText("IDS_TXT_NOTHING"))
                {
                    if (kindGroups.Contains(groupName) == false)
                    {
                        kindGroups.Add(groupName);
                    }
                }
            }
            kindGroups.Add(base.CmpAttribute.ResourceText("IDS_TXT_NOTHING"));

            int roomID = 0;
            //string  levelNameCur = "";
            //string  groupNameCur = "";
            int levelCnt = 0;
            int groupCnt = 0;
            double area = 0.0;
            double areaNec = 0.0;
            double areaUsa = 0.0;
            bool flag = false;
            bool flagGroup = false;

            string sValue = "";
            double dValue = 0.0;

            System.Data.DataRow row;
            _RowBordersAryAry = new Collections.Generic.List<Collections.Generic.IList<int>>();
            Collections.Generic.IList<int> bordersData;
            int bordersCol = 0;

            // 階
            for (int i = 0; i < kindFloors.Count; ++i)
            {
                string levelName = kindFloors[i];
                levelCnt = 0;

                // グループ
                for (int j = 0; j < kindGroups.Count; ++j)
                {
                    string groupName = kindGroups[j];

                    flagGroup = true;
                    if (groupName == base.CmpAttribute.ResourceText("IDS_TXT_NOTHING"))
                    {
                        flagGroup = false;
                    }
                    groupCnt = 0;

                    // 部屋
                    base.SetVisbleRooms(dataRoom, levelName, groupName);

                    areaNec = 0.0;
                    areaUsa = 0.0;
                    foreach (System.Data.DataRowView rowViewRoom in dataRoom.DefaultView)
                    {
                        levelCnt++;
                        groupCnt++;

                        // 必要面積
                        area = 0.0;
                        sValue = rowViewRoom[base.ColNameNecessaryArea].ToString();
                        if (UtilValue.IsNumber(sValue) == true)
                        {
                            area = double.Parse(sValue);
                        }
                        areaNec += area;

                        // 合計有効面積
                        area = 0.0;
                        sValue = rowViewRoom[base.ColNameTotalUsableArea].ToString();
                        if (UtilValue.IsNumber(sValue) == true)
                        {
                            area = double.Parse(sValue);
                        }
                        areaUsa += area;

                        // roomID
                        roomID = 0;
                        sValue = rowViewRoom[base.ColNameID].ToString();
                        if (UtilValue.IsNumber(sValue) == true)
                        {
                            roomID = int.Parse(sValue);
                        }
                        base.SetVisbleWinDoor(dataWinDoor, roomID);

                        // 行作成
                        if (_Data.Rows.Count != 2)
                        {
                            if (groupCnt == 1)
                            {
                                row = _Data.NewRow();
                                _Data.Rows.Add(row);
                            }
                        }
                        row = _Data.NewRow();

                        // 部屋設定
                        for (int k = 0; k < _Data.Columns.Count; ++k)
                        {
                            string colNameExcel = _Data.Columns[k].ColumnName;
                            System.Type sysType = _ColSysType[k];
                            flag = false;

                            // レベル名
                            if (colNameExcel == base.ColNameLevelName)
                            {
                                flag = true;
                                if (levelCnt == 1)
                                {
                                    sValue = levelName;
                                }
                                else
                                {
                                    sValue = "";
                                }
                            }
                            // グループ名
                            else if (colNameExcel == base.ColNameGroupName)
                            {
                                flag = true;
                                if (flagGroup == true)
                                {
                                    if (groupCnt == 1)
                                    {
                                        sValue = groupName;
                                    }
                                    else
                                    {
                                        sValue = "";
                                    }
                                }
                                else
                                {
                                    sValue = "";
                                }
                            }
                            // 判定
                            else if (colNameExcel == ColNameJudgment)
                            {
                                flag = true;
                                if (flagGroup == false)
                                {
                                    sValue = rowViewRoom[ColNameJudgment].ToString();
                                }
                                else
                                {
                                    sValue = "";
                                }
                            }
                            // その他
                            else
                            {
                                if (dataRoom.Columns.Contains(colNameExcel) == true)
                                {
                                    flag = true;
                                    sValue = rowViewRoom[colNameExcel].ToString();
                                }
                            }

                            //　列設定
                            if (flag == true)
                            {
                                // 数値
                                if (sysType == typeof(double))
                                {
                                    if (UtilValue.IsNumber(sValue) == true)
                                    {
                                        //sValue = UtilValue.Rounding(sValue, 3, 2);
                                        sValue = sValue.ToString();
                                    }
                                    else
                                    {
                                        sValue = "0.0";
                                    }
                                }

                                row[colNameExcel] = sValue;
                            }
                        }

                        // 行罫線
                        bordersData = new Collections.Generic.List<int>();
                        bordersData.Add(_Data.Rows.Count);
                        bordersCol = 1;
                        if (levelCnt != 1)
                        {
                            bordersCol = 2;
                        }
                        if (groupCnt != 1)
                        {
                            bordersCol = 3;
                        }
                        bordersData.Add(bordersCol);
                        _RowBordersAryAry.Add(bordersData);

                        // 建具設定
                        bool flagArea = true;
                        System.Data.DataView dataViewWinDoor = dataWinDoor.DefaultView;
                        if (dataViewWinDoor.Count > 0)
                        {
                            foreach (System.Data.DataRowView rowViewWinDoor in dataViewWinDoor)
                            {
                                // 有効面積
                                area = 0.0;
                                sValue = rowViewWinDoor[ColNameUsableArea].ToString();
                                if (UtilValue.IsNumber(sValue) == true)
                                {
                                    area = double.Parse(sValue);
                                }

                                if (area <= 0.0)
                                {
                                    continue;
                                }

                                if (flagArea == true)
                                {
                                    flagArea = false;
                                }
                                else
                                {
                                    row = _Data.NewRow();
                                }

                                // 列データ設定
                                for (int k = 0; k < _Data.Columns.Count; ++k)
                                {
                                    string colNameExcel = _Data.Columns[k].ColumnName;
                                    flag = false;
                                    System.Type sysType = _ColSysType[k];

                                    if (dataWinDoor.Columns.Contains(colNameExcel) == true)
                                    {
                                        sValue = rowViewWinDoor[colNameExcel].ToString();
                                        flag = true;
                                    }
                                    if (flag == true)
                                    {
                                        // 数値
                                        if (sysType == typeof(double))
                                        {
                                            if (UtilValue.IsNumber(sValue) == true)
                                            {
                                                dValue = double.Parse(sValue);
                                                if (_ColConvMAry[k] == true)
                                                {
                                                    dValue /= 1000.0;
                                                }
                                                //sValue = UtilValue.Rounding(dValue, 3, 2);
                                                sValue = dValue.ToString();
                                            }
                                            else
                                            {
                                                sValue = "0.0";
                                            }
                                        }
                                        row[colNameExcel] = sValue;
                                    }
                                }
                                _Data.Rows.Add(row);
                            }
                        }
                        else
                        {
                            _Data.Rows.Add(row);
                        }
                    }
                    // グループ集計
                    if (flagGroup == true)
                    {
                        if (groupCnt > 0)
                        {
                            row = _Data.NewRow();

                            // 行罫線
                            bordersData = new Collections.Generic.List<int>();
                            bordersData.Add(_Data.Rows.Count);
                            bordersCol = 3;
                            bordersData.Add(bordersCol);
                            _RowBordersAryAry.Add(bordersData);

                            // 必要面積
                            row[base.ColNameNecessaryArea] = areaNec.ToString();

                            // 合計有効面積
                            row[base.ColNameTotalUsableArea] = areaUsa.ToString();

                            // 判定
                            sValue = "NG";
                            if (areaNec < areaUsa)
                            {
                                sValue = "OK";
                            }
                            row[base.ColNameJudgment] = sValue;

                            _Data.Rows.Add(row);
                        }
                    }
                }
            }
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>データ</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Data.DataTable Data
        {
            get
            {
                return _Data;
            }
        }

        /// ================================================================================
        /// <summary>列整列フラグ</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        Collections.Generic.IList<int> ColAlignmentAry
        {
            get
            {
                return _ColAlignmentAry;
            }
            set
            {
                _ColAlignmentAry = value;
            }
        }

        /// ================================================================================
        /// <summary>列幅</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        Collections.Generic.IList<double> ColWidthAry
        {
            get
            {
                return _ColWidthAry;
            }
            set
            {
                _ColWidthAry = value;
            }
        }

        /// ================================================================================
        /// <summary>行罫線フラグ</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Collections.Generic.IList<int>> RowBordersAryAry
        {
            get
            {
                return _RowBordersAryAry;
            }
            set
            {
                _RowBordersAryAry = value;
            }
        }

        #endregion Properties
    }
}