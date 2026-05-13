using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AutoLayoutTag;
using System.Linq;
using System.Text;

namespace ADSK.JExtRAC.AutoLayoutTag.Entities
{
    /// ================================================================================
    /// <summary>Data Tag</summary>
    /// ================================================================================
    public class DtTag : RvtExtApp.Entities.DtBase
    {
        // Member variables

        #region Member Variables

        /// <summary>Option area premises</summary>
        private int _rdbAreaPremises;

        /// <summary>Option tag leader</summary>
        private int _rdbTagLeader;

        /// <summary>Loading for the first time</summary>
        private int _numberShow;

        /// <summary>Option handle preset tag</summary>
        private int _rdbHandlePresetTag;

        /// <summary>option get object</summary>
        private int _rdbGetObject;

        /// <summary>checkbox left right</summary>
        private bool _chkLeftRight;

        /// <summary>checkbox top bottom</summary>
        private bool _chkTopBottom;

        /// <summary>outline</summary>
        private Revit.DB.Outline _outLine;

        /// <summary>list element user pick </summary>
        private Collections.Generic.List<Revit.DB.Element> _lstElement;

        /// <summary>list category user select </summary>
        private Collections.Generic.List<Revit.DB.BuiltInCategory> _lstBuiltInCategory;

        /// <summary>value setting</summary>
        private Collections.Generic.Dictionary<Revit.DB.BuiltInCategory, Revit.DB.FamilySymbol> _dicCategory;

        #endregion Member Variables

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="cmpAttribute"  >Attribute</param>
        /// <param name="cmpElements"   >Elements</param>
        /// <param name="cmpGeometry"   >Geometry</param>
        /// <param name="cmpParameters" >Parameters</param>
        /// <param name="cmpSettings"   >Settings</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public DtTag(RvtExtApp.Components.Attribute cmpAttribute,
                      RvtExtApp.Components.Elements cmpElements,
                      RvtExtApp.Components.Geometry cmpGeometry,
                      RvtExtApp.Components.Parameters cmpParameters,
                      RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            // Value default
            Initvalue();
            _lstElement = new Collections.Generic.List<Revit.DB.Element>();
            _lstBuiltInCategory = new Collections.Generic.List<Revit.DB.BuiltInCategory>();
            _dicCategory = new Collections.Generic.Dictionary<Revit.DB.BuiltInCategory, Revit.DB.FamilySymbol>();
        }

        #endregion Constructor

        // Member functions

        #region Member Functions

        /// ================================================================================
        /// <summary>Init value</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public void Initvalue()
        {
            _rdbGetObject = 1;
            _chkLeftRight = true;
            _chkTopBottom = true;
            _rdbAreaPremises = 0;
            _rdbHandlePresetTag = 1;
        }

        #endregion Member Functions

        // Properties

        #region Properties

        /// ================================================================================
        /// <summary>list element user pick</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public Collections.Generic.List<Revit.DB.Element> LstElement
        {
            get { return _lstElement; }
            set { _lstElement = value; }
        }

        /// ================================================================================
        /// <summary>list category user select</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public Collections.Generic.List<Revit.DB.BuiltInCategory> LstBuiltInCategory
        {
            get { return _lstBuiltInCategory; }
            set { _lstBuiltInCategory = value; }
        }

        /// ================================================================================
        /// <summary>value settings</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public Collections.Generic.Dictionary<Revit.DB.BuiltInCategory, Revit.DB.FamilySymbol> DicCategory
        {
            get { return _dicCategory; }
            set { _dicCategory = value; }
        }

        /// ================================================================================
        /// <summary>Number show form</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public int NumberShow
        {
            get
            {
                return _numberShow;
            }
            set
            {
                _numberShow = value;
            }
        }

        /// ================================================================================
        /// <summary>Option get object</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public int GetObjectOpt
        {
            get
            {
                return _rdbGetObject;
            }
            set
            {
                _rdbGetObject = value;
            }
        }

        /// ================================================================================
        /// <summary>Value checkbox left right</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public bool ChkLeftRight
        {
            get
            {
                return _chkLeftRight;
            }
            set
            {
                _chkLeftRight = value;
            }
        }

        /// ================================================================================
        /// <summary>Value checkbox top bottom</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public bool ChkTopBottom
        {
            get
            {
                return _chkTopBottom;
            }
            set
            {
                _chkTopBottom = value;
            }
        }

        /// ================================================================================
        /// <summary>Option ara premises</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public int AreaPremisesOpt
        {
            get
            {
                return _rdbAreaPremises;
            }
            set
            {
                _rdbAreaPremises = value;
            }
        }

        /// ================================================================================
        /// <summary>Option tag leader</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public int TagLeaderOtp
        {
            get
            {
                return _rdbTagLeader;
            }
            set
            {
                _rdbTagLeader = value;
            }
        }

        /// ================================================================================
        /// <summary>Option handle preset tag</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public int HandlePresetTagOpt
        {
            get
            {
                return _rdbHandlePresetTag;
            }
            set
            {
                _rdbHandlePresetTag = value;
            }
        }

        /// ================================================================================
        /// <summary>Outline</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public Revit.DB.Outline OutLine
        {
            get
            {
                return _outLine;
            }
            set
            {
                _outLine = value;
            }
        }

        #endregion Properties
    }
}