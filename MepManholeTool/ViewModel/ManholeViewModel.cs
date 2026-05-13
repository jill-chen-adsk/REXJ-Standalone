using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MepManholeTool.Commands;
using MepManholeTool.Models;
using MepManholeTool.Tools ;
using MepManholeTool.Utils ;
using MepManholeTool.Views ;
using Microsoft.Win32 ;
using Revit = Autodesk.Revit;

namespace MepManholeTool.ViewModel
{
    public sealed class ManholeViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #region IDataErrorInfo Implementation
        public string Error => null;

        public string this[string propertyName]
        {
            get
            {
                string error = null;
                if (propertyName == nameof(SelectedSymbol))
                {
                    if (SelectedSymbol == null || SelectedSymbol.Value.Key == ElementId.InvalidElementId)
                    {
                        error = "Please select a tag.";
                    }
                }
                return error;
            }
        }
        #endregion

        #region 変数

        private int _currentIndex;
        private int _createViewMode;
        
        private UIDocument _uiDocument;
        private ObservableCollection<MasuItem> _masuItems;
        private ObservableCollection<MasuItem> _originalItems;
        private string _currentLevel;
        private List<FamilySymbol> _annotationSymbols ;
        private KeyValuePair<ElementId, string>? _selectedSymbol ;
        private List<KeyValuePair<ElementId, string>> _annotationSymbolPairs ;

        #endregion
        
        #region プロパティ

        /// <summary>桝画面の呼出し時の親Window</summary>
        public System.Windows.Window Window { get; set; }

        /// <summary>削除確認メッセージファンクション</summary>
        public Func<System.Windows.MessageBoxResult> DeleteMessageFunc { get; set; }

        /// <summary>ListBoxItemにFocusするアクション</summary>
        public Action ListBoxItemFocus { get; set; }

        /// <summary>桝情報一覧</summary>
        public ObservableCollection<MasuItem> MasuItems
        {
            get => _masuItems;
            set
            {
                _masuItems = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>選択中の行インデックス</summary>
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                OnPropertyChanged();

                if (value >= 0) _uiDocument.Selection.SetElementIds(new List<ElementId>{ MasuItems.ElementAt(value).MasuElement.Id });
            }
        }

        /// <summary>
        /// ビューの作成モード
        /// </summary>
        public int CreateViewMode
        {
            get => _createViewMode;
            set
            {
                _createViewMode = value;
                OnPropertyChanged(nameof(CreateViewMode));
            }
        }
        
        public IList<MeshTriangle> TopoTriMeshes { get; set; }
        public Revit.DB.Document RvtDoc { get; set; }
        public Revit.UI.UIDocument RvtUiDoc { get ; set ; }
        public Revit.UI.UIApplication UiApp { get ; set ; }
        
        public ExternalEvent ExEvent { get ; set ; }
        public MepModelLineCommandHandler MepHandler { get; set; }

        public ObservableCollection<string> Levels { get; set; }

        public List<KeyValuePair<ElementId, string>> SymbolPairs => AnnotationSymbolPairs();
        
        public List<KeyValuePair<ElementId, string>> AnnotationSymbolPairs()
        {
            _annotationSymbols = RvtDoc.GetAnnotationSymbols() ;
            _annotationSymbolPairs = new List<KeyValuePair<ElementId, string>>() ;
            foreach ( var fs in _annotationSymbols ) {
                var name = $"{fs.FamilyName} : {fs.Name} " ;
                _annotationSymbolPairs.Add( new KeyValuePair<ElementId, string>( fs.Id, name ) ) ;
            }

            return _annotationSymbolPairs ;
        }
        
        public KeyValuePair<ElementId, string>? SelectedSymbol
        {
            get => _selectedSymbol;
            set
            {
                _selectedSymbol = value;
                OnPropertyChanged( nameof( SelectedSymbol ) ) ;
            }
        }

        public string CurrentLevel
        {
            get => _currentLevel;
            set
            {
                _currentLevel = value;
                OnPropertyChanged(nameof(CurrentLevel));
                if (_originalItems != null)
                {
                    MasuItems = new ObservableCollection<MasuItem>(_originalItems.ToList()
                        .Where(x => x.GroundLevel == CurrentLevel).ToList());
                }
                    
            }
        }

        #endregion
        
        #region 初期処理
        
        /// <summary>コンストラクタです。</summary>
        public ManholeViewModel()
        { }

        public ManholeViewModel(UIDocument uiDocument, List<MasuItem> listMasu)
        {
            this.InitializeCommand();
            this.LoadMasuItem(listMasu);
            Levels = new ObservableCollection<string>(
                listMasu
                    .Select(item => item.GroundLevel)
                    .Distinct()
            );
            CurrentLevel = Levels[0];
            
            if (_originalItems != null)
                MasuItems = new ObservableCollection<MasuItem>(_originalItems.ToList()
                    .Where(x => x.GroundLevel == CurrentLevel).ToList());
            this._uiDocument = uiDocument;
            
            SetDefaultSelectedSymbol();
        }

