using ADSK.JExtRAC.EnhancedSectionBox.Common;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Form = System.Windows.Forms.Form;
using TextBox = System.Windows.Forms.TextBox;

namespace ADSK.JExtRAC.EnhancedSectionBox.Screen
{
    public partial class FormBoxView : Form
    {
        public static ExternalCommandData CommandData;
        public static UIApplication UiApp;
        public static Autodesk.Revit.ApplicationServices.Application App;
        public static Document Doc;
        public static UIDocument UiDoc;
        public static Element selectionElement;
        public static List<Element> selectionElementList;
        public static List<Level> levels = new List<Level>();
        private HashSet<string> levelSet = new HashSet<string>();
        public PickedBox pickedBox;

        public static double topValue;
        public static double lowerValue;

        public bool _isSelectObject;
        public bool _isObject;
        public bool _isLink;
        public bool _isRegion;
        private bool returnFlag;

        public static View3D newView3d;
        private List<Element> elements = new List<Element>();
        public static List<Element> mainElementList = new List<Element>();
        public static List<Element> linkElementList = new List<Element>();
        public static IList<Reference> linkRefList = new List<Reference>();

        private View baseView;
        private readonly Components.Attribute _res = new Components.Attribute();

        public FormBoxView(ExternalCommandData commandData)
        {
            InitializeComponent();
            SetText();

            UiApp = commandData.Application;
            UiDoc = UiApp.ActiveUIDocument;
            Doc = UiDoc.Document;
            App = UiApp.Application;

            mainElementList = new List<Element>();
            linkElementList = new List<Element>();
            linkRefList = new List<Reference>();
            selectionElement = null;
            selectionElementList = new List<Element>();
            levels = new List<Level>();
            mainSelectionButton.Click += MainSelectionButton_Click;
            linkSelectionButton.Click += LinkSelectionButton_Click;
            rangeSpecificationButton.Click += RangeSpecificationButton_Click;
            executionButton.Click += ExecutionButton_Click;
            selectObjectRadio.CheckedChanged += Radio_Changed;
            rangeSpecificationRadio.CheckedChanged += Radio_Changed;
            floorBoxRadio.CheckedChanged += Radio_Changed;
            levelListBox.SelectedIndexChanged += ListBoxItem_Select;
            executionButton.Enabled = false;
            returnFlag = false;
            topMargin.MaxLength = 7;
            lowerMargin.MaxLength = 7;

            levels = GetElements<Level>(Doc);
            levels.Sort((a, b) => Math.Sign((a.Elevation - b.Elevation)));
            foreach (Level level in levels) {
                levelSet.Add(level.Name);
            }
            foreach (string name in levelSet) {
                levelListBox.Items.Add(name);
            }
            foreach (string name in levelSet) {
                topEdgeCombo.Items.Add(name);
                topEdgeCombo.SelectedIndex = topEdgeCombo.Items.Count - 1;
                lowerEndCombo.Items.Add(name);
                lowerEndCombo.SelectedIndex = 0;
            }

            List<View> views = GetElements<View>(Doc);
            List<View> viewList = new List<View>();

            foreach (View view in views) {
                if (view.ViewType == ViewType.ThreeD) {
                    viewList.Add(view);
                }
            }

            nameCombo.Items.Clear();
            List<string> viewNameList = new List<string>();
            foreach (View view in viewList) {
                if (view.IsTemplate == false && view.ViewType == ViewType.ThreeD && view.Id != view.Document.ActiveView.Id) {
                    viewNameList.Add(view.Name);
                }
                if (view.Name == "{3D}") {
                    baseView = view;
                }
            }
            viewNameList.Sort();
            foreach (string name in viewNameList) {
                nameCombo.Items.Add(name);
            }

            if (UiDoc.ActiveView.ViewType == ViewType.ThreeD
                || UiDoc.ActiveView.ViewType == ViewType.Elevation
                || UiDoc.ActiveView.ViewType == ViewType.Section) {
                rangeSpecificationRadio.Enabled = false;
            }
        }

        private void SetText()
        {
            this.Text = _res.ResourceText("IDS_FORM_BOXVIEW_TITLE");
            groupBox1.Text = _res.ResourceText("IDS_GRP_XYPLANE");
            regionLabel.Text = _res.ResourceText("IDS_LBL_NOT_SPECIFIED");
            linkCountLabel.Text = _res.ResourceText("IDS_LBL_SELCOUNT_0");
            mainCountLabel.Text = _res.ResourceText("IDS_LBL_SELCOUNT_0");
            linkSelectionButton.Text = _res.ResourceText("IDS_BTN_SELECT_LINK");
            rangeSpecificationButton.Text = _res.ResourceText("IDS_BTN_RANGE_SPEC");
            mainSelectionButton.Text = _res.ResourceText("IDS_BTN_SELECT_MAIN");
            floorBoxRadio.Text = _res.ResourceText("IDS_RADIO_LEVEL_SPEC");
            rangeSpecificationRadio.Text = _res.ResourceText("IDS_BTN_RANGE_SPEC");
            objectCheck.Text = _res.ResourceText("IDS_CHK_ALIGN_ANGLE");
            selectObjectRadio.Text = _res.ResourceText("IDS_RADIO_OBJECT_SPEC");
            groupBox2.Text = _res.ResourceText("IDS_GRP_LEVEL_OFFSET");
            label4.Text = _res.ResourceText("IDS_LBL_OFFSET");
            label3.Text = _res.ResourceText("IDS_LBL_OFFSET");
            label2.Text = _res.ResourceText("IDS_LBL_LOWER_LEVEL");
            label1.Text = _res.ResourceText("IDS_LBL_UPPER_LEVEL");
            hiddenFloorCheck.Text = _res.ResourceText("IDS_CHK_FLOOR");
            label9.Text = _res.ResourceText("IDS_LBL_VIEW_NAME");
            groupBox3.Text = _res.ResourceText("IDS_GRP_HIDE_UPPER_OBJECTS");
            hiddenBeamCheck.Text = _res.ResourceText("IDS_CHK_BEAM");
            executionButton.Text = _res.ResourceText("IDS_BTN_OK");
            button4.Text = _res.ResourceText("IDS_BTN_CANCEL");
        }

