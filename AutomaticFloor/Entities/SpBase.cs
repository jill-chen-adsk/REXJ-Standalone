using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RvtExtApp = ADSK.JExtRAC.AutomaticFloor;

namespace ADSK.JExtRAC.AutomaticFloor.Entities
{
    public abstract class SpBase
    {
        private RvtExtApp.Components.Attribute _CmpAttribute;
        private RvtExtApp.Components.Parameters _CmpParameters;
        private RvtExtApp.Components.Settings _CmpSettings;
        private bool _DefSuccess;
        private string _ErrDefName;
        private string _DefCatName;
        private Element _CurrentElem;

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

        protected void SetDefCatName(Category category)
        {
            var names = string.IsNullOrEmpty(_DefCatName) ? new List<string>() : _DefCatName.Split(',').ToList();
            if (!names.Contains(category.Name))
            {
                if (_DefCatName != "") _DefCatName += ",";
                _DefCatName += category.Name;
            }
        }

        protected void SetDefCatName(IList<Category> categories)
        {
            foreach (Category category in categories)
                SetDefCatName(category);
        }

        protected RvtExtApp.Components.Attribute CmpAttribute => _CmpAttribute;
        protected RvtExtApp.Components.Parameters CmpParameters => _CmpParameters;
        protected RvtExtApp.Components.Settings CmpSettings => _CmpSettings;

        public bool DefSuccess { get => _DefSuccess; set => _DefSuccess = value; }
        public string ErrDefName { get => _ErrDefName; set => _ErrDefName = value; }
        public string DefCatName => _DefCatName;
        public Element CurrentElem { get => _CurrentElem; set => _CurrentElem = value; }

        public string FamilyTypeName
        {
            get
            {
                string ret = "";
                if (CurrentElem != null)
                    CmpParameters.GetValue(CurrentElem, BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM, ref ret);
                return ret;
            }
        }
    }
}
