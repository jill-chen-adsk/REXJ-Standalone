using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitMEPAddin.Common;
using System;

namespace RevitMEPAddin.Filters.SelectionFilters
{
    ///// <summary>
    ///// ダクト選択フィルタクラス
    ///// </summary>
    public class DuctPipeGridSelectionFilter : ISelectionFilter
    {
        private UIDocument uidoc;
        private Logger log;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="doc"></param>
        public DuctPipeGridSelectionFilter(UIDocument uidoc, Logger log )
        {
            this.uidoc = uidoc;
            this.log = log;
        }

        public bool AllowElement(Element element)
        {
            Document doc = uidoc.Document;
            WrpViews _view = new WrpViews(uidoc, log);
            if (!_view.IsViewPlan())
            {
                // 平面図ビューまたは、伏図ビューからは選択させない
                return false;
            }
            if (element is Pipe || element is Duct)
            {
                // ダクトルートは選択させない
                if (element.Category.Id.Equals(Category.GetCategory(doc, BuiltInCategory.OST_PlaceHolderDucts).Id)) return false;
                // パイプルートは選択させない
                if (element.Category.Id.Equals(Category.GetCategory(doc,BuiltInCategory.OST_PlaceHolderPipes).Id)) return false;

                XYZ viewDir = doc.ActiveView.ViewDirection;
                LocationCurve lCurve = element.Location as LocationCurve;
                Line line = lCurve.Curve as Line;
                if (line != null)
                {
                    WrpGeometry _geometry = new WrpGeometry(uidoc, log);
                    // 現在のビューでの勾配の有無チェック結果を返す
                    return _geometry.NearlyEquals(line.Direction.AngleTo(viewDir), Math.PI / 2);
                }
            }else if(element is Grid)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            Document doc = uidoc.Document;
            Duct duct = doc.GetElement(refer) as Duct;
            return duct != null;
        }
    }
}
