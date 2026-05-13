using System;
using ADSK.JExtRAC.ParameterFilter.Entities;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using System.Collections.Generic;
using System.Linq;
using Revit = Autodesk.Revit;

namespace ADSK.JExtRAC.ParameterFilter.Components
{
    /// ================================================================================
    /// <summary>要素</summary>
    /// ================================================================================
    public class Elements
    {
        public Revit.UI.UIDocument RvtUIDoc { get; }

        public Document RvtDBDoc => RvtUIDoc.Document;

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="rvtUIDoc">Revit UIドキュメント</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public Elements(Revit.UI.UIDocument rvtUIDoc)
        {
            RvtUIDoc = rvtUIDoc;
        }

        public Element GetElementDoc(int id)
        {
            var elemId = new ElementId((long)id);
            return RvtDBDoc.GetElement(elemId);
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>Get element selected in project</summary>
        ///
        /// <returns>List of element selected</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public IList<Element> SelElems
        {
            get
            {
                var elementList = new List<Element>();
                foreach (ElementId elementId in RvtUIDoc.Selection.GetElementIds())
                {
                    if (elementId == ElementId.InvalidElementId)
                        continue;
                    Element element = RvtDBDoc.GetElement(elementId);
                    if (element != null)
                        elementList.Add(element);
                }
                return elementList;
            }
        }


        #region Legacy_GetAllGroupTypeElement

                /// ================================================================================
        /// <summary>Get all type of group</summary>
        ///
        /// <param name="objectElements" >List of object element</param>
        ///
        /// <returns>List of object group element</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public List<ObjectSelectGroup> GetAllGroupTypeElement(List<ObjectElement> objectElements)
        {
            List<ObjectSelectGroup> retVal = new List<ObjectSelectGroup>();

            {
                // Analysis Results
                var prGroup = GroupTypeId.AnalysisResults;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Analytical Alignment
                var prGroup = GroupTypeId.AnalyticalAlignment;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);

            }
            {
                // Analytical Model
                var prGroup = GroupTypeId.AnalyticalModel;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Constraints
                var prGroup = GroupTypeId.Constraints;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Construction
                var prGroup = GroupTypeId.Construction;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Data
                var prGroup = GroupTypeId.Data;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Dimensions
                var prGroup = GroupTypeId.Geometry;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Division Geometry
                var prGroup = GroupTypeId.DivisionGeometry;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Electrical
                var prGroup = GroupTypeId.Electrical;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Electrical - Circuiting
                var prGroup = GroupTypeId.ElectricalCircuiting;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Electrical - Lighting
                var prGroup = GroupTypeId.ElectricalLighting;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Electrical - Loads
                var prGroup = GroupTypeId.ElectricalLoads;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Electrical Analysis
                var prGroup = GroupTypeId.ElectricalAnalysis;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Energy Analysis
                var prGroup = GroupTypeId.EnergyAnalysis;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Fire Protection
                var prGroup = GroupTypeId.FireProtection;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Forces
                var prGroup = GroupTypeId.Forces;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // General
                var prGroup = GroupTypeId.General;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Graphics
                var prGroup = GroupTypeId.Graphics;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Green Building Properties
                var prGroup = GroupTypeId.GreenBuilding;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Identity Data
                var prGroup = GroupTypeId.IdentityData;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // IFC Parameters
                var prGroup = GroupTypeId.Ifc;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Layers
                var prGroup = GroupTypeId.RebarSystemLayers;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Materials and Finishes
                var prGroup = GroupTypeId.Materials;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Mechanical
                var prGroup = GroupTypeId.Mechanical;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Mechanical - Flow
                var prGroup = GroupTypeId.MechanicalAirflow;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Mechanical - Loads
                var prGroup = GroupTypeId.MechanicalLoads;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Mechanical - Loads
                var prGroup = GroupTypeId.MechanicalLoads;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Model Properties
                var prGroup = GroupTypeId.AdskModelProperties;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Moments
                var prGroup = GroupTypeId.Moments;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Other
                var prGroup = new Revit.DB.ForgeTypeId(string.Empty);
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Overall Legend
                var prGroup = GroupTypeId.OverallLegend;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Phasing
                var prGroup = GroupTypeId.Phasing;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Photometrics
                var prGroup = GroupTypeId.LightPhotometrics;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Plumbing
                var prGroup = GroupTypeId.Plumbing;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Primary End
                var prGroup = GroupTypeId.PrimaryEnd;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Rebar Set
                var prGroup = GroupTypeId.RebarArray;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Releases / Member Forces
                var prGroup = GroupTypeId.ReleasesMemberForces;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Secondary End
                var prGroup = GroupTypeId.SecondaryEnd;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Segments and Fittings
                var prGroup = GroupTypeId.SegmentsFittings;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Set
                var prGroup = GroupTypeId.CouplerArray;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Slab Shape Edit
                var prGroup = GroupTypeId.SlabShapeEdit;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Structural
                var prGroup = GroupTypeId.Structural;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Structural Analysis
                var prGroup = GroupTypeId.StructuralAnalysis;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Text
                var prGroup = GroupTypeId.Text;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Title Text
                var prGroup = GroupTypeId.Title;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }
            {
                // Visibility
                var prGroup = GroupTypeId.Visibility;
                ObjectSelectGroup objGroup = new ObjectSelectGroup();
                objGroup.GroupTypeId = prGroup;
                objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                objGroup.IsSelected = true;
                retVal.Add(objGroup);
            }

            retVal = retVal.OrderBy(x => x.ParameterGroupVal).ToList();

            return retVal;
        }


