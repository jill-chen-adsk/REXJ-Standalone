using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using JExtComCompat;
namespace RSTExtension.Components
{
    /// ================================================================================
    /// <summary>設定</summary>
    /// ================================================================================
    public class ESM_Settings : RvtSettings
    {
        // メンバ変数

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="rvtUIDoc">Revit UIドキュメント</p></param>
        ///
        /// <history>2011/12/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public ESM_Settings(UIDocument rvtUIDoc) : base(rvtUIDoc)
        {
        }

        #endregion Constructor

        // メンバ関数

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>カテゴリ- ビュー</summary>
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        Category CategoryView
        {
            get
            {
                return GetCategory(BuiltInCategory.OST_Views);
            }
        }

        #endregion Properties
    }
}
