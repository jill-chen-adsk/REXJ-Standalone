using System;
using System.Collections.Generic;
using System.Data;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.LocateSlab.Entities
{
    public class DtCmd : DtBase
    {
        private readonly SpCmd _entSpCmd;
        private IList<string> _data;
        private DataTable _dataDirection;
        private double _degreeAngle;
        private readonly int _numberMin;
        private readonly int _numberMax;

        public DtCmd(Components.Attribute cmpAttribute, Components.Elements cmpElements,
            Components.Geometry cmpGeometry, Components.Parameters cmpParameters, Components.Settings cmpSettings,
            ProjectInfo elemProjInfo, string defName, int itemNum)
            : base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            _entSpCmd = new SpCmd(cmpAttribute, cmpParameters, cmpSettings,
                elemProjInfo, defName, itemNum);

            if (!_entSpCmd.DefSuccess)
                ErrMsg = cmpAttribute.ResourceText("IDS_ERR_PARAMDEF");
            else
                _data = _entSpCmd.GetData();

            _numberMin = -90;
            _numberMax = 90;
        }

        public void SetData()
        {
            _entSpCmd.SetData(_data);
        }

        public string SetErrPvdDecimalText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return CmpAttribute.ResourceText("IDS_ERR_VALNULL");

            if (!double.TryParse(value, out double dValue))
                return CmpAttribute.ResourceText("IDS_ERR_VALNUMBER");

            if (dValue < _numberMin || dValue > _numberMax)
                return CmpAttribute.ResourceText("IDS_ERR_VALOUT");

            return "";
        }

        public IList<string> Data => _data;

        public double DegreeAngle { get => _degreeAngle; set => _degreeAngle = value; }

        public DataTable DataDirection
        {
            get
            {
                if (_dataDirection == null)
                {
                    _dataDirection = new DataTable();
                    _dataDirection.Columns.Add("Name", typeof(string));
                    _dataDirection.Columns.Add("Value", typeof(string));

                    int[] angles = { -90, -75, -60, -45, -30, -15, 0, 15, 30, 45, 60, 75, 90 };
                    foreach (int a in angles)
                    {
                        var row = _dataDirection.NewRow();
                        row["Name"] = a.ToString();
                        row["Value"] = a.ToString();
                        _dataDirection.Rows.Add(row);
                    }
                }
                return _dataDirection;
            }
        }

        public int NumberMin => _numberMin;
        public int NumberMax => _numberMax;
    }
}
