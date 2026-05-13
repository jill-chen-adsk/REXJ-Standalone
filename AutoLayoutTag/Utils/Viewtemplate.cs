using Collections = System.Collections;

namespace ADSK.JExtRAC.AutoLayoutTag.Utils
{
    /// ================================================================================
    /// <summary>This class for view template</summary>
    /// ================================================================================
    public class Viewtemplate
    {
        //Member variables

        #region Member Variables

        /// <summary>ID view template</summary>
        private int _idViewTemplate;

        /// <summary>Option tag leader</summary>
        private int _tagLeader;

        /// <summary>name view template</summary>
        private string _viewTemplateName;

        /// <summary>Target object type</summary>
        private int _targetObjectType;

        /// <summary>Target object ids</summary>
        private Collections.Generic.List<int> _targetObjectIds;

        /// <summary>Target object category</summary>
        private Collections.Generic.List<string> _targetObjectCategories;

        /// <summary>tag position</summary>
        private TagPosition _tagPosition;

        /// <summary>tag palacing method</summary>
        private TagPlacingMethod _tagPlacingMethod;

        /// <summary>Existed tag processing type</summary>
        private int _existedTagProcessingType;

        /// <summary>list tag family type</summary>
        private Collections.Generic.List<TagFamilyType> _tagFamilyType;

        #endregion Member Variables

        //Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public Viewtemplate()
        {
            _targetObjectIds = new Collections.Generic.List<int>();
            _targetObjectCategories = new Collections.Generic.List<string>();
            _tagFamilyType = new Collections.Generic.List<TagFamilyType>();
        }

        #endregion Constructor

        //Properties

        #region Properties

        /// ================================================================================
        /// <summary>ID view template</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public int ViewTemplateId
        {
            get
            {
                return _idViewTemplate;
            }
            set
            {
                _idViewTemplate = value;
            }
        }

        /// ================================================================================
        /// <summary>Option tag leader</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public int TagLeader
        {
            get
            {
                return _tagLeader;
            }
            set
            {
                _tagLeader = value;
            }
        }

        /// ================================================================================
        /// <summary>name view template</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public string ViewTemplateName
        {
            get
            {
                return _viewTemplateName;
            }
            set
            {
                _viewTemplateName = value;
            }
        }

        /// ================================================================================
        /// <summary>TargetObjectType</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public int TargetObjectType
        {
            get
            {
                return _targetObjectType;
            }
            set
            {
                _targetObjectType = value;
            }
        }

        /// ================================================================================
        /// <summary>List  TargetObjectIds</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public Collections.Generic.List<int> TargetObjectIds
        {
            get
            {
                return _targetObjectIds;
            }
            set
            {
                _targetObjectIds = value;
            }
        }

        /// ================================================================================
        /// <summary>List TargetObjectCategories</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public Collections.Generic.List<string> TargetObjectCategories
        {
            get
            {
                return _targetObjectCategories;
            }
            set
            {
                _targetObjectCategories = value;
            }
        }

        /// ================================================================================
        /// <summary>TagPosition</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public TagPosition TagPosition
        {
            get
            {
                return _tagPosition;
            }
            set
            {
                _tagPosition = value;
            }
        }

        /// ================================================================================
        /// <summary>TagPlacingMethod</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public TagPlacingMethod TagPlacingMethod
        {
            get
            {
                return _tagPlacingMethod;
            }
            set
            {
                _tagPlacingMethod = value;
            }
        }

        /// ================================================================================
        /// <summary>ExistedTagProcessingType</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public int ExistedTagProcessingType
        {
            get
            {
                return _existedTagProcessingType;
            }
            set
            {
                _existedTagProcessingType = value;
            }
        }

        /// ================================================================================
        /// <summary>List TagFamilyType</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public Collections.Generic.List<TagFamilyType> TagFamilyType
        {
            get
            {
                return _tagFamilyType;
            }
            set
            {
                _tagFamilyType = value;
            }
        }

        #endregion Properties
    }
}