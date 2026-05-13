
using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;

namespace ADSK.JExtRAC.CheckingALVS.Components
{
    /// ================================================================================
    /// <summary>サービス</summary>
    /// ================================================================================
    public class Service
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>要素</summary>
        private RvtExtApp.Components.Elements _CmpElements;

        /// <summary>図形</summary>
        private RvtExtApp.Components.Geometry _CmpGeometry;

        /// <summary>パラメータ</summary>
        private RvtExtApp.Components.Parameters _CmpParameters;

        /// <summary>設定</summary>
        private RvtExtApp.Components.Settings _CmpSettings;

        /// <summary>データテーブル - 部屋</summary>
        private RvtExtApp.Entities.DtRoom _EntDtRoom;

        /// <summary>データテーブル - 建具</summary>
        private RvtExtApp.Entities.DtWinDoor _EntDtWinDoor;

        /// <summary>データテーブル - エクセル</summary>
        private RvtExtApp.Entities.DtExcel _EntDtExcel;

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
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Service(RvtExtApp.Components.Attribute cmpAttribute,
                       RvtExtApp.Components.Elements cmpElements,
                       RvtExtApp.Components.Geometry cmpGeometry,
                       RvtExtApp.Components.Parameters cmpParameters,
                       RvtExtApp.Components.Settings cmpSettings)
        {
            // 初期化
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;

            Revit.DB.Transaction trans = new Autodesk.Revit.DB.Transaction(_CmpElements.RvtDBDoc);

            trans.Start("SetParamDefs");

            // データテーブル - 部屋
            _EntDtRoom = new RvtExtApp.Entities.DtRoom(_CmpAttribute,
                                                       _CmpElements,
                                                       _CmpGeometry,
                                                       _CmpParameters,
                                                       _CmpSettings);

            // データテーブル - 建具
            _EntDtWinDoor = new RvtExtApp.Entities.DtWinDoor(_CmpAttribute,
                                                             _CmpElements,
                                                             _CmpGeometry,
                                                             _CmpParameters,
                                                             _CmpSettings);

            // データテーブル - エクセル
            _EntDtExcel = new RvtExtApp.Entities.DtExcel(_CmpAttribute,
                                                         _CmpElements,
                                                         _CmpGeometry,
                                                         _CmpParameters,
                                                         _CmpSettings);

            trans.Commit();
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>部屋の面積を取得</summary>
        ///
        /// <param name="rooms"     >部屋</param>
        /// <param name="legalAreas">法定面積リスト</param>
        /// <param name="rvtAreas"  >Revit面積リスト</param>
        ///
        /// <returns><p>法定面積取得の結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        /// <p>2021/11/24 Created Modified Applied Technology</p><history>
        /// ================================================================================
        private
        bool GetRoomArea(Collections.Generic.IList<Revit.DB.Architecture.Room> rooms,
                         ref Collections.Generic.IList<String> legalAreas,
                         ref Collections.Generic.IList<String> rvtAreas)
        {
            bool ret = true;
            double unitCoeM2 = _CmpGeometry.UnitCoeM2;

            foreach (Revit.DB.Architecture.Room room in rooms)
            {
                // Revitの面積と計算面積
                RvtExtApp.Entities.SpRoom entSpRoom = _EntDtRoom.EntSpRoom;
                entSpRoom.CurrentElem = room;

                double roomRvtArea = entSpRoom.RoomArea * unitCoeM2;
                string roomRvtAreaStr = _CmpSettings.Round(roomRvtArea).ToString();
                //string roomRvtAreaStr = roomRvtArea.ToString();

                double roomLglArea = entSpRoom.LegalArea * unitCoeM2;
                string roomLglAreaStr = _CmpSettings.Round(roomLglArea).ToString();
                //string roomLglAreaStr = roomLglArea.ToString();

                legalAreas.Add(roomLglAreaStr);
                rvtAreas.Add(roomRvtAreaStr);
                if (roomLglArea == 0.0)
                {
                    ret = false;
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>部屋の高さを取得</summary>
        ///
        /// <param name="rooms"   >部屋</param>
        /// <param name="heights" >高さ</param>
        ///
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        /// <p>2021/11/24 Created Modified Applied Technology</p><history>
        /// ================================================================================
        private
        void GetRoomHeight(Collections.Generic.IList<Revit.DB.Architecture.Room> rooms,
                           ref Collections.Generic.IList<String> heights)
        {
            double unitCoeM2 = _CmpGeometry.UnitCoeM2;
            double unitCoeM3 = _CmpGeometry.UnitCoeM3;

            Revit.DB.SpatialElementBoundaryLocation roomBndLocTypeCurrent = _CmpSettings.GetRoomAreaComputation();
            Revit.DB.SpatialElementBoundaryLocation roomBndLocType = Revit.DB.SpatialElementBoundaryLocation.Finish;
            _CmpSettings.SetRoomAreaComputation(roomBndLocType);

            bool roomVolCalcOptCurrent = _CmpSettings.GetRoomVolumeComputation();
            _CmpSettings.SetRoomVolumeComputation(true);

            foreach (Revit.DB.Architecture.Room room in rooms)
            {
                double roomArea = room.Area * unitCoeM2;
                double roomVol = room.Volume * unitCoeM3;
                double roomHeight = 0.0;
                if (roomArea != 0.0)
                {
                    roomHeight = (roomVol / roomArea) * 1000.0;
                }
                string roomHeightStr = _CmpSettings.Round(roomHeight).ToString();
                heights.Add(roomHeightStr);
            }

            _CmpSettings.SetRoomAreaComputation(roomBndLocTypeCurrent);
            _CmpSettings.SetRoomVolumeComputation(roomVolCalcOptCurrent);
        }

        /// ================================================================================
        /// <summary>ファミリインスタンスとカーブの距離を取得</summary>
        ///
        /// <param name="familyInstance">ファミリインスタンス</param>
        /// <param name="curves"        >カーブリスト</param>
        /// <param name="normalDist"    >法線の長さ</param>
        ///
        /// <returns>距離</returns>
        ///
        /// <history>2011/07/29 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        double GetDistFamilyInstanceAndCurves(Revit.DB.FamilyInstance familyInstance,
                                              Collections.Generic.IList<Revit.DB.Curve> curves,
                                              double normalDist)
        {
            // 戻り値
            double dist = 0.0;

            // カーブのZ値
            double curveZ = 0.0;
            if (curves.Count > 0)
            {
                curveZ = curves[0].GetEndPoint(0).Z;
            }

            // ファミリ位置
            Revit.DB.Transform instanceTransform = _CmpGeometry.GetElemTransform(familyInstance);
            Revit.DB.XYZ instancePos = new Revit.DB.XYZ(instanceTransform.Origin.X,
                                                        instanceTransform.Origin.Y,
                                                        curveZ);

            // フリップ対応
            double normalFlag = 1.0;
            if (_CmpGeometry.Distance2D(instanceTransform.BasisY, familyInstance.FacingOrientation) > _CmpGeometry.Approx0Len)
            {
                normalFlag = -1.0;
            }

            // 法線方向
            double posX = 0.0;
            double posY = normalDist * normalFlag;
            double posZ = 0.0;
            Revit.DB.XYZ basisX = new Revit.DB.XYZ(posX * instanceTransform.BasisX.X, posX * instanceTransform.BasisX.Y, posX * instanceTransform.BasisX.Z);
            Revit.DB.XYZ basisY = new Revit.DB.XYZ(posY * instanceTransform.BasisY.X, posY * instanceTransform.BasisY.Y, posY * instanceTransform.BasisY.Z);
            Revit.DB.XYZ basisZ = new Revit.DB.XYZ(posZ * instanceTransform.BasisZ.X, posZ * instanceTransform.BasisZ.Y, posZ * instanceTransform.BasisZ.Z);

            posX = basisX.X + basisY.X + basisZ.X;
            posY = basisX.Y + basisY.Y + basisZ.Y;
            posZ = basisX.Z + basisY.Z + basisZ.Z;
            Revit.DB.XYZ normalPos = new Revit.DB.XYZ(instanceTransform.Origin.X + posX,
                                                      instanceTransform.Origin.Y + posY,
                                                      curveZ);

            // 法線
            Revit.DB.Line normLine = Revit.DB.Line.CreateBound(instancePos, normalPos);

            // 敷地境界線との交点
            foreach (Revit.DB.Curve curve in curves)
            {
                Revit.DB.XYZ interPos = null;
                _CmpGeometry.IntersecCurve2D(normLine, curve, ref interPos);
                if (interPos != null)
                {
                    dist = _CmpGeometry.Distance2D(instancePos, interPos);
                    break;
                }
            }
            return dist;
        }

        /// ================================================================================
        /// <summary>建具と敷地境界線の距離を取得</summary>
        ///
        /// <param name="elemParts">建具</param>
        /// <param name="distParts">建具と敷地境界線の距離</param>
        ///
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        /// <p>2021/11/24 Created Modified Applied Technology</p><history>
        /// ================================================================================
        private
        void GetDistPropertyLineAndParts(Collections.Generic.IList<Revit.DB.FamilyInstance> elemparts,
                                         ref Collections.Generic.IList<string> distParts)
        {
            double unitCoe = _CmpGeometry.UnitCoe;

            // 敷地境界線
            Collections.Generic.IList<Revit.DB.PropertyLine> propLines = _CmpElements.PropertyLines;
            Collections.Generic.IList<Revit.DB.Curve> propCurves = new Collections.Generic.List<Revit.DB.Curve>();
            foreach (Revit.DB.PropertyLine propLine in propLines)
            {
                propCurves = _CmpGeometry.GetCurveElem(propLine);
            }

            Revit.DB.BoundingBoxXYZ propBBoxXYZ = null;
            double propCurvesDist = 0.0;
            if (propCurves.Count > 0)
            {
                // 敷地境界線境界の範囲
                _CmpGeometry.GetCurvesBound(propCurves, ref propBBoxXYZ);
                propCurvesDist = _CmpGeometry.Distance(propBBoxXYZ.Min, propBBoxXYZ.Max);
            }

            // 建具と敷地境界線の距離
            foreach (Revit.DB.FamilyInstance familyInstance in elemparts)
            {
                double dist = 0.0;

                if (propCurves.Count > 0)
                {
                    dist = GetDistFamilyInstanceAndCurves(familyInstance, propCurves, propCurvesDist);
                }

                if (dist == 0.0)
                {
                    distParts.Add("0.0");
                }
                else
                {
                    distParts.Add(UtilValue.Rounding(dist * unitCoe, 5, 2));
                    //distParts.Add((dist * unitCoe).ToString());
                }
            }
        }

        /// ================================================================================
        /// <summary>建具の中心高さを取得</summary>
        ///
        /// <param name="parts">建具</param>
        ///
        /// <returns>建具の中心高さ</returns>
        ///
        /// <history>2011/07/29 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        double GetCenterHeightOfParts(Revit.DB.FamilyInstance parts)
        {
            // 戻り値
            double heightCenter = 0.0;

            RvtExtApp.Entities.SpWinDoor entSpWinDoor = _EntDtWinDoor.EntSpWinDoor;
            RvtExtApp.Entities.SpWinDoorType entSpWinDoorType = _EntDtWinDoor.EntSpWinDoorType;

            // ファミリインスタンスのレベル高さ
            double heightLevel = 0.0;
            _CmpElements.GetElementLevelElevation(parts, ref heightLevel);

            entSpWinDoor.CurrentElem = parts;

            // ファミリインスタンスの敷居高さ
            double heightSill = entSpWinDoor.SillHeight;

            // ファミリシンボル
            Revit.DB.FamilySymbol partsSymbol = parts.Symbol;
            entSpWinDoorType.CurrentElem = partsSymbol;

            // ファミリシンボルの高さ
            double heightParts = entSpWinDoorType.Height;

            // 建具中心高さ
            heightCenter = heightLevel + heightSill + (heightParts * 0.5);

            return heightCenter;
        }

        /// ================================================================================
        /// <summary>建具と最高高さレベルの距離を取得</summary>
        ///
        /// <param name="elemParts">建具</param>
        /// <param name="distParts">建具と最高高さレベルの距離</param>
        ///
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        /// <p>2021/11/24 Created Modified Applied Technology</p><history>
        /// ================================================================================
        private
        void GetDistHighestLevelAndParts(Collections.Generic.IList<Revit.DB.FamilyInstance> elemparts,
                                         ref Collections.Generic.IList<string> distParts)
        {
            double unitCoe = _CmpGeometry.UnitCoe;

            // 最高高さレベル
            Collections.Generic.IList<Revit.DB.Level> levels = _CmpElements.Levels;
            double levelHHight = 0.0;
            foreach (Revit.DB.Level level in levels)
            {
                double levelHight = level.Elevation;
                if (levelHHight < levelHight)
                {
                    levelHHight = levelHight;
                }
            }

            // 建具中心と最高高さレベルの距離
            foreach (Revit.DB.FamilyInstance familyInstance in elemparts)
            {
                double dist = 0.0;

                // 建具の中心高さ
                double heightCenter = GetCenterHeightOfParts(familyInstance);

                // 高さ距離
                dist = levelHHight - heightCenter;

                if (dist > 0.0)
                {
                    distParts.Add(UtilValue.Rounding(dist * unitCoe, 5, 2));
                    //distParts.Add((dist * unitCoe).ToString());
                }
                else
                {
                    distParts.Add("0.0");
                }
            }
        }

        /// ================================================================================
        /// <summary>建具の所属部屋取得</summary>
        ///
        /// <param name="elemparts"           >建具</param>
        /// <param name="affiliationRoomParts">建具の所属部屋</param>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void GetAffiliationRoomParts(Collections.Generic.IList<Revit.DB.FamilyInstance> elemparts,
                                     ref Collections.Generic.IList<int> affiliationRoomParts)
        {
            RvtExtApp.Entities.SpRoom entSpRoom = _EntDtRoom.EntSpRoom;
            RvtExtApp.Entities.SpWinDoor entSpWinDoor = _EntDtWinDoor.EntSpWinDoor;

            foreach (Revit.DB.FamilyInstance familyInstance in elemparts)
            {
                entSpWinDoor.CurrentElem = familyInstance;

                // FromRoom
                int roomIDFrom = _CmpElements.GetIdFromRoom(familyInstance);
                Revit.DB.Architecture.Room elemRoomFrom = _CmpElements.GetRoom(roomIDFrom);
                entSpRoom.CurrentElem = elemRoomFrom;
                bool outDoorsFrom = entSpRoom.OutDoors;

                // ToRoom
                int roomIDTo = _CmpElements.GetIdToRoom(familyInstance);
                Revit.DB.Architecture.Room elemRoomTo = _CmpElements.GetRoom(roomIDTo);
                entSpRoom.CurrentElem = elemRoomTo;
                bool outDoorsTo = entSpRoom.OutDoors;

                // ID
                int roomIDAffiliation = -1;

                // ２部屋
                if ((roomIDFrom > -1) && (roomIDTo > -1))
                {
                    // FromRoom
                    if ((outDoorsFrom == false) && (outDoorsTo == true))
                    {
                        roomIDAffiliation = roomIDFrom;
                    }
                    // ToRoom
                    else if ((outDoorsFrom == true) && (outDoorsTo == false))
                    {
                        roomIDAffiliation = roomIDTo;
                    }
                }
                // FromRoom
                else if ((roomIDFrom > -1) && (roomIDTo == -1))
                {
                    roomIDAffiliation = roomIDFrom;
                }
                // ToRoom
                else if ((roomIDFrom == -1) && (roomIDTo > -1))
                {
                    roomIDAffiliation = roomIDTo;
                }

                affiliationRoomParts.Add(roomIDAffiliation);
            }
        }

        /// ================================================================================
        /// <summary>Excelデータ設定</summary>
        ///
        /// <param name="utilExcel"       >Excelユーティリティ</param>
        /// <param name="entDtExcel"      >データテーブル - 設定</param>
        /// <param name="chkCreateHeader" >見出し作成チェック</param>
        ///
        /// <history><p>2009/08/24 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        private
        void SetExcelData(UtilExcel utilExcel,
                          RvtExtApp.Entities.DtExcel entDtExcel,
                          bool chkCreateHeader)
        {
            // Excelアプリケーション
            if (utilExcel.ExistXlsApp == false)
            {
                return;
            }

            // Excelワークブック
            if (utilExcel.FlagNewXlsApp == false)
            {
                if (utilExcel.SetExcelActiveWorkbook() == false)
                {
                    utilExcel.SetExcelWorkbook(null);
                }
            }
            else
            {
                utilExcel.SetExcelWorkbook(null);
            }

            if (utilExcel.FlagNewXlsbook == false)
            {
                if (System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_EXCELUPDATE"),
                                                         "",
                                                         System.Windows.Forms.MessageBoxButtons.YesNo) ==
                                                         System.Windows.Forms.DialogResult.No)
                {
                    utilExcel.SetExcelWorkbook(null);
                }
            }

            // Excelワークシート
            utilExcel.SetExcelWorksheet(null);
            utilExcel.SetExcelVisible(true);

            int rowNo = 0;
            int colNo = 0;

            // Excle選択範囲
            int rowStart = 0;
            int colStart = 0;
            int rowStartExcel = utilExcel.SelectionRowNo;
            int colStartExcel = utilExcel.SelectionColumnNo;
            int rowCountExcel = 0;
            int colCountExcel = 0;
            if (rowStartExcel == 0)
            {
                rowStartExcel = 1;
            }

            if (colStartExcel == 0)
            {
                colStartExcel = 1;
            }

            if (entDtExcel.Data != null)
            {
                int rowsCount = entDtExcel.Data.Rows.Count;
                int colsCount = entDtExcel.Data.Columns.Count;

                if (chkCreateHeader == false)
                {
                    rowStart = 2;
                }

                if ((rowsCount > rowStart) && (colsCount > 0))
                {
                    rowCountExcel = rowStartExcel - 1;
                    for (int i = rowStart; i < rowsCount; ++i)
                    {
                        rowCountExcel++;
                        colCountExcel = colStartExcel - 1;
                        for (int j = colStart; j < colsCount; ++j)
                        {
                            colCountExcel++;
                            utilExcel.SetCellValue(rowCountExcel, colCountExcel, entDtExcel.Data.Rows[i][j]);
                        }
                    }

                    // 列の整列、幅設定
                    colCountExcel = colStartExcel - 1;
                    for (int i = colStart; i < colsCount; ++i)
                    {
                        colCountExcel++;
                        utilExcel.SetAlignmentHorizontalCell(0, colCountExcel, entDtExcel.ColAlignmentAry[i]);
                        utilExcel.SetWidthCells(0, colCountExcel, entDtExcel.ColWidthAry[i]);

                        // 表示形式
                        if (entDtExcel.ColAlignmentAry[i] == 2)
                        {
                            utilExcel.SetNumberFormatCells(0, colCountExcel, "0.000");
                        }
                    }

                    // 行罫線設定
                    if ((entDtExcel.RowBordersAryAry != null) && (entDtExcel.RowBordersAryAry.Count > 0))
                    {
                        for (int i = 0; i < entDtExcel.RowBordersAryAry.Count; ++i)
                        {
                            rowNo = rowStartExcel + entDtExcel.RowBordersAryAry[i][0] - 1;
                            colNo = colStartExcel + entDtExcel.RowBordersAryAry[i][1] - 1;

                            if (chkCreateHeader == false)
                            {
                                rowNo -= 2;
                            }
                            if (rowNo > 0)
                            {
                                utilExcel.SetBordersCells(rowNo, colNo, 0, colStartExcel + colsCount - 1, false, 0, 1);
                            }
                        }
                    }
                    rowNo = rowStartExcel + rowsCount - 1;
                    colNo = colStartExcel;
                    if (chkCreateHeader == false)
                    {
                        rowNo -= 2;
                    }
                    if (rowNo > 0)
                    {
                        utilExcel.SetBordersCells(rowNo, colNo, 0, colNo + colsCount - 1, false, 0, 1);
                    }

                    // 見出し設定
                    if (chkCreateHeader == true)
                    {
                        // 整列設定
                        utilExcel.SetAlignmentHorizontalCell(rowStartExcel, 0, 1);
                        utilExcel.SetAlignmentHorizontalCell(rowStartExcel + 1, 0, 1);

                        // 罫線設定
                        utilExcel.SetBordersCells(rowStartExcel + 1, colStartExcel, 0, colStartExcel + colsCount - 1, false, 1, 1);
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>ワークフロー</summary>
        ///
        /// <param name="commandKind" ><p>コマンド種類</p>
        ///                               <p>0=採光</p>
        ///                               <p>1=排煙</p>
        ///                               <p>2=換気</p> </param>
        ///
        /// <returns><p>エラー文字列</p>
        ///             <p>空白時はエラーなし</p></returns>
        ///
        /// <history><p>2011/07/30 Created  GSA,Inc. Shinichi Ishii</p>
        ///           <p>2015/09/11 Modified GSA,Inc. Ryo Kuroda</p>
        ///           <p>2021/11/24 Modified Applied Technology</p></history>
        /// ================================================================================
        public
        string WorkFlow(int commandKind)
        {
            string ret = null;
            System.Windows.Forms.DialogResult formResult;

            // Excelユーティリティ
            // 既存
            UtilExcel utilExcel = new UtilExcel(1);
            if (utilExcel.ExistXlsApp == false)
            {
                // 新規
                utilExcel = new UtilExcel(0);

                if (utilExcel.ExistXlsApp == false)
                {
                    ret = _CmpAttribute.ResourceText("IDS_ERR_INSTALLEXCEL");
                    _CmpParameters.SetSharedParamDefault();
                    return ret;
                }
            }

            String cmdName = _CmpAttribute.ResourceText("IDS_TXT_LIGHTING_SMOKE_VENTILATION");
            if (commandKind == 0) cmdName = _CmpAttribute.ResourceText("IDS_TXT_LIGHTING");
            else if (commandKind == 1) cmdName = _CmpAttribute.ResourceText("IDS_TXT_SMOKE");
            else if (commandKind == 2) cmdName = _CmpAttribute.ResourceText("IDS_TXT_VENTILATION");
            // トランザクショングループ
            Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(_CmpElements.RvtDBDoc);
            // スタート
            transGroup.Start(cmdName);

            Revit.DB.Transaction trans = new Autodesk.Revit.DB.Transaction(_CmpElements.RvtDBDoc);

            try
            {
                // 選択セットチェック[部屋]
                Collections.Generic.IList<Revit.DB.Architecture.Room> selSetRooms = _CmpElements.SelSetRooms;
                if (selSetRooms.Count == 0)
                {
                    ret = _CmpAttribute.ResourceText("IDS_ERR_SELROOM");
                    
                    _CmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    utilExcel.CloseExcel();
                    return ret;
                }

                // 選択セットチェック[建具]
                Collections.Generic.IList<Revit.DB.FamilyInstance> selSetWinDoors = _CmpElements.SelSetWinDoor;
                if (selSetWinDoors.Count == 0)
                {
                    ret = _CmpAttribute.ResourceText("IDS_ERR_SELPARTS");
                    
                    _CmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    utilExcel.CloseExcel();
                    return ret;
                }

                trans.Start("SetCommand");

                // 要素 - プロジェクト情報
                Revit.DB.ProjectInfo elemProjInfo = _CmpElements.ProjectInfo;

                // コマンドデータ
                RvtExtApp.Entities.DtCmd entDtCmd =
                    new RvtExtApp.Entities.DtCmd(_CmpAttribute,
                                                 _CmpElements,
                                                 _CmpGeometry,
                                                 _CmpParameters,
                                                 _CmpSettings,
                                                 elemProjInfo,
                                                 _CmpAttribute.ResourceText("IDS_SHPARAM_DEF_CMD"),
                                                 30);
                if (entDtCmd.ErrMsg != "")
                {
                    System.Windows.Forms.MessageBox.Show(entDtCmd.ErrMsg);

                    trans.RollBack();
                    _CmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return ret;
                }

                trans.Commit();

                // 法定面積
                Collections.Generic.IList<string> roomRvtAreas = new Collections.Generic.List<string>();
                Collections.Generic.IList<string> roomLglAreas = new Collections.Generic.List<string>();
                Collections.Generic.IList<string> roomAreas;
                if (GetRoomArea(selSetRooms, ref roomLglAreas, ref roomRvtAreas) == false)
                {
                    if (System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_ROOMNOLEGALAREA"),
                                                             "",
                                                             System.Windows.Forms.MessageBoxButtons.YesNo) ==
                        System.Windows.Forms.DialogResult.No)
                    {
                        
                        _CmpParameters.SetSharedParamDefault();
                        // トランザクションを統合
                        transGroup.Assimilate();
                        utilExcel.CloseExcel();
                        return ret;
                    }
                    roomAreas = roomRvtAreas;
                }
                else
                {
                    roomAreas = roomLglAreas;
                }

                // 天井高
                Collections.Generic.IList<string> roomHeight = new Collections.Generic.List<string>();
                if (commandKind == 1)
                {
                    GetRoomHeight(selSetRooms, ref roomHeight);
                }

                // 水平距離
                Collections.Generic.IList<string> distHorizontal = new Collections.Generic.List<string>();
                if (commandKind == 0)
                {
                    GetDistPropertyLineAndParts(selSetWinDoors, ref distHorizontal);
                }

                // 垂直距離
                Collections.Generic.IList<string> distVertical = new Collections.Generic.List<string>();
                if (commandKind == 0)
                {
                    GetDistHighestLevelAndParts(selSetWinDoors, ref distVertical);
                }

                // 建具の所属部屋
                Collections.Generic.IList<int> affiliationRoomParts = new Collections.Generic.List<int>();
                GetAffiliationRoomParts(selSetWinDoors, ref affiliationRoomParts);

                RvtExtApp.Components.FormSetting formSetting = new RvtExtApp.Components.FormSetting(commandKind,
                                                                                                    _CmpAttribute,
                                                                                                    entDtCmd);
                formSetting.ShowDialog();
                if (formSetting.DialogResult != System.Windows.Forms.DialogResult.OK)
                {
                    _CmpParameters.SetSharedParamDefault();
                    // トランザクションを統合
                    transGroup.Assimilate();
                    return ret;
                }

                // 「OK」ボタンを押下後一旦設定情報の保存
                trans.Start("SetParamValue");
                // コマンドデータ設定
                entDtCmd.SetData();
                trans.Commit();

                // テーブルデータ取得
                _EntDtRoom.CommandKind = commandKind;
                _EntDtRoom.Rooms = selSetRooms;
                _EntDtRoom.RoomAreas = roomAreas;
                _EntDtRoom.RoomHeights = roomHeight;
                _EntDtRoom.GetData(entDtCmd);

                _EntDtWinDoor.CommandKind = commandKind;
                _EntDtWinDoor.WinDoors = selSetWinDoors;
                _EntDtWinDoor.WinDoorDistHoriAry = distHorizontal;
                _EntDtWinDoor.WinDoorDistVertAry = distVertical;
                _EntDtWinDoor.WinDoorAffRoomAry = affiliationRoomParts;
                _EntDtWinDoor.GetData(entDtCmd, entDtCmd.CvUseDistrictOpt, _EntDtRoom.Data);

                // 画面表示
                bool flagExcel = false;
                RvtExtApp.Components.FormEnvironmentalCheck form = new RvtExtApp.Components.FormEnvironmentalCheck(_CmpAttribute,
                                                                                                                   _EntDtRoom,
                                                                                                                   _EntDtWinDoor,
                                                                                                                   entDtCmd);
                formResult = System.Windows.Forms.DialogResult.Ignore;

                while (formResult == System.Windows.Forms.DialogResult.Ignore)
                {
                    formResult = form.ShowDialog();

                    if (formResult != System.Windows.Forms.DialogResult.Cancel)
                    {
                        if (formResult != System.Windows.Forms.DialogResult.Ignore)
                        {
                            trans.Start("SetParamValue");

                            _EntDtExcel.CommandKind = commandKind;
                            _EntDtRoom.SetData();
                            _EntDtWinDoor.SetData();

                            trans.Commit();

                            if (formResult == System.Windows.Forms.DialogResult.Yes)
                            {
                                trans.Start("SetValueExcel");
                                _EntDtExcel.SetData(_EntDtRoom, _EntDtWinDoor);

                                // Excelデータ設定
                                SetExcelData(utilExcel, _EntDtExcel, entDtCmd.CvChkCreateHeader);
                                flagExcel = true;
                                trans.Commit();
                            }
                        }
                        else
                        {
                            // 図面から建具を選択
                            _EntDtWinDoor.SelectWinDoorFromDraw();
                        }
                    }
                }

                if (flagExcel == false)
                {
                    if (utilExcel.FlagNewXlsApp == true)
                    {
                        utilExcel.CloseExcel();
                    }
                }
            }

            catch (System.Exception ex)
            {
                ret = _CmpAttribute.ResourceText("IDS_ERR_COMMAND")
                    + System.Environment.NewLine + System.Environment.NewLine
                    + ex.GetType().Name + ": " + ex.Message
                    + System.Environment.NewLine + ex.StackTrace;
                if (utilExcel != null)
                {
                    utilExcel.CloseExcel();
                }

                if (trans.GetStatus() != Revit.DB.TransactionStatus.Committed)
                {
                    trans.RollBack();
                }
            }

            // トランザクションを統合
            transGroup.Assimilate();
            _CmpParameters.SetSharedParamDefault();
            return ret;
        }

        #endregion Member Functions

        // プロパティ
    }
}