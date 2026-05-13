using ADSK.JExtRAC.ValueCopy.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using RvtExtApp = ADSK.JExtRAC.ValueCopy;

namespace ADSK.JExtRAC.ValueCopy.UI
{
    /// <summary>
    /// 真偽値を反転してVisibilityに変換するコンバーター
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Collapsed;
            }
            return false;
        }
    }

    /// ================================================================================
    /// <summary>FormParameterWPF</summary>
    ///
    /// <history>2024/03/21 Created</history>
    /// ================================================================================
    public partial class FormParameterWPF : Window
    {
        // Member variable

        #region Member Variables

        /// <summary>Attributes</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>ObjectElement</summary>
        private ObjectElement _ObjElement = null;

        /// <summary>List ObjectIndexGroup </summary>
        private List<ObjectIndexGroup> _IndexGroups = null;

        /// <summary>項目リスト</summary>
        private List<ParameterGridItem> _items = null;

        /// <summary>チェックボックス状態変更イベントを無視するフラグ</summary>
        private bool _ignoreCheckboxEvents = false;

        #endregion Member Variables

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="cmpAttribute">Parameter</param>
        /// <param name="objElement">ObjectElement</param>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        public FormParameterWPF(RvtExtApp.Components.Attribute cmpAttribute, ObjectElement objElement)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;
            _ObjElement = objElement;
            _IndexGroups = new List<ObjectIndexGroup>();

            InitText();
            InitData();
        }

        #endregion Constructor

        // Member Functions

        #region Member Function

        /// ================================================================================
        /// <summary>Form character setting</summary>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void InitText()
        {
            this.Title = _CmpAttribute.ResourceText("IDS_TXT_COPYFORM");
            btApply.Content = _CmpAttribute.ResourceText("IDS_TXT_APPLY");
            btCancel.Content = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        /// ================================================================================
        /// <summary>Form data setting</summary>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void InitData()
        {
            if (_ObjElement == null)
                return;

            _items = new List<ParameterGridItem>();

            // グループごとにデータを整理
            var groups = _ObjElement.ObjectParameterData.GroupBy(x => x.ParameterGroupName).OrderBy(x => x.Key);
            foreach (var group in groups)
            {
                // グループ行を追加
                var groupRow = new ParameterGridItem
                {
                    IsGroup = true,
                    GroupName = group.Key,
                    ElementId = group.FirstOrDefault().ElementIdGroup,
                    Name = group.Key,
                    Value = string.Empty,
                    IsParameterChecked = null // 三状態の初期状態
                };
                _items.Add(groupRow);

                ObjectIndexGroup objIndexGroup = new ObjectIndexGroup
                {
                    ParameterGroupName = group.Key,
                    IndexOnDatagridview = _items.Count - 1
                };
                _IndexGroups.Add(objIndexGroup);

                // パラメータ行を追加
                foreach (var objParameter in group)
                {
                    var parameterRow = new ParameterGridItem
                    {
                        IsGroup = false,
                        GroupName = group.Key,
                        ElementId = objParameter,
                        IsParameterChecked = false,
                        Name = objParameter.NameParameter,
                        Value = objParameter.ParameterValue
                    };
                    _items.Add(parameterRow);
                }
            }

            lvParameters.ItemsSource = _items;
        }

        /// ================================================================================
        /// <summary>グループ内の子アイテムの状態を更新</summary>
        ///
        /// <param name="groupName">グループ名</param>
        /// <param name="newState">新しい状態</param>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void UpdateChildItems(string groupName, bool? newState)
        {
            _ignoreCheckboxEvents = true;

            try
            {
                foreach (var item in _items.Where(x => x.GroupName == groupName && !x.IsGroup))
                {
                    // 三状態のチェックボックスの場合、nullの場合はfalseとして扱う
                    item.IsParameterChecked = newState.HasValue ? newState.Value : false;
                }
            }
            finally
            {
                _ignoreCheckboxEvents = false;
            }
        }

        /// ================================================================================
        /// <summary>グループの状態を子アイテムの状態に基づいて更新</summary>
        ///
        /// <param name="groupName">グループ名</param>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void UpdateGroupState(string groupName)
        {
            _ignoreCheckboxEvents = true;

            try
            {
                var groupItem = _items.FirstOrDefault(x => x.IsGroup && x.GroupName == groupName);
                if (groupItem == null)
                    return;

                var childItems = _items.Where(x => x.GroupName == groupName && !x.IsGroup).ToList();
                if (!childItems.Any())
                    return;

                if (childItems.All(x => x.IsParameterChecked == true))
                {
                    // すべての子アイテムがチェックされている場合
                    groupItem.IsParameterChecked = true;
                }
                else if (childItems.All(x => x.IsParameterChecked == false || x.IsParameterChecked == null))
                {
                    // すべての子アイテムがチェックされていない場合
                    groupItem.IsParameterChecked = false;
                }
                else
                {
                    // 一部の子アイテムがチェックされている場合（不確定状態）
                    groupItem.IsParameterChecked = null;
                }
            }
            finally
            {
                _ignoreCheckboxEvents = false;
            }
        }

        #endregion Member Function

        // Events

        #region Events

        /// ================================================================================
        /// <summary>チェックボックスの状態変更イベントハンドラー</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void CheckBox_StateChanged(object sender, RoutedEventArgs e)
        {
            if (_ignoreCheckboxEvents)
                return;

            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) 
                return;

            ParameterGridItem item = checkBox.DataContext as ParameterGridItem;
            if (item == null) 
                return;

            if (item.IsGroup)
            {
                // グループのチェックボックスが変更された場合、すべての子アイテムを更新
                UpdateChildItems(item.GroupName, checkBox.IsChecked);
            }
            else
            {
                // 子アイテムのチェックボックスが変更された場合、グループの状態を更新
                UpdateGroupState(item.GroupName);
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btApply control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void btApply_Click(object sender, RoutedEventArgs e)
        {
            bool isHasCopy = false;

            // 子アイテムのチェック状態をオブジェクトに設定
            foreach (var row in _items)
            {
                if (row.IsGroup) continue;

                var objParameter = row.ElementId as ObjectParameter;
                if (objParameter == null) continue;

                objParameter.IsCopy = row.IsParameterChecked ?? false;
                if (row.IsParameterChecked == true)
                    isHasCopy = true;
            }

            if (!isHasCopy)
            {
                MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_INPUTVALUE"));
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        #endregion Events
    }

    /// <summary>
    /// ParameterGridItem class used for data binding in ListView
    /// </summary>
    public class ParameterGridItem : INotifyPropertyChanged
    {
        public bool IsGroup { get; set; }
        public string GroupName { get; set; }
        public object ElementId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        private bool? _isParameterChecked;
        public bool? IsParameterChecked
        {
            get { return _isParameterChecked; }
            set
            {
                if (_isParameterChecked != value)
                {
                    _isParameterChecked = value;
                    OnPropertyChanged("IsParameterChecked");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}