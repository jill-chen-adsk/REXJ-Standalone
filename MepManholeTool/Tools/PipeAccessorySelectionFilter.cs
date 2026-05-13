using Autodesk.Revit.DB;
using Revit = Autodesk.Revit;

namespace MepManholeTool.Tools
{
    /// <summary>
    /// 合流桝選択フィルター
    /// </summary>
    public class PipeAccessorySelectionFilter : Revit.UI.Selection.ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
                return element.Category != null && element.Category.Id.Value == (int)BuiltInCategory.OST_PipeAccessory;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}