using Autodesk.Revit.DB;

namespace MepDuctPipeTool.Utils
{
  public static class FamilySymbolUtils
  {
    public static void ActivateSymbolIfNeeded( Document document, FamilySymbol familySymbol )
    {
      if ( ! familySymbol.IsValidObject ) return;
      if ( familySymbol.IsActive ) return;
      using var trans = new Transaction( document, "Activate symbol" );
      trans.Start();
      familySymbol.Activate();
      trans.Commit();
    }
  }
}