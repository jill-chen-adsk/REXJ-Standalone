using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Quantity.Components
{
  /// <summary>Project/category conveniences shared by commands.</summary>
  public sealed class Settings
  {
    public Settings(UIDocument rvtUIDoc, Attribute _)
    {
      _rvtUIDoc = rvtUIDoc;
    }

    private readonly UIDocument _rvtUIDoc;

    public Category CategoryProjInfo =>
      Category.GetCategory(_rvtUIDoc.Document, BuiltInCategory.OST_ProjectInformation);
  }
}
