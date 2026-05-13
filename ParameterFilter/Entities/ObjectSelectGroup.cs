using Autodesk.Revit.DB ;

namespace ADSK.JExtRAC.ParameterFilter.Entities
{
    public class ObjectSelectGroup
    {
        #region Member Variables

        /// <summary>Parameter group name</summary>
        public string ParameterGroupVal { get; set; }

        /// <summary>Id of group parameter</summary>
        public ForgeTypeId GroupTypeId { get; set; }

        /// <summary>Group is selected or not</summary>
        public bool IsSelected { get; set; }

        #endregion Member Variables
    }
}