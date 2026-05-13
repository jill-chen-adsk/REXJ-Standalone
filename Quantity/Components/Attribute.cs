using System;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Quantity.Components
{
  /// <summary>Resource paths and lookups for Quantity add-in.</summary>
  public sealed class Attribute
  {
    private static readonly Lazy<ResourceManager> _text =
      new Lazy<ResourceManager>(() => new ResourceManager("Quantity.Resources.Text", typeof(Attribute).Assembly));

    private static readonly Lazy<ResourceManager> _image =
      new Lazy<ResourceManager>(() => new ResourceManager("Quantity.Resources.Image", typeof(Attribute).Assembly));

    public Attribute()
    {
      string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
      ExecuteFolder = dir.TrimEnd('\\');
      ExecuteFile = Path.Combine(dir, ExecuteAssemblyFileName());
    }

    static string ExecuteAssemblyFileName()
    {
      try
      {
        return Path.GetFileName(Assembly.GetExecutingAssembly().Location) ?? "Quantity.dll";
      }
      catch
      {
        return "Quantity.dll";
      }
    }

    /// <summary>Folder containing Quantity.dll.</summary>
    public string ExecuteFolder { get; }

    /// <summary>Full path to Quantity.dll.</summary>
    public string ExecuteFile { get; }

    public string ResourceText(string key) => _text.Value.GetString(key) ?? string.Empty;

    public object ResourceImage(string key)
    {
      try { return _image.Value.GetObject(key); }
      catch { return null; }
    }
  }
}
