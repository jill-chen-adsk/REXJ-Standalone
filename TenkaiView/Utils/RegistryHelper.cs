using System;
using Microsoft.Win32;

namespace ADSK.ViewExtension.TenkaiView.Utils
{
    /// <summary>
    /// Wraps registry access compatible with VB GetSetting/SaveSetting layout:
    /// HKCU\Software\VB and VBA Program Settings\{appName}\{section}\{key}
    /// </summary>
    internal static class RegistryHelper
    {
        private static string GetSectionKeyPath(string appName, string section)
        {
            return $@"Software\VB and VBA Program Settings\{appName}\{section}";
        }

        public static string GetSetting(string appName, string section, string key, string defaultValue)
        {
            try
            {
                using RegistryKey rk = Registry.CurrentUser.OpenSubKey(GetSectionKeyPath(appName, section));
                if (rk == null)
                    return defaultValue;
                object v = rk.GetValue(key);
                return v?.ToString() ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static void SaveSetting(string appName, string section, string key, string value)
        {
            try
            {
                using RegistryKey rk = Registry.CurrentUser.CreateSubKey(GetSectionKeyPath(appName, section));
                rk?.SetValue(key, value);
            }
            catch
            {
                // ignore
            }
        }
    }
}
