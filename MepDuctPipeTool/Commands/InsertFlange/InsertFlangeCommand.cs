using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MepDuctPipeTool.Geometry;
using MepDuctPipeTool.RevitDBAccess;
using MepDuctPipeTool.RevitUIServices;
using MepDuctPipeTool.Utils;
using MepDuctPipeTool.ViewModels;

namespace MepDuctPipeTool.Commands.InsertFlange
{
  [ Transaction( TransactionMode.Manual ) ]
  internal class InsertFlangeCommand : DockPanelCommandBase
  {
    private protected override string CommandName => Resources.COMMAND_NAME_INSERT_FLANGE;

    private protected override Result ExecuteImpl( UIDocument uiDoc, AccessoryFlangeSettingViewModel viewModel )
    {
      var document = uiDoc.Document;

      // 1. 配管付属品のタイプを選択
      // 2. フランジのタイプを選択
      if ( GetDialogOperationResult( viewModel ) is not { } flangeSymbol )
      {
        MessageDialog.ShowWarning( CommandName, Resources.WARN_INVALID_FAMILT_TYPE_IS_SELECTED );
        ShowDockablePaneCmd.ShowDockablePane( uiDoc.Application );
        return Result.Cancelled;
      }

      // 3. 挿入先の配管の選択
      var pickedRef = uiDoc.Selection.PickObject( ObjectType.PointOnElement, new PipeAndAccessoryFilter( document ), Resources.STATUS_PROMPT_SELECT_PIPE_OR_ACCESSORY );
      var pickedElement = document.GetElement( pickedRef );

      // pipe ->      配管を分割してフランジ挿入
      // accessory -> 配管付属品の端部にフランジ挿入
      try
      {
        switch ( pickedElement )
        {
          case Pipe pipe:
            InsertFlangeOnPipe( document, pipe, pickedRef.GlobalPoint, flangeSymbol );
            return Result.Succeeded;

          case FamilyInstance accessory:
            InsertFlangeOnAccessorySide( document, accessory, flangeSymbol );

            return Result.Succeeded;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
      catch ( FlangeOriginIsNotOnConnectorException )
      {
        MessageDialog.ShowWarning( CommandName, Resources.WARN_FLANGE_FAMILY_ORIGIN_IS_NOT_ON_CONNECTOR );
        return Result.Cancelled;
      }
      catch ( BothSidePipeSlopesDifferentException )
      {
        if ( pickedElement is FamilyInstance )
        {
          MessageDialog.ShowWarning( CommandName, Resources.WARN_PIPE_SLOPE_IS_UNMATCH );
        }

        return Result.Cancelled;
      }
      catch ( Autodesk.Revit.Exceptions.InvalidOperationException e )
      {
        if ( ! e.Message.StartsWith( "The parameter is read-only." ) ) throw;
        MessageDialog.ShowWarning( CommandName, Resources.WARN_RADIUS_PARAMETER_IS_READONLY );
        return Result.Cancelled;
      }
      catch ( ConnectorPositionMismatchException )
      {
        MessageDialog.ShowErrorWithManualLink( CommandName, Resources.ERR_CONNECTOR_POS_MISMATCH );
        return Result.Cancelled;
      }
    }

    private static void InsertFlangeOnPipe( Document document, Pipe pipe, XYZ insertPoint, FamilySymbol flangeSymbol )
    {
      using var tg = new TransactionGroup( document, Resources.TRANSACTION_NAME_INSERT_FLANGE );
      tg.Start();
      var pickedPosition = MepCurveUtils.ProjectToAxis( pipe, insertPoint );
      var inserter = new FlangeOnPipeInserter( document, flangeSymbol );
      inserter.InsertTo( pipe, pickedPosition );
      tg.Assimilate();
    }

    private static void InsertFlangeOnAccessorySide( Document document, FamilyInstance accessory, FamilySymbol flangeSymbol )
    {
      using var tg = new TransactionGroup( document, Resources.TRANSACTION_NAME_INSERT_PIPE_ACCESSORY_WITH_FLANGE );
      tg.Start();
      var inserter = new FlangeOnAccessorySideInserter( document, flangeSymbol );
      inserter.AttachTo( accessory );
      tg.Assimilate();
    }

    private static FamilySymbol? GetDialogOperationResult( AccessoryFlangeSettingViewModel viewModel )
    {
      var selectedFlangeSymbol = viewModel.Model.SelectedFlangeSymbol;
      if ( selectedFlangeSymbol is null || ! selectedFlangeSymbol.IsValidObject ) return null;

      return selectedFlangeSymbol;
    }

    private class PipeAndAccessoryFilter : ISelectionFilter
    {
      private readonly Document _document;

      public PipeAndAccessoryFilter( Document document )
      {
        _document = document;
      }

      public bool AllowElement( Element elem )
      {
        if ( elem.Category is null ) return false;
        var accessoryCategoryId = Category.GetCategory( _document, BuiltInCategory.OST_PipeAccessory ).Id;
        return elem is Pipe || elem.Category.Id == accessoryCategoryId;
      }

      public bool AllowReference( Reference reference, XYZ position )
      {
        if ( _document.GetElement( reference ) is not Pipe pipe ) return true; // 配管付属品の場合、True
        var pickedPosition = MepCurveUtils.ProjectToAxis( pipe, reference.GlobalPoint );
        return ! PickPositionEvaluator.IsPickPositionBeyondPipeEnd( pickedPosition, pipe, 0.5 ); // marginの値は経験的に決めた
      }
    }
  }
}