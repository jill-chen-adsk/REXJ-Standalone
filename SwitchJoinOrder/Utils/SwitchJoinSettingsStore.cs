using System.Reflection;
using Microsoft.Win32;

namespace ADSK.JExtRAC.SwitchJoinOrder.Utils
{
    internal static class SwitchJoinSettingsStore
    {
        private static string RegistryBasePath =>
            @"Software\VB and VBA Program Settings\" + Assembly.GetExecutingAssembly().GetName().Name;

        public static string Get(string section, string key, string defaultValue)
        {
            try
            {
                string path = RegistryBasePath + "\\" + section;
                object val = Registry.GetValue(@"HKEY_CURRENT_USER\" + path, key, defaultValue);
                return val?.ToString() ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static void Save(string section, string key, string value)
        {
            try
            {
                string path = RegistryBasePath + "\\" + section;
                Registry.SetValue(@"HKEY_CURRENT_USER\" + path, key, value);
            }
            catch
            {
                // ignored
            }
        }
    }
}
