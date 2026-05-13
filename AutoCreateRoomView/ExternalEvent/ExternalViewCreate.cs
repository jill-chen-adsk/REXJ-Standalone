using ADSK.JExtRAC.AutoCreateRoomView.Commands;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace ADSK.JExtRAC.AutoCreateRoomView
{
    public class ExternalViewCreate : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument UiDoc = app.ActiveUIDocument;
            Document Doc = UiDoc.Document;
            using (Transaction tran = new Transaction(Doc, "Room View Creation"))
            {
                tran.Start();
                CmdAutoCreateRoomView.afp.View_Create();
                tran.Commit();
            }
            foreach (View view in CmdAutoCreateRoomView.afp.createViews)
            {
                UiDoc.ActiveView = view;
            }
            UiDoc.ActiveView = CmdAutoCreateRoomView.afp.preView;
            List<UIView> closeList = new List<UIView>();
            foreach (UIView uIView in UiDoc.GetOpenUIViews())
            {
                foreach (View view in CmdAutoCreateRoomView.afp.createViews)
                {
                    if (uIView.ViewId == view.Id)
                    {
                        uIView.ZoomToFit();
                        closeList.Add(uIView);
                    }
                }
            }
        }

        public string GetName()
        {
            return "External Event";
        }
    }
}
