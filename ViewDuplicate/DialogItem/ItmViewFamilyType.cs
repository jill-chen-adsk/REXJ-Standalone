using Autodesk.Revit.DB;
using R = ADSK.ViewExtension.ViewDuplicate.Resources;

namespace ADSK.ViewExtension.ViewDuplicate.DialogItem
{
    public class ItmViewFamilyType
    {
        private ViewFamilyType _viewFamilyType;

        public ItmViewFamilyType(ViewFamilyType myVft)
        {
            _viewFamilyType = myVft;
        }

        public ItmViewFamilyType()
        {
            _viewFamilyType = null;
        }

        public ViewFamilyType ViewfamilyType
        {
            get => _viewFamilyType;
            set => _viewFamilyType = value;
        }

        public override string ToString()
        {
            if (_viewFamilyType == null)
                return R.Text.TXT_VIEWFAMILY_ALL;
            return _viewFamilyType.Name;
        }

        public ElementId AppliedViewTemplateId
        {
            get
            {
                if (_viewFamilyType == null)
                    return ElementId.InvalidElementId;
                try
                {
                    Parameter prmDefVt = _viewFamilyType.get_Parameter(BuiltInParameter.DEFAULT_VIEW_TEMPLATE);
                    if (prmDefVt != null && prmDefVt.HasValue)
                        return prmDefVt.AsElementId();
                    return ElementId.InvalidElementId;
                }
                catch
                {
                    return ElementId.InvalidElementId;
                }
            }
        }
    }
}
