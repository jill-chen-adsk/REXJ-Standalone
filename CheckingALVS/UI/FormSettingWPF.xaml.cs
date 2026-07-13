using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ADSK.JExtRAC.CheckingALVS.UI.Controls;
using ADSK.JExtRAC.CheckingALVS.Utils;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;

namespace ADSK.JExtRAC.CheckingALVS.UI
{
    public partial class FormSettingWPF : Window, IWeaveChromeWindow
    {
        sealed class SectionBinding
        {
            public RoundingSectionPanel Panel;
            public Func<int> GetDecimal;
            public Action<int> SetDecimal;
            public Func<int> GetOption;
            public Action<int> SetOption;
            public int DefaultInitFlag;
        }

        readonly RvtExtApp.Components.Attribute _cmpAttribute;
        readonly RvtExtApp.Entities.DtCmd _entDtCmd;
        readonly List<SectionBinding> _activeSections = new List<SectionBinding>();

        public FormSettingWPF(int commandKind,
                              RvtExtApp.Components.Attribute cmpAttribute,
                              RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();

            _cmpAttribute = cmpAttribute;
            _entDtCmd = entDtCmd;
            _entDtCmd.CommandKind = commandKind;

            string title = CheckingCommandTitles.GetCommandTitle(_cmpAttribute, commandKind);
            WeaveTheme.Apply(this, this, title, CancelDialog);

            ConfigureSections();
            ApplyCommandKindLayout();
            SetText();
            SetData();

            btnOK.Click += BtnOK_Click;
            btnCancel.Click += (_, __) => CancelDialog();
        }

        public System.Windows.Controls.Border ChromeOuterBorder => chromeOuterBorder;
        public System.Windows.Controls.Grid ChromeTitleBar => chromeTitleBar;
        public System.Windows.Controls.Border ChromeDivider => chromeDivider;
        public System.Windows.Controls.TextBlock ChromeTitleText => chromeTitleText;
        public System.Windows.Controls.Button ChromeCloseButton => chromeCloseButton;

        void ConfigureSections()
        {
            BindSection(sectionLegal, "legal", 1,
                () => _entDtCmd.LegalAreaRoundingDecimal,
                v => _entDtCmd.LegalAreaRoundingDecimal = v,
                () => _entDtCmd.LegalAreaRoundingOpt,
                v => _entDtCmd.LegalAreaRoundingOpt = v);

            BindSection(sectionAreaToGetLight, "areaToGetLight", 2,
                () => _entDtCmd.AreaToGetLightRoundingDecimal,
                v => _entDtCmd.AreaToGetLightRoundingDecimal = v,
                () => _entDtCmd.AreaToGetLightRoundingOpt,
                v => _entDtCmd.AreaToGetLightRoundingOpt = v);

            BindSection(sectionDh, "dh", 3,
                () => _entDtCmd.DHRoundingDecimal,
                v => _entDtCmd.DHRoundingDecimal = v,
                () => _entDtCmd.DHRoundingOpt,
                v => _entDtCmd.DHRoundingOpt = v);

            BindSection(sectionEffectiveOpening, "effectiveOpening", 4,
                () => _entDtCmd.EffectiveOpeningAreaRoundingDecimal,
                v => _entDtCmd.EffectiveOpeningAreaRoundingDecimal = v,
                () => _entDtCmd.EffectiveOpeningAreaRoundingOpt,
                v => _entDtCmd.EffectiveOpeningAreaRoundingOpt = v);

            BindSection(sectionEffectiveLighting, "effectiveLighting", 5,
                () => _entDtCmd.EffectiveLightingAreaRoundingDecimal,
                v => _entDtCmd.EffectiveLightingAreaRoundingDecimal = v,
                () => _entDtCmd.EffectiveLightingAreaRoundingOpt,
                v => _entDtCmd.EffectiveLightingAreaRoundingOpt = v);

            BindSection(sectionAreaToBeSmoked, "areaToBeSmoked", 6,
                () => _entDtCmd.AreaToBeSmokedRoundingDecimal,
                v => _entDtCmd.AreaToBeSmokedRoundingDecimal = v,
                () => _entDtCmd.AreaToBeSmokedRoundingOtp,
                v => _entDtCmd.AreaToBeSmokedRoundingOtp = v);

            BindSection(sectionEffectiveSmoke, "effectiveSmoke", 7,
                () => _entDtCmd.EffectiveSmokeExtractionAreaRoundingDecimal,
                v => _entDtCmd.EffectiveSmokeExtractionAreaRoundingDecimal = v,
                () => _entDtCmd.EffectiveSmokeExtractionAreaRoundingOtp,
                v => _entDtCmd.EffectiveSmokeExtractionAreaRoundingOtp = v);

            BindSection(sectionAreaToBeVentilated, "areaToBeVentilated", 8,
                () => _entDtCmd.AreaToBeVentilatedRoundingDecimal,
                v => _entDtCmd.AreaToBeVentilatedRoundingDecimal = v,
                () => _entDtCmd.AreaToBeVentilatedRoundingOtp,
                v => _entDtCmd.AreaToBeVentilatedRoundingOtp = v);

            BindSection(sectionEffectiveVentilation, "effectiveVentilation", 9,
                () => _entDtCmd.EffectiveVentilationAreaRoundingDecimal,
                v => _entDtCmd.EffectiveVentilationAreaRoundingDecimal = v,
                () => _entDtCmd.EffectiveVentilationAreaRoundingOtp,
                v => _entDtCmd.EffectiveVentilationAreaRoundingOtp = v);
        }

