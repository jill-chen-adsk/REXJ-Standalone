namespace ADSK.JExtRAC.ExportExcel.Entities
{
    public class ObjectItem
    {
        private string _Name = null;

        public ObjectItem(string name)
        {
            this._Name = name;
        }

        public override string ToString()
        {
            return this._Name;
        }
    }
}
