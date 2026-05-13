using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Mechanical;
using RevitMEPAddin.Common;
using CmdDuctDisplacement.Constant;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using CmdDuctDisplacement.Resource;
using CmdDuctDisplacement.UI.Common;


namespace CmdDuctDisplacement.Logic
{
    public class MEPOperationForPipe : MEPOperation
    {
        #region メンバ変数
        #endregion


        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="log"></param>
        public MEPOperationForPipe(Autodesk.Revit.ApplicationServices.Application app, UIDocument uidoc, Logger log) : base(app, uidoc, log) { }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="log"></param>
        /// <param name="curve1"></param>
        /// <param name="pt1"></param>
        /// <param name="curve2"></param>
        /// <param name="pt2"></param>
        /// <param name="sLineId"></param>
        /// <param name="eLineId"></param>
        /// <param name="view"></param>
        public MEPOperationForPipe(Autodesk.Revit.ApplicationServices.Application app, UIDocument uidoc, Logger log,
            MEPCurve curve1, XYZ pt1, MEPCurve curve2, XYZ pt2,
            ElementId sLineId, ElementId eLineId, Autodesk.Revit.DB.View view) : base(app, uidoc, log)
        {
            this.curve1 = curve1;
            this.pt1 = pt1;
            this.curve2 = curve2;
            this.pt2 = pt2;
            this.sLineId = sLineId;
            this.eLineId = eLineId;
            this.view = view;
        }




        #endregion

        #region メンバ関数

        #region 【コマンド関連】移動量算出

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
        public override Result CalculateDiff(out double hDiff, out double clearance, out double offset,
            int offsetPos, int direction, int roundUnit, double minClear, double insulate)
        {
            // 単位変換
            WrpGeometry _geometry = new WrpGeometry(uidoc, log);

            RoundNum roundnum = new RoundNum();

            double ductHeight = 0; // ダクト位置情報
            double objHeight = 0;  // 高さ基準図形位置情報
            double clearExtra = 0;
            log.Trace("offsetPos:" + offsetPos);
            Pipe pipe = null;
            if (whichSideDuct == DuctDisplacementDefine.START_SIDE)
            {// １点目入力が始点側の場合
                pipe = curve1 as Pipe;
            }
            else if (whichSideDuct == DuctDisplacementDefine.END_SIDE)
            {// １点目入力が終点側の場合
                pipe = curve2 as Pipe;
            }

            Parameter diameterParam = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
            Parameter middleParam = pipe.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM);
            

            double herfDiameterParam = 0.5 * diameterParam.AsDouble();

            //パイプ上面高さを、オフセット+直径の半分で対応
            //パイプ下面高さを、オフセット-直径の半分で対応
            double topParam = middleParam.AsDouble() + herfDiameterParam;
            double bottomParam = middleParam.AsDouble() - herfDiameterParam;

            double hDiffMin = 0;
            switch (direction)
            {
                case DuctDisplacementDefine.DIR_UPPER:
                    // 上方向にかわす場合

                    ductHeight = (double)((decimal)bottomParam - (decimal)_mep.GetInsulationThickness(pipe) + (decimal)pipe.ReferenceLevel.Elevation);
                    log.Trace("RBS_PIPE_BOTTOM_ELEVATION:" + bottomParam);
                    log.Trace("pipe.ReferenceLevel.Elevation:" + pipe.ReferenceLevel.Elevation);
                    // BindingBoxのMaxポイントをグローバル座標系で考える
                    objHeight = GetObjReferenceLevel(DuctDisplacementDefine.InstructionObj.TargetObj, DuctDisplacementDefine.Line.Top);

                    //梁でなければ、耐火被覆厚を無効にする
                    if (needInsulate)
                    {
                        objHeight = (double)((decimal)objHeight + (decimal)_mep.GetInsulationThickness(target) + (decimal)_geometry.ConvertMillimetersToFeet(insulate));// 断熱材と耐火被覆考慮
                    }

                    else
                    {
                        objHeight = (double)((decimal)objHeight + (decimal)_mep.GetInsulationThickness(target));// 断熱材考慮
                    }

                    log.Trace("box.Max.Z:" + GetObjReferenceLevel(DuctDisplacementDefine.InstructionObj.TargetObj, DuctDisplacementDefine.Line.Top));
                    log.Trace("box.Max.Z(transform):" + CastElementIdToBoundingBoxXYZ(target).Transform.OfPoint(CastElementIdToBoundingBoxXYZ(target).Max).Z);
                    // 避ける方向担保のための最小移動量計算
                    hDiffMin = (double)((decimal)objHeight - (decimal)ductHeight);
                    clearExtra = minClear;

                    break;
                case DuctDisplacementDefine.DIR_DOWN:
                    // 下方向にかわす場合

                    ductHeight = (double)((decimal)topParam + (decimal)_mep.GetInsulationThickness(pipe) + (decimal)pipe.ReferenceLevel.Elevation);
                    log.Trace("RBS_PIPE_TOP_ELEVATION:" + topParam);
                    log.Trace("pipe.ReferenceLevel.Elevation:" + pipe.ReferenceLevel.Elevation);
                    // BindingBoxのMinポイントをグローバル座標系で考える
                    objHeight = GetObjReferenceLevel(DuctDisplacementDefine.InstructionObj.TargetObj, DuctDisplacementDefine.Line.Bottom);

                    //梁でなければ、耐火被覆厚を無効にする
                    if (needInsulate)
                    {
                        objHeight = (double)((decimal)objHeight - (decimal)_mep.GetInsulationThickness(target) - (decimal)_geometry.ConvertMillimetersToFeet(insulate));// 保温材厚考慮
                    }

                    else
                    {
                        objHeight = (double)((decimal)objHeight - (decimal)_mep.GetInsulationThickness(target));// 保温材厚考慮
                    }

                    log.Trace("box.Min.Z:" + GetObjReferenceLevel(DuctDisplacementDefine.InstructionObj.TargetObj, DuctDisplacementDefine.Line.Bottom));
                    log.Trace("box.Min.Z(transform):" + CastElementIdToBoundingBoxXYZ(target).Transform.OfPoint(CastElementIdToBoundingBoxXYZ(target).Min).Z);
                    // 避ける方向担保のための最小移動量計算
                    hDiffMin = (double)((decimal)objHeight - (decimal)ductHeight);
                    clearExtra = -minClear;
                    break;
            }


            switch (offsetPos)
            {
                case DuctDisplacementDefine.OFFSET_POS_TOP:
                    // レベルオフセット値算出
                    offset = roundnum.RoundUnnecessaryNum((double)((decimal)_geometry.ConvertFeetToMillimeters(topParam + hDiffMin) + (decimal)clearExtra));
                    Round(ref offset, roundUnit, direction);
                    // 移動量算出
                    hDiff = roundnum.RoundUnnecessaryNum((double)((decimal)offset - (decimal)_geometry.ConvertFeetToMillimeters(topParam)));
                    break;
                case DuctDisplacementDefine.OFFSET_POS_MIDDLE:
                    // レベルオフセット値算出
                    offset = roundnum.RoundUnnecessaryNum((double)((decimal)_geometry.ConvertFeetToMillimeters(middleParam.AsDouble() + hDiffMin) + (decimal)clearExtra));
                    Round(ref offset, roundUnit, direction);
                    // 移動量算出
                    hDiff = roundnum.RoundUnnecessaryNum((double)((decimal)offset - (decimal)_geometry.ConvertFeetToMillimeters(middleParam.AsDouble())));
                    log.Trace("pipeoffset:" + middleParam.AsDouble());
                    break;
                case DuctDisplacementDefine.OFFSET_POS_BOTTOM:
                    // レベルオフセット値算出
                    offset = roundnum.RoundUnnecessaryNum((double)((decimal)_geometry.ConvertFeetToMillimeters(bottomParam + hDiffMin) + (decimal)clearExtra));
                    Round(ref offset, roundUnit, direction);
                    // 移動量算出
                    hDiff = roundnum.RoundUnnecessaryNum((double)((decimal)offset - (decimal)_geometry.ConvertFeetToMillimeters(bottomParam)));
                    break;
                default:
                    log.Error("Check OFFSET_POS");
                    hDiff = 0;
                    offset = 0;
                    break;
            }
            // 離隔算出
            clearance = roundnum.RoundUnnecessaryNum(Math.Abs((double)((decimal)hDiff - (decimal)_geometry.ConvertFeetToMillimeters(hDiffMin))));


            log.Trace("hDiff" + hDiff);
            log.Trace("offset" + offset);
            log.Trace("clearance" + clearance);

            return Result.Succeeded;
        }

        #endregion
        #region【コマンド関連】移動

