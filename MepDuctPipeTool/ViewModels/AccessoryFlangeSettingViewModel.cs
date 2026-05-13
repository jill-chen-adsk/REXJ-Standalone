using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Autodesk.Revit.DB;
using MepDuctPipeTool.Models;

namespace MepDuctPipeTool.ViewModels
{
  public class AccessoryFlangeSettingViewModel : INotifyPropertyChanged
  {
    private bool? _isPanelControlEnabled;

    public readonly AccessoryFlangeSettingModel Model;
    public bool NeedsRefresh = false;

    public ObservableCollection<Family> AccessoryFamilies { get; set; }
    public ObservableCollection<Family> FlangeFamilies { get; set; }
    public ObservableCollection<FamilySymbol> AccessorySymbols { get; set; }
    public ObservableCollection<FamilySymbol> FlangeSymbols { get; set; }

    public bool IsPanelControlEnabled
    {
      get => _isPanelControlEnabled ?? false;
      set
      {
        _isPanelControlEnabled = value;
        OnPropertyChanged( nameof( IsPanelControlEnabled ) );
      }
    }

    public Family? SelectedAccessoryFamily
    {
      get => Model.SelectedAccessoryFamily;
      set
      {
        Model.SelectedAccessoryFamily = value;
        OnPropertyChanged( nameof( SelectedAccessoryFamily ) );
        UpdateAccessorySymbols();
      }
    }

    public Family? SelectedFlangeFamily
    {
      get => Model.SelectedFlangeFamily;
      set
      {
        Model.SelectedFlangeFamily = value;
        OnPropertyChanged( nameof( SelectedFlangeFamily ) );
        UpdateFlangeSymbols();
      }
    }

    public FamilySymbol? SelectedAccessorySymbol
    {
      get => Model.SelectedAccessorySymbol;
      set
      {
        Model.SelectedAccessorySymbol = value;
        OnPropertyChanged( nameof( SelectedAccessorySymbol ) );
      }
    }

    public FamilySymbol? SelectedFlangeSymbol
    {
      get => Model.SelectedFlangeSymbol;
      set
      {
        Model.SelectedFlangeSymbol = value;
        OnPropertyChanged( nameof( SelectedFlangeSymbol ) );
      }
    }


    #region ComboboxDisplayText

    // ComboboxDisplayText: コンボボックス未選択時のHintテキストを表示するための仕組み。他にうまい方法があれば作り変える。

    private string _accessoryFamilyDisplayText = Resources.COMBOBOX_TXT_PROMPT_SELECT_FAMILY;

    public string AccessoryFamilyDisplayText
    {
      get => _accessoryFamilyDisplayText;
      set
      {
        _accessoryFamilyDisplayText = value;
        OnPropertyChanged( nameof( AccessoryFamilyDisplayText ) );
      }
    }

    private string _accessorySymbolDisplayText = Resources.COMBOBOX_TXT_PROMPT_SELECT_TYPE;

    public string AccessorySymbolDisplayText
    {
      get => _accessorySymbolDisplayText;
      set
      {
        _accessorySymbolDisplayText = value;
        OnPropertyChanged( nameof( AccessorySymbolDisplayText ) );
      }
    }


    private string _flangeFamilyDisplayText = Resources.COMBOBOX_TXT_PROMPT_SELECT_FAMILY;

    public string FlangeFamilyDisplayText
    {
      get => _flangeFamilyDisplayText;
      set
      {
        _flangeFamilyDisplayText = value;
        OnPropertyChanged( nameof( FlangeFamilyDisplayText ) );
      }
    }

    private string _flangeSymbolDisplayText = Resources.COMBOBOX_TXT_PROMPT_SELECT_TYPE;

    public string FlangeSymbolDisplayText
    {
      get => _flangeSymbolDisplayText;
      set
      {
        _flangeSymbolDisplayText = value;
        OnPropertyChanged( nameof( FlangeSymbolDisplayText ) );
      }
    }

    #endregion


    public event PropertyChangedEventHandler? PropertyChanged;

    public AccessoryFlangeSettingViewModel( AccessoryFlangeSettingModel model )
    {
      Model = model;
      AccessoryFamilies = new ObservableCollection<Family>( Model.AccessoryFamilies );
      FlangeFamilies = new ObservableCollection<Family>( Model.FlangeFamilies );
      AccessorySymbols = new ObservableCollection<FamilySymbol>( Model.AccessorySymbols );
      FlangeSymbols = new ObservableCollection<FamilySymbol>( Model.FlangeSymbols );

      // modelに既に選択項目が登録されている場合はそれを反映
      SelectedAccessoryFamily = Model.SelectedAccessoryFamily;
      SelectedAccessorySymbol = Model.SelectedAccessorySymbol;
      SelectedFlangeFamily = Model.SelectedFlangeFamily;
      SelectedFlangeSymbol = Model.SelectedFlangeSymbol;
      AccessoryFamilyDisplayText = SelectedAccessoryFamily?.Name ?? Resources.COMBOBOX_TXT_PROMPT_SELECT_FAMILY;
      AccessorySymbolDisplayText = SelectedAccessorySymbol?.Name ?? Resources.COMBOBOX_TXT_PROMPT_SELECT_TYPE;
      FlangeFamilyDisplayText = SelectedFlangeFamily?.Name ?? Resources.COMBOBOX_TXT_PROMPT_SELECT_FAMILY;
      FlangeSymbolDisplayText = SelectedFlangeSymbol?.Name ?? Resources.COMBOBOX_TXT_PROMPT_SELECT_TYPE;
    }

    private void OnPropertyChanged( string propertyName )
    {
      PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    private void UpdateAccessorySymbols()
    {
      AccessorySymbols.Clear();
      var symbols = Model.AccessorySymbols;
      symbols.ToList().ForEach( AccessorySymbols.Add );
    }

    private void UpdateFlangeSymbols()
    {
      FlangeSymbols.Clear();
      var symbols = Model.FlangeSymbols;
      symbols.ToList().ForEach( FlangeSymbols.Add );
    }

    public void OnPanelCommandExecuted() => IsPanelControlEnabled = false;
    public void OnPanelCommandFinished() => IsPanelControlEnabled = true;
    public void OnBeforeSelectionCommand() => IsPanelControlEnabled = true;
  }
}