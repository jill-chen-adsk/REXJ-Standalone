using System;
using Autodesk.Revit.DB;

namespace MepDuctPipeTool.Utils
{
  public static class FamilyInstanceTransformer
  {
    public static void RotateHandToAlignWith( this FamilyInstance familyInstance, XYZ targetDirection, Document document )
    {
      var targetDir = targetDirection.Normalize();
      var handOrientation = familyInstance.HandOrientation;
      var center = familyInstance.GetLocationPoint().Point;

      if ( handOrientation.IsSameDirectionTo( targetDir ) ) return;
      if ( handOrientation.IsOppositeDirectionTo( targetDir ) )
      {
        var rotAxisDirFor180Flip = handOrientation.CrossProduct( familyInstance.FacingOrientation ).Normalize();
        var axisFor180Flip = CreateRotationAxis( center, rotAxisDirFor180Flip );
        ElementTransformUtils.RotateElement( document, familyInstance.Id, axisFor180Flip, Math.PI );
        return;
      }

      var rotAxisDir = handOrientation.CrossProduct( targetDir ).Normalize();
      var axis = CreateRotationAxis( center, rotAxisDir );
      var angle = handOrientation.AngleTo( targetDir );

      ElementTransformUtils.RotateElement( document, familyInstance.Id, axis, angle );
    }


    public static void RotateToReverseAlign( this FamilyInstance familyInstance, XYZ targetDirection, Document document )
      => familyInstance.RotateHandToAlignWith( -targetDirection, document );

    /// <summary>
    /// インスタンスの上面方向のベクトルをTopOrientationと定義し、それをtargetDirectionに沿わせるように回転する
    /// </summary>
    public static void RotateTopToAlignWith( this FamilyInstance familyInstance, XYZ targetDirection, Document document )
    {
      var targetDir = targetDirection.Normalize();
      var topOrientation = familyInstance.HandOrientation.CrossProduct( familyInstance.FacingOrientation );
      if ( topOrientation.IsSameDirectionTo( targetDir ) ) return;

      var center = familyInstance.GetLocationPoint().Point;
      var rotAxisDir = familyInstance.HandOrientation;
      var axis = CreateRotationAxis( center, rotAxisDir );

      var angleToTarget = topOrientation.AngleOnPlaneTo( targetDir, rotAxisDir );
      ElementTransformUtils.RotateElement( document, familyInstance.Id, axis, angleToTarget );
    }

    private static Line CreateRotationAxis( XYZ origin, XYZ direction )
      => Line.CreateBound( origin, origin + direction * 10 );
  }
}