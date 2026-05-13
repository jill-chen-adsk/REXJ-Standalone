namespace ADSK.JExtRAC.LocateSlab.Entities
{
    public abstract class DtBase
    {
        private readonly Components.Attribute _cmpAttribute;
        private readonly Components.Elements _cmpElements;
        private readonly Components.Geometry _cmpGeometry;
        private readonly Components.Parameters _cmpParameters;
        private readonly Components.Settings _cmpSettings;
        private string _errMsg;
        private string _colNameID;
        private string _colNameName;

        protected DtBase(Components.Attribute cmpAttribute, Components.Elements cmpElements,
            Components.Geometry cmpGeometry, Components.Parameters cmpParameters, Components.Settings cmpSettings)
        {
            _cmpAttribute = cmpAttribute;
            _cmpElements = cmpElements;
            _cmpGeometry = cmpGeometry;
            _cmpParameters = cmpParameters;
            _cmpSettings = cmpSettings;
            _errMsg = "";
        }

        protected Components.Attribute CmpAttribute => _cmpAttribute;
        protected Components.Elements CmpElements => _cmpElements;
        protected Components.Geometry CmpGeometry => _cmpGeometry;
        protected Components.Parameters CmpParameters => _cmpParameters;
        protected Components.Settings CmpSettings => _cmpSettings;

        public string ErrMsg
        {
            get => _errMsg;
            set => _errMsg = value;
        }

        public string ColNameID => _colNameID ??= _cmpAttribute.ResourceText("IDS_COLNAME_ID");
        public string ColNameName => _colNameName ??= _cmpAttribute.ResourceText("IDS_COLNAME_NAME");
    }
}
