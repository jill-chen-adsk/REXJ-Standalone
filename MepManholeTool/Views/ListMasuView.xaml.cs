using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MepManholeTool.ViewModel;

namespace MepManholeTool.Views
{
    public partial class ListMasuView : Window
    {
        #region 初期処理
        public ListMasuView()
        {
            // ViewModelへのViewアクション設定
            DataContextChanged += (o, e) =>
            {
                var vm = this.DataContext as ManholeViewModel;

                // Closeコマンド実行時のNextアクションを設定
                if (vm != null && vm.NextAction == null)
                    vm.NextAction = new Action(this.Next);

                // 桝画面用に自Windowを設定
                if (vm != null && vm.Window == null)
                    vm.Window = this;

                // 削除コマンド実行時のメッセージボックスを設定
                if (vm != null && vm.DeleteMessageFunc == null)
                    vm.DeleteMessageFunc = new Func<MessageBoxResult>(this.DeleteMessage);

                // DataGridCellにFocusするアクションを設定
                if (vm != null && vm.ListBoxItemFocus == null)
                    vm.ListBoxItemFocus = new Action(this.ListBoxItemFocus);
            };
            InitializeComponent();
        }
        #endregion
        
        #region イベント

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 初期フォーカス
            this.ListBoxItemFocus();
        }

        private void MasuTagFamilyComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (MasuTagFamilyComboBox.Items.Count > 0)
            {
                var vm = this.DataContext as ManholeViewModel;
                if (vm != null)
                {
                    var preferredSymbol = vm.SymbolPairs.FirstOrDefault(pair => 
                        pair.Value.Contains("00_RJ_タグ_配管付属品_CL15"));
                    
                    if (preferredSymbol.Key != null && preferredSymbol.Key != Autodesk.Revit.DB.ElementId.InvalidElementId)
                    {
                        vm.SelectedSymbol = preferredSymbol;
                    }
                    else if (MasuTagFamilyComboBox.Items.Count > 0)
                    {
                        MasuTagFamilyComboBox.SelectedIndex = 0;
                    }
                }
            }
        }

        #endregion
        
        #region メソッド

        /// <summary>「次へ」ボタン押下時のアクション</summary>
        private void Next() => this.Close() ;

        /// <summary>削除確認メッセージ</summary>
        private MessageBoxResult DeleteMessage()
        {
            string txt = "Delete this row. Are you sure?";
            return MessageBox.Show(this, txt, this.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question);
        }

        /// <summary>リストの対象セルにフォーカスするアクション</summary>
        private void ListBoxItemFocus()
        {
            DataGrid grid = this.ListMasuGrid;
            int currentIndex = grid.SelectedIndex;
            grid.SelectedIndex = -1;
            grid.Focus();
            if (currentIndex >= 0)
                grid.SelectedIndex = currentIndex;
        }

        #endregion
    }
}