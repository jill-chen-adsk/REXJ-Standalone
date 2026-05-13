using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.LocateSlab.Entities
{
    public abstract class SpBase
    {
        private readonly Components.Attribute _cmpAttribute;
        private readonly Components.Parameters _cmpParameters;
        private readonly Components.Settings _cmpSettings;
        private bool _defSuccess;
        private string _errDefName;
        private string _defCatName;
        private Element _currentElem;

        protected SpBase(Components.Attribute cmpAttribute, Components.Parameters cmpParameters, Components.Settings cmpSettings)
        {
            _cmpAttribute = cmpAttribute;
            _cmpParameters = cmpParameters;
            _cmpSettings = cmpSettings;
            _defSuccess = true;
            _errDefName = "";
            _defCatName = "";
        }

        protected Components.Attribute CmpAttribute => _cmpAttribute;
        protected Components.Parameters CmpParameters => _cmpParameters;
        protected Components.Settings CmpSettings => _cmpSettings;

        protected void SetDefCatName(Category category)
        {
            if (!_defCatName.Contains(category.Name))
            {
                if (_defCatName != "") _defCatName += ",";
                _defCatName += category.Name;
            }
        }

        protected void SetDefCatName(IList<Category> categories)
        {
            foreach (var cat in categories) SetDefCatName(cat);
        }

        public bool DefSuccess { get => _defSuccess; set => _defSuccess = value; }
        public string ErrDefName { get => _errDefName; set => _errDefName = value; }
        public string DefCatName => _defCatName;

        public Element CurrentElem { get => _currentElem; set => _currentElem = value; }

        public string FamilyTypeName
        {
            get
            {
                string ret = "";
                if (CurrentElem != null)
                    _cmpParameters.GetValue(CurrentElem,
                        BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM, ref ret);
                return ret;
            }
        }
    }
}
