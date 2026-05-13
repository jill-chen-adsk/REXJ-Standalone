using Collections = System.Collections;

namespace ADSK.JExtRAC.AutoLayoutTag.Utils
{
    /// ================================================================================
    /// <summary>This class for Root of view template</summary>
    /// ================================================================================
    public class Root
    {
        // Member variables

        #region Member Variables

        /// <summary>List viewTemplate;</summary>
        private Collections.Generic.List<Viewtemplate> _viewTemplates;

        #endregion Member Variables

        //Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public Root()
        {
            _viewTemplates = new Collections.Generic.List<Viewtemplate>();
        }

        #endregion Constructor

        //Properties

        #region Properties

        /// ================================================================================
        /// <summary>ViewTemplate</summary>
        /// ================================================================================
        public Collections.Generic.List<Viewtemplate> ViewTemplates
        {
            get
            {
                return _viewTemplates;
            }
            set
            {
                _viewTemplates = value;
            }
        }

        #endregion Properties
    }
}