using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace ADSK.Ext.Fukashi.Opening.Components
{
  /// ================================================================================
  /// <summary>属性</summary>
  /// ================================================================================
  public class Attribute
  {
    private ResourceManager _resourceManager;
    private ResourceManager _resourceManagerEn;
    private string _executeFolder;

    public string ExecuteFolder => _executeFolder;

    public Attribute()
    {
      _executeFolder = Path.GetDirectoryName(
          Assembly.GetExecutingAssembly().Location);
      _resourceManager = new ResourceManager(
          "ADSK.Ext.Fukashi.Resources.Opening.Text",
          Assembly.GetExecutingAssembly());
      try
      {
        _resourceManagerEn = new ResourceManager(
            "ADSK.Ext.Fukashi.Resources.Opening.Text",
            Assembly.GetExecutingAssembly());
      }
      catch { }
    }

    public string ResourceText(string name)
    {
      if (_resourceManager == null) return name;
      try
      {
        string val = _resourceManagerEn?.GetString(name, new CultureInfo("en"));
        if (!string.IsNullOrEmpty(val)) return val;
      }
      catch { }
      try
      {
        string val = _resourceManager.GetString(name);
        if (!string.IsNullOrEmpty(val)) return val;
      }
      catch { }
      return name;
    }

    public string ResourceTextJa(string name)
    {
      if (_resourceManager == null) return name;
      try
      {
        string val = _resourceManager.GetString(name, CultureInfo.InvariantCulture);
        if (!string.IsNullOrEmpty(val)) return val;
      }
      catch { }
      return name;
    }
  }
}
