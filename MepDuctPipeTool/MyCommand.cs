using Autodesk.Revit.UI ;

namespace MepDuctPipeTool
{
  public interface MyCommand
  {
    public Result Execute( UIDocument uiDoc , bool isRibbon) ;
  }
}