        public static List<T> GetElements<T>(Document doc)
        {
            Type type = typeof(T);
            return new FilteredElementCollector(doc).OfClass(typeof(T)).Cast<T>().ToList();
        }

        private void MainSelectionButton_Click(object sender, System.EventArgs e)
        {
            pickedBox = null;
            _isSelectObject = true;
            _isObject = true;
            _isLink = false;
            _isRegion = false;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void LinkSelectionButton_Click(object sender, System.EventArgs e)
        {
            pickedBox = null;
            _isSelectObject = true;
            _isObject = false;
            _isLink = true;
            _isRegion = false;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void RangeSpecificationButton_Click(object sender, System.EventArgs e)
        {
            _isSelectObject = true;
            _isObject = false;
            _isLink = false;
            _isRegion = true;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void ExecutionButton_Click(object sender, System.EventArgs e)
        {
            topValue = 0;
            lowerValue = 0;
            if (topMargin.Text != "" && topMargin.Text != null) {
                topValue = double.Parse(topMargin.Text);
                topValue = UnitUtils.ConvertToInternalUnits(topValue, UnitTypeId.Millimeters);
            }
            if (lowerMargin.Text != "" && lowerMargin.Text != null) {
                lowerValue = double.Parse(lowerMargin.Text);
                lowerValue = UnitUtils.ConvertToInternalUnits(lowerValue, UnitTypeId.Millimeters);
            }
            CreateView();
            if (returnFlag) {
                return;
            }
            UiDoc.ActiveView = FormBoxView.newView3d;

            List<ElementId> idList = new List<ElementId>();
            if (hiddenFloorCheck.Checked) {
                foreach (Element element in elements) {
                    if (element.Category != null
                        && element.Category.BuiltInCategory == BuiltInCategory.OST_Floors) {
                        idList.Add(element.Id);
                    }
                }
            }
            if (hiddenBeamCheck.Checked) {
                foreach (Element element in elements) {
                    if (element.Category != null
                        && element.Category.BuiltInCategory == BuiltInCategory.OST_StructuralFraming) {
                        idList.Add(element.Id);
                    }
                }
            }
            if (idList.Count > 0) {
                using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_HIDE"))) {
                    tran.Start();
                    UiDoc.ActiveView.HideElements(idList);
                    tran.Commit();
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Close();
        }

        private void ListBoxItem_Select(object sender, System.EventArgs e)
        {
            selectionElement = null;
            List<Level> selectLevelList = new List<Level>();
            System.Windows.Forms.ListBox.SelectedObjectCollection items = levelListBox.SelectedItems;
            List<string> nameList = new List<string>();
            foreach (object item in items) {
                nameList.Add(item.ToString());
            }

            foreach (Level level in levels) {
                foreach (String name in nameList) {
                    if (level.Name == name) {
                        selectLevelList.Add(level);
                        BoundingBoxXYZ box = level.get_BoundingBox(null);
                    }
                }
            }
        }

        private void Radio_Changed(object sender, System.EventArgs e)
        {
            if (selectObjectRadio.Checked) {
                mainSelectionButton.Enabled = true;
                linkSelectionButton.Enabled = true;
                objectCheck.Enabled = true;
            }
            else {
                mainSelectionButton.Enabled = false;
                linkSelectionButton.Enabled = false;
                objectCheck.Enabled = false;
            }
            if (rangeSpecificationRadio.Checked) {
                rangeSpecificationButton.Enabled = true;
            }
            else {
                rangeSpecificationButton.Enabled = false;
            }
            if (floorBoxRadio.Checked) {
                levelListBox.Enabled = true;
            }
            else {
                levelListBox.Enabled = false;
            }
        }

        private void ViewName_Changed(object sender, System.EventArgs e)
        {
            if (nameCombo.Text == "" || nameCombo.Text == null) {
                executionButton.Enabled = false;
            }
            else if (IsWhiteSpaceOnly(nameCombo.Text)) {
                executionButton.Enabled = false;
            }
            else {
                executionButton.Enabled = true;
            }
        }

        public void CreateView()
        {
            string viewName = nameCombo.Text;
            List<View3D> views = GetElements<View3D>(Doc);
            bool viewFlag = false;
            View3D selectView = null;

            foreach (View3D view in views) {
                if (view.Name == viewName) {
                    viewFlag = true;
                    selectView = view;
                    break;
                }
            }

            Level level1 = null;
            Level level2 = null;
            Level topEdge = null;
            String topEdgeName = topEdgeCombo.SelectedItem.ToString();
            Level lowerEnd = null;
            String lowerEndName = lowerEndCombo.SelectedItem.ToString();
            foreach (Level level in levels) {
                if (level.Name == topEdgeName) {
                    level1 = level;
                }
                if (level.Name == lowerEndName) {
                    level2 = level;
                }
            }
            if (level1.Elevation + topValue > level2.Elevation + lowerValue) {
                topEdge = level1;
                lowerEnd = level2;
            }
            else {
                ComDialog.ShowDialog(_res.ResourceText("IDS_WARN_TITLE"), TaskDialogIcon.TaskDialogIconWarning, _res.ResourceText("IDS_WARN_LEVEL_ORDER"), false);
                returnFlag = true;
                return;
            }

            if (viewFlag) {
                List<XYZ> boxValueList = new List<XYZ>();
                List<XYZ> cornerList = new List<XYZ>();
                double tilt = double.MinValue;
                double rad = 0;
                XYZ baseDirection = null;
                bool rotateFlag = false;
                double rightLength = 0;
                double leftLength = 0;
                foreach (Element element in mainElementList) {
                    BoundingBoxXYZ box = element.get_BoundingBox(UiDoc.ActiveView);
                    boxValueList.Add(box.Min);
                    boxValueList.Add(box.Max);
                    Options opt = new Options();
                    opt.ComputeReferences = true;
                    List<Solid> solidList = new List<Solid>();
                    if (element.GetType().Name == "FamilyInstance" || element.GetType().Name == "FabricationPart") {
                        GeometryElement geo = element.get_Geometry(opt);
                        foreach (GeometryObject obj in geo) {
                            if (obj.GetType().Name == "GeometryInstance") {
                                GeometryInstance gi = obj as GeometryInstance;
                                GeometryElement geo2 = gi.GetInstanceGeometry();
                                foreach (GeometryObject obj2 in geo2) {
                                    Solid solid = obj2 as Solid;
                                    if (solid != null) {
                                        solidList.Add(solid);
                                    }
                                }
                            }
                            else if (obj.GetType().Name == "Solid") {
                                Solid solid = obj as Solid;
                                if (solid != null) {
                                    solidList.Add(solid);
                                }
                            }
                        }
                    }
                    else {
                        GeometryElement geo = element.get_Geometry(opt);
                        if (geo != null) {
                            foreach (GeometryObject obj in geo) {
                                if (obj.GetType().Name == "Solid") {
                                    Solid solid = obj as Solid;
                                    if (solid != null) {
                                        solidList.Add(solid);
                                    }
                                }
                            }
                        }
                    }
                    Dictionary<XYZ, int> corners = GetCorners(solidList);
                    foreach (KeyValuePair<XYZ, int> kvp in corners) {
                        cornerList.Add(kvp.Key);
                    }
                    LocationCurve locationCurve = element.Location as LocationCurve;
                    if (locationCurve != null && locationCurve.Curve.GetType().Name == "Line") {
                        Line curve = (Line)locationCurve.Curve;
                        XYZ point1 = curve.GetEndPoint(0);
                        XYZ point2 = curve.GetEndPoint(1);
                        Line line;
                        if (point1.Y <= point2.Y) {
                            line = Line.CreateBound(point1, point2);
                        }
                        else {
                            line = Line.CreateBound(point2, point1);
                        }
                        XYZ direction = new XYZ(line.Direction.X, line.Direction.Y, 0);
                        XYZ zero = new XYZ(1, 0, 0);
                        rad = zero.AngleTo(direction);
                        if (baseDirection == null) {
                            baseDirection = direction;
                            rotateFlag = true;
                        }
                        else {
                            double rad2 = baseDirection.AngleTo(direction);
                            double angle2 = Math.Round(rad2 / Math.PI * 180, 0, MidpointRounding.AwayFromZero);
                            if (angle2 != 0 && angle2 != 90 && angle2 != 180 && angle2 != 270) {
                                rotateFlag = false;
                            }
                        }
                        if (Math.Tan(rad) > tilt) {
                            tilt = Math.Tan(rad);
                        }
                    }
                }

                foreach (Reference reference in linkRefList) {
                    Element element = Doc.GetElement(reference);
                    RevitLinkInstance linkInstance = (RevitLinkInstance)element;
                    Document linkDoc = linkInstance.GetLinkDocument();
                    Transform t = linkInstance.GetTotalTransform();
                    Element linkedElement = linkDoc.GetElement(reference.LinkedElementId);
                    BoundingBoxXYZ box = linkedElement.get_BoundingBox(UiDoc.ActiveView);
                    boxValueList.Add(t.OfPoint(box.Min));
                    boxValueList.Add(t.OfPoint(box.Max));
                    Options opt = new Options();
                    opt.ComputeReferences = true;
                    opt.DetailLevel = ViewDetailLevel.Fine;
                    List<Solid> solidList = new List<Solid>();
                    if (linkedElement.GetType().Name == "FamilyInstance" || linkedElement.GetType().Name == "FabricationPart") {
                        GeometryElement geo = linkedElement.get_Geometry(opt);
                        if (geo != null) {
                            foreach (GeometryObject obj in geo) {
                                if (obj.GetType().Name == "GeometryInstance") {
                                    GeometryInstance gi = obj as GeometryInstance;
                                    GeometryElement geo2 = gi.GetInstanceGeometry();
                                    foreach (GeometryObject obj2 in geo2) {
                                        Solid solid = obj2 as Solid;
                                        if (solid != null) {
                                            solidList.Add(solid);
                                        }
                                    }
                                }
                                else if (obj.GetType().Name == "Solid") {
                                    Solid solid = obj as Solid;
                                    if (solid != null) {
                                        solidList.Add(solid);
                                    }
                                }
                            }
                        }
                    }
                    else {
                        GeometryElement geo = linkedElement.get_Geometry(opt);
                        if (geo != null) {
                            foreach (GeometryObject obj in geo) {
                                if (obj.GetType().Name == "Solid") {
                                    Solid solid = obj as Solid;
                                    if (solid != null) {
                                        solidList.Add(solid);
                                    }
                                }
                            }
                        }
                    }
                    Dictionary<XYZ, int> corners = new Dictionary<XYZ, int>();
                    if (solidList.Count > 0) {
                        corners = GetCorners(solidList);
                    }
                    foreach (KeyValuePair<XYZ, int> kvp in corners) {
                        cornerList.Add(kvp.Key);
                    }
                    LocationCurve locationCurve = linkedElement.Location as LocationCurve;
                    if (locationCurve != null && locationCurve.Curve.GetType().Name == "Line") {
                        Line curve = (Line)locationCurve.Curve;
                        XYZ point1 = curve.GetEndPoint(0);
                        XYZ point2 = curve.GetEndPoint(1);
                        Line line;
                        if (point1.Y <= point2.Y) {
                            line = Line.CreateBound(point1, point2);
                        }
                        else {
                            line = Line.CreateBound(point2, point1);
                        }
                        XYZ direction = line.Direction;
                        XYZ linkDirection = t.OfPoint(direction);
                        linkDirection = new XYZ(linkDirection.X, linkDirection.Y, 0);
                        XYZ zero = new XYZ(1, 0, 0);
                        rad = zero.AngleTo(linkDirection);
                        if (baseDirection == null) {
                            baseDirection = linkDirection;
                            rotateFlag = true;
                        }
                        else {
                            double rad2 = baseDirection.AngleTo(linkDirection);
                            double angle2 = Math.Round(rad2 / Math.PI * 180, 0, MidpointRounding.AwayFromZero);
                            if (angle2 != 0 && angle2 != 90 && angle2 != 180 && angle2 != 270) {
                                rotateFlag = false;
                            }
                        }
                        if (Math.Tan(rad) > tilt) {
                            tilt = Math.Tan(rad);
                        }
                    }
                }

                if (boxValueList.Count > 0 && selectObjectRadio.Checked) {
                    boxValueList.Sort((a, b) => Math.Sign(a.X - b.X));
                    double maxX = boxValueList.Last().X;
                    double minX = boxValueList.First().X;
                    boxValueList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                    double maxY = boxValueList.Last().Y;
                    double minY = boxValueList.First().Y;
                    BoundingBoxXYZ box = new BoundingBoxXYZ();
                    box.Max = new XYZ(maxX, maxY, topEdge.Elevation + topValue);
                    box.Min = new XYZ(minX, minY, lowerEnd.Elevation + lowerValue);
                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                        tran.Start();
                        selectView.SetSectionBox(box);
                        tran.Commit();
                    }
                }
                if (cornerList.Count == 0) {
                    rotateFlag = false;
                }

                if (boxValueList.Count > 0 && objectCheck.Checked && rotateFlag) {
                    double mTilt = -(1 / tilt);
                    double mTilt2 = tilt + (1 / tilt);
                    double boxX = 0;
                    double boxY = 0;
                    cornerList.Sort((a, b) => Math.Sign(GetIntercept(tilt, a) - GetIntercept(tilt, b)));
                    XYZ minTiltPoint = cornerList.First();
                    XYZ maxTiltPoint = cornerList.Last();
                    cornerList.Sort((a, b) => Math.Sign(GetIntercept(mTilt, a) - GetIntercept(mTilt, b)));
                    XYZ minMtiltPoint = cornerList.First();
                    XYZ maxMtiltPoint = cornerList.Last();
                    XYZ leftBottomPoint = new XYZ(0, 0, 0);
                    double x1 = (GetInterceptXY(minMtiltPoint, mTilt) - GetInterceptXY(minTiltPoint, tilt)) / mTilt2;
                    double y1 = tilt * x1 + GetInterceptXY(minTiltPoint, tilt);
                    leftBottomPoint = new XYZ(x1, y1, 0);
                    XYZ leftTopPoint = new XYZ(0, 0, 0);
                    if (tilt > 0) {
                        x1 = (GetInterceptXY(minMtiltPoint, mTilt) - GetInterceptXY(maxTiltPoint, tilt)) / mTilt2;
                        y1 = tilt * x1 + GetInterceptXY(maxTiltPoint, tilt);
                        leftTopPoint = new XYZ(x1, y1, 0);
                    }
                    else {
                        x1 = (GetInterceptXY(maxMtiltPoint, mTilt) - GetInterceptXY(minTiltPoint, tilt)) / mTilt2;
                        y1 = tilt * x1 + GetInterceptXY(minTiltPoint, tilt);
                        leftTopPoint = new XYZ(x1, y1, 0);
                    }
                    XYZ rightBottomPoint = new XYZ(0, 0, 0);
                    if (tilt > 0) {
                        x1 = (GetInterceptXY(maxMtiltPoint, mTilt) - GetInterceptXY(minTiltPoint, tilt)) / mTilt2;
                        y1 = tilt * x1 + GetInterceptXY(minTiltPoint, tilt);
                        rightBottomPoint = new XYZ(x1, y1, 0);
                    }
                    else {
                        x1 = (GetInterceptXY(minMtiltPoint, mTilt) - GetInterceptXY(maxTiltPoint, tilt)) / mTilt2;
                        y1 = tilt * x1 + GetInterceptXY(maxTiltPoint, tilt);
                        rightBottomPoint = new XYZ(x1, y1, 0);
                    }
                    double BottomPoint1 = leftBottomPoint.X > rightBottomPoint.X ? leftBottomPoint.X - rightBottomPoint.X : rightBottomPoint.X - leftBottomPoint.X;
                    double height1 = leftBottomPoint.Y > rightBottomPoint.Y ? leftBottomPoint.Y - rightBottomPoint.Y : rightBottomPoint.Y - leftBottomPoint.Y;
                    rightLength = Math.Sqrt(Math.Pow(BottomPoint1, 2) + Math.Pow(height1, 2));
                    double BottomPoint2 = leftBottomPoint.X > leftTopPoint.X ? leftBottomPoint.X - leftTopPoint.X : leftTopPoint.X - leftBottomPoint.X;
                    double height2 = leftBottomPoint.Y > leftTopPoint.Y ? leftBottomPoint.Y - leftTopPoint.Y : leftTopPoint.Y - leftBottomPoint.Y;
                    leftLength = Math.Sqrt(Math.Pow(BottomPoint2, 2) + Math.Pow(height2, 2));
                    boxX = leftBottomPoint.X + rightLength;
                    boxY = leftBottomPoint.Y + leftLength;

                    BoundingBoxXYZ box = new BoundingBoxXYZ();
                    box.Max = new XYZ(boxX, boxY, topEdge.Elevation + topValue);
                    box.Min = new XYZ(leftBottomPoint.X, leftBottomPoint.Y, lowerEnd.Elevation + lowerValue);
                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                        tran.Start();
                        selectView.SetSectionBox(box);
                        tran.Commit();
                    }
                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                        tran.Start();
                        FilteredElementCollector boxCollector = new FilteredElementCollector(UiDoc.Document);
                        IList<Element> boxCollection = boxCollector.OfCategory(BuiltInCategory.OST_SectionBox).ToElements();
                        Element createBox = null;
                        foreach (Element element in boxCollection) {
                            BoundingBoxXYZ elementBox = element.get_BoundingBox(selectView);
                            if (elementBox == null) {
                                continue;
                            }
                            double elementMaxX = Math.Round(elementBox.Max.X, 4, MidpointRounding.AwayFromZero);
                            double elementMinX = Math.Round(elementBox.Min.X, 4, MidpointRounding.AwayFromZero);
                            double newMaxX = Math.Round(selectView.GetSectionBox().Max.X, 4, MidpointRounding.AwayFromZero);
                            double newMinX = Math.Round(selectView.GetSectionBox().Min.X, 4, MidpointRounding.AwayFromZero);

                            if (newMaxX == elementMaxX && newMinX == elementMinX) {
                                createBox = element;
                            }
                        }
                        XYZ point1 = new XYZ(leftBottomPoint.X, leftBottomPoint.Y, 0);
                        XYZ point2 = new XYZ(leftBottomPoint.X, leftBottomPoint.Y, 10);
                        Line axis = Line.CreateBound(point1, point2);
                        double angle = Math.Round(rad / Math.PI * 180, 0, MidpointRounding.AwayFromZero);
                        if (angle < 180 && angle > 90) {
                            angle = 90 - (180 - angle);
                            rad = angle * Math.PI / 180;
                        }
                        ElementTransformUtils.RotateElement(Doc, createBox.Id, axis, rad);
                        tran.Commit();
                    }
                }

                if (pickedBox != null && rangeSpecificationRadio.Checked) {
                    try {
                        XYZ min = pickedBox.Min;
                        XYZ max = pickedBox.Max;
                        double maxX = max.X > min.X ? max.X : min.X;
                        double minX = max.X > min.X ? min.X : max.X;
                        double maxY = max.Y > min.Y ? max.Y : min.Y;
                        double minY = max.Y > min.Y ? min.Y : max.Y;
                        BoundingBoxXYZ box = new BoundingBoxXYZ();
                        box.Max = new XYZ(maxX, maxY, topEdge.Elevation + topValue);
                        box.Min = new XYZ(minX, minY, lowerEnd.Elevation + lowerValue);
                        using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                            tran.Start();
                            selectView.SetSectionBox(box);
                            tran.Commit();
                        }
                    }
                    catch (Exception ex) {
                        string n = ex.ToString();
                    }
                }

                if (floorBoxRadio.Checked) {
                    List<XYZ> valueList = new List<XYZ>();
                    selectionElement = null;
                    List<Level> selectLevelList = new List<Level>();
                    System.Windows.Forms.ListBox.SelectedObjectCollection items = levelListBox.SelectedItems;
                    List<string> nameList = new List<string>();
                    foreach (object item in items) {
                        nameList.Add(item.ToString());
                    }

                    foreach (Level level in levels) {
                        foreach (String name in nameList) {
                            if (level.Name == name) {
                                using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                                    tran.Start();
                                    List<ElementId> elementIds = new List<ElementId>();
                                    elementIds.Add(level.Id);
                                    selectView.EnableRevealHiddenMode();
                                    Doc.Regenerate();
                                    BoundingBoxXYZ box = level.get_BoundingBox(selectView);
                                    if (box != null) {
                                        valueList.Add(box.Min);
                                        valueList.Add(box.Max);
                                    }
                                    selectView.DisableTemporaryViewMode(TemporaryViewMode.RevealHiddenElements);

                                    tran.Commit();
                                }
                            }
                        }
                    }

                    if (valueList.Count > 0) {
                        valueList.Sort((a, b) => Math.Sign(a.X - b.X));
                        double maxX = valueList.Last().X;
                        double minX = valueList.First().X;
                        valueList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                        double maxY = valueList.Last().Y;
                        double minY = valueList.First().Y;
                        BoundingBoxXYZ box = new BoundingBoxXYZ();
                        box.Max = new XYZ(maxX, maxY, topEdge.Elevation + topValue);
                        box.Min = new XYZ(minX, minY, lowerEnd.Elevation + lowerValue);
                        using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                            tran.Start();
                            selectView.SetSectionBox(box);
                            tran.Commit();
                        }
                    }
                }

