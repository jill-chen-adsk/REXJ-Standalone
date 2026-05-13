using Microsoft.Win32;

namespace ADSK.ViewExtension.SheetLayout.Utils
{
    internal static class RegistryHelper
    {
        private const string VbSettingsRoot = @"Software\VB and VBA Program Settings";

        public static string GetSetting(string appName, string section, string key, string defaultValue)
        {
            try
            {
                using var regKey = Registry.CurrentUser.OpenSubKey($"{VbSettingsRoot}\\{appName}\\{section}");
                if (regKey == null)
                    return defaultValue;
                return regKey.GetValue(key)?.ToString() ?? defaultValue;
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
                using var regKey = Registry.CurrentUser.CreateSubKey($"{VbSettingsRoot}\\{appName}\\{section}");
                regKey?.SetValue(key, value);
            }
            catch { }
        }
    }
}
