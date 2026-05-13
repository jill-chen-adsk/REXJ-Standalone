using Autodesk.Revit.DB ;
using Autodesk.Revit.UI.Selection ;

namespace MEPConnectTool.SelectionFilter ;

public class CategoryFilter : ISelectionFilter
{
  private readonly BuiltInCategory _category ;

  public CategoryFilter( BuiltInCategory category )
  {
    _category = category ;
  }

  public bool AllowElement( Element elem )
  {
    if ( elem.Category == null ) return false ;
    
    return elem.Category.Id.Value == (long)_category ;
  }

  public bool AllowReference( Reference reference, XYZ position )
  {
    return false ;
  }
}