        /// <summary>
        /// 2点で切断して移動
        /// </summary>
        ///<param name="movePtn">移動方法（オフセット0,レベル統一:1）</param>
        ///<param name="fifPtn">接続方法（45度:0,90度:1,S管:2）</param>
        ///<param name="hDiff">移動距離</param>
        ///<param name="flg">S管ロード済フラグ辞書（角型ダクト:1,円型ダクト:2,楕円型ダクト:3,配管:4）</param>
        ///<param name="message">移動失敗時ダイアログ表示メッセージ</param>
        /// <returns></returns>
        public override bool ModDuctLevelPartially(int movePtn, int fifPtn, double hDiff, ref Dictionary<int, bool> doneSCurveLoad, out string message)
        {

            WrpGeometry _geometry = new WrpGeometry(uidoc, log);
            bool result = true;
            message = null;

            double ductHeight = pt1.Z; // 移動パイプの元の高さ(グローバル)
            double hDiffFeet = _geometry.ConvertMillimetersToFeet(hDiff);// 移動量(フィート)

            // 区間指定箇所で切断
            MEPCurve[] cutDuctPair1 = null;
            MEPCurve[] cutDuctPair2 = null;
            CutSpecifiedDuctNetworkSection(ref cutDuctPair1, ref cutDuctPair2);

            // 枝分かれ部の処置
            Dictionary<ElementId, ElementId> cutDuctPairs = new Dictionary<ElementId, ElementId>();
            List<ElementId> sysMemberIds = new List<ElementId>();
            List<BranchConnectInfo> branchConnectInfoList = new List<BranchConnectInfo>();

            //1パイプ内の句化に同の場合は、パイプルートによる分岐以外に間に存在しない。
            //複数にまたがる場合にのみ分岐切断を行う。
            if (NeedBranchCut())
            {
                if(!CutBranchCurve(ref branchConnectInfoList, ref sysMemberIds, curve1, curve2))
                {
                    message = ExResources.ResxString(DuctDisplacementDefine.MSG_ERROR3);
                    return false;
                }
            }

            

            if (movePtn == (int)DuctDisplacementDefine.MOVE_PTN.OFFSET)
            {
                // **************
                // オフセット移動
                // **************

                // 移動
                if (!MovePipingSystem(hDiff, ref branchConnectInfoList))
                {
                    message = ExResources.ResxString(DuctDisplacementDefine.MSG_ERROR1);
                    return false;
                }
                // 指定切断部の接続
                result = ConnectCurves(curve1, outDuct1, pt1, fifPtn, hDiff, ref doneSCurveLoad);
                result = result && ConnectCurves(curve2, outDuct2, pt2, fifPtn, hDiff, ref doneSCurveLoad);

                // 枝分かれ部接続
                foreach (BranchConnectInfo info in branchConnectInfoList)
                {
                    ConnectCurves((Pipe)info.InDuct, (Pipe)info.OutDuct, info.BranchCutPt, fifPtn, hDiff, ref doneSCurveLoad);
                }
                if (result == false)
                {
                    message = ExResources.ResxString(DuctDisplacementDefine.MSG_ERROR2);
                }
                return result;
            }
            else if (movePtn == (int)DuctDisplacementDefine.MOVE_PTN.UNIFIEDLVEL)
            {
                // **************
                // レベル統一移動
                // **************
                // 指定レベルに機械システムを作成
                //UnifyLevel(systemId, ductHeight, hDiffFeet);
                //TODO 接続★

                return true;
            }

            return false;
        }

        /// <summary>
        /// 分岐の切断（CutBranchCurve)を実施する必要があるかチェック
        /// </summary>
        /// <returns></returns>
        private bool NeedBranchCut()
        {
            bool needBranchCut = false;
            if (!curve1.Id.Equals(curve2.Id))
            {
                needBranchCut = true;
            }
            else
            {
                ConnectorSet cons = null;
                _mep.GetConnectorsFromMSystemMember(ref cons, curve1);
                foreach (Connector con in cons)
                {
                    foreach (Connector acon in con.AllRefs)
                    {
                        {
                            if (acon.Owner.Category.Id.Equals(Category.GetCategory(doc, BuiltInCategory.OST_PlaceHolderPipes).Id))
                            {
                                if (_geometry.IsBetweenTwoPoints(pt1, pt2, acon.Origin))
                                {
                                    needBranchCut = true;
                                    break;
                                }
                            }
                            if (needBranchCut) break;
                        }
                    }
                }
            }
            return needBranchCut;
        }

        /// <summary>
        /// (2019/06/18 今回は対応なし。)
        /// 切断部のパイプが区間に含まれるかを判定し、
        /// 区間内側と外側のパイプの（再）セットを行う
        /// </summary>
        /// <param name="cutDuctPair">切断部両側のパイプID</param>
        /// <param name="systemId">配管システムID</param>
        /// <param name="inDuct"></param>
        /// <param name="outDuct"></param>
        /// <returns></returns>
        protected override bool ResetInOutDuct(MEPCurve[] cutDuctPair, ElementId mSystemId, ref MEPCurve inDuct, ref MEPCurve outDuct)
        {

            foreach (Pipe pipe in cutDuctPair)
            {
                if (_mep.IncludedPipeInPSystem(pipe.Id, mSystemId))
                {
                    inDuct = pipe;
                    log.Trace("システム内側：" + pipe.Id + "mSystemID:" + mSystemId);
                }
                else
                {
                    outDuct = pipe;
                    log.Trace("システム外側：" + pipe.Id);
                }
            }
            return true;
        }


