using MepDuctPipeTool.RevitUIServices;

namespace MepDuctPipeTool.Utils
{
  internal static class CommandUtils
  {
    internal static void DisplayNotExecutedErrorMessage( string commandName )
    {
      MessageDialog.ShowError( commandName, Resources.ERR_CANNOT_EXECUTE_COMMAND );
    }
  }
}