        /// <summary>コマンドの初期化</summary>
        private void InitializeCommand()
        {
            this.ResetCommand = new ManholeToolCommand(this.CanResetExecute, this.ResetExecute);
            this.RemoveCommand = new ManholeToolCommand(this.CanRemoveExecute, this.RemoveExecute);
            this.UpCommand = new ManholeToolCommand(this.CanUpExecute, this.UpExecute);
            this.DownCommand = new ManholeToolCommand(this.CanDownExecute, this.DownExecute);
            this.NextCommand = new ManholeToolCommand(() => true, this.NextExecute);
            this.ImportCommand = new ManholeToolCommand(() => true, this.ImportExecute ) ;
        }
        
        #endregion
        
        #region コマンド
        
        #region リセットコマンド
        /// <summary>リセットコマンドを取得します。</summary>
        public ICommand ResetCommand { get; private set; }

        /// <summary>リセットコマンドが実行可能かどうか</summary>
        private bool CanResetExecute()
        {
            if (this.CurrentIndex < 0 || this.MasuItems.Count <= 0) return false;
            return true;
        }

        /// <summary>リセットコマンドを実行します。</summary>
        private void ResetExecute()
        {
            this.MasuItems = new ObservableCollection<MasuItem>(_originalItems);
            this.ListBoxItemFocus.Invoke();
        }
        #endregion
        
        #region ↑コマンド
        /// <summary>↑コマンドを取得します。</summary>
        public ICommand UpCommand { get; private set; }

        /// <summary>↑コマンドが実行可能かどうか</summary>
        private bool CanUpExecute()
        {
            if (this.CurrentIndex <= 0) return false;
            return true;
        }

        /// <summary>↑コマンドを実行します。</summary>
        private void UpExecute()
        {
            int oldIndex = this.CurrentIndex;
            int newIndex = oldIndex - 1;
            // アクティブ行と上の行を入れ替え
            this.MasuItems.Move(oldIndex, newIndex);
            // 該当セルにフォーカスし直す
            this.ListBoxItemFocus.Invoke();
        }
        #endregion

        #region Family import
        /// <summary>↑コマンドを取得します。</summary>
        public ICommand ImportCommand { get; private set; }

