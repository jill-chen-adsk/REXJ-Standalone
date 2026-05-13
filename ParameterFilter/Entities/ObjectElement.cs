using System ;
using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ADSK.JExtRAC.ParameterFilter.Entities
{
    public class ObjectElement
    {
        #region Member Variables

        /// <summary>Element</summary>
        public Element ElementCurrent { get; set; }

        /// <summary>Category of element</summary>
        public string CategoriesElement { get; set; }

        /// <summary>Family name of element</summary>
        public string FamilyNameElement { get; set; }

        /// <summary>Type name of element</summary>
        public string TypeNameElement { get; set; }

        /// <summary>List of length parameter</summary>
        public List<ObjectLengthParameter> ObjectLengths { get; set; }

        #endregion Member Variables

        #region Member Functions

        /// ================================================================================
        /// <summary>Get parameter group and length of parameter</summary>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public void GetLengthAndGroupParamter()
        {
            if (ElementCurrent == null)
                return;

            ObjectLengths = new List<ObjectLengthParameter>();

            foreach (Parameter pr in ElementCurrent.Parameters)
            {
                if (pr == null || pr.Definition == null)
                    continue;

                if (pr.Definition.GetDataType() != SpecTypeId.Length)
                    continue;

                try {
                    ObjectLengthParameter obj = new ObjectLengthParameter();
                    obj.ElementCurrent = ElementCurrent;
                    obj.NameParameterLength = pr.Definition.Name;
                    obj.LengthVal = System.Math.Round(UnitUtils.ConvertFromInternalUnits(pr.AsDouble(), UnitTypeId.Millimeters), 6);

                    ObjectSelectGroup objGroup = new ObjectSelectGroup();
                    objGroup.GroupTypeId = pr.Definition.GetGroupTypeId();
                    objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(pr.Definition.GetGroupTypeId());
                    // LabelUtils.GetLabelForBuiltInParameter は見つからない場合に例外を吐く
                    
                    objGroup.IsSelected = true;
                    obj.ObjectGroupVal = objGroup;

                    ObjectLengths.Add(obj);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            // Add fake length
            if (ObjectLengths.Count == 0)
            {
                ObjectLengthParameter obj = new ObjectLengthParameter();
                obj.ElementCurrent = ElementCurrent;
                obj.NameParameterLength = string.Empty;
                obj.LengthVal = double.MinValue;

                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = new ForgeTypeId(string.Empty);
                objGroup.ParameterGroupVal = string.Empty;
                objGroup.IsSelected = true;
                obj.ObjectGroupVal = objGroup;

                ObjectLengths.Add(obj);
            }
        }

        #endregion Member Functions
    }
}