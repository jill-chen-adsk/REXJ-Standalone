/* ko-mimura
 *  チーズの回転コマンド(ジェネリックパーツ)
 *  使用方法
 *  １．チーズを選択する。
 *  ２．ダイアログが表示されるので、回転角度をインクリメントする。
 *  ３．OKボタン押下で回転確定される。
 */

//#define DEBUG_DRAW
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using MEPCommon;

namespace CmdRotateTees
{
    // ---------------------------------------------------
    internal class ExceptionEsc : System.Exception
    {
    }

    // ---------------------------------------------------
    public interface MainWindowIF
    {
        void MainWindowIF_Rotate(double angle);

        void MainWindowIF_ChangeConnector(System.Windows.Controls.TextBox txtAngle);

        void MainWindowIF_Reset();

        void MainWindowIF_BackupInsulationParameters();

        void MainWindowIF_RestoreInsulationParameters();
    }

    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private MainWindowIF m_IF;

        protected void EraseCloseButtonOnTheToolBar(EventArgs e)
        {
            const int GWL_STYLE = -16;
            const int WS_SYSMENU = 0x80000;
            base.OnSourceInitialized(e);
            IntPtr handle = new WindowInteropHelper(this).Handle;
            int style = GetWindowLong(handle, GWL_STYLE);
            style = style & (~WS_SYSMENU);
            SetWindowLong(handle, GWL_STYLE, style);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            //	EraseCloseButtonOnTheToolBar(e);
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            var uc = Content as UserControl1;
            if (uc.m_bCommit == false)
                m_IF.MainWindowIF_Reset();
        }

