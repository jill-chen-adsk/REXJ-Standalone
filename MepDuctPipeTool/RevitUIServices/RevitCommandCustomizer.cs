using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace MepDuctPipeTool.RevitUIServices
{
  // TODO 共通部分を継承か委譲で切り出し。3つのCustomizerを別クラスに切り出す。
  public class RevitCommandCustomizer
  {
    private readonly UIControlledApplication _application;

    // private const string PipeCommandLookupId = "ID_RBS_PIPE_PIPE";
    // private const string PipeCreateCommandLookupId = "IDS_RBS_CREATE_PIPE";
    private const string SelectCommandLookupId = "ID_BUTTON_SELECT";

    // private static readonly RevitCommandId CommandIdPipe; // Stored statically to allow for removal of the command binding.
    // private static readonly RevitCommandId CommandIdCreatePipe;
    private static readonly RevitCommandId CommandIdSelect;

    // public EventHandler<BeforeExecutedEventArgs>? BeforePipeCommandEvent;
    // public EventHandler<BeforeExecutedEventArgs>? BeforePipeCreationCommandEvent;
    public EventHandler<BeforeExecutedEventArgs>? BeforeSelectionCommandEvent;

    static RevitCommandCustomizer()
    {
      // Initialize static fields in static constructor
      // CommandIdPipe = RevitCommandId.LookupCommandId( PipeCommandLookupId ) ?? throw new InvalidOperationException();
      // CommandIdCreatePipe = RevitCommandId.LookupCommandId( PipeCreateCommandLookupId ) ?? throw new InvalidOperationException();
      CommandIdSelect = RevitCommandId.LookupCommandId( SelectCommandLookupId ) ?? throw new InvalidOperationException();
    }

    public RevitCommandCustomizer( UIControlledApplication application )
    {
      _application = application;
    }


    public void RegisterBeforeExecuteEventHandlers()
    {
      // if ( BeforePipeCommandEvent is not null ) CreateCommandBinding( CommandIdPipe, BeforePipeCommandEvent! );
      // if ( BeforePipeCreationCommandEvent is not null ) CreateCommandBinding( CommandIdCreatePipe, BeforePipeCreationCommandEvent! );
      if ( BeforeSelectionCommandEvent is not null ) CreateCommandBinding( CommandIdSelect, BeforeSelectionCommandEvent! );
    }


    private void CreateCommandBinding( RevitCommandId? commandId, EventHandler<BeforeExecutedEventArgs> handler )
    {
      try
      {
        var commandBinding = _application.CreateAddInCommandBinding( commandId );
        commandBinding.BeforeExecuted += handler;
      }
      // Most likely, this is because someone else has bound this command already.
      catch ( Exception )
      {
        MessageDialog.ShowError( Resources.ADDIN_NAME, Resources.ERR_IN_COMMAND_CUSTOMIZER );
        throw;
      }
    }

    public void RemoveBeforeExecuteEventHandlers()
    {
      // RemoveCommandBinding( _application, CommandIdPipe );
      // RemoveCommandBinding( _application, CommandIdCreatePipe );
      RemoveCommandBinding( _application, CommandIdSelect );
    }

    private void RemoveCommandBinding( UIControlledApplication application, RevitCommandId commandId )
    {
      if ( commandId.HasBinding ) application.RemoveAddInCommandBinding( commandId );
    }
  }
}