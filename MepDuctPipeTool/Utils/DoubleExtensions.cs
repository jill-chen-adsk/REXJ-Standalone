using System;

namespace MepDuctPipeTool.Utils
{
  internal static class DoubleExtensions
  {
    // *** 等しさの判定 ***
    public static bool IsNearlyEqualTo( this double left, double right, double tolerance )
      => Math.Abs( left - right ) < tolerance;

    public static bool IsDefinitelyDifferTo( this double left, double right, double tolerance )
      => ! left.IsNearlyEqualTo( right, tolerance );

    // *** 比較演算子 ***
    /// <summary>
    /// left が right より小さく, nearly equal でもない
    /// </summary>
    internal static bool IsDefinitelyLessThan( this double left, double right, double tolerance )
      => left < right - tolerance;
  }
}