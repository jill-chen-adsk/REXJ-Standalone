using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSK.JExtRAC.ValueCopy.Entities
{
    /// ================================================================================
    /// <summary>ClassObjectReportCopy</summary>
    /// ================================================================================
    public class ObjectReportCopy
    {
        // Member Variables

        #region Member Variables

        /// <summary>Element current</summary>
        public Element ElementCurrent { get; set; }

        /// <summary>Family name of element</summary>
        public string FamilyNameElement { get; set; }

        /// <summary>Type name of element</summary>
        public string TypeNameElement { get; set; }

        /// <summary>List ObjectParameter</summary>
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
        public ObjectReportCopy(Element elementCurrent)
        {
            ElementCurrent = elementCurrent;
        }

        #endregion Constructor

        //Member functions

        #region Member Functions

        /// ================================================================================
        /// <summary>Get Parameter Name</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public void GetParameterName()
        {
            if (ElementCurrent == null)
                return;

            ObjectParameterData = new List<ObjectParameter>();

            foreach (Parameter pr in ElementCurrent.Parameters)
            {
                if (pr == null || pr.Definition == null)
                    continue;

                ObjectParameter obj = new ObjectParameter();
                obj.NameParameter = pr.Definition.Name;
                obj.ElementIdGroup = pr.Definition.GetGroupTypeId();
                obj.CurrentParameter = pr;

                ObjectParameterData.Add(obj);
            }
        }

        /// ================================================================================
        /// <summary>Get family and type name of element</summary>
        ///
        /// <param name="currentDoc" >Current document</param>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public void GetFamilyAndTypeNameElement(Document currentDoc)
        {
            if (ElementCurrent == null)
                return;

            string strOrder = "なし";

            // Family name
            var familyParameter = ElementCurrent.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM);
            if (familyParameter != null)
                FamilyNameElement = familyParameter.AsValueString();
            else
                FamilyNameElement = strOrder;

            // Type name
            string typeName = ElementCurrent.Name;

            FamilyInstance familyInstance = ElementCurrent as FamilyInstance;
            if (familyInstance != null)
                typeName = familyInstance.Symbol.Name;
            else if (ElementCurrent.GetTypeId() != ElementId.InvalidElementId)
            {
                var typeElement = currentDoc.GetElement(ElementCurrent.GetTypeId());
                typeName = typeElement.Name;
            }
            if (ElementCurrent.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_Lines).ToString()))
            {
                CurveElement curveElement = ElementCurrent as CurveElement;
                typeName = typeName + "(" + curveElement.LineStyle.Name + ")";
            }
            if (ElementCurrent.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_Rooms).ToString()))
                typeName = typeName.Substring(0, typeName.LastIndexOf(" "));

            // Cant get type name
            if (string.IsNullOrEmpty(typeName))
                typeName = strOrder;

            TypeNameElement = typeName;
        }

        #endregion Member Functions
    }
}