        void BindSection(
            RoundingSectionPanel panel,
            string radioGroup,
            int defaultInitFlag,
            Func<int> getDecimal,
            Action<int> setDecimal,
            Func<int> getOption,
            Action<int> setOption)
        {
            panel.ConfigureRadioGroup(radioGroup);
            panel.DefaultInitFlag = defaultInitFlag;
            panel.LostFocusValidation += (_, __) =>
                panel.ValidateNumeric(value => _entDtCmd.SetErrPvdNumeric(value, true));
            panel.DefaultClick += (_, __) => ApplyDefault(panel);

            _activeSections.Add(new SectionBinding
            {
                Panel = panel,
                GetDecimal = getDecimal,
                SetDecimal = setDecimal,
                GetOption = getOption,
                SetOption = setOption,
                DefaultInitFlag = defaultInitFlag
            });
        }

        void ApplyCommandKindLayout()
        {
            switch (_entDtCmd.CommandKind)
            {
                case 0:
                    SetSectionVisibility(sectionLegal, true);
                    SetSectionVisibility(sectionAreaToGetLight, true);
                    SetSectionVisibility(sectionDh, true);
                    SetSectionVisibility(sectionEffectiveOpening, true);
                    SetSectionVisibility(sectionEffectiveLighting, true);
                    SetSectionVisibility(sectionAreaToBeSmoked, false);
                    SetSectionVisibility(sectionEffectiveSmoke, false);
                    SetSectionVisibility(sectionAreaToBeVentilated, false);
                    SetSectionVisibility(sectionEffectiveVentilation, false);
                    MinHeight = 420;
                    Height = 640;
                    break;

                case 1:
                    SetSectionVisibility(sectionLegal, true);
                    SetSectionVisibility(sectionAreaToGetLight, false);
                    SetSectionVisibility(sectionDh, false);
                    SetSectionVisibility(sectionEffectiveOpening, false);
                    SetSectionVisibility(sectionEffectiveLighting, false);
                    SetSectionVisibility(sectionAreaToBeSmoked, true);
                    SetSectionVisibility(sectionEffectiveSmoke, true);
                    SetSectionVisibility(sectionAreaToBeVentilated, false);
                    SetSectionVisibility(sectionEffectiveVentilation, false);
                    MinHeight = 380;
                    Height = 480;
                    break;

                case 2:
                    SetSectionVisibility(sectionLegal, true);
                    SetSectionVisibility(sectionAreaToGetLight, false);
                    SetSectionVisibility(sectionDh, false);
                    SetSectionVisibility(sectionEffectiveOpening, false);
                    SetSectionVisibility(sectionEffectiveLighting, false);
                    SetSectionVisibility(sectionAreaToBeSmoked, false);
                    SetSectionVisibility(sectionEffectiveSmoke, false);
                    SetSectionVisibility(sectionAreaToBeVentilated, true);
                    SetSectionVisibility(sectionEffectiveVentilation, true);
                    MinHeight = 380;
                    Height = 480;
                    break;
            }
        }

