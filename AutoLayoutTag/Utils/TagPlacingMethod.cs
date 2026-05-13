namespace ADSK.JExtRAC.AutoLayoutTag.Utils
{/// ================================================================================
 /// <summary>This class for Tag Placing Method </summary>
 /// ================================================================================
    public class TagPlacingMethod
    {
        //Member variables

        #region Member Variables

        /// <summary>PlacingMethodType</summary>
        private int _placingMethodType;

        /// <summary>Point1</summary>
        private string _point1;

        /// <summary>Point2</summary>
        private string _point2;

        #endregion Member Variables

        //Properties

        #region Properties

        /// ================================================================================
        /// <summary>Placing Method Type</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public int PlacingMethodType
        {
            get
            {
                return _placingMethodType;
            }
            set
            {
                _placingMethodType = value;
            }
        }

        /// ================================================================================
        /// <summary>Point1</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public string Point1
        {
            get
            {
                return _point1;
            }
            set
            {
                _point1 = value;
            }
        }

        /// ================================================================================
        /// <summary>Point2</summary>
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public string Point2
        {
            get
            {
                return _point2;
            }
            set
            {
                _point2 = value;
            }
        }

        #endregion Properties
    }
}