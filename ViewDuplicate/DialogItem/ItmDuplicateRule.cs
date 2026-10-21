using R = ADSK.ViewExtension.ViewDuplicate.Resources;

namespace ADSK.ViewExtension.ViewDuplicate.DialogItem
{
    public class ItmDuplicateRule
    {
        public ItmDuplicateRule(bool isPrefix, string addedText, ItmView viewTemplateItem, ItmViewFamilyType viewFamilyTypeItem)
        {
            IsPrefix = isPrefix;
            AddedText = addedText;
            ViewTemplateItem = viewTemplateItem;
            ViewFamilyTypeItem = viewFamilyTypeItem;
        }

        public bool IsPrefix { get; }

        public string PositionText => IsPrefix ? R.Text.TXT_PREFIX : R.Text.TXT_SUFFIX;

        public string AddedText { get; set; }

        /// <summary>Null when no view template applies to the chosen view type.</summary>
        public ItmView ViewTemplateItem { get; }

        public string ViewTemplateText => ViewTemplateItem?.ToString() ?? R.Text.TXT_NO_TEMPLATE;

        public ItmViewFamilyType ViewFamilyTypeItem { get; }

        public string ViewFamilyTypeText => ViewFamilyTypeItem.ToString();
    }
}