        #endregion

        /// <summary>
        /// Get all type of group
        /// </summary>
        /// <param name="objectElements"></param>
        /// <returns>List of object group element</returns>
        public List<ObjectSelectGroup> GetAllGroupTypeElement_( List<ObjectElement> objectElements )
        {
            var retVal = new List<ObjectSelectGroup>();
            var groupTypeIds = new List<ForgeTypeId>
            {
                GroupTypeId.AnalysisResults,
                GroupTypeId.AnalyticalAlignment,
                GroupTypeId.AnalyticalModel,
                GroupTypeId.Constraints,
                GroupTypeId.Construction,
                GroupTypeId.Data,
                GroupTypeId.Geometry,
                GroupTypeId.DivisionGeometry,
                GroupTypeId.Electrical,
                GroupTypeId.ElectricalCircuiting,
                GroupTypeId.ElectricalLighting,
                GroupTypeId.ElectricalLoads,
                GroupTypeId.ElectricalAnalysis,
                GroupTypeId.EnergyAnalysis,
                GroupTypeId.FireProtection,
                GroupTypeId.Forces,
                GroupTypeId.General,
                GroupTypeId.Graphics,
                GroupTypeId.GreenBuilding,
                GroupTypeId.IdentityData,
                GroupTypeId.Ifc,
                GroupTypeId.RebarSystemLayers,
                GroupTypeId.Materials,
                GroupTypeId.Mechanical,
                GroupTypeId.MechanicalAirflow,
                GroupTypeId.MechanicalLoads,
                GroupTypeId.AdskModelProperties,
                GroupTypeId.Moments,
                new Revit.DB.ForgeTypeId(string.Empty), // Other
                GroupTypeId.OverallLegend,
                GroupTypeId.Phasing,
                GroupTypeId.LightPhotometrics,
                GroupTypeId.Plumbing,
                GroupTypeId.PrimaryEnd,
                GroupTypeId.RebarArray,
                GroupTypeId.ReleasesMemberForces,
                GroupTypeId.SecondaryEnd,
                GroupTypeId.SegmentsFittings,
                GroupTypeId.CouplerArray,
                GroupTypeId.SlabShapeEdit,
                GroupTypeId.Structural,
                GroupTypeId.StructuralAnalysis,
                GroupTypeId.Text,
                GroupTypeId.Title,
                GroupTypeId.Visibility
            };
            
            // 各GroupTypeIdについてObjectSelectGroupを作成
            foreach (var prGroup in groupTypeIds)
            {
                try {
                    var objGroup = new ObjectSelectGroup();
                    objGroup.GroupTypeId = prGroup;
                    objGroup.ParameterGroupVal = LabelUtils.GetLabelForBuiltInParameter(prGroup);
                    objGroup.IsSelected = true;
                    retVal.Add(objGroup);
                }
                catch (Exception)
                {
                    //GetLabelForBuiltInParameterで取得できない場合の例外処理
                }
            }

            // 結果を並べ替えて返す
            return retVal.OrderBy(x => x.ParameterGroupVal).ToList();
            
        }
        
        
        
