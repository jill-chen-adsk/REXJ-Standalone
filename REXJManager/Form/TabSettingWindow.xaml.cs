using System.ComponentModel ;
using System.Windows ;
using System.Windows.Controls ;
using System.Windows.Input ;
using System.Windows.Media ;
using Autodesk.Revit.UI ;
using UIFramework ;
using CheckBox = System.Windows.Controls.CheckBox ;
using ComboBox = System.Windows.Controls.ComboBox ;
using KeyEventArgs = System.Windows.Input.KeyEventArgs ;
using Res = REXJManager.Resource ;

namespace REXJManager
{
  public partial class TabSettingWindow : Window
  {
    private readonly UIDocument _uiDocument ;
    private readonly MyTreeNode _treeNode ;
    private bool _shouldSave ;  //状態が更新されたときにtrueにする

    public bool IsShown ;

    public TabSettingWindow(UIDocument uiDocument) 
    {
      _uiDocument = uiDocument ;
      IsShown = true ;
      InitializeComponent() ;
      _treeNode = RevitRibbonControl.RibbonControl.Extract<MyTreeNode>() ;
      TabTreeView.ItemsSource = _treeNode.Children ;
      InitPresetComboBoxItems() ;
      PresetComboBox.Visibility = Visibility.Visible ;
      PresetNameTextBox.Visibility = Visibility.Hidden ;
    }

    /// <summary>
    /// プリセットコンボボックスのアイテム初期化
    /// 読み込み専用のプリセットをグレーにする
    /// </summary>
    private void InitPresetComboBoxItems()
    {
      PresetComboBox.Items.Clear();
      var list = Preset.Names() ;
      foreach ( var name in list ) {
        var foreground = Preset.IsReadOnly( name ) ? Brushes.DimGray : Brushes.Black ;  
        var item = new ComboBoxItem { Content = name, Tag = name, Foreground = foreground } ;
        PresetComboBox.Items.Add( item ) ;
        if ( name == Preset.Name ) PresetComboBox.SelectedItem = item ;
      }
      
      if((ComboBoxItem)PresetComboBox.SelectedValue==null) PresetComboBox.SelectedIndex = 0 ;
      LoadPreset(((ComboBoxItem)PresetComboBox.SelectedValue)?.Content?.ToString()) ;
    }

    /// <summary>
    /// ツリー内のチェックボックスをクリックしたときの処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckBox_OnClick( object sender, RoutedEventArgs e )
    {
      _shouldSave = true ;
      var checkBox = (CheckBox)sender ;
      var treeNode = (MyTreeNode)checkBox.DataContext ;
      var isChecked = checkBox.IsChecked == true ;
      treeNode.IsChecked = isChecked ;
      
      // 子要素も同じチェック状態に設定
      SetChildrenCheckedState( treeNode, isChecked ) ;
    }
    
    /// <summary>
    /// 子要素のチェック状態を再帰的に設定
    /// </summary>
    /// <param name="node">対象のノード</param>
    /// <param name="isChecked">設定するチェック状態</param>
    private void SetChildrenCheckedState( MyTreeNode node, bool isChecked )
    {
      if ( node.Children == null || node.Children.Count == 0 ) return ;
      
      foreach ( var child in node.Children ) {
        if (child.IsChecked != isChecked) child.IsChecked = isChecked ;
        SetChildrenCheckedState( child, isChecked ) ;
      }
    }

    /// <summary>
    /// プリセットコンボボックスの選択変更時の処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PresetComboBox_OnSelectionChanged( object sender, SelectionChangedEventArgs e )
    {
      if ( sender is not ComboBox comboBox ) return ;
      if ( comboBox.SelectedValue == null ) return ;
      SavePreset();
      var name = ((ComboBoxItem)comboBox.SelectedValue).Content.ToString() ;
      LoadPreset(name) ;
      var isReadOnly = Preset.IsReadOnly( name ) ;
      TrashButton.Visibility = isReadOnly ? Visibility.Hidden : Visibility.Visible;
      Preset.SaveConf();
    }