                newView3d = selectView;
            }
            else {
                if (baseView == null) {
                    ComDialog.ShowDialog(_res.ResourceText("IDS_ERR_TITLE"), TaskDialogIcon.TaskDialogIconError, _res.ResourceText("IDS_ERR_NO_DEFAULT_3DVIEW"), false);
                    returnFlag = true;
                    Close();
                    return;
                }
                View3D newView = null;
                ElementId id;
                using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                    tran.Start();
                    ViewDuplicateOption opt = new ViewDuplicateOption();
                    id = baseView.Duplicate(opt);
                    tran.Commit();
                }
                newView = (View3D)(Doc.GetElement(id));

                List<XYZ> boxValueList = new List<XYZ>();
                List<XYZ> cornerList = new List<XYZ>();
                double tilt = double.MinValue;
                double rad = 0;
                XYZ baseDirection = null;
                bool rotateFlag = false;
                double rightLength = 0;
                double leftLength = 0;
                foreach (Element element in mainElementList) {
                    BoundingBoxXYZ box = element.get_BoundingBox(UiDoc.ActiveView);
                    boxValueList.Add(box.Min);
                    boxValueList.Add(box.Max);
                    Options opt = new Options();
                    opt.ComputeReferences = true;
                    opt.DetailLevel = ViewDetailLevel.Fine;
                    List<Solid> solidList = new List<Solid>();
                    if (element.GetType().Name == "FamilyInstance" || element.GetType().Name == "FabricationPart") {
                        GeometryElement geo = element.get_Geometry(opt);
                        foreach (GeometryObject obj in geo) {
                            if (obj.GetType().Name == "GeometryInstance") {
                                GeometryInstance gi = obj as GeometryInstance;
                                GeometryElement geo2 = gi.GetInstanceGeometry();
                                foreach (GeometryObject obj2 in geo2) {
                                    Solid solid = obj2 as Solid;
                                    if (solid != null) {
                                        solidList.Add(solid);
                                    }
                                }
                            }
                            else if (obj.GetType().Name == "Solid") {
                                Solid solid = obj as Solid;
                                if (solid != null) {
                                    solidList.Add(solid);
                                }
                            }
                        }
                    }
                    else {
                        GeometryElement geo = element.get_Geometry(opt);
                        if (geo != null) {
                            foreach (GeometryObject obj in geo) {
                                if (obj.GetType().Name == "Solid") {
                                    Solid solid = obj as Solid;
                                    if (solid != null) {
                                        solidList.Add(solid);
                                    }
                                }
                            }
                        }
                    }
                    Dictionary<XYZ, int> corners = GetCorners(solidList);
                    foreach (KeyValuePair<XYZ, int> kvp in corners) {
                        cornerList.Add(kvp.Key);
                    }
                    LocationCurve locationCurve = element.Location as LocationCurve;
                    if (locationCurve != null && locationCurve.Curve.GetType().Name == "Line") {
                        Line curve = (Line)locationCurve.Curve;
                        XYZ point1 = curve.GetEndPoint(0);
                        XYZ point2 = curve.GetEndPoint(1);
                        Line line;
                        if (point1.Y <= point2.Y) {
                            line = Line.CreateBound(point1, point2);
                        }
                        else {
                            line = Line.CreateBound(point2, point1);
                        }
                        XYZ direction = new XYZ(line.Direction.X, line.Direction.Y, 0);
                        XYZ zero = new XYZ(1, 0, 0);
                        rad = zero.AngleTo(direction);
                        double angle = Math.Round(rad / Math.PI * 180, 0, MidpointRounding.AwayFromZero);
                        if (baseDirection == null) {
                            baseDirection = direction;
                            rotateFlag = true;
                        }
                        else {
                            double rad2 = baseDirection.AngleTo(direction);
                            double angle2 = Math.Round(rad2 / Math.PI * 180, 0, MidpointRounding.AwayFromZero);
                            if (angle2 != 0 && angle2 != 90 && angle2 != 180 && angle2 != 270) {
                                rotateFlag = false;
                            }
                        }
                        if (Math.Tan(rad) > tilt) {
                            tilt = Math.Tan(rad);
                        }
                    }
                }