        /// ================================================================================
        /// <summary>Get all element is connected with select element</summary>
        ///
        /// <param name="lstElement" >List of all object length parameter</param>
        ///
        /// <returns>List of element id is connected</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public List<ObjectLengthParameter> GetSelectElementConnect(List<ObjectLengthParameter> lstElement)
        {
            List<ObjectLengthParameter> retVal = new List<ObjectLengthParameter>();

            Dictionary<Element, List<ObjectElement>> dicSystem = new Dictionary<Element, List<ObjectElement>>();
            List<ObjectElement> objectElementFound = new List<ObjectElement>();
            List<ObjectElement> lstObjectCompair = new List<ObjectElement>();
            foreach (var objLength in lstElement)
            {
                List<ObjectLengthParameter> lstSelected = new List<ObjectLengthParameter>();

                var eleFound = dicSystem.Where(x => x.Value.Any(y => y.ElementCurrent.Id == objLength.ElementCurrent.Id)).ToList();
                if (eleFound.Count != 0)
                {
                    objectElementFound = eleFound.FirstOrDefault().Value;

                    var findObjectCompair = lstObjectCompair.Where(x => x.ElementCurrent.Id == objLength.ElementCurrent.Id).FirstOrDefault();
                    if (findObjectCompair == null)
                    {
                        List<Element> lstEleTempCompair = new List<Element> { objLength.ElementCurrent };
                        findObjectCompair = GetDataElement(lstEleTempCompair).FirstOrDefault();

                        lstObjectCompair.Add(findObjectCompair);
                    }

                    // Start filter by value and min max value
                    lstSelected = FilterByMinMaxValueConnectElement(objectElementFound, objLength, findObjectCompair);
                }
                else
                {
                    // Get connector
                    var elementFound = new List<Element> { objLength.ElementCurrent };
                    GetReferenceConnectors(elementFound, objLength.ElementCurrent);
                    // Remove duplicate
                    elementFound = elementFound.GroupBy(x => x.Id).Select(y => y.FirstOrDefault()).ToList();

                    // Get data parameter of element
                    var objectElementData = GetDataElement(elementFound);

                    // Filter current checkbox object length
                    objectElementData = FilterObjectLengthByCurrentFilter(objectElementData, lstElement.FirstOrDefault());

                    if (dicSystem.Any(x => x.Key.Id == objLength.ElementCurrent.Id) == false)
                        dicSystem.Add(objLength.ElementCurrent, objectElementData);

                    var findObjectCompair = lstObjectCompair.Where(x => x.ElementCurrent.Id == objLength.ElementCurrent.Id).FirstOrDefault();
                    if (findObjectCompair == null)
                    {
                        List<Element> lstEleTempCompair = new List<Element> { objLength.ElementCurrent };
                        findObjectCompair = GetDataElement(lstEleTempCompair).FirstOrDefault();

                        lstObjectCompair.Add(findObjectCompair);
                    }

                    // Start filter by value and min max value
                    lstSelected = FilterByMinMaxValueConnectElement(objectElementData, objLength, findObjectCompair);
                }

                // Add to list
                retVal.AddRange(lstSelected);
            }

            // Remove duplicate
            retVal = retVal.GroupBy(x => x).Select(y => y.First()).ToList();
            return retVal;
        }

        /// ================================================================================
        /// <summary>Filter Object length by user chose only 1 </summary>
        ///
        /// <param name="objectElementData" >List all object length of current element</param>
        /// <param name="objLengthCurrent" >Current user need to find object length</param>
        ///
        /// <returns>List object length of current element</returns>
        ///
        /// <history>2022/01/26 Created Applied Technology</history>
        /// ================================================================================
        ///
        private List<ObjectElement> FilterObjectLengthByCurrentFilter(List<ObjectElement> objectElementData, ObjectLengthParameter objLengthCurrent)
        {
            if (objLengthCurrent == null)
                return objectElementData;

            foreach (var objElement in objectElementData)
            {
                var findVal = objElement.ObjectLengths.Where(x => x.ObjectGroupVal.GroupTypeId == objLengthCurrent.ObjectGroupVal.GroupTypeId &&
                                                x.NameParameterLength == objLengthCurrent.NameParameterLength).FirstOrDefault();
                if (findVal == null)
                    continue;

                objElement.ObjectLengths.Clear();
                objElement.ObjectLengths.Add(findVal);
            }

            return objectElementData;
        }

