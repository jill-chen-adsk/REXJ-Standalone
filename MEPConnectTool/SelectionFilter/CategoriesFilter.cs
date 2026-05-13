using System.Collections.Generic ;
using System.Linq ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI.Selection ;

namespace MEPConnectTool.SelectionFilter ;

public class CategoriesFilter : ISelectionFilter
{
  private readonly List<BuiltInCategory> _categories ;

  public CategoriesFilter( List<BuiltInCategory> categories )
  {
    _categories = categories ;
  }

  public bool AllowElement( Element elem )
  {
    if ( elem.Category == null ) return false ;

    return _categories.Any( x => (long)x == elem.Category.Id.Value ) ;
  }

  public bool AllowReference( Reference reference, XYZ position )
  {
    return false ;
  }
}