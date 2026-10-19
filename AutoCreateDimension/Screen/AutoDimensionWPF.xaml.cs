using ADSK.JExtRAC.AutoCreateDimension.Utils;
using ResText = ADSK.JExtRAC.AutoCreateDimension.Resources.Text;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ADSK.JExtRAC.AutoCreateDimension.Screen
{
    public partial class AutoDimensionWPF : Window, IWeaveChromeWindow
    {
        private readonly AutoDimension _logic;

        public AutoDimensionWPF(AutoDimension logic)
        {
            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");

            _logic = logic;

            WeaveTheme.Apply(this, this, ResText.IDS_DIM_FORM_TITLE);
            SetText();
            SetData();
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public System.Windows.Controls.Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        private void SetText()
        {
            lblPlacementType.Text = ResText.IDS_DIM_GROUPBOX_PLACEMENT_TYPE;
            rbCoreCore.Content = ResText.IDS_DIM_RADIO_CORE_CORE;
            rbFaceFace.Content = ResText.IDS_DIM_RADIO_FACE_FACE;
            rbFaceCore.Content = ResText.IDS_DIM_RADIO_FACE_CORE;
            txtDetailLine.Text = JoinResourceLines(ResText.IDS_DIM_CHECK_DETAIL_LINE_NOTE);
            btnOK.Content = ResText.IDS_TXT_OK;
            btnCancel.Content = ResText.IDS_TXT_CANCEL;
        }

        private void SetData()
        {
            switch (_logic.PlacementType)
            {
                case DimensionPlacementType.FaceFace:
                    rbFaceFace.IsChecked = true;
                    break;
                case DimensionPlacementType.FaceCore:
                    rbFaceCore.IsChecked = true;
                    break;
                default:
                    rbCoreCore.IsChecked = true;
                    break;
            }

            ckbDetailLine.IsChecked = _logic.CreateDetailLines;
            ckbDetailLine.IsEnabled = _logic.CanCreateDetailLines;
        }

        // The resource strings carry WinForms line breaks for the old fixed-width layout.
        private static string JoinResourceLines(string text)
        {
            string[] lines = (text ?? string.Empty)
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .ToArray();

            var sb = new StringBuilder();
            foreach (string line in lines)
            {
                if (sb.Length > 0 && !IsCjk(sb[sb.Length - 1]))
                    sb.Append(' ');
                sb.Append(line);
            }
            return sb.ToString();
        }

        private static bool IsCjk(char c) => c >= '\u2E80';

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DimensionPlacementType placement =
                rbFaceFace.IsChecked == true ? DimensionPlacementType.FaceFace :
                rbFaceCore.IsChecked == true ? DimensionPlacementType.FaceCore :
                DimensionPlacementType.CoreCore;

            _logic.ConfirmOptions(placement, ckbDetailLine.IsChecked == true);
            DialogResult = true;
            Close();
        }
    }
}
