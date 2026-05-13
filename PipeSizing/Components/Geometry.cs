using Autodesk.Revit.UI;

namespace PipeSizing.Components
{
  public sealed class Geometry
  {
    public Geometry(UIDocument rvtUIDoc)
    {
      RvtUIDoc = rvtUIDoc;
    }

    public UIDocument RvtUIDoc { get; }

    public double ToHalfAdjust(double value, int digits)
    {
      digits *= -1;
      double dCoef = System.Math.Pow(10, digits);

      return value > 0 ? System.Math.Floor((value * dCoef) + 0.5) / dCoef :
        System.Math.Ceiling((value * dCoef) - 0.5) / dCoef;
    }
  }
}
