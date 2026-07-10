
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Reflection;

using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule;
using ADSK.JExtRAC.AreaSchedule.Utils;

namespace ADSK.JExtRAC.AreaSchedule.LegalArea
{
    /// ================================================================================
    /// <summary>画面 警告部屋 (WPF版)</summary>
    /// ================================================================================
    public partial class FormWarningRoomsWPF : Window, IWeaveChromeWindow
    {
        #region Member Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル</summary>
        private DataTable _Data;

        /// <summary>面積表示単位</summary>
        private string _AreaUnitLabel;

        #endregion

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="data">データテーブル</param>
        /// <param name="areaUnitLabel">面積表示単位ラベル</param>
        /// ================================================================================
        public FormWarningRoomsWPF(RvtExtApp.Components.Attribute cmpAttribute, DataTable data, string areaUnitLabel)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _Data = data;
            _AreaUnitLabel = areaUnitLabel ?? string.Empty;

            WeaveTheme.Apply(this, this, WeaveCommandTitles.LegalArea(_CmpAttribute), CancelDialog);

            // コマンドの設定
            btnCancel.Command = new RoutedCommand();
            btnCancel.CommandBindings.Add(new CommandBinding(btnCancel.Command, BtnCancel_Click));

            Loaded += FormWarningRoomsWPF_Loaded;
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        #endregion

        #region Event Handlers

        private void FormWarningRoomsWPF_Loaded(object sender, RoutedEventArgs e)
        {
            SetText();
            SetData();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
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

        #endregion

        #region Private Methods

        private void SetText()
        {
            WeaveWindowChrome.SetTitle(this, this, WeaveCommandTitles.LegalArea(_CmpAttribute));

            // DataGridの列ヘッダーテキストを設定
            var columns = dgvWarningRooms.Columns;
            columns[0].Header = _CmpAttribute.ResourceText("IDS_TXT_ROOMNAME");
            columns[1].Header = _CmpAttribute.ResourceText("IDS_TXT_ROOMNUMBER");
            columns[2].Header = string.Format(_CmpAttribute.ResourceText("IDS_TXT_AREA_RVT"), _AreaUnitLabel);
            columns[3].Header = string.Format(_CmpAttribute.ResourceText("IDS_TXT_AREA_LEGAL"), _AreaUnitLabel);

            // ボタンのテキストを設定
            btnOK.Content = _CmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _CmpAttribute.ResourceText("IDS_TXT_CANCEL") + " (_C)";
        }

        private void SetData()
        {
            dgvWarningRooms.ItemsSource = _Data.DefaultView;
            int rowCount = _Data?.Rows.Count ?? 0;
            SummaryText.Text = rowCount == 1
                ? "1 room listed below."
                : $"{rowCount} rooms listed below.";
        }

        #endregion
    }
} 