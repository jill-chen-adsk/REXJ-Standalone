using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input ;
using MepManholeTool.Utils ;
using MepManholeTool.ViewModel;

namespace MepManholeTool.Views
{
    public partial class SetupParameter
    {
        public SetupParameter()
        {
            // ViewModelへのViewアクション設定
            DataContextChanged += (_, _) =>
            {
                var vm = this.DataContext as SetupParameterViewModel;

                // Nextコマンド実行時のOKアクションを設定
                if (vm != null && vm.OKAction == null)
                    vm.OKAction = new Action(this.Next);

                // 桝設定画面用に自Windowを設定
                if (vm != null && vm.Window == null)
                    vm.Window = this;

                // DataGridCellにFocusするアクションを設定
                if (vm != null && vm.GridItemFocus == null)
                    vm.GridItemFocus = new Action(this.DataGridCellFocus);
            };
            
            InitializeComponent();
        }
        
        #region イベント

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 初期フォーカス
            this.DataGridCellFocus();
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
        private void DataGridCellFocus()
        {
            DataGrid grid = this.MasuSettingGrid;
            int currentIndex = grid.SelectedIndex;
            grid.SelectedIndex = -1;
            grid.Focus();
            if (currentIndex >= 0)
                grid.SelectedIndex = currentIndex;
        }

        #endregion

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as SetupParameterViewModel;
            DataGrid grid = this.MasuSettingGrid;
            foreach ( var masuSetting in vm.MasuSetting ) {
                masuSetting.MasuItem!.HeightChanged -= vm.HeightChanged! ;
                masuSetting.HeightChanged -= vm.BottomHeightChanged ;
                masuSetting.BottomHeight.HeightChanged -= vm.UnitChanged ;
            }
            vm!.MasuSetting[grid.SelectedIndex].BottomHeight = vm.MasuSetting[grid.SelectedIndex].RequiredBottomHeight;
            ManholeUtl.CalcBottomHeight( vm.MasuSetting, false, vm.SelectedUnit ) ;
            foreach ( var masuSetting in vm.MasuSetting ) {
                masuSetting.MasuItem!.HeightChanged += vm.HeightChanged! ;
                masuSetting.HeightChanged += vm.BottomHeightChanged ;
                masuSetting.BottomHeight.HeightChanged += vm.UnitChanged ;
            }
        }

        private void ButtonTopDown_OnClick( object sender, RoutedEventArgs e )
        {
            if ( this.DataContext is SetupParameterViewModel vm ) {
                foreach ( var masuSetting in vm.MasuSetting ) {
                    masuSetting.MasuItem!.HeightChanged -= vm.HeightChanged! ;
                    masuSetting.HeightChanged -= vm.BottomHeightChanged ;
                    masuSetting.BottomHeight.HeightChanged -= vm.UnitChanged ;
                }
                ManholeUtl.CalcBottomHeightTopDown(vm.MasuSetting, vm.SelectedUnit);
                foreach ( var masuSetting in vm.MasuSetting ) {
                    masuSetting.MasuItem!.HeightChanged += vm.HeightChanged! ;
                    masuSetting.HeightChanged += vm.BottomHeightChanged ;
                    masuSetting.BottomHeight.HeightChanged += vm.UnitChanged ;
                }
            }
        }

        private void ButtonBottomUp_OnClick( object sender, RoutedEventArgs e )
        {
            if ( this.DataContext is SetupParameterViewModel vm ) {
                foreach ( var masuSetting in vm.MasuSetting ) {
                    masuSetting.MasuItem!.HeightChanged -= vm.HeightChanged! ;
                    masuSetting.HeightChanged -= vm.BottomHeightChanged ;
                    masuSetting.BottomHeight.HeightChanged -= vm.UnitChanged ;
                }
                ManholeUtl.CalcBottomHeightBottomUp( vm.MasuSetting, vm.SelectedUnit ) ;
                foreach ( var masuSetting in vm.MasuSetting ) {
                    masuSetting.MasuItem!.HeightChanged += vm.HeightChanged! ;
                    masuSetting.HeightChanged += vm.BottomHeightChanged ;
                    masuSetting.BottomHeight.HeightChanged += vm.UnitChanged ;
                }
            }
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                tb.Focus();
                tb.SelectAll();
            }
        }

        private void ParameterSetting_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                e.Handled = true;
            }
        }
    }
}