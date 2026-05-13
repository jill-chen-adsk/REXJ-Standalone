using System ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using MEPConnectTool.RibbonPanel ;

namespace MEPConnectTool
{
  public class App: IExternalApplication, IExternalCommandAvailability
  {
    public Result OnStartup( UIControlledApplication application )
    {
      var tabName = "REXJ Standalone" ;

      try {
        application.CreateRibbonTab( tabName ) ;
      }
      catch {
      }
      
      var panel = new MyRibbonPanel( application ) ;
      return Result.Succeeded ;
    }
    public Result OnShutdown( UIControlledApplication application )
    {
      return Result.Succeeded ;
    }
    public bool IsCommandAvailable( UIApplication a, CategorySet b )
    {
      // if ( ! a.Application.IsMechanicalEnabled ) return false ;

      if ( a.ActiveUIDocument != null ) {
        return ! a.ActiveUIDocument.Document.IsFamilyDocument ;
      }

      return false ;
    }
    
  }
  
}