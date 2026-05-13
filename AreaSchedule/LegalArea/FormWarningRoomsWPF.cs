
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Reflection;

using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule;

namespace ADSK.JExtRAC.AreaSchedule.LegalArea
{
    /// ================================================================================
    /// <summary>画面 警告部屋 (WPF版)</summary>
    /// ================================================================================
    public partial class FormWarningRoomsWPF : Window
    {
        #region Member Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル</summary>
        private DataTable _Data;

        #endregion

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="data">データテーブル</param>
        /// ================================================================================
        public FormWarningRoomsWPF(RvtExtApp.Components.Attribute cmpAttribute, DataTable data)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _Data = data;

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
            Close();
        }

        #endregion

        #region Private Methods

        private void SetText()
        {
            Title = _CmpAttribute.ResourceText("IDS_TXT_AREAMARGINERROR") + 
                   string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            // DataGridの列ヘッダーテキストを設定
            var columns = dgvWarningRooms.Columns;
            columns[0].Header = _CmpAttribute.ResourceText("IDS_TXT_ROOMNAME");
            columns[1].Header = _CmpAttribute.ResourceText("IDS_TXT_ROOMNUMBER");
            columns[2].Header = _CmpAttribute.ResourceText("IDS_TXT_AREA_RVT");
            columns[3].Header = _CmpAttribute.ResourceText("IDS_TXT_AREA_LEGAL");

            // ボタンのテキストを設定
            btnOK.Content = "_" + _CmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _CmpAttribute.ResourceText("IDS_TXT_CANCEL") + "(_C)";
        }

        private void SetData()
        {
            dgvWarningRooms.ItemsSource = _Data.DefaultView;
        }

        #endregion
    }
} 