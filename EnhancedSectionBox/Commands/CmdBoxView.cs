using ADSK.JExtRAC.EnhancedSectionBox.Screen;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

namespace ADSK.JExtRAC.EnhancedSectionBox.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CmdBoxViewN : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uiDoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uiDoc.Document;

            var res = new Components.Attribute();

            using (TransactionGroup transGroup = new TransactionGroup(doc, res.ResourceText("IDS_TRAN_BOXVIEW_CREATE"))) {
                transGroup.Start(res.ResourceText("IDS_TRAN_BOXVIEW_CREATE"));
                while (true) {
                    FormBoxView form = new FormBoxView(commandData);
                    System.Windows.Forms.NativeWindow nativeWindow = System.Windows.Forms.NativeWindow.FromHandle(uiapp.MainWindowHandle);
                    form.ShowDialog(nativeWindow);

                    while (form.DialogResult == System.Windows.Forms.DialogResult.OK && form._isSelectObject) {
                        if (form._isObject) {
                            try {
                                ISelectionFilter setFilter = new ObjectSelectionFilter();
                                IList<Reference> refList = uiDoc.Selection.PickObjects(ObjectType.Element, setFilter);
                                List<Element> elementList = new List<Element>();
                                foreach (Reference reference in refList) {
                                    elementList.Add(doc.GetElement(reference));
                                }

                                FormBoxView.mainElementList = elementList;
                                HashSet<ElementId> elementIdList = new HashSet<ElementId>();
                                if (elementList.Count > 0) {
                                    FormBoxView.selectionElementList = (List<Element>)elementList;
                                    foreach (Element element in elementList) {
                                        elementIdList.Add(element.Id);
                                    }
                                }

                                foreach (Element element in FormBoxView.linkElementList) {
                                    elementIdList.Add(element.Id);
                                }

                                uiDoc.Selection.SetElementIds(elementIdList);
                                form.mainCountLabel.Text = res.ResourceText("IDS_TXT_SELCOUNT") + FormBoxView.mainElementList.Count;
                            }
                            catch (System.Exception ex) {
                                string mess = ex.Message;
                            }

                            form.ShowDialog();
                        }

                        if (form._isLink) {
                            try {
                                IList<Reference> refList = uiDoc.Selection.PickObjects(ObjectType.LinkedElement);
                                List<Element> elementList = new List<Element>();
                                FormBoxView.linkRefList = refList;
                                foreach (Reference reference in refList) {
                                    Element element = doc.GetElement(reference);
                                    Element linkedElement = doc.GetElement(reference.LinkedElementId);
                                    RevitLinkInstance instance = (RevitLinkInstance)element;
                                    Document linkDoc = instance.GetLinkDocument();
                                    Element linkedElement2 = linkDoc.GetElement(reference.LinkedElementId);
                                    elementList.Add(linkedElement2);
                                }

                                FormBoxView.linkElementList = elementList;
                                HashSet<ElementId> elementIdList = new HashSet<ElementId>();
                                if (elementList.Count > 0) {
                                    FormBoxView.selectionElementList = (List<Element>)elementList;
                                    foreach (Element element in elementList) {
                                        elementIdList.Add(element.Id);
                                    }
                                }

                                foreach (Element element in FormBoxView.mainElementList) {
                                    elementIdList.Add(element.Id);
                                }

                                uiDoc.Selection.SetElementIds(elementIdList);
                                form.linkCountLabel.Text = res.ResourceText("IDS_TXT_SELCOUNT") + FormBoxView.linkElementList.Count;
                            }
                            catch (System.Exception ex) {
                                string mess = ex.Message;
                            }

                            form.ShowDialog();
                        }

                        if (form._isRegion) {
                            try {
                                PickedBox box = uiDoc.Selection.PickBox(PickBoxStyle.Enclosing);
                                form.pickedBox = box;
                                if (form.pickedBox != null) {
                                    form.regionLabel.Text = res.ResourceText("IDS_TXT_RANGE_SPECIFIED");
                                }
                            }
                            catch (System.Exception ex) {
                                string mess = ex.Message;
                            }

                            form.ShowDialog();
                        }
                    }

                    if (form.DialogResult != System.Windows.Forms.DialogResult.OK) {
                        transGroup.Assimilate();
                        return Result.Succeeded;
                    }
                }
            }
        }
    }

    public class ObjectSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category != null
                && element.Category.BuiltInCategory == BuiltInCategory.OST_RvtLinks) {
                return false;
            }
            return true;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
}
