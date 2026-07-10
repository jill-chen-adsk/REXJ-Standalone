using System;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using RvtExtApp = ADSK.JExtRAC.AreaSchedule;
using ADSK.JExtRAC.AreaSchedule.Utils;

namespace ADSK.JExtRAC.AreaSchedule.RoomConvertedToArea
{
    public partial class FormChoiceWorkWPF : Window, IWeaveChromeWindow
    {
        private readonly RvtExtApp.Components.Attribute _CmpAttribute;
        private readonly RvtExtApp.Entities.DtRoom _EntDtRoom;
        private readonly RvtExtApp.Entities.DtArea _EntDtArea;
        private readonly RvtExtApp.Entities.DtCmd _EntDtCmd;
        private string _tagIdColumn = string.Empty;
        private string _tagNameColumn = string.Empty;

        public FormChoiceWorkWPF(
            RvtExtApp.Components.Attribute cmpAttribute,
            RvtExtApp.Entities.DtRoom entDtRoom,
            RvtExtApp.Entities.DtArea entDtArea,
            RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;
            _EntDtRoom = entDtRoom;
            _EntDtArea = entDtArea;
            _EntDtCmd = entDtCmd;

            WeaveTheme.Apply(this, this, WeaveCommandTitles.RoomToArea(_CmpAttribute), CancelDialog);

            btnCancel.Command = new RoutedCommand();
            btnCancel.CommandBindings.Add(new CommandBinding(btnCancel.Command, BtnCancel_Executed));

            Loaded += FormChoiceWorkWPF_Loaded;
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void FormChoiceWorkWPF_Loaded(object sender, RoutedEventArgs e)
        {
            SetText();
            SetData();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            GetData();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelDialog();
        }

        private void BtnCancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CancelDialog();
        }

        private void CancelDialog()
        {
            DialogResult = false;
            Close();
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        private void ChkConvertArea_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ChangeTagPanel(chkConvertArea.IsChecked == true);
        }

        private void SetText()
        {
            WeaveWindowChrome.SetTitle(this, this, WeaveCommandTitles.RoomToArea(_CmpAttribute));

            chkConvertArea.Content = _CmpAttribute.ResourceText("IDS_TXT_CONVERTAREABOUNDARY") + " (_A)";
            lblAreaCalcSection.Text = _CmpAttribute.ResourceText("IDS_TXT_AREACALC");
            rdbWallFinish.Content = _CmpAttribute.ResourceText("IDS_TXT_WALLFINISH") + " (_F)";
            rdbWallCenter.Content = _CmpAttribute.ResourceText("IDS_TXT_WALLCENTER") + " (_N)";
            rdbWallCoreLayer.Content = _CmpAttribute.ResourceText("IDS_TXT_WALLCORELAYER") + " (_L)";
            rdbWallCoreCenter.Content = _CmpAttribute.ResourceText("IDS_TXT_WALLCORECENTER") + " (_C)";
            chkAddAreaTag.Content = _CmpAttribute.ResourceText("IDS_TXT_ADDAREATAG") + " (_T)";
            lblTag.Text = _CmpAttribute.ResourceText("IDS_TXT_TAG");
            lblTagNameSection.Text = _CmpAttribute.ResourceText("IDS_TXT_NAME");
            rdbUseRoomName.Content = _CmpAttribute.ResourceText("IDS_TXT_USEROOMNAME");
            rdbUseRoomNo.Content = _CmpAttribute.ResourceText("IDS_TXT_USEROOMNO");
            btnOK.Content = _CmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _CmpAttribute.ResourceText("IDS_TXT_CANCEL") + " (_C)";
        }

        private void SetData()
        {
            chkConvertArea.IsChecked = _EntDtRoom.ChkConvertArea;
            chkAddAreaTag.IsChecked = _EntDtArea.ChkAddAreaTag;
            RdbAreaCalc = _EntDtRoom.GetRoomBndLocTypeNo();
            RdbTagName = _EntDtArea.TagNameOpt;

            DataTable tags = _EntDtArea.DataAreaTags;
            if (tags != null && tags.Columns.Count >= 2)
            {
                _tagIdColumn = tags.Columns[0].ColumnName;
                _tagNameColumn = tags.Columns[1].ColumnName;
                cboTag.ItemsSource = tags.DefaultView;
                cboTag.DisplayMemberPath = _tagNameColumn;

                if (tags.DefaultView.Count > 0)
                {
                    SelectTag(_EntDtArea.TagID);
                    if (cboTag.SelectedIndex < 0)
                        cboTag.SelectedIndex = 0;
                }
            }

            ChangeTagPanel(chkConvertArea.IsChecked == true);
        }

        private void SelectTag(int tagId)
        {
            if (cboTag.ItemsSource is not DataView view)
                return;

            foreach (DataRowView row in view)
            {
                if (Convert.ToInt32(row[_tagIdColumn]) == tagId)
                {
                    cboTag.SelectedItem = row;
                    return;
                }
            }
        }

        private void GetData()
        {
            _EntDtRoom.ChkConvertArea = chkConvertArea.IsChecked == true;
            _EntDtArea.ChkAddAreaTag = chkAddAreaTag.IsChecked == true;
            _EntDtRoom.SetRoomBndLocType(RdbAreaCalc);
            _EntDtArea.TagNameOpt = RdbTagName;
            _EntDtArea.TagID = -1;

            if (cboTag.SelectedItem is DataRowView selectedRow)
                _EntDtArea.TagID = Convert.ToInt32(selectedRow[_tagIdColumn]);

            _EntDtCmd.Data[0] = Convert.ToByte(_EntDtRoom.ChkConvertArea).ToString();
            _EntDtCmd.Data[1] = Convert.ToByte(_EntDtArea.ChkAddAreaTag).ToString();
            _EntDtCmd.Data[2] = _EntDtArea.TagID.ToString();
            _EntDtCmd.Data[3] = _EntDtArea.TagNameOpt.ToString();
        }

        private void ChangeTagPanel(bool enabled)
        {
            bool allowTag = enabled;

            if (cboTag.Items.Count == 0)
                allowTag = false;

            if (!allowTag)
                chkAddAreaTag.IsChecked = false;

            chkAddAreaTag.IsEnabled = allowTag;
            gpbTag.IsEnabled = allowTag;
        }

        private int RdbAreaCalc
        {
            get
            {
                if (rdbWallFinish.IsChecked == true)
                    return 0;
                if (rdbWallCenter.IsChecked == true)
                    return 1;
                if (rdbWallCoreLayer.IsChecked == true)
                    return 2;
                if (rdbWallCoreCenter.IsChecked == true)
                    return 3;
                return 0;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        rdbWallFinish.IsChecked = true;
                        break;
                    case 1:
                        rdbWallCenter.IsChecked = true;
                        break;
                    case 2:
                        rdbWallCoreLayer.IsChecked = true;
                        break;
                    case 3:
                        rdbWallCoreCenter.IsChecked = true;
                        break;
                }
            }
        }

        private int RdbTagName
        {
            get => rdbUseRoomNo.IsChecked == true ? 1 : 0;
            set
            {
                if (value == 1)
                    rdbUseRoomNo.IsChecked = true;
                else
                    rdbUseRoomName.IsChecked = true;
            }
        }
    }
}
