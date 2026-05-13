using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.Ext.Fukashi.Opening.Components
{
  /// ================================================================================
  /// <summary>設定</summary>
  /// ================================================================================
  public class Settings
  {
    private readonly UIDocument _rvtUidoc;

    public Settings(UIDocument rvtUiDoc)
    {
      _rvtUidoc = rvtUiDoc ?? throw new ArgumentNullException(nameof(rvtUiDoc));
    }

    public Document RvtDBDoc => _rvtUidoc.Document;

    public Category GetCategory(BuiltInCategory bic)
    {
      return Category.GetCategory(RvtDBDoc, bic);
    }

    public Category CategoryProjInfo => GetCategory(BuiltInCategory.OST_ProjectInformation);
  }
}
