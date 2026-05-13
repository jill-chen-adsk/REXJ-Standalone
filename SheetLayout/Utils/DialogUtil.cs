using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace ADSK.ViewExtension.SheetLayout.Utils
{
    internal static class DialogUtil
    {
        private static string AppName => Assembly.GetExecutingAssembly().GetName().Name;

        public static string SetLastValue(TextBox textBoxControl, string dialogBoxName, string defaultValue = "", bool onlyGetValue = false)
        {
            string strLast = RegistryHelper.GetSetting(AppName, dialogBoxName, textBoxControl.Name, defaultValue);
            if (!onlyGetValue)
                textBoxControl.Text = strLast;
            return strLast;
        }

        public static void SaveLastValue(Button buttonControl, string dialogBoxName)
        {
            RegistryHelper.SaveSetting(AppName, dialogBoxName, buttonControl.Name, buttonControl.Text);
        }

        public static string SetLastValue(Button buttonControl, string dialogBoxName, string defaultValue = "", bool onlyGetValue = false)
        {
            string strLast = RegistryHelper.GetSetting(AppName, dialogBoxName, buttonControl.Name, defaultValue);
            if (!onlyGetValue)
                buttonControl.Text = strLast;
            return strLast;
        }

        public static void SaveLastValue(TextBox textBoxControl, string dialogBoxName)
        {
            RegistryHelper.SaveSetting(AppName, dialogBoxName, textBoxControl.Name, textBoxControl.Text);
        }

        public static int SetLastValue(ComboBox comboBoxControl, string dialogBoxName, string defaultValue = "", bool onlyGetValue = false)
        {
            string strLast = RegistryHelper.GetSetting(AppName, dialogBoxName, comboBoxControl.Name, defaultValue);
            if (comboBoxControl.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                int intLast = 0;
                if (comboBoxControl.Items.Count == 0)
                {
                    intLast = -1;
                }
                else
                {
                    if (int.TryParse(strLast, out int parsed))
                    {
                        intLast = parsed;
                        if (intLast < 0)
                            intLast = 0;
                        else if (comboBoxControl.Items.Count <= intLast)
                            intLast = comboBoxControl.Items.Count - 1;
                    }
                    else
                    {
                        for (int i = 0; i < comboBoxControl.Items.Count; i++)
                        {
                            if (comboBoxControl.Items[i].ToString() == defaultValue)
                            {
                                intLast = i;
                                break;
                            }
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
                RegistryHelper.SaveSetting(AppName, dialogBoxName, comboBoxControl.Name, comboBoxControl.SelectedIndex.ToString());
            else
                RegistryHelper.SaveSetting(AppName, dialogBoxName, comboBoxControl.Name, comboBoxControl.Text);
        }

        public static int SetLastValue(ListBox listBoxControl, string dialogBoxName, int defaultValue = 0, bool onlyGetValue = false)
        {
            string strLast = RegistryHelper.GetSetting(AppName, dialogBoxName, listBoxControl.Name, defaultValue.ToString());
            int intLast = 0;
            if (int.TryParse(strLast, out int parsed))
                intLast = parsed;

            if (!onlyGetValue && listBoxControl.Items.Count > 0)
            {
                if (intLast >= -1 && intLast < listBoxControl.Items.Count)
                    listBoxControl.SelectedIndex = intLast;
                else
                    listBoxControl.SelectedIndex = 0;
            }

            return intLast;
        }

        public static void SaveLastValue(ListBox listBoxControl, string dialogBoxName)
        {
            RegistryHelper.SaveSetting(AppName, dialogBoxName, listBoxControl.Name, listBoxControl.SelectedIndex.ToString());
        }

        public static bool SetLastValue(CheckBox checkBoxControl, string dialogBoxName, bool defaultValue = true, bool onlyGetValue = false)
        {
            string strLast = RegistryHelper.GetSetting(AppName, dialogBoxName, checkBoxControl.Name, defaultValue.ToString());
            bool bolLast = defaultValue;
            try
            {
                bolLast = bool.Parse(strLast);
            }
            catch { }

            if (!onlyGetValue)
                checkBoxControl.Checked = bolLast;

            return bolLast;
        }

        public static void SaveLastValue(CheckBox checkBoxControl, string dialogBoxName)
        {
            RegistryHelper.SaveSetting(AppName, dialogBoxName, checkBoxControl.Name, checkBoxControl.Checked.ToString());
        }

        public static bool SetLastValue(RadioButton radioButtonControl, string dialogBoxName, bool defaultValue = true, bool onlyGetValue = false)
        {
            string strLast = RegistryHelper.GetSetting(AppName, dialogBoxName, radioButtonControl.Name, defaultValue.ToString());
            bool bolLast = defaultValue;
            try
            {
                bolLast = bool.Parse(strLast);
            }
            catch { }

            if (!onlyGetValue)
                radioButtonControl.Checked = bolLast;

            return bolLast;
        }

        public static void SaveLastValue(RadioButton radioButtonControl, string dialogBoxName)
        {
            RegistryHelper.SaveSetting(AppName, dialogBoxName, radioButtonControl.Name, radioButtonControl.Checked.ToString());
        }

        public static decimal SetLastValue(NumericUpDown numericUpDnControl, string dialogBoxName, decimal defaultValue = 0, bool onlyGetValue = false)
        {
            string strLast = RegistryHelper.GetSetting(AppName, dialogBoxName, numericUpDnControl.Name, defaultValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
            decimal intLast = 0;
            if (decimal.TryParse(strLast, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal parsed))
                intLast = parsed;

            if (intLast < numericUpDnControl.Minimum)
                intLast = numericUpDnControl.Minimum;
            else if (numericUpDnControl.Maximum < intLast)
                intLast = numericUpDnControl.Maximum;

            if (!onlyGetValue)
                numericUpDnControl.Value = intLast;

            return intLast;
        }

        public static void SaveLastValue(NumericUpDown numericUpDnControl, string dialogBoxName)
        {
            RegistryHelper.SaveSetting(AppName, dialogBoxName, numericUpDnControl.Name, numericUpDnControl.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public static int SetComboboxItemByText(ComboBox cmbBox1, List<string> valueStrings)
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
