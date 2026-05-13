using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
namespace SectionListSteel.Components
{
    /// ================================================================================
    /// <summary>属性</summary>
    /// ================================================================================
    public class Attribute : SectionListSteel.JExtComCompat.UtilAttrib
    {
        // コンストラクタ

        #region Member Constructor

        /// ================================================================================
        /// <summary>アセンブリ設定</summary>
        ///
        /// <history><p>2016/08/05 Created  GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public Attribute() : base()
        {
            // 属性
            base.SetAssembly(System.Reflection.Assembly.GetExecutingAssembly(),
                             "SectionListSteel.Resources.Text",
                             "SectionListSteel.Resources.Image",
                             System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
        }

        #endregion Member Constructor
    }
}