using System.IO;
using System.Reflection;
using System.Resources;

namespace SectionListSteel.JExtComCompat
{
    public abstract class UtilAttrib
    {
        private Assembly? _assembly;
        private ResourceManager? _textResources;
        private string _dataFolder = "";

        protected UtilAttrib()
        {
        }

        protected void SetAssembly(Assembly assembly, string textResourceName, string imageResourceName, string dataFolder)
        {
            _assembly = assembly;
            _textResources = new ResourceManager(textResourceName, assembly);
            _dataFolder = dataFolder;
        }

        public string ResourceText(string key)
        {
            return _textResources?.GetString(key) ?? key;
        }

        public string ExecuteFolder => _assembly != null
            ? Path.GetDirectoryName(_assembly.Location) ?? ""
            : "";

        public string ExecuteFile => _assembly != null
            ? Path.GetFileName(_assembly.Location) ?? ""
            : "";

        public string DataFolder => _dataFolder;
    }
}
