using System;
using System.IO;
using System.Reflection;
using System.Resources;

namespace PipeSizing.Components
{
  /// <summary>Assembly path and localized string/image accessors.</summary>
  public sealed class Attribute
  {
    private static readonly Lazy<ResourceManager> TextResources = new Lazy<ResourceManager>(() =>
      new ResourceManager("PipeSizing.Resources.Text", typeof(Attribute).Assembly));

    private static readonly Lazy<ResourceManager> ImageResources = new Lazy<ResourceManager>(() =>
      new ResourceManager("PipeSizing.Resources.Image", typeof(Attribute).Assembly));

    public string ExecuteFolder => Path.GetDirectoryName(ExecuteFile) ?? string.Empty;

    public string ExecuteFile => Assembly.GetExecutingAssembly().Location;

    public string ResourceText(string key)
    {
      string s = TextResources.Value.GetString(key, System.Globalization.CultureInfo.CurrentUICulture);
      return s ?? key;
    }

    public object ResourceImage(string key)
    {
      try
      {
        return ImageResources.Value.GetObject(key, System.Globalization.CultureInfo.CurrentUICulture);
      }
      catch
      {
        return null;
      }
    }
  }
}
