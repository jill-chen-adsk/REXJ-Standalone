namespace ADSK.JExtRAC.AutoLayoutTag.Utils
{
    /// ================================================================================
    /// <summary>This class for tag position</summary>
    /// ================================================================================
    public class TagPosition
    {
        //Member variables

        #region Member Variables

        /// <summary>Left Right</summary>
        private string _leftRight;

        /// <summary>Top- Bottom</summary>
        private string _topBottom;

        #endregion Member Variables

        //Properties

        #region Properties

        /// ================================================================================
        /// <summary>LeftRight</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public string LeftRight
        {
            get { return _leftRight; }
            set { _leftRight = value; }
        }

        /// ================================================================================
        /// <summary>TopBottom</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public string TopBottom
        {
            get { return _topBottom; }
            set { _topBottom = value; }
        }

        #endregion Properties
    }
}