using Microsoft.Win32;
using System.Reflection;
using System.Windows.Controls.Primitives;

namespace ADSK.ViewExtension.ViewDuplicate.Utils
{
    public static class DialogUtil
    {
        private static string AppName => Assembly.GetExecutingAssembly().GetName().Name;

        private static string SectionPath(string dialogBoxName) =>
            @"Software\VB and VBA Program Settings\" + AppName + @"\" + dialogBoxName;

        public static string GetSetting(string dialogBoxName, string keyName, string defaultValue)
        {
            try
            {
                using (var rk = Registry.CurrentUser.OpenSubKey(SectionPath(dialogBoxName)))
                {
                    if (rk?.GetValue(keyName) is object v)
                        return v.ToString();
                }
            }
            catch { }
            return defaultValue;
        }

        public static void SaveSetting(string dialogBoxName, string keyName, string value)
        {
            try
            {
                using (var rk = Registry.CurrentUser.CreateSubKey(SectionPath(dialogBoxName)))
                {
                    rk?.SetValue(keyName, value);
                }
            }
            catch { }
        }

        public static void RestoreSelectedIndex(Selector selector, string dialogBoxName, string keyName)
        {
            int count = selector.Items.Count;
            if (count == 0)
                return;

            int index = int.TryParse(GetSetting(dialogBoxName, keyName, "0"), out int parsed) ? parsed : 0;
            if (index < 0)
                index = 0;
            else if (index >= count)
                index = count - 1;

            selector.SelectedIndex = index;
        }

        public static void SaveSelectedIndex(Selector selector, string dialogBoxName, string keyName) =>
            SaveSetting(dialogBoxName, keyName, selector.SelectedIndex.ToString());

        public static bool GetBool(string dialogBoxName, string keyName, bool defaultValue) =>
            bool.TryParse(GetSetting(dialogBoxName, keyName, defaultValue.ToString()), out bool value)
                ? value
                : defaultValue;

        public static void SaveBool(string dialogBoxName, string keyName, bool value) =>
            SaveSetting(dialogBoxName, keyName, value.ToString());
    }
}
