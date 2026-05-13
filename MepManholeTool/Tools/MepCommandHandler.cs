using System;
using System.Linq ;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MepManholeTool.Const ;
using MepManholeTool.Models;
using MepManholeTool.Utils;
using MepManholeTool.ViewModel ;

namespace MepManholeTool.Tools
{
  public class MepModelLineCommandHandler : IExternalEventHandler
  {
    public SetupParameterViewModel Context { get ; set ; }
    public Document RevitDoc { get ; set ; }
    
    public void Execute(UIApplication app)
    {
      UIDocument uidoc = app.ActiveUIDocument;
      Document doc = uidoc.Document;

      using ( TransactionGroup tg = new TransactionGroup( RevitDoc, "REXJ MEP 桝ツール: ビュー作成") ) {
        tg.Start() ;
        
        //Clear old connection
                    ManholeUtl.ClearConnection( doc, doc.ActiveView, Context.MasuSetting ) ;
        
        using ( Transaction tx = new Transaction( RevitDoc, $"Update Parameters" ) ) {
            try {
                tx.Start() ;
                
                foreach (RoutingParameter routingParameter in Context.MasuSetting)
                {
                    //パラメータ更新
                    ManholeUtl.UpdateParameters(RevitDoc, routingParameter, Context.OverrideLevelHeight ) ;

                    //桝深さ
                    var puddle = routingParameter.MasuItem!.MudPuddle.HasValue ? routingParameter.MasuItem!.MudPuddle.Value : 0;
                    routingParameter.Depth = -routingParameter.BottomHeight.MmValue + puddle + routingParameter.MasuItem!.HeightFromGroundLevel;
                }

                tx.Commit() ;
            }
            catch ( Exception e ) {
                tx?.RollBack();
            }
        }

        var fromOrgLoc = (Context.MasuSetting.Last().MasuItem!.MasuElement as FamilyInstance)?.GetTotalTransform()
            .Origin;
        var fromLoc = (Context.MasuSetting.Last().MasuItem!.MasuElement!.Location as LocationPoint)!;
        var fromBottomHeight = Context.MasuSetting.Last().BottomHeight;
        var fromHeightBase = Context.MasuSetting.Last().MasuItem!.HeightFromBase;

        // Check if tag creation is successful, if not, rollback and end the command
        bool tagCreationSuccess = ManholeUtl.SetSymbol( uidoc, Context.MasuSetting.Select(x => x.MasuItem).ToList(), Context.TagId);
        if (!tagCreationSuccess)
        {
            tg.RollBack();
            return;
        }
        
        for (int i = Context.MasuSetting.Count - 1; i >= 0; i--)
        {
            var toOrgLoc = (Context.MasuSetting[i].MasuItem!.MasuElement as FamilyInstance)!.GetTotalTransform().Origin;
            var toLoc = (Context.MasuSetting[i].MasuItem!.MasuElement!.Location as LocationPoint)!;
            var toBottomHeight = Context.MasuSetting[i].BottomHeight;
            var toHeightBase = Context.MasuSetting[i].MasuItem!.HeightFromBase;
            var orgRotation = fromLoc.Rotation;

            // モデル線分を作成する
            if (Math.Round(Context.MasuSetting[i].PipeLength, 0) != 0)
            {
                if (fromOrgLoc != null)
                {
                    ManholeUtl.CreateLine( RevitDoc,
                        fromOrgLoc - new XYZ( 0, 0,
                        UnitUtils.ConvertToInternalUnits( - fromBottomHeight.MmValue + fromHeightBase,
                            UnitTypeId.Millimeters ) ),
                        toOrgLoc - new XYZ( 0, 0,
                        UnitUtils.ConvertToInternalUnits( - toBottomHeight.MmValue + toHeightBase,
                            UnitTypeId.Millimeters ) ) ) ;
                }
            }

            // 回転・流入・流出角度調整
            if (i != Context.MasuSetting.Count - 1 && fromOrgLoc != null)
            {
                XYZ vector = toOrgLoc - fromOrgLoc;
                vector = new XYZ(vector.X, vector.Y, 0).Normalize();
                XYZ right = XYZ.BasisY.Normalize();
                double angleRad = vector.AngleOnPlaneTo(right, XYZ.BasisZ);
                double angleDe = 180 * angleRad / Math.PI;

                bool isMaruMasu = Context.MasuSetting[ i + 1 ].MasuItem.MasuElement
                    .LookupParameter( MepModelLineConst.桝パラメータ_配管段差_丸 ) != null ;
                // 桝の回転(丸桝)
                if ( isMaruMasu ) {
                    ManholeUtl.RotateElement( RevitDoc, Context.MasuSetting[ i + 1 ].MasuItem!.MasuElement.Id,
                        fromLoc.Point, fromLoc.Point,
                        angleRad < Math.PI ? Math.PI * ( 90 - angleDe ) / 180 : Math.PI / 2 - angleRad,
                        orgRotation ) ;
                }

                // 流出角度調整
                isMaruMasu = Context.MasuSetting[ i ].MasuItem.MasuElement
                    .LookupParameter( MepModelLineConst.桝パラメータ_配管段差_丸 ) != null ;
                if (i != 0)
                {
                    if ( ! isMaruMasu ) {
                        var startLoc = toOrgLoc + XYZ.BasisY;
                        ManholeUtl.AdjOutAngle(RevitDoc, Context.MasuSetting[i], startLoc, toOrgLoc, fromOrgLoc, toLoc.Rotation );

                        startLoc = toOrgLoc + XYZ.BasisY ;
                        ManholeUtl.AdjInAngle(RevitDoc, Context.MasuSetting[i], startLoc, toOrgLoc, (Context.MasuSetting[i - 1].MasuItem!.MasuElement as FamilyInstance)!.GetTotalTransform().Origin, toLoc.Rotation );                                    
                    }
                    else {
                        var startLoc = (Context.MasuSetting[i - 1].MasuItem!.MasuElement as FamilyInstance)!
                            .GetTotalTransform().Origin;
                        ManholeUtl.AdjOutAngle(RevitDoc, Context.MasuSetting[i], startLoc, toOrgLoc, fromOrgLoc);
                    }
                }
                else
                {
                    if ( ! isMaruMasu ) {
                        var startLoc = toOrgLoc + XYZ.BasisY;
                        ManholeUtl.AdjOutAngle(RevitDoc, Context.MasuSetting[i], startLoc, toOrgLoc, fromOrgLoc, toLoc.Rotation );                                    
                    }
                    else {
                        ManholeUtl.ResetRotationBasis(RevitDoc, Context.MasuSetting[i].MasuItem!.MasuElement.Id,
                            toLoc.Point, XYZ.BasisZ, toLoc.Rotation);
                        var startLoc = toOrgLoc + XYZ.BasisX;
                        ManholeUtl.AdjOutAngle(RevitDoc, Context.MasuSetting[i], startLoc, toOrgLoc, fromOrgLoc);
                    }
                }
            }
            else {
                bool isMaruMasu = Context.MasuSetting[ i ].MasuItem.MasuElement
                    .LookupParameter( MepModelLineConst.桝パラメータ_配管段差_丸 ) != null ;
                if ( ! isMaruMasu ) {
                    var startLoc = toOrgLoc + XYZ.BasisY ;
                    ManholeUtl.AdjInAngle(RevitDoc, Context.MasuSetting[i], startLoc, toOrgLoc, (Context.MasuSetting[i - 1].MasuItem!.MasuElement as FamilyInstance)!.GetTotalTransform().Origin, toLoc.Rotation );                                
                }
                else {
                    ManholeUtl.ResetRotationBasis(RevitDoc, Context.MasuSetting[i].MasuItem!.MasuElement.Id,
                        fromLoc.Point, XYZ.BasisZ, orgRotation);
                }
            }

            fromOrgLoc = toOrgLoc;
            fromLoc = toLoc;
            fromBottomHeight = toBottomHeight;
            fromHeightBase = toHeightBase ;
        }
        
        //実行ボタンを押し、詳細ビューを作成していく
        if ( Context.CreateViewMode != 2 ) {
            if ( Context.CreateViewMode == 1 ) {
                FilteredElementCollector collector = new FilteredElementCollector(doc);
    
                //桝縦断図_実寸
                View view = collector
                    .OfClass( typeof(View) )
                    .Cast<View>()
                    .FirstOrDefault( v => !v.IsTemplate && v.Name.Equals(Context.DraftingViewName, StringComparison.OrdinalIgnoreCase) );
                if ( view != null )
                    DraftingViewUlt.DrawDraftingView( RevitDoc, view,
                        Context.MasuSetting.ToList() ) ;
                //桝縦断図
                view = collector
                    .OfClass( typeof(View) )
                    .Cast<View>()
                    .FirstOrDefault( v => !v.IsTemplate && v.Name.Equals(Context.DraftingViewNameFixLenght, StringComparison.OrdinalIgnoreCase) );
                if ( view != null )
                    DraftingViewUlt.DrawDraftingView( RevitDoc, view,
                        Context.MasuSetting.ToList(), 1000 ) ;

            }
            else {
                //桝縦断図_実寸
                var dftView = CreateDraftingView( RevitDoc, Context.DraftingViewName) ;
                if ( dftView != null )
                    DraftingViewUlt.DrawDraftingView( RevitDoc, dftView,
                        Context.MasuSetting.ToList() ) ;

                //桝縦断図
                dftView = CreateDraftingView( RevitDoc, Context.DraftingViewNameFixLenght) ;
                if ( dftView != null )
                    DraftingViewUlt.DrawDraftingView( RevitDoc, dftView,
                        Context.MasuSetting.ToList(), 1000 ) ;
            }
        }
        
        tg.Assimilate() ;
      }   
    }
    