                foreach (Reference reference in linkRefList) {
                    Element element = Doc.GetElement(reference);
                    RevitLinkInstance linkInstance = (RevitLinkInstance)element;
                    Document linkDoc = linkInstance.GetLinkDocument();
                    Transform t = linkInstance.GetTotalTransform();
                    Element linkedElement = linkDoc.GetElement(reference.LinkedElementId);
                    BoundingBoxXYZ box = linkedElement.get_BoundingBox(UiDoc.ActiveView);
                    boxValueList.Add(t.OfPoint(box.Min));
                    boxValueList.Add(t.OfPoint(box.Max));
                    Options opt = new Options();
                    opt.ComputeReferences = true;
                    opt.DetailLevel = ViewDetailLevel.Fine;
                    List<Solid> solidList = new List<Solid>();
                    if (linkedElement.GetType().Name == "FamilyInstance" || linkedElement.GetType().Name == "FabricationPart") {
                        GeometryElement geo = linkedElement.get_Geometry(opt);
                        if (geo != null) {
                            foreach (GeometryObject obj in geo) {
                                if (obj.GetType().Name == "GeometryInstance") {
                                    GeometryInstance gi = obj as GeometryInstance;
                                    GeometryElement geo2 = gi.GetInstanceGeometry();
                                    foreach (GeometryObject obj2 in geo2) {
                                        Solid solid = obj2 as Solid;
                                        if (solid != null) {
                                            solidList.Add(solid);
                                        }
                                    }
                                }
                                else if (obj.GetType().Name == "Solid") {
                                    Solid solid = obj as Solid;
                                    if (solid != null) {
                                        solidList.Add(solid);
                                    }
                                }
                            }
                        }
                    }
                    else {
                        GeometryElement geo = linkedElement.get_Geometry(opt);
                        if (geo != null) {
                            foreach (GeometryObject obj in geo) {
                                if (obj.GetType().Name == "Solid") {
                                    Solid solid = obj as Solid;
                                    if (solid != null) {
                                        solidList.Add(solid);
                                    }
                                }
                            }
                        }
                    }
                    Dictionary<XYZ, int> corners = new Dictionary<XYZ, int>();
                    if (solidList.Count > 0) {
                        corners = GetCorners(solidList);
                    }
                    foreach (KeyValuePair<XYZ, int> kvp in corners) {
                        cornerList.Add(kvp.Key);
                    }
                    LocationCurve locationCurve = linkedElement.Location as LocationCurve;
                    if (locationCurve != null && locationCurve.Curve.GetType().Name == "Line") {
                        Line curve = (Line)locationCurve.Curve;
                        XYZ point1 = curve.GetEndPoint(0);
                        XYZ point2 = curve.GetEndPoint(1);
                        Line line;
                        if (point1.Y <= point2.Y) {
                            line = Line.CreateBound(point1, point2);
                        }
                        else {
                            line = Line.CreateBound(point2, point1);
                        }
                        XYZ direction = line.Direction;
                        XYZ linkDirection = t.OfPoint(direction);
                        linkDirection = new XYZ(linkDirection.X, linkDirection.Y, 0);
                        XYZ zero = new XYZ(1, 0, 0);
                        rad = zero.AngleTo(linkDirection);
                        if (baseDirection == null) {
                            baseDirection = linkDirection;
                            rotateFlag = true;
                        }
                        else {
                            double rad2 = baseDirection.AngleTo(linkDirection);
                            double angle2 = Math.Round(rad2 / Math.PI * 180, 0, MidpointRounding.AwayFromZero);
                            if (angle2 != 0 && angle2 != 90 && angle2 != 180 && angle2 != 270) {
                                rotateFlag = false;
                            }
                        }
                        if (Math.Tan(rad) > tilt) {
                            tilt = Math.Tan(rad);
                        }
                    }
                }

