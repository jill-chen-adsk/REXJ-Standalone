using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.GridDimension;

namespace ADSK.JExtRAC.GridDimension.Entities
{
    /// <summary>共有パラメータ - 基底</summary>
    public abstract class SpBase
    {
        private readonly RvtExtApp.Components.Parameters _CmpParameters;
        private readonly RvtExtApp.Components.Settings _CmpSettings;
        private bool _DefSuccess;

        protected SpBase(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.Parameters cmpParameters,
                         RvtExtApp.Components.Settings cmpSettings)
        {
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _DefSuccess = true;
        }

        protected RvtExtApp.Components.Parameters CmpParameters => _CmpParameters;

        protected RvtExtApp.Components.Settings CmpSettings => _CmpSettings;

        public bool DefSuccess
        {
            get => _DefSuccess;
            set => _DefSuccess = value;
        }
    }
}
