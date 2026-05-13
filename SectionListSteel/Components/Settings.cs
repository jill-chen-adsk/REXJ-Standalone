using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Collections.Generic;

namespace SectionListSteel.Components
{
    /// ================================================================================
    /// <summary>設定</summary>
    /// ================================================================================
    public class Settings : SectionListSteel.JExtComCompat.RvtSettings
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private SectionListSteel.Components.Attribute _CmpAttribute;

        #endregion Member Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="rvtUIDoc"    >Revit UI ドキュメント</param>
        ///
        /// <history>2016/08/05 Created Ryo Kuroda</history>
        /// ================================================================================
        public
        Settings(SectionListSteel.Components.Attribute cmpAttribute,
                    Revit.UI.UIDocument rvtUIDoc) :
          base(rvtUIDoc)
        {
            _CmpAttribute = cmpAttribute;
        }

        #endregion Constructor

        // プロパティ
        #region Properties

        /// <summary>カテゴリ - プロジェクト情報</summary>
        public Revit.DB.Category CategoryProjInfo
        {
            get { return base.GetCategory(Revit.DB.BuiltInCategory.OST_ProjectInformation); }
        }

        /// <summary>カテゴリ - 柱</summary>
        public Collections.Generic.IList<Revit.DB.Category> CategoryColumn
        {
            get
            {
                var ret = new Collections.Generic.List<Revit.DB.Category>
                {
                    base.GetCategory(Revit.DB.BuiltInCategory.OST_Columns),
                    base.GetCategory(Revit.DB.BuiltInCategory.OST_StructuralColumns),
                };
                return ret;
            }
        }

        /// <summary>カテゴリ - 梁</summary>
        public Collections.Generic.IList<Revit.DB.Category> CategoryBeam
        {
            get
            {
                var ret = new Collections.Generic.List<Revit.DB.Category>
                {
                    base.GetCategory(Revit.DB.BuiltInCategory.OST_StructuralFraming),
                };
                return ret;
            }
        }

        #endregion Properties
    }
}