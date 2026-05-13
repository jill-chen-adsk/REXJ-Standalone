using System;
using System.Collections.Generic;
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

namespace MepDuctPipeTool.Commands.InsertPipeAccessoryWithFlange
{
  [ Transaction( TransactionMode.Manual ) ]
  internal class InsertPipeAccessoriesWithFlangeCommand : DockPanelCommandBase
  {
    private protected override string CommandName => Resources.COMMAND_NAME_INSERT_PIPE_ACCESSORIES_WITH_FLANGE;

    private protected override Result ExecuteImpl( UIDocument uiDoc, AccessoryFlangeSettingViewModel viewModel )
    {
      var document = uiDoc.Document;

      // 1. 配管付属品のタイプを選択
      // 2. フランジのタイプを選択
      if ( GetDialogOperationResult( viewModel ) is not { } uiResult )
      {
        MessageDialog.ShowWarning( CommandName, Resources.WARN_INVALID_FAMILT_TYPE_IS_SELECTED );
        ShowDockablePaneCmd.ShowDockablePane( uiDoc.Application );
        return Result.Cancelled;
      }

      // 3. 挿入先の配管の選択
      var pickedRef = uiDoc.Selection.PickObject( ObjectType.PointOnElement, new PipeFilter( uiDoc.Document ), Resources.STATUS_PROMPT_SLECT_PIPE );
      if ( document.GetElement( pickedRef ) is not Pipe pipe ) throw new InvalidOperationException( Resources.ERR_PIPE_IS_NOT_SELECTED );

      if ( pipe is not MEPCurve mepCurve ) throw new InvalidOperationException();

      // pick位置
      var pickedPosition = MepCurveUtils.ProjectToAxis( mepCurve, pickedRef.GlobalPoint ); // TODO 近すぎる場合警告を出す


      try
      {
        // 4-9. トランザクショングループで処理
        using var tg = new TransactionGroup( document, Resources.TRANSACTION_NAME_INSERT_PIPE_ACCESSORY_WITH_FLANGE );
        tg.Start();

        // 4-9. パイプを切断して切断位置に付属品とフランジを配置して移動して接続
        var inserter = new PipeAccessoryWithFlangeInserter( document, uiResult.accessorySymbol, uiResult.flangeSymbol );
        inserter.InsertTo( pipe, pickedPosition );

        tg.Assimilate();
      }
      catch ( FlangeOriginIsNotOnConnectorException )
      {
        MessageDialog.ShowWarning( CommandName, Resources.WARN_FLANGE_FAMILY_ORIGIN_IS_NOT_ON_CONNECTOR );
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

      return Result.Succeeded;
    }

    private static (FamilySymbol accessorySymbol, FamilySymbol flangeSymbol)? GetDialogOperationResult( AccessoryFlangeSettingViewModel viewModel )
    {
      var selectedAccessorySymbol = viewModel.Model.SelectedAccessorySymbol;
      var selectedFlangeSymbol = viewModel.Model.SelectedFlangeSymbol;

      if ( selectedAccessorySymbol is null || selectedFlangeSymbol is null ) return null;
      if ( ! selectedAccessorySymbol.IsValidObject || ! selectedFlangeSymbol.IsValidObject ) return null;
      return ( selectedAccessorySymbol, selectedFlangeSymbol );
    }


    private class PipeFilter : ISelectionFilter
    {
      private readonly Document _document;

      public PipeFilter( Document document )
      {
        _document = document;
      }

      public bool AllowElement( Element elem )
      {
        return elem is Pipe;
      }

      public bool AllowReference( Reference reference, XYZ position )
      {
        if ( _document.GetElement( reference ) is not Pipe pipe ) throw new ArgumentException();
        var pickedPosition = MepCurveUtils.ProjectToAxis( pipe, reference.GlobalPoint );
        return ! PickPositionEvaluator.IsPickPositionBeyondPipeEnd( pickedPosition, pipe, 1.0 ); // marginの値は経験的に決めた
      }
    }
  }


  // .Netのバージョンを上げればDistinctByが使えるのでこれらComparerは不要になる
  public class FamilySymbolComparer : IEqualityComparer<FamilySymbol>
  {
    public bool Equals( FamilySymbol? x, FamilySymbol? y )
    {
      if ( x is null || y is null ) return false;
      return x.Id == y.Id;
    }

    public int GetHashCode( FamilySymbol? obj )
    {
      return obj is null ? 0 : obj.Id.GetHashCode();
    }
  }

  public class FamilyComparer : IEqualityComparer<Family>
  {
    public bool Equals( Family? x, Family? y )
    {
      if ( x is null || y is null ) return false;
      return x.Id == y.Id;
    }

    public int GetHashCode( Family? obj )
    {
      return obj is null ? 0 : obj.Id.GetHashCode();
    }
  }
}