        /// <summary>
        /// (2019/06/18 今回は対応なし)
        /// レベル統一
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="ductHeight"></param>
        /// <param name="hDiffFeet"></param>
        /// <returns></returns>
        protected override bool UnifyLevel(ElementId systemId, double ductHeight, double hDiffFeet)
        {
            using (Transaction tran = new Transaction(doc))
            {
                tran.Start("UnifyLevel");
                // 切断区間の機械システムに関する情報
                PipingSystem pSystem = doc.GetElement(systemId) as PipingSystem;
                ElementSet pipeNetwork = pSystem.PipingNetwork;
                // パイピングネットワークを順に並べたリスト
                List<ElementId> orderedPipeNetwork = new List<ElementId>();
                if (whichSideDuct == DuctDisplacementDefine.START_SIDE)
                {// １点目入力が始点側の場合
                    GetOrderdSysMemberList(ref orderedPipeNetwork, new List<ElementId>(), curve1);
                }
                else if (whichSideDuct == DuctDisplacementDefine.END_SIDE)
                {// １点目入力が終点側の場合
                    GetOrderdSysMemberList(ref orderedPipeNetwork, new List<ElementId>(), curve2);
                }

                // 旧新パイプID辞書
                Dictionary<ElementId, ElementId> ductDict = new Dictionary<ElementId, ElementId>();
                // つぶれた部分を無視して接続するための情報を保持したい。
                List<List<Connector>> connectingList = new List<List<Connector>>();


                // レベル統一後のパイプを作成
                foreach (Element e in pipeNetwork)
                {
                    if (!(e is Pipe)) continue;

                    Pipe pipe = e as Pipe;
                    XYZ sPt = null;
                    XYZ ePt = null;
                    _geometry.GetLocationCurveStartPoint(ref sPt, pipe);
                    _geometry.GetLocationCurveEndPoint(ref ePt, pipe);
                    sPt = sPt.Add(new XYZ(0, 0, -sPt.Z + ductHeight + hDiffFeet));
                    ePt = ePt.Add(new XYZ(0, 0, -ePt.Z + ductHeight + hDiffFeet));
                    if (!sPt.IsAlmostEqualTo(ePt))
                    {
                        Pipe newPipe = Pipe.Create(doc, pSystem.GetTypeId(), pipe.PipeType.Id, pipe.ReferenceLevel.Id, sPt, ePt);
                        // 形状に関するパラメータをコピーする
                        // 幅
                        _mep.CopyParam(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM, pipe, newPipe);
                        // 高さ
                        _mep.CopyParam(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM, pipe, newPipe);
                        // 直径
                        _mep.CopyParam(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM, pipe, newPipe);
                        //TODO 耐火被覆は？★

                        ductDict.Add(pipe.Id, newPipe.Id);
                    }
                    else
                    {
                        // 垂直パイプの場合（レベル統一でつぶれる）
                        // つぶれた部分を無視して接続するための情報を保持したい。

                        List<Connector> conList = new List<Connector>();
                        _mep.GetConnectionsWithSpMEPElmRef(pipe, ref conList);
                        if (conList.Count == 2)
                        {
                            connectingList.Add(conList);
                        }
                    }
                }
                // レベル統一前にトランジションだった部分の接続
                foreach (Element e in pipeNetwork)
                {
                    if (e is Pipe) continue;
                    FamilyInstance instance = e as FamilyInstance;
                    LocationPoint lPt = instance.Location as LocationPoint;
                    MechanicalFitting fitting = instance.MEPModel as MechanicalFitting;
                    if (fitting.PartType != PartType.Transition) continue;
                    // トランジションはパイプ
                    // トランジションの接続先エレメントのコネクション取得
                    Pipe tranPipe = null;
                    List<Connector> conList = new List<Connector>();
                    _mep.GetConnectionsWithSpMEPElmRef(instance, ref conList);
                    foreach (Connector con in conList)
                    {
                        if (!(con.Owner is Pipe)) continue;
                        if (tranPipe != null && !con.Origin.IsAlmostEqualTo(lPt.Point)) continue;
                        // pipeType == nullの時と、そうでないけど始点に近い方もパイプだった場合
                        tranPipe = con.Owner as Pipe;

                    }
                    //if(tranDuct != null)
                    //{
                    //    // Transition分延長

                    //    // 辞書には延長したパイプで記載
                    //    ductDict.Add(e.Id, ductDict[tranDuct.Id]);
                    //}
                    //else 
                    if (tranPipe == null)
                    {
                        int idx = orderedPipeNetwork.FindIndex(n => n == e.Id);
                        int i = 2;
                        Element elm = null;
                        while (idx + i < orderedPipeNetwork.Count || idx - i > -1)
                        {
                            if (idx - i > -1)
                            {
                                elm = doc.GetElement(orderedPipeNetwork[idx - i]);
                                if (elm is Pipe)
                                {
                                    tranPipe = elm as Pipe;
                                    break;
                                }
                            }
                            if (idx + i < orderedPipeNetwork.Count)
                            {
                                elm = doc.GetElement(orderedPipeNetwork[idx + i]);
                                if (elm is Pipe)
                                {
                                    tranPipe = elm as Pipe;
                                    break;
                                }
                            }
                            i++;
                        }
                    }
                    if (tranPipe != null && conList.Count == 2)
                    {

                        XYZ pt0 = (conList[0].Origin).Add(new XYZ(0, 0, -(conList[0].Origin).Z + ductHeight + hDiffFeet));
                        XYZ pt1 = (conList[1].Origin).Add(new XYZ(0, 0, -(conList[1].Origin).Z + ductHeight + hDiffFeet));
                        Pipe newPipe = null;
                        if (lPt.Point.IsAlmostEqualTo(pt0))
                        {
                            newPipe = Pipe.Create(doc, pSystem.GetTypeId(), tranPipe.PipeType.Id, tranPipe.ReferenceLevel.Id, pt0, pt1);
                        }
                        else
                        {
                            newPipe = Pipe.Create(doc, pSystem.GetTypeId(), tranPipe.PipeType.Id, tranPipe.ReferenceLevel.Id, pt1, pt0);
                        }
                        // 形状に関するパラメータをコピーする
                        // 幅
                        _mep.CopyParam(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM, tranPipe, newPipe);
                        // 高さ
                        _mep.CopyParam(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM, tranPipe, newPipe);
                        // 直径
                        _mep.CopyParam(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM, tranPipe, newPipe);
                        //TODO 耐火被覆は？★
                        ductDict.Add(e.Id, newPipe.Id);
                    }
                }
                // レベル統一前にエルボだった部分の接続
                foreach (Element e in pipeNetwork)
                {
                    if (e is Pipe) continue;
                    FamilyInstance instance = e as FamilyInstance;
                    MechanicalFitting fitting = instance.MEPModel as MechanicalFitting;
                    LocationPoint lPt = instance.Location as LocationPoint;
                    if (fitting.PartType != PartType.Elbow) continue;

                    // エルボの接続先エレメントのコネクション取得
                    List<Connector> conList = new List<Connector>();
                    _mep.GetConnectionsWithSpMEPElmRef(instance, ref conList);

                    if (conList.Count == 2)
                    {
                        if (!ductDict.ContainsKey(conList[0].Owner.Id))
                        {
                            if (!ductDict.ContainsKey(conList[1].Owner.Id)) continue;
                            // 垂直パイプに接続していて消えるときくらい？
                            Pipe pipe = doc.GetElement(ductDict[conList[1].Owner.Id]) as Pipe;

                            XYZ pt0 = (conList[0].Origin).Add(new XYZ(0, 0, -(conList[0].Origin).Z + ductHeight + hDiffFeet));
                            XYZ pt1 = (conList[1].Origin).Add(new XYZ(0, 0, -(conList[1].Origin).Z + ductHeight + hDiffFeet));
                            Pipe newPipe = null;
                            XYZ pt = null;
                            _geometry.GetLocationCurveStartPoint(ref pt, pipe);
                            if (lPt.Point.IsAlmostEqualTo(pt1))
                            {
                                newPipe = Pipe.Create(doc, pSystem.GetTypeId(), pipe.PipeType.Id, pipe.ReferenceLevel.Id, pt1, pt0);
                            }
                            else
                            {
                                newPipe = Pipe.Create(doc, pSystem.GetTypeId(), pipe.PipeType.Id, pipe.ReferenceLevel.Id, pt0, pt1);
                            }
                            // 形状に関するパラメータをコピーする
                            // 幅
                            _mep.CopyParam(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM, pipe, newPipe);
                            // 高さ
                            _mep.CopyParam(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM, pipe, newPipe);
                            // 直径
                            _mep.CopyParam(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM, pipe, newPipe);
                            //TODO 耐火被覆は？★
                            ductDict.Add(e.Id, newPipe.Id);

                        }
                        else if (!ductDict.ContainsKey(conList[1].Owner.Id))
                        {
                            if (!ductDict.ContainsKey(conList[0].Owner.Id)) continue;
                            // 垂直パイプに接続していて消えるときくらい？

                            Pipe pipe = doc.GetElement(ductDict[conList[0].Owner.Id]) as Pipe;

                            XYZ pt0 = (conList[0].Origin).Add(new XYZ(0, 0, -(conList[0].Origin).Z + ductHeight + hDiffFeet));
                            XYZ pt1 = (conList[1].Origin).Add(new XYZ(0, 0, -(conList[1].Origin).Z + ductHeight + hDiffFeet));
                            Pipe newPipe = null;
                            XYZ pt = null;
                            _geometry.GetLocationCurveStartPoint(ref pt, pipe);
                            if (lPt.Point.IsAlmostEqualTo(pt0))
                            {
                                newPipe = Pipe.Create(doc, pSystem.GetTypeId(), pipe.PipeType.Id, pipe.ReferenceLevel.Id, pt0, pt1);
                            }
                            else
                            {
                                newPipe = Pipe.Create(doc, pSystem.GetTypeId(), pipe.PipeType.Id, pipe.ReferenceLevel.Id, pt1, pt0);
                            }
                            // 形状に関するパラメータをコピーする
                            // 幅
                            _mep.CopyParam(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM, pipe, newPipe);
                            // 高さ
                            _mep.CopyParam(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM, pipe, newPipe);
                            // 直径
                            _mep.CopyParam(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM, pipe, newPipe);
                            //TODO 耐火被覆は？★
                            ductDict.Add(e.Id, newPipe.Id);
                        }
                        else
                        {

                            // memo 接続先がパイプ以外（トランジション）であっても、パイプに変換しているので、
                            //      下記、MEPCurveでも大丈夫のはず。
                            MEPCurve tCurve1 = doc.GetElement(ductDict[conList[0].Owner.Id]) as MEPCurve;
                            MEPCurve tCurve2 = doc.GetElement(ductDict[conList[1].Owner.Id]) as MEPCurve;

                            double angle = 0;
                            _mep.AngleBtwMEPCurves(tCurve1, tCurve2, ref angle);
                            if (_geometry.NearlyEquals(angle, 0) || _geometry.NearlyEquals(angle, Math.PI))
                            {
                                //LocationPoint lPt = instance.Location as LocationPoint;
                                //Connector con1 = _mep.FindConnector(tCurve1, new XYZ(lPt.Point.X, lPt.Point.Y, ));
                                //Connector con2 = _mep.FindConnector(curve2, lPt.Point);

                                Connector sCon1 = _mep.GetStartSideConnector(tCurve1);
                                Connector eCon1 = _mep.GetEndSideConnector(tCurve1);
                                Connector sCon2 = _mep.GetStartSideConnector(tCurve2);
                                Connector eCon2 = _mep.GetEndSideConnector(tCurve2);
                                Pipe mPipe = null;

                                if (eCon1.Origin.DistanceTo(sCon2.Origin)
                                    < eCon2.Origin.DistanceTo(sCon1.Origin))
                                {
                                    if (eCon1.Origin.DistanceTo(sCon2.Origin) == 0)
                                    {
                                        eCon1.ConnectTo(sCon2);
                                        log.Trace("接続します：" + tCurve1.Id + ", " + tCurve2.Id);
                                    }
                                    else
                                    {
                                        mPipe = Pipe.Create(doc, tCurve1.GetTypeId(), tCurve1.ReferenceLevel.Id, eCon1, sCon2);
                                        //eCon1.ConnectTo(_mep.GetStartSideConnector(mDuct));
                                        //sCon2.ConnectTo(_mep.GetEndSideConnector(mDuct));
                                    }

                                }
                                else if (eCon1.Origin.DistanceTo(sCon2.Origin)
                                    > eCon2.Origin.DistanceTo(sCon1.Origin))
                                {
                                    if (eCon1.Origin.DistanceTo(sCon2.Origin) == 0)
                                    {
                                        eCon2.ConnectTo(sCon1);
                                        log.Trace("接続します：" + tCurve1.Id + ", " + tCurve2.Id);
                                    }
                                    else
                                    {
                                        mPipe = Pipe.Create(doc, tCurve2.GetTypeId(), tCurve1.ReferenceLevel.Id, eCon2, sCon1);
                                        //eCon2.ConnectTo(_mep.GetStartSideConnector(mDuct));
                                        //sCon1.ConnectTo(_mep.GetEndSideConnector(mDuct));
                                    }
                                }

                            }
                            else
                            {
                                // エルボで接続
                                FamilyInstance elbow = null;
                                _mep.ConnectMEPCurveWithElbow(tCurve1, tCurve2, ref elbow);
                            }
                        }
                    }
                }
                // レベル統一前に垂直パイプだった部分の接続
                {
                    //foreach (List<Connector> list in connectingList)
                    //{
                    //    Duct curve1 = doc.GetElement(ductDict[list[0].Owner.Id]) as Duct;
                    //    Duct curve2 = doc.GetElement(ductDict[list[0].Owner.Id]) as Duct;
                    //    Connector con1 = _mep.FindConnector(curve1, list[0].Origin);
                    //    Connector con2 = _mep.FindConnector(curve2, list[1].Origin);
                    //    con1.ConnectTo(con2);
                    //    log.Trace("垂直ダクトがなくなった部分接続する");
                    //}
                }
                tran.Commit();
            }

            return true;
        }