    public string GetName() => "Mep Model line External Event Handler";
    
    #region Ults
    /// <summary>
    /// 桝確認ビュー
    /// </summary>
    /// <param name="doc">Revit Doc</param>
    /// <param name="viewName">ビュー名</param>
    /// <returns>作成できたビュー</returns>
    private View? CreateDraftingView(Document doc, string viewName)
    {
        View? resultView = null;
        using (Transaction trans = new Transaction(doc, "Create Drafting View"))
        {
            trans.Start();

            try
            {
                FilteredElementCollector draftCollector = new FilteredElementCollector(doc);
                var draftView = draftCollector.OfClass(typeof(ViewDrafting)).FirstOrDefault(x => x.Name.Equals(viewName));

                if (draftView != null)
                {
                    doc.Delete(draftView.Id);
                }
                    
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                collector.OfClass(typeof(ViewFamilyType));
                ViewFamilyType viewFamilyType = collector.Cast<ViewFamilyType>().First(vft => vft.ViewFamily == ViewFamily.Drafting);
                resultView = ViewDrafting.Create(doc, viewFamilyType.Id);
                if (resultView != null)
                {
                    resultView.Scale = 50;
                    if (resultView.Name != viewName) resultView.Name = viewName;
                }
                    
                trans.Commit();

                return resultView;
            }
            catch
            {
                trans.RollBack();
            }
        }

        return resultView;
    }
    #endregion
  }
}