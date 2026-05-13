using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.GridDimension;

namespace ADSK.JExtRAC.GridDimension.Entities
{
    /// <summary>データテーブル - コマンド</summary>
    public class DtCmd : RvtExtApp.Entities.DtBase
    {
        private readonly RvtExtApp.Entities.SpCmd _EntSpCmd;
        private Collections.Generic.IList<string> _Data;

        public DtCmd(RvtExtApp.Components.Attribute cmpAttribute,
                     RvtExtApp.Components.Elements cmpElements,
                     RvtExtApp.Components.Geometry cmpGeometry,
                     RvtExtApp.Components.Parameters cmpParameters,
                     RvtExtApp.Components.Settings cmpSettings,
                     Revit.DB.ProjectInfo elemProjInfo,
                     string defName,
                     int itemNum)
               : base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            _EntSpCmd = new RvtExtApp.Entities.SpCmd(cmpAttribute,
                                                     cmpParameters,
                                                     cmpSettings,
                                                     elemProjInfo,
                                                     defName,
                                                     itemNum);
            if (_EntSpCmd.DefSuccess == false)
                base.ErrMsg = base.CmpAttribute.ResourceText("IDS_ERR_PARAMDEF");
            else
                _Data = _EntSpCmd.GetData();
        }

        public void SetData()
        {
            _EntSpCmd.SetData(_Data);
        }

        public Collections.Generic.IList<string> Data => _Data;
    }
}
