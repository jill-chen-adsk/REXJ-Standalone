using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MepManholeTool.Models;

namespace MepManholeTool.Properties
{
  public class GlobalMappings
  {
    private GlobalMappings() { }

    public List<ManholeMapping> Manholes { get; private set; } = new();

    public static GlobalMappings Instance { get; } = new();

    public void Init()
    {
      Manholes = new List<ManholeMapping>();
      var folder =
        System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
      var file = Path.Combine(folder, "Resources", "ParameterMapping.json");
      string jsonString = File.ReadAllText(file);
      Manholes = JsonSerializer.Deserialize<List<ManholeMapping>>(jsonString) ?? new List<ManholeMapping>();
    }

    public void SaveMappings()
    {
      var folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
      var file = Path.Combine(folder, "Resources", "ParameterMapping.json");
      var options = new JsonSerializerOptions
      {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
      };
      string jsonString = JsonSerializer.Serialize(Manholes, options);
      File.WriteAllText(file, jsonString, System.Text.Encoding.UTF8);
    }

    public void OverrideMapping(string familyName, List<ParameterMapping> mapping)
    {
      var updateManhole = Manholes.FirstOrDefault(x => x.Family == familyName);
      if (updateManhole != null)
      {
        updateManhole.Mapping = mapping;
      }
      else
      {
        Manholes.Add(new ManholeMapping { Family = familyName, Mapping = mapping });
      }
    }
  }
}
