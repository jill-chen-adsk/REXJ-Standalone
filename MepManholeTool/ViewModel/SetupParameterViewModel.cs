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
using MepManholeTool.Const ;
using MepManholeTool.Models;
using MepManholeTool.Tools ;
using MepManholeTool.Utils;
using FamilyInstance = Autodesk.Revit.DB.FamilyInstance;
using Revit = Autodesk.Revit;
// ReSharper disable All

namespace MepManholeTool.ViewModel
{
    public class SetupParameterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged = null!;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #region 変数

        private Document revitDoc = null!;

        private int currentIndex;
        
        private UIDocument uiDocument;

        private MasuToolUnit selectedUnit;

        #region 一括設定

        private double bulkGroundLevel;
        private double bulkOutputDiameter;
        private double bulkMudPuddle;
        private double bulkPipeStep;
        private double bulkGradientDenominator;
        
        #endregion

        private bool isUpdating = false;

        #endregion
        
        #region プロパティ

        /// <summary>桝画面の呼出し時の親Window</summary>
        public System.Windows.Window Window { get; set; } = null!;

        /// <summary>削除確認メッセージファンクション</summary>
        public Func<System.Windows.MessageBoxResult> DeleteMessageFunc { get; set; } = null!;

        /// <summary>GridItemにFocusするアクション</summary>
        public Action GridItemFocus { get; set; } = null!;
        
        /// <summary>Drafting view creation mode</summary>
        public int CreateViewMode { get; set; }
        
        public ExternalEvent ExEvent { get ; set ; }
        public MepModelLineCommandHandler MepHandler { get; set; }
        public ElementId TagId { get; set; }

        /// <summary>桝情報一覧</summary>
        public ObservableCollection<RoutingParameter> MasuSetting { get; set; } = null!;
        
        public ObservableCollection<MasuToolUnit> MasuToolUnits { get; set; } = null!;

