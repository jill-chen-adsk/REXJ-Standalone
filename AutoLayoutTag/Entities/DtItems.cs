using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AutoLayoutTag;

namespace ADSK.JExtRAC.AutoLayoutTag.Entities
{
    /// ================================================================================
    /// <summary>DtItems</summary>
    /// ================================================================================
    public class DtItems
    {
        // Member variables

        #region Member Variables

        /// <summary>Attribute</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>File Path</summary>
        private string _FilePath;

        #endregion Member Variables

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="cmpAttribute">Attribute</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public DtItems(RvtExtApp.Components.Attribute cmpAttribute)
        {
            _CmpAttribute = cmpAttribute;

            string itemsFoldr = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // file path
            _FilePath = itemsFoldr + "\\" + _CmpAttribute.ResourceText("IDS_FILE_ITEMS");
            if (System.IO.File.Exists(_FilePath) == false)
            {
                _FilePath = null;
            }
        }

        #endregion Constructor

        // Properties

        #region Properties

        /// ================================================================================
        /// <summary>File path settings</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public string FilePath
        {
            get
            {
                return _FilePath;
            }
        }

        #endregion Properties
    }
}