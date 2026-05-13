#region Namespaces

using ADSK.JExtRAC.LayoutInstanceInRegion.Common;
using ADSK.JExtRAC.LayoutInstanceInRegion.Resources;
using ADSK.JExtRAC.LayoutInstanceInRegion.Screen;
using ADSK.JExtRAC.LayoutInstanceInRegion.Utils;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;

#endregion Namespaces

namespace ADSK.JExtRAC.LayoutInstanceInRegion.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CmdLayoutInstance : IExternalCommand
    {
        public static TransactionGroup transGroup;

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            CultureHelper.InitializeCulture();

            UIApplication UiApp = commandData.Application;
            UIDocument UiDoc = UiApp.ActiveUIDocument;
            Application App = UiApp.Application;
            Document Doc = UiDoc.Document;

            View view = UiDoc.ActiveView;

            if (view.ViewType != ViewType.CeilingPlan && view.ViewType != ViewType.EngineeringPlan && view.ViewType != ViewType.FloorPlan) {
                ComDialog.ShowDialog(Text.DialogWarning, TaskDialogIcon.TaskDialogIconWarning, Text.MsgViewTypeError, false);
                return Result.Succeeded;
            }
            transGroup = new TransactionGroup(Doc, Text.TransactionArrayLayout);
            transGroup.Start(Text.TransactionArrayLayout);

            while (true) {
                FormLayoutInstance form = new FormLayoutInstance(commandData);
                System.Windows.Forms.NativeWindow nativeWindow = System.Windows.Forms.NativeWindow.FromHandle(UiApp.MainWindowHandle);
                form.ShowDialog(nativeWindow);

                while (form.DialogResult == System.Windows.Forms.DialogResult.OK && form._isSelectObject) {
                    if (form._isObject) {
                        try {
                            ISelectionFilter selFilter = new RoomSelectionFilter();
                            Reference reference = UiDoc.Selection.PickObject(ObjectType.Element, selFilter);
                            if (reference != null) {
                                Element element = Doc.GetElement(reference.ElementId);
                                form.selectElement = element;
                                HashSet<ElementId> elementIdList = new HashSet<ElementId>();
                                elementIdList.Add(element.Id);
                                UiDoc.Selection.SetElementIds(elementIdList);
                                form.objectLabel.Text = Text.LabelSelected;
                                if (form.objectLabel.Text == Text.LabelNotSelected) {
                                    form.okButton.Enabled = false;
                                    form.applyButton.Enabled = false;
                                }
                                else {
                                    form.okButton.Enabled = true;
                                    form.applyButton.Enabled = true;
                                }
                            }
                        }
                        catch (System.Exception ex) {
                            string mess = ex.Message;
                        }
                        form.ShowDialog();
                    }
                    if (form._isRegion) {
                        try {
                            form.pickedBox = UiDoc.Selection.PickBox(PickBoxStyle.Enclosing);
                            if (form.pickedBox != null) {
                                form.regionLabel.Text = Text.LabelSpecified;
                                if (form.regionLabel.Text == Text.LabelNotSpecified) {
                                    form.okButton.Enabled = false;
                                    form.applyButton.Enabled = false;
                                }
                                else {
                                    form.okButton.Enabled = true;
                                    form.applyButton.Enabled = true;
                                }
                            }
                        }
                        catch (System.Exception ex) {
                            string mess = ex.Message;
                        }
                        form.ShowDialog();
                    }
                    if (form._isAngle) {
                        try {
                            XYZ point1 = UiDoc.Selection.PickPoint(Text.MsgPickStartPoint);
                            XYZ point2 = UiDoc.Selection.PickPoint(Text.MsgPickEndPoint);
                            if (point1 != null && point2 != null) {
                                Line line = Line.CreateBound(point1, point2);
                                XYZ zero = new XYZ(1, 0, 0);
                                XYZ mP2 = new XYZ(point2.X - point1.X, point2.Y - point1.Y, point2.Z - point1.Z);
                                double angle = zero.AngleTo(mP2) * 180 / Math.PI;
                                double mAngle = Math.Round(angle, 0, MidpointRounding.AwayFromZero);
                                while (mAngle > 90 || mAngle < -90) {
                                    if (mAngle > 0) {
                                        mAngle -= 180;
                                    }
                                    else {
                                        mAngle += 180;
                                    }
                                }
                                form.axisAngleCombo.Text = null;
                                form.axisAngleCombo.Text = mAngle.ToString();
                                using (Transaction tran = new Transaction(Doc, Text.TransactionArrayLayout)) {
                                    tran.Start();
                                    if (form.detailLine != null) {
                                        Doc.Delete(form.detailLine.Id);
                                    }
                                    form.detailLine = Doc.Create.NewDetailCurve(UiDoc.ActiveView, line);
                                    tran.Commit();
                                }
                            }
                        }
                        catch (System.Exception ex) {
                            ComDialog.ShowDialog(Text.DialogWarning, TaskDialogIcon.TaskDialogIconWarning, Text.MsgPointsTooClose, false);
                            string mess = ex.Message;
                        }
                        form.ShowDialog();
                    }
                    if (form.DialogResult == System.Windows.Forms.DialogResult.Yes) {
                        form._isObject = false;
                        form._isRegion = false;
                        form._isAngle = false;

                        transGroup.Commit();
                        transGroup.Start();
                        form.DialogResult = System.Windows.Forms.DialogResult.OK;
                        form.ShowDialog();
                    }
                }
                if (form.DialogResult != System.Windows.Forms.DialogResult.OK && form.DialogResult != System.Windows.Forms.DialogResult.Yes) {
                    transGroup.Assimilate();
                    return Result.Succeeded;
                }
            }
        }
    }

    public class RoomSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category == null) return false;

            var builtInCat = (BuiltInCategory)element.Category.Id.Value;
            return builtInCat == BuiltInCategory.OST_Rooms || builtInCat == BuiltInCategory.OST_MEPSpaces;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
}
