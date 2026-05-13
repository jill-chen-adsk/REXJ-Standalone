using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.ExcelImageInsert.Utils;

namespace ADSK.JExtRAC.ExcelImageInsert.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CmdExcelImageInsert : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            CultureHelper.InitializeCulture();

            UIApplication rvtUIApp = commandData.Application;
            UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            var cmpAttribute = new Components.Attribute();

            Result retResult = Result.Cancelled;

            if (!ExcelHelper.IsExcelRunning())
            {
                MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_STARTEXCEL"));
                return retResult;
            }

            TransactionGroup transGroup = new TransactionGroup(rvtUIDoc.Document);
            transGroup.Start(cmpAttribute.ResourceText("IDS_TRANSACTION_GROUP"));
            Transaction trans = new Transaction(rvtUIDoc.Document);

            try
            {
                string fileName = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    cmpAttribute.ResourceText("IDS_TXT_IMAGEFILENAME"));

                string errorMsg = ExcelHelper.CaptureSelectionToFile(cmpAttribute, fileName);
                if (errorMsg != null)
                {
                    MessageBox.Show(errorMsg);
                    transGroup.Assimilate();
                    return retResult;
                }

                Autodesk.Revit.DB.View view = rvtUIDoc.Document.ActiveView;

                trans.Start("ImportImageFile");

                ImageTypeOptions op = new ImageTypeOptions(fileName, false, ImageTypeSource.Import);
                ImageType imgType = ImageType.Create(rvtUIDoc.Document, op);
                ImagePlacementOptions imgOp = new ImagePlacementOptions();
                imgOp.PlacementPoint = BoxPlacement.Center;

                ImageInstance imageInstance = ImageInstance.Create(rvtUIDoc.Document, view, imgType.Id, imgOp);

                if (imageInstance == null)
                {
                    trans.RollBack();
                    MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_IMPIMAGEFILE"));
                    transGroup.Assimilate();
                    return retResult;
                }

                trans.Commit();
                retResult = Result.Succeeded;
            }
            catch (Exception)
            {
                if (trans.HasStarted())
                    trans.RollBack();
                MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_COMMAND"));
            }

            transGroup.Assimilate();
            return retResult;
        }
    }
}
