//#define TESTPLOT

#region Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

//using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Mechanical;

using FlexibleDuctMaking;
using WPFView.WPF;

#endregion Namespaces

namespace CmdFlexDuctConvert
{
    // ?O???R?}???h

    #region ?O???R?}???h

    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const double EEPS = 0.000001;                   // ???e??
        private const double EEPS_CONNECTOR = 1.0E-3;           // ???e??(?R?l?N?^?[??u)
        private const double DEFAULT_DIVIDE_LENGTH = 1000.0;    // ????????(mm)
        private const string DESIGN_LENGTH_PARAM = "Design Length";

        private static double divideLength                      // ????????(??????P??)
        {
            get { return UnitUtils.ConvertToInternalUnits(DEFAULT_DIVIDE_LENGTH, UnitTypeId.Millimeters); }
        }

        private List<Element> ductElementList = new List<Element>();            // ?_?N?g????X?g
        private List<Element> ductElementListWithBranch = new List<Element>();  // ?_?N?g????X?g(?I?[??Tee?ACross?????)
        private Connector startConnector = null;    // ?n?[??R?l?N?^?[
        private Duct takeoffFittingDuct = null;     // ?e?C?N?I?t?t?B?b?e?B???O???????????_?N?g
        private Element endTransition = null;       // ?I?[??_?N?g?p??
        private Element brokenDuct = null;          // ??f??u??????_?N?g
        private bool frontPart = false;             // ??f??u???O????????
        private double distanceToBreakPoint = 0.0;  // ??f??u???????

        public class ductElementInfo
        {
            public XYZ pointStart { get; set; } = new XYZ();
            public XYZ pointEnd { get; set; } = new XYZ();
            public List<XYZ> pointMid { get; set; } = new List<XYZ>();
            public Element elem { get; set; } = null;
            public double lengthElem { get; set; } = 0.0;
            public double lengthStart { get; set; } = 0.0;
            public double lengthEnd { get; set; } = 0.0;
            public bool IsChecked { get; set; } = false;
        }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            Selection sel = uidoc.Selection;

            // ??v????????_?C?A???O
            var inputDesignLengthDialog = InputDesignLengthDialog.GetInstance();

            // ??v????????_?C?A???O??\??
            inputDesignLengthDialog.Show(uiapp);

            // ??v????
            double designLength = UnitUtils.ConvertToInternalUnits(
                            inputDesignLengthDialog.designLength,
                            UnitTypeId.Millimeters);

            try
            {
                do
                {
                    while (true)
                    {
                        ductElementList.Clear();
                        ductElementListWithBranch.Clear();
                        startConnector = null;
                        takeoffFittingDuct = null;
                        endTransition = null;
                        brokenDuct = null;
                        frontPart = false;
                        distanceToBreakPoint = 0.0;

                        MechanicalSystem targetMechanicalSystem = null;
                        ElementId targetMechanicalSystemTypeId = null;
                        ConnectorSet connectorSet = null;
                        Element selectedEquipment = null;
                        Element selectedDuct = null;

                        try
                        {
                            if (!inputDesignLengthDialog.isVisible)
                            {
                                break;
                            }

                            // ?@?B????A?????o??????I??????
                            if (selectEquipment(
                                ref selectedEquipment,
                                ref connectorSet,
                                ref targetMechanicalSystem,
                                ref targetMechanicalSystemTypeId,
                                uiapp) != Result.Succeeded)
                            {
                                continue;
                            }

                            // ?R?l?N?^?[???????????
                            if (connectorSet != null && connectorSet.Size > 1)
                            {
                                // ?_?N?g??I??????
                                Result result = selectDuct(
                                    uiapp,
                                    selectedEquipment,
                                    connectorSet,
                                    ref targetMechanicalSystem,
                                    ref targetMechanicalSystemTypeId,
                                    ref selectedDuct);
                                if (selectedDuct == null)
                                {
                                    continue;
                                }
                            }

                            if (selectedEquipment == null
                             || connectorSet == null
                             || targetMechanicalSystem == null
                             || targetMechanicalSystemTypeId == null)
                            {
                                continue;
                            }

                            designLength = UnitUtils.ConvertToInternalUnits(
                                inputDesignLengthDialog.designLength,
                                UnitTypeId.Millimeters);
                            if (lessThanEqual(designLength, 0.0))
                            {
                                continue;
                            }

                            distanceToBreakPoint = designLength;

                            // ?t???L?V?u???_?N?g???
                            if (convertToFlexDuct(
                                uiapp,
                                connectorSet,
                                targetMechanicalSystem,
                                targetMechanicalSystemTypeId,
                                selectedEquipment,
                                selectedDuct) != Result.Succeeded)
                            {
                                continue;
                            }
                        }
                        catch (Autodesk.Revit.Exceptions.OperationCanceledException /*e*/)
                        {
                            inputDesignLengthDialog.dlgHide();
                            break;
                        }
                    }
                } while (inputDesignLengthDialog.dlgVisible());
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                throw;
            }
            finally
            {
                inputDesignLengthDialog.dlgClose();
            }