        /// ================================================================================
        /// <summary>Filter element by min and max</summary>
        ///
        /// <param name="lstObjectElement" >List of object element</param>
        /// <param name="objLengthPr" >Object length min max of user selected</param>
        /// <param name="objElementOfLengthPr" >Category, family, family type compair</param>
        ///
        /// <returns>List of element id</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        ///
        private List<ObjectLengthParameter> FilterByMinMaxValueConnectElement(List<ObjectElement> lstObjectElement, ObjectLengthParameter objLengthPr, ObjectElement objElementOfLengthPr)
        {
            List<ObjectLengthParameter> retVal = new List<ObjectLengthParameter>();

            foreach (var objElement in lstObjectElement)
            {
                if (objElement.CategoriesElement != objElementOfLengthPr.CategoriesElement ||
                    objElement.FamilyNameElement != objElementOfLengthPr.FamilyNameElement ||
                    objElement.TypeNameElement != objElementOfLengthPr.TypeNameElement)
                    continue;

                // Filter by user input value min max
                var lstObjElementNeedSelect = FilterParameterByUserInput(objElement.ObjectLengths, objLengthPr.prValueDgv, objLengthPr.prMinDgv, objLengthPr.prMaxDgv);
                if (lstObjElementNeedSelect == null)
                    return retVal;

                retVal.AddRange(lstObjElementNeedSelect);
            }

            // Remove duplicate
            retVal = retVal.GroupBy(x => x).Select(y => y.First()).ToList();
            return retVal;
        }

        /// ================================================================================
        /// <summary>Get all Connector element</summary>
        ///
        /// <param name="elementFound" >List all element</param>
        /// <param name="element" >Element need to get connector</param>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void GetReferenceConnectors(List<Element> elementFound, Element element)
        {
            try
            {
                ConnectorSet connectors = GetConnectors(element);
                if (connectors == null || connectors.Size == 0)
                    return;

                foreach (Connector con in connectors)
                {
                    // Check data
                    if (con.IsValidObject == false || con.AllRefs == null || con.AllRefs.Size == 0)
                        continue;
                    //if (con.IsConnected == false)
                    //    continue;

                    foreach (Connector connectTo in con.AllRefs)
                    {
                        if (connectTo.IsValidObject == false)
                            continue;

                        if (connectTo.Owner == null)
                            continue;

                        // Check connected
                        if (con.IsConnectedTo(connectTo) == false && connectTo.IsConnectedTo(con) == false)
                            continue;

                        Element elementOwner = connectTo.Owner;

                        ////Check element category
                        //if (IsAcceptElement(element) == false)
                        //    continue;

                        if (elementFound.Any(x => x.Id == elementOwner.Id))
                            continue;

                        elementFound.Add(elementOwner);

                        GetReferenceConnectors(elementFound, elementOwner);
                    }
                }
            }
            catch (System.Exception ex)
            {
                string mess = ex.Message;
            }
        }

        /// ================================================================================
        /// <summary>Get all Connector of element</summary>
        ///
        /// <param name="element" >Element</param>
        ///
        /// <returns>All Connector</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private ConnectorSet GetConnectors(Element element)
        {
            try
            {
                ConnectorSet connectors = null;

                if (element is FamilyInstance)                          // Element is family instance
                {
                    MEPModel m = ((FamilyInstance)element).MEPModel;

                    if (m != null && m.ConnectorManager != null)
                        connectors = m.ConnectorManager.Connectors;
                }
                else if (element is Wire)                               // Element is Wire
                    connectors = ((Wire)element).ConnectorManager.Connectors;
                else
                {
                    if (element is MEPCurve)                            // Element is curve
                        connectors = ((MEPCurve)element).ConnectorManager.Connectors;
                }
                return connectors;
            }
            catch (System.Exception ex)
            {
                string mess = ex.Message;
                return null;
            }
        }

        /// ================================================================================
        /// <summary>Get data (Family, type, parameter) of element</summary>
        ///
        /// <param name="elemSet" >List of element</param>
        ///
        /// <returns>List of object element</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public List<ObjectElement> GetDataElement(List<Element> elemSet)
        {
            List<ObjectElement> retVal = new List<ObjectElement>();
            string strOrder = "＜なし＞";

            foreach (Element elem in (IEnumerable<Element>)elemSet)
            {
                ObjectElement objElement = new ObjectElement();

                objElement.ElementCurrent = elem;
                if (elem.Category != null)
                {
                    if (elem.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_Lines).ToString()))
                        objElement.CategoriesElement = elem.Name;
                    else
                        objElement.CategoriesElement = elem.Category.Name;
                }

                // Family name
                var familyParameter = elem.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM);
                if (familyParameter != null)
                    objElement.FamilyNameElement = familyParameter.AsValueString();
                else
                    objElement.FamilyNameElement = strOrder;

                // Type name and value of length type parameter
                string typeName = elem.Name;