        static void SetSectionVisibility(RoundingSectionPanel panel, bool visible)
        {
            panel.Visibility = visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        void SetText()
        {
            string decimals = _cmpAttribute.ResourceText("IDS_TXT_DECIMAL");
            string order = _cmpAttribute.ResourceText("IDS_TXT_ORDER");
            string cutoff = _cmpAttribute.ResourceText("IDS_TXT_CUTOFF");
            string close = _cmpAttribute.ResourceText("IDS_TXT_CLOS");
            string safeSide = _cmpAttribute.ResourceText("IDS_TXT_SAFE_SIDE");
            string roundOff = _cmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");
            string defaultText = _cmpAttribute.ResourceText("IDS_TXT_DEFAULT");

            ConfigureSectionText(sectionLegal, _cmpAttribute.ResourceText("IDS_TXT_LEGALAREA"),
                decimals, order, cutoff, close + safeSide, roundOff, defaultText);

            switch (_entDtCmd.CommandKind)
            {
                case 0:
                    ConfigureSectionText(sectionAreaToGetLight, _cmpAttribute.ResourceText("IDS_TXT_LIGHTINGNESAREA"),
                        decimals, order, cutoff, close + safeSide, roundOff, defaultText);
                    ConfigureSectionText(sectionDh, _cmpAttribute.ResourceText("IDS_TXT_COMMON"),
                        decimals, order, cutoff + safeSide, close, roundOff, defaultText);
                    ConfigureSectionText(sectionEffectiveOpening, _cmpAttribute.ResourceText("IDS_TXT_OPENINGUSABLEAREA"),
                        decimals, order, cutoff + safeSide, close, roundOff, defaultText);
                    ConfigureSectionText(sectionEffectiveLighting, _cmpAttribute.ResourceText("IDS_TXT_LIGHTINGUSABLEAREA"),
                        decimals, order, cutoff + safeSide, close, roundOff, defaultText);
                    break;

                case 1:
                    ConfigureSectionText(sectionAreaToBeSmoked, _cmpAttribute.ResourceText("IDS_TXT_SMOKENESAREA"),
                        decimals, order, cutoff, close + safeSide, roundOff, defaultText);
                    ConfigureSectionText(sectionEffectiveSmoke, _cmpAttribute.ResourceText("IDS_TXT_WINDOWSMOKEUSABLEAREA"),
                        decimals, order, cutoff + safeSide, close, roundOff, defaultText);
                    break;

                case 2:
                    ConfigureSectionText(sectionAreaToBeVentilated, _cmpAttribute.ResourceText("IDS_TXT_VENTILATIONNESAREA"),
                        decimals, order, cutoff, close + safeSide, roundOff, defaultText);
                    ConfigureSectionText(sectionEffectiveVentilation, _cmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEAREA"),
                        decimals, order, cutoff + safeSide, close, roundOff, defaultText);
                    break;
            }

            btnOK.Content = _cmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _cmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        static void ConfigureSectionText(
            RoundingSectionPanel panel,
            string title,
            string decimalLabel,
            string orderLabel,
            string cutText,
            string closeText,
            string roundingText,
            string defaultText)
        {
            panel.SetSectionTitle(title);
            panel.SetDecimalLabel(decimalLabel);
            panel.SetOrderLabel(orderLabel);
            panel.SetCutText(cutText);
            panel.SetCloseText(closeText);
            panel.SetRoundingText(roundingText);
            panel.SetDefaultButtonText(defaultText);
        }

        void SetData()
        {
            foreach (SectionBinding binding in _activeSections)
            {
                if (binding.Panel.Visibility != System.Windows.Visibility.Visible)
                    continue;

                binding.Panel.DecimalText = binding.GetDecimal().ToString();
                binding.Panel.RoundingOption = binding.GetOption();
                binding.Panel.SetError(string.Empty);
            }
        }

        void GetData()
        {
            foreach (SectionBinding binding in _activeSections)
            {
                if (binding.Panel.Visibility != System.Windows.Visibility.Visible)
                    continue;

                binding.SetDecimal(int.Parse(binding.Panel.DecimalText));
                binding.SetOption(binding.Panel.RoundingOption);
            }
        }

        bool ValidateAll()
        {
            bool valid = true;

            foreach (SectionBinding binding in _activeSections)
            {
                if (binding.Panel.Visibility != System.Windows.Visibility.Visible)
                    continue;

                string error = binding.Panel.ValidateNumeric(value => _entDtCmd.SetErrPvdNumeric(value, true));
                if (!string.IsNullOrEmpty(error))
                    valid = false;
            }

            return valid;
        }

        void ApplyDefault(RoundingSectionPanel panel)
        {
            _entDtCmd.Initvalue(panel.DefaultInitFlag);

            foreach (SectionBinding binding in _activeSections)
            {
                if (!ReferenceEquals(binding.Panel, panel))
                    continue;

                binding.Panel.DecimalText = binding.GetDecimal().ToString();
                binding.Panel.RoundingOption = binding.GetOption();
                binding.Panel.SetError(string.Empty);
                return;
            }
        }

        void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAll())
                return;

            GetData();
            DialogResult = true;
            Close();
        }

        void CancelDialog()
        {
            DialogResult = false;
            Close();
        }
    }
}
