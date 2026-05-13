using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Form = System.Windows.Forms.Form;
using TextBox = System.Windows.Forms.TextBox;

namespace ADSK.JExtRAC.EnhancedSectionBox.Screen
{
    public partial class FormSectionBoxAdjustment : Form
    {
        public static UIApplication UiApp;
        public static Autodesk.Revit.ApplicationServices.Application App;
        public static Document Doc;
        public static UIDocument UiDoc;

        private BoundingBoxXYZ defaultBox;
        private double defaultMaxX;
        private double defaultMinX;
        private double defaultMaxY;
        private double defaultMinY;
        private double defaultMaxZ;
        private double defaultMinZ;
        private bool okFlag;

        private readonly Components.Attribute _res = new Components.Attribute();

        public FormSectionBoxAdjustment(ExternalCommandData commandData)
        {
            InitializeComponent();
            SetText();

            UiApp = commandData.Application;
            UiDoc = UiApp.ActiveUIDocument;
            Doc = UiDoc.Document;
            App = UiApp.Application;

            View3D view3d = (View3D)UiDoc.ActiveView;
            BoundingBoxXYZ box = view3d.GetSectionBox();
            defaultBox = box;

            okFlag = false;

            double maxX = box.Max.X;
            double minX = box.Min.X;
            double maxY = box.Max.Y;
            double minY = box.Min.Y;
            double maxZ = box.Max.Z;
            double minZ = box.Min.Z;

            defaultMaxX = box.Max.X;
            defaultMinX = box.Min.X;
            defaultMaxY = box.Max.Y;
            defaultMinY = box.Min.Y;
            defaultMaxZ = box.Max.Z;
            defaultMinZ = box.Min.Z;

            double centerX = UnitUtils.Convert((maxX + minX) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
            double mCenterX = Math.Round(centerX, 1, MidpointRounding.AwayFromZero);
            double centerY = UnitUtils.Convert((maxY + minY) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
            double mCenterY = Math.Round(centerY, 1, MidpointRounding.AwayFromZero);
            double centerZ = UnitUtils.Convert((maxZ + minZ) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
            double mCenterZ = Math.Round(centerZ, 1, MidpointRounding.AwayFromZero);

            double lengthX = UnitUtils.Convert((maxX - minX), UnitTypeId.Feet, UnitTypeId.Millimeters);
            double mLengthX = Math.Round(lengthX, 1, MidpointRounding.AwayFromZero);
            double lengthY = UnitUtils.Convert((maxY - minY), UnitTypeId.Feet, UnitTypeId.Millimeters);
            double mLengthY = Math.Round(lengthY, 1, MidpointRounding.AwayFromZero);
            double lengthZ = UnitUtils.Convert((maxZ - minZ), UnitTypeId.Feet, UnitTypeId.Millimeters);
            double mLengthZ = Math.Round(lengthZ, 1, MidpointRounding.AwayFromZero);

            textOffsetLeft.TextChanged += OffsetLeft_TextChanged;
            textOffsetRight.TextChanged += OffsetRight_TextChanged;
            textOffsetForward.TextChanged += OffsetForward_TextChanged;
            textOffsetBack.TextChanged += OffsetBack_TextChanged;
            textOffsetTop.TextChanged += OffsetTop_TextChanged;
            textOffsetBottom.TextChanged += OffsetBottom_TextChanged;

            textOffsetLeft.KeyPress += TextBoxPrice_PreviewTextInput;
            textOffsetRight.KeyPress += TextBoxPrice_PreviewTextInput;
            textOffsetForward.KeyPress += TextBoxPrice_PreviewTextInput;
            textOffsetBack.KeyPress += TextBoxPrice_PreviewTextInput;
            textOffsetTop.KeyPress += TextBoxPrice_PreviewTextInput;
            textOffsetBottom.KeyPress += TextBoxPrice_PreviewTextInput;

            textOffsetLeft.MaxLength = 7;
            textOffsetRight.MaxLength = 7;
            textOffsetForward.MaxLength = 7;
            textOffsetBack.MaxLength = 7;
            textOffsetTop.MaxLength = 7;
            textOffsetBottom.MaxLength = 7;
        }

        private void SetText()
        {
            this.Text = _res.ResourceText("IDS_FORM_SECTIONBOX_TITLE");
            groupBox2.Text = _res.ResourceText("IDS_GRP_LENGTH_ADJUST");
            label16.Text = _res.ResourceText("IDS_LBL_LEFT");
            label15.Text = _res.ResourceText("IDS_LBL_RIGHT");
            label18.Text = _res.ResourceText("IDS_LBL_FRONT");
            label17.Text = _res.ResourceText("IDS_LBL_BACK");
            label20.Text = _res.ResourceText("IDS_LBL_TOP");
            label19.Text = _res.ResourceText("IDS_LBL_BOTTOM");
            confirmButton.Text = _res.ResourceText("IDS_BTN_OK");
            canselButton.Text = _res.ResourceText("IDS_BTN_CANCEL");
        }

        private void LengthX_TextChanged(object sender, EventArgs e)
        {
            View3D view3d = (View3D)UiDoc.ActiveView;
            BoundingBoxXYZ box = view3d.GetSectionBox();

            double maxX = box.Max.X;
            double minX = box.Min.X;
            double maxY = box.Max.Y;
            double minY = box.Min.Y;
            double maxZ = box.Max.Z;
            double minZ = box.Min.Z;

            double centerPos = (maxX + minX) / 2;

            TextBox textBox = (TextBox)sender;
            double newMaxX = 0;
            double newMinX = 0;
            try {
                if (double.TryParse(textBox.Text, out double length)) {
                    double value = length / 2;
                    if (double.TryParse(textOffsetLeft.Text, out double leftOffset)) {
                        leftOffset = double.Parse(textOffsetLeft.Text);
                    }
                    if (double.TryParse(textOffsetRight.Text, out double rightOffset)) {
                        rightOffset = double.Parse(textOffsetRight.Text);
                    }
                    value = UnitUtils.Convert(value, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    newMaxX = centerPos + value + rightOffset;
                    newMinX = centerPos - value - leftOffset;

                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_CENTER_CHANGE"))) {
                        tran.Start();
                        box.Max = new XYZ(newMaxX, maxY, maxZ);
                        box.Min = new XYZ(newMinX, minY, minZ);
                        view3d.SetSectionBox(box);
                        tran.Commit();
                    }
                }
            }
            catch {
            }
        }

        private void LengthY_TextChanged(object sender, EventArgs e)
        {
            View3D view3d = (View3D)UiDoc.ActiveView;
            BoundingBoxXYZ box = view3d.GetSectionBox();

            double maxX = box.Max.X;
            double minX = box.Min.X;
            double maxY = box.Max.Y;
            double minY = box.Min.Y;
            double maxZ = box.Max.Z;
            double minZ = box.Min.Z;

            double centerPos = (maxY + minY) / 2;

            TextBox textBox = (TextBox)sender;
            double newMaxY = 0;
            double newMinY = 0;
            try {
                if (double.TryParse(textBox.Text, out double length)) {
                    double value = length / 2;
                    if (double.TryParse(textOffsetForward.Text, out double forwardOffset)) {
                        forwardOffset = double.Parse(textOffsetForward.Text);
                    }
                    if (double.TryParse(textOffsetBack.Text, out double backOffset)) {
                        backOffset = double.Parse(textOffsetBack.Text);
                    }
                    value = UnitUtils.Convert(value, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    newMaxY = centerPos + value + backOffset;
                    newMinY = centerPos - value - forwardOffset;

                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_CENTER_CHANGE"))) {
                        tran.Start();
                        box.Max = new XYZ(maxX, newMaxY, maxZ);
                        box.Min = new XYZ(minX, newMinY, minZ);
                        view3d.SetSectionBox(box);
                        tran.Commit();
                    }
                }
            }
            catch {
            }
        }

        private void LengthZ_TextChanged(object sender, EventArgs e)
        {
            View3D view3d = (View3D)UiDoc.ActiveView;
            BoundingBoxXYZ box = view3d.GetSectionBox();

            double maxX = box.Max.X;
            double minX = box.Min.X;
            double maxY = box.Max.Y;
            double minY = box.Min.Y;
            double maxZ = box.Max.Z;
            double minZ = box.Min.Z;

            double centerPos = (maxZ + minZ) / 2;

            TextBox textBox = (TextBox)sender;
            double newMaxZ = 0;
            double newMinZ = 0;
            try {
                if (double.TryParse(textBox.Text, out double length)) {
                    double value = length / 2;
                    if (double.TryParse(textOffsetTop.Text, out double topOffset)) {
                        topOffset = double.Parse(textOffsetTop.Text);
                    }
                    if (double.TryParse(textOffsetBottom.Text, out double bottomOffset)) {
                        bottomOffset = double.Parse(textOffsetBottom.Text);
                    }
                    value = UnitUtils.Convert(value, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    newMaxZ = centerPos + value + topOffset;
                    newMinZ = centerPos - value - topOffset;

                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_CENTER_CHANGE"))) {
                        tran.Start();
                        box.Max = new XYZ(maxX, maxY, newMaxZ);
                        box.Min = new XYZ(minX, minY, newMinZ);
                        view3d.SetSectionBox(box);
                        tran.Commit();
                    }
                }
            }
            catch {
            }
        }

        private void OffsetLeft_TextChanged(object sender, EventArgs e)
        {
            View3D view3d = (View3D)UiDoc.ActiveView;
            BoundingBoxXYZ box = defaultBox;

            double maxX = defaultMaxX;
            double minX = defaultMinX;
            double maxY = defaultMaxY;
            double minY = defaultMinY;
            double maxZ = defaultMaxZ;
            double minZ = defaultMinZ;

            double centerPos = (maxX + minX) / 2;
            double length = (maxX - minX) / 2;

            TextBox textBox = (TextBox)sender;
            double newMinX = 0;
            try {
                if (double.TryParse(textOffsetLeft.Text, out double leftOffset)) {
                    leftOffset = UnitUtils.Convert(leftOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    double.TryParse(textOffsetRight.Text, out double rightOffset);
                    rightOffset = UnitUtils.Convert(rightOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    newMinX = centerPos - length - leftOffset;

                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_OFFSET"))) {
                        tran.Start();
                        BoundingBoxXYZ nBox = view3d.GetSectionBox();
                        nBox.Max = new XYZ(nBox.Max.X, nBox.Max.Y, nBox.Max.Z);
                        nBox.Min = new XYZ(newMinX, nBox.Min.Y, nBox.Min.Z);
                        view3d.SetSectionBox(nBox);
                        tran.Commit();
                    }
                    BoundingBoxXYZ newBox = view3d.GetSectionBox();
                    maxX = newBox.Max.X;
                    minX = newBox.Min.X;
                    maxY = newBox.Max.Y;
                    minY = newBox.Min.Y;
                    maxZ = newBox.Max.Z;
                    minZ = newBox.Min.Z;

                    double centerX = UnitUtils.Convert((maxX + minX) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterX = Math.Round(centerX, 1, MidpointRounding.AwayFromZero);
                    double centerY = UnitUtils.Convert((maxY + minY) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterY = Math.Round(centerY, 1, MidpointRounding.AwayFromZero);
                    double centerZ = UnitUtils.Convert((maxZ + minZ) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterZ = Math.Round(centerZ, 1, MidpointRounding.AwayFromZero);
                }
            }
            catch {
            }
        }

        private void OffsetRight_TextChanged(object sender, EventArgs e)
        {
            View3D view3d = (View3D)UiDoc.ActiveView;
            BoundingBoxXYZ box = defaultBox;

            double maxX = defaultMaxX;
            double minX = defaultMinX;
            double maxY = defaultMaxY;
            double minY = defaultMinY;
            double maxZ = defaultMaxZ;
            double minZ = defaultMinZ;

            double centerPos = (maxX + minX) / 2;
            double length = (maxX - minX) / 2;

            TextBox textBox = (TextBox)sender;
            double newMaxX = 0;
            try {
                if (double.TryParse(textOffsetRight.Text, out double rightOffset)) {
                    double.TryParse(textOffsetLeft.Text, out double leftOffset);
                    leftOffset = UnitUtils.Convert(leftOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    rightOffset = UnitUtils.Convert(rightOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    newMaxX = centerPos + length + rightOffset;

                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_OFFSET"))) {
                        tran.Start();
                        BoundingBoxXYZ nBox = view3d.GetSectionBox();
                        nBox.Max = new XYZ(newMaxX, nBox.Max.Y, nBox.Max.Z);
                        nBox.Min = new XYZ(nBox.Min.X, nBox.Min.Y, nBox.Min.Z);
                        view3d.SetSectionBox(nBox);
                        tran.Commit();
                    }
                    BoundingBoxXYZ newBox = view3d.GetSectionBox();
                    maxX = newBox.Max.X;
                    minX = newBox.Min.X;
                    maxY = newBox.Max.Y;
                    minY = newBox.Min.Y;
                    maxZ = newBox.Max.Z;
                    minZ = newBox.Min.Z;

                    double centerX = UnitUtils.Convert((maxX + minX) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterX = Math.Round(centerX, 1, MidpointRounding.AwayFromZero);
                    double centerY = UnitUtils.Convert((maxY + minY) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterY = Math.Round(centerY, 1, MidpointRounding.AwayFromZero);
                    double centerZ = UnitUtils.Convert((maxZ + minZ) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterZ = Math.Round(centerZ, 1, MidpointRounding.AwayFromZero);
                }
            }
            catch {
            }
        }

        private void OffsetForward_TextChanged(object sender, EventArgs e)
        {
            View3D view3d = (View3D)UiDoc.ActiveView;
            BoundingBoxXYZ box = defaultBox;

            double maxX = defaultMaxX;
            double minX = defaultMinX;
            double maxY = defaultMaxY;
            double minY = defaultMinY;
            double maxZ = defaultMaxZ;
            double minZ = defaultMinZ;

            double centerPos = (maxY + minY) / 2;
            double length = (maxY - minY) / 2;

            TextBox textBox = (TextBox)sender;
            double newMinY = 0;
            try {
                if (double.TryParse(textOffsetForward.Text, out double forwardOffset)) {
                    forwardOffset = UnitUtils.Convert(forwardOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    double.TryParse(textOffsetBack.Text, out double backOffset);
                    backOffset = UnitUtils.Convert(backOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    newMinY = centerPos - length - forwardOffset;

                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_OFFSET"))) {
                        tran.Start();
                        BoundingBoxXYZ nBox = view3d.GetSectionBox();
                        nBox.Max = new XYZ(nBox.Max.X, nBox.Max.Y, nBox.Max.Z);
                        nBox.Min = new XYZ(nBox.Min.X, newMinY, nBox.Min.Z);
                        view3d.SetSectionBox(nBox);
                        tran.Commit();
                    }
                    BoundingBoxXYZ newBox = view3d.GetSectionBox();
                    maxX = newBox.Max.X;
                    minX = newBox.Min.X;
                    maxY = newBox.Max.Y;
                    minY = newBox.Min.Y;
                    maxZ = newBox.Max.Z;
                    minZ = newBox.Min.Z;

                    double centerX = UnitUtils.Convert((maxX + minX) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterX = Math.Round(centerX, 1, MidpointRounding.AwayFromZero);
                    double centerY = UnitUtils.Convert((maxY + minY) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterY = Math.Round(centerY, 1, MidpointRounding.AwayFromZero);
                    double centerZ = UnitUtils.Convert((maxZ + minZ) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterZ = Math.Round(centerZ, 1, MidpointRounding.AwayFromZero);
                }
            }
            catch {
            }
        }

        private void OffsetBack_TextChanged(object sender, EventArgs e)
        {
            View3D view3d = (View3D)UiDoc.ActiveView;
            BoundingBoxXYZ box = defaultBox;

            double maxX = defaultMaxX;
            double minX = defaultMinX;
            double maxY = defaultMaxY;
            double minY = defaultMinY;
            double maxZ = defaultMaxZ;
            double minZ = defaultMinZ;

            double centerPos = (maxY + minY) / 2;
            double length = (maxY - minY) / 2;

            TextBox textBox = (TextBox)sender;
            double newMaxY = 0;
            try {
                if (double.TryParse(textOffsetBack.Text, out double backOffset)) {
                    double.TryParse(textOffsetBack.Text, out double forwardOffset);
                    forwardOffset = UnitUtils.Convert(forwardOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);

                    backOffset = UnitUtils.Convert(backOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    newMaxY = centerPos + length + backOffset;

                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_OFFSET"))) {
                        tran.Start();
                        BoundingBoxXYZ nBox = view3d.GetSectionBox();
                        nBox.Max = new XYZ(nBox.Max.X, newMaxY, nBox.Max.Z);
                        nBox.Min = new XYZ(nBox.Min.X, nBox.Min.Y, nBox.Min.Z);
                        view3d.SetSectionBox(nBox);
                        tran.Commit();
                    }
                    BoundingBoxXYZ newBox = view3d.GetSectionBox();
                    maxX = newBox.Max.X;
                    minX = newBox.Min.X;
                    maxY = newBox.Max.Y;
                    minY = newBox.Min.Y;
                    maxZ = newBox.Max.Z;
                    minZ = newBox.Min.Z;

                    double centerX = UnitUtils.Convert((maxX + minX) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterX = Math.Round(centerX, 1, MidpointRounding.AwayFromZero);
                    double centerY = UnitUtils.Convert((maxY + minY) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterY = Math.Round(centerY, 1, MidpointRounding.AwayFromZero);
                    double centerZ = UnitUtils.Convert((maxZ + minZ) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterZ = Math.Round(centerZ, 1, MidpointRounding.AwayFromZero);
                }
            }
            catch {
            }
        }

        private void OffsetTop_TextChanged(object sender, EventArgs e)
        {
            View3D view3d = (View3D)UiDoc.ActiveView;
            BoundingBoxXYZ box = defaultBox;

            double maxX = defaultMaxX;
            double minX = defaultMinX;
            double maxY = defaultMaxY;
            double minY = defaultMinY;
            double maxZ = defaultMaxZ;
            double minZ = defaultMinZ;

            double centerPos = (maxZ + minZ) / 2;
            double length = (maxZ - minZ) / 2;

            TextBox textBox = (TextBox)sender;
            double newMaxZ = 0;
            try {
                if (double.TryParse(textOffsetTop.Text, out double topOffset)) {
                    topOffset = UnitUtils.Convert(topOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    double.TryParse(textOffsetBottom.Text, out double bottomOffset);
                    bottomOffset = UnitUtils.Convert(bottomOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    newMaxZ = centerPos + length + topOffset;

                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_OFFSET"))) {
                        tran.Start();
                        BoundingBoxXYZ nBox = view3d.GetSectionBox();
                        nBox.Max = new XYZ(nBox.Max.X, nBox.Max.Y, newMaxZ);
                        nBox.Min = new XYZ(nBox.Min.X, nBox.Min.Y, nBox.Min.Z);
                        view3d.SetSectionBox(nBox);
                        tran.Commit();
                    }
                    BoundingBoxXYZ newBox = view3d.GetSectionBox();
                    maxX = newBox.Max.X;
                    minX = newBox.Min.X;
                    maxY = newBox.Max.Y;
                    minY = newBox.Min.Y;
                    maxZ = newBox.Max.Z;
                    minZ = newBox.Min.Z;

                    double centerX = UnitUtils.Convert((maxX + minX) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterX = Math.Round(centerX, 1, MidpointRounding.AwayFromZero);
                    double centerY = UnitUtils.Convert((maxY + minY) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterY = Math.Round(centerY, 1, MidpointRounding.AwayFromZero);
                    double centerZ = UnitUtils.Convert((maxZ + minZ) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterZ = Math.Round(centerZ, 1, MidpointRounding.AwayFromZero);
                }
            }
            catch {
            }
        }

        private void OffsetBottom_TextChanged(object sender, EventArgs e)
        {
            View3D view3d = (View3D)UiDoc.ActiveView;
            BoundingBoxXYZ box = defaultBox;

            double maxX = defaultMaxX;
            double minX = defaultMinX;
            double maxY = defaultMaxY;
            double minY = defaultMinY;
            double maxZ = defaultMaxZ;
            double minZ = defaultMinZ;

            double centerPos = (maxZ + minZ) / 2;
            double length = (maxZ - minZ) / 2;

            TextBox textBox = (TextBox)sender;
            double newMinZ = 0;
            try {
                if (double.TryParse(textOffsetBottom.Text, out double bottomOffset)) {
                    double.TryParse(textOffsetTop.Text, out double topOffset);
                    topOffset = UnitUtils.Convert(topOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    bottomOffset = UnitUtils.Convert(bottomOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                    newMinZ = centerPos - length - bottomOffset;

                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_OFFSET"))) {
                        tran.Start();
                        BoundingBoxXYZ nBox = view3d.GetSectionBox();
                        nBox.Max = new XYZ(nBox.Max.X, nBox.Max.Y, nBox.Max.Z);
                        nBox.Min = new XYZ(nBox.Min.X, nBox.Min.Y, newMinZ);
                        view3d.SetSectionBox(nBox);
                        tran.Commit();
                    }
                    BoundingBoxXYZ newBox = view3d.GetSectionBox();
                    maxX = newBox.Max.X;
                    minX = newBox.Min.X;
                    maxY = newBox.Max.Y;
                    minY = newBox.Min.Y;
                    maxZ = newBox.Max.Z;
                    minZ = newBox.Min.Z;

                    double centerX = UnitUtils.Convert((maxX + minX) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterX = Math.Round(centerX, 1, MidpointRounding.AwayFromZero);
                    double centerY = UnitUtils.Convert((maxY + minY) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterY = Math.Round(centerY, 1, MidpointRounding.AwayFromZero);
                    double centerZ = UnitUtils.Convert((maxZ + minZ) / 2, UnitTypeId.Feet, UnitTypeId.Millimeters);
                    double mCenterZ = Math.Round(centerZ, 1, MidpointRounding.AwayFromZero);
                }
            }
            catch {
            }
        }

        private void OkButton_Click(object sender, System.EventArgs e)
        {
            okFlag = true;
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            Close();
        }

        private void CanselButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormClosing_Event(object sender, FormClosingEventArgs e)
        {
            if (!okFlag) {
                this.DialogResult = System.Windows.Forms.DialogResult.No;
            }
        }

        private void TextBoxPrice_PreviewTextInput(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b') {
                return;
            }
            if (e.KeyChar == '-' || e.KeyChar == '.') {
                return;
            }
            if ((e.KeyChar < '0' || '9' < e.KeyChar)) {
                e.Handled = true;
            }
        }

        private void Text_Validation(object sender, CancelEventArgs e)
        {
            if (sender.GetType().Name == "TextBox") {
                TextBox text = (TextBox)sender;
                if (!int.TryParse(text.Text, out int t)) {
                    e.Cancel = false;
                    errorProviderApp.SetError(text, _res.ResourceText("IDS_ERR_INVALID_INPUT"));
                    confirmButton.Enabled = false;
                }
                else {
                    e.Cancel = false;
                    errorProviderApp.SetError(text, "");
                }
            }
            if (!int.TryParse(textOffsetLeft.Text, out int fa)) {
                confirmButton.Enabled = false;
            }
            else if (!int.TryParse(textOffsetRight.Text, out int aa)) {
                confirmButton.Enabled = false;
            }
            else if (!int.TryParse(textOffsetForward.Text, out int fr)) {
                confirmButton.Enabled = false;
            }
            else if (!int.TryParse(textOffsetBack.Text, out int ba)) {
                confirmButton.Enabled = false;
            }
            else if (!int.TryParse(textOffsetTop.Text, out int to)) {
                confirmButton.Enabled = false;
            }
            else if (!int.TryParse(textOffsetBottom.Text, out int bo)) {
                confirmButton.Enabled = false;
            }
            else {
                confirmButton.Enabled = true;
            }
        }
    }
}
