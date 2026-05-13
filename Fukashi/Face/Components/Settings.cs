using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.Ext.Fukashi.Face.Components
{
  /// ================================================================================
  /// <summary>設定</summary>
  /// ================================================================================
  public class Settings
  {
    private readonly UIDocument _rvtUIDoc;

    public Settings(UIDocument rvtUiDoc)
    {
      _rvtUIDoc = rvtUiDoc ?? throw new ArgumentNullException(nameof(rvtUiDoc));
    }

    public Document RvtDBDoc => _rvtUIDoc.Document;

    public Category GetCategory(BuiltInCategory bic)
    {
      return Category.GetCategory(RvtDBDoc, bic);
    }

    public Category CategoryProjInfo => GetCategory(BuiltInCategory.OST_ProjectInformation);
  }
}
