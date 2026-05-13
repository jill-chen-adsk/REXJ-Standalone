using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.LocateSlab.Entities
{
    public class SpCmd : SpBase
    {
        private readonly ProjectInfo _elemProjInfo;
        private readonly string _paramNameCmd;
        private readonly int _itemNum;

        public SpCmd(Components.Attribute cmpAttribute, Components.Parameters cmpParameters, Components.Settings cmpSettings,
            ProjectInfo elemProjInfo, string defName, int itemNum)
            : base(cmpAttribute, cmpParameters, cmpSettings)
        {
            _elemProjInfo = elemProjInfo;
            _paramNameCmd = defName;
            _itemNum = itemNum;
            DefSuccess = SetDef();
        }

        private bool SetDef()
        {
            return CmpParameters.SetDefinition(null,
                CmpSettings.CategoryProjInfo,
                _paramNameCmd,
                SpecTypeId.String.Text,
                new ForgeTypeId(string.Empty),
                false, 0);
        }

        public IList<string> GetData()
        {
            string sValue = "";
            var ret = new List<string>();

            CmpParameters.GetValue(_elemProjInfo, _paramNameCmd,
                SpecTypeId.String.Text, new ForgeTypeId(string.Empty), ref sValue);

            var valueSplit = SplitString(sValue, ",");
            bool flag = _itemNum == valueSplit.Count;

            if (_itemNum > 0)
            {
                for (int i = 0; i < _itemNum; ++i)
                    ret.Add(flag ? valueSplit[i] : "");
            }
            else
            {
                foreach (var v in valueSplit)
                    ret.Add(v);
            }
            return ret;
        }

        public bool SetData(IList<string> value)
        {
            if (value == null) return false;
            string valueStr = string.Join(",", value);
            CmpParameters.SetValue(_elemProjInfo, _paramNameCmd,
                SpecTypeId.String.Text, new ForgeTypeId(string.Empty), valueStr);
            return true;
        }

        private static IList<string> SplitString(string input, string separator)
        {
            var ret = new List<string>();
            if (string.IsNullOrEmpty(input)) return ret;
            var parts = input.Split(new[] { separator }, StringSplitOptions.None);
            foreach (var p in parts) ret.Add(p);
            return ret;
        }
    }
}
