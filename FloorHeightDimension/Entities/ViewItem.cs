using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSK.JExtRAC.FloorHeightDimension.Entities
{
    internal class ViewItem
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>tag view</summary>
        public Autodesk.Revit.DB.ViewSection Tag;

        /// <summary>text show in view</summary>
        public string Text;

        #endregion Memeber Variables

        /// ================================================================================
        /// <summary>override tostring</summary>
        ///
        /// <history>2018/11/12 Created Applied Technology</history>
        /// ================================================================================
        public override string ToString() { return Text; }
    }
}