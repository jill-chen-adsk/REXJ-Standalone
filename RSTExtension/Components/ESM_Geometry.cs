using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;


using Autodesk.Revit.UI;

namespace RSTExtension.Components
{
    /// ================================================================================
    /// <summary>図形</summary>
    /// ================================================================================
    public class ESM_Geometry : RvtGeometry
    {
        // メンバ変数

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="rvtUIDoc">Revit UIドキュメント</param>
        ///
        /// <history>2011/12/01 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public ESM_Geometry(UIDocument rvtUIDoc) : base(rvtUIDoc)
        {
        }

        #endregion Constructor

        // メンバ関数

        // プロパティ
    }
}
