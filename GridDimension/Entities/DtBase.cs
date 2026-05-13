using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.GridDimension;

namespace ADSK.JExtRAC.GridDimension.Entities
{
    /// <summary>データテーブル - 基底</summary>
    public abstract class DtBase
    {
        private readonly RvtExtApp.Components.Attribute _CmpAttribute;
        private readonly RvtExtApp.Components.Elements _CmpElements;
        private readonly RvtExtApp.Components.Geometry _CmpGeometry;
        private readonly RvtExtApp.Components.Parameters _CmpParameters;
        private readonly RvtExtApp.Components.Settings _CmpSettings;
        private string _ErrMsg;

        protected DtBase(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.Elements cmpElements,
                         RvtExtApp.Components.Geometry cmpGeometry,
                         RvtExtApp.Components.Parameters cmpParameters,
                         RvtExtApp.Components.Settings cmpSettings)
        {
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _ErrMsg = "";
        }

        protected RvtExtApp.Components.Attribute CmpAttribute => _CmpAttribute;

        protected RvtExtApp.Components.Elements CmpElements => _CmpElements;

        protected RvtExtApp.Components.Geometry CmpGeometry => _CmpGeometry;

        protected RvtExtApp.Components.Parameters CmpParameters => _CmpParameters;

        protected RvtExtApp.Components.Settings CmpSettings => _CmpSettings;

        public string ErrMsg
        {
            get => _ErrMsg;
            set => _ErrMsg = value;
        }
    }
}
