using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSK.JExtRAC.ValueCopy.Entities
{
    /// ================================================================================
    /// <summary>Class ObjectIndexGroup</summary>
    /// ================================================================================
    internal class ObjectIndexGroup
    {
        // Member Variables

        #region Member Variables

        /// <summary>Name of parameter group</summary>
        public string ParameterGroupName { get; set; }

        /// <summary>Name of parameter</summary>
        public int IndexOnDatagridview { get; set; }

        #endregion Member Variables
    }
}