        public MainWindow(MainWindowIF IF, Window owner)
        {
            this.Owner = owner;
            this.Left = owner.Left;
            this.Top = owner.Top;
            m_IF = IF;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    // ---------------------------------------------------
    // 不要なワーニングを表示させないようにする。
    public class FailureHandler : IFailuresPreprocessor
    {
        public string ErrorMessage { set; get; }
        public string ErrorSeverity { set; get; }

        public FailureHandler()
        {
            ErrorMessage = "";
            ErrorSeverity = "";
        }

        public FailureProcessingResult PreprocessFailures(
            FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> failureMessages
                = failuresAccessor.GetFailureMessages();
            foreach (FailureMessageAccessor
                 failureMessageAccessor in failureMessages)
            {
                // We're just deleting all of the warning level
                // failures and rolling back any others
                FailureDefinitionId id = failureMessageAccessor
                    .GetFailureDefinitionId();
                try
                {
                    ErrorMessage = failureMessageAccessor
                        .GetDescriptionText();
                }
                catch
                {
                    ErrorMessage = "Unknown Error";
                }
                try
                {
                    FailureSeverity failureSeverity
                        = failureMessageAccessor.GetSeverity();
                    ErrorSeverity = failureSeverity.ToString();
                    if (failureSeverity == FailureSeverity.Warning)
                    {
                        failuresAccessor.DeleteWarning(
                            failureMessageAccessor);
                    }
                    else
                    {
                        return FailureProcessingResult
                            .ProceedWithRollBack;
                    }
                }
                catch
                {
                }
            }
            return FailureProcessingResult.Continue;
        }
    }

    // ---------------------------------------------------
    public abstract class RotateTeesA
    {
        protected enum PIPE_TYPE : int
        {
            PIPE = 0,
            DUCT = 1,
        };

        protected class ConnectedPipe
        {
            public Element m_pipe;
            public Connector m_FamilyInstanceConnector;
            public Connector m_mepCurveConnector;

            public ConnectedPipe(Element pipe, Connector familyInstanceConnector, Connector mepCurveConnector)
            {
                m_pipe = pipe;
                m_mepCurveConnector = mepCurveConnector;
                m_FamilyInstanceConnector = familyInstanceConnector;
            }
        };

        protected int m_connectorIndex = 0;
        protected Transform m_Axis;
        protected PIPE_TYPE m_pipeType;
        protected List<CurveElement> m_guideLines = new List<CurveElement>();
        protected List<Connector> m_connectors;
        protected Dictionary<Connector, Connector> m_connectionOrg = new Dictionary<Connector, Connector>();
        protected TransactionGroup m_transGroup;
        protected List<ConnectedPipe> m_connectedPipes;

        public abstract BoundingBoxXYZ getBoundingBoxInWCS();

        public abstract bool IsTargetElement(Element element);

        public abstract void ExecuteSub();

        protected abstract ConnectorManager GetConnectorManager();

        protected virtual void Disconnect()
        {
            var connector_cons = GetConnectorManager().Connectors;
            foreach (Connector connector_con in connector_cons)
            {
                var type1 = connector_con.ConnectorType.ToString();
                foreach (Connector pipe_con in connector_con.AllRefs)
                {
                    var type2 = pipe_con.ConnectorType.ToString();
                    pipe_con.DisconnectFrom(connector_con);
                    m_connectionOrg.Add(pipe_con, connector_con);
                }
            }
        }

        public void MainWindowIF_Reset()
        {
            m_transGroup.RollBack();
            m_transGroup.Start();
            {
                m_guideLines.Clear();
                m_connectors = GetConnectorsList();
                m_connectorIndex = -1;
            }
            using (var tran = new Transaction(MepCommon.m_doc, "Transaction1"))
            {
                DisableWarning(tran);
                tran.Start();
                {
                    Disconnect();
                }
                tran.Commit();
            }
        }

        public void MainWindowIF_ChangeConnector(System.Windows.Controls.TextBox txtAngle)
        {
            m_connectorIndex++;
            m_Axis = GetCurrentConnectorCoordSystem();
            CreateGuideArrow();
            var c = GetCurrentConnector();
            {
                //txtAngle.IsEnabled = true;
                if (c.Shape == ConnectorProfileType.Rectangular || c.Shape == ConnectorProfileType.Oval)
                {
                    txtAngle.Text = "90";
                    //txtAngle.IsEnabled = false;
                }
            }
        }

        public void UserControl1_IF_Esc()
        {
            m_transGroup.RollBack();
            m_transGroup.Start();
        }

        public void ReConnect()
        {
            foreach (var d in m_connectionOrg)
            {
                try
                {
                    var pipe_con = d.Key;
                    var connector_con = d.Value;
                    pipe_con.ConnectTo(connector_con);
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException e)
                {
                    var str = e.Message;
                }
                catch (Exception e)
                {
                    var str = e.Message;
                }
            }
        }

        protected List<ConnectedPipe> GetConnectedPipes(ConnectorSet connector_cons)
        {
            var connectedPipes = new List<ConnectedPipe>();
            {
                foreach (Connector connector_con in connector_cons)
                {
                    if (connector_con.AllRefs.Size != 1)
                        continue;
                    foreach (Connector pipe_con in connector_con.AllRefs)
                        if (pipe_con.Owner is MEPCurve || pipe_con.Owner is FabricationPart)
                            connectedPipes.Add(new ConnectedPipe(pipe_con.Owner, connector_con, pipe_con));
                }
            }
            return connectedPipes;
        }

        public Connector GetCurrentConnector()
        {
            var idx = m_connectorIndex % m_connectors.Count;
            return m_connectors[idx];
        }

        public Transform GetCurrentConnectorCoordSystem()
        {
            Transform tran = null;
            {
                var con = GetCurrentConnector();
                tran = con.CoordinateSystem;
            }
            return tran;
        }

        virtual protected XYZ GetAxisZ()
        {
            return m_Axis.BasisZ;
        }

        virtual protected XYZ GetAxisX()
        {
            return m_Axis.BasisX;
        }

        virtual protected XYZ GetAxisY()
        {
            return m_Axis.BasisY;
        }

        virtual protected XYZ GetAxisPt()
        {
            return m_Axis.Origin;
        }

        protected void ASSERT(bool b)
        {
            System.Diagnostics.Debug.Assert(b);
        }

        protected void messageBox(string msg)
        {
            System.Windows.MessageBox.Show(msg);
        }

        protected Curve GetCurve(FabricationPart pipe)
        {
            if (!pipe.IsAStraight())
                throw new System.Exception("GetCurve(FabricationPart pipe)");
            var trans = pipe.GetTransform();
            var org = trans.Origin;
            var v = trans.BasisX;
            return Line.CreateUnbound(org, v);
        }

        protected Curve GetCurve(MEPCurve pipe)
        {
            var loc = pipe.Location as LocationCurve;
            var srcCurve = loc.Curve;
            return srcCurve;
        }

        protected XYZ GetDir(Curve curve)
        {
            var deriv = curve.ComputeDerivatives(0, false);// true);
            var v = deriv.BasisX;
            return v.Normalize();
        }

        protected XYZ GetDir(FabricationPart pipe)
        {
            var curve = GetCurve(pipe);
            return GetDir(curve);
        }

        protected XYZ GetDir(MEPCurve pipe)
        {
            var curve = GetCurve(pipe);
            var deriv = curve.ComputeDerivatives(0, true);
            var v = deriv.BasisX;
            return v.Normalize();
        }

        protected XYZ GetDir(Connector a, Connector b)
        {
            var v = a.Origin - b.Origin;
            return v.Normalize();
        }

        protected string GetPipeTypeStr()
        {
            if (m_pipeType == PIPE_TYPE.PIPE)
                return "パイプ";
            return "ダクト";
        }

        public double distanceTo(Plane plane, XYZ p)
        {
            XYZ v = p - plane.Origin;
            return Math.Abs(plane.Normal.DotProduct(v));
        }

        private double mmToFeet(double mm)
        {
            return UnitUtils.Convert(mm, UnitTypeId.Millimeters, UnitTypeId.Feet);
        }

        protected void DisableWarning(Transaction tran)
        {
            FailureHandlingOptions failureHandlingOptions
                = tran.GetFailureHandlingOptions();
            FailureHandler failureHandler
                = new FailureHandler();
            failureHandlingOptions.SetFailuresPreprocessor(
                failureHandler);
            failureHandlingOptions.SetClearAfterRollback(
                true);
            tran.SetFailureHandlingOptions(
                failureHandlingOptions);
        }

        private List<XYZ> GetArrowPts()
        {
            View view = MepCommon.m_uidoc.ActiveView;
            View3D view3d = view as View3D;
            bool IsPerspective = false;
            {
                if (view3d != null)
                    IsPerspective = view3d.IsPerspective;
            }
            var arrowPts = new List<XYZ>();
            {
                double objectPixcelRatio = 0.5;
                {
                    UIView uiview = MepCommon.GetActiveUiView();
                    Rectangle rect = uiview.GetWindowRectangle();
                    var width = rect.Right - rect.Left;
                    IList<XYZ> corners = uiview.GetZoomCorners();
                    var tViewInv = Transform.CreateTranslation(view.Origin);
                    {
                        tViewInv.BasisX = view.RightDirection;
                        tViewInv.BasisY = view.UpDirection;
                        tViewInv.BasisZ = view.ViewDirection;
                        tViewInv = tViewInv.Inverse;
                    }
                    for (int i = 0; i < corners.Count; i++)
                        corners[i] = tViewInv.OfPoint(corners[i]);
                    XYZ a = corners[0];
                    XYZ b = corners[1];
                    objectPixcelRatio = XYZ.BasisX.DotProduct((b - a)) / width;
                    if (IsPerspective)
                    {
                        objectPixcelRatio *= 100.0; // とりああえず。
                    }
                }
                Func<double, double, XYZ> _pt = (x, y) =>
                {
                    const double pix = 30.0;
                    return new XYZ(0, x * pix * objectPixcelRatio, y * pix * objectPixcelRatio);
                };
                arrowPts.Add(_pt(-0.5, 0));
                arrowPts.Add(_pt(0.5, 0));
                arrowPts.Add(_pt(0.5, 3));
                arrowPts.Add(_pt(1, 3));
                arrowPts.Add(_pt(0, 5));
                arrowPts.Add(_pt(-1, 3));
                arrowPts.Add(_pt(-0.5, 3));
            }
            Transform tran = null;
            {
                tran = Transform.CreateTranslation(GetAxisPt());
                tran.BasisX = GetAxisX();
                tran.BasisY = GetAxisY();
                tran.BasisZ = GetAxisZ();
                var tRot = Transform.CreateRotation(tran.BasisZ, Math.PI * 0.5);
                var bRotate = false;
                {
                    Func<Transform, double> _getTriangleArea = (tranTest) =>
                    {
                        var uvTris = new List<UV>();
                        var ptTris = new List<XYZ>();
                        {
                            ptTris.Add(arrowPts[0]);
                            ptTris.Add(arrowPts[1]);
                            ptTris.Add(arrowPts[2]);
                        }
                        var tView = Transform.CreateTranslation(view.Origin);
                        {
                            tView.BasisX = view.RightDirection;
                            tView.BasisY = view.UpDirection;
                            tView.BasisZ = view.ViewDirection;
                        }
                        var viewClippingPlane = Plane.CreateByOriginAndBasis(tView.Origin, tView.BasisX, tView.BasisY);
                        foreach (XYZ ptTri in ptTris)
                        {
                            var ptTri3d = tranTest.OfPoint(ptTri);
                            var ptTriProj = MepCommon.GetProjectedPointOnPlane(viewClippingPlane, ptTri3d);
                            var ptTriProjInv = tView.Inverse.OfPoint(ptTriProj);
                            if (Math.Abs(ptTriProjInv.Z) > MepCommon.tol)
                            { // 1e-3) {
                                throw new System.Exception("z != 0");
                            }
                            uvTris.Add(new UV(ptTriProjInv.X, ptTriProjInv.Y));
                        }
                        var area = Math.Abs(MepCommon.GetSignedPolygonArea(uvTris));
                        return area;
                    };
                    double area1 = 0;
                    {
                        area1 = _getTriangleArea(new Transform(tran));
                    }
                    double area2 = 0;
                    {
                        var tranTest = new Transform(tran);
                        {
                            tranTest.BasisX = tRot.OfPoint(tranTest.BasisX);
                            tranTest.BasisY = tRot.OfPoint(tranTest.BasisY);
                        }
                        area2 = _getTriangleArea(tranTest);
                    }
                    if (area2 > area1)
                        bRotate = true;
                }
                if (bRotate)
                {
                    tran.BasisX = tRot.OfPoint(tran.BasisX);
                    tran.BasisY = tRot.OfPoint(tran.BasisY);
                }
            }
            {
                var tmps = new List<XYZ>();
                {
                    foreach (var pt in arrowPts)
                        tmps.Add(tran.OfPoint(pt));
                }
                arrowPts = tmps;
            }
            return arrowPts;
        }

        private bool IsPerspective()
        {
            View view = MepCommon.m_uidoc.ActiveView;
            View3D view3d = view as View3D;
            bool IsPerspective = false;
            {
                if (view3d != null)
                    IsPerspective = view3d.IsPerspective;
            }
            return IsPerspective;
        }

        private bool Is3D()
        {
            View view = MepCommon.m_uidoc.ActiveView;
            return view is View3D;
        }

        private List<XYZ> GetProjectedPtsOnViewPlane(Plane projPalne, List<XYZ> arrowPts)
        {
            var projPts = new List<XYZ>();
            {
                foreach (var pt in arrowPts)
                {
                    Ray ray = null;
                    {
                        if (IsPerspective())
                        {
                            XYZ eye = null;
                            {
                                var orientation = MepCommon.GetActiveViewAs3D().GetOrientation();
                                eye = orientation.EyePosition;
                            }
                            ray = new Ray(pt, new UnitXYZ(eye - pt));
                        }
                        else
                        {
                            ray = new Ray(pt, new UnitXYZ(projPalne.Normal));
                        }
                    }
                    var ptInt = MepCommon.IntersectPlaneAndRay(projPalne, ray);
                    projPts.Add(ptInt);
#if DEBUG_DRAW
				MepCommon.CreateModelCurve(Line.CreateBound(pt, ptInt));
#endif
                }
            }
            return projPts;
        }

        private void CreateGuideArrowSub()
        {
            var view3d = MepCommon.GetActiveViewAs3D();
            var lines = new List<Line>();
            {
                var arrowPts = GetArrowPts();
                {
                    Plane projPalne = null;
                    {
                        if (Is3D() && IsPerspective())
                        {
                            double advance = 5.0;
                            {
                                XYZ eyePos = null; XYZ eyeDir = null;
                                {
                                    var orientation = view3d.GetOrientation();
                                    eyePos = orientation.EyePosition;
                                    eyeDir = orientation.ForwardDirection;
                                }
                                var target = arrowPts[0];
                                {
                                    var forwards = new List<XYZ>();
                                    {
                                        foreach (var pt in arrowPts)
                                        {
                                            var v = pt - eyePos;
                                            if (eyeDir.DotProduct(v) > 0.0)
                                                forwards.Add(pt);
                                        }
                                    }
                                    if (forwards.Count > 0)
                                        target = MepCommon.GetNearestPt(forwards, eyePos);
                                }
                                var dist = eyePos.DistanceTo(target);
                                advance = dist / 10.0;
                            }
                            projPalne = SelectionUtil.GetProjectPlaneParse(advance);
                            /*
							///////////////////
							var pts = GetProjectedPtsOnViewPlane(projPalne, arrowPts);
							var tView = Transform.CreateTranslation(view3d.Origin); {
								tView.BasisX = view3d.RightDirection;
								tView.BasisY = view3d.UpDirection;
								tView.BasisZ = view3d.ViewDirection;
							}
							var tViewInv = tView.Inverse;
							var ptsInv = new List<XYZ>();
							foreach (var pt in pts)
								ptsInv.Add(tViewInv.OfPoint(pt));
							///////////////////
							*/
                        }
                        else
                        {
                            projPalne = SelectionUtil.GetProjectPlaneIso();
                        }
                    }
                    arrowPts = GetProjectedPtsOnViewPlane(projPalne, arrowPts);
                }
                arrowPts.Add(arrowPts[0]);
                lines = MepCommon.CreateLineSegments(arrowPts);
            }
            foreach (var line in lines)
            {
                try
                {
                    CurveElement c = null;
                    {
                        if (Is3D())
                            c = MepCommon.CreateModelCurve(line);
                        else
                            c = MepCommon.NewDetailCurve(line);
                        GraphicsStyle gs = c.LineStyle as GraphicsStyle;
                        gs.GraphicsStyleCategory.LineColor = new Color(255, 0, 0);
                    }
                    m_guideLines.Add(c);
                }
                catch (System.Exception e)
                {
                    var msg = e.Message;
                    return;
                }
            }
        }

        private void CreateGuideArrow()
        {
            DeleteGuideArrow();
            using (var tran = new Transaction(MepCommon.m_doc, "CreateGuideArrow"))
            {
                DisableWarning(tran);
                tran.Start();
                {
                    CreateGuideArrowSub();
                }
                tran.Commit();
            }
        }

        private void DeleteGuideArrow()
        {
            using (var tran = new Transaction(MepCommon.m_doc, "DeleteGuideArrow"))
            {
                DisableWarning(tran);
                tran.Start();
                {
                    foreach (var elm in m_guideLines)
                        MepCommon.m_doc.Delete(elm.Id);
                    m_guideLines.Clear();
                }
                tran.Commit();
            }
        }

        protected List<Connector> GetConnectorsList()
        {
            // string str = "";
            var cons = new List<Connector>();
            {
                var tmp = new SortedDictionary<double, Connector>();
                {
                    var connectorMan = GetConnectorManager();
                    var e = connectorMan.Connectors.GetEnumerator();
                    while (e.MoveNext())
                    {
                        var con = e.Current as Connector;
                        //str += con.ConnectorType.ToString();
                        //str += "\n";
                        if (con.ConnectorType == ConnectorType.BlankEnd)
                            continue;
                        tmp.Add(con.Id, con);
                    }
                }
                var itr = tmp.GetEnumerator();
                while (itr.MoveNext())
                    cons.Add(itr.Current.Value);
            }
            // MessageBox.Show(str);
            return cons;
        }

        protected void DispDialog(MainWindowIF mainWindowIF)
        {
            Window owner = null;
            {
                var han = MEPCommon.MepCommon.m_uiapp.MainWindowHandle;
                var fromHwnd = System.Windows.Interop.HwndSource.FromHwnd(han);
                owner = fromHwnd.RootVisual as Window;
            }
            var win = new MainWindow(mainWindowIF, owner);
            {
                win.Title = "Tee Rotation Command";
                win.Width = 150;
                win.Height = 150;
                win.Content = new UserControl1(win, mainWindowIF);
                win.WindowStyle = WindowStyle.ToolWindow;
            }
            using (var tran = new Transaction(MepCommon.m_doc, "Transaction1"))
            {
                DisableWarning(tran);
                tran.Start();
                {
                    Disconnect();
                }
                tran.Commit();
            }
            CreateGuideArrow();
            win.ShowDialog();
#if DEBUG_DRAW
#else
            DeleteGuideArrow();
#endif
            if (!(win.Content as UserControl1).m_bCommit)
            {
                throw new ExceptionEsc();
            }
            using (var tran = new Transaction(MepCommon.m_doc, "Transaction2"))
            {
                DisableWarning(tran);
                tran.Start();
                {
#if DEBUG_DRAW
					DebugUtil.DawEyeBeam();
					DebugUtil.DrawViewClippingPlane();
#endif
                    ReConnect();
                }
                tran.Commit();
            }
            // Recreate insulation after user commits the rotation
            using (var tran = new Transaction(MepCommon.m_doc, "Recreate Insulation"))
            {
                DisableWarning(tran);
                tran.Start();
                {
                    mainWindowIF.MainWindowIF_RestoreInsulationParameters();
                }
                tran.Commit();
            }
        }
    };

    [Transaction(TransactionMode.Manual)]
    public class CmdRotateTees : IExternalCommand
    {
        private TransactionGroup m_transGroup;

        private void messageBox(string msg)
        {
            System.Windows.MessageBox.Show(msg);
        }

        private Element PickElement()
        {
            var elms = MepCommon.ConvCollectionToList(MepCommon.m_uidoc.Selection.GetElementIds());
            if (elms.Count == 1)
            {
                var element = MepCommon.m_doc.GetElement(elms[0]);
                MepCommon.m_uidoc.Selection.SetElementIds(new List<ElementId>());
                return element;
            }
            var msg = "Please select a pipe accessory.";
            var reference = MepCommon.m_uidoc.Selection.PickObject(ObjectType.Element, msg);
            return MepCommon.m_uidoc.Document.GetElement(reference);
        }

        // 連続実行
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            MepCommon.Init(commandData);

            //			MepCommon.UnitTest();

            while (true)
            {
                using (m_transGroup = new TransactionGroup(MepCommon.m_doc))
                {
                    m_transGroup.Start("CmdRotateTees::Execute()");
                    try
                    {
                        var element = PickElement();
                        if (element == null)
                        {
                            MessageBox.Show("Please select a pipe accessory.");
                            continue;
                        }
                        var rotateTeesGeneric = new RotateTeesGeneric(m_transGroup);
                        var rotateTeesFabric = new RotateTeesFabric(m_transGroup);
                        var rotateTeesFabricTap = new RotateTeesFabricTap(m_transGroup);
                        if (rotateTeesGeneric.IsTargetElement(element))
                        {
                            rotateTeesGeneric.ExecuteSub();
                        }
                        else if (rotateTeesFabricTap.IsTargetElement(element))
                        {
                            rotateTeesFabricTap.ExecuteSub();
                        }
                        else if (rotateTeesFabric.IsTargetElement(element))
                        {
                            rotateTeesFabric.ExecuteSub();
                        }
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException e)
                    {
                        string mess = e.Message;
                        m_transGroup.RollBack();
                        return Result.Succeeded;
                    }
                    catch (ExceptionEsc e)
                    {
                        string mess = e.Message;
                        m_transGroup.RollBack();
                        return Result.Succeeded;
                    }
                    catch (Exception e)
                    {
                        message = e.Message;
                        m_transGroup.RollBack();
                        return Result.Failed;
                    }
                    m_transGroup.Assimilate();
                }
            }
        }
    }
}