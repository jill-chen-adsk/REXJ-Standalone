using System.Collections.Generic;
using Autodesk.Revit.DB;
using RvtExtApp = ADSK.JExtRAC.AutomaticFloor;

namespace ADSK.JExtRAC.AutomaticFloor.Entities
{
    public class DtCmd : DtBase
    {
        private SpCmd _EntSpCmd;
        private IList<string> _Data;
        private System.Data.DataTable _DataDirection;
        private double _DegreeAngle;
        private int _numberMin;
        private int _numberMax;

        public DtCmd(RvtExtApp.Components.Attribute cmpAttribute,
                     RvtExtApp.Components.Elements cmpElements,
                     RvtExtApp.Components.Geometry cmpGeometry,
                     RvtExtApp.Components.Parameters cmpParameters,
                     RvtExtApp.Components.Settings cmpSettings,
                     ProjectInfo elemProjInfo,
                     string defName,
                     int itemNum) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            _EntSpCmd = new SpCmd(cmpAttribute, cmpParameters, cmpSettings, elemProjInfo, defName, itemNum);
            if (!_EntSpCmd.DefSuccess)
                base.ErrMsg = cmpAttribute.ResourceText("IDS_ERR_PARAMDEF");
            else
            {
                _Data = _EntSpCmd.GetData();
                _numberMin = -90;
                _numberMax = 90;
            }
        }

        public void SetData() { _EntSpCmd.SetData(_Data); }

        public string SetErrPvdDecimalText(string value)
        {
            string errMsg = "";
            if (string.IsNullOrEmpty(value))
                errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALNULL");

            if (errMsg == "" && !double.TryParse(value, out _))
                errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALNUMBER");

            if (errMsg == "")
            {
                double iValue = double.Parse(value);
                if (iValue < NumberMin || iValue > NumberMax)
                    errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALOUT");
            }
            return errMsg;
        }

        public IList<string> Data => _Data;
        public double DegreeAngle { get => _DegreeAngle; set => _DegreeAngle = value; }
        public int NumberMin => _numberMin;
        public int NumberMax => _numberMax;

        public System.Data.DataTable DataDirection
        {
            get
            {
                if (_DataDirection == null)
                {
                    _DataDirection = new System.Data.DataTable();
                    _DataDirection.Columns.Add("Name", typeof(string));
                    _DataDirection.Columns.Add("Value", typeof(string));
                    for (int angle = -90; angle <= 90; angle += 15)
                    {
                        var row = _DataDirection.NewRow();
                        row["Name"] = angle.ToString();
                        row["Value"] = angle.ToString();
                        _DataDirection.Rows.Add(row);
                    }
                }
                return _DataDirection;
            }
        }
    }
}
