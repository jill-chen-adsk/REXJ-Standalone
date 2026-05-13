
using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;

namespace ADSK.JExtRAC.CheckingALVS.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - 建具</summary>
    /// ================================================================================
    public class DtWinDoor : RvtExtApp.Entities.DtBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>データテーブル - コマンド</summary>
        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        /// <summary>共有パラメータ - 建具</summary>
        private RvtExtApp.Entities.SpWinDoor _EntSpWinDoor;

        /// <summary>共有パラメータ - 建具タイプ</summary>
        private RvtExtApp.Entities.SpWinDoorType _EntSpWinDoorType;

        /// <summary>データ</summary>
        private System.Data.DataTable _Data;

        /// <summary>データ - 部屋</summary>
        private System.Data.DataTable _DataRoom;

        /// <summary>建具</summary>
        private Collections.Generic.IList<Revit.DB.FamilyInstance> _WinDoors;

        /// <summary>図面からの建具</summary>
        private Collections.Generic.IList<int> _WinDoorFromDraw;

        /// <summary>建具の水平距離</summary>
        private Collections.Generic.IList<string> _WinDoorDistHoriAry;

        /// <summary>建具の垂直距離</summary>
        private Collections.Generic.IList<string> _WinDoorDistVertAry;

        /// <summary>建具の所属部屋</summary>
        private Collections.Generic.IList<int> _WinDoorAffRoomAry;

        /// <summary>列名 幅</summary>
        private string _ColNameWidth;

        /// <summary>列名 高さ</summary>
        private string _ColNameHeight;

        /// <summary>列名 縁側</summary>
        private string _ColNameVeranda;

        /// <summary>列名 道路</summary>
        private string _ColNameRoadSide;

        /// <summary>列名 水平測定距離</summary>
        private string _ColNameDistHorizontalMeas;

        /// <summary>列名 水平補正距離</summary>
        private string _ColNameDistHorizontalCorr;

        /// <summary>列名 垂直測定距離</summary>
        private string _ColNameDistVerticalMeas;

        /// <summary>列名 垂直補正距離</summary>
        private string _ColNameDistVerticalCorr;

        /// <summary>列名 天端高さ</summary>
        private string _ColNameHeadHeight;

        /// <summary>列名 天井高さ</summary>
        private string _ColNameCeilingHeight;

        /// <summary>列名 防煙壁長さ</summary>
        private string _ColNameSmokeWallLength;

        /// <summary>列名 TypeID</summary>
        private string _ColNameTypeID;

        /// <summary>列名 変更前有効幅</summary>
        private string _ColNameUsableWidthCB;

        /// <summary>列名 変更後有効幅</summary>
        private string _ColNameUsableWidthCA;

        /// <summary>列名 変更前有効高さ</summary>
        private string _ColNameUsableHeightCB;

        /// <summary>列名 変更前有効高さ</summary>
        private string _ColNameUsableHeightCA;

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
        public DtWinDoor(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.Elements cmpElements,
                         RvtExtApp.Components.Geometry cmpGeometry,
                         RvtExtApp.Components.Parameters cmpParameters,
                         RvtExtApp.Components.Settings cmpSettings
                         ) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            // 共有パラメータ - 建具
            _EntSpWinDoor = new RvtExtApp.Entities.SpWinDoor(cmpAttribute, cmpParameters, cmpSettings);
            if (_EntSpWinDoor.DefSuccess == false)
            {
                string strCategory = base.CmpAttribute.ResourceText("IDS_TXT_CATEGORY");
                string strParam = base.CmpAttribute.ResourceText("IDS_TXT_PARAMETER");
                base.ErrMsg = base.CmpAttribute.ResourceText("IDS_ERR_PARAMDEF") + "\n" +
                                          strCategory + " = " + _EntSpWinDoor.DefCatName + "\n" +
                                          "    " + strParam + "[" + _EntSpWinDoor.ErrDefName + "]";
            }

            // 共有パラメータ - 建具タイプ
            _EntSpWinDoorType = new RvtExtApp.Entities.SpWinDoorType(cmpAttribute, cmpParameters, cmpSettings);
            if (_EntSpWinDoorType.DefSuccess == false)
            {
                string strCategory = base.CmpAttribute.ResourceText("IDS_TXT_CATEGORY");
                string strParam = base.CmpAttribute.ResourceText("IDS_TXT_PARAMETER");
                base.ErrMsg = base.CmpAttribute.ResourceText("IDS_ERR_PARAMDEF") + "\n" +
                                          strCategory + " = " + _EntSpWinDoorType.DefCatName + "\n" +
                                          "    " + strParam + "[" + _EntSpWinDoorType.ErrDefName + "]";
            }
            _WinDoorFromDraw = new Collections.Generic.List<int>();
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
        private void DefDataFormat(ref System.Data.DataTable data)
        {
            // ID
            data.Columns.Add(base.ColNameID, typeof(int));

            // カテゴリ
            data.Columns.Add(ColNameCategory, typeof(int));

            // 所属部屋
            data.Columns.Add(base.ColNameAffiliationRoom, typeof(int));

            // 幅
            data.Columns.Add(ColNameWidth, typeof(string));

            // 高さ
            data.Columns.Add(ColNameHeight, typeof(string));

            // 符号
            data.Columns.Add(base.ColNameSign, typeof(string));

            // 縁側
            data.Columns.Add(ColNameVeranda, typeof(bool));

            // 道路
            data.Columns.Add(ColNameRoadSide, typeof(bool));

            // 水平測定距離
            data.Columns.Add(ColNameDistHorizontalMeas, typeof(string));

            // 水平補正距離
            data.Columns.Add(ColNameDistHorizontalCorr, typeof(string));

            // 水平距離
            data.Columns.Add(base.ColNameHorizontalDist, typeof(string));

            // 垂直測定距離
            data.Columns.Add(ColNameDistVerticalMeas, typeof(string));

            // 垂直補正距離
            data.Columns.Add(ColNameDistVerticalCorr, typeof(string));

            // 垂直距離
            data.Columns.Add(base.ColNameVerticalDist, typeof(string));

            // d/h
            data.Columns.Add(base.ColNameDsH, typeof(string));

            // α
            data.Columns.Add(base.ColNameA, typeof(string));

            // β
            data.Columns.Add(base.ColNameB, typeof(string));

            // D
            data.Columns.Add(base.ColNameD, typeof(string));

            // A(仮)
            data.Columns.Add(base.ColNameATemp, typeof(string));

            // A(補正値)
            data.Columns.Add(base.ColNameACorr, typeof(string));

            // 開口係数
            data.Columns.Add(base.ColNameOpenCoefficient, typeof(string));

            // 天端高さ
            data.Columns.Add(ColNameHeadHeight, typeof(string));

            // 天井高さ
            data.Columns.Add(ColNameCeilingHeight, typeof(string));

            // 防煙壁長さ
            data.Columns.Add(ColNameSmokeWallLength, typeof(string));

            // 有効幅
            data.Columns.Add(base.ColNameUsableWidth, typeof(string));

            // 有効高さ
            data.Columns.Add(base.ColNameUsableHeight, typeof(string));

            // 排煙有効高さ
            data.Columns.Add(base.ColNameUsableHeightSmoke, typeof(string));

            // 排煙窓幅
            data.Columns.Add(ColNameSmokeWinWidth, typeof(string));

            // 排煙窓高さ
            data.Columns.Add(ColNameSmokeWinHeight, typeof(string));

            // 有効開口面積
            data.Columns.Add(base.ColNameUsableOpenArea, typeof(string));

            // 有効面積
            data.Columns.Add(base.ColNameUsableArea, typeof(string));
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="useDistrictOpt"  >用途地域オプション</param>
        /// <param name="elemWinDoor"     >要素 - 建具</param>
        /// <param name="winDoorDistHori" >建具の水平距離</param>
        /// <param name="winDoorDistVert" >建具の垂直距離</param>
        /// <param name="winDoorAffRoom"  >建具の所属部屋</param>
        /// <param name="row"             >行データ</param>
        ///
        /// <history><p>2011/07/29 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2021/11/24 Modified Applied Technology<p></history>
        /// ================================================================================
        public void GetData(int useDistrictOpt,
                     Revit.DB.FamilyInstance elemWinDoor,
                     string winDoorDistHori,
                     string winDoorDistVert,
                     int winDoorAffRoom,
                     ref System.Data.DataRow row)
        {
            double dValue = 0.0;
            string sValue = "";

            // 要素
            Revit.DB.FamilySymbol elemWinDoorType = elemWinDoor.Symbol;
            _EntSpWinDoor.CurrentElem = elemWinDoor;
            _EntSpWinDoorType.CurrentElem = elemWinDoorType;

            // ID
            row[base.ColNameID] = elemWinDoor.Id.ToString();

            // カテゴリ
            row[base.ColNameCategory] = base.CmpSettings.GetWinDoorSymbolType(elemWinDoorType);

            // 所属部屋
            row[ColNameAffiliationRoom] = winDoorAffRoom;

            // 建具幅
            string width = (_EntSpWinDoorType.Width * base.CmpGeometry.UnitCoe).ToString();
            row[ColNameWidth] = width;

            // 建具高さ
            string height = (_EntSpWinDoorType.Height * base.CmpGeometry.UnitCoe).ToString();
            row[ColNameHeight] = height;

            // 建具符号
            string partsMark = _EntSpWinDoorType.Mark;
            string partsNumber = _EntSpWinDoorType.No;
            row[base.ColNameSign] = partsMark + partsNumber;

            // 建具-縁側
            bool veranda = _EntSpWinDoor.Veranda;
            row[ColNameVeranda] = veranda;

            // 建具-道路
            bool roadSide = _EntSpWinDoor.RoadSide;
            row[ColNameRoadSide] = roadSide;

            // 水平測定距離
            double horizontalMeas = 0.0;
            dValue = _EntSpWinDoor.HorizontalMeasDist;
            if (dValue == 0.0)
            {
                horizontalMeas = double.Parse(winDoorDistHori);
            }
            else
            {
                horizontalMeas = dValue * base.CmpGeometry.UnitCoe;
            }
            row[ColNameDistHorizontalMeas] = UtilValue.Rounding(horizontalMeas, 1, 2);

            // 水平補正距離
            double horizontalCorr = 0.0;
            dValue = _EntSpWinDoor.HorizontalCorrDist;
            horizontalCorr = dValue * base.CmpGeometry.UnitCoe;
            row[ColNameDistHorizontalCorr] = UtilValue.Rounding(horizontalCorr, 1, 2);

            // 水平距離
            double horizontal = horizontalMeas + horizontalCorr;
            row[base.ColNameHorizontalDist] = UtilValue.Rounding(horizontal, 1, 2);

            string strHorizontal = horizontal.ToString();

            // 垂直測定距離
            double verticalMeas = 0.0;
            dValue = _EntSpWinDoor.VerticalMeasDist;
            if (dValue == 0.0)
            {
                verticalMeas = double.Parse(winDoorDistVert);
            }
            else
            {
                verticalMeas = dValue * base.CmpGeometry.UnitCoe;
            }
            row[ColNameDistVerticalMeas] = UtilValue.Rounding(verticalMeas, 1, 2);

            // 垂直補正距離
            double verticalCorr = 0.0;
            dValue = _EntSpWinDoor.VerticalCorrDist;
            verticalCorr = dValue * base.CmpGeometry.UnitCoe;
            row[ColNameDistVerticalCorr] = UtilValue.Rounding(verticalCorr, 1, 2);

            // 垂直距離
            double vertical = verticalMeas + verticalCorr;
            row[base.ColNameVerticalDist] = UtilValue.Rounding(vertical, 1, 2);

            string strVertical = vertical.ToString();

            // d/h
            string dsh = GetDsH(row[base.ColNameHorizontalDist].ToString(), row[base.ColNameVerticalDist].ToString());
            row[base.ColNameDsH] = dsh;

            string a = "-";
            string b = "-";
            string d = "-";
            GetUseDistrictValue(useDistrictOpt, ref a, ref b, ref d);

            // α
            row[base.ColNameA] = a;

            // β
            row[base.ColNameB] = b;

            // D
            row[base.ColNameD] = d;

            // A(仮)
            string aTemp = GetAtempValue(row[base.ColNameHorizontalDist].ToString(), row[base.ColNameVerticalDist].ToString(), a, b);
            row[base.ColNameATemp] = UtilValue.Rounding(aTemp, 4, 2);

            // A(補正値)
            string aCorr = GetACorrValue(veranda, roadSide, strHorizontal, d, aTemp);
            row[base.ColNameACorr] = aCorr;

            // 開口係数
            string openCoefficient = (_EntSpWinDoorType.OpenCoeff).ToString();
            row[base.ColNameOpenCoefficient] = UtilValue.Rounding(openCoefficient, 3, 2);

            // 天端高さ
            string headHeight = "0.0";
            dValue = _EntSpWinDoor.HeadHeight;
            if (dValue == 0.0)
            {
                headHeight = (_EntSpWinDoor.HeadHeightBltIn * base.CmpGeometry.UnitCoe).ToString();
            }
            else
            {
                headHeight = (dValue * base.CmpGeometry.UnitCoe).ToString();
            }
            row[ColNameHeadHeight] = UtilValue.Rounding(headHeight, 1, 2);

            // 天井高さ
            string ceilingHeight = "0.0";
            dValue = _EntSpWinDoor.CeilingHeight;
            if (dValue == 0.0)
            {
                ceilingHeight = UtilData.GetValueTableData(_DataRoom,
                                                                  base.ColNameID,
                                                                  CmpElements.GetIdFromRoom(elemWinDoor).ToString(),
                                                                  base.ColNameAverageCeilingHeight);
                if (UtilValue.IsNumber(ceilingHeight) == false)
                {
                    ceilingHeight = UtilData.GetValueTableData(_DataRoom,
                                                                      base.ColNameID,
                                                                      CmpElements.GetIdToRoom(elemWinDoor).ToString(),
                                                                      base.ColNameAverageCeilingHeight);
                    if (UtilValue.IsNumber(ceilingHeight) == false)
                    {
                        ceilingHeight = "0.0";
                    }
                }
            }
            else
            {
                ceilingHeight = (dValue * base.CmpGeometry.UnitCoe).ToString();
            }
            row[ColNameCeilingHeight] = UtilValue.Rounding(ceilingHeight, 1, 2);

            // 防煙壁長さ
            string smokeWallLength = "0.0";
            dValue = _EntSpWinDoor.SmokeWallLength;
            if (dValue == 0.0)
            {
                smokeWallLength = GetDefaultSmokeWallLengthDefault();
            }
            else
            {
                smokeWallLength = (dValue * base.CmpGeometry.UnitCoe).ToString();
            }
            row[ColNameSmokeWallLength] = UtilValue.Rounding(smokeWallLength, 1, 2);

            // 有効幅
            string usableWidth = "0.0";
            dValue = _EntSpWinDoorType.UsableWidth;
            if (dValue == 0.0)
            {
                usableWidth = width;
            }
            else
            {
                usableWidth = (dValue * base.CmpGeometry.UnitCoe).ToString();
            }
            row[base.ColNameUsableWidth] = UtilValue.Rounding(usableWidth, 1, 2);

            // 有効高さ
            string usableHeight = "0.0";
            dValue = _EntSpWinDoorType.UsableHeight;
            if (dValue == 0.0)
            {
                usableHeight = height;
            }
            else
            {
                usableHeight = (dValue * base.CmpGeometry.UnitCoe).ToString();
            }
            row[base.ColNameUsableHeight] = UtilValue.Rounding(usableHeight, 1, 2);

            // 排煙有効高さ
            string usableHeightSmoke = "0.0";
            dValue = _EntSpWinDoor.UsableHeightSmoke;
            if (dValue == 0.0)
            {
                usableHeightSmoke = GetUsableHeightSmoke(height, headHeight, ceilingHeight, smokeWallLength);
            }
            else
            {
                usableHeightSmoke = (dValue * base.CmpGeometry.UnitCoe).ToString();
            }
            //row[base.ColNameUsableHeightSmoke] = UtilValue.Rounding(usableHeightSmoke, 1, 2);
            row[base.ColNameUsableHeightSmoke] = usableHeightSmoke.ToString();

            // 排煙窓幅
            string smokeWinWidth = "0.0";
            dValue = _EntSpWinDoorType.SmokeWinWidth;
            if (dValue == 0.0)
            {
                smokeWinWidth = width;
            }
            else
            {
                smokeWinWidth = (dValue * base.CmpGeometry.UnitCoe).ToString();
            }
            row[ColNameSmokeWinWidth] = UtilValue.Rounding(smokeWinWidth, 1, 2);

            // 排煙窓高さ
            string smokeWinHeight = "0.0";
            dValue = _EntSpWinDoorType.SmokeWinHeight;
            if (dValue == 0.0)
            {
                smokeWinHeight = height;
            }
            else
            {
                smokeWinHeight = (dValue * base.CmpGeometry.UnitCoe).ToString();
            }
            row[ColNameSmokeWinHeight] = UtilValue.Rounding(smokeWinHeight, 1, 2);

            // 有効開口面積
            string usableOpenArea = GetUsableOpenArea(usableWidth, usableHeight);
            row[base.ColNameUsableOpenArea] = usableOpenArea.ToString();
            // 有効面積
            sValue = "-";
            switch (base.CommandKind)
            {
                case 0:
                    sValue = GetUsableAreaOpenArea(dsh, aCorr, usableOpenArea);
                    row[base.ColNameUsableArea] = UtilValue.Rounding(sValue, _EntDtCmd.EffectiveLightingAreaRoundingDecimal, _EntDtCmd.EffectiveLightingAreaRoundingOpt);
                    break;

                case 1:
                    sValue = GetUsableArea(smokeWinWidth, usableHeightSmoke, openCoefficient);
                    row[base.ColNameUsableArea] = UtilValue.Rounding(sValue, _EntDtCmd.EffectiveSmokeExtractionAreaRoundingDecimal, _EntDtCmd.EffectiveSmokeExtractionAreaRoundingOtp);
                    break;

                case 2:
                    sValue = GetUsableArea(usableWidth, usableHeight, openCoefficient);
                    row[base.ColNameUsableArea] = UtilValue.Rounding(sValue, _EntDtCmd.EffectiveVentilationAreaRoundingDecimal, _EntDtCmd.EffectiveVentilationAreaRoundingOtp);
                    break;
            }
        }

        /// ================================================================================
        /// <summary>データ取得(オーバーロード)</summary>
        ///
        /// <param name="useDistrictOpt">用途地域オプション</param>
        /// <param name="dataRoom"      >データ - 部屋</param>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void GetData(RvtExtApp.Entities.DtCmd entDtCmd, int useDistrictOpt,
                     System.Data.DataTable dataRoom)
        {
            _EntDtCmd = entDtCmd;
            // データテーブル
            if (_Data == null)
            {
                _Data = new System.Data.DataTable();
                DefDataFormat(ref _Data);
            }

            if ((WinDoors == null) || (WinDoorDistHoriAry == null) || (WinDoorDistVertAry == null) || (WinDoorAffRoomAry == null))
            {
                return;
            }

            if (dataRoom == null)
            {
                return;
            }
            _DataRoom = dataRoom;

            for (int i = 0; i < WinDoors.Count; ++i)
            {
                // 建具の水平距離
                string sDistHori = "0.0";
                if (i < WinDoorDistHoriAry.Count)
                {
                    sDistHori = WinDoorDistHoriAry[i];
                }

                // 建具の垂直距離
                string sDistVert = "0.0";
                if (i < WinDoorDistVertAry.Count)
                {
                    sDistVert = WinDoorDistVertAry[i];
                }

                // 建具の所属部屋
                int iAffRoom = -1;
                if (i < WinDoorAffRoomAry.Count)
                {
                    iAffRoom = WinDoorAffRoomAry[i];
                }

                // データ取得
                System.Data.DataRow row = _Data.NewRow();
                GetData(useDistrictOpt, WinDoors[i], sDistHori, sDistVert, iAffRoom, ref row);

                _Data.Rows.Add(row);
            }
        }

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SetData()
        {
            string sValue = "";
            bool bValue = false;
            double dValue = 0.0;
            double unitCoe = base.CmpGeometry.UnitCoe;

            if ((_Data != null) && (_Data.Rows.Count > 0))
            {
                for (int i = 0; i < _Data.Rows.Count; ++i)
                {
                    /// ファミリインスタンス
                    int id = 0;
                    sValue = _Data.Rows[i][base.ColNameID].ToString();
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        id = int.Parse(sValue);
                    }

                    Revit.DB.FamilyInstance familyInstance = base.CmpElements.GetFamilyInstance(id);
                    Revit.DB.FamilySymbol familySymbol = familyInstance.Symbol;
                    if (familyInstance != null)
                    {
                        _EntSpWinDoor.CurrentElem = familyInstance;
                        _EntSpWinDoorType.CurrentElem = familySymbol;

                        // 縁側
                        bValue = bool.Parse(_Data.Rows[i][ColNameVeranda].ToString());
                        _EntSpWinDoor.Veranda = bValue;

                        // 道路面
                        bValue = bool.Parse(_Data.Rows[i][ColNameRoadSide].ToString());
                        _EntSpWinDoor.RoadSide = bValue;

                        // 水平測定距離
                        dValue = double.Parse(_Data.Rows[i][ColNameDistHorizontalMeas].ToString());
                        _EntSpWinDoor.HorizontalMeasDist = dValue / unitCoe;

                        // 水平補正距離
                        dValue = double.Parse(_Data.Rows[i][ColNameDistHorizontalCorr].ToString());
                        _EntSpWinDoor.HorizontalCorrDist = dValue / unitCoe;

                        // 水平距離
                        dValue = double.Parse(_Data.Rows[i][base.ColNameHorizontalDist].ToString());
                        _EntSpWinDoor.HorizontalDist = dValue / unitCoe;

                        // 垂直測定距離
                        dValue = double.Parse(_Data.Rows[i][ColNameDistVerticalMeas].ToString());
                        _EntSpWinDoor.VerticalMeasDist = dValue / unitCoe;

                        // 垂直補正距離
                        dValue = double.Parse(_Data.Rows[i][ColNameDistVerticalCorr].ToString());
                        _EntSpWinDoor.VerticalCorrDist = dValue / unitCoe;

                        // 垂直距離
                        dValue = double.Parse(_Data.Rows[i][base.ColNameVerticalDist].ToString());
                        _EntSpWinDoor.VerticalDist = dValue / unitCoe;

                        // 有効幅
                        dValue = double.Parse(_Data.Rows[i][base.ColNameUsableWidth].ToString());
                        _EntSpWinDoorType.UsableWidth = dValue / unitCoe;

                        // 有効高さ
                        dValue = double.Parse(_Data.Rows[i][base.ColNameUsableHeight].ToString());
                        _EntSpWinDoorType.UsableHeight = dValue / unitCoe;

                        // 排煙有効高さ
                        dValue = double.Parse(_Data.Rows[i][base.ColNameUsableHeightSmoke].ToString());
                        _EntSpWinDoor.UsableHeightSmoke = dValue / unitCoe;

                        // 排煙窓幅
                        dValue = double.Parse(_Data.Rows[i][ColNameSmokeWinWidth].ToString());
                        _EntSpWinDoorType.SmokeWinWidth = dValue / unitCoe;

                        // 排煙窓高さ
                        dValue = double.Parse(_Data.Rows[i][ColNameSmokeWinHeight].ToString());
                        _EntSpWinDoorType.SmokeWinHeight = dValue / unitCoe;

                        // 開口係数
                        dValue = double.Parse(_Data.Rows[i][base.ColNameOpenCoefficient].ToString());
                        _EntSpWinDoorType.OpenCoeff = dValue;

                        // 天端高さ
                        dValue = double.Parse(_Data.Rows[i][ColNameHeadHeight].ToString());
                        _EntSpWinDoor.HeadHeight = dValue / unitCoe;

                        // 天井高さ
                        dValue = double.Parse(_Data.Rows[i][ColNameCeilingHeight].ToString());
                        _EntSpWinDoor.CeilingHeight = dValue / unitCoe;

                        // 防煙長さ
                        dValue = double.Parse(_Data.Rows[i][ColNameSmokeWallLength].ToString());
                        _EntSpWinDoor.SmokeWallLength = dValue / unitCoe;
                    }
                }
            }

            // テーブルデータソート
            base.SortDataWinDoor(_Data);
        }

        /// ================================================================================
        /// <summary>水平垂直距離取得</summary>
        ///
        /// <param name="distMeas">測定距離</param>
        /// <param name="distCorr">補正距離</param>
        ///
        /// <returns>距離</returns>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string GetDistHoriOrVert(string distMeas, string distCorr)
        {
            string ret = null;

            double dDistMeas = 0.0;
            double dDistCorr = 0.0;
            double dDist = 0.0;

            if (UtilValue.IsNumber(distMeas) == true)
            {
                dDistMeas = double.Parse(distMeas);
            }

            if (UtilValue.IsNumber(distCorr) == true)
            {
                dDistCorr = double.Parse(distCorr);
            }

            dDist = dDistMeas + dDistCorr;
            ret = UtilValue.Rounding(dDist, 1, 2);
            return ret;
        }

        /// ================================================================================
        /// <summary>水平垂直距離取得(オーバーロード)</summary>
        ///
        /// <param name="partsID" >建具ID</param>
        /// <param name="mode"    ><p>モード</p>
        ///                           <p>0 = 水平距離</p>
        ///                           <p>1 = 垂直距離</p></param>
        ///
        /// <returns>距離</returns>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string GetDistHoriOrVert(string partsID, int mode)
        {
            string ret = null;
            string distMeas = "0.0";
            string distCorr = "0.0";

            switch (mode)
            {
                case 0:
                    distMeas = UtilData.GetValueTableData(_Data, ColNameID, partsID, ColNameDistHorizontalMeas);
                    distCorr = UtilData.GetValueTableData(_Data, ColNameID, partsID, ColNameDistHorizontalCorr);
                    break;

                case 1:
                    distMeas = UtilData.GetValueTableData(_Data, ColNameID, partsID, ColNameDistVerticalMeas);
                    distCorr = UtilData.GetValueTableData(_Data, ColNameID, partsID, ColNameDistVerticalCorr);
                    break;
            }

            ret = GetDistHoriOrVert(distMeas, distCorr);
            return ret;
        }

        /// ================================================================================
        /// <summary>d/h取得</summary>
        ///
        /// <param name="horizontal">水平距離</param>
        /// <param name="vertical"  >垂直距離</param>
        ///
        /// <returns>d/h</returns>
        ///
        /// <history><p>2011/08/04 Created GSA,Inc. Shinichi Ishii</p>
        ///     <p>2021/11/24 Modified Applied Technology<p></history>
        /// ================================================================================
        public string GetDsH(string horizontal, string vertical)
        {
            string ret = null;

            double dHorizontal = 0.0;
            double dVertical = 0.0;
            double dsh = 0.0;

            if (UtilValue.IsNumber(horizontal) == true)
            {
                dHorizontal = double.Parse(horizontal);
            }

            if (UtilValue.IsNumber(vertical) == true)
            {
                dVertical = double.Parse(vertical);
            }

            if (dVertical != 0.0)
            {
                dsh = dHorizontal / dVertical;
            }

            if (dsh == 0.0)
            {
                ret = "-";
            }
            else
            {
                ret = UtilValue.Rounding(dsh, _EntDtCmd.DHRoundingDecimal, _EntDtCmd.DHRoundingOpt);
            }

            if (ret == null)
            {
                ret = "-";
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>d/h取得(オーバーロード)</summary>
        ///
        /// <param name="winDoorID">建具ID</param>
        ///
        /// <returns>d/h</returns>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string GetDsH(string winDoorID)
        {
            string ret = null;

            string sHorizontal = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameHorizontalDist);
            string sVertical = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameVerticalDist);

            ret = GetDsH(sHorizontal, sVertical);
            return ret;
        }

        /// ================================================================================
        /// <summary>用途地域値取得</summary>
        ///
        /// <param name="useDistrictOpt">用途地域オプション</param>
        /// <param name="a"             >α値</param>
        /// <param name="b"             >β値</param>
        /// <param name="d"             >D値</param>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void GetUseDistrictValue(int useDistrictOpt, ref string a, ref string b, ref string d)
        {
            string sValue = null;
            string useDistrict = UtilData.GetValueTableData(base.EntDtItems.UseDistrict, useDistrictOpt, 0);

            sValue = UtilData.GetValueTableData(base.EntDtItems.UseDistrict, "Name", useDistrict, "a");
            if (sValue == null)
            {
                sValue = "-";
            }
            a = sValue;

            sValue = UtilData.GetValueTableData(base.EntDtItems.UseDistrict, "Name", useDistrict, "b");
            if (sValue == null)
            {
                sValue = "-";
            }
            b = sValue;

            sValue = UtilData.GetValueTableData(base.EntDtItems.UseDistrict, "Name", useDistrict, "d");
            if (sValue == null)
            {
                sValue = "-";
            }
            d = sValue;
        }

        /// ================================================================================
        /// <summary>用途地域値設定</summary>
        ///
        /// <param name="useDistrictOpt">用途地域オプション</param>
        /// <param name="dgv"           >データグリッドビュー</param>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SetUseDistrictValue(int useDistrictOpt, System.Windows.Forms.DataGridView dgv)
        {
            string a = "-";
            string b = "-";
            string D = "-";
            GetUseDistrictValue(useDistrictOpt, ref a, ref b, ref D);
            int count = dgv.RowCount;
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    dgv[base.ColNameA, i].Value = a;
                    dgv[base.ColNameB, i].Value = b;
                    dgv[base.ColNameD, i].Value = D;
                }
            }
        }

        /// ================================================================================
        /// <summary>A(仮)値取得</summary>
        ///
        /// <param name="horizontal">d</param>
        /// <param name="vertical">h</param>
        /// <param name="a"  >α値</param>
        /// <param name="b"  >β値</param>
        ///
        /// <returns>A(仮)</returns>
        ///
        /// <history><p>2011/08/04 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/11/24 Modified Applied Technology<p></history>
        /// ================================================================================
        public string GetAtempValue(string horizontal, string vertical, string a, string b)
        {
            double dA = 0.0;
            double dB = 0.0;
            double dAtemp = 0.0;

            double dHorizontal = 0.0;
            double dVertical = 0.0;
            double dsh = 0.0;

            if (UtilValue.IsNumber(a) == true)
            {
                dA = double.Parse(a);
            }

            if (UtilValue.IsNumber(b) == true)
            {
                dB = double.Parse(b);
            }

            if (UtilValue.IsNumber(horizontal) == true)
            {
                dHorizontal = double.Parse(horizontal);
            }

            if (UtilValue.IsNumber(vertical) == true)
            {
                dVertical = double.Parse(vertical);
            }

            if (dVertical != 0.0)
            {
                dsh = dHorizontal / dVertical;
            }

            dAtemp = (dsh * dA) - dB;

            return dAtemp.ToString();
        }

        /// ================================================================================
        /// <summary>A(仮)値取得(オーバーロード)</summary>
        ///
        /// <param name="winDoorID">建具ID</param>
        ///
        /// <returns>A(仮)</returns>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string GetAtempValue(string winDoorID)
        {
            string ret = null;
            string shorizontal = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameHorizontalDist);
            string svertical = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameVerticalDist);
            string sA = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameA);
            string sB = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameB);

            ret = UtilValue.Rounding(GetAtempValue(shorizontal, svertical, sA, sB), _EntDtCmd.DHRoundingDecimal, _EntDtCmd.DHRoundingOpt);
            return ret;
        }

        /// ================================================================================
        /// <summary>A(補正)値取得</summary>
        ///
        /// <param name="veranda"   >縁側</param>
        /// <param name="roadSide"  >道路面</param>
        /// <param name="horizontal">水平距離</param>
        /// <param name="d"         >D値</param>
        /// <param name="aTemp"     >A(仮)</param>
        ///
        /// <returns>A(補正)値</returns>
        ///
        /// <history><p>2011/08/04 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/11/24 Modified Applied Technology<p></history>
        /// ================================================================================
        public string GetACorrValue(bool veranda, bool roadSide, string horizontal, string d, string aTemp)
        {
            double dHorizontal = 0.0;
            double dD = 0.0;
            double dAtemp = 0.0;
            double dAcorr = 0.0;

            if (UtilValue.IsNumber(horizontal) == true)
            {
                dHorizontal = double.Parse(horizontal);
                // メート変換
                dHorizontal *= 0.001;
            }

            if (UtilValue.IsNumber(d) == true)
            {
                dD = double.Parse(d);
            }

            if (UtilValue.IsNumber(aTemp) == true)
            {
                dAtemp = double.Parse(aTemp);
            }

            // horizontal-D
            double horizontal_D = dHorizontal - dD;

            // 計算1(c1)
            //  A(仮) > 1 : A(仮)
            //    道路面 = true  : 1
            //    道路面 = false : 0
            double c1 = 0.0;
            if (dAtemp > 1)
            {
                c1 = dAtemp;
            }
            else
            {
                if (roadSide == true)
                {
                    c1 = 1;
                }
            }

            // 計算2(c2)
            //  計算1(c1) >= 1 : 計算1(c1)
            //    水平距離-D値 >= 0 : 1
            //    水平距離-D値 <  0 : 0
            double c2 = 0.0;
            if (c1 >= 1)
            {
                c2 = c1;
            }
            else
            {
                if (horizontal_D >= 0)
                {
                    c2 = 1;
                }
            }

            // 計算3(c3)
            //  計算2(c2) >= 1 : 計算2(c2)
            //    A(仮) >  0 : A(仮)
            double c3 = 0.0;
            if (c2 >= 1)
            {
                c3 = c2;
            }
            else
            {
                if (dAtemp > 0)
                {
                    c3 = dAtemp;
                }
            }

            // 計算4(c4)
            //  計算3(c3) > 3 : 3
            double c4 = c3;
            if (c4 > 3)
            {
                c4 = 3;
            }

            // A(補正値)
            //  縁側 = true  : c4 * 0.7
            //  縁側 = false : c4
            dAcorr = c4;
            if (veranda == true)
            {
                dAcorr = c4 * 0.7;
            }

            return dAcorr.ToString();
        }

        /// ================================================================================
        /// <summary>A(補正)値取得(オーバーロード)</summary>
        ///
        /// <param name="winDoorID">建具ID</param>
        ///
        /// <returns>A(補正)値</returns>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string GetACorrValue(string winDoorID)
        {
            string ret = null;

            bool veranda = bool.Parse(UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, ColNameVeranda));
            bool roadSide = bool.Parse(UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, ColNameRoadSide));

            string sHorizontal = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameHorizontalDist);
            string sD = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameD);
            string sATemp = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameATemp);

            ret = UtilValue.Rounding(GetACorrValue(veranda, roadSide, sHorizontal, sD, sATemp), _EntDtCmd.DHRoundingDecimal, _EntDtCmd.DHRoundingOpt); 
            return ret;
        }

        /// ================================================================================
        /// <summary>有効開口面積取得</summary>
        ///
        /// <param name="width" >幅</param>
        /// <param name="height">高さ</param>
        ///
        /// <returns>有効開口面積</returns>
        ///
        /// <history><p>2011/08/04 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/11/24 Modified Applied Technology<p></history>
        /// ================================================================================
        public string GetUsableOpenArea(string width, string height)
        {
            string ret = null;

            double dwidth = 0.0;
            double dheight = 0.0;
            double dArea = 0.0;

            if (UtilValue.IsNumber(width) == true)
            {
                dwidth = double.Parse(width);
            }

            if (UtilValue.IsNumber(height) == true)
            {
                dheight = double.Parse(height);
            }

            dArea = dwidth * dheight;

            if (dArea == 0.0)
            {
                ret = "-";
            }
            else
            {
                dArea = dArea * System.Math.Pow(0.001, 2.0);
                ret = UtilValue.Rounding(dArea, _EntDtCmd.EffectiveOpeningAreaRoundingDecimal, _EntDtCmd.EffectiveOpeningAreaRoundingOpt);
            }

            if (ret == null)
            {
                ret = "-";
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>有効開口面積取得(オーバーロード)</summary>
        ///
        /// <param name="winDoorID">建具ID</param>
        ///
        /// <returns>有効開口面積</returns>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string GetUsableOpenArea(string winDoorID)
        {
            string ret = null;

            string sUsableWidth = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameUsableWidth);
            string sUsableHeight = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameUsableHeight);
            if (base.CommandKind == 1)
            {
                sUsableHeight = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameUsableHeightSmoke);
            }
            ret = GetUsableOpenArea(sUsableWidth, sUsableHeight);
            return ret;
        }

        /// ================================================================================
        /// <summary>有効開口面積から有効面積取得</summary>
        ///
        /// <param name="dsh"           >d/h</param>
        /// <param name="aCorr"         >A(補正値)値</param>
        /// <param name="usableOpenArea">有効開口面積</param>
        ///
        /// <returns>有効面積</returns>
        ///
        /// <history><p>2011/08/04 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/11/24 Modified Applied Technology<p></history>
        /// ================================================================================
        public string GetUsableAreaOpenArea(string dsh, string aCorr, string usableOpenArea)
        {
            string ret = null;

            double dDsh = 0.0;
            double dACorr = 0.0;
            double dUsableOpenArea = 0.0;
            double dArea = 0.0;

            if (UtilValue.IsNumber(dsh) == true)
            {
                dDsh = double.Parse(dsh);
            }

            if (UtilValue.IsNumber(aCorr) == true)
            {
                dACorr = double.Parse(aCorr);
            }

            if (UtilValue.IsNumber(usableOpenArea) == true)
            {
                dUsableOpenArea = double.Parse(usableOpenArea);
            }

            if (dDsh > 0)
            {
                dArea = dUsableOpenArea * dACorr;
            }

            if (dArea == 0.0)
            {
                ret = "-";
            }
            else
            {
                ret = dArea.ToString();
            }

            if (ret == null)
            {
                ret = "-";
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>有効面積取得</summary>
        ///
        /// <param name="width"           >幅</param>
        /// <param name="height"          >高さ</param>
        /// <param name="openCoefficient" >開口係数</param>
        ///
        /// <returns>有効面積</returns>
        ///
        /// <history><p>2011/08/04 Created GSA,Inc. Shinichi Ishii</p>
        /// <p>2021/11/24 Modified Applied Technology</p></history>
        /// ================================================================================
        public string GetUsableArea(string width, string height, string openCoefficient)
        {
            string ret = null;

            double dwidth = 0.0;
            double dheight = 0.0;
            double dOpenCoefficient = 0.0;
            double dArea = 0.0;

            if (UtilValue.IsNumber(width) == true)
            {
                dwidth = double.Parse(width);
            }

            if (UtilValue.IsNumber(height) == true)
            {
                dheight = double.Parse(height);
            }

            if (UtilValue.IsNumber(openCoefficient) == true)
            {
                dOpenCoefficient = double.Parse(openCoefficient);
            }

            dArea = dwidth * dheight * dOpenCoefficient;

            if (dArea == 0.0)
            {
                ret = "-";
            }
            else
            {
                dArea = dArea * System.Math.Pow(0.001, 2.0);
                ret = dArea.ToString();
            }

            if (ret == null)
            {
                ret = "-";
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>有効面積取得(オーバーロード)</summary>
        ///
        /// <param name="winDoorID">建具ID</param>
        ///
        /// <returns>有効面積</returns>
        ///
        /// <history><p>2011/08/04 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2021/11/24 Modified Applied Technology</p></history>
        /// ================================================================================
        public string GetUsableArea(string winDoorID)
        {
            string ret = null;

            string sDsh = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameDsH);
            string sACorr = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameACorr);
            string sUsableOpenArea = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameUsableOpenArea);
            string sUsableWidth = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameUsableWidth);
            string sUsableHeight = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameUsableHeight);
            string sOpenCoefficient = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameOpenCoefficient);

            if (base.CommandKind == 1)
            {
                sUsableWidth = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, ColNameSmokeWinWidth);
                sUsableHeight = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameUsableHeightSmoke);
            }

            switch (base.CommandKind)
            {
                case 0:
                    ret = UtilValue.Rounding(GetUsableAreaOpenArea(sDsh, sACorr, sUsableOpenArea), _EntDtCmd.EffectiveLightingAreaRoundingDecimal, _EntDtCmd.EffectiveLightingAreaRoundingOpt);
                    break;

                case 1:
                    ret = UtilValue.Rounding(GetUsableArea(sUsableWidth, sUsableHeight, sOpenCoefficient), _EntDtCmd.EffectiveSmokeExtractionAreaRoundingDecimal, _EntDtCmd.EffectiveSmokeExtractionAreaRoundingOtp);
                    break;

                case 2:
                    ret = UtilValue.Rounding(GetUsableArea(sUsableWidth, sUsableHeight, sOpenCoefficient), _EntDtCmd.EffectiveVentilationAreaRoundingDecimal, _EntDtCmd.EffectiveVentilationAreaRoundingOtp);
                    break;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>有効面積設定</summary>
        ///
        /// <param name="winDoorID">建具ID</param>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SetUsableArea(string winDoorID)
        {
            // 建具数
            int rowCountParts = _Data.Rows.Count;
            if (rowCountParts == 0)
            {
                return;
            }

            // 対象建具のファミリシンボルID
            int idCurrent = int.Parse(winDoorID);
            int symbolIdCurrent = base.CmpElements.GetIdFamilySymbol(idCurrent);
            if (symbolIdCurrent == 0)
            {
                return;
            }

            // 対象建具の開口係数
            string sOpenCoefficientCurrent = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameOpenCoefficient);

            // 対象建具の有効幅
            string sUsableWidthCurrent = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameUsableWidth);
            if (base.CommandKind == 1)
            {
                sUsableWidthCurrent = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, ColNameSmokeWinWidth);
            }

            // 対象建具の有効高さ
            string sUsableHeightCurrent = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, base.ColNameUsableHeight);
            if (base.CommandKind == 1)
            {
                sUsableHeightCurrent = UtilData.GetValueTableData(_Data, base.ColNameID, winDoorID, ColNameSmokeWinHeight);
            }

            for (int i = 0; i < rowCountParts; ++i)
            {
                // ID
                int id = int.Parse(_Data.Rows[i][base.ColNameID].ToString());

                // ファミリシンボルID
                int symbolId = base.CmpElements.GetIdFamilySymbol(id);
                if (symbolId == 0)
                {
                    continue;
                }

                // ファミリシンボルID比較
                if (symbolIdCurrent != symbolId)
                {
                    continue;
                }

                // 開口係数設定
                _Data.Rows[i][base.ColNameOpenCoefficient] = UtilValue.Rounding(sOpenCoefficientCurrent, 3, 2);

                // 有効幅設定
                if (base.CommandKind != 1)
                {
                    _Data.Rows[i][base.ColNameUsableWidth] = UtilValue.Rounding(sUsableWidthCurrent, 1, 2);
                }
                else
                {
                    _Data.Rows[i][ColNameSmokeWinWidth] = UtilValue.Rounding(sUsableWidthCurrent, 1, 2);
                }

                // 有効高さ設定
                if (base.CommandKind != 1)
                {
                    _Data.Rows[i][base.ColNameUsableHeight] = UtilValue.Rounding(sUsableHeightCurrent, 1, 2);
                }
                else
                {
                    _Data.Rows[i][ColNameSmokeWinHeight] = UtilValue.Rounding(sUsableHeightCurrent, 1, 2);
                }

                // 有効面積
                switch (base.CommandKind)
                {
                    case 0:
                        _Data.Rows[i][base.ColNameUsableOpenArea] = GetUsableOpenArea(id.ToString());
                        _Data.Rows[i][base.ColNameUsableArea] = GetUsableArea(id.ToString());
                        break;

                    case 1:
                        _Data.Rows[i][base.ColNameUsableArea] = GetUsableArea(id.ToString());
                        break;

                    case 2:
                        _Data.Rows[i][base.ColNameUsableArea] = GetUsableArea(id.ToString());
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary>排煙有効高さ取得</summary>
        ///
        /// <param name="height"          >建具高さ</param>
        /// <param name="headHeight"      >建具天端高さ</param>
        /// <param name="ceilingHeight"   >天井高さ</param>
        /// <param name="smokeWallLength" >防煙壁長さ</param>
        ///
        /// <returns>排煙有効高さ</returns>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string GetUsableHeightSmoke(string height, string headHeight, string ceilingHeight, string smokeWallLength)
        {
            double dHeight = 0.0;
            double dHeadHeight = 0.0;
            double dCeilingHeight = 0.0;
            double dSmokeWallLength = 0.0;
            double dUsableHeightSmoke = 0.0;
            double dValue = 0.0;
            double stdBtm = 0;
            double stdTop = 0;

            if (UtilValue.IsNumber(height) == true)
            {
                dHeight = double.Parse(height);
            }

            if (UtilValue.IsNumber(headHeight) == true)
            {
                dHeadHeight = double.Parse(headHeight);
            }

            if (UtilValue.IsNumber(ceilingHeight) == true)
            {
                dCeilingHeight = double.Parse(ceilingHeight);
            }

            if (UtilValue.IsNumber(smokeWallLength) == true)
            {
                dSmokeWallLength = double.Parse(smokeWallLength);
            }

            // 計算1(c1)
            //  天井高さ >= 3000 :
            //    天井高さ / 2 < 2100 : 2100
            //                       : 天井高さ / 2
            double c1 = 0.0;
            stdBtm = 2100;
            if (dCeilingHeight >= 3000)
            {
                dValue = dCeilingHeight * 0.5;
                if (dValue < stdBtm)
                {
                    c1 = stdBtm;
                }
                else
                {
                    c1 = dValue;
                }
            }

            // 計算2(c2)
            //  計算1(c1) > 0 : 計算1(c1)
            //    防煙壁長さ >= 800 : 天井高さ - 800
            //      防煙壁長さ >= 500 : 天井高さ - 防煙壁長さ
            double c2 = 0.0;
            stdBtm = 500;
            stdTop = 800;
            if (c1 > 0)
            {
                c2 = c1;
            }
            else
            {
                if (dSmokeWallLength >= stdTop)
                {
                    c2 = dCeilingHeight - stdTop;
                }
                else
                {
                    if (dSmokeWallLength >= stdBtm)
                    {
                        c2 = dCeilingHeight - dSmokeWallLength;
                    }
                }
            }

            // 計算3(c3)
            //  計算2(c2) > 0 : 建具天端高さ - 計算2(c2)
            double c3 = -1.0;
            if (c2 > 0)
            {
                c3 = dHeadHeight - c2;
            }

            // 有効高さ
            //  計算3(c3) >= 0
            //    計算3(c3) >= 建具高さ : 建具高さ
            //                          : 計算3(c3)
            if (c3 >= 0)
            {
                if (c3 >= dHeight)
                {
                    dUsableHeightSmoke = dHeight;
                }
                else
                {
                    dUsableHeightSmoke = c3;
                }
            }

            //return UtilValue.Rounding(dUsableHeightSmoke, 3, 2);
            return dUsableHeightSmoke.ToString();
        }

        /// ================================================================================
        /// <summary>排煙有効高さ設定</summary>
        ///
        /// <param name="winDoorID">建具ID</param>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SetUsableHeightSmoke(string winDoorID)
        {
            //　有効面積設定
            SetUsableArea(winDoorID);

            // 建具数
            int rowCountParts = _Data.Rows.Count;
            if (rowCountParts == 0)
            {
                return;
            }

            // 対象建具のファミリシンボルID
            int idCurrent = int.Parse(winDoorID);
            int symbolIdCurrent = base.CmpElements.GetIdFamilySymbol(idCurrent);
            if (symbolIdCurrent == 0)
            {
                return;
            }

            for (int i = 0; i < rowCountParts; ++i)
            {
                // ID
                int id = int.Parse(_Data.Rows[i][base.ColNameID].ToString());

                // ファミリシンボルID
                int symbolId = base.CmpElements.GetIdFamilySymbol(id);
                if (symbolId == 0)
                {
                    continue;
                }

                // ファミリシンボルID比較
                if (symbolIdCurrent != symbolId)
                {
                    continue;
                }

                string sHeight = _Data.Rows[i][ColNameSmokeWinHeight].ToString();
                string sHeadHeight = _Data.Rows[i][ColNameHeadHeight].ToString();
                string sCeilingHeight = _Data.Rows[i][ColNameCeilingHeight].ToString();
                string sSmokeWallLength = _Data.Rows[i][ColNameSmokeWallLength].ToString();

                string UsableHeightSmoke = GetUsableHeightSmoke(sHeight, sHeadHeight, sCeilingHeight, sSmokeWallLength);

                // 排煙有効高さ設定
                _Data.Rows[i][base.ColNameUsableHeightSmoke] = UsableHeightSmoke;
            }
        }

        /// ================================================================================
        /// <summary>有効寸法を変更した建具を取得</summary>
        ///
        /// <returns>結果</returns>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public System.Data.DataTable GetPartsChangedUsableDim()
        {
            // 戻り値
            System.Data.DataTable ret = new System.Data.DataTable();

            // ID
            ret.Columns.Add(base.ColNameID, typeof(int));

            // TypeID
            ret.Columns.Add(ColNameTypeID, typeof(int));

            // 建具符号
            ret.Columns.Add(base.ColNameSign, typeof(string));

            // 変更前有効幅
            ret.Columns.Add(ColNameUsableWidthCB, typeof(string));

            // 変更後有効幅
            ret.Columns.Add(ColNameUsableWidthCA, typeof(string));

            // 変更前有効高さ
            ret.Columns.Add(ColNameUsableHeightCB, typeof(string));

            // 変更前有効高さ
            ret.Columns.Add(ColNameUsableHeightCA, typeof(string));

            // テーブルデータ
            if ((_Data == null))
            {
                return ret;
            }

            int rowCountWinDoor = _Data.Rows.Count;
            if (rowCountWinDoor == 0)
            {
                return ret;
            }

            Collections.Generic.IList<int> typeIdList = new Collections.Generic.List<int>();
            for (int i = 0; i < _Data.Rows.Count; ++i)
            {
                // ID
                int id = int.Parse(_Data.Rows[i][base.ColNameID].ToString());

                Revit.DB.FamilyInstance familyInstance = base.CmpElements.GetFamilyInstance(id);
                if (familyInstance == null)
                {
                    continue;
                }
                Revit.DB.FamilySymbol familySymbol = familyInstance.Symbol;
                _EntSpWinDoorType.CurrentElem = familySymbol;

                // TypeID
                int typeId = Int32.Parse(familySymbol.Id.ToString());

                // 建具符号
                string sign = _Data.Rows[i][base.ColNameSign].ToString();

                // 有効幅
                double usableWidthData = double.Parse(_Data.Rows[i][base.ColNameUsableWidth].ToString());
                double usableWidthElem = _EntSpWinDoorType.UsableWidth * base.CmpGeometry.UnitCoe;
                //if (base.CommandKind == 1)
                //{
                //  usableWidthData = double.Parse(_Data.Rows[i][ColNameSmokeWinWidth].ToString());
                //  usableWidthElem = _EntSpWinDoorType.SmokeWinWidth * base.CmpGeometry.UnitCoe;
                //}

                // 有効高さ
                double usableHeightData = double.Parse(_Data.Rows[i][base.ColNameUsableHeight].ToString());
                double usableHeightElem = _EntSpWinDoorType.UsableHeight * base.CmpGeometry.UnitCoe;
                //if (base.CommandKind == 1)
                //{
                //  usableHeightData = double.Parse(_Data.Rows[i][ColNameSmokeWinHeight].ToString());
                //  usableHeightElem = _EntSpWinDoorType.SmokeWinHeight * base.CmpGeometry.UnitCoe;
                //}

                // 比較
                bool flag = false;
                if (System.Math.Abs(usableWidthData - usableWidthElem) > base.CmpGeometry.Approx0Len)
                {
                    flag = true;
                }
                if (flag == false)
                {
                    if (System.Math.Abs(usableHeightData - usableHeightElem) > base.CmpGeometry.Approx0Len)
                    {
                        flag = true;
                    }
                }

                if (flag == true)
                {
                    bool flag2 = true;
                    if (typeIdList.Count > 0)
                    {
                        for (int j = 0; j < typeIdList.Count; ++j)
                        {
                            if (typeId == typeIdList[j])
                            {
                                flag2 = false;
                                break;
                            }
                        }
                    }

                    if (flag2 == true)
                    {
                        System.Data.DataRow row = ret.NewRow();
                        row[base.ColNameID] = id;
                        row[ColNameTypeID] = typeId;
                        row[base.ColNameSign] = sign;
                        //row[ColNameUsableWidthCB] = UtilValue.Rounding(usableWidthElem, 1, 2);
                        //row[ColNameUsableWidthCA] = UtilValue.Rounding(usableWidthData, 1, 2);
                        //row[ColNameUsableHeightCB] = UtilValue.Rounding(usableHeightElem, 1, 2);
                        //row[ColNameUsableHeightCA] = UtilValue.Rounding(usableHeightData, 1, 2);

                        row[ColNameUsableWidthCB] = usableWidthElem;
                        row[ColNameUsableWidthCA] = usableWidthData;
                        row[ColNameUsableHeightCB] = usableHeightElem;
                        row[ColNameUsableHeightCA] = usableHeightData;

                        ret.Rows.Add(row);
                        typeIdList.Add(typeId);
                    }
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>図面から建具を選択</summary>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SelectWinDoorFromDraw()
        {
            Collections.Generic.IList<Revit.DB.Element> targetElems = new Collections.Generic.List<Revit.DB.Element>();

            // 図形選択
            try
            {
                Collections.Generic.IList<System.Type> sysTypes = new Collections.Generic.List<System.Type>();
                sysTypes.Add(typeof(Revit.DB.FamilyInstance));

                Collections.Generic.IList<Revit.DB.Category> categories = base.CmpSettings.CategoryWinDoor;

                Collections.Generic.IList<Revit.DB.Element> selElems = new Collections.Generic.List<Revit.DB.Element>();

                if (base.CmpElements.GetElementsSelection(sysTypes,
                                                          categories,
                                                          null,
                                                          true,
                                                          base.CmpAttribute.ResourceText("IDS_PRT_SELECTWINDOOR"),
                                                          ref selElems) == false)
                {
                }

                foreach (Revit.DB.Element elem in selElems)
                {
                    targetElems.Add(elem);
                }
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message;
            }

            // 要素ID
            _WinDoorFromDraw = new Collections.Generic.List<int>();
            if (targetElems.Count > 0)
            {
                for (int i = 0; i < targetElems.Count; ++i)
                {
                    _WinDoorFromDraw.Add(Int32.Parse(targetElems[i].Id.ToString()));
                }
            }
        }

        /// ================================================================================
        /// <summary>合計有効面積設定</summary>
        ///
        /// <param name="dataRoom"    >データ - 部屋</param>
        /// <param name="dataWinDoor" >データ - 建具</param>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SetTotalUsableArea(System.Data.DataTable dataRoom, System.Data.DataTable dataWinDoor)
        {
            double area = 0.0;

            if ((dataRoom == null) || (dataWinDoor == null))
            {
                return;
            }

            int rowCountRoom = dataRoom.Rows.Count;
            if (rowCountRoom == 0)
            {
                return;
            }

            int rowCountWinDoor = dataWinDoor.Rows.Count;
            if (rowCountWinDoor == 0)
            {
                return;
            }

            for (int i = 0; i < rowCountRoom; ++i)
            {
                string idRoom = dataRoom.Rows[i][ColNameID].ToString();
                area = 0.0;

                for (int j = 0; j < rowCountWinDoor; ++j)
                {
                    string idAffiliationRoomID = dataWinDoor.Rows[j][ColNameAffiliationRoom].ToString();
                    if (idRoom == idAffiliationRoomID)
                    {
                        string sVal = dataWinDoor.Rows[j][ColNameUsableArea].ToString();
                        if (UtilValue.IsNumber(sVal) == true)
                        {
                            area += double.Parse(sVal);
                        }
                    }
                }
                string sArea = area.ToString();
                if (area == 0.0)
                {
                    sArea = "-";
                }
                switch (_EntDtCmd.CommandKind)
                {
                    case 0:
                        dataRoom.Rows[i][ColNameTotalUsableArea] = UtilValue.Rounding(sArea, _EntDtCmd.EffectiveLightingAreaRoundingDecimal, _EntDtCmd.EffectiveLightingAreaRoundingOpt);
                        break;

                    case 1:
                        dataRoom.Rows[i][ColNameTotalUsableArea] = UtilValue.Rounding(sArea, _EntDtCmd.EffectiveSmokeExtractionAreaRoundingDecimal, _EntDtCmd.EffectiveSmokeExtractionAreaRoundingOtp);

                        break;

                    case 2:
                        dataRoom.Rows[i][ColNameTotalUsableArea] = UtilValue.Rounding(sArea, _EntDtCmd.EffectiveVentilationAreaRoundingDecimal, _EntDtCmd.EffectiveVentilationAreaRoundingOtp);
                        break;
                }
            }
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>共有パラメータ - 建具</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public RvtExtApp.Entities.SpWinDoor EntSpWinDoor
        {
            get
            {
                return _EntSpWinDoor;
            }
        }

        /// ================================================================================
        /// <summary>共有パラメータ - 建具タイプ</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public RvtExtApp.Entities.SpWinDoorType EntSpWinDoorType
        {
            get
            {
                return _EntSpWinDoorType;
            }
        }

        /// ================================================================================
        /// <summary>データ</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public System.Data.DataTable Data
        {
            get
            {
                return _Data;
            }
        }

        /// ================================================================================
        /// <summary>建具</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Collections.Generic.IList<Revit.DB.FamilyInstance> WinDoors
        {
            get
            {
                return _WinDoors;
            }
            set
            {
                _WinDoors = value;
            }
        }

        /// ================================================================================
        /// <summary>建具の水平距離</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Collections.Generic.IList<string> WinDoorDistHoriAry
        {
            get
            {
                return _WinDoorDistHoriAry;
            }
            set
            {
                _WinDoorDistHoriAry = value;
            }
        }

        /// ================================================================================
        /// <summary>建具の垂直距離</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Collections.Generic.IList<string> WinDoorDistVertAry
        {
            get
            {
                return _WinDoorDistVertAry;
            }
            set
            {
                _WinDoorDistVertAry = value;
            }
        }

        /// ================================================================================
        /// <summary>建具の所属部屋</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Collections.Generic.IList<int> WinDoorAffRoomAry
        {
            get
            {
                return _WinDoorAffRoomAry;
            }
            set
            {
                _WinDoorAffRoomAry = value;
            }
        }

        /// ================================================================================
        /// <summary>列名 幅</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameWidth
        {
            get
            {
                if (_ColNameWidth == null)
                {
                    _ColNameWidth = base.CmpAttribute.ResourceText("IDS_COLNAME_WIDTH");
                }
                return _ColNameWidth;
            }
        }

        /// ================================================================================
        /// <summary>列名 高さ</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameHeight
        {
            get
            {
                if (_ColNameHeight == null)
                {
                    _ColNameHeight = base.CmpAttribute.ResourceText("IDS_COLNAME_HEIGHT");
                }
                return _ColNameHeight;
            }
        }

        /// ================================================================================
        /// <summary>列名 縁側</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameVeranda
        {
            get
            {
                if (_ColNameVeranda == null)
                {
                    _ColNameVeranda = base.CmpAttribute.ResourceText("IDS_COLNAME_VERANDA");
                }
                return _ColNameVeranda;
            }
        }

        /// ================================================================================
        /// <summary>列名 道路</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameRoadSide
        {
            get
            {
                if (_ColNameRoadSide == null)
                {
                    _ColNameRoadSide = base.CmpAttribute.ResourceText("IDS_COLNAME_ROADSIDE");
                }
                return _ColNameRoadSide;
            }
        }

        /// ================================================================================
        /// <summary>列名 水平測定距離</summary>
        /// <history>2011/09/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameDistHorizontalMeas
        {
            get
            {
                if (_ColNameDistHorizontalMeas == null)
                {
                    _ColNameDistHorizontalMeas = base.CmpAttribute.ResourceText("IDS_COLNAME_DISTHORIZONTAL_MEAS");
                }
                return _ColNameDistHorizontalMeas;
            }
        }

        /// ================================================================================
        /// <summary>列名 水平補正距離</summary>
        /// <history>2011/09/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameDistHorizontalCorr
        {
            get
            {
                if (_ColNameDistHorizontalCorr == null)
                {
                    _ColNameDistHorizontalCorr = base.CmpAttribute.ResourceText("IDS_COLNAME_DISTHORIZONTAL_CORR");
                }
                return _ColNameDistHorizontalCorr;
            }
        }

        /// ================================================================================
        /// <summary>列名 垂直測定距離</summary>
        /// <history>2011/09/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameDistVerticalMeas
        {
            get
            {
                if (_ColNameDistVerticalMeas == null)
                {
                    _ColNameDistVerticalMeas = base.CmpAttribute.ResourceText("IDS_COLNAME_DISTVERTICAL_MEAS");
                }
                return _ColNameDistVerticalMeas;
            }
        }

        /// ================================================================================
        /// <summary>列名 垂直補正距離</summary>
        /// <history>2011/09/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameDistVerticalCorr
        {
            get
            {
                if (_ColNameDistVerticalCorr == null)
                {
                    _ColNameDistVerticalCorr = base.CmpAttribute.ResourceText("IDS_COLNAME_DISTVERTICAL_CORR");
                }
                return _ColNameDistVerticalCorr;
            }
        }

        /// ================================================================================
        /// <summary>列名 天端高さ</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameHeadHeight
        {
            get
            {
                if (_ColNameHeadHeight == null)
                {
                    _ColNameHeadHeight = base.CmpAttribute.ResourceText("IDS_COLNAME_HEADHEIGHT");
                }
                return _ColNameHeadHeight;
            }
        }

        /// ================================================================================
        /// <summary>列名 天井高さ</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameCeilingHeight
        {
            get
            {
                if (_ColNameCeilingHeight == null)
                {
                    _ColNameCeilingHeight = base.CmpAttribute.ResourceText("IDS_COLNAME_CEILINGHEIGHT");
                }
                return _ColNameCeilingHeight;
            }
        }

        /// ================================================================================
        /// <summary>列名 防煙壁長さ</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameSmokeWallLength
        {
            get
            {
                if (_ColNameSmokeWallLength == null)
                {
                    _ColNameSmokeWallLength = base.CmpAttribute.ResourceText("IDS_COLNAME_SMOKEWALLLENGTH");
                }
                return _ColNameSmokeWallLength;
            }
        }

        /// ================================================================================
        /// <summary>列名 TypeID</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameTypeID
        {
            get
            {
                if (_ColNameTypeID == null)
                {
                    _ColNameTypeID = base.CmpAttribute.ResourceText("IDS_COLNAME_TYPEID");
                }
                return _ColNameTypeID;
            }
        }

        /// ================================================================================
        /// <summary>列名 変更前有効幅</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameUsableWidthCB
        {
            get
            {
                if (_ColNameUsableWidthCB == null)
                {
                    _ColNameUsableWidthCB = base.CmpAttribute.ResourceText("IDS_COLNAME_USABLEWIDTHCB");
                }
                return _ColNameUsableWidthCB;
            }
        }

        /// ================================================================================
        /// <summary>列名 変更後有効幅</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameUsableWidthCA
        {
            get
            {
                if (_ColNameUsableWidthCA == null)
                {
                    _ColNameUsableWidthCA = base.CmpAttribute.ResourceText("IDS_COLNAME_USABLEWIDTHCA");
                }
                return _ColNameUsableWidthCA;
            }
        }

        /// ================================================================================
        /// <summary>列名 変更前有効高さ</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameUsableHeightCB
        {
            get
            {
                if (_ColNameUsableHeightCB == null)
                {
                    _ColNameUsableHeightCB = base.CmpAttribute.ResourceText("IDS_COLNAME_USABLEHEIGHTCB");
                }
                return _ColNameUsableHeightCB;
            }
        }

        /// ================================================================================
        /// <summary>列名 変更前有効高さ</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameUsableHeightCA
        {
            get
            {
                if (_ColNameUsableHeightCA == null)
                {
                    _ColNameUsableHeightCA = base.CmpAttribute.ResourceText("IDS_COLNAME_USABLEHEIGHTCA");
                }
                return _ColNameUsableHeightCA;
            }
        }

        /// ================================================================================
        /// <summary>図面からの建具</summary>
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Collections.Generic.IList<int> WinDoorFromDraw
        {
            get
            {
                return _WinDoorFromDraw;
            }
        }

        #endregion Properties
    }
}