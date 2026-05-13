using RvtExtApp = ADSK.JExtRAC.AutomaticFloor;

namespace ADSK.JExtRAC.AutomaticFloor.Entities
{
    public abstract class DtBase
    {
        private RvtExtApp.Components.Attribute _CmpAttribute;
        private RvtExtApp.Components.Elements _CmpElements;
        private RvtExtApp.Components.Geometry _CmpGeometry;
        private RvtExtApp.Components.Parameters _CmpParameters;
        private RvtExtApp.Components.Settings _CmpSettings;
        private string _ErrMsg;
        private string _ColNameID;
        private string _ColNameName;

        protected DtBase(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.Elements cmpElements,
                         RvtExtApp.Components.Geometry cmpGeometry,
                         RvtExtApp.Components.Parameters cmpParameters,
                         RvtExtApp.Components.Settings cmpSettings)
        {
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _ErrMsg = "";
        }

        protected RvtExtApp.Components.Attribute CmpAttribute => _CmpAttribute;
        protected RvtExtApp.Components.Elements CmpElements => _CmpElements;
        protected RvtExtApp.Components.Geometry CmpGeometry => _CmpGeometry;
        protected RvtExtApp.Components.Parameters CmpParameters => _CmpParameters;
        protected RvtExtApp.Components.Settings CmpSettings => _CmpSettings;

        public string ErrMsg { get => _ErrMsg; set => _ErrMsg = value; }

        public string ColNameID
        {
            get
            {
                if (_ColNameID == null)
                    _ColNameID = _CmpAttribute.ResourceText("IDS_COLNAME_ID");
                return _ColNameID;
            }
        }

        public string ColNameName
        {
            get
            {
                if (_ColNameName == null)
                    _ColNameName = _CmpAttribute.ResourceText("IDS_COLNAME_NAME");
                return _ColNameName;
            }
        }
    }
}
