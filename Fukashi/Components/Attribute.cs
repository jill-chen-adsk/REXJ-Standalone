using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace ADSK.Ext.Fukashi.Components
{
    public class Attribute
    {
        private ResourceManager _resourceManager;
        private ResourceManager _resourceManagerEn;
        private string _executeFolder;

        public string ExecuteFolder => _executeFolder;

        public Attribute()
        {
            _executeFolder = System.IO.Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            SetResourceBaseName("ADSK.Ext.Fukashi.Resources.Text");
        }

        public Attribute(string resourceBaseName)
        {
            _executeFolder = System.IO.Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            _resourceManager = new ResourceManager(resourceBaseName,
                Assembly.GetExecutingAssembly());
            try
            {
                _resourceManagerEn = new ResourceManager(resourceBaseName,
                    Assembly.GetExecutingAssembly());
            }
            catch { }
        }

        public void SetResourceBaseName(string baseName)
        {
            _resourceManager = new ResourceManager(baseName,
                Assembly.GetExecutingAssembly());
            try
            {
                _resourceManagerEn = new ResourceManager(baseName,
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
