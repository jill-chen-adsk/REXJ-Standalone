using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AutoLayoutTag;

namespace ADSK.JExtRAC.AutoLayoutTag.Entities
{
    /// ================================================================================
    /// <summary>DtBase</summary>
    /// ================================================================================
    public abstract class DtBase
    {
        // Member variable

        #region Member Variables

        /// <summary>Attribute</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>Elements</summary>
        private RvtExtApp.Components.Elements _CmpElements;

        /// <summary>Geometry</summary>
        private RvtExtApp.Components.Geometry _CmpGeometry;

        /// <summary>Parameters</summary>
        private RvtExtApp.Components.Parameters _CmpParameters;

        /// <summary>Settings</summary>
        private RvtExtApp.Components.Settings _CmpSettings;

        /// <summary>Error message</summary>
        private string _ErrMsg;

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
        protected DtBase(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.Elements cmpElements,
                         RvtExtApp.Components.Geometry cmpGeometry,
                         RvtExtApp.Components.Parameters cmpParameters,
                         RvtExtApp.Components.Settings cmpSettings)
        {
            // Initialization
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _ErrMsg = "";
        }

        #endregion Constructor

        // Properties

        #region Properties

        /// ================================================================================
        /// <summary>Attribute</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        protected RvtExtApp.Components.Attribute CmpAttribute
        {
            get
            {
                return _CmpAttribute;
            }
        }

        /// ================================================================================
        /// <summary>Elements</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        protected RvtExtApp.Components.Elements CmpElements
        {
            get
            {
                return _CmpElements;
            }
        }

        /// ================================================================================
        /// <summary>Geometry</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        protected RvtExtApp.Components.Geometry CmpGeometry
        {
            get
            {
                return _CmpGeometry;
            }
        }

        /// ================================================================================
        /// <summary>Parameters</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        protected RvtExtApp.Components.Parameters CmpParameters
        {
            get
            {
                return _CmpParameters;
            }
        }

        /// ================================================================================
        /// <summary>Settings</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        protected RvtExtApp.Components.Settings CmpSettings
        {
            get
            {
                return _CmpSettings;
            }
        }

        /// ================================================================================
        /// <summary>Error message</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public string ErrMsg
        {
            get
            {
                return _ErrMsg;
            }
            set
            {
                _ErrMsg = value;
            }
        }

        #endregion Properties
    }
}