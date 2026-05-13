using System.Collections.Generic;
using Autodesk.Revit.DB;
using RvtExtApp = ADSK.JExtRAC.AutomaticFloor;

namespace ADSK.JExtRAC.AutomaticFloor.Entities
{
    public class SpCmd : SpBase
    {
        private ProjectInfo _ElemProjInfo;
        private string _ParamNameCmd;
        private int _ItemNum;

        public SpCmd(RvtExtApp.Components.Attribute cmpAttribute,
                     RvtExtApp.Components.Parameters cmpParameters,
                     RvtExtApp.Components.Settings cmpSettings,
                     ProjectInfo elemProjInfo,
                     string defName,
                     int itemNum) :
               base(cmpAttribute, cmpParameters, cmpSettings)
        {
            _ElemProjInfo = elemProjInfo;
            _ParamNameCmd = defName;
            _ItemNum = itemNum;
            base.DefSuccess = SetDef();
        }

        private bool SetDef()
        {
            return base.CmpParameters.SetDefinition(null,
                                                    base.CmpSettings.CategoryProjInfo,
                                                    _ParamNameCmd,
                                                    SpecTypeId.String.Text,
                                                    new ForgeTypeId(string.Empty),
                                                    false, 0);
        }

        public IList<string> GetData()
        {
            string sValue = "";
            base.CmpParameters.GetValue(_ElemProjInfo, _ParamNameCmd,
                SpecTypeId.String.Text, new ForgeTypeId(string.Empty), ref sValue);

            var valueSplit = string.IsNullOrEmpty(sValue)
                ? new List<string>()
                : new List<string>(sValue.Split(','));

            bool flag = _ItemNum == valueSplit.Count;
            IList<string> ret = new List<string>();

            if (_ItemNum > 0)
            {
                for (int i = 0; i < _ItemNum; ++i)
                    ret.Add(flag ? valueSplit[i] : "");
            }
            else
            {
                foreach (string s in valueSplit)
                    ret.Add(s);
            }
            return ret;
        }

        public bool SetData(IList<string> value)
        {
            if (value == null) return false;
            string valueStr = string.Join(",", value);

            base.CmpParameters.SetValue(_ElemProjInfo, _ParamNameCmd,
                SpecTypeId.String.Text, new ForgeTypeId(string.Empty), valueStr);
            return true;
        }
    }
}
