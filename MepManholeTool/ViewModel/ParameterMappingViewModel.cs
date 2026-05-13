using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure ;
using Autodesk.Revit.UI;
using MepManholeTool.Commands;
using MepManholeTool.Model;
using MepManholeTool.Models;
using MepManholeTool.Properties;

namespace MepManholeTool.ViewModel
{
    public partial class ParameterMappingViewModel : INotifyPropertyChanged
    {
        private readonly UIDocument _uiDocument;
        private Family _selectedFamily;
        private ObservableCollection<Family> _families;
        private ObservableCollection<ParameterMappingModel> _parameterMappings;
        private ObservableCollection<string> _availableParameters;
        private MasuParameter _selectedParameter;
        private Window _window;

        public event PropertyChangedEventHandler? PropertyChanged;

        #region Initialize
        public ParameterMappingViewModel(UIDocument uiDocument)
        {
            _uiDocument = uiDocument;
            _parameterMappings = new ObservableCollection<ParameterMappingModel>();
            _availableParameters = new ObservableCollection<string>();
            LoadFamilies();
            LoadParameterMappings();
            OKCommand = new ManholeToolCommand(() => true, OKExecute);
            CancelCommand = new ManholeToolCommand(() => true, CancelExecute);
        }

        /// <summary>
        /// プロジェクト内の桝ファミリを取得する
        /// </summary>
        private void LoadFamilies()
        {
            var doc = _uiDocument.Document;
            var collector = new FilteredElementCollector(doc);
            var families = collector.OfClass(typeof(Family))
                                 .Cast<Family>()
                                 .Where(f => f.Name.Contains("桝"))
                                 .OrderBy(f => f.Name)
                                 .ToList();

            Families = new ObservableCollection<Family>(families);
        }

        private void LoadParameterMappings()
        {
            ParameterMappings.Clear();

            if (GlobalMappings.Instance.Manholes != null)
            {
                foreach (var item in GlobalMappings.Instance.Manholes.First().Mapping)
                {
                    ParameterMappings.Add(new ParameterMappingModel { RevitCategory = item.Category, MasuSymbol = "", FromParameter = item.FromParameter, ToParameter = item.ToParameter, Required = item.Required });
                }
            }
        }
        #endregion

        #region プロパティ
        public ObservableCollection<Family> Families
        {
            get => _families;
            set
            {
                _families = value;
                OnPropertyChanged();
            }
        }

        public Family SelectedFamily
        {
            get => _selectedFamily;
            set
            {
                _selectedFamily = value;
                OnPropertyChanged();
                LoadParameterSymbols() ;
            }
        }