        public MasuToolUnit SelectedUnit
        {
            get => selectedUnit;
            set
            {
                if (value == null) return;
                
                selectedUnit = value;
                if (MasuSetting == null) return;

                try
                {
                    isUpdating = true;
                    
                    // Store current CheckBottomHeight values
                    var checkBottomHeights = MasuSetting
                        .Where(x => x?.CheckBottomHeight != null)
                        .Select(x => x.CheckBottomHeight.Value)
                        .ToList();
                    
                    // Temporarily unsubscribe from events
                    foreach (var masuSetting in MasuSetting.Where(x => x != null))
                    {
                        if (masuSetting.MasuItem != null)
                            masuSetting.MasuItem.HeightChanged -= HeightChanged!;
                        masuSetting.HeightChanged -= BottomHeightChanged;
                        if (masuSetting.BottomHeight != null)
                            masuSetting.BottomHeight.HeightChanged -= UnitChanged;
                    }
                    
                    // Recalculate bottom heights
                    ManholeUtl.CalcBottomHeight(MasuSetting, true, SelectedUnit);
                    
                    // Restore CheckBottomHeight values and update BottomHeight accordingly
                    for (int i = 0; i < MasuSetting.Count && i < checkBottomHeights.Count; i++)
                    {
                        if (checkBottomHeights[i] != 0 && MasuSetting[i]?.CheckBottomHeight != null)
                        {
                            MasuSetting[i].BottomHeight = MasuSetting[i].CheckBottomHeight;
                        }
                    }
                    
                    // Resubscribe to events
                    foreach (var masuSetting in MasuSetting.Where(x => x != null))
                    {
                        if (masuSetting.MasuItem != null)
                            masuSetting.MasuItem.HeightChanged += HeightChanged!;
                        masuSetting.HeightChanged += BottomHeightChanged;
                        if (masuSetting.BottomHeight != null)
                            masuSetting.BottomHeight.HeightChanged += UnitChanged;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in SelectedUnit setter: {ex.Message}");
                }
                finally
                {
                    isUpdating = false;
                }
            }
        }

        public double TotalLength { get; set; } = 0;

        private string _draftingViewName = "Drafting View";
        private string _draftingViewNameFixLenght = "Drafting View";
        /// <summary>Drafting view name</summary>
        public string DraftingViewName 
        { 
            get => _draftingViewName;
            set
            {
                _draftingViewName = value;
                OnPropertyChanged();
            }
        }

        public string DraftingViewNameFixLenght
        {
            get => _draftingViewNameFixLenght;
            set
            {
                _draftingViewNameFixLenght = value;
                OnPropertyChanged();
            }
        }


        /// <summary>選択中の行インデックス</summary>
        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                currentIndex = value;
                OnPropertyChanged();

                if (value >= 0 && MasuSetting.ElementAt(value)?.MasuItem != null)
                {
                    uiDocument.Selection.SetElementIds(new List<ElementId>{ MasuSetting.ElementAt(value).MasuItem!.MasuElement.Id });
                }
            }
        }

        private bool _overrideLevelHeight = false;

        public bool OverrideLevelHeight
        {
            get => _overrideLevelHeight;
            set
            {
                _overrideLevelHeight = value;
                OnPropertyChanged(nameof(OverrideLevelHeight));

                ReadOnlyTopo = !value;
                MasuSetting?.ToList().ForEach(parameter => parameter.EnableTopo = value);
                if (value == false) MasuSetting?.ToList().ForEach(parameter => parameter.MasuItem.HeightToTopo = parameter.MasuItem.OrgHeightToTopo);
            }
        }
        
        private bool _readOnlyTopo = true;

        public bool ReadOnlyTopo
        {
            get => _readOnlyTopo;
            set
            {
                _readOnlyTopo = value;
                OnPropertyChanged(nameof(ReadOnlyTopo));
            }
        }
        
        #region 一括設定

        public ObservableCollection<BulkSetting> BulkSettings { get; set; } = null!;
        
        public double BulkGroundLevel
        {
            get => bulkGroundLevel;
            set
            {
                bulkGroundLevel = value;
                OnPropertyChanged();
                
                MasuSetting.ToList().ForEach(x => x.MasuItem!.HeightToTopo = value);
            }
        }

        public double BulkOutputDiameter
        {
            get => bulkOutputDiameter;
            set
            {
                bulkOutputDiameter = value;
                OnPropertyChanged();
                
                MasuSetting.ToList().ForEach(x => {
                    if (x.MasuItem!.OutputDiameter.HasValue)
                        x.MasuItem!.OutputDiameter = value;
                });
            }
        }
        public double BulkMudPuddle
        {
            get => bulkMudPuddle;
            set
            {
                bulkMudPuddle = value;
                OnPropertyChanged();
                
                MasuSetting.ToList().ForEach(x => {
                    if (x.MasuItem!.MudPuddle.HasValue)
                        x.MasuItem!.MudPuddle = value;
                });
            }
        }
        
        public double BulkPipeStep
        {
            get => bulkPipeStep;
            set
            {
                bulkPipeStep = value;
                OnPropertyChanged();
                
                MasuSetting.ToList().ForEach(x => {
                    if (x.MasuItem!.PipeStep.HasValue)
                        x.MasuItem!.PipeStep = value;
                });
            }
        }

        public double GradientDenominator
        {
            get => bulkGradientDenominator;
            set
            {
                bulkGradientDenominator = value;
                OnPropertyChanged();
                
                // Temporarily unsubscribe from events to prevent infinite loops
                foreach (var masuSetting in MasuSetting)
                {
                    masuSetting.MasuItem!.HeightChanged -= HeightChanged!;
                    masuSetting.HeightChanged -= BottomHeightChanged;
                    masuSetting.BottomHeight.HeightChanged -= UnitChanged;
                }

                // Update the values
                MasuSetting.ToList().ForEach(x => x.GradientDenominator = value);
                
                // Calculate gradient denominator differences
                foreach (var masuSetting in MasuSetting)
                {
                    if (masuSetting.GradientDenominator != 0)
                    {
                        masuSetting.GradientDenominatorDifference = masuSetting.PipeLength / masuSetting.GradientDenominator;
                    }
                    else
                    {
                        masuSetting.GradientDenominatorDifference = 0;
                    }
                }

                // Recalculate bottom heights
                ManholeUtl.CalcBottomHeight(MasuSetting, true, SelectedUnit);

                // Resubscribe to events
                foreach (var masuSetting in MasuSetting)
                {
                    masuSetting.MasuItem!.HeightChanged += HeightChanged!;
                    masuSetting.HeightChanged += BottomHeightChanged;
                    masuSetting.BottomHeight.HeightChanged += UnitChanged;
                }
            }
        }

        private double _gradientDenominatorDifference;
        public double GradientDenominatorDifference
        {
            get => _gradientDenominatorDifference;
            set
            {
                _gradientDenominatorDifference = value;
                OnPropertyChanged();
            }
        }

        #endregion
        
        #endregion
        
        #region 初期処理
        
        /// <summary>コンストラクタです。</summary>
        public SetupParameterViewModel()
        { }

        public SetupParameterViewModel(Document doc, UIDocument uiDoc, List<MasuItem> listMasu)
        {
            revitDoc = doc;
            uiDocument = uiDoc;
            InitializeCommand();
            InitManholeToolSymbol();
            LoadManholeSetting(listMasu);
            InitBulkSetting();
        }

        /// <summary>コマンドの初期化</summary>
        private void InitializeCommand()
        {
            OKCommand = new ManholeToolCommand(() => true, this.OKExecute);
            CopyHeightCommand = new ManholeToolCommand(() => true, this.CopyHeightExecute);
        }
        
        #endregion
        
        #region コマンド

        #region 管底高コピーコマンド

        public ICommand CopyHeightCommand { get; private set; } = null!;

        private void CopyHeightExecute()
        {
            CopyAction?.Invoke();
            MasuSetting[CurrentIndex].BottomHeight = MasuSetting[CurrentIndex].RequiredBottomHeight;
        }

        #endregion
        
        #region OKコマンド
        /// <summary>OKコマンドを取得します。</summary>
        public ICommand OKCommand { get; private set; } = null!;

        /// <summary>「実行」処理を実行します。</summary>
        private void OKExecute()
        {
            MepHandler.Context = this;
            MepHandler.RevitDoc = revitDoc;
            ExEvent.Raise();
            
            OKAction?.Invoke();
            
        }
        #endregion
        
        #endregion
        
        #region メソッド

        /// <summary>桝設定情報を参照します。</summary>
        private void LoadManholeSetting(List<MasuItem> masuItems)
        {
            this.MasuSetting = ManholeUtl.GetRoutingParameters(revitDoc, masuItems, SelectedUnit);
            var fromOrgLoc = (MasuSetting.Last().MasuItem!.MasuElement as FamilyInstance)?.GetTotalTransform().Origin;
            
            for (int i = MasuSetting.Count - 1; i >= 0 ; i--) {
                var toOrgLoc = (MasuSetting[i].MasuItem!.MasuElement as FamilyInstance)!.GetTotalTransform().Origin;
                MasuSetting[i].PipeLength = UnitUtils.ConvertFromInternalUnits(fromOrgLoc!.DistanceTo(toOrgLoc), UnitTypeId.Millimeters);
                fromOrgLoc = toOrgLoc; 
            }

            ManholeUtl.CalcBottomHeight(MasuSetting, true, SelectedUnit, true);

            foreach (var masuSetting in MasuSetting)
            {
                TotalLength += masuSetting.PipeLength;
                masuSetting.MasuItem!.HeightChanged += HeightChanged!;
                masuSetting.HeightChanged += BottomHeightChanged;
                masuSetting.BottomHeight.HeightChanged += UnitChanged;
                if (masuSetting.CheckBottomHeight != null)
                {
                    masuSetting.CheckBottomHeight.HeightChanged += CheckBottomHeightChanged;
                }
            }

            //最初・最終桝の記号が変わった時に製図ビュー名を更新する
            MasuSetting.First().MasuItem.SymbolChanged += ManholeSymbolChanged;
            MasuSetting.Last().MasuItem.SymbolChanged += ManholeSymbolChanged;

            // 初期化時は backing field に直接代入（PropertyChanged が null の可能性があるため）
            _draftingViewName = $"Manhole profile_{MasuSetting.First().MasuItem.MasuSymbol}-{MasuSetting.Last().MasuItem.MasuSymbol}_full scale_{DateTime.Now.ToString("yyyyMMdd")}";
            _draftingViewNameFixLenght = $"Manhole profile_{MasuSetting.First().MasuItem.MasuSymbol}-{MasuSetting.Last().MasuItem.MasuSymbol}_{DateTime.Now.ToString("yyyyMMdd")}";
        }

        private void InitBulkSetting()
        {
            BulkSetting setting = new BulkSetting();
            if (MasuSetting.Count > 0 && MasuSetting.First().MasuItem != null)
            {
                setting.BulkGroundLevel = MasuSetting.First().MasuItem!.HeightFromGroundLevel; 
                setting.BulkGradientDenominator = MasuSetting.First().GradientDenominator; 
                setting.BulkMudPuddle = MasuSetting.FirstOrDefault(x => x.MasuItem?.MudPuddle.HasValue == true)?.MasuItem?.MudPuddle ?? 0; 
                setting.BulkPipeStep = MasuSetting.FirstOrDefault(x => x.MasuItem?.PipeStep.HasValue == true)?.MasuItem?.PipeStep ?? 0; 
                setting.BulkOutputDiameter = MasuSetting.FirstOrDefault(x => x.MasuItem?.OutputDiameter.HasValue == true)?.MasuItem?.OutputDiameter ?? 0; 
                setting.BulkSettingChanged += this.BulkSettingChanged!;
            }

            BulkSettings = new ObservableCollection<BulkSetting>(new List<BulkSetting> { setting });
        }

        private void InitManholeToolSymbol()
        {
            var masuToolUnits = new List<MasuToolUnit>( ) {new MasuToolUnit("1mm", 1), new MasuToolUnit("10mm", 10), new MasuToolUnit("50mm", 50)};
            MasuToolUnits = new ObservableCollection<MasuToolUnit>(masuToolUnits);
            SelectedUnit = masuToolUnits.First();
        }
        
        #endregion
        
        #region アクション

        /// <summary>OKアクション</summary>
        public Action OKAction { get; set; }

        // <summary>コピーアクション</summary>
        public Action CopyAction { get; set; } = null!;

        public void HeightChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;
            
            try
            {
                isUpdating = true;
                foreach (var masuSetting in MasuSetting)
                {
                    masuSetting.HeightChanged -= BottomHeightChanged;
                    masuSetting.BottomHeight.HeightChanged -= UnitChanged;
                }
                ManholeUtl.CalcBottomHeight(MasuSetting, false, SelectedUnit);
                foreach (var masuSetting in MasuSetting)
                {
                    masuSetting.HeightChanged += BottomHeightChanged;
                    masuSetting.BottomHeight.HeightChanged += UnitChanged;
                }
            }
            finally
            {
                isUpdating = false;
            }
        }

        public void UnitChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;
            
            try
            {
                isUpdating = true;
                foreach (var masuSetting in MasuSetting)
                {
                    masuSetting.MasuItem!.HeightChanged -= HeightChanged!;
                    masuSetting.HeightChanged -= BottomHeightChanged;
                }
                ManholeUtl.CalcBottomHeight(MasuSetting, false, SelectedUnit);
                foreach (var masuSetting in MasuSetting)
                {
                    masuSetting.MasuItem!.HeightChanged += HeightChanged!;
                    masuSetting.HeightChanged += BottomHeightChanged;
                }
            }
            finally
            {
                isUpdating = false;
            }
        }

        public void BottomHeightChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;
            
            try
            {
                isUpdating = true;
                foreach (var masuSetting in MasuSetting)
                {
                    masuSetting.MasuItem!.HeightChanged -= HeightChanged;
                    masuSetting.BottomHeight.HeightChanged -= UnitChanged;
                }
                ManholeUtl.CalcBottomHeight(MasuSetting, true, SelectedUnit);
                foreach (var masuSetting in MasuSetting)
                {
                    masuSetting.MasuItem!.HeightChanged += HeightChanged;
                    masuSetting.BottomHeight.HeightChanged += UnitChanged;
                }
            }
            finally
            {
                isUpdating = false;
            }
        }

        public void CheckBottomHeightChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;
            
            try
            {
                isUpdating = true;

                // Temporarily unsubscribe from events to prevent loops
                foreach (var masuSetting in MasuSetting)
                {
                    if (masuSetting.MasuItem != null)
                        masuSetting.MasuItem.HeightChanged -= HeightChanged;
                    if (masuSetting.BottomHeight != null)
                        masuSetting.BottomHeight.HeightChanged -= UnitChanged;
                    masuSetting.HeightChanged -= BottomHeightChanged;
                }

                // Update BottomHeight values based on CheckBottomHeight
                foreach (var masuSetting in MasuSetting)
                {
                    if (masuSetting.CheckBottomHeight != null)
                    {
                        if(masuSetting.CheckBottomHeight.Value != 0)
                            masuSetting.BottomHeight.Value = masuSetting.CheckBottomHeight.Value;
                        else {
                            masuSetting.BottomHeight.Value = masuSetting.RequiredBottomHeight.Value ;
                        }
                    }
                }
                
                // Recalculate bottom heights
                ManholeUtl.CalcBottomHeight(MasuSetting, true, SelectedUnit);
                
                // Resubscribe to events
                foreach (var masuSetting in MasuSetting)
                {
                    if (masuSetting.MasuItem != null)
                        masuSetting.MasuItem.HeightChanged += HeightChanged;
                    if (masuSetting.BottomHeight != null)
                        masuSetting.BottomHeight.HeightChanged += UnitChanged;
                    masuSetting.HeightChanged += BottomHeightChanged;
                }
            }
            finally
            {
                isUpdating = false;
            }
        }
        
        private void BulkSettingChanged(object sender, EventArgs e)
        {
            Console.WriteLine(sender.ToString());
        }

        public void ManholeSymbolChanged(object sender, EventArgs e)
        {
            var firstSymbol = MasuSetting.First().MasuItem.MasuSymbol;
            var lastSymbol = MasuSetting.Last().MasuItem.MasuSymbol;

            this.DraftingViewName = $"Manhole profile_{firstSymbol}-{lastSymbol}_full scale_{DateTime.Now.ToString("yyyyMMdd")}";
            this.DraftingViewNameFixLenght = $"Manhole profile_{firstSymbol}-{lastSymbol}_{DateTime.Now.ToString("yyyyMMdd")}";
        }

        #endregion
    }
}