using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collections = System.Collections;
using Revit = Autodesk.Revit;

namespace ADSK.JExtRAC.AutoLayoutTag.Entities
{
    /// ================================================================================
    /// <summary>This class settings element need set tag</summary>
    /// ================================================================================
    public class ElementData
    {
        // Member variables

        #region Member Variables

        /// <summary>Element</summary>
        private Autodesk.Revit.DB.Element _element;

        /// <summary> is check </summary>
        private bool _isCheck;

        /// <summary>Type of tag element</summary>
        private Autodesk.Revit.DB.FamilySymbol _tagSymbol;

        /// <summary>bounding box of element</summary>
        private Revit.DB.BoundingBoxXYZ _boundingbox;

        /// <summary>Point center bounding box of element</summary>
        private Revit.DB.XYZ _centerPoint;

        /// <summary>width</summary>
        private double _width;

        /// <summary>height</summary>
        private double _height;

        /// <summary>Distance </summary>
        private double _distacnce;

        /// <summary>Enum position </summary>
        private POST_ELEMENT _postELement;

        #endregion Member Variables

        // Properties

        #region Properties

        /// ================================================================================
        /// <summary>Element</summary>
        /// <history>2021/12/11 Created Applied Technology</history>
        /// ================================================================================
        public Autodesk.Revit.DB.Element ElementOrigin
        {
            get { return _element; }
            set { _element = value; }
        }

        /// ================================================================================
        /// <summary>Type of tag </summary>
        /// <history>2021/12/11 Created Applied Technology</history>
        /// ================================================================================
        public Autodesk.Revit.DB.FamilySymbol TagSymbol
        {
            get { return _tagSymbol; }
            set { _tagSymbol = value; }
        }

        /// ================================================================================
        /// <summary> is check</summary>
        /// <history>2021/12/11 Created Applied Technology</history>
        /// ================================================================================
        public bool IsCheck
        {
            get { return _isCheck; }
            set { _isCheck = value; }
        }

        /// ================================================================================
        /// <summary>Bounding box element</summary>
        /// <history>2021/12/11 Created Applied Technology</history>
        /// ================================================================================
        public Revit.DB.BoundingBoxXYZ BoundingBoxEle
        {
            get { return _boundingbox; }
            set { _boundingbox = value; }
        }

        /// ================================================================================
        /// <summary>Point center element</summary>
        /// <history>2021/12/11 Created Applied Technology</history>
        /// ================================================================================
        public Revit.DB.XYZ CenterPoint
        {
            get { return _centerPoint; }
            set { _centerPoint = value; }
        }

        /// ================================================================================
        /// <summary>Height</summary>
        /// <history>2021/12/11 Created Applied Technology</history>
        /// ================================================================================
        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// ================================================================================
        /// <summary>Width</summary>
        /// <history>20121/12/11 Created Applied Technology</history>
        /// ================================================================================
        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// ================================================================================
        /// <summary>Distance to line</summary>
        /// <history>2021/12/11 Created Applied Technology</history>
        /// ================================================================================
        public double Distance
        {
            get { return _distacnce; }
            set { _distacnce = value; }
        }

        /// ================================================================================
        /// <summary>POST_ELEMENT</summary>
        /// <history>2021/12/11 Created Applied Technology</history>
        /// ================================================================================
        public POST_ELEMENT PostElement
        {
            get { return _postELement; }
            set { _postELement = value; }
        }

        #endregion Properties
    }

    /// ================================================================================
    /// <summary>the index of the region to which the element's position belongs</summary>
    /// ================================================================================
    public enum POST_ELEMENT
    { CS_NULL, CS_REGION1, CS_REGION2, CS_REGION3, CS_REGION4, CS_REGION5, CS_REGION6, CS_REGION7, CS_REGION8 }
}