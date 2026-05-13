using System;
using Autodesk.Revit.DB;

namespace MepDuctPipeTool.Utils
{
  public static class DocumentExtensions
  {
    public static TElement? GetElementById<TElement>( this Document document, Guid uniqueId ) where TElement : Element
    {
      return GetElementById<TElement>( document, uniqueId.ToString() );
    }

    public static TElement? GetElementById<TElement>( this Document document, string uniqueId ) where TElement : Element
    {
      if ( string.IsNullOrEmpty( uniqueId ) ) return null;

      try
      {
        return document.GetElement( uniqueId ) as TElement;
      }
      catch
      {
        return null; // already deleted
      }
    }

    public static TElement? GetElementById<TElement>( this Document document, ElementId elementId ) where TElement : Element
    {
      if ( ElementId.InvalidElementId == elementId ) return null;
      return document.GetElement( elementId ) as TElement;
    }
  }
}