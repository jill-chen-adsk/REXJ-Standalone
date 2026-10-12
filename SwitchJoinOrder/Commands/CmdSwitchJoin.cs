using ADSK.JExtRAC.SwitchJoinOrder.UI;
using ADSK.JExtRAC.SwitchJoinOrder.Entities;
using ADSK.JExtRAC.SwitchJoinOrder.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
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
            try
            {
            CultureHelper.InitializeCulture();

            _rvtUIApp = commandData.Application;
            UIDocument rvtUIDoc = _rvtUIApp.ActiveUIDocument;
            RvtExtApp.Components.Attribute cmpAttribute = new RvtExtApp.Components.Attribute();
            RvtExtApp.Components.Elements cmpElements = new RvtExtApp.Components.Elements(rvtUIDoc);
            IntPtr ownerHandle = _rvtUIApp.MainWindowHandle;

            Result retExtCom = Result.Succeeded;

            List<Element> listElement = cmpElements.getAllElement(rvtUIDoc, rvtUIDoc.Document);

            if (listElement == null || listElement.Count == 0)
            {
                WeaveDialogHost.ShowMessage(
                    ownerHandle,
                    cmpAttribute.ResourceText("IDS_TXT_INFORMATION"),
                    cmpAttribute.ResourceText("IDS_TXT_ERROR"),
                    cmpAttribute.ResourceText("IDS_TXT_OK"));
                return Result.Failed;
            }

            CategoryItems groupCategory = cmpElements.GroupingData(rvtUIDoc.Document, listElement);

            var form = new FormSwitchJoinWPF(rvtUIDoc.Document, cmpAttribute, groupCategory);
            if (WeaveDialogHost.ShowDialog(form, ownerHandle) != true)
                return Result.Cancelled;

            Transaction tr = null;
            try
            {
                bool processOnlyJoined = form.GetChecked;
                bool isJoinGeometry = !processOnlyJoined;
                var refusedPairs = new List<ElementId[]>();
                m_DicSolidsElement.Clear();

                int numProgress = CountProcessingSteps(rvtUIDoc.Document, groupCategory, processOnlyJoined);
                int cntProgress = 0;

                using ProgressDialog progressDialog = new ProgressDialog(cmpAttribute.ResourceText("IDS_TXT_PROGESSBAR"), numProgress);
                progressDialog.SetOwner(ownerHandle);
                progressDialog.ShowNonModal();

                tr = new Transaction(cmpElements.RvtDBDoc);
                tr.Start("SwitchJoinOder");

                foreach (var cate in groupCategory._categoryShow)
                {
                    if (cate._isJoinFami)
                        SwitchJoinFamily(rvtUIDoc, cmpAttribute, cmpElements, cate, processOnlyJoined, isJoinGeometry, refusedPairs, ref cntProgress, progressDialog);

                    List<ElementId> listElementIdFind = new List<ElementId>();
                    foreach (var ss in groupCategory._categoryShow)
                    {
                        if (cate.index >= ss.index)
                            continue;
                        listElementIdFind.AddRange(ss._listElementId);
                    }
                    if (listElementIdFind.Count == 0)
                        continue;

                    var candidateSet = new HashSet<ElementId>(listElementIdFind);
                    foreach (var fistElementId in cate._listElementId)
                    {
                        var firstElement = rvtUIDoc.Document.GetElement(fistElementId) as Element;
                        if (processOnlyJoined && !HasJoinedPartnerAmong(rvtUIDoc.Document, firstElement, candidateSet))
                            continue;

                        Join(cmpElements, firstElement, listElementIdFind, isJoinGeometry, false, refusedPairs);
                        progressDialog.UpdateProgress(++cntProgress);
                    }
                }

                progressDialog.Close();

                foreach (ElementId[] pair in refusedPairs)
                    JoinFailures.Post(rvtUIDoc.Document, pair[0], pair[1]);

                rvtUIDoc.Document.Regenerate();
                tr.Commit();
            }
            catch (Exception)
            {
                if (tr != null && tr.HasStarted() && !tr.HasEnded())
                    tr.RollBack();
                throw;
            }
            finally
            {
                tr?.Dispose();
            }

            return retExtCom;
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;
            }
        }

        private bool SwitchJoinFamily(UIDocument rvtUIDoc,
            RvtExtApp.Components.Attribute cmpAttribute,
            RvtExtApp.Components.Elements cmpElements, CategoryItem cate, bool processOnlyJoined, bool isJoinGeometry,
            List<ElementId[]> refusedPairs,
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

                    var candidateSet = new HashSet<ElementId>(listElementIdFind);
                    foreach (var fistElementId in fami._listElementIdOfFamily)
                    {
                        var firstElement = rvtUIDoc.Document.GetElement(fistElementId) as Element;
                        if (processOnlyJoined && !HasJoinedPartnerAmong(rvtUIDoc.Document, firstElement, candidateSet))
                            continue;

                        Join(cmpElements, firstElement, listElementIdFind, isJoinGeometry, true, refusedPairs);
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
            List<ElementId[]> refusedPairs)
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
                                    catch (Exception)
                                    {
                                        refusedPairs.Add(new[] { firstElement.Id, secondElement.Id });
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
                                catch (Exception)
                                {
                                    refusedPairs.Add(new[] { firstElement.Id, secondElement.Id });
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

        private static bool HasJoinedPartnerAmong(Document doc, Element element, HashSet<ElementId> candidates)
        {
            if (doc == null || element == null || candidates == null || candidates.Count == 0)
                return false;

            foreach (ElementId joinedId in JoinGeometryUtils.GetJoinedElements(doc, element))
            {
                if (candidates.Contains(joinedId))
                    return true;
            }

            return false;
        }

        private static int CountProcessingSteps(Document doc, CategoryItems groupCategory, bool processOnlyJoined)
        {
            if (groupCategory?._categoryShow == null || groupCategory._categoryShow.Count == 0)
                return 0;

            if (!processOnlyJoined)
                return groupCategory.CountEle();

            int count = 0;
            for (int i = 0; i < groupCategory._categoryShow.Count; i++)
            {
                CategoryItem cate = groupCategory._categoryShow[i];

                if (cate._isJoinFami && cate._listFamilyItem != null)
                {
                    foreach (FamilyItem fami in cate._listFamilyItem)
                    {
                        var lowerPriorityFamilyIds = new HashSet<ElementId>();
                        foreach (FamilyItem ss in cate._listFamilyItem)
                        {
                            if (fami._indexFami >= ss._indexFami)
                                continue;
                            lowerPriorityFamilyIds.UnionWith(ss._listElementIdOfFamily);
                        }

                        if (lowerPriorityFamilyIds.Count == 0 || fami._listElementIdOfFamily == null)
                            continue;

                        foreach (ElementId elementId in fami._listElementIdOfFamily)
                        {
                            Element element = doc.GetElement(elementId);
                            if (element != null && HasJoinedPartnerAmong(doc, element, lowerPriorityFamilyIds))
                                count++;
                        }
                    }
                }

                var lowerPriorityIds = new HashSet<ElementId>();
                foreach (CategoryItem ss in groupCategory._categoryShow)
                {
                    if (cate.index >= ss.index)
                        continue;
                    lowerPriorityIds.UnionWith(ss._listElementId);
                }

                if (lowerPriorityIds.Count == 0 || cate._listElementId == null)
                    continue;

                foreach (ElementId elementId in cate._listElementId)
                {
                    Element element = doc.GetElement(elementId);
                    if (element != null && HasJoinedPartnerAmong(doc, element, lowerPriorityIds))
                        count++;
                }
            }

            return count;
        }
    }
}