        private void ImportExecute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Revit Family Files (*.rfa)|*.rfa";
            openFileDialog.Title = "Select Revit Family File";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                if ( string.IsNullOrEmpty(filePath) ) {
                    string familyName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                    FilteredElementCollector collector = new FilteredElementCollector(RvtDoc);
                    collector.OfClass(typeof(Family));
                    if ( collector.Any( e => e.Name.Equals( familyName, StringComparison.OrdinalIgnoreCase ) ) ) {
                        using (Transaction tran = new Transaction(RvtDoc, familyName))
                        {
                            tran.Start();
                            RvtDoc.LoadFamily(filePath);
                            tran.Commit();
                        }
                    }
                }
            }
        }
        #endregion

        #endregion
        
        #region ↓コマンド
        /// <summary>↓コマンドを取得します。</summary>
        public ICommand DownCommand { get; private set; }

        /// <summary>↓コマンドが実行可能かどうか</summary>
        private bool CanDownExecute()
        {
            if (this.CurrentIndex >= this.MasuItems.Count - 1) return false;
            return true;
        }

        /// <summary>↓コマンドを実行します。</summary>
        private void DownExecute()
        {
            int oldIndex = this.CurrentIndex;
            int newIndex = oldIndex + 1;
            // アクティブ行と下の行を入れ替え
            this.MasuItems.Move(oldIndex, newIndex);
            // 該当セルにフォーカスし直す
            this.ListBoxItemFocus.Invoke();
        }
        #endregion
        
        #region 削除コマンド
        /// <summary>削除コマンドを取得します。</summary>
        public ICommand RemoveCommand { get; private set; }

        /// <summary>削除コマンドが実行可能かどうか</summary>
        private bool CanRemoveExecute()
        {
            if (this.CurrentIndex < 0 || this.MasuItems.Count <= 0) return false;
            return true;
        }

        /// <summary>削除コマンドを実行します。</summary>
        private void RemoveExecute()
        {
            int removeIndex = this.CurrentIndex;

            // 削除確認メッセージを表示
            var msgResult = this.DeleteMessageFunc.Invoke();
            if (msgResult == System.Windows.MessageBoxResult.Cancel) return;

            // 該当行を削除
            this.MasuItems.RemoveAt(removeIndex);
            // 削除した行の次の行をアクティブにする
            this.CurrentIndex = Math.Min(removeIndex, this.MasuItems.Count - 1);
            // 作成モード
            this.CreateViewMode = 0;
            // 該当セルにフォーカスし直す
            this.ListBoxItemFocus.Invoke();
        }
        #endregion
        
        #region Nextコマンド
        /// <summary>Nextコマンドを取得します。</summary>
        public ICommand NextCommand { get; private set; }

        /// <summary>「次へ」処理を実行します。</summary>
        private void NextExecute()
        {
            // Validate SelectedSymbol
            if (SelectedSymbol == null || SelectedSymbol.Value.Key == ElementId.InvalidElementId)
            {
                System.Windows.MessageBox.Show("Please select a tag.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            // Check for null PipeStep values (excluding the last item)
            if (MasuItems.Count >= 2)
            {
                var itemsWithNullPipeStep = new List<string>();
                for (int i = 0; i < MasuItems.Count - 1; i++) // Exclude the last item
                {
                    if (MasuItems[i].PipeStep == null)
                    {
                        itemsWithNullPipeStep.Add(MasuItems[i].MasuSymbol);
                    }
                }

                if (itemsWithNullPipeStep.Count > 0)
                {
                    string masuSymbols = string.Join(", ", itemsWithNullPipeStep);
                    string message = $"The following manhole items have no pipe step set:\n{masuSymbols}\n\nDo you want to continue?";
                    
                    var result = System.Windows.MessageBox.Show(
                        message,
                        "Confirm",
                        System.Windows.MessageBoxButton.YesNo,
                        System.Windows.MessageBoxImage.Question);
                    
                    if (result == System.Windows.MessageBoxResult.No)
                    {
                        return;
                    }
                }
            }

            //Topo solid高さを設定
            if ( MasuItems.Count >= 2 ) {
                foreach ( var m in MasuItems ) {
                    double z = double.MinValue ;
                    var orgLoc = ( m.MasuElement as FamilyInstance )?.GetTotalTransform().Origin ;
                    if ( TopoTriMeshes.Count > 0 ) {
                        if ( GeometryUtl.CheckTrianglePoint( orgLoc, TopoTriMeshes, ref z ) ) {
                            m.HeightToTopo =
                                Math.Round(
                                    UnitUtils.ConvertFromInternalUnits( ( z - orgLoc.Z ),
                                        UnitTypeId.Millimeters ), 0 ) ;
                        }

                        m.OrgHeightToTopo = m.HeightToTopo ;
                    }
                }

                var setupView = new SetupParameter() ;
                var setupViewModel =
                    new SetupParameterViewModel( RvtDoc, RvtUiDoc, MasuItems.ToList() ) ;
                setupViewModel.CreateViewMode = CreateViewMode ;
                MepModelLineCommandHandler handler = new MepModelLineCommandHandler() ;
                setupViewModel.ExEvent = ExEvent ;
                setupViewModel.MepHandler = MepHandler ;
                setupViewModel.TagId = SelectedSymbol.Value.Key ;
                setupView.DataContext = setupViewModel ;
                if ( TopoTriMeshes != null && TopoTriMeshes.Count == 0 ) {
                    setupViewModel.OverrideLevelHeight = true ;
                    setupViewModel.ReadOnlyTopo = false ;
                    setupView.OverrideTopo.Visibility = System.Windows.Visibility.Collapsed ;
                }
                var mainWin = MainWindowHelper.GetRevitMainWindow( UiApp ) ;
                setupView.Owner = mainWin ;
                setupView.Show() ;
                
                NextAction.Invoke();
            }
        }
        #endregion
        
        #region メソッド

        /// <summary>桝情報を参照します。</summary>
        private void LoadMasuItem(List<MasuItem> masuItems)
        {
            this.MasuItems = new ObservableCollection<MasuItem>(masuItems);
            this._originalItems = new ObservableCollection<MasuItem>(MasuItems);
        }

        /// <summary>SelectedSymbolの初期値を設定します。</summary>
        private void SetDefaultSelectedSymbol()
        {
            if (RvtDoc != null)
            {
                var symbolPairs = AnnotationSymbolPairs();
                if (symbolPairs != null && symbolPairs.Count > 0)
                {
                    // "00_RJ_タグ_配管付属品_CL15" を優先的に選択
                    var preferredSymbol = symbolPairs.FirstOrDefault(pair => 
                        pair.Value.Contains("00_RJ_タグ_配管付属品_CL15"));
                    
                    if (preferredSymbol.Key != null && preferredSymbol.Key != ElementId.InvalidElementId)
                    {
                        SelectedSymbol = preferredSymbol;
                    }
                    else
                    {
                        // 見つからない場合は最初の項目を選択
                        SelectedSymbol = symbolPairs.First();
                    }
                }
            }
        }
        
        #endregion
        
        #region アクション

        /// <summary>Nextアクション</summary>
        public Action NextAction { get; set; }

        #endregion
    }
} 