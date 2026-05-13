using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using MepDuctPipeTool.RevitDBAccess;

namespace MepDuctPipeTool.Models
{
  public class AccessoryFlangeSettingModel
  {
    private readonly Document _document;
    public IReadOnlyCollection<Family> AccessoryFamilies { get; }
    public IReadOnlyCollection<Family> FlangeFamilies { get; }

    public IReadOnlyCollection<FamilySymbol> AccessorySymbols => GetChildSymbols( _document, SelectedAccessoryFamily ).OrderBy( s => s.Name ).ToArray();

    public IReadOnlyCollection<FamilySymbol> FlangeSymbols => GetChildSymbols( _document, SelectedFlangeFamily ).OrderBy( s => s.Name ).ToArray();

    private Family? _selectedAccessoryFamily;

    public Family? SelectedAccessoryFamily
    {
      get
      {
        if ( _selectedAccessoryFamily is null ) return null;
        return _selectedAccessoryFamily.IsValidObject ? _selectedAccessoryFamily : null;
      }
      set
      {
        if ( value is null )
        {
          _selectedAccessoryFamily = null;
          return;
        }

        _selectedAccessoryFamily = value.IsValidObject ? value : null;
      }
    }

    private Family? _selectedFlangeFamily;

    public Family? SelectedFlangeFamily
    {
      get
      {
        if ( _selectedFlangeFamily is null ) return null;
        return _selectedFlangeFamily.IsValidObject ? _selectedFlangeFamily : null;
      }
      set
      {
        if ( value is null )
        {
          _selectedFlangeFamily = null;
          return;
        }

        _selectedFlangeFamily = value.IsValidObject ? value : null;
      }
    }

    private FamilySymbol? _selectedAccessorySymbol;

    public FamilySymbol? SelectedAccessorySymbol
    {
      get
      {
        if ( _selectedAccessorySymbol is null ) return null;
        return _selectedAccessorySymbol.IsValidObject ? _selectedAccessorySymbol : null;
      }
      set
      {
        if ( value is null )
        {
          _selectedAccessorySymbol = null;
          return;
        }

        _selectedAccessorySymbol = value.IsValidObject ? value : null;
      }
    }

    private FamilySymbol? _selectedFlangeSymbol;

    public FamilySymbol? SelectedFlangeSymbol
    {
      get
      {
        if ( _selectedFlangeSymbol is null ) return null;
        return _selectedFlangeSymbol.IsValidObject ? _selectedFlangeSymbol : null;
      }
      set
      {
        if ( value is null )
        {
          _selectedFlangeSymbol = null;
          return;
        }

        _selectedFlangeSymbol = value.IsValidObject ? value : null;
      }
    }

    private AccessoryFlangeSettingModel( Document document, IEnumerable<Family> accessoryFamilies, IEnumerable<Family> flangeFamilies )
    {
      _document = document;
      AccessoryFamilies = accessoryFamilies.ToArray();
      FlangeFamilies = flangeFamilies.ToArray();
    }

    public static AccessoryFlangeSettingModel Create( Document document )
    {
      var flangeFamilyCollector = new FlangeFamilyCollector();
      var pipeAccessoryFamilyCollector = new PipeAccessoryFamilyCollector();

      var flangeFamilies = flangeFamilyCollector.Collect( document ).OrderBy( f => f.Name );
      var accessoryFamilies = pipeAccessoryFamilyCollector.Collect( document ).OrderBy( f => f.Name );
      return new AccessoryFlangeSettingModel( document, accessoryFamilies, flangeFamilies );
    }

    // TODO 別クラスに切り出す？
    private IEnumerable<FamilySymbol> GetChildSymbols( Document document, Family? family )
    {
      if ( family is null || ! family.IsValidObject ) return Enumerable.Empty<FamilySymbol>();
      return family.GetFamilySymbolIds().Select( id => document.GetElement( id ) as FamilySymbol ).Where( fs => fs != null )!;
    }

    internal bool HasDeletedFamily()
    {
      return HasDeleted( AccessoryFamilies ) || HasDeleted( FlangeFamilies );

      static bool HasDeleted( IReadOnlyCollection<Family> families )
        => ! families.All( f => f.IsValidObject ); // コレクションが空の場合はfalse
    }
  }
}