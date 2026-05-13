using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule;
using ADSK.JExtRAC.AreaSchedule.Utils;

namespace ADSK.JExtRAC.AreaSchedule.Components
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

        /// <summary>共有パラメータ - エリア</summary>
        private RvtExtApp.Entities.SpArea _EntSpArea;

        /// <summary>共有パラメータ - 部屋</summary>
        private RvtExtApp.Entities.SpRoom _EntSpRoom;

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
        /// <param name="entSpArea"     >共有パラメータ - エリア</param>
        /// <param name="entSpRoom"     >共有パラメータ - 部屋</param>
        ///
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Service(RvtExtApp.Components.Attribute cmpAttribute,
                       RvtExtApp.Components.Elements cmpElements,
                       RvtExtApp.Components.Geometry cmpGeometry,
                       RvtExtApp.Components.Parameters cmpParameters,
                       RvtExtApp.Components.Settings cmpSettings,
                       RvtExtApp.Entities.SpArea entSpArea,
                       RvtExtApp.Entities.SpRoom entSpRoom)
        {
            // 初期化
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _EntSpArea = entSpArea;
            _EntSpRoom = entSpRoom;
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>長さの単位変換</summary>
        ///
        /// <param name="value"           >値</param>
        /// <param name="unitLen"         ><p>長さの単位</p>
        ///                                   <p>0=mm</p>
        ///                                   <p>1=m </p></param>
        /// <param name="decimalPointLen" >長さの小数点位置</param>
        /// <param name="fractionTypeLen" ><p>長さの端数タイプ</p>
        ///                                   <p>0=切り捨て</p>
        ///                                   <p>1=切り上げ</p>
        ///                                   <p>2=四捨五入</p></param>
        /// <param name="valueRound"      >値(丸め処理)</param>
        ///
        /// <returns>結果</returns>
        ///
        /// <history>2011/08/26 Created GSA,Inc. Shinichi Ishii</P>
        ///     <P>2021/11/24 Modified Applied Technology</p></history>
        /// ================================================================================
        private
        string ConvertLengthUnit(double value, int unitLen, int decimalPointLen, int fractionTypeLen, ref double valueRound)
        {
            double unitCoe = _CmpGeometry.UnitCoe;
            double unitCoeTh = _CmpGeometry.UnitCoeTh;
            double dValue = value;
            string sValue = "";

            if (unitLen == 0)
            {
                dValue = _CmpSettings.Round(value * unitCoe);
            }
            else if (unitLen == 1)
            {
                dValue = _CmpSettings.Round(value * unitCoeTh);
            }
            sValue = UtilValue.Rounding(dValue, decimalPointLen, fractionTypeLen);

            valueRound = double.Parse(sValue); ;
            if (unitLen == 0)
            {
                valueRound *= 0.001;
            }

            return sValue;
        }

        /// ================================================================================
        /// <summary>部屋名をエリアに登録</summary>
        ///
        /// <param name="area">エリア</param>
        /// <param name="room">部屋</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool SetAreaRoomName(Revit.DB.Area area, Revit.DB.Architecture.Room room)
        {
            bool ret = true;
            if (room != null)
            {
                // 部屋 パラメータ取得
                _EntSpRoom.CurrentElem = room;
                string roomName = _EntSpRoom.RoomName;
                string roomNumber = _EntSpRoom.RoomNumber;

                // エリア パラメータ設定
                _EntSpArea.CurrentElem = area;
                _EntSpArea.RoomName = roomName;
                _EntSpArea.RoomNo = roomNumber;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>部屋の境界線配置タイプをチェック</summary>
        ///
        /// <param name="type">境界線配置タイプ</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 処理続行</p>
        ///             <p>False = 処理中止</p></returns>
        ///
        /// <history>2011/07/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool CheckRoomBndLocType(Revit.DB.SpatialElementBoundaryLocation type)
        {
            bool ret = true;
            System.Windows.Forms.DialogResult formResult;

            if (type == Revit.DB.SpatialElementBoundaryLocation.CoreBoundary)
            {
                formResult = System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_AREAOPTWALLCORELAYE"),
                                                                  _CmpAttribute.ResourceText("IDS_TXT_AREACALC"),
                                                                  System.Windows.Forms.MessageBoxButtons.YesNo,
                                                                  System.Windows.Forms.MessageBoxIcon.Warning);
                if (formResult == System.Windows.Forms.DialogResult.No)
                {
                    ret = false;
                }
            }
            else if (type == Revit.DB.SpatialElementBoundaryLocation.Finish)
            {
                formResult = System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_AREAOPTWALLFINISH"),
                                                                  _CmpAttribute.ResourceText("IDS_TXT_AREACALC"),
                                                                  System.Windows.Forms.MessageBoxButtons.YesNo,
                                                                  System.Windows.Forms.MessageBoxIcon.Warning);
                if (formResult == System.Windows.Forms.DialogResult.No)
                {
                    ret = false;
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>部屋の境界線配置タイプ設定</summary>
        ///
        /// <param name="type">境界線配置タイプ</param>
        ///
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void SetRoomBndLocType(Revit.DB.SpatialElementBoundaryLocation type)
        {
            _CmpSettings.SetRoomAreaComputation(type);
        }

        /// ================================================================================
        /// <summary>部屋の境界線からエリアの境界線を作成</summary>
        ///
        /// <param name="rooms"             >部屋</param>
        /// <param name="viewPlan"          >平面図ビュー</param>
        /// <param name="progressBarThread" >プログレスバースレッド</param>
        /// <param name="trans"             >トランザクション</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/07/26 Created  GSA,Inc. Shinichi Ishii</p>
        ///           <p>2015/09/10 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        bool CreateAreaBndByRoomBnd(Collections.Generic.IList<Revit.DB.Architecture.Room> rooms,
                                    Revit.DB.ViewPlan viewPlan,
                                    ref ProgressBarThread progressBarThread,
                                    ref Revit.DB.Transaction trans)
        {
            bool ret = false;

            if (rooms == null)
            {
                return ret;
            }

            // スケッチ面作成
            trans.Start("SketchPlane");
            Revit.DB.SketchPlane sketchPlane = _CmpElements.CreateSketchPlaneProjOrigin();
            trans.Commit();

            // 部屋境界取得
            int cntProgress = 0;
            progressBarThread.SetData(rooms.Count, cntProgress);

            // 部屋境界位置タイプ
            Revit.DB.SpatialElementBoundaryLocation bndLocType = _CmpSettings.GetRoomAreaComputation();

            // 部屋
            foreach (Revit.DB.Architecture.Room room in rooms)
            {
                if (room != null)
                {
                    // 部屋の境界線
                    Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> roomCrvAryAry =
                        new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Curve>>();
                    _CmpGeometry.GetRoomCurves(room, bndLocType, ref roomCrvAryAry);
                    if (roomCrvAryAry != null)
                    {
                        foreach (Collections.Generic.IList<Revit.DB.Curve> crvAry in roomCrvAryAry)
                        {
                            if (crvAry != null)
                            {
                                bool flagCircle = false;

                                // エリアの境界線を作成
                                Collections.Generic.IList<Revit.DB.Curve> areaCircleCurves = _CmpGeometry.GetCircle(crvAry);
                                if (areaCircleCurves != null)
                                {
                                    if (areaCircleCurves.Count == 2)
                                    {
                                        flagCircle = true;
                                    }
                                }

                                // 円の場合
                                if (flagCircle == true)
                                {
                                    bool flagArc1 = false;
                                    bool flagArc2 = false;
                                    flagArc1 = CompareAreaCurveElems(areaCircleCurves[0], viewPlan);
                                    flagArc2 = CompareAreaCurveElems(areaCircleCurves[1], viewPlan);

                                    if (flagArc1 == false)
                                    {
                                        trans.Start("ModelCurveArc1");
                                        Revit.DB.ModelCurve modelCurve = _CmpGeometry.RvtDBDoc.Create.NewAreaBoundaryLine(sketchPlane, areaCircleCurves[0], viewPlan);
                                        trans.Commit();
                                    }
                                    if (flagArc2 == false)
                                    {
                                        trans.Start("ModelCurveArc2");
                                        Revit.DB.ModelCurve modelCurve = _CmpGeometry.RvtDBDoc.Create.NewAreaBoundaryLine(sketchPlane, areaCircleCurves[1], viewPlan);
                                        trans.Commit();
                                    }
                                }

                                // 円以外の場合
                                else
                                {
                                    foreach (Revit.DB.Curve curve in crvAry)
                                    {
                                        if (curve != null)
                                        {
                                            if (CompareAreaCurveElems(curve, viewPlan) == false)
                                            {
                                                trans.Start("ModelCurve");
                                                Revit.DB.ModelCurve modelCurve = _CmpGeometry.RvtDBDoc.Create.NewAreaBoundaryLine(sketchPlane, curve, viewPlan);
                                                trans.Commit();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                progressBarThread.SetData(++cntProgress);
            }

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>部屋からエリアを作成</summary>
        ///
        /// <param name="rooms"             >部屋リスト</param>
        /// <param name="viewPlan"          >平面図ビュー</param>
        /// <param name="areas"             >作成したエリアリスト</param>
        /// <param name="progressBarThread" >プログレスバーのスレッド</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool CreateAreaByRoom(Collections.Generic.IList<Revit.DB.Architecture.Room> rooms,
                              Revit.DB.ViewPlan viewPlan,
                              ref Collections.Generic.IList<Revit.DB.Area> areas,
                              ref ProgressBarThread progressBarThread)
        {
            bool ret = true;

            if (areas == null)
            {
                areas = new Collections.Generic.List<Revit.DB.Area>();
            }

            if (rooms != null)
            {
                int cntProgress = 0;
                progressBarThread.SetData(rooms.Count, cntProgress);

                foreach (Revit.DB.Architecture.Room room in rooms)
                {
                    // 部屋位置
                    Revit.DB.XYZ roomPos = _CmpGeometry.GetElementLocPos(room);

                    // エリア作成
                    Revit.DB.Area area = _CmpElements.CreateArea(viewPlan, roomPos);
                    if (area != null)
                    {
                        areas.Add(area);

                        // エリア パラメータ設定
                        if (SetAreaRoomName(area, room) == false)
                        {
                            ret = false;
                            break;
                        }
                    }
                    progressBarThread.SetData(++cntProgress);
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>エリアタグ作成</summary>
        ///
        /// <param name="viewPlan"          >平面図ビュー</param>
        /// <param name="areas"             >エリアリスト</param>
        /// <param name="TagNameOpt"        ><p>タグ名オプション</p>
        ///                                     <p>0=部屋名を使用</p>
        ///                                     <p>1=部屋番号を使用</p></param>
        /// <param name="tagID"             >タグ要素ID</param>
        /// <param name="progressBarThread" >プログレスバースレッド</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool CreateAreaTag(Revit.DB.ViewPlan viewPlan,
                           Collections.Generic.IList<Revit.DB.Area> areas,
                           int TagNameOpt,
                           int tagID,
                           ref ProgressBarThread progressBarThread)
        {
            bool ret = true;

            Revit.DB.FamilySymbol areaTagSymbol = _CmpElements.GetAreaTagFamilySymbol(tagID);
            if (areaTagSymbol == null)
            {
                ret = false;
                return ret;
            }

            if (areas != null)
            {
                int cntProgress = 0;
                progressBarThread.SetData(areas.Count, cntProgress);

                foreach (Revit.DB.Area area in areas)
                {
                    // エリア位置
                    Revit.DB.XYZ areaPos = _CmpGeometry.GetElementLocPos(area);

                    // エリアタグ
                    Revit.DB.AreaTag areaTag = _CmpElements.CreateAreaTag(viewPlan, area, areaPos);

                    if (areaTag != null)
                    {
                        areaTag.ChangeTypeId(areaTagSymbol.Id);
                        areaTag.HasLeader = false;
                    }

                    // エリア名
                    _EntSpArea.CurrentElem = area;
                    if (TagNameOpt == 0)
                    {
                        _EntSpArea.AreaName = _EntSpArea.RoomName;
                    }
                    else if (TagNameOpt == 1)
                    {
                        _EntSpArea.AreaName = _EntSpArea.RoomNo;
                    }
                    progressBarThread.SetData(++cntProgress);
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>エリアのパラメータ設定</summary>
        ///
        /// <param name="areas"   >エリア</param>
        /// <param name="viewPlan">平面図ビュー</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool SetAreaParameter(Collections.Generic.IList<Revit.DB.Area> areas, Revit.DB.ViewPlan viewPlan)
        {
            bool ret = true;

            // 部屋取得
            Collections.Generic.IList<Revit.DB.Architecture.Room> rooms = _CmpElements.GetElementsRoom(1, 1, viewPlan.GenLevel);

            if (areas != null)
            {
                // エリア
                foreach (Revit.DB.Area area in areas)
                {
                    // パラメーター定義
                    _EntSpArea.CurrentElem = area;
                    if (_EntSpArea.RoomName == null)
                    {
                        _EntSpArea.RoomName = "";
                    }
                    if (_EntSpArea.RoomNo == null)
                    {
                        _EntSpArea.RoomNo = "";
                    }

                    // パラメータ値確認
                    bool paramFlag1 = true;
                    bool paramFlag2 = true;
                    if (_EntSpArea.RoomName == "")
                    {
                        paramFlag1 = false;
                    }
                    if (_EntSpArea.RoomNo == "")
                    {
                        paramFlag2 = false;
                    }

                    // エリアのパラメータ設定
                    if (rooms != null)
                    {
                        if ((paramFlag1 == false) || (paramFlag2 == false))
                        {
                            Revit.DB.XYZ areaPos = _CmpGeometry.GetElementLocPos(area);
                            Revit.DB.Architecture.Room fillRoom = _CmpElements.GetRoomInPoint(rooms, areaPos);

                            if (SetAreaRoomName(area, fillRoom) == false)
                            {
                                ret = false;
                                break;
                            }
                        }
                    }
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>ビューのエリア線分要素と比較</summary>
        ///
        /// <param name="areaCurve" >線分</param>
        /// <param name="view"      >ビュー</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 重複している</p>
        ///             <p>False = 重複していない</p></returns>
        ///
        /// <history>2011/07/27 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool CompareAreaCurveElems(Revit.DB.Curve areaCurve, Revit.DB.View view)
        {
            // 戻り値
            bool ret = false;

            Collections.Generic.IList<Revit.DB.CurveElement> areasCurveElems = _CmpElements.GetAreasCurveElemsOfView(view);

            if (areasCurveElems != null)
            {
                // エリア線分要素
                foreach (Revit.DB.CurveElement curveElem in areasCurveElems)
                {
                    Revit.DB.Curve curve = curveElem.GeometryCurve as Revit.DB.Curve;
                    if (curve != null)
                    {
                        // エリア線分要素と比較
                        ret = _CmpGeometry.IsEqualCurve2D(areaCurve, curve);
                        if (ret == true)
                        {
                            break;
                        }
                    }
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>面積の根拠式作成</summary>
        ///
        /// <param name="view"              >ビュー</param>
        /// <param name="areas"             >エリア</param>
        /// <param name="unitLength"        ><p>長さの単位</p>
        ///                                     <p>0=mm</p>
        ///                                     <p>1=m</p></param>
        /// <param name="decimalPointLen"   >長さの小数点位置</param>
        /// <param name="fractionTypeLen"   ><p>長さの端数タイプ</p>
        ///                                     <p>0=切り捨て</p>
        ///                                     <p>1=切り上げ</p>
        ///                                     <p>2=四捨五入</p></param>
        /// <param name="decimalPointArea"  >面積の小数点位置</p></param>
        /// <param name="fractionTypeArea"  >面積の端数タイプ</p>
        ///                                     <p>0=切り捨て</p>
        ///                                     <p>1=切り上げ</p>
        ///                                     <p>2=四捨五入</p></param>
        /// <param name="piStr"             >PIの文字列</param>
        /// <param name="progressBarThread" >プログレスバーのスレッド</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool CreateBasisExpression(Revit.DB.View view,
                                   Collections.Generic.IList<Revit.DB.Area> areas,
                                   int unitLength,
                                   int decimalPointLen,
                                   int fractionTypeLen,
                                   int decimalPointArea,
                                   int fractionTypeArea,
                                   string piStr,
                                   ref ProgressBarThread progressBarThread)
        {
            bool ret = true;

            if (areas != null)
            {
                // 線種取得
                Revit.DB.Element lineStyle = _CmpElements.LineStyleHidden;

                // 面積の単位をm2に設定
                _CmpSettings.SetUnitAreaM2(decimalPointArea - 1);

                // エリア境界取得
                Collections.Generic.IList<Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>>> areasCrvAryAry = _CmpGeometry.AreasBoundaries(areas);

                int cntProgress = 0;
                progressBarThread.SetData(areas.Count, cntProgress);

                foreach (Revit.DB.Area area in areas)
                {
                    // エリア境界
                    Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> segAryAry =
                        new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Curve>>();

                    _CmpGeometry.GetAreaCurves(area, ref segAryAry);

                    if (segAryAry != null)
                    {
                        int figureType = 0;
                        Collections.Generic.IList<Revit.DB.Curve> optimizeCurves = null;

                        if (segAryAry.Count == 1)
                        {
                            Collections.Generic.IList<Revit.DB.Curve> curves = segAryAry[0];

                            // 直線頂点最適化(直線に変換しない）
                            optimizeCurves = _CmpGeometry.OptimizeLineVertexNoConvLine(curves);

                            // 矩形チェック
                            figureType = CheckRectangle(optimizeCurves);

                            // 三角形チェック
                            if (figureType == 0)
                            {
                                figureType = CheckTriangle(optimizeCurves);
                            }

                            // 円・円弧・弓形チェック
                            if (figureType == 0)
                            {
                                figureType = CheckArc(optimizeCurves);
                            }
                            
                            // どの形状でもない場合
                            if ( figureType == 0 ) {
                                return false ;
                            }
                            
                        }

                        // 根拠式値
                        Collections.Generic.IList<string> values = new Collections.Generic.List<string>();
                        Collections.Generic.IList<Revit.DB.XYZ> valuesPos = new Collections.Generic.List<Revit.DB.XYZ>();
                        Collections.Generic.IList<Revit.DB.XYZ> valuesVec = new Collections.Generic.List<Revit.DB.XYZ>();
                        string areaExpnStr = "";
                        string areaCalcStr = "0.0";
                        Collections.Generic.IList<Revit.DB.Line> lines = new Collections.Generic.List<Revit.DB.Line>();
                        bool flaExpn = false;
                        if (figureType > 0)
                        {
                            flaExpn = GetExpn(optimizeCurves,
                                              figureType,
                                              unitLength,
                                              decimalPointLen,
                                              fractionTypeLen,
                                              decimalPointArea,
                                              fractionTypeArea,
                                              piStr,
                                              ref values,
                                              ref valuesPos,
                                              ref valuesVec,
                                              ref areaExpnStr,
                                              ref areaCalcStr,
                                              ref lines);
                        }

                        // 作図
                        if (flaExpn == true)
                        {
                            flaExpn = SetExpnElem(view, lineStyle, values, valuesPos, valuesVec, lines);
                        }

                        // パラメータ設定
                        flaExpn = SetExpnParam(area, areaExpnStr, areaCalcStr);
                    }
                    progressBarThread.SetData(++cntProgress);
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>四角形のチェック</summary>
        ///
        /// <param name="figure">図形の線分</param>
        ///
        /// <returns><p>四角形タイプ</p>
        ///             <p>10= 正方形</p>
        ///             <p>11= ひし形</p>
        ///             <p>12= 長方形</p>
        ///             <p>13= 平行四辺形</p>
        ///             <p>14= 台形</p></returns>
        ///
        /// <history>2011/07/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int CheckRectangle(Collections.Generic.IList<Revit.DB.Curve> figure)
        {
            int ret = 0;

            if (figure == null)
            {
                return ret;
            }

            if (figure.Count != 4)
            {
                return ret;
            }

            // 対角線
            Revit.DB.Line diagLine1 = Revit.DB.Line.CreateBound(figure[0].GetEndPoint(0), figure[2].GetEndPoint(0));

            Revit.DB.Line diagLine2 = Revit.DB.Line.CreateBound(figure[1].GetEndPoint(0), figure[3].GetEndPoint(0));

            // 対角線の交点
            Revit.DB.XYZ interPos = null;
            _CmpGeometry.IntersecCurve2D(diagLine1, diagLine2, ref interPos);
            if (interPos == null)
            {
                return ret;
            }

            // 1. 対角線が互いに他を２等分する
            bool flagDiagPortEq = false;
            double dist11 = _CmpGeometry.Distance2D(diagLine1.GetEndPoint(0), interPos);
            double dist12 = _CmpGeometry.Distance2D(diagLine1.GetEndPoint(1), interPos);
            double dist21 = _CmpGeometry.Distance2D(diagLine2.GetEndPoint(0), interPos);
            double dist22 = _CmpGeometry.Distance2D(diagLine2.GetEndPoint(1), interPos);
            if ((System.Math.Abs(dist11 - dist12) < _CmpGeometry.Approx0Len) &&
                (System.Math.Abs(dist21 - dist22) < _CmpGeometry.Approx0Len))
            {
                flagDiagPortEq = true;
            }

            // 2. 対角線の長さが等しい
            bool flagDiagLenEq = false;
            if (System.Math.Abs(diagLine1.Length - diagLine2.Length) < _CmpGeometry.Approx0Len)
            {
                flagDiagLenEq = true;
            }

            // 3. 対角線は垂直に交わる
            bool flagDiagPerpend = false;
            double ang = System.Math.Atan2(_CmpGeometry.CrossProduct2D(interPos, diagLine1.GetEndPoint(0), diagLine2.GetEndPoint(0)),
                                           _CmpGeometry.DotProduct2D(interPos, diagLine1.GetEndPoint(0), diagLine2.GetEndPoint(0)));
            if (System.Math.Abs(System.Math.Abs(ang) - (System.Math.PI * 0.5)) < _CmpGeometry.Approx0Ang)
            {
                flagDiagPerpend = true;
            }

            // 4. 対角線同士の比率
            bool flagDiagRatio = false;
            double rate12 = dist12 / diagLine1.Length;
            double rate21 = dist21 / diagLine2.Length;
            double rate22 = dist22 / diagLine2.Length;
            if (System.Math.Abs(rate12 - rate21) < _CmpGeometry.Approx0Len)
            {
                flagDiagRatio = true;
            }
            else if (System.Math.Abs(rate12 - rate22) < _CmpGeometry.Approx0Len)
            {
                flagDiagRatio = true;
            }

            // 図形の種類を判断
            // 10 = 正方形
            // 11 = ひし形
            // 12 = 長方形
            // 13 = 平行四辺形
            // 14 = 台形
            if (flagDiagPortEq == true)
            {
                if (flagDiagLenEq == true)
                {
                    if (flagDiagPerpend == true)
                    {
                        ret = 10;
                    }
                    else
                    {
                        ret = 12;
                    }
                }
                else
                {
                    if (flagDiagPerpend == true)
                    {
                        ret = 11;
                    }
                    else
                    {
                        ret = 13;
                    }
                }
            }
            else
            {
                if (flagDiagRatio)
                {
                    ret = 14;
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>三角形のチェック</summary>
        ///
        /// <param name="figure">図形の線分</param>
        ///
        /// <returns><p>三角形タイプ</p>
        ///             <p>20= 三角形</p></returns>
        ///
        /// <history>2011/07/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int CheckTriangle(Collections.Generic.IList<Revit.DB.Curve> figure)
        {
            int ret = 0;

            if (figure == null)
            {
                return ret;
            }

            if (figure.Count != 3)
            {
                return ret;
            }

            // 図形の種類を判断
            // 10 = 三角形
            bool flag = true;
            foreach (Revit.DB.Curve curve in figure)
            {
                if (curve.GetType() != typeof(Revit.DB.Line))
                {
                    flag = false;
                    break;
                }
            }
            if (flag == true)
            {
                ret = 20;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>円弧のチェック</summary>
        ///
        /// <param name="figure">図形の線分</param>
        ///
        /// <returns>arc type<p>円弧タイプ</p>
        ///           <p>30= 円</p>
        ///           <p>31= 円弧</p>
        ///           <p>32= 弓形</p></returns>
        ///
        /// <history>2011/07/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int CheckArc(Collections.Generic.IList<Revit.DB.Curve> figure)
        {
            int ret = 0;

            if (figure == null)
            {
                return ret;
            }

            if (figure.Count == 0)
            {
                return ret;
            }

            // 円弧・直線
            Collections.Generic.IList<Revit.DB.Line> lines = new Collections.Generic.List<Revit.DB.Line>();
            Collections.Generic.IList<Revit.DB.Arc> arcs = new Collections.Generic.List<Revit.DB.Arc>();
            foreach (Revit.DB.Curve curve in figure)
            {
                if (curve.GetType() == typeof(Revit.DB.Line))
                {
                    Revit.DB.Line line = curve as Revit.DB.Line;
                    if (line != null)
                    {
                        lines.Add(line);
                    }
                }
                else if (curve.GetType() == typeof(Revit.DB.Arc))
                {
                    Revit.DB.Arc arc = curve as Revit.DB.Arc;
                    if (arc != null)
                    {
                        arcs.Add(arc);
                    }
                }
            }

            bool isSameCen = false;
            bool isArc = false;
            bool isBow = false;
            bool getArc = false;
            Revit.DB.XYZ cenStd = null;
            Revit.DB.XYZ cenCmp = null;

            isSameCen = true;
            getArc = false;
            if (arcs.Count == 0)
            {
                return ret;
            }
            else
            {
                // 同心円チェック
                for (int i = 0; i < arcs.Count; ++i)
                {
                    cenCmp = arcs[i].Center;
                    if (getArc == true)
                    {
                        if (_CmpGeometry.Distance2D(cenStd, cenCmp) >= _CmpGeometry.Approx0Len)
                        {
                            isSameCen = false;
                        }
                    }
                    else
                    {
                        cenStd = cenCmp;
                        getArc = true;
                    }
                }
            }
            if (isSameCen == false)
            {
                return ret;
            }

            // 図形の種類を判断
            // 30 = circle　円
            if (figure.Count == arcs.Count)
            {
                ret = 30;
            }
            else
            {
                // 直線中心=円弧中心
                isArc = false;
                isBow = false;
                if (lines.Count == 1)
                {
                    cenCmp = _CmpGeometry.Center2Point(lines[0]);
                    if (_CmpGeometry.Distance2D(cenStd, cenCmp) < _CmpGeometry.Approx0Len)
                    {
                        isArc = true;
                    }
                    else
                    {
                        double angle = _CmpGeometry.GetArcAngle(arcs);
                        if (System.Math.Abs(angle) < System.Math.PI)
                        {
                            isBow = true;
                        }
                    }
                }
                else if (lines.Count == 2)
                {
                    cenCmp = null;
                    _CmpGeometry.IntersecCurve2D(lines[0], lines[1], ref cenCmp);
                    if (cenCmp != null)
                    {
                        if (_CmpGeometry.Distance2D(cenStd, cenCmp) < _CmpGeometry.Approx0Len)
                        {
                            isArc = true;
                        }
                    }
                }

                // 図形の種類を判断
                // 31 = 円弧
                // 32 = 弓形
                if (isArc == true)
                {
                    ret = 31;
                }
                else if (isBow)
                {
                    ret = 32;
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>図形の根拠式取得</summary>
        ///
        /// <param name="figure"          >図形の線分リスト</param>
        /// <param name="figureType"      ><p>図形タイプ</p>
        ///                                   <p>10= 正方形</p>
        ///                                   <p>11= ひし形</p>
        ///                                   <p>12= 長方形</p>
        ///                                   <p>13= 平行四辺形</p>
        ///                                   <p>14= 台形</p>
        ///                                   <p>20= 三角形</p>
        ///                                   <p>30= 円</p>
        ///                                   <p>31= 円弧</p>
        ///                                   <p>32= 弓形</p></param>
        /// <param name="unitLength"      ><p>長さの単位</p>
        ///                                   <p>0=mm</p>
        ///                                   <p>1=m</p></param>
        /// <param name="decimalPointLen" >長さの小数点位置</param>
        /// <param name="fractionTypeLen" ><p>長さの端数タイプ</p>
        ///                                   <p>0=切り捨て</p>
        ///                                   <p>1=切り上げ</p>
        ///                                   <p>2=四捨五入</p></param>
        /// <param name="decimalPointArea">面積の小数点位置</param>
        /// <param name="fractionTypeArea"><p>面積の端数タイプ</p>
        ///                                   <p>0=切り捨て</p>
        ///                                   <p>1=切り上げ</p>
        ///                                   <p>2=四捨五入</p></param>
        /// <param name="piStr"           >PIの文字列</param>
        /// <param name="values"          >値リスト</param>
        /// <param name="valuesPos"       >値文字の位置</param>
        /// <param name="valuesVec"       >値文字のベクトル方向</param>
        /// <param name="areaExpnStr"     >面積根拠式の文字</param>
        /// <param name="areaCalcStr"     >計算面積文字</param>
        /// <param name="lines"           >高さまたは半径の線</param>
        /// <param name="lines"           >高さまたは半径の線</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/05/25 Created  GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/11/24 Modifed Applied Techbology</p><history>
        /// ================================================================================
        public
        bool GetExpn(Collections.Generic.IList<Revit.DB.Curve> figure,
                     int figureType,
                     int unitLength,
                     int decimalPointLen,
                     int fractionTypeLen,
                     int decimalPointArea,
                     int fractionTypeArea,
                     string piStr,
                     ref Collections.Generic.IList<string> values,
                     ref Collections.Generic.IList<Revit.DB.XYZ> valuesPos,
                     ref Collections.Generic.IList<Revit.DB.XYZ> valuesVec,
                     ref string areaExpnStr,
                     ref string areaCalcStr,
                     ref Collections.Generic.IList<Revit.DB.Line> lines)
        {
            bool ret = false;

            double unitCoeM2 = _CmpGeometry.UnitCoeM2;
            double unitCoeTh = _CmpGeometry.UnitCoeTh;
            string widthStr = "";
            string heightStr = "";
            string upSideStr = "";
            string btSideStr = "";
            string radiusStr = "";
            string angleStr = "";
            double areaValue = 0.0;
            Revit.DB.XYZ midPos = null;
            Revit.DB.XYZ midPos1 = null;
            Revit.DB.XYZ midPos2 = null;
            Revit.DB.XYZ pos = null;
            Revit.DB.XYZ pos1 = null;
            Revit.DB.XYZ pos2 = null;
            Revit.DB.XYZ verticalPos = null;
            Revit.DB.Line line = null;
            Revit.DB.Line line1 = null;
            Revit.DB.Line line2 = null;
            Revit.DB.Arc arc = null;
            int index1 = 0;
            int index2 = 1;
            int index3 = 2;
            int index4 = 3;
            int disIndex1 = 0;
            int disIndex2 = 1;
            int disIndex3 = 2;
            double width = 0.0;
            double widthF = 0.0;
            double height = 0.0;
            double heightF = 0.0;
            double upSideF = 0.0;
            double upSide = 0.0;
            double btSideF = 0.0;
            double btSide = 0.0;
            double radiusF = 0.0;
            double radius = 0.0;
            double angle = 0.0;
            double length = 0.0;
            double angleDegB = 0.0;
            double angleDeg = 0.0;

            // 左下点インデックス
            index1 = 0;
            index2 = 1;
            Collections.Generic.IList<int> lbIndices = _CmpGeometry.GetLeftBottomCurveIndex(figure);
            if (lbIndices.Count != 2)
            {
                return ret;
            }
            if (lbIndices[0] > -1)
            {
                index1 = lbIndices[0];
            }
            if (lbIndices[1] > -1)
            {
                index2 = lbIndices[1];
            }

            // 正方形・長方形
            if ((figureType == 10) || (figureType == 12))
            {
                // 幅・高さ
                widthF = figure[index1].Length;
                width = widthF;
                heightF = figure[index2].Length;
                height = heightF;
                widthStr = ConvertLengthUnit(widthF, unitLength, decimalPointLen, fractionTypeLen, ref width);
                heightStr = ConvertLengthUnit(heightF, unitLength, decimalPointLen, fractionTypeLen, ref height);
                midPos1 = _CmpGeometry.Center2Point(figure[index1]);
                midPos2 = _CmpGeometry.Center2Point(figure[index2]);
                values.Add(widthStr);
                values.Add(heightStr);
                valuesPos.Add(midPos1);
                valuesPos.Add(midPos2);
                valuesVec.Add(_CmpGeometry.GetLeaning(midPos1, figure[index1].GetEndPoint(1)));
                valuesVec.Add(_CmpGeometry.GetLeaning(midPos2, figure[index2].GetEndPoint(1)));

                // 面積
                areaExpnStr = widthStr + " × " + heightStr;
                areaValue = width * height;
                areaCalcStr = UtilValue.Rounding(areaValue, decimalPointArea, fractionTypeArea);
                ret = true;
            }
            // ひし形
            else if (figureType == 11)
            {
                index3 = UtilValue.GetNextIndex(index2, figure.Count);
                index4 = UtilValue.GetNextIndex(index3, figure.Count);

                // 対角線
                line1 = Revit.DB.Line.CreateBound(figure[index1].GetEndPoint(0), figure[index3].GetEndPoint(0));
                line2 = Revit.DB.Line.CreateBound(figure[index2].GetEndPoint(0), figure[index4].GetEndPoint(0));
                _CmpGeometry.IntersecCurve2D(line1, line2, ref midPos);
                if (midPos != null)
                {
                    widthF = line1.Length;
                    width = widthF;
                    heightF = line2.Length;
                    height = heightF;
                    widthStr = ConvertLengthUnit(widthF, unitLength, decimalPointLen, fractionTypeLen, ref width);
                    heightStr = ConvertLengthUnit(heightF, unitLength, decimalPointLen, fractionTypeLen, ref height);
                    midPos1 = _CmpGeometry.Center2Point(line1.GetEndPoint(0), midPos);
                    midPos2 = _CmpGeometry.Center2Point(line2.GetEndPoint(0), midPos);
                    values.Add(widthStr);
                    values.Add(heightStr);
                    valuesPos.Add(midPos1);
                    valuesPos.Add(midPos2);
                    valuesVec.Add(_CmpGeometry.GetLeaning(midPos1, line1.GetEndPoint(1)));
                    valuesVec.Add(_CmpGeometry.GetLeaning(midPos2, line2.GetEndPoint(1)));
                    lines.Add(line1);
                    lines.Add(line2);

                    // 面積
                    areaExpnStr = widthStr + " × " + heightStr + " ÷ " + "2";
                    areaValue = width * height / 2;
                    areaCalcStr = UtilValue.Rounding(areaValue, decimalPointArea, fractionTypeArea);
                    ret = true;
                }
            }

            // 平行四辺形・台形
            else if ((figureType == 13) || (figureType == 14))
            {
                index3 = UtilValue.GetNextIndex(index2, figure.Count);
                index4 = UtilValue.GetNextIndex(index3, figure.Count);

                // 平行線
                disIndex1 = index1;
                disIndex2 = index3;
                if (_CmpGeometry.CompareParallelism(figure[disIndex1], figure[disIndex2]) == false)
                {
                    disIndex1 = index2;
                    disIndex2 = index4;
                }
                if (figure[disIndex1].Length < figure[disIndex2].Length)
                {
                    int i = disIndex1;
                    disIndex1 = disIndex2;
                    disIndex2 = i;
                }

                // 高さ
                pos1 = _CmpGeometry.Center2Point(figure[disIndex2]);
                pos2 = _CmpGeometry.GetVerticalPos2D(figure[disIndex1], pos1);
                line = Revit.DB.Line.CreateBound(pos1, pos2);
                _CmpGeometry.IntersecCurve2D(figure[disIndex1], line, ref midPos);
                if (midPos == null)
                {
                    for (int i = 0; i < 2; ++i)
                    {
                        pos1 = figure[disIndex2].GetEndPoint(i);
                        pos2 = _CmpGeometry.GetVerticalPos2D(figure[disIndex1], pos1);
                        line = Revit.DB.Line.CreateBound(pos1, pos2);
                        _CmpGeometry.IntersecCurve2D(figure[disIndex1], line, ref midPos);
                        if (midPos != null)
                        {
                            break;
                        }
                    }
                    if (midPos == null)
                    {
                        pos1 = _CmpGeometry.GetVerticalPos2D(figure[disIndex1], figure[disIndex2].GetEndPoint(1));
                        pos2 = _CmpGeometry.GetVerticalPos2D(figure[disIndex2], figure[disIndex1].GetEndPoint(1));
                        line = Revit.DB.Line.CreateBound(pos1, pos2);
                        pos = _CmpGeometry.Center2Point(line);
                        midPos = _CmpGeometry.GetVerticalPos2D(figure[disIndex1], pos);
                    }
                }

                midPos = new Revit.DB.XYZ(midPos.X, midPos.Y, figure[disIndex2].GetEndPoint(0).Z);
                verticalPos = _CmpGeometry.GetVerticalPos2D(figure[disIndex2], midPos);
                double dist = _CmpGeometry.Distance2D(midPos, verticalPos);
                heightF = dist;
                height = heightF;
                heightStr = ConvertLengthUnit(heightF, unitLength, decimalPointLen, fractionTypeLen, ref height);
                values.Add(heightStr);
                valuesPos.Add(_CmpGeometry.Center2Point(midPos, verticalPos));
                valuesVec.Add(_CmpGeometry.GetLeaning(midPos, verticalPos));
                line = Revit.DB.Line.CreateBound(midPos, verticalPos);
                lines.Add(line);

                // 上底・下底
                btSideF = figure[disIndex1].Length;
                btSide = btSideF;
                upSideF = figure[disIndex2].Length;
                upSide = upSideF;
                btSideStr = ConvertLengthUnit(btSideF, unitLength, decimalPointLen, fractionTypeLen, ref btSide);
                upSideStr = ConvertLengthUnit(upSideF, unitLength, decimalPointLen, fractionTypeLen, ref upSide);
                midPos1 = _CmpGeometry.Center2Point(figure[disIndex1]);
                midPos2 = _CmpGeometry.Center2Point(figure[disIndex2]);

                // 下底
                values.Add(btSideStr);
                valuesPos.Add(midPos1);
                valuesVec.Add(_CmpGeometry.GetLeaning(midPos1, figure[disIndex1].GetEndPoint(1)));

                // 上底(台形)
                if (figureType == 14)
                {
                    values.Add(upSideStr);
                    valuesPos.Add(midPos2);
                    valuesVec.Add(_CmpGeometry.GetLeaning(midPos2, figure[disIndex2].GetEndPoint(1)));
                }

                // 面積(平行四辺形)
                if (figureType == 13)
                {
                    areaExpnStr = btSideStr + " × " + heightStr;
                    areaValue = btSide * height;
                    areaCalcStr = UtilValue.Rounding(areaValue, decimalPointArea, fractionTypeArea);
                    ret = true;
                }
                // 面積(台形)
                else if (figureType == 14)
                {
                    areaExpnStr = "(" + upSideStr + " ＋ " + btSideStr + ")" + " × " + heightStr + " ÷ " + "2";
                    areaValue = (upSide + btSide) * height / 2;
                    areaCalcStr = UtilValue.Rounding(areaValue, decimalPointArea, fractionTypeArea);
                    ret = true;
                }
            }

            // 三角形
            else if (figureType == 20)
            {
                index3 = UtilValue.GetNextIndex(index2, figure.Count);

                // インデックス
                disIndex1 = index1;
                disIndex2 = index2;
                disIndex3 = index3;

                for (int i = 0; i < figure.Count - 1; ++i)
                {
                    pos1 = figure[disIndex3].GetEndPoint(0);
                    pos2 = _CmpGeometry.GetVerticalPos2D(figure[disIndex1], pos1);
                    line = Revit.DB.Line.CreateBound(pos1, pos2);
                    _CmpGeometry.IntersecCurve2D(figure[disIndex1], line, ref midPos);
                    if (midPos != null)
                    {
                        break;
                    }
                    disIndex1 = UtilValue.GetNextIndex(disIndex1, figure.Count);
                    disIndex2 = UtilValue.GetNextIndex(disIndex2, figure.Count);
                    disIndex3 = UtilValue.GetNextIndex(disIndex3, figure.Count);
                }
                index1 = disIndex1;
                index2 = disIndex2;
                index3 = disIndex3;

                // 底辺
                btSideF = figure[index1].Length;
                btSide = btSideF;
                btSideStr = ConvertLengthUnit(btSideF, unitLength, decimalPointLen, fractionTypeLen, ref btSide);
                midPos = _CmpGeometry.Center2Point(figure[index1]);
                values.Add(btSideStr);
                valuesPos.Add(midPos);
                valuesVec.Add(_CmpGeometry.GetLeaning(midPos, figure[index1].GetEndPoint(1)));

                // 高さ
                midPos = figure[index3].GetEndPoint(0);
                verticalPos = _CmpGeometry.GetVerticalPos2D(figure[index1], midPos);
                double dist = _CmpGeometry.Distance2D(midPos, verticalPos);
                heightF = dist;
                height = heightF;
                heightStr = ConvertLengthUnit(heightF, unitLength, decimalPointLen, fractionTypeLen, ref height);
                values.Add(heightStr);
                valuesPos.Add(_CmpGeometry.Center2Point(midPos, verticalPos));
                valuesVec.Add(_CmpGeometry.GetLeaning(midPos, verticalPos));
                line = Revit.DB.Line.CreateBound(midPos, verticalPos);
                lines.Add(line);

                // 面積
                areaExpnStr = btSideStr + " × " + heightStr + " ÷ " + "2";
                areaValue = btSide * height / 2;
                areaCalcStr = UtilValue.Rounding(areaValue, decimalPointArea, fractionTypeArea);
                ret = true;
            }

            // 円
            else if (figureType == 30)
            {
                // 円弧
                arc = figure[index1] as Revit.DB.Arc;
                line = Revit.DB.Line.CreateBound(arc.Center, arc.GetEndPoint(0));

                // 半径
                radiusF = line.Length;
                radius = radiusF;
                radiusStr = ConvertLengthUnit(radiusF, unitLength, decimalPointLen, fractionTypeLen, ref radius);
                midPos = _CmpGeometry.Center2Point(line);
                values.Add(radiusStr);
                valuesPos.Add(midPos);
                valuesVec.Add(_CmpGeometry.GetLeaning(midPos, line.GetEndPoint(1)));
                lines.Add(line);

                // 面積
                areaExpnStr = radiusStr + " × " + radiusStr + " × " + piStr;
                areaValue = radius * radius * double.Parse(piStr);
                areaCalcStr = UtilValue.Rounding(areaValue, decimalPointArea, fractionTypeArea);
                ret = true;
            }

            // 円弧
            else if (figureType == 31)
            {
                // arc
                // 円弧
                Collections.Generic.IList<Revit.DB.Arc> arcs = new Collections.Generic.List<Revit.DB.Arc>();
                for (int i = 0; i < figure.Count; ++i)
                {
                    if (figure[i].GetType() == typeof(Revit.DB.Arc))
                    {
                        arc = figure[i] as Revit.DB.Arc;
                        if (arc != null)
                        {
                            arcs.Add(arc);
                        }
                    }
                }

                // 円弧順番
                index1 = 0;
                index2 = 0;
                _CmpGeometry.GetArcOrder(arcs, ref index1, ref index2);

                // 円弧中点
                Revit.DB.XYZ center = arcs[0].Center;
                radius = arcs[0].Radius;
                length = 0.0;
                for (int i = 0; i < arcs.Count; ++i)
                {
                    length += arcs[i].Length;
                }
                pos = arcs[index2].GetEndPoint(1);
                angle = 0.0;
                midPos = _CmpGeometry.GetArcMid(pos, center, radius, length, ref angle);
                line = Revit.DB.Line.CreateBound(center, midPos);

                // 半径
                radiusF = line.Length;
                radius = radiusF;
                radiusStr = ConvertLengthUnit(radiusF, unitLength, decimalPointLen, fractionTypeLen, ref radius);
                midPos = _CmpGeometry.Center2Point(line);
                values.Add(radiusStr);
                valuesPos.Add(midPos);
                valuesVec.Add(_CmpGeometry.GetLeaning(midPos, line.GetEndPoint(1)));
                lines.Add(line);

                // 角度
                angleDegB = UtilGeom.AngleDeg(angle * 2.0);
                angleDeg = _CmpSettings.Round(angleDegB);
                angleStr = UtilValue.Rounding(angleDeg, 4, 2);

                // 面積
                areaExpnStr = radiusStr + " × " + radiusStr + " × " + piStr + " × " + angleStr + " ÷ " + "360";
                areaValue = radius * radius * double.Parse(piStr) * double.Parse(angleStr) / 360;
                areaCalcStr = UtilValue.Rounding(areaValue, decimalPointArea, fractionTypeArea);
                ret = true;
            }

            // 弓形
            else if (figureType == 32)
            {
                // 底辺・円弧
                Collections.Generic.IList<Revit.DB.Arc> arcs = new Collections.Generic.List<Revit.DB.Arc>();
                Revit.DB.Line btLine = null;
                for (int i = 0; i < figure.Count; ++i)
                {
                    if (figure[i].GetType() == typeof(Revit.DB.Arc))
                    {
                        arc = figure[i] as Revit.DB.Arc;
                        if (arc != null)
                        {
                            arcs.Add(arc);
                        }
                    }
                    else if (figure[i].GetType() == typeof(Revit.DB.Line))
                    {
                        line = figure[i] as Revit.DB.Line;
                        if (line != null)
                        {
                            btLine = line;
                        }
                    }
                }

                // 半径
                Revit.DB.XYZ center = arcs[0].Center;
                Revit.DB.Line rdLine = Revit.DB.Line.CreateBound(center, arcs[0].GetEndPoint(0));
                radiusF = rdLine.Length;
                radius = radiusF;
                radiusStr = ConvertLengthUnit(radiusF, unitLength, decimalPointLen, fractionTypeLen, ref radius);
                midPos = _CmpGeometry.Center2Point(rdLine);
                values.Add(radiusStr);
                valuesPos.Add(midPos);
                valuesVec.Add(_CmpGeometry.GetLeaning(midPos, rdLine.GetEndPoint(1)));
                lines.Add(rdLine);

                // 角度
                angle = _CmpGeometry.GetArcAngle(arcs);
                angleDegB = UtilGeom.AngleDeg(angle);
                angleDeg = _CmpSettings.Round(angleDegB);
                angleStr = UtilValue.Rounding(angleDeg, 4, 2);

                // 底辺
                midPos1 = _CmpGeometry.Center2Point(btLine);
                btSideF = btLine.Length;
                btSide = btSideF;
                btSideStr = ConvertLengthUnit(btSideF, unitLength, decimalPointLen, fractionTypeLen, ref btSide);
                values.Add(btSideStr);
                valuesPos.Add(midPos1);
                valuesVec.Add(_CmpGeometry.GetLeaning(midPos1, btLine.GetEndPoint(1)));

                // 高さ
                Revit.DB.Line htLine = null;
                htLine = Revit.DB.Line.CreateBound(center, midPos1);
                heightF = htLine.Length;
                height = heightF;
                heightStr = ConvertLengthUnit(heightF, unitLength, decimalPointLen, fractionTypeLen, ref height);
                midPos = _CmpGeometry.Center2Point(htLine);
                values.Add(heightStr);
                valuesPos.Add(midPos);
                valuesVec.Add(_CmpGeometry.GetLeaning(midPos, htLine.GetEndPoint(1)));
                lines.Add(htLine);

                // 面積
                areaExpnStr = "(" + radiusStr + " × " + radiusStr + " × " + piStr + " × " + angleStr + " ÷ " + "360" + ")" + " － " + "(" + btSideStr + " × " + heightStr + " ÷ " + "2" + ")";
                areaValue = (radius * radius * double.Parse(piStr) * double.Parse(angleStr) / 360) - (btSide * height / 2);
                areaCalcStr = UtilValue.Rounding(areaValue, decimalPointArea, fractionTypeArea);
                ret = true;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>根拠式の要素を設定</summary>
        ///
        /// <param name="view"      >ビュー</param>
        /// <param name="lineStyle" >線種</param>
        /// <param name="values"    >値</param>
        /// <param name="valuesPos" >値文字位置</param>
        /// <param name="valuesVec" >値文字ベクトル方向</param>
        /// <param name="lines"     >高さまたは半径線</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/07/28 Created GSA,Inc. Shinichi Ishii</p>
        ///           <p>2015/04/02 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        bool SetExpnElem(Revit.DB.View view,
                         Revit.DB.Element lineStyle,
                         Collections.Generic.IList<string> values,
                         Collections.Generic.IList<Revit.DB.XYZ> valuesPos,
                         Collections.Generic.IList<Revit.DB.XYZ> valuesVec,
                         Collections.Generic.IList<Revit.DB.Line> lines)
        {
            bool ret = false;

            if (view == null)
            {
                return ret;
            }

            // 水平配置タイプ
            Revit.DB.HorizontalTextAlignment horiznTextAlign = Revit.DB.HorizontalTextAlignment.Center;

            // 文字作成
            if ((values != null) && (valuesPos != null) && (valuesVec != null))
            {
                if (values.Count > 0)
                {
                    if (values.Count == valuesPos.Count)
                    {
                        for (int i = 0; i < values.Count; ++i)
                        {
                            Revit.DB.XYZ p0 = new Revit.DB.XYZ(0, 0, 0);
                            Revit.DB.XYZ p1 = new Revit.DB.XYZ(1, 0, 0);
                            Revit.DB.XYZ p2 = valuesVec[i];

                            double dotProduct = _CmpGeometry.DotProduct2D(p0, p1, p2);
                            double crossProduct = _CmpGeometry.CrossProduct2D(p0, p1, p2);

                            double rotate = System.Math.Atan2(crossProduct, dotProduct);

                            Revit.DB.TextNote textNote = _CmpElements.CreateTextNote(view, valuesPos[i], rotate, horiznTextAlign, values[i]);
                        }
                    }
                }
            }

            // 高さ・半径の線分作成
            if (lines != null)
            {
                if (lines.Count > 0)
                {
                    for (int i = 0; i < lines.Count; ++i)
                    {
                        Revit.DB.CurveElement curveElem = _CmpElements.CreateDetailCurve(view, lines[i], lineStyle);
                    }
                }
            }
            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>根拠式のパラメータを設定</summary>
        ///
        /// <param name="area"        >エリア</param>
        /// <param name="areaExpnStr" >面積根拠式文字</param>
        /// <param name="areaCalcStr" >計算面積文字</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool SetExpnParam(Revit.DB.Area area, string areaExpnStr, string areaCalcStr)
        {
            bool ret = true;
            if (area != null)
            {
                _EntSpArea.CurrentElem = area;

                // エリア パラメータ設定
                _EntSpArea.AreaExpn = areaExpnStr;
                _EntSpArea.AreaCalcStr = areaCalcStr;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>計算面積集計</summary>
        ///
        /// <param name="rooms"             >部屋</param>
        /// <param name="viewPlan"          >平面図ビュー</param>
        /// <param name="warningRooms"      >警告する部屋</param>
        /// <param name="progressBarThread" >プログレスバースレッド</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/28 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool CountArea(Collections.Generic.IList<Revit.DB.Architecture.Room> rooms,
                       Revit.DB.ViewPlan viewPlan,
                       ref Collections.Generic.IList<Revit.DB.Architecture.Room> warningRooms,
                       ref ProgressBarThread progressBarThread)
        {
            bool ret = true;

            double unitCoeM2 = _CmpGeometry.UnitCoeM2;
            double areaMarginError = double.Parse(_CmpAttribute.ResourceText("IDS_SET_AREAMARGINERROR"));

            // エリア取得
            Collections.Generic.IList<Revit.DB.Area> areas = _CmpElements.GetAreasOfView(viewPlan);

            if (areas.Count == 0)
            {
                ret = false;
                return ret;
            }

            if (rooms != null)
            {
                int cntProgress = 0;
                progressBarThread.SetData(rooms.Count, cntProgress);

                foreach (Revit.DB.Architecture.Room room in rooms)
                {
                    // 部屋と一致するエリア
                    Collections.Generic.IList<Revit.DB.Area> belongAreas = GetAreasOfRoom(room, areas);

                    double valAreaCalc = 0.0;
                    int valNumber = 0;
                    foreach (Revit.DB.Area area in belongAreas)
                    {
                        _EntSpArea.CurrentElem = area;

                        // 面積集計
                        valAreaCalc += _EntSpArea.AreaCalc;

                        // 枝番号登録
                        valNumber++;
                        _EntSpArea.BranchNo = valNumber.ToString();
                    }

                    // 面積集計登録
                    _EntSpRoom.CurrentElem = room;
                    _EntSpRoom.LegalArea = valAreaCalc;

                    // 面積誤差チェック
                    double roomRvtArea = _EntSpRoom.RoomArea * unitCoeM2;
                    double roomLglArea = valAreaCalc * unitCoeM2;
                    if (System.Math.Abs(roomRvtArea - roomLglArea) > areaMarginError)
                    {
                        warningRooms.Add(room);
                    }
                    progressBarThread.SetData(++cntProgress);
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>部屋名と部屋番号が一致するエリア取得</summary>
        ///
        /// <param name="room"  >部屋</param>
        /// <param name="areas" >エリア</param>
        ///
        /// <returns>エリア</returns>
        ///
        /// <history>2011/07/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Area> GetAreasOfRoom(Revit.DB.Architecture.Room room,
                                                                Collections.Generic.IList<Revit.DB.Area> areas)
        {
            Collections.Generic.IList<Revit.DB.Area> retAreas = new Collections.Generic.List<Revit.DB.Area>();

            // 部屋名・部屋番号
            _EntSpRoom.CurrentElem = room;
            string roomName = _EntSpRoom.RoomName;
            string roomNumber = _EntSpRoom.RoomNumber;

            if (areas != null)
            {
                // エリア
                foreach (Revit.DB.Area area in areas)
                {
                    // 注：暗黙の型変換ではRevitAPIのElementとRevitAPIUIのElementを取り違えてしまいエラーが出るため型変換を明示している
                    _EntSpArea.CurrentElem = (Revit.DB.Element)area;

                    // エリア名・エリア番号
                    string areaName = _EntSpArea.RoomName;
                    string areaNumber = _EntSpArea.RoomNo;

                    // 名前と番号を比較
                    if ((areaName != null) && (areaNumber != null))
                    {
                        if ((areaName == roomName) && (areaNumber == roomNumber))
                        {
                            retAreas.Add(area);
                        }
                    }
                }
            }
            return retAreas;
        }

        /// ================================================================================
        /// <summary>警告部屋テーブルデータ取得</summary>
        ///
        /// <param name="rooms">部屋</param>
        ///
        /// <returns>部屋テーブルデータ</returns>
        ///
        /// <history>2011/07/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Data.DataTable GetWarningRoomsTable(Collections.Generic.IList<Revit.DB.Architecture.Room> rooms)
        {
            double unitCoeM2 = _CmpGeometry.UnitCoeM2;
            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("RoomName", typeof(string));
            table.Columns.Add("RoomNumber", typeof(string));
            table.Columns.Add("AreaRvt", typeof(string));
            table.Columns.Add("AreaLegal", typeof(string));

            if (rooms != null)
            {
                // 部屋
                foreach (Revit.DB.Architecture.Room room in rooms)
                {
                    _EntSpRoom.CurrentElem = room;

                    // Revitの面積と計算面積
                    double roomRvtArea = _EntSpRoom.RoomArea * unitCoeM2;
                    string roomRvtAreaStr = roomRvtArea.ToString();
                    double roomLglArea = _EntSpRoom.LegalArea * unitCoeM2;
                    string roomLglAreaStr = roomLglArea.ToString();

                    // 面積誤差が大きいと警告対象
                    if (roomRvtAreaStr.Length > roomLglAreaStr.Length)
                    {
                        roomRvtAreaStr = roomRvtAreaStr.Substring(0, roomLglAreaStr.Length);
                    }

                    // 警告する部屋をテーブルデータに設定
                    System.Data.DataRow row = table.NewRow();
                    row[0] = _EntSpRoom.RoomName;
                    row[1] = _EntSpRoom.RoomNumber;
                    row[2] = roomRvtAreaStr;
                    row[3] = roomLglAreaStr;
                    table.Rows.Add(row);
                }
            }
            return table;
        }

        #endregion Member Functions

        // プロパティ
    }
}