using Microsoft.Win32;
using System;
using System.Reflection;
using System.Windows.Forms;

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

        public static string SetLastValue(TextBox textBoxControl, string dialogBoxName, string defaultValue = "", bool onlyGetValue = false)
        {
            string strLast = GetSetting(dialogBoxName, textBoxControl.Name, defaultValue);
            if (!onlyGetValue)
                textBoxControl.Text = strLast;
            return strLast;
        }

        public static void SaveLastValue(Button buttonControl, string dialogBoxName) =>
            SaveSetting(dialogBoxName, buttonControl.Name, buttonControl.Text);

        public static string SetLastValue(Button buttonControl, string dialogBoxName, string defaultValue = "", bool onlyGetValue = false)
        {
            string strLast = GetSetting(dialogBoxName, buttonControl.Name, defaultValue);
            if (!onlyGetValue)
                buttonControl.Text = strLast;
            return strLast;
        }

        public static void SaveLastValue(TextBox textBoxControl, string dialogBoxName) =>
            SaveSetting(dialogBoxName, textBoxControl.Name, textBoxControl.Text);

        public static int SetLastValue(ComboBox comboBoxControl, string dialogBoxName, string defaultValue = "", bool onlyGetValue = false)
        {
            string strLast = GetSetting(dialogBoxName, comboBoxControl.Name, defaultValue);
            if (comboBoxControl.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                int intLast;
                if (comboBoxControl.Items.Count == 0)
                    intLast = -1;
                else if (int.TryParse(strLast, out int parsed))
                {
                    intLast = parsed;
                    if (intLast < 0)
                        intLast = 0;
                    else if (comboBoxControl.Items.Count <= intLast)
                        intLast = comboBoxControl.Items.Count - 1;
                }
                else
                {
                    intLast = 0;
                    for (int i = 0; i < comboBoxControl.Items.Count; i++)
                    {
                        if (comboBoxControl.Items[i].ToString() == defaultValue)
                        {
                            intLast = i;
                            break;
                        }
                    }
                }

                if (!onlyGetValue)
                    comboBoxControl.SelectedIndex = intLast;
                return intLast;
            }

            comboBoxControl.Text = strLast;
            return -1;
        }

        public static void SaveLastValue(ComboBox comboBoxControl, string dialogBoxName)
        {
            if (comboBoxControl.DropDownStyle == ComboBoxStyle.DropDownList)
                SaveSetting(dialogBoxName, comboBoxControl.Name, comboBoxControl.SelectedIndex.ToString());
            else
                SaveSetting(dialogBoxName, comboBoxControl.Name, comboBoxControl.Text);
        }

        public static int SetLastValue(ListBox listBoxControl, string dialogBoxName, int defaultValue = 0, bool onlyGetValue = false)
        {
            string strLast = GetSetting(dialogBoxName, listBoxControl.Name, defaultValue.ToString());
            int intLast = int.TryParse(strLast, out int p) ? p : 0;
            if (!onlyGetValue && listBoxControl.Items.Count > 0)
            {
                if (-1 <= intLast && intLast < listBoxControl.Items.Count)
                    listBoxControl.SelectedIndex = intLast;
                else
                    listBoxControl.SelectedIndex = 0;
            }
            return intLast;
        }

        public static void SaveLastValue(ListBox listBoxControl, string dialogBoxName) =>
            SaveSetting(dialogBoxName, listBoxControl.Name, listBoxControl.SelectedIndex.ToString());

        public static bool SetLastValue(CheckBox checkBoxControl, string dialogBoxName, bool defaultValue = true, bool onlyGetValue = false)
        {
            string strLast = GetSetting(dialogBoxName, checkBoxControl.Name, defaultValue.ToString());
            bool bolLast = defaultValue;
            try { bolLast = bool.Parse(strLast); } catch { }

            if (!onlyGetValue)
                checkBoxControl.Checked = bolLast;
            return bolLast;
        }

        public static void SaveLastValue(CheckBox checkBoxControl, string dialogBoxName) =>
            SaveSetting(dialogBoxName, checkBoxControl.Name, checkBoxControl.Checked.ToString());

        public static bool SetLastValue(RadioButton radioButtonControl, string dialogBoxName, bool defaultValue = true, bool onlyGetValue = false)
        {
            string strLast = GetSetting(dialogBoxName, radioButtonControl.Name, defaultValue.ToString());
            bool bolLast = defaultValue;
            try { bolLast = bool.Parse(strLast); } catch { }

            if (!onlyGetValue)
                radioButtonControl.Checked = bolLast;
            return bolLast;
        }

        public static void SaveLastValue(RadioButton radioButtonControl, string dialogBoxName) =>
            SaveSetting(dialogBoxName, radioButtonControl.Name, radioButtonControl.Checked.ToString());

        public static decimal SetLastValue(NumericUpDown numericUpDnControl, string dialogBoxName, decimal defaultValue = 0, bool onlyGetValue = false)
        {
            string strLast = GetSetting(dialogBoxName, numericUpDnControl.Name, defaultValue.ToString());
            decimal intLast = decimal.TryParse(strLast, out decimal d) ? d : defaultValue;
            if (intLast < numericUpDnControl.Minimum)
                intLast = numericUpDnControl.Minimum;
            else if (numericUpDnControl.Maximum < intLast)
                intLast = numericUpDnControl.Maximum;

            if (!onlyGetValue)
                numericUpDnControl.Value = intLast;
            return intLast;
        }

        public static void SaveLastValue(NumericUpDown numericUpDnControl, string dialogBoxName) =>
            SaveSetting(dialogBoxName, numericUpDnControl.Name, numericUpDnControl.Value.ToString());

        public static int SetComboboxItemByText(ComboBox cmbBox1, System.Collections.Generic.List<string> valueStrings)
        {
            int iFinish = 0;
            for (int i = 0; i < cmbBox1.Items.Count; i++)
            {
                string itm = cmbBox1.Items[i].ToString();
                if (valueStrings.Contains(itm))
                {
                    iFinish = i;
                    break;
                }
            }
            cmbBox1.SelectedIndex = iFinish;
            return iFinish;
        }
    }
}