        public ObservableCollection<ParameterMappingModel> ParameterMappings
        {
            get => _parameterMappings;
            set
            {
                _parameterMappings = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> AvailableParameters
        {
            get => _availableParameters;
            set
            {
                _availableParameters = value;
                OnPropertyChanged();
            }
        }
        
        public MasuParameter ParameterName
        {
            get => _selectedParameter;
            set
            {
                _selectedParameter = value;
                OnPropertyChanged(nameof(ParameterName));
            }
        }

        public Window Window
        {
            get => _window;
            set
            {
                _window = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region OKコマンド
        /// <summary>OKコマンドを取得します。</summary>
        public ICommand OKCommand { get; private set; }

        /// <summary>「OK」処理を実行します。</summary>
        private void OKExecute()
        {
            //Override global parameter mapping
            if(SelectedFamily is not null && ParameterMappings is not null)
            {
                // Check if all required parameters are mapped
                var unmappedRequiredParams = ParameterMappings
                    .Where(p => p.Required && string.IsNullOrWhiteSpace(p.ToParameter))
                    .ToList();

                if (unmappedRequiredParams.Any())
                {
                    var unmappedParamNames = string.Join("; ", unmappedRequiredParams.Select(p => p.FromParameter));
                    TaskDialog.Show(
                        "Mapping Error",
                        $"The following required parameters are not mapped:\n{unmappedParamNames}");
                    
                    // Bring window back to front after dialog is closed
                    Window?.Activate();
                    Window?.Focus();
                    
                    return;
                }

                List<ParameterMapping> Manholes = new List<ParameterMapping>();
                foreach (var item in ParameterMappings)
                {
                    Manholes.Add(new ParameterMapping { Category = item.RevitCategory, FromParameter = item.FromParameter, ToParameter = item.ToParameter, Required = item.Required });
                }
                if (Manholes.FirstOrDefault(x => !string.IsNullOrEmpty(x.ToParameter)) is not null) {
                    GlobalMappings.Instance.OverrideMapping(SelectedFamily.Name, Manholes);
                }

                if ( Manholes.Any() ) {
                    GlobalMappings.Instance.SaveMappings() ;
                }
                
                // Only close the form if validation passed
                this.OKAction.Invoke();
            }
        }
        #endregion

        #region　キャンセルコマンド

        /// <summary>Cancelコマンドを取得します。</summary>
        public ICommand CancelCommand { get; private set; }

        /// <summary>「Cancel」処理を実行します。</summary>
        private void CancelExecute()
        {
            CancelAction.Invoke();
        }
        #endregion

        #region アクション

        /// <summary>Nextアクション</summary>
        public Action OKAction { get; set; }
        
        /// <summary>Cancelアクション</summary>
        public Action CancelAction { get; set; }

        #endregion

        /// <summary>
        /// 選択したファミリに対してすでにマッピングしたパラメータを設定する
        /// </summary>
        private void LoadParameterSymbols()
        {
            AvailableParameters.Clear();
            var doc = _uiDocument.Document;
            
            if (SelectedFamily != null) {
                
                //一旦マッピング情報をクリアする
                foreach ( var mapping in ParameterMappings ) {
                    mapping.ToParameter = "" ;
                }
                
                //タイプパラメータを取得する
                var symbols = SelectedFamily.GetFamilySymbolIds();
                if (symbols != null)
                {
                    foreach (var item in symbols)
                    {
                        FamilySymbol symbol = doc.GetElement(item) as FamilySymbol;
                        foreach (Parameter param in symbol.Parameters)
                        {
                            if (param.IsShared && param.Definition != null)
                            {
                                Definition def = param.Definition;
                                if (def != null)
                                {
                                    AvailableParameters.Add(def.Name);
                                }
                            }    
                        }
                    }

                    //ファミリインスタンスのパラメータを取得する
                    FamilySymbol tmpSymbol = doc.GetElement(symbols.First()) as FamilySymbol;
                    if (tmpSymbol != null)
                    {
                        using (Transaction t = new Transaction(doc, "Get Family Instance's Parameters"))
                        {
                            t.Start();
                            tmpSymbol.Activate();
                            XYZ location = new XYZ(0, 0, 0);
                            Level level = new FilteredElementCollector(doc)
                                .OfClass(typeof(Level))
                                .Cast<Level>()
                                .FirstOrDefault();

                            var instance = doc.Create.NewFamilyInstance(location, tmpSymbol, level, StructuralType.NonStructural);
                            foreach (Parameter param in instance.Parameters)
                            {
                                if (param.Definition != null && !AvailableParameters.Contains(param.Definition.Name))
                                {
                                    AvailableParameters.Add(param.Definition.Name);
                                }
                            }

                            t.RollBack();
                        }
                    }
                }

                //マッピングされた場合、反映する
                var setting = GlobalMappings.Instance.Manholes.Where(x => x.Family == SelectedFamily.Name);
                if (!setting.Any()) {
                    setting = GlobalMappings.Instance.Manholes.Where(x => x.Family == "");
                }
                if (setting.Any())
                {
                    ParameterMappings?.Clear();
                    foreach (var item in setting.First().Mapping)
                    {
                        ParameterMappings.Add(new ParameterMappingModel { RevitCategory = item.Category, MasuSymbol = "", FromParameter = item.FromParameter, ToParameter = item.ToParameter, Required = item.Required });
                    }
                }
            }
        }
    }
} 