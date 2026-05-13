using ADSK.JExtRAC.AutoCreateRoomView;
using ADSK.JExtRAC.AutoCreateRoomView.Screen;
using ADSK.JExtRAC.AutoCreateRoomView.Utils;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AutoCreateRoomView.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CmdAutoCreateRoomView : IExternalCommand
    {
        public static FormAutoCreateRoomView afp;

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            CultureHelper.InitializeCulture();

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            ExternalViewCreate handler = new ExternalViewCreate();
            Autodesk.Revit.UI.ExternalEvent exEvent = Autodesk.Revit.UI.ExternalEvent.Create(handler);

            using (TransactionGroup transGroup = new TransactionGroup(doc, "Create Room Views"))
            {
                transGroup.Start("Room View Creation");
                System.Windows.Forms.NativeWindow nativeWindow = System.Windows.Forms.NativeWindow.FromHandle(uiapp.MainWindowHandle);
                FormAutoCreateRoomView form = new FormAutoCreateRoomView(commandData, exEvent, handler);
                afp = form;
                form.Show(nativeWindow);
                return Result.Succeeded;
            }
        }
    }
}
