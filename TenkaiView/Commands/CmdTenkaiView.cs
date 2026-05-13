using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ADSK.ViewExtension.TenkaiView.Resources;
using ADSK.ViewExtension.TenkaiView.UI;

namespace ADSK.ViewExtension.TenkaiView.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CmdTenkaiView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document dbDoc = uiDoc.Document;
            Result uiRet = Result.Cancelled;

            DlgTenkaiView dlg1;
            try
            {
                dlg1 = new DlgTenkaiView(commandData);
                if (dlg1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return Result.Cancelled;
            }
            catch (System.Exception ex)
            {
                TaskDialog.Show(Text.TXT_ERR, ex.Message);
                return Result.Failed;
            }

            List<ElementId> lstRoomIds = dlg1.RoomIds;
            if (lstRoomIds.Count == 0)
                return Result.Cancelled;

            AreaVolumeSettings curAvs = AreaVolumeSettings.GetAreaVolumeSettings(dbDoc);
            bool curCompVolume = curAvs.ComputeVolumes;

            List<UIView> openViews = uiDoc.GetOpenUIViews().ToList();

            using (TransactionGroup trgp = new TransactionGroup(dbDoc, Text.TXT_TRANSACTIONNAME))
            {
                if (trgp.Start() == TransactionStatus.Started)
                {
                    try
                    {
                        if (!curAvs.ComputeVolumes)
                        {
                            using (Transaction tr1 = new Transaction(dbDoc, Text.TRANS_SETROOMBOUNDARY))
                            {
                                if (tr1.Start() == TransactionStatus.Started)
                                {
                                    try
                                    {
                                        curAvs.ComputeVolumes = true;
                                        dbDoc.Regenerate();
                                        tr1.Commit();
                                    }
                                    catch (System.Exception)
                                    {
                                        tr1.RollBack();
                                        throw new System.Exception(Text.ERR_CHANGEROOMBOUNDARYFAIL);
                                    }
                                }
                            }
                        }

                        DlgCreateTenkaiProcess dlg2 = new DlgCreateTenkaiProcess(commandData, lstRoomIds, dlg1.TenkaiKondition);
                        if (dlg2.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                            throw new System.Exception(Text.ERR_OPERATIONCANCEL);

                        List<UIView> curntViews = uiDoc.GetOpenUIViews().ToList();
                        foreach (UIView crntView in curntViews)
                        {
                            bool bFind = false;
                            foreach (UIView existView in openViews)
                            {
                                if (crntView.ViewId.Equals(existView.ViewId))
                                {
                                    bFind = true;
                                    break;
                                }
                            }
                            if (!bFind)
                                crntView.Close();
                        }

                        if (!curCompVolume)
                        {
                            using (Transaction tr1 = new Transaction(dbDoc, Text.TRANS_ROLLBACKROOMBOUNDARY))
                            {
                                if (tr1.Start() == TransactionStatus.Started)
                                {
                                    try
                                    {
                                        curAvs.ComputeVolumes = false;
                                        tr1.Commit();
                                    }
                                    catch (System.Exception)
                                    {
                                        tr1.RollBack();
                                        throw new System.Exception(Text.ERR_ROLLBACKFAIL);
                                    }
                                }
                            }
                        }

                        trgp.Assimilate();
                        uiRet = Result.Succeeded;
                    }
                    catch (System.Exception)
                    {
                        trgp.RollBack();
                        uiRet = Result.Failed;
                    }
                }
            }

            return uiRet;
        }
    }
}
