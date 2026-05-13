using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Mechanical;
using RevitMEPAddin.Common;
using RevitMEPAddin.Filters.SelectionFilters;
using CmdDuctDisplacement.Constant;
using Autodesk.Revit.DB.Plumbing;
using CmdDuctDisplacement.Filters.SelectionFilters;
using CmdDuctDisplacement.Resource;

namespace CmdDuctDisplacement.Logic
{
    abstract public class MEPOperation
    {
        // メンバ変数
        #region Memeber Variables
        protected Autodesk.Revit.ApplicationServices.Application app;
        protected UIDocument uidoc;
        protected Document doc;
        protected WrpGeometry _geometry;
        protected WrpMEP _mep;
        protected WrpViews _view;
        protected Logger log;

        #region 図面編集情報
        // 参照ビュー
        protected View view;
        // 1つ目の切断点(0:始点側、1:終点側)
        protected int whichSideDuct;
        // 始点側切断点
        protected MEPCurve curve1;
        protected MEPCurve outDuct1;
        protected XYZ pt1;
        // 終点側切断点
        protected MEPCurve curve2;
        protected MEPCurve outDuct2;
        protected XYZ pt2;
        // 切断点補助線ID
        protected ElementId sLineId = null, eLineId = null;
        // 区間内ダクトIDリスト
        protected List<ElementId> mDuctIds;
        // 移動量計算対象図形情報
        protected Element target;
        // 耐火被覆考慮要否
        protected bool needInsulate;
        #endregion

        #endregion

        // コンストラクタ
        #region Constructor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="log"></param>
        public MEPOperation(Autodesk.Revit.ApplicationServices.Application app, UIDocument uidoc, Logger log)
        {
            this.app = app;
            this.uidoc = uidoc;
            doc = uidoc.Document;
            this.log = log;
            _geometry = new WrpGeometry(uidoc, log);
            _mep = new WrpMEP(uidoc, log);
            _view = new WrpViews(uidoc, log);
            // 初期化
            Init();
        }
        #endregion

        // メンバ関数
        #region Member Functions
        #region 【コマンド関連】初期化
        /// <summary>
        /// メンバの初期化
        /// </summary>
        public void Init()
        {
            // 始点側切断点
            curve1 = null;
            outDuct1 = null;
            pt1 = new XYZ();
            // 終点側切断点
            curve2 = null;
            outDuct2 = null;
            pt2 = new XYZ();
            // 1つ目の切断点(0:始点側、1:終点側)
            whichSideDuct = DuctDisplacementDefine.START_SIDE;
            // 切断点補助線ID
            sLineId = null;
            eLineId = null;
            // 移動するダクトのElementIdリスト
            mDuctIds = new List<ElementId>();

            // 移動量計算対象図形情報
            target = null;
            needInsulate = false;
        }
        #endregion

        #region【コマンド関連】PickPoint

        /// <summary>
        /// ただ点をピック
        /// </summary>
        /// <param name="pt">切断点</param>
        /// <param name="duct">ダクト/配管</param>
        /// <param name="ptCnt">切断点No</param>
        /// <param name="lineId">切断点表記ラインのID</param>
        /// <param name="isFirstPt">アクティブビューをメンバのviewに設定するフラグ</param>
        /// <returns></returns>
        public bool PickPointOnDuct1(ref XYZ pt, ref MEPCurve duct, int ptCnt, ref ElementId lineId, bool isFirstPt)
        {
            // SketchPlaneをセット
            if (_view.SetSketchPlane() == null) return false;
            // 点とダクトを取得
            while (duct == null)
            {
                // 点をピック
                pt = uidoc.Selection.PickPoint(ObjectSnapTypes.Intersections | ObjectSnapTypes.WorkPlaneGrid, "切断する点を入力してください。" + ptCnt + "点目：");
                if (isFirstPt)
                {
                    view = doc.ActiveView;
                }
                // 点を含むダクトを取得
                _mep.GetDuctByPoint(ref duct, pt);
                if (!_mep.GetCutPoint(duct, ref pt)) continue;
            }
            // 切断点箇所にモデルライン作成
            lineId = CreateCutLine(duct, pt);
            if (lineId == null) return false;

            return true;
        }

        /// <summary>
        /// ダクトのFaceを取得し、そのFaceをSketchPlaneに設定し、
        /// 点をピック
        /// </summary>
        /// <param name="pt">切断点</param>
        /// <param name="duct">ダクト/配管</param>
        /// <param name="ptCnt">切断点No</param>
        /// <param name="lineId">切断点表記ラインのID</param>
        /// <param name="isFirstPt">アクティブビューをメンバのviewに設定するフラグ</param>
        /// <returns></returns>
        public bool PickPointOnDuct2(ref XYZ pt, ref MEPCurve duct, int ptCnt, ref ElementId lineId, bool isFirstPt)
        {
            WrpViews _view = new WrpViews(uidoc, log);

            while (duct == null)
            {
                // 面をピック
                ISelectionFilter filter = new DuctPipeSelectionFilter(uidoc, log);
                Reference r = uidoc.Selection.PickObject(ObjectType.Face, filter, "切断するダクト(の面)を選択してください。");
                duct = doc.GetElement(r) as MEPCurve;
                if (null != duct)
                {
                    PlanarFace face
                        = duct.GetGeometryObjectFromReference(r) as PlanarFace;
                    if (face != null && _view.SetSketchPlane(face.FaceNormal, face.Origin) != null)
                    {
                        // 点をピック
                        pt = uidoc.Selection.PickPoint(ObjectSnapTypes.Intersections | ObjectSnapTypes.WorkPlaneGrid, "切断する点を入力してください。" + ptCnt + "点目：");
                        if (isFirstPt)
                        {
                            view = doc.ActiveView;
                        }
                    }
                }
                if (!_mep.GetCutPoint(duct, ref pt)) continue;
            }
            // 切断点箇所にモデルライン作成
            lineId = CreateCutLine(duct, pt);
            if (lineId == null) return false;
            return true;
        }


