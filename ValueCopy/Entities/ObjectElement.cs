using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSK.JExtRAC.ValueCopy.Entities
{
    /// ================================================================================
    /// <summary>Class ObjectElement</summary>
    /// ================================================================================
    public class ObjectElement
    {
        // Member Variables

        #region Member Variables

        /// <summary>Element</summary>
        public Element ElementCurrent { get; set; }

        /// <summary>List of length parameter</summary>
        public List<ObjectParameter> ObjectParameterData { get; set; }

        #endregion Member Variables

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="elementCurrent">Element</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public ObjectElement(Element elementCurrent)
        {
            ElementCurrent = elementCurrent;
        }

        #endregion Constructor

        // Member Functions

        #region Member Functions

        /// ================================================================================
        /// <summary>Get parameter group and length of parameter</summary>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public void GetParameterAndGroupParamter()
        {
            if (ElementCurrent == null)
                return;

            ObjectParameterData = new List<ObjectParameter>();

            foreach (Parameter pr in ElementCurrent.ParametersMap)
            {
                if (pr == null || pr.Definition == null)
                    continue;

                ObjectParameter obj = new ObjectParameter();
                obj.NameParameter = pr.Definition.Name;

                //if (pr.StorageType == StorageType.ElementId)
                //{
                //    var currentElement = doc.GetElement(pr.AsElementId()) as Element;
                //    if (currentElement != null)
                //        obj.ParameterValue = currentElement.Name;
                //}
                //else
                obj.ParameterValue = pr.AsValueString();

                obj.CurrentParameter = pr;
                obj.ElementIdGroup = pr.Definition.GetGroupTypeId();
                obj.ParameterGroupName = LabelUtils.GetLabelForGroup( pr.Definition.GetGroupTypeId() ) ;

                ObjectParameterData.Add(obj);
            }
        }

        #endregion Member Functions
    }
}