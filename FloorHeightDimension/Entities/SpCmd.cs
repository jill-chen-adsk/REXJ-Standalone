using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FloorHeightDimension;
using System.Linq;

namespace ADSK.JExtRAC.FloorHeightDimension.Entities
{
    public class SpCmd : RvtExtApp.Entities.SpBase
    {
        private Revit.DB.ProjectInfo _ElemProjInfo;
        private string _ParamNameCmd;
        private int _ItemNum;

        public SpCmd(RvtExtApp.Components.Attribute cmpAttribute,
                     RvtExtApp.Components.Parameters cmpParameters,
                     RvtExtApp.Components.Settings cmpSettings,
                     Revit.DB.ProjectInfo elemProjInfo,
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
                                                    Revit.DB.SpecTypeId.String.Text,
                                                    new Revit.DB.ForgeTypeId(string.Empty),
                                                    false,
                                                    0);
        }

        public Collections.Generic.IList<string> GetData()
        {
            string sValue = "";
            Collections.Generic.IList<string> valueSplit;

            Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

            if (base.CmpParameters.GetValue(_ElemProjInfo,
                                            _ParamNameCmd,
                                            Revit.DB.SpecTypeId.String.Text,
                                            new Revit.DB.ForgeTypeId(string.Empty),
                                            ref sValue) < -1)
            {
            }
            valueSplit = string.IsNullOrEmpty(sValue)
                ? new Collections.Generic.List<string>()
                : sValue.Split(',').Select(x => x.Trim()).ToList();

            bool flag = false;
            if (_ItemNum == valueSplit.Count)
            {
                flag = true;
            }

            if (_ItemNum > 0)
            {
                for (int i = 0; i < _ItemNum; ++i)
                {
                    if (flag == true)
                    {
                        ret.Add(valueSplit[i]);
                    }
                    else
                    {
                        ret.Add("");
                    }
                }
            }
            else
            {
                if (valueSplit.Count > 0)
                {
                    for (int i = 0; i < valueSplit.Count; ++i)
                    {
                        ret.Add(valueSplit[i]);
                    }
                }
            }
            return ret;
        }

        public bool SetData(Collections.Generic.IList<string> value)
        {
            string valueStr = null;
            string separator = ",";

            bool retVal = false;

            if (value != null)
            {
                foreach (string str in value)
                {
                    valueStr += str + separator;
                }
            }

            if (valueStr != null)
            {
                valueStr = valueStr.Substring(0, valueStr.Length - 1);
            }

            if (valueStr != null)
            {
                base.CmpParameters.SetValue(_ElemProjInfo,
                                            _ParamNameCmd,
                                            Revit.DB.SpecTypeId.String.Text,
                                            new Revit.DB.ForgeTypeId(string.Empty),
                                            valueStr);
                retVal = true;
            }
            return retVal;
        }
    }
}