        /// <summary>
        /// 【★こちらを採用？★】ダクト選択+点ピック
        /// ※点ピックではスナップあり、ピックした点を
        /// 　ダクト(ローケーションカーブ)へ射影した点を取得
        /// </summary>
        /// <param name="pt">切断点</param>
        /// <param name="duct">ダクト/配管</param>
        /// <param name="ptCnt">切断点No</param>
        /// <param name="lineId">切断点表記ラインのID</param>
        /// <param name="isFirstPt">アクティブビューをメンバのviewに設定するフラグ</param>
        /// <returns></returns>
        public bool PickPointOnDuct3(ref XYZ pt, ref MEPCurve duct, int ptCnt, ref ElementId lineId, bool isFirstPt)
        {
            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("PickPointOnDuct3");

                while (duct == null)
                {
                    // ダクトをピック
                    ISelectionFilter filter = new DuctPipeSelectionFilter(uidoc, log);
                    Reference r = uidoc.Selection.PickObject(ObjectType.Element, filter, ExResources.ResxString(DuctDisplacementDefine.REQ_ENTER_CUT_OBJ));
                    duct = doc.GetElement(r) as MEPCurve;
                    if (isFirstPt)
                    {
                        view = doc.ActiveView;
                    }
                    if (null != duct)
                    {
                        if (_view.SetSketchPlane() == null) return false;

                        // 点をピック
                        pt = uidoc.Selection.PickPoint(ObjectSnapTypes.Intersections | ObjectSnapTypes.WorkPlaneGrid
                                                        /*| ObjectSnapTypes.Endpoints | ObjectSnapTypes.Midpoints*/,
                                                        ExResources.ResxString(DuctDisplacementDefine.REQ_ENTER_POINT) + ptCnt + ExResources.ResxString(DuctDisplacementDefine.REQ_ENTER_POINT_CNT));
                        log.Trace(pt.ToString());
                        if (!_mep.GetCutPoint(duct, ref pt)) continue;
                        //// 切断点が端点でかつ製造パーツに接続する場合は
                        //// 選択を無効にする
                        //if (CheckEndPointConnectingFab(pt, duct))
                        //{
                        //    duct = null;
                        //    pt = null;
                        //    TaskDialog.Show(DuctDisplacementDefine.DIALOG_TITLE_WARN, ExResources.ResxString(DuctDisplacementDefine.MSG_WARN3));
                        //}
                    }
                    if (!_mep.GetCutPoint(duct, ref pt)) continue;
                }

                // 切断点箇所にモデルライン作成
                lineId = CreateCutLine(duct, pt);
                if (lineId == null)
                {
                    tran.RollBack();
                    return false;
                }
                tran.Commit();
                return true;
            }
        }

