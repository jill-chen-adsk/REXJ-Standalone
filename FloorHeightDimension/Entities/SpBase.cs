using System;
using System.Collections.Generic;
using System.Linq;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FloorHeightDimension;

namespace ADSK.JExtRAC.FloorHeightDimension.Entities
{
    public abstract class SpBase
    {
        private RvtExtApp.Components.Attribute _CmpAttribute;
        private RvtExtApp.Components.Parameters _CmpParameters;
        private RvtExtApp.Components.Settings _CmpSettings;
        private bool _DefSuccess;
        private string _ErrDefName;
        private string _DefCatName;
        private Revit.DB.Element _CurrentElem;

        protected SpBase(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.Parameters cmpParameters,
                         RvtExtApp.Components.Settings cmpSettings)
        {
            _CmpAttribute = cmpAttribute;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _DefSuccess = true;
            _ErrDefName = "";
            _DefCatName = "";
        }

        protected void SetDefCatName(Revit.DB.Category category)
        {
            IList<string> defCatNames = string.IsNullOrEmpty(_DefCatName)
                ? new List<string>()
                : _DefCatName.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).ToList();
            if (!defCatNames.Contains(category.Name))
            {
                if (_DefCatName != "")
                {
                    _DefCatName += ",";
                }
                _DefCatName += category.Name;
            }
        }

        protected void SetDefCatName(IList<Revit.DB.Category> categories)
        {
            foreach (Revit.DB.Category category in categories)
            {
                SetDefCatName(category);
            }
        }

        protected RvtExtApp.Components.Attribute CmpAttribute => _CmpAttribute;
        protected RvtExtApp.Components.Parameters CmpParameters => _CmpParameters;
        protected RvtExtApp.Components.Settings CmpSettings => _CmpSettings;

        public bool DefSuccess
        {
            get => _DefSuccess;
            set => _DefSuccess = value;
        }

        public string ErrDefName
        {
            get => _ErrDefName;
            set => _ErrDefName = value;
        }

        public string DefCatName => _DefCatName;

        public Revit.DB.Element CurrentElem
        {
            get => _CurrentElem;
            set => _CurrentElem = value;
        }
    }
}
