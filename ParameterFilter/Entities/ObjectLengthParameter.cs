using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.ParameterFilter.Entities
{
    public class ObjectLengthParameter
    {
        #region Member Variables

        /// <summary>Element</summary>
        public Element ElementCurrent { get; set; }

        /// <summary>Name of parameter</summary>
        public string NameParameterLength { get; set; }

        /// <summary>Value of parameter</summary>
        public double LengthVal { get; set; }

        /// <summary>User input value</summary>
        public object prValueDgv { get; set; }

        /// <summary>User input min value</summary>
        public object prMinDgv { get; set; }

        /// <summary>User input max value</summary>
        public object prMaxDgv { get; set; }

        /// <summary>Group data</summary>
        public ObjectSelectGroup ObjectGroupVal { get; set; }

        #endregion Member Variables
    }
}