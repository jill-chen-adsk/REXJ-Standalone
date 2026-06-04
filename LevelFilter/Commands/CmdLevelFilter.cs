using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.LevelFilter.Utils;
using ADSK.JExtRAC.LevelFilter.UI;
using RvtExtApp = ADSK.JExtRAC.LevelFilter;

namespace ADSK.JExtRAC.LevelFilter.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CmdLevelFilter : IExternalCommand
    {
        public static UIApplication _RvtUIApp;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            CultureHelper.InitializeCulture();

            UIApplication rvtUIApp = commandData.Application;
            UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            Document rvtDbDoc = rvtUIDoc.Document;

            var cmpAttribute = new RvtExtApp.Components.Attribute();
            var cmpElements = new RvtExtApp.Components.Elements(rvtUIDoc);
            var cmpParameters = new RvtExtApp.Components.Parameters(rvtUIDoc);
            var cmpSettings = new RvtExtApp.Components.Settings(rvtUIDoc);
            var cmpService = new RvtExtApp.Components.Service(cmpAttribute, cmpElements, cmpParameters, cmpSettings);

            Result result = Result.Cancelled;
            IList<Element> selElems = cmpElements.SelElems;

            if (selElems.Count == 1 && selElems[0] is Group)
            {
                Group group = selElems[0] as Group;
                selElems.Clear();
                foreach (ElementId memberId in group.GetMemberIds())
                {
                    Element element = rvtUIDoc.Document.GetElement(memberId);
                    selElems.Add(element);
                }
            }

            IList<Element> elemSet = new List<Element>();
            IList<Part> partList = new List<Part>();
            IList<Material> materialSet = new List<Material>();

            if (selElems.Count == 0)
            {
                MessageWindow.Show(
                    cmpAttribute.ResourceText("IDS_ERR_ERROR"),
                    cmpAttribute.ResourceText("IDS_ERR_NOELEMENTSELECT"));
                return result;
            }

            foreach (Element element in selElems)
            {
                elemSet.Add(element);
                try
                {
                    Part part = element as Part;
                    if (part != null)
                        partList.Add(part);
                }
                catch { }
            }

            if (partList.Count > 0)
                materialSet = cmpElements.PartsMaterials(partList);

            List<ParameterFilterElement> lstRuleFilter = cmpElements.GetRuleFilterElements(rvtUIDoc.Document);

            string filterTabNum = "0";

            Dictionary<ElementId, IList<ElementId>> dicCat = new Dictionary<ElementId, IList<ElementId>>();
            Dictionary<string, IList<ElementId>> dicFam = new Dictionary<string, IList<ElementId>>();
            Dictionary<string, IList<ElementId>> dicFamType = new Dictionary<string, IList<ElementId>>();
            Dictionary<ElementId, IList<ElementId>> dicPart = new Dictionary<ElementId, IList<ElementId>>();
            Dictionary<ElementId, IList<ElementId>> dicFilter = new Dictionary<ElementId, IList<ElementId>>();

            cmpService.GetFormData(rvtUIDoc.Document, lstRuleFilter, elemSet, materialSet, ref dicCat, ref dicFam, ref dicFamType, ref dicPart, ref dicFilter);

            var filterWindow = new LevelFilterWindow(rvtUIDoc, cmpAttribute, elemSet, partList, dicCat, dicFam, dicFamType, dicPart, dicFilter, selElems, filterTabNum);
            filterWindow.ShowDialog();

            result = Result.Succeeded;
            return result;
        }
    }
}
