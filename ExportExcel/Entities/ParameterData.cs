namespace ADSK.JExtRAC.ExportExcel.Entities
{
    public class ParameterData : ObjectItem
    {
        public const int _NotExport = -1;

        private string _OriginalName = null;

        public int _IndexExport = _NotExport;

        public string OrignalName
        {
            get
            {
                return _OriginalName;
            }
        }

        public ParameterData(string originalName, string name) : base(name)
        {
            this._OriginalName = originalName;
        }
    }
}