        /// <summary>
        /// 切断点に続かない分岐は
        /// 最初に出現したジェネリックなダクト/配管の真ん中で切断する。
        /// </summary>
        /// <param name="branchConnectInfoLis">分岐切断情報（切断点・内側ダクト/配管・外側ダクト/配管）リスト</param>
        /// <param name="sysMemberIds">※再起呼び出し時利用</param>
        /// <param name="sysMember">接続をたどる開始位置エレメント</param>
        /// <param name="ductE">反対側の切断点を含むダクト/配管</param>
    
        /// <returns></returns>
        public override bool CutBranchCurve(ref List<BranchConnectInfo> branchConnectInfoList, ref List<ElementId> sysMemberIds, Element sysMember, MEPCurve ductE)
        {
            if (!sysMemberIds.Contains(sysMember.Id))
            {
                sysMemberIds.Add(sysMember.Id);
            }
            // コネクターたちを取得
            ConnectorSet cons = null;
            _mep.GetConnectorsFromMSystemMember(ref cons, sysMember);

            foreach (Connector con in cons)
            {
                // このElementがもつコネクタと接続するエレメントたちを走査
                ConnectorSet refCons = con.AllRefs;
                foreach (Connector refCon in refCons)
                {

                    if (!sysMemberIds.Contains(refCon.Owner.Id) && !(refCon.Owner is PipingSystem)
                                                                    /* systemId != refCon.Owner.Id */)
                    {
                        List<ElementId> ids = new List<ElementId>();
                        GetOrderdSysMemberList(ref ids, sysMemberIds, refCon.Owner);
                        if (ids.Contains(ductE.Id))
                        {
                            log.Trace("（区間）refコネクタ（オーナー）：" + refCon.Owner.Id);
                            // 再帰呼び出し
                            if (!CutBranchCurve(ref branchConnectInfoList, ref sysMemberIds, refCon.Owner, ductE)) return false;
                        }
                        else
                        {
                            // その先に終点側切断箇所を含まない枝分かれは
                            // その最初のパイプの中点で切断
                            ElementId cutDuctId = ids.Find(id => doc.GetElement(id) is Pipe
                            && !doc.GetElement(id).Category.Id.Equals(Category.GetCategory(doc, BuiltInCategory.OST_PlaceHolderPipes).Id));
                            　　

                            if (cutDuctId == null)
                            {
                                ElementId ductTerminalId = ids.Find(id => doc.GetElement(id) is FamilyInstance
                                                  && (doc.GetElement(id).Category.Id.Equals(Category.GetCategory(doc, BuiltInCategory.OST_PlumbingFixtures).Id)
                                                  || (doc.GetElement(id).Category.Id.Equals(Category.GetCategory(doc, BuiltInCategory.OST_Sprinklers).Id))));
                                // 基本的に切断して移動するけど、切断できない場合衛生器具・スプリンクラにつながっていなければ、
                                // 移動不可とはしない
                                if (ductTerminalId == null) continue;
                                return false;
                            }



                            using (Transaction tran = new Transaction(doc))
                            {
                                tran.Start("CutBranchCurve");
                                log.Trace("[枝分かれ切断]ダクトID:" + cutDuctId);

                                // 中点取得
                                Pipe cutPipe = doc.GetElement(cutDuctId) as Pipe;
                                XYZ middlePt = null;
                                _geometry.GetLocationCurvePoint(ref middlePt, cutPipe, 2);
                                // パイプを中点でカット
                                ElementId newDuctId = PlumbingUtils.BreakCurve(doc, cutDuctId, middlePt);

                                // あとでつなぐために切断パイプ情報を保持
                                BranchConnectInfo info = new BranchConnectInfo();
                                info.BranchCutPt = middlePt;

                                List<ElementId> connectedIds = new List<ElementId>();
                                GetOrderdSysMemberList(ref connectedIds, new List<ElementId>(), doc.GetElement(newDuctId) as MEPCurve);
                                if (connectedIds.Contains(curve1.Id))
                                {
                                    info.InDuct = doc.GetElement(newDuctId) as MEPCurve;
                                    info.OutDuct = doc.GetElement(cutDuctId) as MEPCurve;
                                    log.Trace("ブランチ内側パイプ：" + newDuctId.ToString());
                                    log.Trace("ブランチ外側パイプ：" + cutDuctId.ToString());
                                }
                                else
                                {
                                    info.InDuct = doc.GetElement(cutDuctId) as MEPCurve;
                                    info.OutDuct = doc.GetElement(newDuctId) as MEPCurve;
                                    log.Trace("ブランチ内側パイプ：" + cutDuctId.ToString());
                                    log.Trace("ブランチ外側パイプ：" + newDuctId.ToString());
                                }
                                branchConnectInfoList.Add(info);

                                tran.Commit();
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 切断指定部の接続
        /// </summary>
        /// <param name="duct">区間内パイプ</param>
        /// <param name="outDuct">区間外パイプ</param>
        /// <param name="pt">切断点</param>
        /// <param name="fifPtn">接続方法（45度:0,90度:1,S管:2）</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="doneSCurveLoad">S管ロード済フラグ辞書(角型ダクト:1,円型ダクト:2,楕円型ダクト:3,配管:4)</param>
        /// <returns></returns>
        public override bool ConnectCurves(MEPCurve duct, MEPCurve outDuct, XYZ pt, int fifPtn, double hDiff, ref Dictionary<int, bool> doneSCurveLoad)
        {
            bool result = true;
            if (outDuct == null)
            {
                log.Info("切断していないので接合しない。");
                return true;
            }
            //using(Transaction tran = new Transaction(doc))
            //{
            //tran.Start("ConnectDucts");
            XYZ sPt = null;
            _geometry.GetLocationCurveStartPoint(ref sPt, duct);
            bool isStartSide = sPt.IsAlmostEqualTo(pt);
            log.Trace("始点：" + sPt.ToString());
            log.Trace("切断点：" + pt.ToString());


            if (isStartSide)
            {
                log.Info("始点側パターンの接続");
                // ptが始点側の場合
                switch (fifPtn)
                {
                    case (int)DuctDisplacementDefine.FITTING_PTN.deg90:
                        result = PartialDuctOperationStartSideElbo90((Pipe)outDuct, (Pipe)duct, hDiff, fifPtn, ref doneSCurveLoad);                                
                        break;
                    case (int)DuctDisplacementDefine.FITTING_PTN.deg45:
                        result = PartialDuctOperationStartSideElbo45((Pipe)outDuct, (Pipe)duct, hDiff, fifPtn, ref doneSCurveLoad);
                        break;
                    case (int)DuctDisplacementDefine.FITTING_PTN.S:
                        result = PartialDuctOperationStartSideSCurve((Pipe)outDuct, (Pipe)duct, hDiff, fifPtn, ref doneSCurveLoad);
                        break;
                }
            }
            else
            {
                log.Info("終点側パターンの接続");
                // ptが始点側の場合
                switch (fifPtn)
                {
                    case (int)DuctDisplacementDefine.FITTING_PTN.deg90:
                        result = PartialDuctOperationEndSideElbo90((Pipe)duct, (Pipe)outDuct, hDiff, fifPtn, ref doneSCurveLoad);
                        break;
                    case (int)DuctDisplacementDefine.FITTING_PTN.deg45:
                        result = PartialDuctOperationEndSideElbo45((Pipe)duct, (Pipe)outDuct, hDiff, fifPtn, ref doneSCurveLoad);
                        break;
                    case (int)DuctDisplacementDefine.FITTING_PTN.S:
                        result = PartialDuctOperationEndSideSCurve((Pipe)duct, (Pipe)outDuct, hDiff, fifPtn, ref doneSCurveLoad);
                        break;
                }
            }

            //tran.Commit();
            //}
            return result;
        }

        /// <summary>
        /// パイプ切断
        /// </summary>
        /// <param name="cutDuctPair">切断部(区間内側パイプ＆区間外側パイプ)ペア</param>
        /// <param name="duct">切断するパイプ</param>
        /// <param name="pt">切断点</param>
        /// <returns></returns>
        protected override bool BreakCurve(ref MEPCurve[] cutDuctPair, ref MEPCurve duct, ref XYZ pt)
        {
            // ptでのパイプカット要否判定
            LocationCurve lCurve = duct.Location as LocationCurve;
            if (lCurve == null) return false;
            if (lCurve.Curve.GetEndPoint(0).IsAlmostEqualTo(pt))
            {
                cutDuctPair = new MEPCurve[1] { duct };

                Connector sCon = _mep.GetStartSideConnector(duct);
                //foreach (Connector con in sCon.AllRefs)
                //{
                //    // システムの端っこであれば、切断しない
                //    if (con.Owner is PipingSystem)
                //    {
                //        log.Trace("システム端のパイプ端点での切断指定のため、切断しない");
                //        return true;
                //    }

                //}
                foreach (Connector con in sCon.AllRefs)
                {
                    log.Trace(con.Owner.Name);
                    // システム途中のパイプ端点であれば、コネクタの接続を解除
                    if (!con.Owner.Id.Equals(sCon.Owner.Id) && !(con.Owner is PipingSystem))
                    {
                        sCon.DisconnectFrom(con);
                        log.Trace("パイプ端点での切断指定のため、コネクタを解除:" + sCon.Owner.Id + ":" + con.Owner.Id);
                    }
                }

            }
            else if (lCurve.Curve.GetEndPoint(1).IsAlmostEqualTo(pt))
            {
                cutDuctPair = new MEPCurve[1] { duct };

                Connector eCon = _mep.GetEndSideConnector(duct);
                //foreach (Connector con in eCon.AllRefs)
                //{
                //    // システムの端っこであれば、切断しない
                //    if (con.Owner is PipingSystem)
                //    {
                //        log.Trace("システム端のパイプ端点での切断指定のため、切断しない");
                //        return true;
                //    }
                //}
                foreach (Connector con in eCon.AllRefs)
                {
                    // システム途中のパイプ端点であれば、コネクタの接続を解除
                    if (!con.Owner.Id.Equals(eCon.Owner.Id) && !(con.Owner is PipingSystem))
                    {
                        eCon.DisconnectFrom(con);
                        log.Trace("パイプ端点での切断指定のため、コネクタを解除:" + eCon.Owner.Id + ":" + con.Owner.Id);
                    }
                }
            }
            else
            {
                // ptでパイプ切断実施
                // ※ブレーク後のパイプの並び(s/~/newduct/duct/~/e)
                log.Trace("duct:" + duct.Id + "  point:" + pt);
                ElementId newDuctId = PlumbingUtils.BreakCurve(doc, duct.Id, pt);
                Pipe newPipe = doc.GetElement(newDuctId) as Pipe;
                cutDuctPair = new MEPCurve[2] { duct, newPipe };
                log.Trace("newDuctId:" + cutDuctPair[1].Id);
            }

            return true;
        }



        #endregion

        #region 【コマンド関連】接続
        /// <summary>
        /// 90度返し選択時処理(始点側)
        /// </summary>
        /// <param name="outPipe">区間外パイプ</param>
        /// <param name="pipe">区間内パイプ</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="fifPtn">接続方法(45度:0,90度:1,S管:2)</param>
        /// <param name="doneSCurveLoad">S管ロード済フラグ辞書(角型ダクト:1,円型ダクト:2,楕円型ダクト:3,配管:4)</param>
        /// <returns></returns>
        public bool PartialDuctOperationStartSideElbo90(Pipe outPipe, Pipe pipe, double hDiff, int fifPtn, ref Dictionary<int, bool> doneSCurveLoad)
        {
            bool result = true;
            using (Transaction tran = new Transaction(doc))
            {
                try
                {

                    tran.Start("PartialDuctOperationStartSideElbo90");
                    // 処理エラーが出た場合に何も言わずにロールバック
                    FailureRollback(tran);
                    _mep.ConnectPipeStartSide(outPipe, pipe, true);
                    tran.Commit();

                    // 処理エラーでロールバックの場合
                    TransactionStatus tranStatus = tran.GetStatus();
                    if (tranStatus == TransactionStatus.RolledBack)
                    {
                        log.Info("処理エラーでロールバック");
                        result = PartialDuctOperationStartSideElbo90Alt(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                    }
                }
                catch (Autodesk.Revit.Exceptions.ApplicationException ex)
                {
                    tran.RollBack();
                    log.Error(ex.Message);
                    result = PartialDuctOperationStartSideElbo90Alt(outPipe, pipe, hDiff, fifPtn , ref doneSCurveLoad);
                }
            }

            return result;
        }

        /// <summary>
        /// 90度返し選択時処理(終点側)
        /// </summary>
        /// <param name="pipe">区間内パイプ</param>
        /// <param name="outPipe">区間外パイプ</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="fifPtn">接続方法(45度:0,90度:1,S管:2)</param>
        /// <param name="doneSCurveLoad">S管ロード済フラグ辞書(角型ダクト:1,円型ダクト:2,楕円型ダクト:3,配管:4)</param>
        /// <returns></returns>
        public bool PartialDuctOperationEndSideElbo90(Pipe pipe, Pipe outPipe, double hDiff, int fifPtn, ref Dictionary<int, bool> doneSCurveLoad)
        {
            bool result = true;
            using (Transaction tran = new Transaction(doc))
            {
                try
                {
                    tran.Start("PartialDuctOperationEndSideElbo90");
                    // 処理エラーが出た場合に何も言わずにロールバック
                    FailureRollback(tran);
                    _mep.ConnectPipeEndSide(pipe, outPipe, true);
                    tran.Commit();

                    // 処理エラーでロールバックの場合
                    TransactionStatus tranStatus = tran.GetStatus();
                    if (tranStatus == TransactionStatus.RolledBack)
                    {
                        log.Info("処理エラーでロールバック");
                        result = PartialDuctOperationEndSideElbo90Alt(pipe, outPipe, hDiff, fifPtn , ref doneSCurveLoad);
                    }
                }
                catch (Autodesk.Revit.Exceptions.ApplicationException ex)
                {
                    tran.RollBack();
                    log.Error(ex.Message);
                    result = PartialDuctOperationEndSideElbo90Alt(pipe, outPipe, hDiff, fifPtn , ref doneSCurveLoad);
                }
            }
            return result;
        }

        /// <summary>
        /// 45度返し選択時処理(始点側)
        /// </summary>
        /// <param name="outPipe">区間外パイプ</param>
        /// <param name="pipe">区間内パイプ</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="fifPtn">接続方法(45度:0,90度:1,S管:2)</param>
        /// <param name="doneSCurveLoad">S管ロード済フラグ辞書(角型ダクト:1,円型ダクト:2,楕円型ダクト:3,配管:4)</param>
        /// <returns></returns>
        public bool PartialDuctOperationStartSideElbo45(Pipe outPipe, Pipe pipe, double hDiff, int fifPtn, ref Dictionary<int, bool> doneSCurveLoad)
        {
            WrpGeometry _geometry = new WrpGeometry(uidoc, log);
            bool result = true;

            using (Transaction tran = new Transaction(doc))
            {
                try
                {
                    tran.Start("PartialDuctOperationStartSideElbo45");
                    // 処理エラーが出た場合に何も言わずにロールバック
                    FailureRollback(tran);

                    //**********************
                    // outDuctの不要部分切断
                    //**********************
                    XYZ sPt1 = new XYZ();
                    _geometry.GetLocationCurveStartPoint(ref sPt1, outPipe);
                    XYZ ePt1 = new XYZ();
                    _geometry.GetLocationCurveEndPoint(ref ePt1, outPipe);

                    // 切断点を求める
                    double scalar = Math.Abs(_geometry.ConvertMillimetersToFeet(hDiff)) / sPt1.DistanceTo(ePt1);
                    log.Trace("スカラー：" + scalar);

                    if (scalar <= 0 || scalar >= 1) // 端点もダメなので "=" も排除。
                    {
                        log.Info("45°配置はできません。");
                        tran.Dispose();
                        // 変わりの配置
                        result = PartialDuctOperationStartSideElbo45Alt(outPipe, pipe, hDiff, fifPtn , ref doneSCurveLoad);
                        return result;
                    }
                    XYZ breakPt1 = ePt1.Add((sPt1.Subtract(ePt1)).Multiply(scalar));
                    log.Trace("切断点：" + breakPt1.ToString());

                    // outDuctを短縮する
                    Connector eConOutDuct = _mep.GetEndSideConnector(outPipe);
                    eConOutDuct.Origin = breakPt1;

                    // newDuct, ductを接続
                    _mep.ConnectPipeStartSide(outPipe, pipe, false);

                    tran.Commit();

                    // 処理エラーでロールバックの場合
                    TransactionStatus tranStatus = tran.GetStatus();
                    if (tranStatus == TransactionStatus.RolledBack)
                    {
                        log.Info("処理エラーでロールバック");
                        result = PartialDuctOperationStartSideElbo45Alt(outPipe, pipe, hDiff, fifPtn , ref doneSCurveLoad);
                    }
                }
                catch (Autodesk.Revit.Exceptions.ApplicationException ex)
                {
                    tran.RollBack();
                    log.Error(ex.Message);
                    // (接続方法を変更して)変わりの接続
                    result = PartialDuctOperationStartSideElbo45Alt(outPipe, pipe, hDiff, fifPtn , ref doneSCurveLoad);
                }
            }

            return result;
        }


        /// <summary>
        /// 45度返し選択時処理(終点側)
        /// </summary>
        /// <param name="pipe">区間内パイプ</param>
        /// <param name="outPipe">区間外パイプ</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="fifPtn">接続方法(45度:0,90度:1,S管:2)</param>
        /// <param name="doneSCurveLoad">S管ロード済フラグ辞書(角型ダクト:1,円型ダクト:2,楕円型ダクト:3,配管:4)</param>
        /// <returns></returns>
        public bool PartialDuctOperationEndSideElbo45(Pipe pipe, Pipe outPipe, double hDiff, int fifPtn, ref Dictionary<int, bool> doneSCurveLoad)
        {
            WrpGeometry _geometry = new WrpGeometry(uidoc, log);
            bool result = true;
            using (Transaction tran = new Transaction(doc))
            {
                try
                {
                    tran.Start("PartialDuctOperationEndSideElbo45");
                    // 処理エラーが出た場合に何も言わずにロールバック
                    FailureRollback(tran);

                    //**********************
                    // outDuctの不要部分切断
                    //**********************
                    XYZ sPt2 = new XYZ();
                    _geometry.GetLocationCurveStartPoint(ref sPt2, outPipe);
                    XYZ ePt2 = new XYZ();
                    _geometry.GetLocationCurveEndPoint(ref ePt2, outPipe);
                    // 切断点を求める
                    double scalar = Math.Abs(_geometry.ConvertMillimetersToFeet(hDiff)) / sPt2.DistanceTo(ePt2);
                    log.Trace("スカラー：" + scalar.ToString());
                    if (scalar <= 0 || scalar >= 1) // 端点もダメなので "=" も排除。
                    {
                        log.Info("45°配置はできません。");
                        tran.Dispose();
                        // 変わりの配置
                        result = PartialDuctOperationEndSideElbo45Alt(pipe, outPipe, hDiff, fifPtn , ref doneSCurveLoad);
                        return result;
                    }

                    XYZ breakPt2 = sPt2.Add((ePt2.Subtract(sPt2)).Multiply(scalar));
                    log.Trace("切断点：" + breakPt2.ToString());

                    // outDuctを短縮する
                    Connector sConOutDuct = _mep.GetStartSideConnector(outPipe);
                    sConOutDuct.Origin = breakPt2;

                    // duct, 2を接続
                    //_mep.ConnectDuct(duct, outDuct);
                    _mep.ConnectPipeEndSide(pipe, outPipe, false);
                    tran.Commit();

                    // 処理エラーでロールバックの場合
                    TransactionStatus tranStatus = tran.GetStatus();
                    if (tranStatus == TransactionStatus.RolledBack)
                    {
                        log.Info("処理エラーでロールバック");
                        result = PartialDuctOperationEndSideElbo45Alt(pipe, outPipe, hDiff, fifPtn , ref doneSCurveLoad);
                    }
                }
                catch (Autodesk.Revit.Exceptions.ApplicationException ex)
                {
                    tran.RollBack();
                    log.Error(ex.Message);
                    result = PartialDuctOperationEndSideElbo45Alt(pipe, outPipe, hDiff, fifPtn , ref doneSCurveLoad);
                }
            }

            return result;
        }

        /// <summary>
        /// S管選択時処理(始点側)
        /// </summary>
        /// <param name="outPipe">区間外パイプ</param>
        /// <param name="pipe">区間内パイプ</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="fifPtn">接続方法(45度:0,90度:1,S管:2)</param>
        /// <param name="doneSCurveLoad">S管ロード済フラグ辞書(角型ダクト:1,円型ダクト:2,楕円型ダクト:3,配管:4)</param>
        /// <returns></returns>
        public bool PartialDuctOperationStartSideSCurve(Pipe outPipe, Pipe pipe, double hDiff, int fifPtn, ref Dictionary<int, bool> doneSCurveLoad)
        {
            bool result = true;
            using (Transaction tran = new Transaction(doc))
            {
                try
                {
                    tran.Start("PartialDuctOperationStartSideSCurve");
                    // 処理エラーが出た場合に何も言わずにロールバック
                    FailureRollback(tran);
                    // S管で接続
                    result = ConnectSOffset(pipe, outPipe, hDiff, ref doneSCurveLoad);
             
                    TransactionStatus tranStatus = tran.GetStatus();
                    if (result)
                    {
                        tran.Commit();
                        // 処理エラー、または、水平方向長さ不足で
                        // ロールバックの場合
                        tranStatus = tran.GetStatus();
                        if (tranStatus == TransactionStatus.RolledBack)
                        {
                            log.Info("処理エラーでロールバック");
                            result = PartialDuctOperationStartSideSCurveAlt(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                        }
                    }
                    else
                    {
                        // S管接続失敗の際はトランザクションを破棄。
                        tranStatus = tran.GetStatus();
                        if (tranStatus == TransactionStatus.Started)
                        {
                            tran.Dispose();
                            result = PartialDuctOperationStartSideSCurveAlt(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                        }
                    }
                   
                }
                catch (Autodesk.Revit.Exceptions.ApplicationException ex)
                {
                    tran.RollBack();
                    log.Error(ex.Message);
                    result = PartialDuctOperationStartSideSCurveAlt(outPipe, pipe, hDiff, fifPtn , ref doneSCurveLoad);
                }
                catch (Exception ex)
                {
                    tran.RollBack();
                    log.Error(ex.Message);
                    result = PartialDuctOperationStartSideSCurveAlt(outPipe, pipe, hDiff, fifPtn , ref doneSCurveLoad);
                }
            }
            return result;
        }

        /// <summary>
        /// S管選択時処理(終わり点側)
        /// </summary>
        /// <param name="pipe">区間内パイプ</param>
        /// <param name="outPipe">区間外パイプ</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="fifPtn">接続方法(45度:0,90度:1,S管:2)</param>
        /// <param name="doneSCurveLoad">S管ロード済フラグ辞書(角型ダクト:1,円型ダクト:2,楕円型ダクト:3,配管:4)</param>
        /// <returns></returns>
        public bool PartialDuctOperationEndSideSCurve(Pipe pipe, Pipe outPipe, double hDiff, int fifPtn, ref Dictionary<int, bool> doneSCurveLoad)
        {
            bool result = true;
            using (Transaction tran = new Transaction(doc))
            {
                try
                {
                    tran.Start("PartialDuctOperationEndSideSCurve");
                    // 処理エラーが出た場合に何も言わずにロールバック
                    FailureRollback(tran);

                    // S管で接続
                    result = ConnectEOffset(pipe, outPipe, hDiff, ref doneSCurveLoad);
                    TransactionStatus tranStatus = tran.GetStatus();

                    if (result)
                    {
                        // 接続成功時
                        tran.Commit();
                        // 処理エラー、または、水平方向長さ不足で
                        // ロールバックの場合
                        tranStatus = tran.GetStatus();
                        if (tranStatus == TransactionStatus.RolledBack)
                        {
                            log.Info("処理エラーでロールバック");
                            result = PartialDuctOperationEndSideSCurveAlt(pipe, outPipe, hDiff, fifPtn, ref doneSCurveLoad);
                        }
                    }
                    else
                    {
                        // 接続失敗時
                        tranStatus = tran.GetStatus();
                        if (tranStatus == TransactionStatus.Started)
                        {
                            tran.Dispose();
                            result = PartialDuctOperationEndSideSCurveAlt(pipe, outPipe, hDiff, fifPtn, ref doneSCurveLoad);
                        }
                    }
                


                    
                }
                catch (Autodesk.Revit.Exceptions.ApplicationException ex)
                {
                    tran.RollBack();
                    log.Error(ex.Message);
                    result = PartialDuctOperationEndSideSCurveAlt(pipe, outPipe, hDiff, fifPtn , ref doneSCurveLoad);
                }
                catch (Exception ex)
                {
                    tran.RollBack();
                    log.Error(ex.Message);
                    result = PartialDuctOperationStartSideSCurveAlt(outPipe, pipe, hDiff, fifPtn , ref doneSCurveLoad);
                }
            }
            return result;
        }

        /// <summary>
        /// 配管用オフセット部材のロード
        /// </summary>
        /// <param name="pipe">S管に接続するパイプ</param>
        /// <param name="sOffset">S管ファミリ</param>
        /// <param name="doneSCurveLoad">S管ロード済フラグ辞書(角型ダクト:1,円型ダクト:2,楕円型ダクト:3,配管:4)</param>
        /// <returns></returns>
        private bool LoadRecOffset(Pipe pipe, ref FamilySymbol sOffset, ref Dictionary<int, bool> doneSCurveLoad)
        {
            WrpArrangement _arrange = new WrpArrangement(uidoc, log);

            // ロード必要情報
            String dataFolder = null;
            String fileName = null;
            String filePath = null;
            String familyName = null;
            String typeName = null;
            bool needLoad = false;

            // パイプ形状確認
            Connector con = _mep.GetStartSideConnector(pipe);

            // パイプ
            dataFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + DuctDisplacementDefine.FAMILY_FOLDER;
            fileName = ExResources.ResxString(DuctDisplacementDefine.S_CURVE_PIPE_FILE);
            filePath = dataFolder + fileName;
            familyName = ExResources.ResxString(DuctDisplacementDefine.S_CURVE_PIPE_FAMILY);
            typeName = ExResources.ResxString(DuctDisplacementDefine.S_CURVE_PIPE_TYPE);
            needLoad = doneSCurveLoad[(int)DuctDisplacementDefine.S_CURVE_PTN.PIPE];
            doneSCurveLoad[(int)DuctDisplacementDefine.S_CURVE_PTN.PIPE] = true;

            sOffset = _arrange.GetFamilySymbol(familyName, typeName, BuiltInCategory.OST_PipeFitting);

            // オフセット部材がない場合、ロードする
            //if (sOffset == null || !needLoad)
            //{
            bool loadRes = _arrange.LoadFamilySymbol(ref sOffset, filePath, typeName);
            if (loadRes == false) return false;
            //}



            return true;
        }

        /// <summary>
        /// S管のファミリを接続(始点側)
        /// </summary>
        /// <param name="pipe">区間内パイプ</param>
        /// <param name="outPipe">区間外パイプ</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="doneSCurveLoad">S管ロード済フラグ辞書(角型ダクト:1,円型ダクト:2,楕円型ダクト:3,配管:4)</param>
        /// <returns></returns>
        private bool ConnectSOffset(Pipe pipe, Pipe outPipe, double hDiff, ref Dictionary<int, bool> doneSCurveLoad)
        {
            // S字の形状を表現できるか判定
            if (!CanDrawSCurve(pipe, hDiff)) return false;
            // オフセット部材ロード
            FamilySymbol sOffset = null;

            if (!LoadRecOffset(pipe, ref sOffset, ref doneSCurveLoad)) return false;
            // outductの向きに合わせて配置
            // オフセット配置
            XYZ pt = null;
            _geometry.GetLocationCurveStartPoint(ref pt, pipe);
            FamilyInstance iOffset = null;
            ArrangeMEPFittingStartSide(sOffset, pt, pipe, outPipe, hDiff, ref iOffset);

            // ductとオフセット部材接続
            Connector ductCon = _mep.GetStartSideConnector(pipe);
            Connector offsetCon = _mep.FindFittingConnector(iOffset, true);
            ductCon.ConnectTo(offsetCon);
            // outductとオフセット部材接続
            Connector eConOutDuct = _mep.GetEndSideConnector(outPipe);
            Connector eConOffset = _mep.FindFittingEndSideConnector(iOffset);
            // 外側のパイプと接続できるかどうか？
            double lengthShorten = (eConOutDuct.Origin.Subtract(eConOffset.Origin)).GetLength();
            double lengthOutDuct = ((LocationCurve)outPipe.Location).Curve.Length;
            log.Trace("短縮距離：" + lengthShorten);
            log.Trace("パイプ距離:" + lengthOutDuct);
            if (lengthShorten >= lengthOutDuct)
            {
                log.Info("水平方向で長さが不足のためS管配置できません。");
                return false;
            }


            // outDuctを縮めて接続する
            eConOutDuct.Origin = eConOffset.Origin;
            eConOutDuct.ConnectTo(eConOffset);

            // 断熱材作成
            _mep.CreatePipeInsulation(pipe, iOffset.Id);
            return true;
        }

        /// <summary>
        /// S管のファミリを接続(終点側)
        /// </summary>
        /// <param name="outPipe">区間外パイプ</param>
        /// <param name="pipe">区間内パイプ</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="doneSCurveLoad">S管ロード済フラグ辞書(角型ダクト:1,円型ダクト:2,楕円型ダクト:3,配管:4)</param>
        /// <returns></returns>
        private bool ConnectEOffset(Pipe pipe, Pipe outPipe, double hDiff, ref Dictionary<int, bool> doneSCurveLoad)
        {
            // S字の形状を表現できるか判定
            if (!CanDrawSCurve(pipe, hDiff)) return false;
            // オフセット部材ロード
            FamilySymbol sOffset = null;
            if (!LoadRecOffset(pipe, ref sOffset, ref doneSCurveLoad)) return false;
            // outductの向きに合わせて配置
            // オフセット配置
            XYZ pt = null;
            _geometry.GetLocationCurveEndPoint(ref pt, pipe);
            FamilyInstance iOffset = null;
            ArrangeMEPFittingEndSide(sOffset, pt, pipe, outPipe, hDiff, ref iOffset);
            // ductとオフセット部材接続
            Connector ductCon = _mep.GetEndSideConnector(pipe);
            Connector offsetCon = _mep.FindFittingConnector(iOffset, true);
            ductCon.ConnectTo(offsetCon);
            // outductとオフセット部材接続
            Connector sConOutDuct = _mep.GetStartSideConnector(outPipe);
            Connector eConOffset = _mep.FindFittingEndSideConnector(iOffset);
            // 外側のパイプと接続できるかどうか？
            double lengthShorten = (sConOutDuct.Origin.Subtract(eConOffset.Origin)).GetLength();
            double lengthOutDuct = ((LocationCurve)outPipe.Location).Curve.Length;
            log.Trace("短縮距離：" + lengthShorten);
            log.Trace("パイプ距離:" + lengthOutDuct);
            if (lengthShorten >= lengthOutDuct)
            {
                log.Info("水平方向で長さが不足のためS管配置できません。");
                return false;
            }

            // outDuctを縮めて接続する
            sConOutDuct.Origin = eConOffset.Origin;
            sConOutDuct.ConnectTo(eConOffset);

            // 断熱材作成
            _mep.CreatePipeInsulation(pipe, iOffset.Id);
            return true;
        }



        /// <summary>
        /// 指定のシンボルを指定MEPCurveに合わせて配置（始点側）
        /// </summary>
        /// <param name="symbol">配置したいシンボル</param>
        /// <param name="pt">切断点</param>
        /// <param name="pipe">区間内パイプ</param>
        /// <param name="outPipe">区間外パイプ</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="fInstance">配置したインスタンス</param>
        /// <returns></returns>
        private bool ArrangeMEPFittingStartSide(FamilySymbol symbol, XYZ pt, Pipe pipe, Pipe outPipe, double hDiff, ref FamilyInstance fInstance)
        {
            // 配置
            fInstance = doc.Create.NewFamilyInstance(pt, symbol, StructuralType.NonStructural);
            // オフセットパラメータ変更
            SetOffsetParm(pipe, hDiff, ref fInstance);
            // オフセット方向に合わせてフリップさせる
            if (hDiff > 0)
            {
                fInstance.Location.Rotate(Line.CreateUnbound(pt, XYZ.BasisX), Math.PI * 1.5);
            }
            else
            {
                fInstance.Location.Rotate(Line.CreateUnbound(pt, XYZ.BasisX), Math.PI * 0.5);
            }
            LocationCurve lCurve = pipe.Location as LocationCurve;
            // パイプ方向に合わせて回転させる
            fInstance.Location.Rotate(Line.CreateUnbound(pt, XYZ.BasisZ), XYZ.BasisX.AngleOnPlaneTo(((Line)lCurve.Curve).Direction * (-1), XYZ.BasisZ));
            return true;
        }

        /// <summary>
        /// 指定のシンボルを指定MEPCurveに合わせて配置（終点側）
        /// </summary>
        /// <param name="symbol">配置したいシンボル</param>
        /// <param name="pt">切断点</param>
        /// <param name="pipe">区間内パイプ</param>
        /// <param name="outPipe">区間外パイプ</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="fInstance">配置したインスタンス</param>
        /// <returns></returns>
        private bool ArrangeMEPFittingEndSide(FamilySymbol symbol, XYZ pt, Pipe pipe, Pipe outPipe, double hDiff, ref FamilyInstance fInstance)
        {
            // 配置
            fInstance = doc.Create.NewFamilyInstance(pt, symbol, StructuralType.NonStructural);
            // オフセットパラメータ変更
            SetOffsetParm(pipe, hDiff, ref fInstance);
            // オフセット方向に合わせてフリップさせる
            if (hDiff > 0)
            {
                fInstance.Location.Rotate(Line.CreateUnbound(pt, XYZ.BasisX), Math.PI * (-0.5));
            }
            else
            {
                fInstance.Location.Rotate(Line.CreateUnbound(pt, XYZ.BasisX), Math.PI * 0.5);
            }
            LocationCurve lCurve = pipe.Location as LocationCurve;
            // パイプ方向に合わせて回転させる
            fInstance.Location.Rotate(Line.CreateUnbound(pt, XYZ.BasisZ), XYZ.BasisX.AngleOnPlaneTo(((Line)lCurve.Curve).Direction, XYZ.BasisZ));
            return true;
        }

        /// <summary>
        /// S管ファミリへのパラメータセット
        /// </summary>
        /// <param name="pipe">接続するパイプ</param>
        /// <param name="hDiff">移動距離</param>
        /// <param name="iOffset">(配置した)S管(インスタンス)</param>
        /// <returns></returns>
        private bool SetOffsetParm(Pipe pipe, double hDiff, ref FamilyInstance iOffset)
        {
            // オフセットサイズ調整

            // オフセットパラメータ取得
            Parameter hParam = iOffset.LookupParameter("幅");
            Parameter wParam = iOffset.LookupParameter("高さ");
            Parameter diaParam = iOffset.LookupParameter("直径");
            Parameter offsetParam = iOffset.LookupParameter("オフセット");
            // オフセットパラメータ変更
            if (hParam != null && wParam != null && offsetParam != null)
            {
                // パイプパラメータ取得
                double h = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
                double w = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
                h = _geometry.ConvertFeetToMillimeters(h);
                w = _geometry.ConvertFeetToMillimeters(w);
                hParam.SetValueString(h.ToString());
                wParam.SetValueString(w.ToString());
                offsetParam.SetValueString(Math.Abs(hDiff).ToString());
                return true;
            }
            else if (diaParam != null && offsetParam != null)
            {
                // パイプパラメータ取得
                double diameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
                diameter = _geometry.ConvertFeetToMillimeters(diameter);
                diaParam.SetValueString(diameter.ToString());
                offsetParam.SetValueString(Math.Abs(hDiff).ToString());
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// S管に設定するパラメータがS管形状を保てるものかチェック
        /// </summary>
        /// <param name="pipe">接続するパイプ</param>
        /// <param name="hDiff">移動距離</param>
        /// <returns></returns>
        private bool CanDrawSCurve(Pipe pipe, double hDiff)
        {
            Parameter hParam = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
            Parameter diaParam = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
            if (hParam != null)
            {
                double h = _geometry.ConvertFeetToMillimeters(hParam.AsDouble());
                if (Math.Abs(hDiff) < h * 0.5 + DuctDisplacementDefine.S_CURVE_THRESHOLD)
                {
                    log.Info("S管は形状を保てないため、配置できません。");
                    return false;
                }
                else return true;
            }
            if (diaParam != null)
            {
                double diameter = _geometry.ConvertFeetToMillimeters(diaParam.AsDouble());
                if (Math.Abs(hDiff) < diameter * 0.5 + DuctDisplacementDefine.S_CURVE_THRESHOLD)
                {
                    log.Info("S管は形状を保てないため、配置できません。");
                    return false;
                }
                else return true;
            }
            return false;
        }

        private bool PartialDuctOperationStartSideElbo90Alt(Pipe outPipe, Pipe pipe, double hDiff, int fifPtn, ref Dictionary<int, bool> doneSCurveLoad)
        {
            bool result = true;
            log.Info("変わりの接続を実施します");
            switch (fifPtn)
            {
                case (int)DuctDisplacementDefine.FITTING_PTN.deg90:
                    // 90→45→S
                    result = PartialDuctOperationStartSideElbo45(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                    break;
                case (int)DuctDisplacementDefine.FITTING_PTN.deg45:
                    // 45→90→
                    result = PartialDuctOperationStartSideSCurve(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                    break;
                case (int)DuctDisplacementDefine.FITTING_PTN.S:
                    // S→45→90
                    //TODO エラー
                    return false;
            }
            return result;
        }

        private bool PartialDuctOperationEndSideElbo90Alt(Pipe outPipe, Pipe pipe, double hDiff, int fifPtn, ref Dictionary<int, bool> doneSCurveLoad)
        {
            bool result = true;
            log.Info("変わりの接続を実施します");
            switch (fifPtn)
            {
                case (int)DuctDisplacementDefine.FITTING_PTN.deg90:
                    // 90→45→S
                    result = PartialDuctOperationEndSideElbo45(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                    break;
                case (int)DuctDisplacementDefine.FITTING_PTN.deg45:
                    // 45→90→S
                    result = PartialDuctOperationEndSideSCurve(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                    break;
                case (int)DuctDisplacementDefine.FITTING_PTN.S:
                    // S→45→90
                    return false;
            }
            return result;
        }

        private bool PartialDuctOperationStartSideElbo45Alt(Pipe outPipe, Pipe pipe, double hDiff, int fifPtn, ref Dictionary<int, bool> doneSCurveLoad)
        {
            bool result = true;
            log.Info("変わりの接続を実施します");
            switch (fifPtn)
            {
                case (int)DuctDisplacementDefine.FITTING_PTN.deg90:
                    // 90→45→S
                    result = PartialDuctOperationStartSideSCurve(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                    break;
                case (int)DuctDisplacementDefine.FITTING_PTN.deg45:
                    // 45→90→S
                    result = PartialDuctOperationStartSideElbo90(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                    break;
                case (int)DuctDisplacementDefine.FITTING_PTN.S:
                    // S→45→90
                    result = PartialDuctOperationStartSideElbo90(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                    break;
            }
            return result;
        }

        private bool PartialDuctOperationEndSideElbo45Alt(Pipe outPipe, Pipe pipe, double hDiff, int fifPtn, ref Dictionary<int, bool> doneSCurveLoad)
        {
            bool result = true;
            log.Info("変わりの接続を実施します");
            switch (fifPtn)
            {
                case (int)DuctDisplacementDefine.FITTING_PTN.deg90:
                    // 90→45→S
                    result = PartialDuctOperationEndSideSCurve(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                    break;
                case (int)DuctDisplacementDefine.FITTING_PTN.deg45:
                    // 45→90→S
                    result = PartialDuctOperationEndSideElbo90(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                    break;
                case (int)DuctDisplacementDefine.FITTING_PTN.S:
                    // S→45→90
                    result = PartialDuctOperationEndSideElbo90(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                    break;
            }
            return result;
        }

        private bool PartialDuctOperationStartSideSCurveAlt(Pipe outPipe, Pipe pipe, double hDiff, int fifPtn, ref Dictionary<int, bool> doneSCurveLoad)
        {
            bool result = true;
            log.Info("変わりの接続を実施します");
            switch (fifPtn)
            {
                case (int)DuctDisplacementDefine.FITTING_PTN.deg90:
                    // 90→45→S
                    return false;
                case (int)DuctDisplacementDefine.FITTING_PTN.deg45:
                    // 45→90→S
                    return false;
                case (int)DuctDisplacementDefine.FITTING_PTN.S:
                    // S→45→90
                    result = PartialDuctOperationStartSideElbo45(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                    break;
            }
            return result;
        }

        private bool PartialDuctOperationEndSideSCurveAlt(Pipe outPipe, Pipe pipe, double hDiff, int fifPtn, ref Dictionary<int, bool> doneSCurveLoad)
        {
            bool result = true;
            log.Info("変わりの接続を実施します");
            switch (fifPtn)
            {
                case (int)DuctDisplacementDefine.FITTING_PTN.deg90:
                    // 90→45→S
                    return false;
                case (int)DuctDisplacementDefine.FITTING_PTN.deg45:
                    // 45→90→S
                    return false;
                case (int)DuctDisplacementDefine.FITTING_PTN.S:
                    // S→45→90
                    result = PartialDuctOperationEndSideElbo45(outPipe, pipe, hDiff, fifPtn, ref doneSCurveLoad);
                    break;
            }
            return result;
        }
        #endregion

        #region 配管システム

        /// <summary>
        /// document内PipingSystemの中身
        /// </summary>
        /// <returns></returns>
        public override bool ShowMEPSystemMember()
        {
            FilteredElementCollector collector
            = new FilteredElementCollector(doc);
            // DB内のElementたちでPipingSystemを取得
            ICollection<Element> pSystems
              = collector.OfClass(typeof(PipingSystem))
                .ToElements();
            // 現在のビューIDがViewPlanたちに含まれていれば、
            // 現在のビューは平面
            foreach (Element e in pSystems)
            {
                PipingSystem pSystem = e as PipingSystem;
                ElementSet nMember = pSystem.PipingNetwork;
                ElementSet elements = pSystem.Elements;

                string pipeNetworkMemberStr = "pipeNetworkMemberStr:";

                foreach (Element member in nMember)
                {
                    pipeNetworkMemberStr = pipeNetworkMemberStr + member.Id + ":";
                }
                //TaskDialog.Show("test", ductNetworkMemberStr);

                string elementsStr = "elements(terminal):";
                foreach (Element member in nMember)
                {
                    elementsStr = elementsStr + member.Id + ":";
                }
                //TaskDialog.Show("test", elementsStr);

            }

            return true;
        }
        #endregion
        #endregion

        //// プロパティ
        #region Properties
        #endregion
    }

}
