using ADSK.JExtRAC.SwitchJoinOrder.UI;
using ADSK.JExtRAC.SwitchJoinOrder.Entities;
using ADSK.JExtRAC.SwitchJoinOrder.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RvtExtApp = ADSK.JExtRAC.SwitchJoinOrder;

namespace ADSK.JExtRAC.SwitchJoinOrder.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class CmdSwitchJoin : IExternalCommand
    {
        public static UIApplication _rvtUIApp;

        private Dictionary<ElementId, List<Solid>> m_DicSolidsElement =
            new Dictionary<ElementId, List<Solid>>();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            CultureHelper.InitializeCulture();

            _rvtUIApp = commandData.Application;
            UIDocument rvtUIDoc = _rvtUIApp.ActiveUIDocument;
            RvtExtApp.Components.Attribute cmpAttribute = new RvtExtApp.Components.Attribute();
            RvtExtApp.Components.Elements cmpElements = new RvtExtApp.Components.Elements(rvtUIDoc);

            Result retExtCom = Result.Succeeded;

            List<Element> listElement = cmpElements.getAllElement(rvtUIDoc, rvtUIDoc.Document);

            if (listElement == null || listElement.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_INFORMATION"), cmpAttribute.ResourceText("IDS_TXT_ERROR"), System.Windows.Forms.MessageBoxButtons.OK);
                return Result.Failed;
            }

            CategoryItems groupCategory = cmpElements.GroupingData(rvtUIDoc.Document, listElement);

            RvtExtApp.UI.FormSwitchJoin form = new RvtExtApp.UI.FormSwitchJoin(rvtUIDoc.Document, cmpAttribute, groupCategory);

            foreach (System.Windows.Forms.Form openedForm in System.Windows.Forms.Application.OpenForms)
            {
                if (openedForm.GetType().Name == form.Name)
                    return Result.Failed;
            }
            if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return Result.Cancelled;

            try
            {
                bool isJoinGeometry = !form.GetChecked;
                System.Text.StringBuilder strLog = new System.Text.StringBuilder();
                m_DicSolidsElement.Clear();

                int numProgress = groupCategory.CountEle();
                int cntProgress = 0;

                ProgressDialog progressDialog = new ProgressDialog(cmpAttribute.ResourceText("IDS_TXT_PROGESSBAR"), numProgress);
                progressDialog.ShowNonModal();

                Transaction tr = new Transaction(cmpElements.RvtDBDoc);
                tr.Start("SwitchJoinOder");

                foreach (var cate in groupCategory._categoryShow)
                {
                    if (cate._isJoinFami)
                        SwitchJoinFamily(rvtUIDoc, cmpAttribute, cmpElements, cate, isJoinGeometry, ref strLog, ref cntProgress, progressDialog);

                    List<ElementId> listElementIdFind = new List<ElementId>();
                    foreach (var ss in groupCategory._categoryShow)
                    {
                        if (cate.index >= ss.index)
                            continue;
                        listElementIdFind.AddRange(ss._listElementId);
                    }
                    if (listElementIdFind.Count == 0)
                        continue;

                    foreach (var fistElementId in cate._listElementId)
                    {
                        var firstElement = rvtUIDoc.Document.GetElement(fistElementId) as Element;
                        Join(cmpElements, firstElement, listElementIdFind, isJoinGeometry, false, ref strLog);
                        progressDialog.UpdateProgress(++cntProgress);
                    }
                }

                progressDialog.Close();
                progressDialog.Dispose();

                rvtUIDoc.Document.Regenerate();
                tr.Commit();

                if (strLog.Length != 0)
                {
                    FormLog frmLog = new FormLog(cmpAttribute, strLog);
                    frmLog.ShowDialog();
                }
            }
            catch (Exception)
            {
            }

            return retExtCom;
        }

        private bool SwitchJoinFamily(UIDocument rvtUIDoc,
            RvtExtApp.Components.Attribute cmpAttribute,
            RvtExtApp.Components.Elements cmpElements, CategoryItem cate, bool isJoinGeometry,
            ref System.Text.StringBuilder strLog,
            ref int cntProgress,
            ProgressDialog progressDialog)
        {
            bool ret = true;
            try
            {
                if (cate == null || cate._listFamilyItem == null || cate._listFamilyItem.Count == 0)
                    return false;

                SubTransaction subTr = new SubTransaction(cmpElements.RvtDBDoc);
                subTr.Start();

                foreach (var fami in cate._listFamilyItem)
                {
                    List<ElementId> listElementIdFind = new List<ElementId>();
                    foreach (var ss in cate._listFamilyItem)
                    {
                        if (fami._indexFami >= ss._indexFami)
                            continue;
                        listElementIdFind.AddRange(ss._listElementIdOfFamily);
                    }
                    if (listElementIdFind.Count == 0)
                        continue;

                    foreach (var fistElementId in fami._listElementIdOfFamily)
                    {
                        var firstElement = rvtUIDoc.Document.GetElement(fistElementId) as Element;
                        Join(cmpElements, firstElement, listElementIdFind, isJoinGeometry, true, ref strLog);
                        progressDialog.UpdateProgress(++cntProgress);
                    }
                }

                subTr.Commit();
            }
            catch (Exception) { }
            return ret;
        }

        private void Join(RvtExtApp.Components.Elements cmpElements,
            Element firstElement,
            List<ElementId> listElementIdFind,
            bool isJoinGeometry,
            bool isSameCategory,
            ref System.Text.StringBuilder strLog)
        {
            try
            {
                UIDocument rvtUIDoc = _rvtUIApp.ActiveUIDocument;
                List<ElementId> listJoinedId = new List<ElementId>();

                if (isJoinGeometry)
                {
                    ICollection<ElementId> listIntersection
                                = cmpElements.GetElementIntersectsFilter(rvtUIDoc.Document, listElementIdFind, firstElement);

                    if (listIntersection != null && listIntersection.Count > 0)
                    {
                        foreach (ElementId eleId in listIntersection)
                        {
                            Element secondElement = rvtUIDoc.Document.GetElement(eleId);
                            if (secondElement == null) continue;
                            if (secondElement.Category == null || firstElement.Category == null) continue;

                            if (isSameCategory)
                            {
                                if (secondElement.Category.Name != firstElement.Category.Name)
                                    continue;
                            }
                            else
                            {
                                if (secondElement.Category.Name == firstElement.Category.Name)
                                    continue;
                            }

                            if (!JoinGeometryUtils.AreElementsJoined(rvtUIDoc.Document, firstElement, secondElement))
                            {
                                if (ElementIntersectsElementFilter.IsElementSupported(firstElement) && ElementIntersectsElementFilter.IsElementSupported(secondElement) &&
                                    (ElementIntersectsElementFilter.IsCategorySupported(firstElement) && ElementIntersectsElementFilter.IsCategorySupported(secondElement)))
                                {
                                    SubTransaction subtr = new SubTransaction(rvtUIDoc.Document);
                                    subtr.Start();
                                    try
                                    {
                                        JoinGeometryUtils.JoinGeometry(rvtUIDoc.Document, secondElement, firstElement);
                                    }
                                    catch (Exception ex)
                                    {
                                        strLog.AppendLine("-----------------------");
                                        strLog.AppendLine(ex.Message);
                                        strLog.AppendLine("\t" + firstElement.Category.Name + ": " + (rvtUIDoc.Document.GetElement(firstElement.GetTypeId()) as ElementType)?.FamilyName + ": " + firstElement.Name + " [ID: " + firstElement.Id.ToString() + "]");
                                        strLog.AppendLine("\t" + secondElement.Category.Name + ": " + (rvtUIDoc.Document.GetElement(secondElement.GetTypeId()) as ElementType)?.FamilyName + ": " + secondElement.Name + " [ID: " + secondElement.Id.ToString() + "]");
                                        strLog.AppendLine("-----------------------");
                                        subtr.Dispose();
                                        continue;
                                    }
                                    subtr.Commit();
                                }
                            }
                            else
                            {
                                if (cmpElements.IsOverlappedBySolid(_rvtUIApp.Application, ref m_DicSolidsElement, firstElement, secondElement))
                                    listJoinedId.Add(secondElement.Id);
                            }
                        }
                    }
                }

                List<ElementId> jointElementId = JoinGeometryUtils.GetJoinedElements(rvtUIDoc.Document, firstElement).ToList();
                jointElementId.RemoveAll(i => listJoinedId.Contains(i));
                jointElementId = (jointElementId.Where(item => listElementIdFind.Select(item2 => item2).Contains(item))).ToList();

                if (jointElementId != null && jointElementId.Count > 0)
                {
                    foreach (ElementId eleId in jointElementId)
                    {
                        Element secondElement = rvtUIDoc.Document.GetElement(eleId);
                        if (firstElement.Category == null || secondElement.Category == null) continue;

                        if (isSameCategory)
                        {
                            if (secondElement.Category.Name != firstElement.Category.Name) continue;
                        }
                        else
                        {
                            if (secondElement.Category.Name == firstElement.Category.Name) continue;
                        }

                        if (JoinGeometryUtils.AreElementsJoined(rvtUIDoc.Document, firstElement, secondElement))
                        {
                            if (!JoinGeometryUtils.IsCuttingElementInJoin(rvtUIDoc.Document, firstElement, secondElement))
                            {
                                SubTransaction subtr = new SubTransaction(rvtUIDoc.Document);
                                subtr.Start();
                                try
                                {
                                    JoinGeometryUtils.SwitchJoinOrder(rvtUIDoc.Document, firstElement, secondElement);
                                }
                                catch (Exception ex)
                                {
                                    strLog.AppendLine("-----------------------");
                                    strLog.AppendLine(ex.Message);
                                    strLog.AppendLine("\t" + firstElement.Category.Name + ": " + (rvtUIDoc.Document.GetElement(firstElement.GetTypeId()) as ElementType)?.FamilyName + ": " + firstElement.Name + " [ID: " + firstElement.Id.ToString() + "]");
                                    strLog.AppendLine("\t" + secondElement.Category.Name + ": " + (rvtUIDoc.Document.GetElement(secondElement.GetTypeId()) as ElementType)?.FamilyName + ": " + secondElement.Name + " [ID: " + secondElement.Id.ToString() + "]");
                                    strLog.AppendLine("-----------------------");
                                    subtr.Dispose();
                                    continue;
                                }
                                subtr.Commit();
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
