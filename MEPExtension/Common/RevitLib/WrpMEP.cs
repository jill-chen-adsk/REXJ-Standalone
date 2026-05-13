using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using RevitMEPAddin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitMEPAddin.Common
{
    public class WrpMEP
    {
        #region member
        UIDocument uidoc;
        Document doc;
        Logger log;
        WrpGeometry _geometry;
        #endregion
        #region constractor
        public WrpMEP(UIDocument uidoc, Logger log)
        {
            this.uidoc = uidoc;
            this.doc = uidoc.Document;
            _geometry = new WrpGeometry(uidoc, log);
            this.log = log;
        }
        #endregion

        #region method
        /// <summary>
        /// 2つのMEPCurveをエルボで接続する
        /// </summary>
        /// <param name="curve1"></param>
        /// <param name="curve2"></param>
        /// <param name="elbow"></param>
        /// <returns></returns>
        public bool ConnectMEPCurveWithElbow(MEPCurve curve1, MEPCurve curve2, ref FamilyInstance elbow)
        {
            Connector sCon1 = GetStartSideConnector(curve1);
            Connector eCon1 = GetEndSideConnector(curve1);
            Connector sCon2 = GetStartSideConnector(curve2);
            Connector eCon2 = GetEndSideConnector(curve2);

            if (eCon1.Origin.DistanceTo(sCon2.Origin)
                < eCon2.Origin.DistanceTo(sCon1.Origin))
            {
                elbow = doc.Create.NewElbowFitting(eCon1, sCon2);
            }
            else if (eCon1.Origin.DistanceTo(sCon2.Origin)
                               > eCon2.Origin.DistanceTo(sCon1.Origin))
            {
                elbow = doc.Create.NewElbowFitting(eCon2, sCon1);
            }

                return true;
        }

        /// <summary>
        /// ２つのダクトをエルボ＋ダクト＋エルボで接続する処理
        /// </summary>
        /// <param name="duct1"></param>
        /// <param name="duct2"></param>
        /// <returns></returns>
        public bool ConnectDuct(Duct duct1, Duct duct2)
        {
            // duct1, 2のConnector取得
            Connector eCon1, sCon2;
            eCon1 = GetEndSideConnector(duct1);
            sCon2 = GetStartSideConnector(duct2);

            // duct1, 2を接続するためのダクトを新規配置
            Duct newDuct = Duct.Create(
                doc, duct1.DuctType.Id, duct1.ReferenceLevel.Id, eCon1, sCon2);

            // 新規ダクトとduct1, 2を接続するエルボを新規配置
            Connector sConNew = GetStartSideConnector(newDuct);
            Connector eConNew = GetEndSideConnector(newDuct);

            doc.Create.NewElbowFitting(eCon1, sConNew);
            doc.Create.NewElbowFitting(eConNew, sCon2);

            return true;// とりあえず
        }

        /// <summary>
        /// ２つのパイプをエルボ＋パイプ＋エルボで接続する処理
        /// </summary>
        /// <param name="pipe1"></param>
        /// <param name="pipe2"></param>
        /// <returns></returns>
        public bool ConnectPipe(Pipe pipe1, Pipe pipe2)
        {
            // pipe1, 2のConnector取得
            Connector eCon1, sCon2;
            eCon1 = GetEndSideConnector(pipe1);
            sCon2 = GetStartSideConnector(pipe2);

            // pipe1, 2を接続するためのダクトを新規配置
            Pipe newPipe = Pipe.Create(
                doc, pipe1.PipeType.Id, pipe1.ReferenceLevel.Id, eCon1, sCon2);

            // 新規ダクトとpipe1, 2を接続するエルボを新規配置
            Connector sConNew = GetStartSideConnector(newPipe);
            Connector eConNew = GetEndSideConnector(newPipe);

            doc.Create.NewElbowFitting(eCon1, sConNew);
            doc.Create.NewElbowFitting(eConNew, sCon2);

            return true;// とりあえず
        }

        /// <summary>
        /// 区間距離を担保して接続
        /// </summary>
        /// <param name="duct1">外側ダクト</param>
        /// <param name="duct2">内側ダクト</param>
        /// /// <param name="needRotate">90°接続かどうか</param>
        /// <returns></returns>
        public bool ConnectDuctStartSide(Duct duct1, Duct duct2, bool needRotate)
        {
            // duct1, 2のConnector取得
            Connector eCon1, sCon2;
            eCon1 = GetEndSideConnector(duct1);
            sCon2 = GetStartSideConnector(duct2);

            // duct1, 2を接続するためのダクトを新規配置
            Duct newDuct = Duct.Create(
                doc, duct1.DuctType.Id, duct1.ReferenceLevel.Id, eCon1, sCon2);

            // 新規ダクトとduct1, 2を接続するエルボを新規配置
            Connector sConNew = GetStartSideConnector(newDuct);
            Connector eConNew = GetEndSideConnector(newDuct);

            if (needRotate)
            {
                Transform csWCon2 = sCon2.CoordinateSystem;
                Transform csEConNew = eConNew.CoordinateSystem;
                //Transform tForm = Transform.CreateRotationAtPoint(csWCon2.BasisX, Math.PI * 0.5, csWCon2.Origin);
                //csEConNew.BasisX = tForm.OfVector(csWCon2.BasisX);
                //csEConNew.BasisY = tForm.OfVector(csWCon2.BasisY);
                //csEConNew.BasisZ = tForm.OfVector(csWCon2.BasisZ).Multiply(-1);
                // ダクトを回転
                newDuct.Location.Rotate(Line.CreateUnbound(csEConNew.Origin, XYZ.BasisZ.Multiply(-1)),
                    ((Line)((LocationCurve)duct1.Location).Curve).Direction.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ) + Math.PI * (0.5));
                log.Trace("inDuct:X" + csWCon2.BasisX.ToString());
                log.Trace("inDuct:Y" + csWCon2.BasisY.ToString());
                log.Trace("inDuct:Z" + csWCon2.BasisZ.ToString());
                log.Trace("newDuct:X" + csEConNew.BasisX.ToString());
                log.Trace("newDuct:Y" + csEConNew.BasisY.ToString());
                log.Trace("newDuct:Z" + csEConNew.BasisZ.ToString());
            }
            // 区間側の接続
            FamilyInstance elbowIn = doc.Create.NewElbowFitting(eConNew, sCon2);

            // 区間が短縮されている部分の調整
            XYZ moveVector = ((LocationPoint)elbowIn.Location).Point.Subtract(sCon2.Origin);
            elbowIn.Location.Move(moveVector);
            newDuct.Location.Move(moveVector);

            // 区間外側の接続
            FamilyInstance elbowOut = doc.Create.NewElbowFitting(eCon1, sConNew);

    
            // 断熱材ありの場合に断熱材巻く。
            CreateDuctInsulation(duct2, newDuct.Id);
            CreateDuctInsulation(duct2, elbowIn.Id);
            CreateDuctInsulation(duct2, elbowOut.Id);
            

            return true;// とりあえず
        }

        /// <summary>
        /// 区間距離を担保して接続
        /// </summary>
        /// <param name="pipe1"></param>
        /// <param name="pipe2"></param>
        /// <param name="needRotate"></param>
        /// <returns></returns>
        public bool ConnectPipeStartSide(Pipe pipe1, Pipe pipe2, bool needRotate)
        {
            // duct1, 2のConnector取得
            Connector eCon1, sCon2;
            eCon1 = GetEndSideConnector(pipe1);
            sCon2 = GetStartSideConnector(pipe2);

            // duct1, 2を接続するためのダクトを新規配置
            Pipe newPipe = Pipe.Create(
                doc, pipe1.PipeType.Id, pipe1.ReferenceLevel.Id, eCon1, sCon2);

            // 新規ダクトとduct1, 2を接続するエルボを新規配置
            Connector sConNew = GetStartSideConnector(newPipe);
            Connector eConNew = GetEndSideConnector(newPipe);

            if (needRotate)
            {
                Transform csWCon2 = sCon2.CoordinateSystem;
                Transform csEConNew = eConNew.CoordinateSystem;
                //Transform tForm = Transform.CreateRotationAtPoint(csWCon2.BasisX, Math.PI * 0.5, csWCon2.Origin);
                //csEConNew.BasisX = tForm.OfVector(csWCon2.BasisX);
                //csEConNew.BasisY = tForm.OfVector(csWCon2.BasisY);
                //csEConNew.BasisZ = tForm.OfVector(csWCon2.BasisZ).Multiply(-1);
                // ダクトを回転
                newPipe.Location.Rotate(Line.CreateUnbound(csEConNew.Origin, XYZ.BasisZ.Multiply(-1)),
                    ((Line)((LocationCurve)pipe1.Location).Curve).Direction.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ) + Math.PI * (0.5));
                log.Trace("inDuct:X" + csWCon2.BasisX.ToString());
                log.Trace("inDuct:Y" + csWCon2.BasisY.ToString());
                log.Trace("inDuct:Z" + csWCon2.BasisZ.ToString());
                log.Trace("newDuct:X" + csEConNew.BasisX.ToString());
                log.Trace("newDuct:Y" + csEConNew.BasisY.ToString());
                log.Trace("newDuct:Z" + csEConNew.BasisZ.ToString());
            }
            // 区間側の接続
            FamilyInstance elbowIn = doc.Create.NewElbowFitting(eConNew, sCon2);

            // 区間が短縮されている部分の調整
            XYZ moveVector = ((LocationPoint)elbowIn.Location).Point.Subtract(sCon2.Origin);
            elbowIn.Location.Move(moveVector);
            newPipe.Location.Move(moveVector);

            // 区間外側の接続
            FamilyInstance elbowOut = doc.Create.NewElbowFitting(eCon1, sConNew);


            // 断熱材ありの場合に断熱材巻く。
            CreatePipeInsulation(pipe2, newPipe.Id);
            CreatePipeInsulation(pipe2, elbowIn.Id);
            CreatePipeInsulation(pipe2, elbowOut.Id);


            return true;// とりあえず
        }

        /// <summary>
        /// 区間距離を担保して接続
        /// </summary>
        /// <param name="duct1">内側ダクト</param>
        /// <param name="duct2">外側ダクト</param>
        /// <param name="needRotate">90°接続かどうか</param>
        /// <returns></returns>
        public bool ConnectDuctEndSide(Duct duct1, Duct duct2, bool needRotate)
        {
            
            // duct1, 2のConnector取得
            Connector eCon1, sCon2;
            eCon1 = GetEndSideConnector(duct1);
            sCon2 = GetStartSideConnector(duct2);

            // duct1, 2を接続するためのダクトを新規配置
            Duct newDuct = Duct.Create(
                doc, duct1.DuctType.Id, duct1.ReferenceLevel.Id, eCon1, sCon2);
            
            // 新規ダクトとduct1, 2を接続するエルボを新規配置
            Connector sConNew = GetStartSideConnector(newDuct);
            Connector eConNew = GetEndSideConnector(newDuct);

            if (needRotate)
            {
                Transform csECon1 = eCon1.CoordinateSystem;
                Transform csSConNew = sConNew.CoordinateSystem;
                //Transform tForm = Transform.CreateRotationAtPoint(csECon1.BasisX, Math.PI * 0.5, csECon1.Origin);
                //csSConNew.BasisX = tForm.OfVector(csECon1.BasisX);
                //csSConNew.BasisY = tForm.OfVector(csECon1.BasisY);
                //csSConNew.BasisZ = tForm.OfVector(csECon1.BasisZ).Multiply(-1);
                // ダクトを回転
                newDuct.Location.Rotate(Line.CreateUnbound(csSConNew.Origin, XYZ.BasisZ.Multiply(-1)),
                    ((Line)((LocationCurve)duct1.Location).Curve).Direction.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ) + Math.PI * (0.5));

                log.Trace("inDuct:X" + csECon1.BasisX.ToString());
                log.Trace("inDuct:Y" + csECon1.BasisY.ToString());
                log.Trace("inDuct:Z" + csECon1.BasisZ.ToString());
                log.Trace("newDuct:X" + csSConNew.BasisX.ToString());
                log.Trace("newDuct:Y" + csSConNew.BasisY.ToString());
                log.Trace("newDuct:Z" + csSConNew.BasisZ.ToString());
            }
            // 区間側の接続
            FamilyInstance elbowIn = doc.Create.NewElbowFitting(eCon1, sConNew);

            // 区間が短縮されている部分の調整
            XYZ moveVector = ((LocationPoint)elbowIn.Location).Point.Subtract(eCon1.Origin);
            elbowIn.Location.Move(moveVector);
            newDuct.Location.Move(moveVector);

            // 区間外側の接続
            FamilyInstance elbowOut = doc.Create.NewElbowFitting(eConNew, sCon2);

            // 断熱材ありの場合に断熱材巻く。
            CreateDuctInsulation(duct1, newDuct.Id);
            CreateDuctInsulation(duct1, elbowIn.Id);
            CreateDuctInsulation(duct1, elbowOut.Id);

            return true;// とりあえず
        }

        /// <summary>
        /// 区間距離を担保して接続
        /// </summary>
        /// <param name="duct1"></param>
        /// <param name="duct2"></param>
        /// <param name="needRotate"></param>
        /// <returns></returns>
        public bool ConnectPipeEndSide(Pipe pipe1, Pipe pipe2, bool needRotate)
        {

            // duct1, 2のConnector取得
            Connector eCon1, sCon2;
            eCon1 = GetEndSideConnector(pipe1);
            sCon2 = GetStartSideConnector(pipe2);

            // duct1, 2を接続するためのダクトを新規配置
            Pipe newPipe = Pipe.Create(
                doc, pipe1.PipeType.Id, pipe1.ReferenceLevel.Id, eCon1, sCon2);

            // 新規ダクトとduct1, 2を接続するエルボを新規配置
            Connector sConNew = GetStartSideConnector(newPipe);
            Connector eConNew = GetEndSideConnector(newPipe);

            if (needRotate)
            {
                Transform csECon1 = eCon1.CoordinateSystem;
                Transform csSConNew = sConNew.CoordinateSystem;
                //Transform tForm = Transform.CreateRotationAtPoint(csECon1.BasisX, Math.PI * 0.5, csECon1.Origin);
                //csSConNew.BasisX = tForm.OfVector(csECon1.BasisX);
                //csSConNew.BasisY = tForm.OfVector(csECon1.BasisY);
                //csSConNew.BasisZ = tForm.OfVector(csECon1.BasisZ).Multiply(-1);
                // ダクトを回転
                newPipe.Location.Rotate(Line.CreateUnbound(csSConNew.Origin, XYZ.BasisZ.Multiply(-1)),
                    ((Line)((LocationCurve)pipe1.Location).Curve).Direction.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ) + Math.PI * (0.5));

                log.Trace("inDuct:X" + csECon1.BasisX.ToString());
                log.Trace("inDuct:Y" + csECon1.BasisY.ToString());
                log.Trace("inDuct:Z" + csECon1.BasisZ.ToString());
                log.Trace("newDuct:X" + csSConNew.BasisX.ToString());
                log.Trace("newDuct:Y" + csSConNew.BasisY.ToString());
                log.Trace("newDuct:Z" + csSConNew.BasisZ.ToString());
            }
            // 区間側の接続
            //  eCon1 = sConNew;
            FamilyInstance elbowIn = doc.Create.NewElbowFitting(eCon1, sConNew);

            // 区間が短縮されている部分の調整
            XYZ moveVector = ((LocationPoint)elbowIn.Location).Point.Subtract(eCon1.Origin);
            elbowIn.Location.Move(moveVector);
            newPipe.Location.Move(moveVector);

            // 区間外側の接続
            FamilyInstance elbowOut = doc.Create.NewElbowFitting(eConNew, sCon2);

            // 断熱材ありの場合に断熱材巻く。
            CreatePipeInsulation(pipe1, newPipe.Id);
            CreatePipeInsulation(pipe1, elbowIn.Id);
            CreatePipeInsulation(pipe1, elbowOut.Id);

            return true;// とりあえず
        }

        /// <summary>
        /// ダクトの幅・高さを入れ替える
        /// </summary>
        /// <param name="duct"></param>
        /// <returns></returns>
        private bool SwapDuctHandW(Duct duct)
        {
            Parameter wParam = duct.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM);
            Parameter hParam = duct.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM);
            if (wParam != null && hParam != null)
            {
                double w = _geometry.ConvertFeetToMillimeters(wParam.AsDouble());
                double h = _geometry.ConvertFeetToMillimeters(hParam.AsDouble());
                wParam.SetValueString(h.ToString());
                hParam.SetValueString(w.ToString());
                log.Trace("幅：" + _geometry.ConvertFeetToMillimeters(wParam.AsDouble()));
                log.Trace("高さ：" + _geometry.ConvertFeetToMillimeters(hParam.AsDouble()));
            }
            return true;
        }

        /// <summary>
        /// 指定のMEP要素の接続先のコネクタたちを取得
        /// メモ）接続先エレメントが欲しければOwnerで取得してください。
        /// </summary>
        /// <param name="fitting"></param>
        /// <param name="exceptElmList"></param>
        /// <param name="conList"></param>
        /// <returns></returns>
        public bool GetConnectionsWithSpMEPElmRef(Element elm, ref List<Connector> conList, List<ElementId> exceptElmList = null)
        {
            // 省略時のため。
            if (exceptElmList == null) exceptElmList = new List<ElementId>();

            ConnectorSet cons = null;
            GetConnectorsFromMSystemMember(ref cons, elm);

            conList = new List<Connector>();
            foreach (Connector con in cons)
            {
                foreach (Connector refCon in con.AllRefs)
                {
                    if (refCon.Owner.Id == elm.Id) continue;
                    if (exceptElmList.Contains(refCon.Owner.Id)) continue;
                    conList.Add(refCon);
                    break;
                }
            }
            return true;
        }

        
        /// <summary>
        /// 2本のMEPCurveの方向ベクトルたちの間の角度を求める。
        /// </summary>
        /// <param name="curve1"></param>
        /// <param name="curve2"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public bool AngleBtwMEPCurves(MEPCurve curve1, MEPCurve curve2, ref double angle)
        {
            LocationCurve lCurve1 = curve1.Location as LocationCurve;
            LocationCurve lCurve2 = curve2.Location as LocationCurve;
            Line line1 = lCurve1.Curve as Line;
            Line line2 = lCurve2.Curve as Line;

            // 直線の方向ベクトルたちの間の角度を求める
            return _geometry.getAngleBetweenVectors(ref angle, line1.Direction, line2.Direction);
        }

        /// <summary>
        /// DuctやPipeの始点側のコネクタ取得
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public Connector GetStartSideConnector(MEPCurve curve)
        {
            XYZ pt = new XYZ();
            _geometry.GetLocationCurveStartPoint(ref pt, curve);
            return FindConnector(curve, pt);
        }

        /// <summary>
        /// DuctやPipeの終点側のコネクタ取得
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public Connector GetEndSideConnector(MEPCurve curve)
        {
            XYZ pt = new XYZ();
            _geometry.GetLocationCurveEndPoint(ref pt, curve);
            return FindConnector(curve, pt);
        }

        /// <summary>
        /// Find out a connector from Duct(Pipe) with a specified point.
        /// </summary>
        /// <param name="curve">Duct(Pipe) to find the connector</param>
        /// <param name="conXYZ">Specified point</param>
        /// <returns>Connector whose origin is conXYZ</returns>
        public Connector FindConnector(MEPCurve curve, Autodesk.Revit.DB.XYZ conXYZ)
        {
            ConnectorSet conns = curve.ConnectorManager.Connectors;
            foreach (Connector conn in conns)
            {
                if (conn.Origin.IsAlmostEqualTo(conXYZ))
                {
                    return conn;
                }
            }
            return null;
        }

        /// <summary>
        /// FamilySymbolの始点側コネクタ取得
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Connector FindFittingStartSideConnector(FamilyInstance symbol, XYZ pt = null)
        {
            Connector con = FindFittingConnector(symbol, true, pt);
            return con;
        }
        /// <summary>
        /// FamilySymbolの始点側以外のコネクタ取得
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Connector FindFittingEndSideConnector(FamilyInstance symbol, XYZ pt = null)
        {
            return FindFittingConnector(symbol, false, pt);
        }


        /// <summary>
        /// FamilySymbolのコネクタ取得
        /// 　始点側かそれ以外のコネクタかを指定して取得できる。
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="isStartPt">始点側？</param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public Connector FindFittingConnector(FamilyInstance instance, bool isStartPt, XYZ pt = null)
        {
            ConnectorSet conns = instance.MEPModel.ConnectorManager.Connectors;
            if (pt == null)
            {
                LocationPoint lPt = instance.Location as LocationPoint;
                pt = lPt.Point;
            }
            foreach (Connector conn in conns)
            {
                if (isStartPt == true && conn.Origin.IsAlmostEqualTo(pt))
                {
                    return conn;
                }
                else if (isStartPt == false && !conn.Origin.IsAlmostEqualTo(pt))
                {
                    return conn;
                }
            }
            return null;
        }

        /// <summary>
        /// (MEPSystemのメンバーである)Elementのコネクションセットを取得
        /// </summary>
        /// <param name="cons"></param>
        /// <param name="sysMember"></param>
        /// <returns></returns>
        public bool GetConnectorsFromMSystemMember(ref ConnectorSet cons, Element sysMember)
        {
            // コネクタたちを取得
            MEPCurve mepCurve = sysMember as MEPCurve;
            FabricationPart fabPart = sysMember as FabricationPart;

            if (mepCurve == null && fabPart == null)
            {
                // ダクト・配管以外の場合
                MEPModel mepModel = (sysMember as FamilyInstance).MEPModel;
                cons = mepModel.ConnectorManager.Connectors;

            }
            else if(mepCurve != null)
            {
                // ダクト・配管の場合
                cons = mepCurve.ConnectorManager.Connectors;
            }
            else if(fabPart != null)
            {
                // 製造用パーツの場合
                cons = fabPart.ConnectorManager.Connectors;
            }

            return true;
        }

        /// <summary>
        /// MEPCurve(ダクト・パイプ)の切断点算出
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool GetCutPoint(MEPCurve curve, ref XYZ pt)
        {
            if (curve == null) return false;
            LocationCurve lCurve = curve.Location as LocationCurve;
            XYZ prjPt = lCurve.Curve.Project(pt).XYZPoint;
            if (prjPt == null) return false;
            pt = prjPt;

            return true;
        }

        

        /// <summary>
        /// 指定の点を含むダクト/配管を取得する
        /// </summary>
        /// <param name="resDuct"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool GetDuctByPoint(ref MEPCurve resDuct, XYZ pt)
        {
            FilteredElementCollector collector
            = new FilteredElementCollector(doc, doc.ActiveView.Id);
            // アクティブビュー内のElementたちでDuctを取得
            ICollection<Element> ducts
              = collector.OfClass(typeof(MEPCurve)).ToElements();

            foreach (Element e in ducts)
            {
                MEPCurve duct = e as MEPCurve;
                bool res = false;
                if (!IsInDuct(ref res, duct, pt)) return false;
                if (!res) continue;
                resDuct = duct;
                return true;
            }
            return true;
        }
        /// <summary>
        /// 指定の点が指定のダクト/配管に含まれるかどうか？
        /// </summary>
        /// <param name="result"></param>
        /// <param name="duct"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool IsInDuct(ref bool result, MEPCurve duct, XYZ pt)
        {
            // 初期化
            result = false;
            // LocationCurveに含まれるかどうか？
            LocationCurve curve = duct.Location as LocationCurve;
            IntersectionResult intersection = curve.Curve.Project(pt);
            //if (intersection != null && intersection.XYZPoint != null /*&& pt.IsAlmostEqualTo(intersection.XYZPoint)*/)
            //{
            //    TaskDialog.Show("test", "内積" + ((intersection.XYZPoint - pt).Normalize()).DotProduct(doc.ActiveView.ViewDirection).ToString());
            //    TaskDialog.Show("test", "カーブ:" + pt.ToString() + "" + intersection.XYZPoint.ToString());
            //    result = true;
            //    return true;
            //}

            // 面またはエッジに含まれるかどうか？
            Options opt = new Options();
            GeometryElement geomElem = duct.get_Geometry(opt);
            foreach (GeometryObject geomObj in geomElem)
            {
                Solid geomSolid = geomObj as Solid;
                if (null != geomSolid)
                {
                    // 面に含まれるかどうか？
                    foreach (Face geomFace in geomSolid.Faces)
                    {
                        intersection = geomFace.Project(pt);
                        if (intersection != null && intersection.XYZPoint != null
                            /*&& pt.IsAlmostEqualTo(intersection.XYZPoint)*/)
                        {
                            //TaskDialog.Show("test", "内積" + ((intersection.XYZPoint - pt).Normalize()).DotProduct(doc.ActiveView.ViewDirection).ToString());
                            //TaskDialog.Show("test", "面:" + pt.ToString() + "" + intersection.XYZPoint.ToString());
                            result = true;
                            //return true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    // エッジに含まれるかどうか？<= faceで調べきれる
                    //foreach (Edge geomEdge in geomSolid.Edges)
                    //{
                    //    intersection = geomEdge.AsCurve().Project(pt);
                    //    if (intersection != null && intersection.XYZPoint != null
                    //        /*&& pt.IsAlmostEqualTo(intersection.XYZPoint)*/)
                    //    {
                    //        TaskDialog.Show("test", "内積" + ((intersection.XYZPoint - pt).Normalize()).DotProduct(doc.ActiveView.ViewDirection).ToString());
                    //        TaskDialog.Show("test", "edge:" + pt.ToString() + "" + intersection.XYZPoint.ToString());
                    //        result = true;
                    //        //return true;
                    //    }
                    //    else
                    //    {
                    //        continue;
                    //    }
                    //}

                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve1"></param>
        /// <param name="curve2"></param>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public bool GetMEPSystem(MEPCurve curve1, MEPCurve curve2, ref ElementId systemId)
        {
            systemId = null;
            FilteredElementCollector collector
            = new FilteredElementCollector(doc);
            // DB内のElementたちでMEPSystemを取得
            ICollection<Element> mSystems
            = collector.OfClass(typeof(MEPSystem))
            .ToElements();
            // 2つのダクトを含むMEPSystemを探す
            foreach (Element e in mSystems)
            {
                ElementSet tMembers = null;
                MechanicalSystem mSystem = e as MechanicalSystem;
                if(mSystem != null)
                {
                    tMembers = mSystem.DuctNetwork;
                    if (tMembers.Contains(curve1) && tMembers.Contains(curve2))
                    {
                        systemId = mSystem.Id;
                        return true;
                    }
                }
                PipingSystem pSystem = e as PipingSystem;
                if (pSystem != null)
                {
                    tMembers = pSystem.PipingNetwork;
                    if (tMembers.Contains(curve1) && tMembers.Contains(curve2))
                    {
                        systemId = pSystem.Id;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 選択区間の切断後MEPSystemのID取得
        /// （システムの端にあるMEPCurveだけを対象に判断）
        /// </summary>
        /// <param name="curvePair1"></param>
        /// <param name="curvePair2"></param>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public bool GetMEPSystem(MEPCurve[] curvePair1, MEPCurve[] curvePair2, ref ElementId systemId)
        {
            systemId = null;
            FilteredElementCollector collector
            = new FilteredElementCollector(doc);
            // DB内のElementたちでMEPSystemを取得
            ICollection<Element> mSystems
            = collector.OfClass(typeof(MEPSystem))
            .ToElements();
            // 2つのダクトを含むMEPSystemを探す
            foreach (Element e in mSystems)
            {
                MEPSystem mSystem = e as MEPSystem;
                ElementSet tMembers = mSystem.Elements;
                bool isIncludePair1Member = false;
                bool isIncludePair2Member = false;

                foreach (MEPCurve curve in curvePair1)
                {
                    if (tMembers.Contains(curve))
                    {
                        isIncludePair1Member = true;
                        break;
                    }
                }
                foreach (MEPCurve curve in curvePair2)
                {
                    if (tMembers.Contains(curve))
                    {
                        isIncludePair2Member = true;
                        break;
                    }
                }
                if (isIncludePair1Member && isIncludePair2Member)
                {
                    systemId = mSystem.Id;
                    log.Trace("systemId:" + systemId + " systemName:" + mSystem.Name);
                    return true;
                }
            }
            log.Error("選択されたダクトが共通のMEPSystemに含まれていません。");
            return false;
        }

        /// <summary>
        /// 指定の機械システムに指定のダクトIDが含まれているかどうか
        /// </summary>
        /// <param name="ductId"></param>
        /// <param name="mSystemId"></param>
        /// <returns></returns>
        public bool IncludedDuctInMSystem(ElementId ductId, ElementId mSystemId)
        {
            MechanicalSystem mSystem = doc.GetElement(mSystemId) as MechanicalSystem;
            //指定の機械システムがない場合
            if (mSystem == null) throw new Exception("指定の機械システムがありません");
            return mSystem.DuctNetwork.Contains(doc.GetElement(ductId));
        }

        /// <summary>
        /// 指定の配管システムに指定のダクトIDが含まれているかどうか
        /// </summary>
        /// <param name="ductId"></param>
        /// <param name="mSystemId"></param>
        /// <returns></returns>
        public bool IncludedPipeInPSystem(ElementId pipeId, ElementId pSystemId)
        {
            PipingSystem pSystem = doc.GetElement(pSystemId) as PipingSystem;
            //指定の機械システムがない場合
            if (pSystem == null) throw new Exception("指定の配管システムがありません");
            return pSystem.PipingNetwork.Contains(doc.GetElement(pipeId));
        }

        /// <summary>
        /// MEPカーブのパラメータコピー
        /// </summary>
        /// <param name="prmNm"></param>
        /// <param name="curve"></param>
        /// <param name="newCurve"></param>
        /// <returns></returns>
        public bool CopyParam(BuiltInParameter prmNm, MEPCurve curve, MEPCurve newCurve)
        {
            Parameter prm = curve.get_Parameter(prmNm);
            Parameter newPrm = newCurve.get_Parameter(prmNm);
            if(prm != null)
            {
                switch (prm.StorageType)
                {
                    case StorageType.Double:
                        newPrm.Set(prm.AsDouble());
                        break;
                    case StorageType.Integer:
                        newPrm.Set(prm.AsInteger());
                        break;
                    case StorageType.String:
                        newPrm.Set(prm.AsString());
                        break;
                    case StorageType.ElementId:
                        newPrm.Set(prm.AsElementId());
                        break;
                    case StorageType.None:
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// 指定Elementに保温材がある場合にその厚みを返す
        /// ※保温材なしの場合は0
        /// ※単位はFeet!!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public double GetInsulationThickness(Element el)
        {
            double thickness = 0;
            if (el is MEPCurve)
            {
                Parameter insulationParam = ((MEPCurve)el as MEPCurve).get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_THICKNESS);
                if (insulationParam != null)
                {
                    thickness = insulationParam.AsDouble();
                }
            }

            return thickness;
        }

        /// <summary>
        /// ダクト保温材クラス取得
        /// </summary>
        /// <param name="duct"></param>
        /// <returns></returns>
        public DuctInsulation GetDuctInsulation(Duct duct)
        {
            FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(DuctInsulation));
            IList<Element> list = col.ToElements();

            foreach (DuctInsulation e in list)
            {
                if (e.HostElementId.Equals(duct.Id))
                {
                    return e;
                }
            }
            return null;
        }

        /// <summary>
        /// 配管保温材クラス取得
        /// </summary>
        /// <param name="pipe"></param>
        /// <returns></returns>
        public PipeInsulation GetPipeInsulation(Pipe pipe)
        {
            FilteredElementCollector col = new FilteredElementCollector(doc).OfClass(typeof(PipeInsulation));
            IList<Element> list = col.ToElements();

            foreach (PipeInsulation e in list)
            {
                if (e.HostElementId.Equals(pipe.Id))
                {
                    return e;
                }
            }
            return null;
        }

        /// <summary>
        /// 指定ダクトに巻かれている断熱材情報に沿って
        /// 指定されたIDのエレメントにも断熱材を巻く
        /// </summary>
        /// <param name="duct"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CreateDuctInsulation(Duct duct, ElementId id)
        {
            double insulationThick = GetInsulationThickness(duct);
            if (insulationThick <= 0) return true;
            DuctInsulation insulation = GetDuctInsulation(duct);
            if (insulation != null)
            {
                DuctInsulation newInsulation = DuctInsulation.Create(doc, id, insulation.GetTypeId(), insulationThick);
                if(newInsulation == null)
                {
                    log.Error("断熱材作成失敗。");
                    return false;
                }else
                {
                    log.Trace(newInsulation.Thickness.ToString());
                    log.Trace("断熱材作成成功。");
                }
            }
            return true;
        }

        /// <summary>
        /// 指定配管に巻かれている断熱材情報に沿って
        /// 指定されたIDのエレメントにも断熱材を巻く
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CreatePipeInsulation(Pipe pipe, ElementId id)
        {
            double insulationThick = GetInsulationThickness(pipe);
            if (insulationThick <= 0) return true;
            PipeInsulation insulation = GetPipeInsulation(pipe);
            if (insulation != null)
            {
                PipeInsulation newInsulation = PipeInsulation.Create(doc, id, insulation.GetTypeId(), insulationThick);
                if (newInsulation == null)
                {
                    log.Error("断熱材作成失敗。");
                    return false;
                }
                else
                {
                    log.Trace(newInsulation.Thickness.ToString());
                    log.Trace("断熱材作成成功。");
                }
            }
            return true;
        }

        /// <summary>
        /// 指定のコネクタが製造パーツと接続しているか？
        /// </summary>
        /// <param name="con"></param>
        /// <returns>true:製造パーツと接続している
        ///          false:製造パーツとの接続はない</returns>
        public bool IsConnectedToFabPart(Connector con)
        {
            ConnectorSet conAllRefs = con.AllRefs;
            foreach (Connector refCon in conAllRefs)
            {
                FabricationPart fab = refCon.Owner as FabricationPart;
                if(fab != null) return true;
            }
            return false;
        }

        #endregion
    }

    
}
