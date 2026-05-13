using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Structure;
using MepDuctPipeTool.Commands.InsertPipeAccessoryWithFlange;
using MepDuctPipeTool.Utils;

namespace MepDuctPipeTool.RevitDBAccess
{
  internal abstract class FamilyCollectorBase
  {
    internal IEnumerable<Family> Collect( Document document )
    {
      // FamilyInstanceがView上に無いときでも収集できる方法で実装
      // symbolをインスタンス化して、それが対象のファミリの条件を満たすかを見る

      var familySymbols = CollectSymbols( document );

      var families = new List<Family>();
      using var trans = new Transaction( document, "temp" );
      trans.Start();

      // TODO チェックしたファミリをとっておき、既にチェックしていたらcontinueする
      foreach ( var symbol in familySymbols )
      {
        if ( ! symbol.IsActive )
        {
          symbol.Activate();
        }

        // ファミリインスタンスを作成して対象のファミリかを確認
        var instance = document.Create.NewFamilyInstance( XYZ.Zero, symbol, StructuralType.NonStructural );
        if ( IsTargetFamily( instance ) )
        {
          families.Add( symbol.Family );
        }
      }

      trans.RollBack();
      return families.Distinct( new FamilyComparer() ).ToArray();
    }

    protected abstract IEnumerable<FamilySymbol> CollectSymbols( Document document );
    protected abstract bool IsTargetFamily( FamilyInstance instance );
  }

  internal class FlangeFamilyCollector : FamilyCollectorBase
  {
    protected override IEnumerable<FamilySymbol> CollectSymbols( Document document )
    {
      var fittingSymbols = new FilteredElementCollector( document )
        .OfCategory( BuiltInCategory.OST_PipeFitting )
        .WhereElementIsElementType()
        .OfType<FamilySymbol>()
        .ToArray(); // 遅延評価だと例外が発生するためここで即時評価する

      return fittingSymbols.Where( s => DoesNameIncludeFlange( s.Family ) );

      static bool DoesNameIncludeFlange( Family family )
        => Regex.IsMatch( family.Name, @"フランジ" );
    }

    protected override bool IsTargetFamily( FamilyInstance instance )
    {
      // Revit APIの仕様で、PartType.Flangeの径がAPIで変更できないため、ユーザにPartType.Union版のフランジを作ってもらう運用にしている。
      // そのため、PartType.Unionの継ぎ手ファミリを収集する。
      return instance.MEPModel is MechanicalFitting { PartType: PartType.Union };
    }
  }

  internal class PipeAccessoryFamilyCollector : FamilyCollectorBase
  {
    protected override IEnumerable<FamilySymbol> CollectSymbols( Document document )
    {
      return new FilteredElementCollector( document )
        .OfClass( typeof( FamilySymbol ) )
        .OfCategory( BuiltInCategory.OST_PipeAccessory )
        .Cast<FamilySymbol>()
        .Distinct( new FamilySymbolComparer() );
    }

    protected override bool IsTargetFamily( FamilyInstance instance )
    {
      // バルブ-割り込みの二方・三方弁を対象とする。コネクタの数で判別する。
      return instance.GetConnectors().Count() is 2 or 3;
    }
  }
}