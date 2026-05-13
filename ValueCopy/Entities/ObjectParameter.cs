using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSK.JExtRAC.ValueCopy.Entities
{
    /// ================================================================================
    /// <summary>Status copy</summary>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    public enum StatusCopy
    {
        CS_Null,
        CS_Success,
        CS_CanFindParameter,
        CS_OutOfRange,
        CS_ReadOnlyOrRecipe,
        CS_CantCopy
    };

    /// ================================================================================
    /// <summary>Class Object Parameter</summary>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    public class ObjectParameter
    {
        #region Member Variables

        /// <summary>Is copy or not</summary>
        public bool IsCopy { get; set; }

        /// <summary>Current parameter</summary>
        public Parameter CurrentParameter { get; set; }

        /// <summary>Name of parameter</summary>
        public string NameParameter { get; set; }

        /// <summary>Value of parameter</summary>
        public string ParameterValue { get; set; }

        /// <summary>Id of group parameter</summary>
        public ForgeTypeId ElementIdGroup { get; set; }

        /// <summary>Parameter group name</summary>
        public string ParameterGroupName { get; set; }

        /// <summary>Status copy parameter</summary>
        public StatusCopy StatusCopyParameter { get; set; }

        #endregion Member Variables
    }
}