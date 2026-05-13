using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FloorHeightDimension;
using System.Linq;

namespace ADSK.JExtRAC.FloorHeightDimension.Commands
{
    [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdFloorHeightDimension : Revit.UI.IExternalCommand
    {
        public Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                                ref string message,
                                Revit.DB.ElementSet elements)
        {
            Revit.UI.UIApplication rvtUIApp = commandData.Application;
            Revit.UI.UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            RvtExtApp.Components.Attribute cmpAttribute = new RvtExtApp.Components.Attribute();
            RvtExtApp.Components.Elements cmpElements = new RvtExtApp.Components.Elements(rvtUIDoc);
            RvtExtApp.Components.Geometry cmpGeometry = new RvtExtApp.Components.Geometry(rvtUIDoc);
            RvtExtApp.Components.Parameters cmpParameters = new RvtExtApp.Components.Parameters(cmpAttribute, rvtUIDoc);
            RvtExtApp.Components.Settings cmpSettings = new RvtExtApp.Components.Settings(rvtUIDoc);
            RvtExtApp.Components.Service cmpService = new RvtExtApp.Components.Service(cmpAttribute,
                                                                                             cmpElements,
                                                                                             cmpGeometry,
                                                                                             cmpParameters,
                                                                                             cmpSettings);

            Revit.UI.Result retExtCom = Revit.UI.Result.Cancelled;

            System.Windows.Forms.DialogResult retDlg;

            Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(cmpElements.RvtDBDoc);
            transGroup.Start(cmpAttribute.ResourceText("IDS_TXT_FLOORHEIGHTDIMENSION"));
            Revit.DB.Transaction trans = new Autodesk.Revit.DB.Transaction(cmpElements.RvtDBDoc);

            try
            {
                Revit.DB.ViewSection viewSection = cmpElements.ActiveViewSection;
                if (viewSection == null)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_OPENVIEWSECTION"));
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                Collections.Generic.IList<Revit.DB.Element> elemLevels = cmpElements.SelSetLevels;
                if (elemLevels.Count == 0)
                {
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOLEVELSELECT"), cmpAttribute.ResourceText("IDS_TXT_ERROR"), System.Windows.Forms.MessageBoxButtons.OK);
                    return retExtCom;
                }

                Revit.DB.FilteredElementCollector dimesionTypeCollector = new Revit.DB.FilteredElementCollector(rvtUIDoc.Document);
                dimesionTypeCollector.OfClass(typeof(Revit.DB.DimensionType));
                Collections.Generic.IList<Revit.DB.DimensionType> list_dimesionType = dimesionTypeCollector.Cast<Revit.DB.DimensionType>().ToList()
                                                                                        .Where(x => x.StyleType == Revit.DB.DimensionStyleType.Linear)
                                                                                        .Where(x => x.GetSimilarTypes().Count != 0).ToList();

                if (list_dimesionType.Count == 0)
                    return retExtCom;

                Revit.DB.ProjectInfo elemProjInfo = cmpElements.ProjectInfo;

                trans.Start("SetCommand");
                RvtExtApp.Entities.DtCmd entDtCmd = new RvtExtApp.Entities.DtCmd(cmpAttribute,
                                                                                 cmpElements,
                                                                                 cmpGeometry,
                                                                                 cmpParameters,
                                                                                 cmpSettings,
                                                                                 elemProjInfo,
                                                                                 cmpAttribute.ResourceText("IDS_SHPARAM_DEF_CMD_FLOORHEIGHTDIMENSION"),
                                                                                 4);
                if (entDtCmd.ErrMsg != "")
                {
                    trans.RollBack();
                    System.Windows.Forms.MessageBox.Show(entDtCmd.ErrMsg);
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }
                trans.Commit();

                Revit.DB.XYZ locPos = null;

                RvtExtApp.UI.FormConfig form = new RvtExtApp.UI.FormConfig(cmpAttribute, entDtCmd, list_dimesionType);

                retDlg = System.Windows.Forms.DialogResult.Yes;

                while (retDlg == System.Windows.Forms.DialogResult.Yes)
                {
                    retDlg = form.ShowDialog();
                    form.StartPosition = System.Windows.Forms.FormStartPosition.Manual;

                    if (retDlg == System.Windows.Forms.DialogResult.Yes)
                    {
                        trans.Start("SelPos");
                        Revit.DB.XYZ selPos = cmpService.SelPos(viewSection);
                        if (selPos != null)
                        {
                            locPos = selPos;
                        }
                        trans.Commit();
                    }
                }

                if (form.DialogResult != System.Windows.Forms.DialogResult.OK)
                {
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                Collections.Generic.List<Revit.DB.ViewSection> viewList = new Collections.Generic.List<Revit.DB.ViewSection>();
                Collections.Generic.Dictionary<string, System.Collections.Generic.List<Autodesk.Revit.DB.ViewSection>> dic_SortView = new Collections.Generic.Dictionary<string, System.Collections.Generic.List<Autodesk.Revit.DB.ViewSection>>();
                if (bool.Parse(entDtCmd.Data[2]))
                {
                    System.Collections.Generic.ICollection<Revit.DB.Element> elementList = new Revit.DB.FilteredElementCollector(rvtUIDoc.Document).OfCategory(Revit.DB.BuiltInCategory.OST_Views).ToElements();
                    foreach (Revit.DB.View view in elementList)
                    {
                        if (view.IsTemplate)
                            continue;

                        if (view is Revit.DB.ViewSection)
                        {
                            Revit.DB.ViewSection mView = view as Revit.DB.ViewSection;
                            if (mView == null)
                                continue;

                            if (cmpGeometry.IsSameDirection(mView.ViewDirection, viewSection.ViewDirection))
                            {
                                if (dic_SortView.ContainsKey(mView.ViewType.ToString()))
                                {
                                    dic_SortView[mView.ViewType.ToString()].Add(mView);
                                }
                                else
                                {
                                    Collections.Generic.List<Revit.DB.ViewSection> viewListSort = new Collections.Generic.List<Revit.DB.ViewSection>();
                                    viewListSort.Add(mView);
                                    dic_SortView.Add(mView.ViewType.ToString(), viewListSort);
                                }
                            }
                        }
                    }
                    foreach (var item in dic_SortView)
                    {
                        var list = item.Value;

                        list.Sort(delegate (Revit.DB.ViewSection v1, Revit.DB.ViewSection v2)
                        {
                            return v1.Title.CompareTo(v2.Title);
                        });

                        viewList.AddRange(list);
                    }

                    if (viewList.Count != 0)
                    {
                        RvtExtApp.UI.FormSelectView frmSelectView = new RvtExtApp.UI.FormSelectView(cmpAttribute, entDtCmd, viewList, viewSection);

                        frmSelectView.ShowDialog();
                        if (frmSelectView.DialogResult != System.Windows.Forms.DialogResult.OK)
                        {
                            cmpParameters.SetSharedParamDefault();
                            transGroup.Assimilate();
                            return retExtCom;
                        }
                    }
                }
                else
                {
                    if (rvtUIDoc.Document.ActiveView is Revit.DB.ViewSection)
                        viewList.Add(rvtUIDoc.Document.ActiveView as Revit.DB.ViewSection);
                }

                trans.Start("CreateDimension");
                foreach (Revit.DB.ViewSection view in viewList)
                {
                    Revit.DB.SubTransaction subtr = new Revit.DB.SubTransaction(rvtUIDoc.Document);
                    subtr.Start();

                    try
                    {
                        cmpService.CreateDimension(view, elemLevels, locPos, entDtCmd.Data[0], entDtCmd.Data[1], form.GetSelectDimensionType);
                        subtr.Commit();
                    }
                    catch
                    {
                        subtr.Dispose();
                        continue;
                    }
                    entDtCmd.SetData();
                }
                trans.Commit();

                retExtCom = Revit.UI.Result.Succeeded;
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message;
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_COMMAND"));
            }

            cmpParameters.SetSharedParamDefault();
            transGroup.Assimilate();
            return retExtCom;
        }
    }
}
