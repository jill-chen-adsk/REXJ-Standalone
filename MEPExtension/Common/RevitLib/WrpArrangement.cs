using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Mechanical;
using Common.Constant;

namespace RevitMEPAddin.Common
{
    public class WrpArrangement
    {
        // メンバ変数
        #region Memeber Variables
        private UIDocument uidoc;
        private Document doc;
        private Logger log;
        #endregion

        // コンストラクタ
        #region Constructor
        public WrpArrangement(UIDocument uidoc, Logger log)
        {
            this.uidoc = uidoc;
            doc = uidoc.Document;
            this.log = log;
        }
        #endregion

        // メンバ関数
        #region Member Functions

        /// <summary>
        /// ファミリシンボルのロード
        /// </summary>
        /// <param name="symbol">ファミリシンボル(タイプ)</param>
        /// <param name="filePath">ファミリファイルのパス</param>
        /// <param name="typeName">タイプ名称</param>
        /// <returns></returns>
        public bool LoadFamilySymbol(ref FamilySymbol symbol, string filePath, string typeName)
        {
            //using (Transaction tx = new Transaction(doc))
            //{
            //tx.Start("Load");
            // try to load familysymbol
            log.Info("filePath:" + filePath);
            log.Info("typeName:" + typeName);
            OverwriteOKFamilyLoadOpt opt = new OverwriteOKFamilyLoadOpt();
            if (!doc.LoadFamilySymbol(filePath, typeName, opt, out symbol))
            {
                //tx.RollBack();
                log.Error("S管ファミリのload失敗！");
                return false;
            }
            if (!symbol.IsActive)
            {
                symbol.Activate();
            }
            //    tx.Commit();
            //}
            return true;
        }


        /// <summary>
        /// ファミリのロード
        /// </summary>
        /// <param name="family">ファミリ</param>
        /// <param name="symbols">ファミリが持っているタイプのリスト</param>
        /// <param name="filePath">ファミリファイルのパス</param>
        /// <returns></returns>
        public bool LoadFamily(ref Family family, ref List<FamilySymbol> symbols , string filePath)
        {
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Load");
                // try to load family
                Family tFamily = null;
                if (!doc.LoadFamily(filePath, out tFamily))
                {
                    tx.RollBack();
                    return false;
                }
                family = tFamily;
                ISet<ElementId> symbolIds = family.GetFamilySymbolIds();
                foreach(ElementId id in symbolIds)
                {
                    FamilySymbol s = doc.GetElement(id) as FamilySymbol;
                    if (!s.IsActive)
                    {
                        s.Activate();
                        symbols.Add(s);
                    }
                }
                tx.Commit();
            }
                return true;
        }

        /// <summary>
        /// FamilySymbol取得
        /// </summary>
        /// <param name="familyName">ファミリ名称</param>
        /// <param name="typeName">タイプ名称</param>
        /// <param name="category">ファミリのビルトインカテゴリ</param>
        /// <returns></returns>
        public FamilySymbol GetFamilySymbol(string familyName, string typeName, BuiltInCategory category)
        {
            FilteredElementCollector collector
            = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol))
                .OfCategory(category);

            ICollection<Element> symbols
              = collector.ToElements();
            FamilySymbol symbol = null;
            foreach (FamilySymbol e in symbols)
            {
                if (e.Name.Equals(typeName)
                     && e.get_Parameter(
                            BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM)
                                .AsString().Equals(familyName))
                {
                    symbol = e;
                    break;
                }
            }
            return symbol;
        }

        #endregion

        // プロパティ
        #region Properties
        #endregion
    }

    /// <summary>
    /// 【ファミリロードに関するオプション】
    /// 上書き可能の設定(まったく同じファミリならロードされていてもOnFamilyFoundは通ってなさそう。)
    /// </summary>
    class OverwriteOKFamilyLoadOpt : IFamilyLoadOptions
    {
        bool IFamilyLoadOptions.OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;
            //if (familyInUse)
            //{
                // ロードするか確認メッセージ
                TaskDialogResult res = TaskDialog.Show(CommonDefine.DIALOG_TITLE_CONFIRM , CommonDefine.DIALOG_MSG_CONFIRM1, TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No);
                if (TaskDialogResult.No == res)
                {
                    overwriteParameterValues = false;
                    return false;
                }
            //}
              
            return true;
        }

        bool IFamilyLoadOptions.OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            throw new NotImplementedException();
        }
    }
}
