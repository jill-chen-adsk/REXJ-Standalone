using Collections = System.Collections;
using Revit = Autodesk.Revit;

namespace ADSK.JExtRAC.AutoLayoutTag.Utils
{
    /// ================================================================================
    /// <summary>This class for tag family type</summary>
    /// ================================================================================
    public class TagFamilyType
    {
        //Member variables

        #region Member Variables

        /// <summary>Category</summary>
        private string _category;

        /// <summary>Category</summary>
        private Revit.DB.BuiltInCategory _builIncategory;

        /// <summary>Tag Family Type Name</summary>
        private string _tagFamilyTypeName;

        /// <summary>ElementId type tag</summary>
        private int _tagTypeId;

        #endregion Member Variables

        //Properties

        #region Properties

        /// ================================================================================
        /// <summary>Category</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
            }
        }

        /// ================================================================================
        /// <summary>Category</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public Revit.DB.BuiltInCategory CategoryId
        {
            get
            {
                return _builIncategory;
            }
            set
            {
                _builIncategory = value;
            }
        }

        /// ================================================================================
        /// <summary>Tag Family Type Name</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public string TagFamilyTypeName
        {
            get
            {
                return _tagFamilyTypeName;
            }
            set
            {
                _tagFamilyTypeName = value;
            }
        }

        /// ================================================================================
        /// <summary>ElementId type tag</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public int TagTypeId
        {
            get
            {
                return _tagTypeId;
            }
            set
            {
                _tagTypeId = value;
            }
        }

        #endregion Properties
    }
}