            return Result.Succeeded;
        }

        #endregion ?O???R?}???h

        // ?w??R?l?N?^?[???N?_?????????????A??_?N?g?Q(?_?N?g?p????)?????????

        #region ?w??R?l?N?^?[???N?_?????????????A??_?N?g?Q(?_?N?g?p????)?????????

        public List<Element> getDuctElementList(
            Connector conn,
            bool withFlexDuct = false,  // true: ?t???L?V?u???_?N?g??????
            bool withBranch = false)    // true: ?_?N?g?p???????
        {
            if (conn == null)
            {
                return null;
            }

            List<Element> elemList = new List<Element>();
            elemList.Add(conn.Owner);
            Connector connPrev = getConnectedConnector(conn);
            if (connPrev != null)
            {
                elemList.Add(connPrev.Owner);
                do
                {
                    connPrev = getConnectedConnectorNext(connPrev.Owner, connPrev);
                    if (connPrev != null)
                    {
                        if ((withFlexDuct && isFlexDuct(connPrev.Owner))
                         || (withBranch && (isTee(connPrev.Owner) || isCross(connPrev.Owner)))
                         || (!isTee(connPrev.Owner) && !isCross(connPrev.Owner)))
                        {
                            elemList.Add(connPrev.Owner);
                        }
                        if (withFlexDuct && elemList.Count > 0 && isFlexDuct(elemList.Last()))
                        {
                            Connector c = getConnectedConnectorNext(connPrev.Owner, connPrev);
                            if (c != null && isTransition(c.Owner))
                            {
                                elemList.Add(c.Owner);
                                break;
                            }
                        }
                        if (isTee(elemList.Last())
                         || isCross(elemList.Last())
                         || isMechanicalEquipment(elemList.Last())
                         || isDuctTerminal(elemList.Last())
                         || isDuctAccessory(elemList.Last())
                         || isTapAdjustable(elemList.Last())
                         || (!withFlexDuct && isFlexDuct(elemList.Last())))
                        {
                            break;
                        }
                    }
                }
                while (connPrev != null);
            }
            return elemList;
        }

        #endregion ?w??R?l?N?^?[???N?_?????????????A??_?N?g?Q(?_?N?g?p????)?????????

        // ?@?B????A?????o??????I??????

        #region ?@?B????A?????o??????I??????

        private Result selectEquipment(
            ref Element selectedEquipment,
            ref ConnectorSet connectorSet,
            ref MechanicalSystem targetMechanicalSystem,
            ref ElementId targetMechanicalSystemTypeId,
            UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            string msg = "Click mechanical equipment or a duct terminal.";
            msg += " Finish with [Close Command] or [ESC].";

            Selection sel = uidoc.Selection;
            Reference r = sel.PickObject(ObjectType.Element, msg);
            selectedEquipment = uidoc.Document.GetElement(r);

            if (!isMechanicalEquipment(selectedEquipment)
             && !isDuctTerminal(selectedEquipment)
             && !isDuctAccessory(selectedEquipment))
            {
                TaskDialog.Show("Error", "No starting connector selected.");
                return Result.Cancelled;
            }

            connectorSet = getTargetConnectors(doc, selectedEquipment);
            if (connectorSet == null || connectorSet.Size == 0)
            {
                TaskDialog.Show("Error", "Starting connector not found.");
                return Result.Cancelled;
            }

            // ?Y??????MechanicalSystem?????????
            if (connectorSet.Size == 1)
            {
                // MechanicalSystem?????????
                targetMechanicalSystem = getMechanicalSystem(doc, selectedEquipment);
                if (targetMechanicalSystem == null)
                {
                    TaskDialog.Show("Error", "Unable to retrieve duct system.");
                    return Result.Cancelled;
                }

                // MechanicalSystem??TypeID?????????
                targetMechanicalSystemTypeId = ((Parameter)targetMechanicalSystem.get_Parameter(BuiltInParameter.SYMBOL_ID_PARAM)).AsElementId();
                if (targetMechanicalSystemTypeId == ElementId.InvalidElementId)
                {
                    TaskDialog.Show("Error", "System type is invalid.");
                    return Result.Cancelled;
                }
            }

            return Result.Succeeded;
        }

        #endregion ?@?B????A?????o??????I??????

        // ?_?N?g??I??????

        #region ?_?N?g??I??????

        private Result selectDuct(
            UIApplication uiapp,
            Element selectedEquipment,
            ConnectorSet connectorSet,
            ref MechanicalSystem targetMechanicalSystem,
            ref ElementId targetMechanicalSystemTypeId,
            ref Element selectedDuct)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            try
            {
                string msg = "Click any duct segment to convert.";
                msg += " Finish with [Close Command] or [ESC].";

                Selection sel = uidoc.Selection;
                Reference r = sel.PickObject(ObjectType.PointOnElement, msg);
                selectedDuct = uidoc.Document.GetElement(r);

                // MechanicalSystem?????????
                targetMechanicalSystem = getMechanicalSystem(doc, selectedEquipment, selectedDuct);
                if (targetMechanicalSystem == null)
                {
                    TaskDialog.Show("Error", "Select a duct on the same run.");
                    return Result.Cancelled;
                }

                // MechanicalSystem??TypeID?????????
                targetMechanicalSystemTypeId = ((Parameter)targetMechanicalSystem.get_Parameter(BuiltInParameter.SYMBOL_ID_PARAM)).AsElementId();
                if (targetMechanicalSystemTypeId == ElementId.InvalidElementId)
                {
                    TaskDialog.Show("Error", "System type is invalid.");
                    return Result.Cancelled;
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException /*ex*/)
            {
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }

        #endregion ?_?N?g??I??????

        // ?t???L?V?u???_?N?g????

        #region ?t???L?V?u???_?N?g????

        private Result convertToFlexDuct(
            UIApplication uiapp,
            ConnectorSet connectorSet,
            MechanicalSystem targetMechanicalSystem,
            ElementId targetMechanicalSystemTypeId,
            Element selectedEquipment,
            Element selectedDuct)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            List<ductElementInfo> mDuctElementInfo = null;
            XYZ startTangent = null;
            XYZ endTangent = null;
            Level level = null;

            // ?_?N?g??????
            if (getDuctElementInfo(
                ref mDuctElementInfo,
                ref startTangent,
                ref endTangent,
                ref level,
                doc,
                connectorSet,
                targetMechanicalSystem,
                selectedDuct) != Result.Succeeded)
            {
                return Result.Cancelled;
            }

            // ????f?[?^(??f??u???????)
            if (lessThanEqual(distanceToBreakPoint, 0.0))
            {
                return Result.Succeeded;
            }

            Element firstDuct = null;
            Element lastDuct = null;
            if (getFirstLastDuct(ref firstDuct, ref lastDuct) != Result.Succeeded)
            {
                return Result.Cancelled;
            }

            // ??????a????
            double ductDiameter = getDuctHydraulicDiameter(firstDuct);

            using (TransactionGroup transGroup = new TransactionGroup(doc, "Create FlexDuct"))
            {
                if (transGroup.Start() == TransactionStatus.Started)
                {
                    try
                    {
                        // ??v?????p?????[?^????i??x??????????A2????~????????j
                        using (Transaction trans = new Transaction(doc))
                        {
                            if (trans.Start("Create project parameter") != TransactionStatus.Started)
                            {
                                return Result.Failed;
                            }

                            Category cat = doc.Settings.Categories.get_Item(BuiltInCategory.OST_FlexDuctCurves);
                            CategorySet cats = app.Create.NewCategorySet();
                            cats.Insert(cat);
                            
                            
                            if (nSetProjectParameter.CSetProjectParameter.CreateProjectParameter(
                                uiapp, DESIGN_LENGTH_PARAM, SpecTypeId.Length, true, cats, GroupTypeId.Geometry, true))
                            {
                                if (TransactionStatus.Committed != trans.Commit())
                                {
                                    throw new FlexDuctConvertException("Failed: Create project parameter");
                                }
                            }
                        }

                        List<XYZ> points = new List<XYZ>();
                        double lengthDesign = 0.0;
                        ConnectorSet breakConnectors = null;
                        CreateMepModelFlexDuct cmmfd = null;

                        // ?t???L?V?u???_?N?g???
                        using (Transaction trans = new Transaction(doc))
                        {
                            if (trans.Start("Create FlexDuct") != TransactionStatus.Started)
                            {
                                return Result.Failed;
                            }

                            // ?t???L?V?u???_?N?g?????_???
                            if (createControlPoints(
                                ref points,
                                ref lengthDesign,
                                ref breakConnectors,
                                doc,
                                mDuctElementInfo) != Result.Succeeded)
                            {
                                return Result.Cancelled;
                            }

                            // ?t???L?V?u???_?N?g???
                            cmmfd = createFlexDuct(
                                doc,
                                level,
                                points,
                                startTangent,
                                endTangent,
                                mDuctElementInfo,
                                targetMechanicalSystem,
                                targetMechanicalSystemTypeId,
                                selectedEquipment,
                                breakConnectors,
                                ductDiameter);

                            if (TransactionStatus.Committed != trans.Commit())
                            {
                                throw new FlexDuctConvertException("Failed: Create FlexDuct");
                            }
                        }

                        // ?t???L?V?u???_?N?g???W
                        using (Transaction trans = new Transaction(doc))
                        {
                            if (trans.Start("Modify control points") != TransactionStatus.Started)
                            {
                                return Result.Failed;
                            }

                            // ?t???L?V?u???_?N?g??s?v?????_???
                            if (modifyControlPoints(
                                doc,
                                level,
                                points,
                                mDuctElementInfo,
                                connectorSet,
                                targetMechanicalSystem,
                                targetMechanicalSystemTypeId,
                                breakConnectors,
                                lengthDesign,
                                ductDiameter,
                                cmmfd) != Result.Succeeded)
                            {
                                return Result.Failed;
                            }

                            if (TransactionStatus.Committed != trans.Commit())
                            {
                                throw new FlexDuctConvertException("Failed: Modify control points");
                            }
                        }

                        transGroup.Assimilate();
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.ToString());
                        transGroup.RollBack();
                        return Result.Failed;
                    }
                }
            }

            return Result.Succeeded;
        }

        #endregion ?t???L?V?u???_?N?g????

        // ?_?N?g???(??)???

        #region ?_?N?g???(??)???

        private List<ductElementInfo> createTempDuctElementInfo(
            List<Element> ductElementList)
        {
            List<ductElementInfo> mDuctElementInfoTemp = new List<ductElementInfo>();

            foreach (Element elem in ductElementList)
            {
                ductElementInfo dtemp = new ductElementInfo();
                dtemp.elem = elem;

                if (isDuct(elem) || isFlexDuct(elem))
                {
                    ConnectorSet connectors = ((MEPCurve)dtemp.elem).ConnectorManager.Connectors;
                    int i = 0;
                    foreach (Connector con in connectors)
                    {
                        if (con == null
                            || (con.ConnectorType != ConnectorType.End
                                /*&& con.ConnectorType != ConnectorType.Curve
                                && con.ConnectorType != ConnectorType.Physical*/))
                        {
                            continue;
                        }

                        if (i == 0)
                        {
                            dtemp.pointStart = con.Origin;
                        }
                        else
                        {
                            dtemp.pointEnd = con.Origin;
                        }
                        ++i;
                    }

                    // ??????Z?o
                    dtemp.lengthElem = dtemp.pointStart.DistanceTo(dtemp.pointEnd);
                }
                else if (isElbow(elem) || isUnion(elem) || isTransition(elem) || isTapAdjustable(elem))
                {
                    ConnectorSet connectors = ((FamilyInstance)dtemp.elem).MEPModel.ConnectorManager.Connectors;
                    int i = 0;
                    foreach (Connector con in connectors)
                    {
                        if (con == null
                            || (con.ConnectorType != ConnectorType.End
                                /*&& con.ConnectorType != ConnectorType.Curve
                                && con.ConnectorType != ConnectorType.Physical*/))
                        {
                            continue;
                        }

                        if (i == 0)
                        {
                            dtemp.pointStart = con.Origin;
                        }
                        else
                        {
                            dtemp.pointEnd = con.Origin;
                        }
                        ++i;
                    }

                    // ?????Z?o
                    if (isUnion(elem) || isTransition(elem) || isTapAdjustable(elem))
                    {
                        dtemp.lengthElem = dtemp.pointStart.DistanceTo(dtemp.pointEnd);
                    }
                    else if (isElbow(elem))
                    {
                        var parameters = elem.Parameters;
                        double radius = 0.0;
                        double angle = 0.0;

                        foreach (Parameter param in parameters)
                        {
                            if (param.Definition.Name == "���S���a")
                            {
                                radius = param.AsDouble();
                            }
                            if (param.Definition.Name == "�p�x")
                            {
                                angle = param.AsDouble();
                            }
                        }
                        dtemp.lengthElem = radius * angle;
                    }
                }
                else
                {
                    Debug.Print("Element category not found. (elem.Category.Name={0}, Index={1}, elem.Id={2}, elem.Name={3})",
                        elem.Category.Name, mDuctElementInfoTemp.Count, elem.Id, elem.Name);
                }

                mDuctElementInfoTemp.Add(dtemp);
            }

            return mDuctElementInfoTemp;
        }

        #endregion ?_?N?g???(??)???

        // ?_?N?g?????(?T?u)

        #region ?_?N?g?????(?T?u)

        private List<ductElementInfo> createDuctElementInfo(
            List<ductElementInfo> mDuctElementInfoTemp,
            XYZ pointStart,
            Element selectedDuct,
            out int result)
        {
            result = 0;
            List<ductElementInfo> mDuctElementInfo = new List<ductElementInfo>();

            int iductCount = 0;
            while (iductCount < mDuctElementInfoTemp.Count)
            {
                ductElementInfo d = new ductElementInfo();
                if (mDuctElementInfo.Count != 0)
                {
                    d.pointStart = mDuctElementInfo[mDuctElementInfo.Count - 1].pointEnd;
                }
                else
                {
                    d.pointStart = pointStart;
                }

                // mDuctElementInfo????
                foreach (ductElementInfo dtemp in mDuctElementInfoTemp)
                {
                    if (dtemp.IsChecked != true)
                    {
                        // ?n?_??????
                        if (equal(d.pointStart, dtemp.pointStart, EEPS_CONNECTOR))
                        {
                            d.pointEnd = dtemp.pointEnd;
                            d.IsChecked = dtemp.IsChecked = true;
                            d.elem = dtemp.elem;

                            d.lengthElem = dtemp.lengthElem;
                            if (mDuctElementInfo.Count == 0)
                            {
                                d.lengthStart = 0.0;
                                d.lengthEnd = d.lengthElem;
                            }
                            else
                            {
                                d.lengthStart = mDuctElementInfo[mDuctElementInfo.Count - 1].lengthEnd;
                                d.lengthEnd = d.lengthStart + d.lengthElem;
                            }

                            mDuctElementInfo.Add(d);
                        }
                        // ?I?_??????
                        else if (equal(d.pointStart, dtemp.pointEnd, EEPS_CONNECTOR))
                        {
                            d.pointEnd = dtemp.pointStart;
                            d.IsChecked = dtemp.IsChecked = true;
                            d.elem = dtemp.elem;

                            d.lengthElem = dtemp.lengthElem;
                            if (mDuctElementInfo.Count == 0)
                            {
                                d.lengthStart = 0.0;
                                d.lengthEnd = d.lengthElem;
                            }
                            else
                            {
                                d.lengthStart = mDuctElementInfo[mDuctElementInfo.Count - 1].lengthEnd;
                                d.lengthEnd = d.lengthStart + d.lengthElem;
                            }

                            mDuctElementInfo.Add(d);
                        }
                    }
                }

                ++iductCount;
            }

            if (selectedDuct != null)
            {
                // ?_?N?g???I????????????A????_?N?g???o?H??????????
                bool selectedDuctFound = false;
                for (int i = 0; i < mDuctElementInfo.Count; ++i)
                {
                    var d = mDuctElementInfo[i];
                    if (d.elem.Id == selectedDuct.Id)
                    {
                        selectedDuctFound = true;
                        break;
                    }
                }
                if (!selectedDuctFound)
                {
                    // ?_?N?g???o?H???????????
                    result = -1;
                    return new List<ductElementInfo>();
                }
            }

            for (int i = 0; i < mDuctElementInfo.Count; ++i)
            {
                // ?t???L?V?u???_?N?g???????o?H??????????
                var flexDuct = mDuctElementInfo[i].elem as FlexDuct;
                if (flexDuct != null)
                {
                    // ?t???L?V?u???_?N?g???????o?H??????????
                    result = -2;
                    return new List<ductElementInfo>();
                }
            }

            for (int i = 0; i < mDuctElementInfo.Count; ++i)
            {
                // ?w???????????f??u??????
                var dei = mDuctElementInfo[i];
                if (lessThanEqual(dei.lengthStart, distanceToBreakPoint) && lessThanEqual(distanceToBreakPoint, dei.lengthEnd))
                {
                    var elem = dei.elem;

                    if (isElbow(elem) || isUnion(elem) || isTransition(elem) || isTapAdjustable(elem))
                    {
                        // ??f????????????
                        // ??f??u???????????????
                        frontPart = false;
                        double center = (dei.lengthStart + dei.lengthEnd) * 0.5;
                        if (isTransition(elem))
                        {
                            frontPart = true;
                        }
                        else
                        {
                            if (lessThanEqual(distanceToBreakPoint, center))
                            {
                                frontPart = true;
                            }
                        }
                        distanceToBreakPoint = (frontPart) ? dei.lengthStart : dei.lengthEnd;
                    }

                    // ??f??u??????_?N?g??????
                    brokenDuct = elem;

                    if (i > 0 && (isTee(dei.elem) || isCross(dei.elem)))
                    {
                        // ????? Tee ????? Cross ???
                        // ???g??~????
                        mDuctElementInfo.RemoveRange(i, mDuctElementInfo.Count - i);
                    }
                    else if (i > 0 && isTransition(dei.elem))
                    {
                        // ??????_?N?g?p????
                        if (i + 1 < mDuctElementInfo.Count
                         && (isDuct(mDuctElementInfo[i + 1].elem) || isElbow(mDuctElementInfo[i + 1].elem)))
                        {
                            // ?_?N?g?p??????_?N?g???
                            // ?_?N?g?p??????~????
                            mDuctElementInfo.RemoveRange(i + 1, mDuctElementInfo.Count - (i + 1));
                        }
                        else
                        {
                            // ?_?N?g?p??????_?N?g??O???
                            // ?_?N?g?p???~????
                            mDuctElementInfo.RemoveRange(i, mDuctElementInfo.Count - i);
                        }
                    }
                    break;
                }
            }

            if (mDuctElementInfo.Count > 0 && lessThan(mDuctElementInfo.Last().lengthEnd, distanceToBreakPoint))
            {
                // ?_?N?g?Q??S??????f??u??????????Z????
                // ??f??u??????????_?N?g?Q??S???????
                distanceToBreakPoint = mDuctElementInfo.Last().lengthEnd;
                if (isTransition(mDuctElementInfo.Last().elem))
                {
                    // ?I?[???_?N?g?p????
                    // ?I?[??_?N?g?p?????
                    mDuctElementInfo.Remove(mDuctElementInfo.Last());
                }
            }

            // ?_?N?g???????_????
            // mDuctElementInfo????pointMid????
            foreach (ductElementInfo d in mDuctElementInfo)
            {
                if (isDuct(d.elem))
                {
                    double elementLength = d.pointStart.DistanceTo(d.pointEnd);

                    int imidCount = (int)(elementLength / divideLength);

                    if (imidCount > 0)
                    {
                        XYZ elementSeparate = (d.pointEnd - d.pointStart) / (imidCount + 1);

                        for (int i = 0; i < imidCount; ++i)
                        {
                            d.pointMid.Add(d.pointStart + (i + 1) * elementSeparate);
                        }
                    }
                }
            }

            return mDuctElementInfo;
        }

        #endregion ?_?N?g?????(?T?u)

        // ?_?N?g?????

        #region ?_?N?g?????

        private List<ductElementInfo> createDuctElementInfo(
            ref List<ductElementInfo> mDuctElementInfo,
            ConnectorSet connectorSet,
            MechanicalSystem targetMechanicalSystem,
            Element selectedDuct)
        {
            int result = 0;
            ConnectorSetIterator csi = connectorSet.ForwardIterator();
            while (csi.MoveNext())
            {
                // ?n?[??R?l?N?^?[
                startConnector = csi.Current as Connector;

                // ????O
                ductElementList = getDuctElementList(startConnector, true/*false*/, false);
                ductElementListWithBranch = getDuctElementList(startConnector, true/*false*/, true);

                // ?_?N?g???(??)???
                List<ductElementInfo> mDuctElementInfoTemp = createTempDuctElementInfo(
                    ductElementList);

                // ?n?[??R?l?N?^?[?????????_?N?g
                ductElementInfo dei = mDuctElementInfoTemp
                    .Where(a => equal(a.pointStart, startConnector.Origin, EEPS_CONNECTOR)
                             || equal(a.pointEnd, startConnector.Origin, EEPS_CONNECTOR))
                    .FirstOrDefault();
                if (dei != null)
                {
                    // ?_?N?g?????(?T?u)
                    mDuctElementInfo = createDuctElementInfo(
                        mDuctElementInfoTemp,
                        startConnector.Origin,
                        selectedDuct,
                        out result);
                    if (mDuctElementInfo.Count > 0)
                    {
                        break;
                    }
                    if (result == -2)
                    {
                        break;
                    }
                }
            }

            if (result == -1)
            {
                TaskDialog.Show("Error", "The duct was not found on the selected run.");
            }
            else if (result == -2)
            {
                TaskDialog.Show("Error", "Flexible duct is already placed on this run.");
            }

            return mDuctElementInfo;
        }

        #endregion ?_?N?g?????

        // ?e?C?N?I?t?t?B?b?e?B???O??(endDuct?????????)?_?N?g????

        #region ?e?C?N?I?t?t?B?b?e?B???O??(endDuct?????????)?_?N?g????

        private Duct getTakeoffFittingDuct(Element takeoff, Duct endDuct)
        {
            Duct duct = null;
            if (isTapAdjustable(takeoff))
            {
                var cs = getConnectors(takeoff);
                ConnectorSetIterator csi = cs.ForwardIterator();
                while (csi.MoveNext())
                {
                    Connector conn = csi.Current as Connector;
                    if (conn == null || !conn.IsConnected)
                    {
                        continue;
                    }

                    var c = getConnectedConnector(conn, true);
                    if (c != null && c.Owner.Id != endDuct.Id)
                    {
                        duct = c.Owner as Duct;
                        break;
                    }
                }
            }
            return duct;
        }

        #endregion ?e?C?N?I?t?t?B?b?e?B???O??(endDuct?????????)?_?N?g????

        // ?_?N?g??????

        #region ?_?N?g??????

        private Result getDuctElementInfo(
            ref List<ductElementInfo> mDuctElementInfo,
            ref XYZ startTangent,
            ref XYZ endTangent,
            ref Level level,
            Document doc,
            ConnectorSet connectorSet,
            MechanicalSystem targetMechanicalSystem,
            Element selectedDuct)
        {
            MEPSystem targetMepSystem = (MEPSystem)targetMechanicalSystem;
            mDuctElementInfo = null;
            startTangent = null;
            endTangent = null;
            level = null;

            // ?_?N?g?????
            mDuctElementInfo = createDuctElementInfo(
                ref mDuctElementInfo,
                connectorSet,
                targetMechanicalSystem,
                selectedDuct);
            if (mDuctElementInfo == null || mDuctElementInfo.Count == 0)
            {
                return Result.Cancelled;
            }

            // ?_?N?g??n?I?[
            ductElementInfo dstart = mDuctElementInfo[0];
            ductElementInfo dend = mDuctElementInfo[mDuctElementInfo.Count - 1];
            Element startJoint = null;
            Element endJoint = null;
            if (getTransitions(mDuctElementInfo, ref startJoint, ref endJoint) != Result.Succeeded)
            {
                return Result.Cancelled;
            }

            if (endJoint != null)
            {
                endTransition = endJoint;
                Element firstDuct = null;
                Element lastDuct = null;
                if (getFirstLastDuct(ref firstDuct, ref lastDuct) != Result.Succeeded)
                {
                    return Result.Cancelled;
                }
                if (isDuct(lastDuct))
                {
                    var duct = lastDuct as Duct;
                    if (isTapAdjustable(endTransition))
                    {
                        takeoffFittingDuct = getTakeoffFittingDuct(endTransition, duct);
                    }
                }
            }

            // ?n?I?[??X??
            startTangent = dstart.pointEnd - dstart.pointStart;
            endTangent = dend.pointEnd - dend.pointStart;

            // ???x??
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            level = (Level)collector.OfClass(typeof(Level)).ToElements().FirstOrDefault();

            return Result.Succeeded;
        }

        #endregion ?_?N?g??????

        // ?_?N?g???

        #region ?_?N?g???

        private static void deleteDucts(Document doc, List<ductElementInfo> deis, int from, int to)
        {
            for (int i = from; i <= to && i < deis.Count; ++i)
            {
                var e = deis[i].elem;
                doc.Delete(e.Id);
            }
        }

        #endregion ?_?N?g???

        // ?t???L?V?u???_?N?g?????_???

        #region ?t???L?V?u???_?N?g?????_???

        private Result createControlPoints(
            ref List<XYZ> points,
            ref double lengthDesign,
            ref ConnectorSet breakConnectors,
            Document doc,
            List<ductElementInfo> mDuctElementInfo)
        {
            Solid solidStartJoint = null;
            Solid solidEndJoint = null;
            var deiFirst = mDuctElementInfo.FirstOrDefault();
            if (deiFirst != null && (isDuct(deiFirst.elem) || isElbow(deiFirst.elem) || isTransition(deiFirst.elem)))
            {
                Element startJoint = deiFirst.elem;
                solidStartJoint = getSolid(startJoint, true);
            }

            Element after = null;

            points = new List<XYZ>();
            XYZ pointBreak = null;
            lengthDesign = 0.0;
            breakConnectors = null;

            for (int i = 0; i < mDuctElementInfo.Count; ++i)
            {
                ductElementInfo dei = mDuctElementInfo[i];
                var elem = dei.elem;
                XYZ end0 = dei.pointStart;
                XYZ end1 = dei.pointEnd;

                if (isDuct(elem) || isTransition(elem) || isTapAdjustable(elem) || isUnion(elem) || isDuctAccessory(elem))
                {
                    // ?n?_
                    if (points.Count == 0)
                    {
                        points.Add(dei.pointStart);
                    }

#if (DEBUG && TESTPLOT)
    CreateLineDirectShape(doc, end0, end1);
    CreateSphereDirectShape(doc, end0, 50.0);
    CreateSphereDirectShape(doc, end1, 50.0);
#endif

                    if (brokenDuct != null && elem.Id == brokenDuct.Id)
                    {
                        // ??f????
                        if ((pointBreak = setBreakPoint(
                            ref lengthDesign,
                            ref points,
                            doc,
                            mDuctElementInfo,
                            i)) == null)
                        {
                            return Result.Cancelled;
                        }
#if (DEBUG && TESTPLOT)
    CreateSphereDirectShape(doc, pointBreak, 20.0, true, new Color(0, 0, 255));
#endif

                        if (lessThan(distanceToBreakPoint, dei.lengthEnd))
                        {
                            // ?????u????f
                            if (isDuct(elem))
                            {
                                setBreakLine(
                                    ref breakConnectors,
                                    ref after,
                                    doc,
                                    mDuctElementInfo,
                                    i,
                                    pointBreak);
                            }
                            else
                            {
                                setBreakLine(
                                    ref lengthDesign,
                                    ref breakConnectors,
                                    ref after,
                                    ref points,
                                    doc,
                                    mDuctElementInfo,
                                    i);
                            }
                        }
                        else
                        {
                            // ?I?[??u????f
                            // ???_?N?g??R?l?N?^?[
                            if (i + 1 < mDuctElementInfo.Count)
                            {
                                breakConnectors = getConnectors(mDuctElementInfo[i + 1].elem);
                                after = mDuctElementInfo[i + 1].elem;
                            }

                            // ?????_?N?g????
                            deleteDucts(doc, mDuctElementInfo, 0, i);
                            doc.Regenerate();
                        }
                        break;
                    }

                    // ??f???
                    // ????_
                    addLineMidPoints(points, elem, end0, end1);

                    // ?I?_
                    points.Add(end1);

                    lengthDesign = mDuctElementInfo[i].lengthEnd;
                }
                else if (isElbow(elem))
                {
                    // ?n?_
                    if (points.Count == 0)
                    {
                        points.Add(dei.pointStart);
                    }

#if (DEBUG && TESTPLOT)
    //FamilyInstance fi = elem as FamilyInstance;
    //var lp = fi.Location as LocationPoint;
    //CreateArcDirectShape(doc, end0, end1, lp.Point, getRadius(elem));
    CreateSphereDirectShape(doc, end0, 50.0);
    CreateSphereDirectShape(doc, end1, 50.0);
#endif

                    if ((brokenDuct != null && elem.Id == brokenDuct.Id))
                    {
                        // ??f????
                        setBreakArc(
                            ref lengthDesign,
                            ref breakConnectors,
                            ref after,
                            ref points,
                            doc,
                            mDuctElementInfo,
                            i);
                        break;
                    }

                    // ??f???
                    // ????_
                    FamilyInstance fi = elem as FamilyInstance;
                    var lp = fi.Location as LocationPoint;
                    if (!equal(getRadius(elem), 0.0))
                    {
                        addArcMidPoints(points, elem, end0, end1);
                    }
                    else
                    {
                        addLineMidPoints(points, elem, end0, lp.Point);
                        addLineMidPoints(points, elem, lp.Point, end1);
                    }

                    // ?I?_
                    points.Add(end1);

                    lengthDesign = mDuctElementInfo[i].lengthEnd;
                }
            }

            // ??f??????????????S?_?N?g???
            if (brokenDuct == null)
            {
                if (mDuctElementInfo.Count > 0)
                {
                    int n = mDuctElementInfo.Count;
                    if (isTapAdjustable(mDuctElementInfo.Last().elem))
                    {
                        // ?I?[?? TapAdjustable ????????
                        --n;
                    }
                    if (n > 0)
                    {
                        deleteDucts(doc, mDuctElementInfo, 0, n - 1);
                        doc.Regenerate();
                    }
                }
            }

            // ??????v?f
            if (after != null)
            {
                solidEndJoint = getSolid(after, true);
            }

            // ?_?N?g?p??}??????G???[??????A???O??n?I?[?t???s?v?????_???????????
            List<XYZ> delPoints = new List<XYZ>();
            foreach (var pt in points)
            {
                if (pt != points[0] && pt != points[points.Count - 1])
                {
                    if (intersect(doc, solidStartJoint, pt) || intersect(doc, solidEndJoint, pt))
                    {
                        delPoints.Add(pt);
                    }
                }
            }
            foreach (var pt in delPoints)
            {
                points.Remove(pt);
            }

            if (points.Count < 2)
            {
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }

        #endregion ?t???L?V?u???_?N?g?????_???

        // ?t???L?V?u???_?N?g??s?v?????_???

        #region ?t???L?V?u???_?N?g??s?v?????_???

        private Result modifyControlPoints(
            Document doc,
            Level level,
            List<XYZ> points,
            List<ductElementInfo> mDuctElementInfo,
            ConnectorSet connectorSet,
            MechanicalSystem targetMechanicalSystem,
            ElementId targetMechanicalSystemTypeId,
            ConnectorSet breakConnectors,
            double lengthDesign,
            double ductDiameter,
            CreateMepModelFlexDuct cmmfd)
        {
            Element startJoint = null;
            Element endJoint = null;
            if (getTransitions(ref startJoint, ref endJoint) != Result.Succeeded)
            {
                return Result.Cancelled;
            }

#if (DEBUG && TESTPLOT)
    drawSolid(doc, startJoint, true);
    drawSolid(doc, endJoint, true);
#endif

            // ?n?I?[?t???s?v?????_????????
            IList<XYZ> pts = cmmfd.flexduct.Points;
            List<XYZ> delPoints = new List<XYZ>();
            foreach (XYZ pt in pts)
            {
                if (pt == pts.First() || pt == pts.Last())
                {
                    continue;
                }

                bool foundPt = false;
                if (startJoint != null && isTransition(startJoint))
                {
                    var con0 = getConnector(startJoint, 0);
                    var con1 = getConnector(startJoint, 1);
                    if ((con0 != null && equal(pt, con0.Origin))
                     || (con1 != null && equal(pt, con1.Origin)))
                    {
                    }
                    else
                    {
                        if (intersect(doc, startJoint, pt))
                        {
                            foundPt = true;
                        }
                    }
                }
                if (endJoint != null && isTransition(endJoint))
                {
                    var con0 = getConnector(endJoint, 0);
                    var con1 = getConnector(endJoint, 1);
                    if ((con0 != null && equal(pt, con0.Origin))
                     || (con1 != null && equal(pt, con1.Origin)))
                    {
                    }
                    else
                    {
                        if (intersect(doc, endJoint, pt))
                        {
                            foundPt = true;
                        }
                    }
                }
                if (foundPt)
                {
                    delPoints.Add(pt);
                }
            }

            foreach (var pt in delPoints)
            {
                pts.Remove(pt);
            }

            // ?S???? divideLength ??????????A????_????????
            delPoints.Clear();
            if (cmmfd.flexduct.Location is LocationCurve)
            {
                Curve curve = (cmmfd.flexduct.Location as LocationCurve).Curve;
                if (lessThan(curve.Length, divideLength))
                {
                    foreach (XYZ pt in pts)
                    {
                        if (pt == pts.First() || pt == pts.Last())
                        {
                            continue;
                        }
                        delPoints.Add(pt);
                    }

                    foreach (var pt in delPoints)
                    {
                        pts.Remove(pt);
                    }
                }
            }

            if (pts.Count > 0)
            {
                cmmfd.flexduct.Points = pts;
            }

            // ?p?????[?^???
            setParameter(cmmfd.flexduct, DESIGN_LENGTH_PARAM, (double)lengthDesign);

            return Result.Succeeded;
        }

        #endregion ?t???L?V?u???_?N?g??s?v?????_???

        // ?n?I?[??_?N?g????

        #region ?n?I?[??_?N?g????

        private Result getFirstLastDuct(
            ref Element first,
            ref Element last)
        {
            first = null;
            last = null;
            var ducts = ductElementListWithBranch;
            if (ducts.Count == 0)
            {
                return Result.Cancelled;
            }

            foreach (Element duct in ducts)
            {
                if (isDuct(duct))
                {
                    if (first == null)
                    {
                        first = duct;
                    }
                    last = duct;
                }
            }

            if (first != null)
            {
                return Result.Succeeded;
            }
            return Result.Cancelled;
        }

        #endregion ?n?I?[??_?N?g????

        // ?n?I?[??_?N?g?p?????

        #region ?n?I?[??_?N?g?p?????

        private Result getTransitions(
            List<ductElementInfo> mDuctElementInfo,
            ref Element first,
            ref Element last)
        {
            List<Element> ducts = new List<Element>();
            foreach (var dei in mDuctElementInfo)
            {
                ducts.Add(dei.elem);
            }
            if (ducts.Count == 0)
            {
                return Result.Cancelled;
            }

            // ?n?[??_?N?g?p??
            if (isTransition(ducts.First()) || isTapAdjustable(ducts.First()) || isDuctAccessory(ducts.First()))
            {
                first = ducts.First();
            }
            else if (ducts.Count > 1 && (isTransition(ducts[1]) || isTapAdjustable(ducts[1]) || isDuctAccessory(ducts[1])))
            {
                first = ducts[1];
            }

            // ?I?[??_?N?g?p??
            if (isTransition(ducts.Last()) || isTapAdjustable(ducts.Last()) || isDuctAccessory(ducts.Last()))
            {
                last = ducts.Last();
            }
            else if (ducts.Count > 1 && (isTransition(ducts[ducts.Count - 2]) || isTapAdjustable(ducts[ducts.Count - 2]) || isDuctAccessory(ducts[ducts.Count - 2])))
            {
                last = ducts[ducts.Count - 2];
            }

            return Result.Succeeded;
        }

        private Result getTransitions(
            ref Element first,
            ref Element last)
        {
            var ducts = ductElementListWithBranch;
            if (ducts.Count == 0)
            {
                return Result.Cancelled;
            }

            // ?n?[??_?N?g?p??
            if (isTransition(ducts.First()) || isTapAdjustable(ducts.First()) || isDuctAccessory(ducts.First()))
            {
                first = ducts.First();
            }
            else if (ducts.Count > 1 && (isTransition(ducts[1]) || isTapAdjustable(ducts[1]) || isDuctAccessory(ducts[1])))
            {
                first = ducts[1];
            }

            // ?I?[??_?N?g?p??
            if (isTransition(ducts.Last()) || isTapAdjustable(ducts.Last()) || isDuctAccessory(ducts.Last()))
            {
                last = ducts.Last();
            }
            else if (ducts.Count > 1 && (isTransition(ducts[ducts.Count - 2]) || isTapAdjustable(ducts[ducts.Count - 2]) || isDuctAccessory(ducts[ducts.Count - 2])))
            {
                last = ducts[ducts.Count - 2];
            }

            return Result.Succeeded;
        }

        #endregion ?n?I?[??_?N?g?p?????

        // ?t???L?V?u???_?N?g???

        #region ?t???L?V?u???_?N?g???

        private CreateMepModelFlexDuct createFlexDuct(
            Document doc,
            Level level,
            List<XYZ> points,
            XYZ startTangent,
            XYZ endTangent,
            List<ductElementInfo> mDuctElementInfo,
            MechanicalSystem targetMechanicalSystem,
            ElementId targetMechanicalSystemTypeId,
            Element selectedEquipment,
            ConnectorSet breakConnectors,
            double ductDiameter)
        {
            // ?t???L?V?u???_?N?g???
            CreateMepModelFlexDuct cmmfd = new CreateMepModelFlexDuct();
            Result result = cmmfd.createmepmodel(doc, level, points, startTangent, endTangent, targetMechanicalSystemTypeId);
            if (result != Result.Succeeded)
            {
                return null;
            }

            // ???a????
            setDuctDiameter(cmmfd.flexduct, ductDiameter);

            // ?R?l?N?^?[????
            ConnectorSet startConnectors = getConnectors(selectedEquipment);
            ConnectorSet flexDuctConnectors = getConnectors(cmmfd.flexduct);
            // ?n?[(?@?B????A?????o????)?{?t???L?V?u???_?N?g
            Element startFI = connectTo(doc, flexDuctConnectors, startConnectors);
            // ??f?????????I?[
            Element endFI = null;
            try
            {
                if (breakConnectors != null)
                {
                    // ??f????
                    // ?t???L?V?u???_?N?g?{??f???c?????_?N?g
                    endFI = connectTo(doc, flexDuctConnectors, breakConnectors);
                }
                else
                {
                    // ?t???L?V?u???_?N?g?{?I?[
                    if (endTransition != null && endTransition.IsValidObject)
                    {
                        ConnectorSet endConnectors = getTargetConnectors(doc, endTransition);
                        if (endConnectors != null && endConnectors.Size > 0)
                        {
                            var cs = getConnectors(endTransition);
                            ConnectorSetIterator csi = cs.ForwardIterator();
                            while (csi.MoveNext())
                            {
                                Connector conn = csi.Current as Connector;
                                var c = getConnectedConnector(conn, true);
                                if (c != null && c.Owner.Id != endTransition.Id && c.Owner.Id != cmmfd.flexduct.Id)
                                {
                                    if ((endFI = connectTo(doc, flexDuctConnectors, c, takeoffFittingDuct)) != null)
                                    {
                                        doc.Delete(endTransition.Id);
                                        doc.Regenerate();
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        endFI = connectTo(doc, flexDuctConnectors, takeoffFittingDuct);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                //TaskDialog.Show("Error", "?t???L?V?u???_?N?g??I?[?????????????B");
            }

            // ?????
            ductElementList = getDuctElementList(startConnector, true, false);
            ductElementListWithBranch = getDuctElementList(startConnector, true, true);

            return cmmfd;
        }

        #endregion ?t???L?V?u???_?N?g???

        // ??f????????

        #region ??f????????

        // ?_????(??f?s??)???f??u???????
        private XYZ setBreakPoint(
            ref double lengthDesign,
            ref List<XYZ> points,
            Document doc,
            List<ductElementInfo> mDuctElementInfo,
            int i)
        {
            ductElementInfo dei = mDuctElementInfo[i];
            Element elem = dei.elem;
            XYZ end0 = dei.pointStart;
            XYZ end1 = dei.pointEnd;

            // ????_(??f??u)
            double lengthBreak = dei.lengthElem - (dei.lengthEnd - distanceToBreakPoint);
            bool reverse = getStartConnectorIndex(dei) == 1;
            if (reverse)
            {
                lengthBreak = dei.lengthElem - lengthBreak;
                XYZ tmp = end0;
                end0 = end1;
                end1 = tmp;
            }

            Curve curve = null;
            if (elem.Location is LocationCurve)
            {
                curve = (elem.Location as LocationCurve).Curve;
            }
            else if (elem.Location is LocationPoint)
            {
                curve = Line.CreateBound(end0, end1);
            }
            else
            {
                return null;
            }

            XYZ pointBreak = curve.Evaluate(lengthBreak / dei.lengthElem, true);

            addLineMidPoints(points, elem, dei.pointStart, pointBreak);

            if (!isTransition(elem) && !isTapAdjustable(elem))
            {
                // ?I?_
                points.Add(pointBreak);
            }

            lengthDesign = distanceToBreakPoint;

            return pointBreak;
        }

        // ????????(??f??)???f??u???????
        private void setBreakLine(
            ref ConnectorSet breakConnectors,
            ref Element after,
            Document doc,
            List<ductElementInfo> mDuctElementInfo,
            int i,
            XYZ pointBreak)
        {
            breakConnectors = null;
            after = null;
            ductElementInfo dei = mDuctElementInfo[i];
            Element elem = dei.elem;
            //XYZ end0 = dei.pointStart;
            XYZ end1 = dei.pointEnd;

            var newElemId = MechanicalUtils.BreakCurve(doc, elem.Id, pointBreak);
            doc.Regenerate();

            // ??f???s?v??_?N?g???
            var oldElemId = elem.Id;
            var conOld0 = getConnector(doc.GetElement(oldElemId), 0);
            var conOld1 = getConnector(doc.GetElement(oldElemId), 1);
            var conNew0 = getConnector(doc.GetElement(newElemId), 0);
            var conNew1 = getConnector(doc.GetElement(newElemId), 1);
            if (equal(conOld0.Origin, end1, EEPS_CONNECTOR) || equal(conOld1.Origin, end1, EEPS_CONNECTOR))
            {
                // ??????_?N?g???c???A?V?K??_?N?g????????
                elem = doc.GetElement(oldElemId);
                doc.Delete(newElemId);
            }
            else if (equal(conNew0.Origin, end1, EEPS_CONNECTOR) || equal(conNew1.Origin, end1, EEPS_CONNECTOR))
            {
                // ?V?K??_?N?g???c???A??????_?N?g????????
                elem = doc.GetElement(newElemId);
                doc.Delete(oldElemId);
            }

            // ?c?????_?N?g??R?l?N?^?[?????_?N?g
            breakConnectors = getConnectors(elem);
            after = elem;

            // ?O??_?N?g????
            deleteDucts(doc, mDuctElementInfo, 0, i - 1);
            doc.Regenerate();
        }

        // ????????(??f?s??)???f??u???????
        private void setBreakLine(
            ref double lengthDesign,
            ref ConnectorSet breakConnectors,
            ref Element after,
            ref List<XYZ> points,
            Document doc,
            List<ductElementInfo> mDuctElementInfo,
            int i)
        {
            lengthDesign = 0.0;
            breakConnectors = null;
            after = null;
            ductElementInfo dei = mDuctElementInfo[i];
            Element elem = dei.elem;
            XYZ end0 = dei.pointStart;
            XYZ end1 = dei.pointEnd;
            XYZ pointBreak = (frontPart) ? end0 : end1;

#if (DEBUG && TESTPLOT)
    Curve curve = Line.CreateBound(end0, end1);
    double elementLength = curve.Length;
    XYZ p = curve.Evaluate((distanceToBreakPoint - dei.lengthStart) / dei.lengthElem, true);
    CreateSphereDirectShape(doc, p, 20.0, true, new Color(255, 0, 255));
    CreateSphereDirectShape(doc, pointBreak, 20.0, true, new Color(0, 0, 255));
#endif

            if (frontPart)
            {
                // ??f??u???O?????????
                if (i > 0)
                {
                    // ?I?_
                    points.Add(pointBreak);

                    lengthDesign = mDuctElementInfo[i - 1].lengthEnd;

                    // ??f?????_?N?g??R?l?N?^?[?????_?N?g
                    breakConnectors = getConnectors(mDuctElementInfo[i].elem);
                    after = mDuctElementInfo[i].elem;

                    // ?O??_?N?g????
                    deleteDucts(doc, mDuctElementInfo, 0, i - 1);
                    doc.Regenerate();
                }
            }
            else
            {
                // ??f??u???????????
                // ????_
                addLineMidPoints(points, elem, end0, end1);

                // ?I?_
                points.Add(pointBreak);

                lengthDesign = mDuctElementInfo[i].lengthEnd;

                // ??f?????_?N?g??R?l?N?^?[?????_?N?g
                if (i + 1 < mDuctElementInfo.Count)
                {
                    breakConnectors = getConnectors(mDuctElementInfo[i + 1].elem);
                    after = mDuctElementInfo[i + 1].elem;
                }

                // ?????_?N?g????
                deleteDucts(doc, mDuctElementInfo, 0, i);
                doc.Regenerate();
            }
        }

        // ???????(??f?s??)???f??u???????
        private void setBreakArc(
            ref double lengthDesign,
            ref ConnectorSet breakConnectors,
            ref Element after,
            ref List<XYZ> points,
            Document doc,
            List<ductElementInfo> mDuctElementInfo,
            int i)
        {
            lengthDesign = 0.0;
            breakConnectors = null;
            after = null;
            ductElementInfo dei = mDuctElementInfo[i];
            Element elem = dei.elem;
            XYZ end0 = dei.pointStart;
            XYZ end1 = dei.pointEnd;
            Arc arc = createArc(elem, end0, end1);
            XYZ pointBreak = (frontPart) ? end0 : end1;

#if (DEBUG && TESTPLOT)
    double elementLength = arc.Length;
    XYZ p = arc.Evaluate((distanceToBreakPoint - dei.lengthStart) / dei.lengthElem, true);
    CreateSphereDirectShape(doc, p, 20.0, true, new Color(255, 0, 255));
    CreateSphereDirectShape(doc, pointBreak, 20.0, true, new Color(0, 0, 255));
#endif

            if (frontPart)
            {
                // ??f??u???O?????????
                if (i > 0)
                {
                    // ?I?_
                    points.Add(end0);

                    lengthDesign = mDuctElementInfo[i - 1].lengthEnd;

                    // ??f?????_?N?g??R?l?N?^?[?????_?N?g
                    breakConnectors = getConnectors(mDuctElementInfo[i].elem);
                    after = mDuctElementInfo[i].elem;

                    // ?O??_?N?g????
                    deleteDucts(doc, mDuctElementInfo, 0, i - 1);
                    doc.Regenerate();
                }
            }
            else
            {
                // ??f??u???????????
                // ????_
                addArcMidPoints(points, elem, arc.GetEndPoint(0), arc.GetEndPoint(1));

                // ?I?_
                points.Add(pointBreak);

                lengthDesign = mDuctElementInfo[i].lengthEnd;

                // ??f?????_?N?g??R?l?N?^?[?????_?N?g
                if (i + 1 < mDuctElementInfo.Count)
                {
                    breakConnectors = getConnectors(mDuctElementInfo[i + 1].elem);
                    after = mDuctElementInfo[i + 1].elem;
                }

                // ?????_?N?g????
                deleteDucts(doc, mDuctElementInfo, 0, i);
                doc.Regenerate();
            }
        }

        #endregion ??f????????

        // ?_?N?g??n?[???????R?l?N?^?[??C???f?b?N?X

        #region ?_?N?g??n?[???????R?l?N?^?[??C???f?b?N?X

        private int getStartConnectorIndex(ductElementInfo dei)
        {
            var conn0 = getConnector(dei.elem, 0);
            var conn1 = getConnector(dei.elem, 1);
            if (conn0 != null && equal(conn0.Origin, dei.pointStart, EEPS_CONNECTOR))
            {
                return 0;
            }
            else if (conn1 != null && equal(conn1.Origin, dei.pointStart, EEPS_CONNECTOR))
            {
                return 1;
            }
            return -1;
        }

        #endregion ?_?N?g??n?[???????R?l?N?^?[??C???f?b?N?X

        // ??????u?????R?l?N?^?[????

        #region ??????u?????R?l?N?^?[????

        private Element connectTo(
            Document doc,
            ConnectorSet conSet0,
            ConnectorSet conSet1,
            Duct duct = null)
        {
            if (conSet0 == null || conSet1 == null)
            {
                return null;
            }

            foreach (Connector con0 in conSet0)
            {
                if (con0 == null
                 || (con0.ConnectorType != ConnectorType.End
                    && !(isTapAdjustable(con0.Owner) && con0.ConnectorType == ConnectorType.Curve)
                    /*&& con0.ConnectorType != ConnectorType.Physical*/))
                {
                    continue;
                }

                foreach (Connector con1 in conSet1)
                {
                    if (con1 == null
                     || (con1.ConnectorType != ConnectorType.End
                        && !(isTapAdjustable(con1.Owner) && con1.ConnectorType == ConnectorType.Curve)
                        /*&& con1.ConnectorType != ConnectorType.Physical*/))
                    {
                        continue;
                    }

                    if (equal(con0.Origin, con1.Origin, EEPS_CONNECTOR))
                    {
                        if (duct != null)
                        {
                            return doc.Create.NewTakeoffFitting(con0, duct);
                        }
                        else
                        {
                            var fi = doc.Create.NewTransitionFitting(con0, con1);
                            if (fi == null)
                            {
                                //fi = doc.Create.NewUnionFitting(con0, con1);
                                var c = getConnectedConnector(con0);
                                if (c != null)
                                {
                                    con0.DisconnectFrom(c);
                                }
                                con0.ConnectTo(con1);
                                return con1.Owner;
                            }
                            return fi;
                        }
                    }
                }
            }

            return null;
        }

        private Element connectTo(
            Document doc,
            ConnectorSet conSet0,
            Connector con1,
            Duct duct = null)
        {
            if (conSet0 == null || con1 == null)
            {
                return null;
            }

            Connector con0 = null;
            double minDist = double.MaxValue;
            foreach (Connector c in conSet0)
            {
                double d = c.Origin.DistanceTo(con1.Origin);
                if (d < minDist)
                {
                    con0 = c;
                    minDist = d;
                }
            }

            if (con0 != null)
            {
                if (duct != null)
                {
                    return doc.Create.NewTakeoffFitting(con0, duct);
                }
                else
                {
                    var fi = doc.Create.NewTransitionFitting(con0, con1);
                    if (fi == null)
                    {
                        //fi = doc.Create.NewUnionFitting(con0, con1);
                        var c = getConnectedConnector(con0);
                        if (c != null)
                        {
                            con0.DisconnectFrom(c);
                        }
                        con0.ConnectTo(con1);
                        return con1.Owner;
                    }
                    return fi;
                }
            }

            return null;
        }

        private Element connectTo(
            Document doc,
            ConnectorSet conSet,
            Duct duct = null)
        {
            if (conSet == null)
            {
                return null;
            }

            foreach (Connector con0 in conSet)
            {
                foreach (var elem in ductElementListWithBranch)
                {
                    if (!elem.IsValidObject)
                    {
                        continue;
                    }

                    ConnectorSet conSet1 = getConnectors(elem);
                    foreach (Connector con1 in conSet1)
                    {
                        if (!con1.Owner.IsValidObject)
                        {
                            continue;
                        }

                        if (equal(con0.Origin, con1.Origin, EEPS_CONNECTOR))
                        {
                            if (duct != null)
                            {
                                return doc.Create.NewTakeoffFitting(con0, duct);
                            }
                            else
                            {
                                var fi = doc.Create.NewTransitionFitting(con0, con1);
                                if (fi == null)
                                {
                                    //fi = doc.Create.NewUnionFitting(con0, con1);
                                    var c = getConnectedConnector(con0);
                                    if (c != null)
                                    {
                                        con0.DisconnectFrom(c);
                                    }
                                    con0.ConnectTo(con1);
                                    return con1.Owner;
                                }
                                return fi;
                            }
                        }
                    }
                }
            }

            return null;
        }

        #endregion ??????u?????R?l?N?^?[????

        // ?R?l?N?^?[??`??

        #region ?R?l?N?^?[??`??

        private static void drawConnectors(Document doc, ConnectorSet connectors)
        {
            if (connectors != null)
            {
                foreach (Connector c in connectors)
                {
                    Solid solid = CreateSphereDirectShape(doc, c.Origin, 10, false);
                    DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                    try
                    {
                        ds.SetShape(new GeometryObject[] { solid });
                    }
                    catch (Autodesk.Revit.Exceptions.ArgumentException ex)
                    {
                        Debug.Print("Failed to drawConnectors; exception {0} {1}", ex.GetType().FullName, ex.Message);
                    }
                }
            }
        }

        #endregion ?R?l?N?^?[??`??

        // ?o?E???f?B???O?{?b?N?X??`??

        #region ?o?E???f?B???O?{?b?N?X??`??

        private static DirectShape drawBoundingBox(Document doc, BoundingBoxXYZ bbox)
        {
            if (bbox == null)
            {
                return null;
            }
            Solid solid = createSolidFromBoundingBox(bbox);
            DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
            try
            {
                ds.SetShape(new GeometryObject[] { solid });
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException ex)
            {
                Debug.Print("Failed to drawBoundingBox; exception {0} {1}", ex.GetType().FullName, ex.Message);
            }
            return ds;
        }

        #endregion ?o?E???f?B???O?{?b?N?X??`??

        // ?\???b?h??`??

        #region ?\???b?h??`??

        private static DirectShape drawSolid(Document doc, Element elem, bool isBbox = false)
        {
            if (elem == null)
            {
                return null;
            }
            Solid solid = getSolid(elem, isBbox);
            DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
            try
            {
                ds.SetShape(new GeometryObject[] { solid });
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException ex)
            {
                Debug.Print("Failed to drawSolid; exception {0} {1}", ex.GetType().FullName, ex.Message);
            }
            return ds;
        }

        private static DirectShape drawSolid(Document doc, Solid solid)
        {
            if (solid == null)
            {
                return null;
            }
            DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
            try
            {
                ds.SetShape(new GeometryObject[] { solid });
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException ex)
            {
                Debug.Print("Failed to drawSolid; exception {0} {1}", ex.GetType().FullName, ex.Message);
            }
            return ds;
        }

        #endregion ?\???b?h??`??

        // ?\???b?h?????????

        #region ?\???b?h?????????

        private static bool intersect(Solid solidA, Solid solidB)
        {
            Solid result = BooleanOperationsUtils.ExecuteBooleanOperation(solidA, solidB, BooleanOperationsType.Union);
            double dArea = Math.Abs(solidA.SurfaceArea + solidB.SurfaceArea - result.SurfaceArea);
            int nEdges = solidA.Edges.Size + solidB.Edges.Size;
            if (!equal(dArea, 0.0) || nEdges != result.Edges.Size)
            {
                return true;
            }
            return false;
        }

        private static bool intersect(Document doc, Solid solid, XYZ pt)
        {
            if (solid == null)
            {
                return false;
            }

            Solid solidA = solid;

            Solid sphere = CreateSphereDirectShape(doc, pt, 0.5, false);
            BoundingBoxXYZ bboxB = sphere.GetBoundingBox();
            Solid solidB = createSolidFromBoundingBox(bboxB);

            return intersect(solidA, solidB);
        }

        private static bool intersect(Document doc, Element elem, XYZ pt)
        {
            if (elem == null)
            {
                return false;
            }

            Solid solidA = getSolid(elem, true);

            Solid sphere = CreateSphereDirectShape(doc, pt, 0.5, false);
            BoundingBoxXYZ bboxB = sphere.GetBoundingBox();
            Solid solidB = createSolidFromBoundingBox(bboxB);

            return intersect(solidA, solidB);
        }

        #endregion ?\???b?h?????????

        // ?\???b?h????

        #region ?\???b?h????

        private static Solid getSolid(Element element, bool isBbox = false)
        {
            if (element == null)
            {
                return null;
            }

            Options geoOptions = new Options();
            geoOptions.ComputeReferences = true;
            geoOptions.IncludeNonVisibleObjects = true;
            geoOptions.DetailLevel = ViewDetailLevel.Fine;

            GeometryElement geoElement = element.get_Geometry(geoOptions);

            foreach (GeometryObject geoObject in geoElement)
            {
                GeometryInstance instance = geoObject as GeometryInstance;
                if (instance != null)
                {
                    foreach (GeometryObject instObj in instance.SymbolGeometry)
                    {
                        Solid solid = instObj as Solid;
                        if (solid == null || solid.Faces.Size == 0 || solid.Edges.Size == 0)
                        {
                            continue;
                        }

                        if (isBbox)
                        {
                            solid = createSolidFromBoundingBox(solid.GetBoundingBox());
                        }
                        Transform instTransform = instance.Transform;
                        Solid transformed = SolidUtils.CreateTransformed(
                            solid, instTransform);
                        return transformed;
                    }
                }
                else
                {
                    Solid solid = geoObject as Solid;
                    if (solid == null || solid.Faces.Size == 0 || solid.Edges.Size == 0)
                    {
                        continue;
                    }

                    if (isBbox)
                    {
                        //break;
                    }
                    return SolidUtils.Clone(solid);
                }
            }
            return null;
        }

        #endregion ?\???b?h????

        // ?w??R?l?N?^?[???????????_?N?g??R?l?N?^?[?????????

        #region ?w??R?l?N?^?[???????????_?N?g??R?l?N?^?[?????????

        public static Connector getConnectedConnector(Connector connector, bool withCurve = false)
        {
            if (connector != null)
            {
                if (!connector.IsConnected)
                {
                    return null;
                }
                foreach (Connector c in connector.AllRefs)
                {
                    if (c == null
                        || (c.ConnectorType != ConnectorType.End
                         && !(withCurve && c.ConnectorType == ConnectorType.Curve)
                         /*&& c.ConnectorType != ConnectorType.Physical*/))
                    {
                        continue;
                    }
                    if (isDuctSystem(c.Owner))
                    {
                        continue;
                    }
                    if (connector.Owner.Id == c.Owner.Id)
                    {
                        continue;
                    }

                    if (c.IsConnected)
                    {
                        return c;
                    }
                }
            }
            return null;
        }

        #endregion ?w??R?l?N?^?[???????????_?N?g??R?l?N?^?[?????????

        // ?w??v?f????????_?N?g??R?l?N?^?[?????????

        #region ?w??v?f????????_?N?g??R?l?N?^?[?????????

        public static Connector getConnectedConnectorNext(Element element, Connector connector, bool withCurve = false)
        {
            var cs = getConnectors(element);
            if (cs != null)
            {
                foreach (Connector c in cs)
                {
                    if (c == null
                     || (c.ConnectorType != ConnectorType.End
                      && !(withCurve && c.ConnectorType == ConnectorType.Curve)
                      /*&& c.ConnectorType != ConnectorType.Physical*/))
                    {
                        continue;
                    }

                    if (c.Id != connector.Id)
                    {
                        Connector con = getConnectedConnector(c);
                        if (con != null)
                        {
                            return con;
                        }
                    }
                }
            }
            return null;
        }

        #endregion ?w??v?f????????_?N?g??R?l?N?^?[?????????

        // ??????R?l?N?^?[????

        #region ??????R?l?N?^?[????

        private static ConnectorSet getTargetConnectors(Document doc, Element element)
        {
            ConnectorSet connectors = new ConnectorSet();

            var cm = getConnectorManager(element);
            if (cm == null)
            {
                return null;
            }

            if (isMechanicalEquipment(element)
             || isDuctTerminal(element)
             || isDuctAccessory(element))
            {
                ConnectorSetIterator csi = cm.Connectors.ForwardIterator();
                while (csi.MoveNext())
                {
                    Connector conn = csi.Current as Connector;
                    if (conn.Domain == Domain.DomainHvac
                        && (conn.DuctSystemType == DuctSystemType.ExhaustAir
                         || conn.DuctSystemType == DuctSystemType.ReturnAir
                         || conn.DuctSystemType == DuctSystemType.SupplyAir))
                    {
                        if (conn.IsConnected)
                        {
                            connectors.Insert(conn);
                        }
                    }
                }
            }

            return connectors;
        }

        #endregion ??????R?l?N?^?[????

        // MechanicalSystem ????

        #region MechanicalSystem ????

        private static MechanicalSystem getMechanicalSystem(Document doc, Element element, Element elementDuct = null)
        {
            ConnectorSet connectors = getTargetConnectors(doc, element);
            if (connectors.Size == 0)
            {
                return null;
            }

            FilteredElementCollector msCollector = new FilteredElementCollector(doc);
            var mechanicalSystemCollection = msCollector.OfClass(typeof(MechanicalSystem));

            var con0 = getConnector(element, 0);
            var con1 = getConnector(element, 1);
            foreach (MechanicalSystem ms in mechanicalSystemCollection)
            {
                ElementSet elems = ms.DuctNetwork;
                foreach (Element e in elems)
                {
                    if (elementDuct == null)
                    {
                        if (e.Id == element.Id)
                        {
                            return ms;
                        }
                    }
                    else
                    {
                        if (e.Id == elementDuct.Id)
                        {
                            foreach (Element ee in elems)
                            {
                                if (ee.Id != elementDuct.Id)
                                {
                                    var conDuct0 = getConnector(ee, 0);
                                    var conDuct1 = getConnector(ee, 1);
                                    if ((con0 != null && conDuct0 != null && equal(con0.Origin, conDuct0.Origin, EEPS_CONNECTOR))
                                     || (con0 != null && conDuct1 != null && equal(con0.Origin, conDuct1.Origin, EEPS_CONNECTOR))
                                     || (con1 != null && conDuct0 != null && equal(con1.Origin, conDuct0.Origin, EEPS_CONNECTOR))
                                     || (con1 != null && conDuct1 != null && equal(con1.Origin, conDuct1.Origin, EEPS_CONNECTOR)))
                                    {
                                        return ms;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        #endregion MechanicalSystem ????

        // ?_?N?g???a??p?????[?^?[

        #region ?_?N?g???a??p?????[?^?[

        // ??????a????
        private static double getDuctHydraulicDiameter(Element elem)
        {
            var p = getParameter(elem, "���͒��a");
            if (p != null)
            {
                return p.AsDouble();
            }
            p = getParameter(elem, "���a");
            if (p != null)
            {
                return p.AsDouble();
            }
            p = getParameter(elem, "�_�N�g���a");
            if (p != null)
            {
                return p.AsDouble();
            }
            p = getParameter(elem, "�_�N�g���a");
            if (p != null)
            {
                return p.AsDouble() * 2.0;
            }
            return 0.0;
        }

        private static bool setDuctDiameter(Element elem, double diameter)
        {
            if (!setParameter(elem, "���a", diameter))
            {
                if (!setParameter(elem, "�_�N�g���a", diameter))
                {
                    if (!setParameter(elem, "�_�N�g���a", diameter * 0.5))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion ?_?N?g???a??p?????[?^?[

        // ???S???a??p?????[?^?[

        #region ???S???a??p?????[?^?[

        private static double getRadius(Element elem)
        {
            var p = getParameter(elem, "���S���a");
            if (p != null)
            {
                return p.AsDouble();
            }
            p = getParameter(elem, "Center Radius");
            if (p != null)
            {
                return p.AsDouble();
            }
            return 0.0;
        }

        #endregion ???S???a??p?????[?^?[

        #region ?~????

        private static Arc createArc(Element elem, XYZ end0, XYZ end1)
        {
            FamilyInstance fi = elem as FamilyInstance;
            var lp = fi.Location as LocationPoint;
            double radius = getRadius(elem);
            Arc arc = CreateArc(end0, end1, lp.Point, radius);
            return arc;
        }

        #endregion ?~????

        // ?I?[??X??

        #region ?I?[??X??

        private static XYZ getEndTangent(ductElementInfo dei, XYZ pointEnd)
        {
            XYZ endTangent = null;

            var elem = dei.elem;
            if (isDuct(elem) || isTransition(elem))
            {
                endTangent = pointEnd - dei.pointStart;
            }
            else if (isElbow(elem))
            {
                if (!equal(getRadius(dei.elem), 0.0))
                {
                    FamilyInstance fi = elem as FamilyInstance;
                    var lp = fi.Location as LocationPoint;
                    Arc arc = createArc(elem, dei.pointStart, dei.pointEnd);
                    XYZ axis = (dei.pointStart - lp.Point).CrossProduct(dei.pointEnd - lp.Point);
                    XYZ p1 = pointEnd - arc.Center;
                    endTangent = p1.CrossProduct(axis);
                }
                else
                {
                    endTangent = pointEnd - dei.pointStart;
                }
            }
            return endTangent;
        }

        #endregion ?I?[??X??

        // ????_?????i???j

        #region ????_?????i???j

        private static void addLineMidPoints(List<XYZ> points, Element elem, XYZ end0, XYZ end1)
        {
            double elementLength = end0.DistanceTo(end1);
            int imidCount = (int)(elementLength / divideLength);
            if (imidCount > 0)
            {
                XYZ elementSeparate = (end1 - end0) / ((double)(imidCount + 1));
                for (int i = 0; i < imidCount; ++i)
                {
                    XYZ p = end0 + (double)(i + 1) * elementSeparate;
                    points.Add(p);
                }
            }
        }

        #endregion ????_?????i???j

        // ????_?????i?~??j

        #region ????_?????i?~??j

        private static void addArcMidPoints(List<XYZ> points, Element elem, XYZ end0, XYZ end1)
        {
            FamilyInstance fi = elem as FamilyInstance;
            var lp = fi.Location as LocationPoint;
            double radius = getRadius(elem);
            Arc arc = CreateArc(end0, end1, lp.Point, radius);
            double elementLength = arc.Length;
            int imidCount = (int)(elementLength / divideLength);
            if (imidCount > 0)
            {
                for (int i = 0; i < imidCount; ++i)
                {
                    XYZ p = arc.Evaluate((double)(i + 1) / imidCount, true);
                    points.Add(p);
                }
            }
        }

        #endregion ????_?????i?~??j

        // ?R?l?N?^?[?}?l?[?W???[

        #region ?R?l?N?^?[?}?l?[?W???[

        private static ConnectorManager getConnectorManager(Element elem)
        {
            if (elem != null)
            {
                if (elem is MEPCurve)
                {
                    MEPCurve mc = elem as MEPCurve;
                    if (mc == null || mc.ConnectorManager == null)
                    {
                        return null;
                    }
                    return ((MEPCurve)elem).ConnectorManager;
                }
                else if (elem is FamilyInstance)
                {
                    FamilyInstance fi = elem as FamilyInstance;
                    if (fi == null || fi.MEPModel == null || fi.MEPModel.ConnectorManager == null)
                    {
                        return null;
                    }
                    return ((FamilyInstance)elem).MEPModel.ConnectorManager;
                }
            }
            return null;
        }

        #endregion ?R?l?N?^?[?}?l?[?W???[

        // ?R?l?N?^?[?Z?b?g

        #region ?R?l?N?^?[?Z?b?g

        public static ConnectorSet getConnectors(Element elem)
        {
            ConnectorManager cm = getConnectorManager(elem);
            if (cm != null)
            {
                return cm.Connectors;
            }
            return null;
        }

        public static List<XYZ> getConnectorsOrigin(Element elem)
        {
            List<XYZ> pts = new List<XYZ>();
            ConnectorManager cm = getConnectorManager(elem);
            if (cm != null)
            {
                foreach (Connector c in cm.Connectors)
                {
                    pts.Add(c.Origin);
                }
            }
            return pts;
        }

        #endregion ?R?l?N?^?[?Z?b?g

        // ?R?l?N?^?[

        #region ?R?l?N?^?[

        // ?R?l?N?^?[?i?C???f?b?N?X?w??j
        private static Connector getConnector(Element elem, int index)
        {
            var cm = getConnectorManager(elem);
            if (cm != null)
            {
                return cm.Lookup(index);
            }
            return null;
        }

        // ?R?l?N?^?[?i??u?w??j
        private static Connector getConnector(Element elem, XYZ p)
        {
            Connector targetConnector = null;

            var cm = getConnectorManager(elem);
            if (cm == null)
            {
                return targetConnector;
            }

            foreach (Connector c in cm.Connectors)
            {
                if (equal(c.Origin, p, EEPS_CONNECTOR))
                {
                    targetConnector = c;
                    break;
                }
            }

            return targetConnector;
        }

        // ?R?l?N?^?[?i????????????????????R?l?N?^?[?j
        private static Connector getConnector(Element elem)
        {
            var cm = getConnectorManager(elem);
            if (cm == null)
            {
                return null;
            }

            foreach (Connector c in cm.Connectors)
            {
                if (c.IsConnected)
                {
                    return c;
                }
            }

            return null;
        }

        #endregion ?R?l?N?^?[

        // ?p?????[?^?[

        #region ?p?????[?^?[

        private static Parameter getParameter(Element elem, string paramName)
        {
            if (elem != null)
            {
                return elem.LookupParameter(paramName);
            }
            return null;
        }

        private static bool setParameter(Element elem, string paramName, double paramValue)
        {
            if (elem != null)
            {
                Parameter par = elem.LookupParameter(paramName);
                if (par != null)
                {
                    par.Set(paramValue);
                    return true;
                }
            }
            return false;
        }

        #endregion ?p?????[?^?[

        // ?_?N?g?V?X?e????

        #region ?_?N?g?V?X?e????

        public static bool isDuctSystem(Element elem)
        {
            var catId = new ElementId(BuiltInCategory.OST_DuctSystem);
            return elem != null && elem.Category.Id == catId;
        }

        #endregion ?_?N?g?V?X?e????

        // ?@?B?????

        #region ?@?B?????

        public static bool isMechanicalEquipment(Element elem)
        {
            var catId = new ElementId(BuiltInCategory.OST_MechanicalEquipment);
            return elem != null && elem.Category.Id == catId;
        }

        #endregion ?@?B?????

        // ?????o??????

        #region ?????o??????

        public static bool isDuctTerminal(Element elem)
        {
            var catId = new ElementId(BuiltInCategory.OST_DuctTerminal);
            return elem != null && elem.Category.Id == catId;
        }

        #endregion ?????o??????

        // ?_?N?g?t???i??

        #region ?_?N?g?t???i??

        public static bool isDuctAccessory(Element elem)
        {
            var catId = new ElementId(BuiltInCategory.OST_DuctAccessory);
            return elem != null && elem.Category.Id == catId;
        }

        #endregion ?_?N?g?t???i??

        // ?t???L?V?u???_?N?g??

        #region ?t???L?V?u???_?N?g??

        private static bool isFlexDuct(Element elem)
        {
            return elem != null && elem.GetType() == typeof(FlexDuct);
        }

        #endregion ?t???L?V?u???_?N?g??

        // ?_?N?g??

        #region ?_?N?g??

        private static bool isDuct(Element elem)
        {
            return elem != null && elem.GetType() == typeof(Duct);
        }

        #endregion ?_?N?g??

        // ?G???{?[??

        #region ?G???{?[??

        private static bool isElbow(Element elem)
        {
            if (elem != null && elem.GetType() == typeof(FamilyInstance))
            {
                MEPModel mm = ((FamilyInstance)elem).MEPModel;
                if (mm != null)
                {
                    MechanicalFitting mf = mm as MechanicalFitting;
                    if (mf != null)
                    {
                        return mf.PartType == PartType.Elbow;
                    }
                }
            }
            return false;
        }

        #endregion ?G???{?[??

        // ?g?????W?V??????

        #region ?g?????W?V??????

        private static bool isTransition(Element elem)
        {
            if (elem != null && elem.GetType() == typeof(FamilyInstance))
            {
                MEPModel mm = ((FamilyInstance)elem).MEPModel;
                if (mm != null)
                {
                    MechanicalFitting mf = mm as MechanicalFitting;
                    if (mf != null)
                    {
                        return mf.PartType == PartType.Transition;
                    }
                }
            }
            return false;
        }

        #endregion ?g?????W?V??????

        // TapAdjustable ??

        #region TapAdjustable ??

        private static bool isTapAdjustable(Element elem)
        {
            if (elem != null && elem.GetType() == typeof(FamilyInstance))
            {
                MEPModel mm = ((FamilyInstance)elem).MEPModel;
                if (mm != null)
                {
                    MechanicalFitting mf = mm as MechanicalFitting;
                    if (mf != null)
                    {
                        return mf.PartType == PartType.TapAdjustable;
                    }
                }
            }
            return false;
        }

        #endregion TapAdjustable ??

        // DuctFitting ??

        #region DuctFitting ??

        public static bool isDuctFitting(Element elem)
        {
            var catId = new ElementId(BuiltInCategory.OST_DuctFitting);
            return elem != null && elem.Category.Id == catId;
        }

        #endregion DuctFitting ??

        // Union ??

        #region Union ??

        private static bool isUnion(Element elem)
        {
            if (elem != null && elem.GetType() == typeof(FamilyInstance))
            {
                MechanicalFitting mf = ((FamilyInstance)elem).MEPModel as MechanicalFitting;
                if (mf != null)
                {
                    return mf.PartType == PartType.Union;
                }
            }
            return false;
        }

        #endregion Union ??

        // Tee ??

        #region Tee ??

        private static bool isTee(Element elem)
        {
            if (elem != null && elem.GetType() == typeof(FamilyInstance))
            {
                MechanicalFitting mf = ((FamilyInstance)elem).MEPModel as MechanicalFitting;
                if (mf != null)
                {
                    return mf.PartType == PartType.Tee;
                }
            }
            return false;
        }

        #endregion Tee ??

        // Cross ??

        #region Cross ??

        private static bool isCross(Element elem)
        {
            if (elem != null && elem.GetType() == typeof(FamilyInstance))
            {
                MEPModel mm = ((FamilyInstance)elem).MEPModel;
                if (mm != null)
                {
                    MechanicalFitting mf = mm as MechanicalFitting;
                    if (mf != null)
                    {
                        return mf.PartType == PartType.Cross;
                    }
                }
            }
            return false;
        }

        #endregion Cross ??

        // ?o?E???f?B???O?{?b?N?X????\???b?h????

        #region ?o?E???f?B???O?{?b?N?X????\???b?h????

        private static Solid createSolidFromBoundingBox(
            /*Solid inputSolid*/BoundingBoxXYZ bbox)
        {
            //BoundingBoxXYZ bbox = inputSolid.GetBoundingBox();

            // Corners in BBox coords

            XYZ pt0 = new XYZ(bbox.Min.X, bbox.Min.Y, bbox.Min.Z);
            XYZ pt1 = new XYZ(bbox.Max.X, bbox.Min.Y, bbox.Min.Z);
            XYZ pt2 = new XYZ(bbox.Max.X, bbox.Max.Y, bbox.Min.Z);
            XYZ pt3 = new XYZ(bbox.Min.X, bbox.Max.Y, bbox.Min.Z);

            // Edges in BBox coords

            Line edge0 = Line.CreateBound(pt0, pt1);
            Line edge1 = Line.CreateBound(pt1, pt2);
            Line edge2 = Line.CreateBound(pt2, pt3);
            Line edge3 = Line.CreateBound(pt3, pt0);

            // Create loop, still in BBox coords

            List<Curve> edges = new List<Curve>();
            edges.Add(edge0);
            edges.Add(edge1);
            edges.Add(edge2);
            edges.Add(edge3);

            double height = bbox.Max.Z - bbox.Min.Z;

            CurveLoop baseLoop = CurveLoop.Create(edges);

            List<CurveLoop> loopList = new List<CurveLoop>();
            loopList.Add(baseLoop);

            Solid preTransformBox = GeometryCreationUtilities
                .CreateExtrusionGeometry(loopList, XYZ.BasisZ, height);

            Solid transformBox = SolidUtils.CreateTransformed(
                preTransformBox, bbox.Transform);

            return transformBox;
        }

        #endregion ?o?E???f?B???O?{?b?N?X????\???b?h????

        // ?~??

        #region ?~??

        public static Arc CreateArc(
            XYZ ps,
            XYZ pe,
            XYZ pm,
            double radius,
            bool largeSagitta = false)
        {
            XYZ midPointChord = 0.5 * (ps + pe);
            XYZ v = pe - ps;
            double d = 0.5 * v.GetLength();

            // Small and large circle sagitta:
            // http://www.mathopenref.com/sagitta.html

            double s = largeSagitta
            ? radius + Math.Sqrt(radius * radius - d * d)   // sagitta large
            : radius - Math.Sqrt(radius * radius - d * d);  // sagitta small
            XYZ norm = (ps - pm).CrossProduct(pe - pm);
            XYZ midPointArc = midPointChord + Transform.CreateRotation(norm, 0.5 * Math.PI).OfVector(v.Normalize().Multiply(s));

            return Arc.Create(ps, pe, midPointArc);
        }

        #endregion ?~??

        // DirectShape

        #region DirectShape

        // ??
        public static void CreateLineDirectShape(Document doc, XYZ ps, XYZ pe, Color color = null)
        {
            WireframeBuilder builder = new WireframeBuilder();
            builder.AddCurve(Line.CreateBound(ps, pe));
            DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
            //ds.ApplicationId = "Application id";
            //ds.ApplicationDataId = "Geometry object id";
            ds.SetShape(builder);

            if (color == null)
            {
                color = new Color(255, 0, 0);
            }

            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            var fpe = (from a in new FilteredElementCollector(doc)
                .OfClass(typeof(FillPatternElement))
                .Cast<FillPatternElement>()
                           //where a.Name.Equals("?h????")
                       select a).FirstOrDefault();
            //ogs.SetProjectionFillPatternId(fpe.Id);
            ogs.SetSurfaceForegroundPatternId(fpe.Id);
            //ogs.SetProjectionFillPatternVisible(true);
            ogs.SetSurfaceForegroundPatternVisible(true);
            ogs.SetProjectionLineWeight(6);
            ogs.SetProjectionLineColor(color);
            //ogs.SetProjectionFillColor(color);
            ogs.SetSurfaceForegroundPatternColor(color);
            //ogs.SetCutFillColor(color);
            ogs.SetCutForegroundPatternColor(color);
            ogs.SetCutLineColor(color);
            ogs.SetCutLineWeight(6);
            ogs.SetSurfaceTransparency(50);
            doc.ActiveView.SetElementOverrides(ds.Id, ogs);
        }

        // ?~??
        public static void CreateArcDirectShape(Document doc, XYZ ps, XYZ pe, XYZ pm, double radius, bool largeSagitta = false, Color color = null)
        {
            WireframeBuilder builder = new WireframeBuilder();
            builder.AddCurve(CreateArc(ps, pe, pm, radius, largeSagitta));
            DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
            //ds.ApplicationId = "Application id";
            //ds.ApplicationDataId = "Geometry object id";
            ds.SetShape(builder);

            if (color == null)
            {
                color = new Color(255, 0, 0);
            }

            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            var fpe = (from a in new FilteredElementCollector(doc)
                .OfClass(typeof(FillPatternElement))
                .Cast<FillPatternElement>()
                           //where a.Name.Equals("?h????")
                       select a).FirstOrDefault();
            //ogs.SetProjectionFillPatternId(fpe.Id);
            ogs.SetSurfaceForegroundPatternId(fpe.Id);
            //ogs.SetProjectionFillPatternVisible(true);
            ogs.SetSurfaceForegroundPatternVisible(true);
            ogs.SetProjectionLineWeight(6);
            ogs.SetProjectionLineColor(color);
            //ogs.SetProjectionFillColor(color);
            ogs.SetSurfaceForegroundPatternColor(color);
            //ogs.SetCutFillColor(color);
            ogs.SetCutForegroundPatternColor(color);
            ogs.SetCutLineColor(color);
            ogs.SetCutLineWeight(6);
            ogs.SetSurfaceTransparency(50);
            doc.ActiveView.SetElementOverrides(ds.Id, ogs);
        }

        // ??
        public static Solid CreateSphereDirectShape(Document doc, XYZ center, double radius, bool create = true, Color color = null)
        {
            Solid sphere = null;
            double r = UnitUtils.ConvertToInternalUnits(radius, UnitTypeId.Millimeters);
            XYZ profilePlus = center + new XYZ(0, r, 0);
            XYZ profileMinus = center - new XYZ(0, r, 0);

            List<Curve> profile = new List<Curve>();
            profile.Add(Line.CreateBound(profilePlus, profileMinus));
            profile.Add(Arc.Create(profileMinus, profilePlus, center + new XYZ(r, 0, 0)));

            CurveLoop curveLoop = CurveLoop.Create(profile);
            SolidOptions options = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);

            Frame frame = new Frame(center, XYZ.BasisX, -XYZ.BasisZ, XYZ.BasisY);
            if (Frame.CanDefineRevitGeometry(frame) == true)
            {
                sphere = GeometryCreationUtilities.CreateRevolvedGeometry(frame, new CurveLoop[] { curveLoop }, 0, 2 * Math.PI, options);
                if (create)
                {
                    DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));

                    //ds.ApplicationId = "Application id";
                    //ds.ApplicationDataId = "Geometry object id";
                    ds.SetShape(new GeometryObject[] { sphere });

                    if (color == null)
                    {
                        color = new Color(255, 0, 0); // RGB (0, 255, 255)
                    }

                    OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                    var fpe = (from a in new FilteredElementCollector(doc)
                        .OfClass(typeof(FillPatternElement))
                        .Cast<FillPatternElement>()
                                   //where a.Name.Equals("?h????")
                               select a).FirstOrDefault();
                    //ogs.SetProjectionFillPatternId(fpe.Id);
                    ogs.SetSurfaceForegroundPatternId(fpe.Id);
                    //ogs.SetProjectionFillPatternVisible(true);
                    ogs.SetSurfaceForegroundPatternVisible(true);
                    ogs.SetProjectionLineWeight(6);
                    ogs.SetProjectionLineColor(color);
                    //ogs.SetProjectionFillColor(color);
                    ogs.SetSurfaceForegroundPatternColor(color);
                    //ogs.SetCutFillColor(color);
                    ogs.SetCutForegroundPatternColor(color);
                    ogs.SetCutLineColor(color);
                    ogs.SetCutLineWeight(6);
                    ogs.SetSurfaceTransparency(50);
                    doc.ActiveView.SetElementOverrides(ds.Id, ogs);
                }
            }
            return sphere;
        }

        #endregion DirectShape

        // ??r

        #region ??r

        // ??????
        public static bool equal(double d1, double d2, double eps = EEPS/*1.0E-5*/)
        {
            return Math.Abs(d1 - d2) < eps;
        }

        public static bool equal(XYZ p1, XYZ p2, double eps = EEPS/*1.0E-5*/)
        {
            return Math.Abs(p1.X - p2.X) < eps
                && Math.Abs(p1.Y - p2.Y) < eps
                && Math.Abs(p1.Z - p2.Z) < eps;
        }

        // ??????
        public static bool greaterThan(double d1, double d2, double eps = EEPS/*1.0E-5*/)
        {
            return !equal(d1, d2, eps) && d1 > d2;
        }

        // ???
        public static bool greaterThanEqual(double d1, double d2, double eps = EEPS/*1.0E-5*/)
        {
            return equal(d1, d2, eps) || d1 > d2;
        }

        // ???????
        public static bool lessThan(double d1, double d2, double eps = EEPS/*1.0E-5*/)
        {
            return !equal(d1, d2, eps) && d1 < d2;
        }

        // ???
        public static bool lessThanEqual(double d1, double d2, double eps = EEPS/*1.0E-5*/)
        {
            return equal(d1, d2, eps) || d1 < d2;
        }

        #endregion ??r
    }

    /// <summary>
    /// FlexDuctConvertException ??O?N???X
    /// </summary>
    [Serializable()]
    public class FlexDuctConvertException : Exception
    {
        public FlexDuctConvertException()
            : base()
        {
        }

        public FlexDuctConvertException(string message)
            : base(message)
        {
        }

        public FlexDuctConvertException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected FlexDuctConvertException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}