    /// <summary>
    /// プリセットを読み込む処理
    /// </summary>
    /// <param name="name">プリセット名（拡張子なしファイル名）</param>
    private void LoadPreset(string name)
    {
      if ( string.IsNullOrEmpty( name ) ) return ;
      _treeNode.Load( name ) ;
      _treeNode.SetIsEnabled( ! Preset.IsReadOnly( name ) );

      TabTreeView.ItemsSource = null ;
      TabTreeView.UpdateLayout();
      TabTreeView.ItemsSource = _treeNode.Children ;
      TabTreeView.UpdateLayout();

    }

    /// <summary>
    /// プリセット追加ボタンを押したときの処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddPresetButton_OnClick( object sender, RoutedEventArgs e )
    {
      SavePreset();
      AddPresetButton.IsEnabled = false ;
      TrashButton.IsEnabled = false ;
      DuplicateButton.IsEnabled = false ;
      
      PresetComboBox.Visibility = Visibility.Hidden ;
      PresetNameTextBox.Visibility = Visibility.Visible ;
      PresetNameTextBox.Text = Res.FILENAME_NEW ;
      PresetNameTextBox.Focus() ;
    }

    /// <summary>
    /// プリセット名のテキストボックスでEnterを押したときに決定にする処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PresetNameTextBox_OnKeyDown( object sender, KeyEventArgs e )
    {
      if ( e.Key != Key.Enter && e.Key != Key.Escape ) return;
      PresetComboBox.Visibility = Visibility.Visible ;
      PresetNameTextBox.Visibility = Visibility.Hidden ;
      AddPresetButton.IsEnabled = true ;
      TrashButton.IsEnabled = true ;
      DuplicateButton.IsEnabled = true ;
      
      if ( e.Key != Key.Enter ) return;
      var name = PresetNameTextBox.Text ;
      var newName = Preset.CreatePreset( name ) ;
      var item = new ComboBoxItem { Content = newName, Tag = newName } ;
      PresetComboBox.Items.Add( item ) ;
      PresetComboBox.SelectedItem = item ;
    }

    /// <summary>
    /// プリセットの保存
    /// </summary>
    private void SavePreset()
    {
      if(!_shouldSave) return;
      var presetString = _treeNode.ToPresetString() ;
      var name = Preset.Name ;
      
      Preset.SavePreset( name, presetString );
    }

    /// <summary>
    /// ウィンドウを閉じるときにプリセットを保存する
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TabSettingWindow_OnClosing( object sender, CancelEventArgs e )
    {
      SavePreset();
      IsShown = false ;
    }

    /// <summary>
    /// プリセット削除ボタンを押したときの処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TrashButton_OnClick( object sender, RoutedEventArgs e )
    {
      var name = Preset.Name ;
      Preset.Delete( name );
      PresetComboBox.Items.Remove( PresetComboBox.SelectedItem );
      PresetComboBox.SelectedIndex = 0 ;
    }

    /// <summary>
    /// プリセット複製ボタンを押したときの処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DuplicateButton_OnClick( object sender, RoutedEventArgs e )
    {
      var name = Preset.Name ;
      var newName = Preset.Duplicate( name );
      var item = new ComboBoxItem { Content = newName, Tag = newName} ;
      PresetComboBox.Items.Add( item ) ;
      PresetComboBox.SelectedItem = item ;
      
    }
    
    private void MenuItem_Info_OnClick( object sender, RoutedEventArgs e )
    {
      ShowInfo();
    }

    private void ShowInfo()
    {
      const string title = "About" ;
      var app = _uiDocument.Application.Application;
      var verName = app.VersionName ;
      var verBuild = app.VersionBuild ;
      var verNumber = app.VersionNumber ;
      
      var body = $"{verName} ({verNumber} : {verBuild})" ;

      TaskDialog.Show(title, body, TaskDialogCommonButtons.Ok) ;
    }
    
  }
}