        /// <summary>
        /// ダクト/配管の面またはエッジ上の点とダクトを取得
        /// </summary>
        /// <param name="pt">切断点</param>
        /// <param name="duct">ダクト/配管</param>
        /// <param name="ptCnt">切断点No</param>
        /// <param name="lineId">切断点表記ラインのID</param>
        /// <param name="isFirstPt">アクティブビューをメンバのviewに設定するフラグ</param>
        /// <returns></returns>
        public bool PickPointOnDuct4(ref XYZ pt, ref MEPCurve duct, int ptCnt, ref ElementId lineId, bool isFirstPt)
        {
            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("PickPointOnDuct4");
                while (duct == null)
                {
                    ISelectionFilter filter = new DuctPipeSelectionFilter(uidoc, log);
                    Reference r = uidoc.Selection.PickObject(ObjectType.Element, filter, ExResources.ResxString(DuctDisplacementDefine.REQ_ENTER_POINT) + ptCnt + ExResources.ResxString(DuctDisplacementDefine.REQ_ENTER_POINT_CNT));
                    duct = doc.GetElement(r) as MEPCurve;
                    pt = r.GlobalPoint;
                    log.Trace(pt.ToString());
                    if (isFirstPt)
                    {
                        view = doc.ActiveView;
                    }
                    if (!_mep.GetCutPoint(duct, ref pt)) continue;
                    //// 切断点が端点でかつ製造パーツに接続する場合は
                    //// 選択を無効にする
                    //if (CheckEndPointConnectingFab(pt, duct))
                    //{
                    //    duct = null;
                    //    pt = null;
                    //    TaskDialog.Show(DuctDisplacementDefine.DIALOG_TITLE_WARN, ExResources.ResxString(DuctDisplacementDefine.MSG_WARN3));
                    //}
                }


                // 切断点箇所にモデルライン作成
                lineId = CreateCutLine(duct, pt);
                if (lineId == null)
                {
                    tran.RollBack();
                    return false;
                }
                tran.Commit();
                return true;
            }


        }

        /// <summary>
        /// ２パターンの入力に対応する
        /// ①通り芯選択＋ダクト選択（交点を切断点とする）
        /// ②ダクト/配管オブジェクト上の点選択
        /// </summary>
        /// <param name="pt">切断点</param>
        /// <param name="duct">ダクト/配管</param>
        /// <param name="ptCnt">切断点No</param>
        /// <param name="lineId">切断点表記ラインのID</param>
        /// <param name="isFirstPt">アクティブビューをメンバのviewに設定するフラグ</param>
        /// <returns></returns>
        public bool PickPointOnDuct5(ref XYZ pt, ref MEPCurve duct, int ptCnt, ref ElementId lineId, bool isFirstPt)
        {
            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("PickPointOnDuct5");
                while (duct == null)
                {
                    ISelectionFilter ductPipeGridFilter = new DuctPipeGridSelectionFilter(uidoc, log);
                    Reference r = uidoc.Selection.PickObject(ObjectType.Element, ductPipeGridFilter, ExResources.ResxString(DuctDisplacementDefine.REQ_ENTER_POINT_OR_GRID) + ptCnt + ExResources.ResxString(DuctDisplacementDefine.REQ_ENTER_POINT_CNT));
                    Grid grid = doc.GetElement(r) as Grid;
                    duct = doc.GetElement(r) as MEPCurve;
                    pt = r.GlobalPoint;

                    if (grid != null && duct == null)
                    {
                        ISelectionFilter filter = new DuctPipeSelectionFilter(uidoc, log);
                        r = uidoc.Selection.PickObject(ObjectType.Element, filter, ExResources.ResxString(DuctDisplacementDefine.REQ_ENTER_MEPCURVE) + ptCnt + ExResources.ResxString(DuctDisplacementDefine.REQ_ENTER_POINT_CNT));
                        duct = doc.GetElement(r) as MEPCurve;
                        pt = _geometry.GetIntersectInXYPlane((Line)grid.Curve, (Line)(duct.Location as LocationCurve).Curve);
                        if (pt == null)
                        {
                            duct = null;
                            continue;
                        }
                    }

                    log.Trace(pt.ToString());
                    if (isFirstPt)
                    {
                        view = doc.ActiveView;
                    }
                    if (!_mep.GetCutPoint(duct, ref pt)) continue;
                    // 切断点が端点でかつ製造パーツに接続する場合は
                    // 選択を無効にする
                    if (CheckEndPointConnectingFab(pt, duct))
                    {
                        duct = null;
                        pt = null;
                        TaskDialog.Show(ExResources.ResxString(DuctDisplacementDefine.DIALOG_TITLE_WARN), ExResources.ResxString(DuctDisplacementDefine.MSG_WARN3));
                    }
                }


                // 切断点箇所にモデルライン作成
                lineId = CreateCutLine(duct, pt);
                if (lineId == null)
                {
                    tran.RollBack();
                    return false;
                }
                tran.Commit();
                return true;
            }


        }

        /// <summary>
        /// 指定の点が指定のカーブの製造パーツに接続する端点であるかどうか？
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        private bool CheckEndPointConnectingFab(XYZ pt, MEPCurve curve)
        {
            if (pt == null || curve == null) return false;

            Connector sCon = _mep.GetStartSideConnector(curve);
            Connector eCon = _mep.GetEndSideConnector(curve);
            if (sCon.Origin.IsAlmostEqualTo(pt) && _mep.IsConnectedToFabPart(sCon))
            {
                // 指定の点が指定のMEPCurveの始点側端点であり、
                // かつ、製造パーツと接続している場合
                return true;
            }
            else if (eCon.Origin.IsAlmostEqualTo(pt) && _mep.IsConnectedToFabPart(eCon))
            {
                // 指定の点が指定のMEPCurveの終点側端点であり、
                // かつ、製造パーツと接続している場合
                return true;
            }

            // そのほかはfalse
            return false;
        }



        /// <summary>
        /// 切断線描画
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public ElementId CreateCutLine(MEPCurve curve, XYZ pt)
        {
            // sketchPlaneに切断点を通り、
            // アクティブビューに平行な面を設定。
            WrpViews _view = new WrpViews(uidoc, log);
            SketchPlane sketchPlane = _view.SetSketchPlane(doc.ActiveView, pt);
            if (sketchPlane == null) return null;

            // 高さ
            Parameter prmH = curve.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM);
            // 幅
            Parameter prmW = curve.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM);
            // 直径
            Parameter prmR = curve.get_Parameter(BuiltInParameter.RBS_CURVE_DIAMETER_PARAM);
            // 直径
            Parameter prmPipeR = curve.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);

            List<double> lengthList = new List<double>();
            if (prmH != null)
            {
                lengthList.Add(prmH.AsDouble());
            }
            if (prmW != null)
            {
                lengthList.Add(prmW.AsDouble());
            }
            if (prmR != null)
            {
                lengthList.Add(prmR.AsDouble());
            }
            if (prmPipeR != null)
            {
                lengthList.Add(prmPipeR.AsDouble());
            }

            double length = lengthList.Max();
            length = length * 1.5;
            LocationCurve lCurve = curve.Location as LocationCurve;
            if (lCurve == null)
            {
                log.Error("");
                return null;
            }
            Line lCurveLine = lCurve.Curve as Line;
            if (lCurveLine == null)
            {
                log.Error("");
                return null;
            }

            XYZ dir = lCurveLine.Direction;

            WrpLineStyle _lineStyle = new WrpLineStyle(uidoc, log);

            // 切断線用のLineStyle作成
            GraphicsStyle lineGraphicStyle = null;
            //TODO リソースから取得
            _lineStyle.CreateLineStyle(ref lineGraphicStyle, DuctDisplacementDefine.CUT_LINE_LINESTYLE_NAME,
                255, 0, 255, 5);
            // 線を描画
            Transform rotate90 = Transform.CreateRotationAtPoint(doc.ActiveView.ViewDirection, Math.PI / 180 * 90, pt);
            XYZ pt1 = pt.Subtract(dir.Normalize().Multiply(length / 2));
            XYZ pt2 = pt.Add(dir.Normalize().Multiply(length / 2));
            Line line = Line.CreateBound(rotate90.OfPoint(pt1), rotate90.OfPoint(pt2));
            DetailLine detailLine = doc.Create.NewDetailCurve(doc.ActiveView, line) as DetailLine;
            if (detailLine == null)
            {
                return null;
            }
            // 切断線用のlineStyleに変更
            detailLine.LineStyle = lineGraphicStyle;

            return detailLine.Id;
        }

        #endregion

        #region【コマンド関連】切断点入力

        /// <summary>
        /// ダクトを区間でカットするための２点を取得
        /// </summary>
        /// <param name="patrnNo">区間選択方法指示No</param>
        /// <returns></returns>
        public Result PickCutPoints(int patrnNo)
        {
            try
            {
                WrpGeometry _geometry = new WrpGeometry(uidoc, log);
                bool done = false;
                while (!done)
                {
                    switch (patrnNo)
                    {
                        case 0:
                            // 1つ目の切断点取得
                            PickPointOnDuct1(ref pt1, ref curve1, 1, ref sLineId, true);
                            // 2つ目の切断点取得
                            PickPointOnDuct1(ref pt2, ref curve2, 2, ref eLineId, false);
                            break;
                        case 1:
                            // 1つ目の切断点取得
                            PickPointOnDuct2(ref pt1, ref curve1, 1, ref sLineId, true);
                            // 2つ目の切断点取得
                            PickPointOnDuct2(ref pt2, ref curve2, 2, ref eLineId, false);
                            break;
                        case 2:
                            // 1つ目の切断点取得
                            PickPointOnDuct3(ref pt1, ref curve1, 1, ref sLineId, true);
                            // 2つ目の切断点取得
                            PickPointOnDuct3(ref pt2, ref curve2, 2, ref eLineId, false);
                            break;
                        case 3:
                            // 1つ目の切断点取得
                            PickPointOnDuct4(ref pt1, ref curve1, 1, ref sLineId, true);
                            // 2つ目の切断点取得
                            PickPointOnDuct4(ref pt2, ref curve2, 2, ref eLineId, false);
                            break;
                        case 4:
                            // 1つ目の切断点取得
                            PickPointOnDuct5(ref pt1, ref curve1, 1, ref sLineId, true);
                            // 2つ目の切断点取得
                            PickPointOnDuct5(ref pt2, ref curve2, 2, ref eLineId, false);
                            break;


                    }
                    log.Info(@"curve1:" + curve1.Id + " pt1:" + pt1.ToString());
                    log.Info(@"curve2:" + curve2.Id + " pt2:" + pt2.ToString());
                    done = true;

                    List<ElementId> ids = new List<ElementId>();
                    GetOrderdSysMemberList(ref ids, new List<ElementId>(), curve1);

                    if (!ids.Contains(curve2.Id))
                    {
                        // 2つのMEPCurveは一つのシステムに含まれていません。
                        TaskDialog.Show(ExResources.ResxString(DuctDisplacementDefine.DIALOG_TITLE_WARN), ExResources.ResxString(DuctDisplacementDefine.MSG_WARN1));
                        DeleteCutLines();
                        curve1 = null;
                        pt1 = null;
                        curve2 = null;
                        pt2 = null;
                        done = false;
                    }
                    else if (pt1.IsAlmostEqualTo(pt2))
                    {
                        // 移動区間を指定するための2点は、異なる点を入力してください。
                        TaskDialog.Show(ExResources.ResxString(DuctDisplacementDefine.DIALOG_TITLE_WARN), ExResources.ResxString(DuctDisplacementDefine.MSG_WARN4));
                        DeleteCutLines();
                        curve1 = null;
                        pt1 = null;
                        curve2 = null;
                        pt2 = null;
                        done = false;
                    }
                }

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                log.Trace("入力が中断されました。");
                mDuctIds = null;
                return Result.Cancelled;
            }
        }

        /// <summary>
        /// 区間指定1点目がダクトであるかどうか？
        /// </summary>
        /// <returns></returns>
        public bool IsDuct()
        {
            return curve1 is Duct;
        }

        /// <summary>
        /// 区間指定1点目が配管であるかどうか？
        /// </summary>
        /// <returns></returns>
        public bool IsPipe()
        {
            return curve1 is Pipe;
        }

        #endregion

        #region 【コマンド関連】参照ビュー
        /// <summary>
        /// ビューの参照レベルの名前取得
        /// </summary>
        /// <returns></returns>
        public string GetViewLevelName()
        {
            return _view.GetViewLevelName(view);
        }

        /// <summary>
        /// ビューの参照レベルの名前取得
        /// </summary>
        /// <returns></returns>
        public double GetViewLevelElevation()
        {
            return _view.GetViewLevelElevation(view);
        }

        #endregion

        #region 【コマンド関連】PickObject

        /// <summary>
        /// ElementまたはLinkedElementのピック
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="fromLinkedMmodel">回避対象物をリンクモデルから取得するかどうか？</param>
        /// <returns></returns>
        public Result PickElementAndLinkedElement(ref Element elm, bool fromLinkedMmodel)
        {
            try
            {
                elm = null;
                ISelectionFilter filter = new DuctPipeBeamSelectionFilter(uidoc, log);

                while (elm == null)
                {
                    Reference r = null;

                    if (!fromLinkedMmodel)
                    {// リンクモデル以外から対象を選択
                        r = uidoc.Selection.PickObject(ObjectType.Element , filter, ExResources.ResxString(DuctDisplacementDefine.REQ_ENTER_TARGET));
                        elm = doc.GetElement(r);
                    }
                    else
                    {// リンクモデルから対象を選択
                        r = uidoc.Selection.PickObject(ObjectType.LinkedElement, filter, ExResources.ResxString(DuctDisplacementDefine.REQ_ENTER_TARGET));
                        RevitLinkInstance pElm = doc.GetElement(r) as RevitLinkInstance;
                        Document pDoc = pElm.GetLinkDocument();
                        elm = pDoc.GetElement(r.LinkedElementId);
                    }
                  
                }

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                elm = null;
                log.Trace("[PickObect]入力が中断されました。");
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                log.Error("[PickObect]" + ex.Message);
                return Result.Cancelled;
            }
        }


        #endregion

        #region 【コマンド関連】移動量算出

        /// <summary>
        /// 移動距離算出対象物と移動方向の取得
        /// 移動距離算出対象物は、targeteIdにセットする
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="fromLinkedMmodel">回避対象物をリンクモデルから取得するかどうか？</param>
        /// <returns></returns>
        public Result PickTargetAndDirectionToAvoid(ref int dir, bool fromLinkedMmodel)
        {
            Element elm = null;
            Result res;
            res = PickElementAndLinkedElement(ref elm, fromLinkedMmodel);
            // 対象物選択失敗はすべてCancelledで返却。
            if (res != Result.Succeeded || elm == null) return Result.Cancelled;

            target = elm;

            // 交わし方向判定
            if (elm.Category.Id.ToString() == ((int)BuiltInCategory.OST_StructuralFraming).ToString()
                || elm is DirectShape)
            {
                // 対象が梁の場合は下方向にかわす
                dir = DuctDisplacementDefine.DIR_DOWN;
                // 耐火被覆圧の考慮を必要とする。
                needInsulate = true;
            }
            else if (elm is MEPCurve)
            {
                // 対象がダクトまたはパイプの場合はLocationCurveの始点高さの比較で決める。
                // 耐火被覆圧の考慮を必要としない。
                needInsulate = false;

                MEPCurve duct = null;
                if (whichSideDuct == DuctDisplacementDefine.START_SIDE)
                {// １点目入力が始点側の場合
                    duct = curve1;
                }
                else if (whichSideDuct == DuctDisplacementDefine.END_SIDE)
                {// １点目入力が終点側の場合
                    duct = curve2;
                }
                if (duct == null) return Result.Failed;
                LocationCurve lDuctCurve = duct.Location as LocationCurve;
                LocationCurve lTargetCurve = elm.Location as LocationCurve;
                if (lDuctCurve == null || lTargetCurve == null) return Result.Failed;
                if (lDuctCurve.Curve.GetEndPoint(0).Z >= lTargetCurve.Curve.GetEndPoint(0).Z)
                {
                    dir = DuctDisplacementDefine.DIR_UPPER;
                    log.Info("移動図形高さ：" + lDuctCurve.Curve.GetEndPoint(0).Z +
                        " 避ける対象図形高さ：" + lTargetCurve.Curve.GetEndPoint(0).Z);
                }
                else
                {
                    dir = DuctDisplacementDefine.DIR_DOWN;
                    log.Info("移動図形高さ：" + lDuctCurve.Curve.GetEndPoint(0).Z +
                        " 避ける対象図形高さ：" + lTargetCurve.Curve.GetEndPoint(0).Z);
                }

            }

            //どちらもなければわたってきた値。
            //TaskDialog.Show("test", elm.GetType().ToString());

            return Result.Succeeded;
        }

        /// <summary>
        /// ダクトと高さ基準オブジェクトの位置情報から
        /// 離隔の場合の移動量算出
        /// </summary>
        /// <param name="hDiff">移動距離</param>
        /// <param name="clearance">離隔</param>
        /// <param name="offset">レベルオフセット</param>
        /// <param name="offsetPos">オフセット基準位置</param>
        /// <param name="direction">かわし方向</param>
        /// <param name="roundUnit">丸め単位</param>
        /// <param name="minClear">最小クリアランス</param>
        /// <param name="insulate">耐火被覆圧</param>
        /// <returns></returns>
        public abstract Result CalculateDiff(out double hDiff, out double clearance, out double offset,
            int offsetPos, int direction, int roundUnit, double minClear, double insulate);


        /// <summary>
        /// 丸め（絶対値が大きくなる方向）
        /// </summary>
        /// <param name="value">丸めたい値</param>
        /// <param name="roundUnit">丸め精度</param>
        /// <returns></returns>
        protected double Round(double value, int roundUnit)
        {
            int sign = 1;
            if (value < 0) sign = -1;

            if (Math.Abs(value) % roundUnit > 0)
            {
                return sign * roundUnit * (Math.Floor(value / roundUnit) + 1);
            }
            else
            {
                return sign * roundUnit * Math.Floor(value / roundUnit);
            }
        }


        /// <summary>
        /// 丸め（方向を指定）
        /// </summary>
        /// <param name="value">丸めたい値</param>
        /// <param name="roundUnit">丸め精度</param>
        /// <param name="mode">0:上方向への丸め、1:下方向への丸め</param>
        /// <returns></returns>
        protected bool Round(ref double value, int roundUnit, int mode)
        {
            if (roundUnit == DuctDisplacementDefine.num_0)
            {
                // 丸めなしの場合
                return true;
            }
            if (mode == DuctDisplacementDefine.DIR_UPPER)
            {
                value = roundUnit * (Math.Ceiling(value / roundUnit));
                return true;
            }
            else if (mode == DuctDisplacementDefine.DIR_DOWN)
            {
                value = roundUnit * (Math.Floor(value / roundUnit));
                return true;
            }
            return false;

        }

        #endregion

        #region【コマンド関連】移動

        /// <summary>
        /// 2点で切断して移動
        /// </summary>
        /// <param name="movePtn">移動方法(オフセット：0, レベル統一：1)</param>
        /// <param name="fifPtn">接続方法(45度：0, 90度：1, S管：2)</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="flg">S管ロード済フラグ辞書(角型ダクト：1, 円型ダクト：2, 楕円型ダクト：3, 配管：4)</param>
        /// <param name="message">移動失敗時ダイアログ表示メッセージ</param>
        /// <returns></returns>
        public abstract bool ModDuctLevelPartially(int movePtn, int fifPtn, double hDiff, ref Dictionary<int, bool> flg, out string message);


        /// <summary>
        /// (2019/06/18 今回は対応なし。)
        /// 切断部のダクトが区間に含まれるかを判定し、
        /// 区間内側と外側のダクトの（再）セットを行う
        /// </summary>
        /// <param name="cutDuctPair">切断部両側のダクトID</param>
        /// <param name="systemId">機械システムID</param>
        /// <param name="inDuct"></param>
        /// <param name="outDuct"></param>
        /// <returns></returns>
        protected abstract bool ResetInOutDuct(MEPCurve[] cutDuctPair, ElementId mSystemId, ref MEPCurve inDuct, ref MEPCurve outDuct);


        /// <summary>
        /// (2019/06/18 今回は対応なし。)
        /// レベル統一
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="ductHeight"></param>
        /// <param name="hDiffFeet"></param>
        /// <returns></returns>
        protected abstract bool UnifyLevel(ElementId systemId, double ductHeight, double hDiffFeet);

        /// <summary>
        /// Duct/PipingNetworkのメンバーリスト取得
        /// ※機械システムやパイプシステムを利用せず、実際の接続されているメンバをたどる
        /// </summary>
        /// <param name="sysMemberIds">接続されているElementたち</param>
        /// <param name="banSysMemberIds">接続をたどってはならないElementたち</param>
        /// <param name="sysMember">接続をたどり始める最初のElement</param>
        /// <returns></returns>
        public bool GetOrderdSysMemberList(ref List<ElementId> sysMemberIds, List<ElementId> banSysMemberIds, Element sysMember)
        {
            try
            {
                if (!sysMemberIds.Contains(sysMember.Id))
                {
                    sysMemberIds.Add(sysMember.Id);
                }

                // コネクターたちを取得
                ConnectorSet cons = null;
                _mep.GetConnectorsFromMSystemMember(ref cons, sysMember);

                // 接続しているElement情報取得
                foreach (Connector con in cons)
                {
                    ConnectorSet refCons = con.AllRefs;
                    foreach (Connector refCon in refCons)
                    {
                        if (!sysMemberIds.Contains(refCon.Owner.Id) && !banSysMemberIds.Contains(refCon.Owner.Id) && !(refCon.Owner is MechanicalSystem) && !(refCon.Owner is PipingSystem))
                        {
                            sysMemberIds.Add(refCon.Owner.Id);
                            // 再帰的に取得
                            if (!GetOrderdSysMemberList(ref sysMemberIds, banSysMemberIds, refCon.Owner))
                            {
                                log.Error("GetOrderdSysMemberList:リストの取得に失敗しました。");
                                return false;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(@"[GetOrderdSysMemberList]" + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 切断点に続かない分岐は、
        /// 最初に出現したジェネリックなダクト/配管の真ん中で切断する
        /// </summary>
        /// <param name = "branchConnectInfoLis" > 分岐切断情報(切断点・内側ダクト / 配管・外側ダクト / 配管)リスト</param>
        /// <param name="sysMemberIds">※再帰呼び出し時利用</param>
        /// <param name="sysMember">接続をたどる開始位置エレメント</param>
        /// <param name="ductE">反対側の切断点を含むダクト/配管</param>
        /// <returns></returns>
        public abstract bool CutBranchCurve(ref List<BranchConnectInfo> branchConnectInfoLis, ref List<ElementId> sysMemberIds, Element sysMember, MEPCurve ductE);

        /// <summary>
        /// 切断指定部の接続
        /// </summary>
        /// <param name="duct">切断部のダクト/配管(区間内側)</param>
        /// <param name="outDuct">切断部のダクト/配管(区間外側)</param>
        /// <param name="pt">切断点</param>
        /// <param name="fifPtn">接続方法(45度：0, 90度：1, S管：2)</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="doneSCurveLoad">S管ロード済フラグ辞書(角型ダクト：1, 円型ダクト：2, 楕円型ダクト：3, 配管：4)</param>
        /// <returns></returns>
        public abstract bool ConnectCurves(MEPCurve duct, MEPCurve outDuct, XYZ pt, int fifPtn, double hDifff, ref Dictionary<int, bool> doneSCurveLoad);

        /// <summary>
        /// curve1とcurve2を含むMechanicalSystemを移動
        /// </summary>
        /// <param name="hDiff">移動距離</param>
        /// <param name="branchConnectInfoList">S管ロード済フラグ辞書(角型ダクト：1, 円型ダクト：2, 楕円型ダクト：3, 配管：4)</param>
        /// <returns></returns>
        public bool MoveMechanicalSystem(double hDiff, ref List<BranchConnectInfo> branchConnectInfoList)
        {
            log.Trace("ダクト移動開始");
            // 切断区間のシステムID取得（再度）
            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("MoveMechanicalSystem");

                List<ElementId> mDuctIds = new List<ElementId>();
                GetOrderdSysMemberList(ref mDuctIds, new List<ElementId>(), curve1);
                if (!mDuctIds.Contains(curve2.Id) || mDuctIds.Count <= 0) return false;
                // 移動図形(MechanicalSystemに含まれるエレメント)をグループ化
                Group group = doc.Create.NewGroup(mDuctIds);

                // 移動量をベクトルで表す。
                XYZ vDiff = new XYZ(0, 0, _geometry.ConvertMillimetersToFeet(hDiff));
                // グループを移動
                group.Location.Move(vDiff);
                group.UngroupMembers();

                // 切断点情報も移動しておく。
                pt1 = pt1.Add(vDiff);
                pt2 = pt2.Add(vDiff);
                foreach (BranchConnectInfo info in branchConnectInfoList)
                {
                    XYZ pt = info.BranchCutPt;
                    info.BranchCutPt = pt.Add(vDiff);
                }

                log.Trace("ダクト移動終了");
                tran.Commit();
            }
            return true;
        }

        /// <summary>
        /// curve1とcurve2を含むPipingSystemを移動
        /// </summary>
        /// <param name="hDiff">移動距離</param>
        /// <param name="branchConnectInfoList">分岐切断情報(切断点・内側ダクト / 配管・外側ダクト / 配管)リスト</param>
        /// <returns></returns>
        public bool MovePipingSystem(double hDiff, ref List<BranchConnectInfo> branchConnectInfoList)
        {
            log.Trace("パイプ移動開始");
            // 切断区間のシステムID取得（再度）
            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("MovePipingSystem");

                List<ElementId> mDuctIds = new List<ElementId>();
                GetOrderdSysMemberList(ref mDuctIds, new List<ElementId>(), curve1);
                if (!mDuctIds.Contains(curve2.Id) || mDuctIds.Count <= 0) return false;
                // 移動図形(PipingSystemに含まれるエレメント)をグループ化
                Group group = doc.Create.NewGroup(mDuctIds);

                // 移動量をベクトルで表す。
                XYZ vDiff = new XYZ(0, 0, _geometry.ConvertMillimetersToFeet(hDiff));
                // グループを移動
                group.Location.Move(vDiff);
                group.UngroupMembers();

                // 切断点情報も移動しておく。
                pt1 = pt1.Add(vDiff);
                pt2 = pt2.Add(vDiff);
                foreach (BranchConnectInfo info in branchConnectInfoList)
                {
                    XYZ pt = info.BranchCutPt;
                    info.BranchCutPt = pt.Add(vDiff);
                }

                log.Trace("パイプ移動終了");
                tran.Commit();
            }
            return true;
        }

        /// <summary>
        /// ダクトシステムから指定区間を切り取る
        /// </summary>
        /// <param name="cutDuctPair1">切断部(区間内側ダクト・配管/区間外側ダクト・配管)ペア</param>
        /// <param name="cutDuctPair2">切断部(区間内側ダクト・配管/区間外側ダクト・配管)ペア</param>
        /// <returns></returns>
        public bool CutSpecifiedDuctNetworkSection(ref MEPCurve[] cutDuctPair1, ref MEPCurve[] cutDuctPair2)
        {
            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("CutSpecifiedDuctNetworkSection");

                // 区間が1つのダクト/配管内で収まる場合
                if (curve1.Id.Equals(curve2.Id))
                {
                    XYZ sPt = new XYZ();
                    if (!_geometry.GetLocationCurveStartPoint(ref sPt, curve1)) return false;
                    if (sPt.DistanceTo(pt1) > sPt.DistanceTo(pt2))
                    {
                        // pt2の方が始点に近い場合(ダクト/配管をカットする順番が大事!)
                        // pt2でのダクト/配管カット
                        BreakCurve(ref cutDuctPair2, ref curve2, ref pt2);
                        // pt1でのダクト/配管カット
                        BreakCurve(ref cutDuctPair1, ref curve1, ref pt1);
                        // 終点側をカットすると、移動ダクト/配管のIDが変わる
                        if (cutDuctPair1.Length == 2)
                        {
                            // 新IDに入れ替え
                            curve1 = cutDuctPair1[1];
                            curve2 = cutDuctPair1[1];
                            outDuct1 = cutDuctPair1[0];
                            // curve2に相当するもの
                            cutDuctPair2[0] = cutDuctPair1[1];

                        }
                        if (cutDuctPair2.Length == 2)
                        {
                            outDuct2 = cutDuctPair2[1];
                        }
                    }
                    else if (sPt.DistanceTo(pt1) < sPt.DistanceTo(pt2))
                    {
                        // pt1でのダクト/配管カット
                        BreakCurve(ref cutDuctPair1, ref curve1, ref pt1);
                        // pt2でのダクト/配管カット
                        BreakCurve(ref cutDuctPair2, ref curve2, ref pt2);
                        // 終点側をカットすると、移動ダクト/配管のIDが変わる
                        if (cutDuctPair2.Length == 2)
                        {
                            // 新IDに入れ替え
                            curve1 = cutDuctPair2[1];
                            curve2 = cutDuctPair2[1];
                            outDuct2 = cutDuctPair2[0];
                            // curve1に相当するもの
                            cutDuctPair1[0] = cutDuctPair2[1];

                        }
                        if (cutDuctPair1.Length == 2)
                        {
                            outDuct1 = cutDuctPair1[1];
                        }
                    }
                    tran.Commit();
                    return true;
                }

                // pt1でのダクト/配管カット
                BreakCurve(ref cutDuctPair1, ref curve1, ref pt1);
                List<ElementId> ids = new List<ElementId>();
                GetOrderdSysMemberList(ref ids, new List<ElementId>(), cutDuctPair1[0]);
                if (cutDuctPair1.Length == 2)
                {
                    if (ids.Contains(Curve2.Id))
                    {
                        curve1 = cutDuctPair1[0];
                        outDuct1 = cutDuctPair1[1];
                    }
                    else
                    {
                        curve1 = cutDuctPair1[1];
                        outDuct1 = cutDuctPair1[0];
                    }
                }

                // pt2でのダクト/配管カット
                BreakCurve(ref cutDuctPair2, ref curve2, ref pt2);
                ids = new List<ElementId>();
                GetOrderdSysMemberList(ref ids, new List<ElementId>(), cutDuctPair2[0]);
                if (cutDuctPair2.Length == 2)
                {
                    if (ids.Contains(curve1.Id))
                    {
                        curve2 = cutDuctPair2[0];
                        outDuct2 = cutDuctPair2[1];
                    }
                    else
                    {
                        curve2 = cutDuctPair2[1];
                        outDuct2 = cutDuctPair2[0];
                    }
                }

                tran.Commit();
                return true;
            }
        }

        /// <summary>
        /// ダクト/配管切断
        /// </summary>
        /// <param name="cutDuctPair">切断部(区間内側ダクト/配管＆区間外側ダクト/配管)ペア</param>
        /// <param name="duct">切断するダクト/配管</param>
        /// <param name="pt">切断点</param>
        /// <returns></returns>
        protected abstract bool BreakCurve(ref MEPCurve[] cutDuctPair, ref MEPCurve duct, ref XYZ pt);



        #endregion

        #region 【コマンド関連】接続

        #endregion

        #region 【コマンド関連】切断補助線削除
        public bool DeleteCutLines()
        {
            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("DeleteCutLines");
                doc.Delete(sLineId);
                doc.Delete(eLineId);
                sLineId = null;
                eLineId = null;
                tran.Commit();
            }
            return true;
        }

        #endregion

        #region 【コマンド関連】処理エラー対応

        /// <summary>
        /// 処理エラーが出た場合に
        /// 黙ってロールバックする処理
        /// </summary>
        /// <param name="tran">トランザクション</param>
        protected void FailureRollback(Transaction tran)
        {
            FailureHandlingOptions failOpt
                     = tran.GetFailureHandlingOptions();
            // ロールバックした場合に処理エラー表示しない設定
            failOpt.SetClearAfterRollback(true);
            failOpt.SetFailuresPreprocessor(
              new ConnectingFailuresPreprocessor(log));
            tran.SetFailureHandlingOptions(failOpt);
        }
        #endregion

        #region 【図形情報取得】

        /// <summary>
        /// picした図形のobject型を取得する
        /// </summary>
        /// <param name="instructionobj">移動図形もしくは、回避対象物</param>
        /// <param name="obj">取得したobject型</param>
        protected void GetMyClassField(DuctDisplacementDefine.InstructionObj instructionobj, out object obj)
        {
            try
            {
                switch (instructionobj)
                {
                    case DuctDisplacementDefine.InstructionObj.MoveObj_1:
                        obj = curve1;
                        break;
                    case DuctDisplacementDefine.InstructionObj.MoveObj_2:
                        obj = curve2;
                        break;
                    case DuctDisplacementDefine.InstructionObj.TargetObj:
                        obj = target;
                        break;
                    default:
                        //error
                        log.Error("GetMyClassField:miss");
                        obj = null;
                        break;
                }
            }

            catch
            {
                log.Error("GetMyClassField:null");
                obj = null;
            }
        }

        /// <summary>
        /// 図形のファミリネームを取得する
        /// </summary>
        /// <param name="instructionobj">移動図形もしくは、回避対象物</param>
        /// <returns>ファミリネーム</returns>
        public string GetFamilyName(DuctDisplacementDefine.InstructionObj instructionobj)
        {
            string familyname;
            object obj;


            GetMyClassField(instructionobj, out obj);
            if (obj == null)
            {
                //error
                log.Error("GetFamilyName Failed");
                return "";
            }


            if (obj is Duct)
            {
                Duct duct = obj as Duct;
                familyname = duct.DuctType.FamilyName;
            }

            else if (obj is Pipe)
            {
                Pipe pipe = obj as Pipe;
                familyname = pipe.PipeType.FamilyName;
            }

            else if (obj is Element)
            {
                Element el = obj as Element;
                if (el.Category.Id.ToString() == ((int)BuiltInCategory.OST_StructuralFraming).ToString())
                {
                    familyname = ExResources.ResxString(DuctDisplacementDefine.LVL_Beam);
                } 
                else if(el is DirectShape)
                {
                    familyname = ExResources.ResxString(DuctDisplacementDefine.LVL_DIRECT_SHAPE);
                }
                else
                {
                    log.Warn("GetFamilyName Failed");
                    familyname = ExResources.ResxString(DuctDisplacementDefine.LVL_Shape);
                }
            }

            else
            {
                log.Warn("GetFamilyName Failed");
                familyname = ExResources.ResxString(DuctDisplacementDefine.LVL_Shape);
            }

            return familyname;
        }

        /// <summary>
        /// 図形の高さ(Z軸方向に対しての幅)を取得する
        /// </summary>
        /// <param name="instructionobj">移動図形もしくは、回避対象物</param>
        /// <returns>図形の高さ(Z軸方向に対しての幅)</returns>
        /// 
        public double GetHeight(DuctDisplacementDefine.InstructionObj instructionobj)
        {
            object obj;
            double rc = 0;

            GetMyClassField(instructionobj, out obj);
            if (obj == null)
            {
                //error
                log.Error("GetHeight Failed");
            }

            if (obj is Duct)
            {
                Duct duct = obj as Duct;

                Parameter topParam = duct.get_Parameter(BuiltInParameter.RBS_DUCT_TOP_ELEVATION);
                Parameter bottomParam = duct.get_Parameter(BuiltInParameter.RBS_DUCT_BOTTOM_ELEVATION);
                rc = topParam.AsDouble() - bottomParam.AsDouble();
            }

            else if (obj is Pipe)
            {
                Pipe pipe = obj as Pipe;

                Parameter diameterParam = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
                rc = diameterParam.AsDouble();

            }

            else if (obj is Element)
            {
                Element el = obj as Element;
                if (el.Category.Id.ToString() == ((int)BuiltInCategory.OST_StructuralFraming).ToString()
                    || el is DirectShape)
                {
                    var objMinHeight = CastElementIdToBoundingBoxXYZ(target).Min.Z;
                    var objMaxHeight = CastElementIdToBoundingBoxXYZ(target).Max.Z;

                    rc = objMaxHeight - objMinHeight;
                }
                
                else
                {
                    log.Error("Check GetHeight out");
                    rc = 0;
                }

            }
            else
            {
                log.Error("Check GetHeight out");
                rc = 0;
            }
            return rc;
        }

        /// <summary>
        /// BoundingBoxで図形の下面、上面の値を取得する
        /// 注意:ダクトのみで使用すること。配管では使用しないこと
        /// </summary>
        /// <param name="instructionobj">移動図形もしくは、回避対象物</param>
        /// <param name="refline">基準ライン</param>
        /// <param name="box">図形の矩形</param>
        /// <returns>図形の基準面を考慮したZ座標</returns>
        public double GetDuctReflenceLineValue(DuctDisplacementDefine.InstructionObj instructionobj, DuctDisplacementDefine.Line refline, BoundingBoxXYZ box)
        {
            double rc;
            if (refline == DuctDisplacementDefine.Line.Bottom)
            {
                rc = box.Min.Z;
            }

            else if (refline == DuctDisplacementDefine.Line.Top)
            {
                rc = box.Max.Z;
            }

            else
            {
                log.Error("Check refline");
                rc = 0;
            }

            return rc;
        }

        /// <summary>
        /// Glを基準としたZ軸座標を取得する
        /// 単位はft
        /// </summary>
        /// <param name="instructionobj">移動図形もしくは、回避対象物</param>
        /// <returns>Z座標</returns>
        public double GetLocationCurveGLLevel_mm(DuctDisplacementDefine.InstructionObj instructionobj)
        {
            object mepcurve;
            XYZ pt = new XYZ();
            WrpGeometry wrpgeometry = new WrpGeometry(uidoc, log);

            GetMyClassField(instructionobj, out mepcurve);
            if ((mepcurve == null) ||
               (!(mepcurve is Element)))
            {
                //error
                log.Error("GetHeight:Failed");
                return 0;
            }

            if (wrpgeometry.GetLocationCurvePoint(ref pt, (mepcurve as Element), 1))
            {
                return pt.Z;
            }

            else
            {
                //error
                log.Error("GetLocationCurveGLLevel:Failed");
                return 0;
            }
        }

        /// <summary>
        /// オブジェクトの所属しているFLレベルを取得する
        /// </summary>
        /// <param name="instructionobj"></param>
        /// <returns></returns>
        //public double GetBelongFlLevel(DuctDisplacementDefine.InstructionObj instructionobj)
        //{
        //    object mepcurve;
        //    XYZ pt = new XYZ();
        //    WrpViews WrpViews = new WrpViews(uidoc, log);

        //    GetMyClassField(instructionobj, out mepcurve);

        //    if ((mepcurve == null) ||
        //       (!(mepcurve is Element)))
        //    {
        //        //error
        //        log.Error("GetBelongFlLevel:Failed");
        //        return 0;
        //    }
        //    var viewId = (mepcurve as MEPCurve).ReferenceLevel.FindAssociatedPlanViewId();
        //    var el = doc.GetElement(viewId);

        //    if (el is View)
        //    {
        //        return WrpViews.GetViewLevelElevation(el as View);
        //    }

        //    else
        //    {
        //        //error
        //        log.Error("GetBelongFlLevel:Failed");
        //        return 0;
        //    }

        //}

        /// <summary>
        /// 1,2点選択時のアクティブビューのレベルを取得する
        /// </summary>
        /// <returns>FL</returns>
        public double GetActiveViewFlLevel()
        {
            return _view.GetViewLevelElevation(view);
        }

        /// <summary>
        /// 図形の上面または下面のFLからのオフセット値を取得する
        /// </summary>
        /// <param name="instructionobj">移動図形もしくは、回避対象物</param>
        /// <param name="line">基準ライン</param>
        /// <returns>1点選択時のアクティブビューからのFLオフセット値</returns>
        public double GetObjReferenceLevel(DuctDisplacementDefine.InstructionObj instructionobj, DuctDisplacementDefine.Line line)
        {
            object obj;
            double rc;
            WrpViews WrpViews = new WrpViews(uidoc, log);
            BoundingBoxXYZ box;

            GetMyClassField(instructionobj, out obj);

            if (obj == null)
            {
                //error
                log.Error("GetObjLineLevel:Failed");
                return 0;
            }

            if (obj is Duct)
            {
                box = (obj as Duct).get_BoundingBox(null);
                rc = GetDuctReflenceLineValue(instructionobj, line, box) - GetActiveViewFlLevel();
            }

            else if (obj is Pipe)
            {
                LocationCurve curve = (obj as Pipe).Location as LocationCurve;
                Line lines = curve.Curve as Line;
                if (line == DuctDisplacementDefine.Line.Top)
                {
                    rc = lines.Origin.Z - GetActiveViewFlLevel() + (GetHeight(instructionobj) / 2);
                }

                else if (line == DuctDisplacementDefine.Line.Bottom)
                {
                    rc = lines.Origin.Z - GetActiveViewFlLevel() - (GetHeight(instructionobj) / 2);
                }

                else
                {
                    //error
                    log.Error("Check line:" + line);
                    rc = lines.Origin.Z - GetActiveViewFlLevel() + (GetHeight(instructionobj) / 2);
                }

            }

            else if (obj is Element)
            {
                Element el = obj as Element;
                if (el.Category.Id.ToString() == ((int)BuiltInCategory.OST_StructuralFraming).ToString()
                    || el is DirectShape)
                {
                    box = el.get_BoundingBox(null);
                    rc = GetDuctReflenceLineValue(instructionobj, line, box) - GetActiveViewFlLevel();
                }

                else
                {
                    //error
                    log.Error("GetObjLineLevel:not Beam");
                    return 0;
                }
            }

            else
            {
                //error
                log.Error("GetObjLineLevel:Failed");
                return 0;
            }

            return rc;
        }

        /// <summary>
        /// 断熱材の厚さを取得する
        /// </summary>
        /// <param name="instructionobj">移動図形もしくは、回避対象物</param>
        /// <returns>断熱材の厚さ</returns>
        public double GetInsulationMaterialThickness(DuctDisplacementDefine.InstructionObj instructionobj)
        {
            object obj;
            double rc = 0;
            GetMyClassField(instructionobj, out obj);
            if (obj == null)
            {
                //error
                log.Error("GetInsulationMaterialThickness:Failed");
                return 0;
            }

            if (obj is MEPCurve)
            {
                MEPCurve mepcurve = obj as MEPCurve;
                Parameter param = mepcurve.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_THICKNESS);
                rc = param.AsDouble();
            }

            else
            {
                rc = 0;
            }

            return rc;
        }

        #endregion

        #region 【その他】

        /// <summary>
        /// ElementIdからBoundingBoxXYZにキャストする
        /// </summary>
        /// <param name="id">キャストするElementId</param>
        /// <returns></returns>
        public BoundingBoxXYZ CastElementIdToBoundingBoxXYZ(Element el)
        {
            return el.get_BoundingBox(null);
        }


        #endregion

        #region 機械システム

        /// <summary>
        /// document内MechanicalSystemの中身
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public abstract bool ShowMEPSystemMember();

        #endregion

        #endregion

        // プロパティ
        #region Properties
        public MEPCurve Curve1
        {
            get { return curve1; }
        }
        public XYZ Pt1
        {
            get { return pt1; }
        }
        public MEPCurve Curve2
        {
            get { return curve2; }
        }
        public XYZ Pt2
        {
            get { return pt2; }
        }
        public ElementId SLineId
        {
            get { return sLineId; }
        }

        public View view1
        {
            get { return view; }
        }

        public ElementId ELineId
        {
            get { return eLineId; }
        }
        #endregion
    }

    /// <summary>
    /// 処理エラーが出た場合の対応に関するクラス
    /// </summary>
    class ConnectingFailuresPreprocessor
  : IFailuresPreprocessor
    {
        Logger log;
        public ConnectingFailuresPreprocessor(Logger log)
        {
            this.log = log;
        }
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            bool needRollback = false;
            // inside event handler, get all warnings
            IList<FailureMessageAccessor> failures
              = failuresAccessor.GetFailureMessages();

            foreach (FailureMessageAccessor f in failures)
            {
                if (f.GetSeverity().Equals(FailureSeverity.Error) || f.GetSeverity().Equals(FailureSeverity.DocumentCorruption))
                {
                    // 重要度がErrorまたはDocumentCorruptionのものが
                    // 含まれた場合はロールバック(勝手に巻き戻すのでエラー内容をログ出力)
                    needRollback = true;
                    log.Trace(f.GetSeverity().ToString());
                    FailureMessage fMessage = f.CloneFailureMessage();
                    log.Trace(fMessage.GetDescriptionText());
                    log.Trace(f.GetCurrentResolutionType().ToString());
                    log.Trace(f.GetDefaultResolutionCaption().ToString());
                }
            }
            if (needRollback)
            {
                return FailureProcessingResult.ProceedWithRollBack;
            }
            return FailureProcessingResult.Continue;
        }
    }

    public class BranchConnectInfo
    {
        public MEPCurve InDuct
        {
            get; set;
        }
        public MEPCurve OutDuct
        {
            get; set;
        }
        public XYZ BranchCutPt
        {
            get; set;
        }
    }
}
