using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSK.JExtRAC.GridDimension.Entities
{
    internal class ViewItem
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>Tag view</summary>
        public Autodesk.Revit.DB.View Tag;

        /// <summary>Text show in view</summary>
        public string Text;

        #endregion Memeber Variables

        /// ================================================================================
        /// <summary>Override tostring</summary>
        ///
        /// <history>2018/11/12 Created Applied Technology</history>
        /// ================================================================================
        public override string ToString() { return Text; }
    }
}