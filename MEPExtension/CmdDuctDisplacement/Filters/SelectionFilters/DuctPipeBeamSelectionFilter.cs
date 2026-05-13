using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitMEPAddin.Common;

namespace CmdDuctDisplacement.Filters.SelectionFilters
{
    /// <summary>
    /// ダクト・パイプ・梁 選択フィルタクラス
    /// </summary>
    class DuctPipeBeamSelectionFilter : ISelectionFilter
    {
        private UIDocument uidoc;
        private Document doc;
        private Logger log;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="doc"></param>
        public DuctPipeBeamSelectionFilter(UIDocument uidoc, Logger log)
        {
            this.uidoc = uidoc;
            this.doc = uidoc.Document;
            this.log = log;
        }

        public bool AllowElement(Element element)
        {
            if (element is RevitLinkInstance || element is Pipe || element is Duct || element.Category.Id.ToString() == ((int)BuiltInCategory.OST_StructuralFraming).ToString())
            {
                // ダクトルートは選択させない
                if (element.Category.Id.Equals(Category.GetCategory(doc, BuiltInCategory.OST_PlaceHolderDucts).Id)) return false;
                // パイプルートは選択させない
                if (element.Category.Id.Equals(Category.GetCategory(doc, BuiltInCategory.OST_PlaceHolderPipes).Id)) return false;

                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {   
            RevitLinkInstance pElm = doc.GetElement(refer) as RevitLinkInstance;
            if (pElm == null) return true;
            Document pDoc = pElm.GetLinkDocument();
            Element elm = pDoc.GetElement(refer.LinkedElementId);
            if(!(elm is DirectShape || elm is Duct || elm is Pipe || elm.Category.Id.ToString() == ((int)BuiltInCategory.OST_StructuralFraming).ToString()))
            {
                return false;
            }
            // ダクトルートは選択させない
            if (elm.Category.Id.Equals(Category.GetCategory(doc, BuiltInCategory.OST_PlaceHolderDucts).Id)) return false;
            // パイプルートは選択させない
            if (elm.Category.Id.Equals(Category.GetCategory(doc, BuiltInCategory.OST_PlaceHolderPipes).Id)) return false;

            return true;

        }
    }
}