                FamilyInstance familyInstance = elem as FamilyInstance;
                if (familyInstance != null)
                    typeName = familyInstance.Symbol.Name;
                else if (elem.GetTypeId() != ElementId.InvalidElementId)
                {
                    var typeElement = RvtDBDoc.GetElement(elem.GetTypeId());
                    if (typeElement != null)
                        typeName = typeElement.Name;
                }
                if (elem.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_Lines).ToString()))
                {
                    CurveElement curveElement = elem as CurveElement;
                    typeName = typeName + "(" + curveElement.LineStyle.Name + ")";
                }
                if (elem.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_Rooms).ToString()))
                    typeName = typeName.Substring(0, typeName.LastIndexOf(" "));

                // Cant get type name
                if (string.IsNullOrEmpty(typeName))
                    typeName = strOrder;
                if (string.IsNullOrEmpty(objElement.FamilyNameElement))
                    objElement.FamilyNameElement = strOrder;

                objElement.TypeNameElement = typeName;

                // Get length parameter
                objElement.GetLengthAndGroupParamter();

                retVal.Add(objElement);
            }

            return retVal;
        }

        /// ================================================================================
        /// <summary>Filter element by min, max and value</summary>
        ///
        /// <param name="lstObjElement" >List of object element</param>
        /// <param name="prValueDgv" >Value filter user input</param>
        /// <param name="prMinDgv" >Min value</param>
        /// <param name="prMaxDgv" >Max value</param>
        ///
        /// <returns>List of object group element</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public List<ObjectLengthParameter> FilterParameterByUserInput(List<ObjectLengthParameter> lstObjElement, object prValueDgv, object prMinDgv, object prMaxDgv)
        {
            List<ObjectLengthParameter> retVal = new List<ObjectLengthParameter>();

            // User didn't input value
            if ((prValueDgv == null || string.IsNullOrEmpty(prValueDgv.ToString())) &&
                (prMinDgv == null || string.IsNullOrEmpty(prMinDgv.ToString())) &&
                (prMaxDgv == null || string.IsNullOrEmpty(prMaxDgv.ToString())))
            {
                retVal = lstObjElement;
                return retVal;
            }

            // User input all value
            if (prValueDgv != null && string.IsNullOrEmpty(prValueDgv.ToString()) == false &&
                prMinDgv != null && string.IsNullOrEmpty(prMinDgv.ToString()) == false &&
                prMaxDgv != null && string.IsNullOrEmpty(prMaxDgv.ToString()) == false)
            {
                prMinDgv = string.Empty;
                prMaxDgv = string.Empty;
            }

            // User input min and max
            if (prMinDgv != null && string.IsNullOrEmpty(prMinDgv.ToString()) == false &&
                prMaxDgv != null && string.IsNullOrEmpty(prMaxDgv.ToString()) == false)
            {
                // User input number
                int minPr = 0, maxPr = 0;
                if (int.TryParse(prMinDgv.ToString(), out minPr) == false || int.TryParse(prMaxDgv.ToString(), out maxPr) == false)
                    return null;

                foreach (var item in lstObjElement)
                {
                    if (item.LengthVal >= minPr && item.LengthVal <= maxPr)
                        retVal.Add(item);
                }
            }
            else if (prValueDgv != null && string.IsNullOrEmpty(prValueDgv.ToString()) == false)        // User input value
            {
                double valuePr = 0;
                if (double.TryParse(prValueDgv.ToString(), out valuePr) == false)
                    return null;

                foreach (var item in lstObjElement)
                {
                    if (item.LengthVal == valuePr)
                        retVal.Add(item);
                }
            }
            else if ((prMinDgv == null || string.IsNullOrEmpty(prMinDgv.ToString())) &&
                    prMaxDgv != null && string.IsNullOrEmpty(prMaxDgv.ToString()) == false)             // User input only max value
            {
                // User input number
                int maxPr = 0;
                if (int.TryParse(prMaxDgv.ToString(), out maxPr) == false)
                    return null;

                foreach (var item in lstObjElement)
                {
                    if (item.LengthVal <= maxPr)
                        retVal.Add(item);
                }
            }
            else if ((prMinDgv != null && string.IsNullOrEmpty(prMinDgv.ToString()) == false) &&
                    prMaxDgv == null || string.IsNullOrEmpty(prMaxDgv.ToString()))                      // User input only min value
            {
                // User input number
                int minPr = 0;
                if (int.TryParse(prMinDgv.ToString(), out minPr) == false)
                    return null;

                foreach (var item in lstObjElement)
                {
                    if (item.LengthVal >= minPr)
                        retVal.Add(item);
                }
            }

            return retVal;
        }

        #endregion Member Functions
    }
}