                if (boxValueList.Count > 0 && selectObjectRadio.Checked) {
                    boxValueList.Sort((a, b) => Math.Sign(a.X - b.X));
                    double maxX = boxValueList.Last().X;
                    double minX = boxValueList.First().X;
                    boxValueList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                    double maxY = boxValueList.Last().Y;
                    double minY = boxValueList.First().Y;
                    BoundingBoxXYZ box = new BoundingBoxXYZ();
                    box.Max = new XYZ(maxX, maxY, topEdge.Elevation + topValue);
                    box.Min = new XYZ(minX, minY, lowerEnd.Elevation + lowerValue);
                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                        tran.Start();
                        newView.SetSectionBox(box);
                        tran.Commit();
                    }
                }
                if (cornerList.Count == 0) {
                    rotateFlag = false;
                }

                if (boxValueList.Count > 0 && objectCheck.Checked && rotateFlag) {
                    double mTilt = -(1 / tilt);
                    double mTilt2 = tilt + (1 / tilt);
                    double boxX = 0;
                    double boxY = 0;
                    cornerList.Sort((a, b) => Math.Sign(GetIntercept(tilt, a) - GetIntercept(tilt, b)));
                    XYZ minTiltPoint = cornerList.First();
                    XYZ maxTiltPoint = cornerList.Last();
                    cornerList.Sort((a, b) => Math.Sign(GetIntercept(mTilt, a) - GetIntercept(mTilt, b)));
                    XYZ minMtiltPoint = cornerList.First();
                    XYZ maxMtiltPoint = cornerList.Last();
                    XYZ leftBottomPoint = new XYZ(0, 0, 0);
                    double x1 = (GetInterceptXY(minMtiltPoint, mTilt) - GetInterceptXY(minTiltPoint, tilt)) / mTilt2;
                    double y1 = tilt * x1 + GetInterceptXY(minTiltPoint, tilt);
                    leftBottomPoint = new XYZ(x1, y1, 0);
                    XYZ leftTopPoint = new XYZ(0, 0, 0);
                    if (tilt > 0) {
                        x1 = (GetInterceptXY(minMtiltPoint, mTilt) - GetInterceptXY(maxTiltPoint, tilt)) / mTilt2;
                        y1 = tilt * x1 + GetInterceptXY(maxTiltPoint, tilt);
                        leftTopPoint = new XYZ(x1, y1, 0);
                    }
                    else {
                        x1 = (GetInterceptXY(maxMtiltPoint, mTilt) - GetInterceptXY(minTiltPoint, tilt)) / mTilt2;
                        y1 = tilt * x1 + GetInterceptXY(minTiltPoint, tilt);
                        leftTopPoint = new XYZ(x1, y1, 0);
                    }
                    XYZ rightBottomPoint = new XYZ(0, 0, 0);
                    if (tilt > 0) {
                        x1 = (GetInterceptXY(maxMtiltPoint, mTilt) - GetInterceptXY(minTiltPoint, tilt)) / mTilt2;
                        y1 = tilt * x1 + GetInterceptXY(minTiltPoint, tilt);
                        rightBottomPoint = new XYZ(x1, y1, 0);
                    }
                    else {
                        x1 = (GetInterceptXY(minMtiltPoint, mTilt) - GetInterceptXY(maxTiltPoint, tilt)) / mTilt2;
                        y1 = tilt * x1 + GetInterceptXY(maxTiltPoint, tilt);
                        rightBottomPoint = new XYZ(x1, y1, 0);
                    }
                    double BottomPoint1 = leftBottomPoint.X > rightBottomPoint.X ? leftBottomPoint.X - rightBottomPoint.X : rightBottomPoint.X - leftBottomPoint.X;
                    double height1 = leftBottomPoint.Y > rightBottomPoint.Y ? leftBottomPoint.Y - rightBottomPoint.Y : rightBottomPoint.Y - leftBottomPoint.Y;
                    rightLength = Math.Sqrt(Math.Pow(BottomPoint1, 2) + Math.Pow(height1, 2));
                    double BottomPoint2 = leftBottomPoint.X > leftTopPoint.X ? leftBottomPoint.X - leftTopPoint.X : leftTopPoint.X - leftBottomPoint.X;
                    double height2 = leftBottomPoint.Y > leftTopPoint.Y ? leftBottomPoint.Y - leftTopPoint.Y : leftTopPoint.Y - leftBottomPoint.Y;
                    leftLength = Math.Sqrt(Math.Pow(BottomPoint2, 2) + Math.Pow(height2, 2));
                    boxX = leftBottomPoint.X + rightLength;
                    boxY = leftBottomPoint.Y + leftLength;

                    BoundingBoxXYZ box = new BoundingBoxXYZ();
                    box.Max = new XYZ(boxX, boxY, topEdge.Elevation + topValue);
                    box.Min = new XYZ(leftBottomPoint.X, leftBottomPoint.Y, lowerEnd.Elevation + lowerValue);
                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                        tran.Start();
                        newView.SetSectionBox(box);
                        tran.Commit();
                    }
                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                        tran.Start();
                        FilteredElementCollector boxCollector = new FilteredElementCollector(UiDoc.Document);
                        IList<Element> boxCollection = boxCollector.OfCategory(BuiltInCategory.OST_SectionBox).ToElements();
                        Element createBox = null;
                        foreach (Element element in boxCollection) {
                            BoundingBoxXYZ elementBox = element.get_BoundingBox(newView);
                            if (elementBox == null) {
                                continue;
                            }
                            double elementMaxX = Math.Round(elementBox.Max.X, 4, MidpointRounding.AwayFromZero);
                            double elementMinX = Math.Round(elementBox.Min.X, 4, MidpointRounding.AwayFromZero);
                            double newMaxX = Math.Round(newView.GetSectionBox().Max.X, 4, MidpointRounding.AwayFromZero);
                            double newMinX = Math.Round(newView.GetSectionBox().Min.X, 4, MidpointRounding.AwayFromZero);

                            if (newMaxX == elementMaxX && newMinX == elementMinX) {
                                createBox = element;
                            }
                        }
                        XYZ point1 = new XYZ(leftBottomPoint.X, leftBottomPoint.Y, 0);
                        XYZ point2 = new XYZ(leftBottomPoint.X, leftBottomPoint.Y, 10);
                        Line axis = Line.CreateBound(point1, point2);
                        double angle = Math.Round(rad / Math.PI * 180, 0, MidpointRounding.AwayFromZero);
                        if (angle < 180 && angle > 90) {
                            angle = 90 - (180 - angle);
                            rad = angle * Math.PI / 180;
                        }
                        ElementTransformUtils.RotateElement(Doc, createBox.Id, axis, rad);
                        tran.Commit();
                    }
                }

                if (pickedBox != null && rangeSpecificationRadio.Checked) {
                    try {
                        XYZ min = pickedBox.Min;
                        XYZ max = pickedBox.Max;
                        double maxX = max.X > min.X ? max.X : min.X;
                        double minX = max.X > min.X ? min.X : max.X;
                        double maxY = max.Y > min.Y ? max.Y : min.Y;
                        double minY = max.Y > min.Y ? min.Y : max.Y;
                        BoundingBoxXYZ box = new BoundingBoxXYZ();
                        box.Max = new XYZ(maxX, maxY, topEdge.Elevation + topValue);
                        box.Min = new XYZ(minX, minY, lowerEnd.Elevation + lowerValue);
                        using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                            tran.Start();
                            newView.SetSectionBox(box);
                            tran.Commit();
                        }
                    }
                    catch (Exception ex) {
                        string n = ex.ToString();
                    }
                }
                if (floorBoxRadio.Checked) {
                    List<XYZ> valueList = new List<XYZ>();
                    selectionElement = null;
                    List<Level> selectLevelList = new List<Level>();
                    System.Windows.Forms.ListBox.SelectedObjectCollection items = levelListBox.SelectedItems;
                    List<string> nameList = new List<string>();
                    foreach (object item in items) {
                        nameList.Add(item.ToString());
                    }

                    foreach (Level level in levels) {
                        foreach (String name in nameList) {
                            if (level.Name == name) {
                                using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                                    tran.Start();
                                    List<ElementId> elementIds = new List<ElementId>();
                                    elementIds.Add(level.Id);
                                    newView.EnableRevealHiddenMode();
                                    Doc.Regenerate();
                                    BoundingBoxXYZ box = level.get_BoundingBox(newView);
                                    if (box != null) {
                                        valueList.Add(box.Min);
                                        valueList.Add(box.Max);
                                    }
                                    newView.DisableTemporaryViewMode(TemporaryViewMode.RevealHiddenElements);

                                    tran.Commit();
                                }
                            }
                        }
                    }

                    if (valueList.Count > 0) {
                        valueList.Sort((a, b) => Math.Sign(a.X - b.X));
                        double maxX = valueList.Last().X;
                        double minX = valueList.First().X;
                        valueList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                        double maxY = valueList.Last().Y;
                        double minY = valueList.First().Y;
                        BoundingBoxXYZ box = new BoundingBoxXYZ();
                        box.Max = new XYZ(maxX, maxY, topEdge.Elevation + topValue);
                        box.Min = new XYZ(minX, minY, lowerEnd.Elevation + lowerValue);
                        using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                            tran.Start();
                            newView.SetSectionBox(box);
                            tran.Commit();
                        }
                    }
                }

                newView3d = newView;

                using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE"))) {
                    tran.Start();
                    newView.Name = viewName;
                    ParameterSet para = newView.Parameters;
                    foreach (Parameter p in para) {
                        if (p.Id == new ElementId(BuiltInParameter.VIEWER_CROP_REGION)) {
                            p.Set(0);
                        }
                        if (p.Id == new ElementId(BuiltInParameter.VIEWER_CROP_REGION_VISIBLE)) {
                            p.Set(0);
                        }
                    }
                    tran.Commit();
                }

                List<BuiltInCategory> builtInCategoryList = new List<BuiltInCategory>(){
                    BuiltInCategory.OST_StructuralFraming,
                    BuiltInCategory.OST_Floors
                };
                ElementMulticategoryFilter elementMulticategoryFilter = new ElementMulticategoryFilter(builtInCategoryList);

                FilteredElementCollector filteredElementCollector = new FilteredElementCollector(Doc);
                List<Element> elementList = new List<Element>();
                elementList = filteredElementCollector.WherePasses(elementMulticategoryFilter).WhereElementIsNotElementType().ToList();
                foreach (Element ele in elementList) {
                    if (ele.LevelId == topEdge.Id) {
                        elements.Add(ele);
                    }
                    else if (ele.GetType().Name == "FamilyInstance") {
                        Parameter refLevelParam = ele.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
                        if (refLevelParam != null && refLevelParam.AsValueString() == topEdge.Name) {
                            elements.Add(ele);
                        }
                    }
                }
            }
        }

        private Double GetInterceptXY(XYZ p1, double sl)
        {
            double intercept = 0;
            if (double.IsInfinity(sl)) {
                intercept = p1.X;
            }
            else if (Math.Round(sl, 3, MidpointRounding.AwayFromZero) == 0) {
                intercept = p1.Y;
            }
            else {
                intercept = p1.Y - (sl * p1.X);
            }
            return intercept;
        }

        private static void GetCorners(Dictionary<XYZ, int> corners, Solid solid)
        {
            foreach (Face f in solid.Faces) {
                foreach (EdgeArray ea in f.EdgeLoops) {
                    foreach (Edge e in ea) {
                        XYZ p = e.AsCurveFollowingFace(f).GetEndPoint(0);
                        if (!corners.ContainsKey(p)) {
                            corners[p] = 0;
                        }
                        ++corners[p];
                    }
                }
            }
        }

        public static Dictionary<XYZ, int> GetCorners(List<Solid> solids)
        {
            Dictionary<XYZ, int> corners = new Dictionary<XYZ, int>(new XyzEqualityComparer());
            foreach (Solid solid in solids) {
                GetCorners(corners, solid);
            }
            return corners;
        }

        private class XyzEqualityComparer : IEqualityComparer<XYZ>
        {
            private const double _sixteenthInchInFeet = 1.0 / (16.0 * 12.0);

            public bool Equals(XYZ p, XYZ q)
            {
                return p.IsAlmostEqualTo(q, _sixteenthInchInFeet);
            }

            public int GetHashCode(XYZ p)
            {
                return PointString(p).GetHashCode();
            }
        }

        public static string PointString(XYZ p)
        {
            return string.Format("({0},{1},{2})", RealString(p.X), RealString(p.Y), RealString(p.Z));
        }

        private static string RealString(double a)
        {
            return a.ToString("0.##");
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private double GetIntercept(double tilt, XYZ point)
        {
            double intercept = 0;
            intercept = point.Y - (tilt * point.X);
            return intercept;
        }

        public bool IsWhiteSpaceOnly(string str)
        {
            return !IsNullOrEmpty(str) && IsNullOrWhiteSpace(str);
        }

        public bool IsNullOrEmpty(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public bool IsNullOrWhiteSpace(string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        private void Text_Validation(object sender, CancelEventArgs e)
        {
            if (sender.GetType().Name == "TextBox") {
                TextBox text = (TextBox)sender;
                if (!int.TryParse(text.Text, out int t)) {
                    e.Cancel = false;
                    errorProviderApp.SetError(text, _res.ResourceText("IDS_ERR_INVALID_INPUT"));
                    executionButton.Enabled = false;
                }
                else {
                    e.Cancel = false;
                    errorProviderApp.SetError(text, "");
                }
            }
            if (!int.TryParse(topMargin.Text, out int fa)) {
                executionButton.Enabled = false;
            }
            else if (!int.TryParse(lowerMargin.Text, out int aa)) {
                executionButton.Enabled = false;
            }
            else {
                executionButton.Enabled = true;
            }
        }
    }
}
