using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitMEPAddin.Common
{
    public class WrpLineStyle
    {
        #region member
        private UIDocument uidoc;
        private Document doc;
        private Logger log;
        #endregion

        #region constractor
        public WrpLineStyle(UIDocument uidoc, Logger log)
        {
            this.uidoc = uidoc;
            doc = this.uidoc.Document;
            this.log = log;
        }
        #endregion

        #region methods
        public bool CreateLineStyle(ref GraphicsStyle graphicStyle, string subCateName, int red, int green, int blue, int weight)
        {
            // 線分カテゴリ取得
            Autodesk.Revit.DB.Categories categories = doc.Settings.Categories;
            Category lineCat = categories.get_Item(BuiltInCategory.OST_Lines);

            // 線分カテゴリに新しいサブカテゴリ追加
            Category newLineStyleCat;
            if (lineCat.SubCategories.Contains(subCateName))
            {
                newLineStyleCat = lineCat.SubCategories.get_Item(subCateName);
            }
            else
            {
                newLineStyleCat = categories.NewSubcategory(lineCat, subCateName);
            }
            // 再作図
            doc.Regenerate();
                
            // LineStyleの中身を設定
            // 太さ設定
            newLineStyleCat.SetLineWeight(weight, GraphicsStyleType.Projection);
            // 色設定
            byte r = byte.Parse(red.ToString());
            byte g = byte.Parse(green.ToString());
            byte b = byte.Parse(blue.ToString());
            newLineStyleCat.LineColor = new Color(r, g, b);
            // 線種：実線設定
            newLineStyleCat.SetLinePatternId(
                LinePatternElement.GetSolidPatternId(),
                GraphicsStyleType.Projection);
                
            // GraphicStyle取得
            graphicStyle = newLineStyleCat.GetGraphicsStyle(GraphicsStyleType.Projection);

            return true;
        }
        #endregion

    }
}
