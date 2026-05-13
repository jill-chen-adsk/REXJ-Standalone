using System;
using System.Collections;
using System.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitMEPAddinApp.Properties
{
    internal class ExResources
    {
        private static ResourceSet resourceSet = Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentUICulture, true, true);

        internal static string ResxString(string key)
        {
            foreach (var entry in resourceSet.OfType<DictionaryEntry>().Select((item, i) => new { Index = i, Key = item.Key, Value = item.Value }))
            {
                if (entry.Key.ToString().Equals(key))
                {
                    return entry.Value.ToString();
                }
            }
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static string BTN_EDIT_LEVEL()
        {
            return ResxString("BTN_EDIT_LEVEL");
        }

        internal static string BTN_ROTATE_TEES()
        {
            return ResxString("BTN_ROTATE_TEES");
        }

        internal static string BTN_MOVE_CONNECTOR()
        {
            return ResxString("BTN_MOVE_CONNECTOR");
        }

        internal static string T_TIP_BTN_MOVE_CONNECTOR()
        {
            return ResxString("T_TIP_BTN_MOVE_CONNECTOR");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static string P_BTN_2PICK()
        {
            return ResxString("P_BTN_2PICK");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static string P_BTN_3PICK()
        {
            return ResxString("P_BTN_3PICK");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static string P_BTN_3PICK_Linkd()
        {
            return ResxString("P_BTN_3PICK_Linkd");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static string PLN_EDIT()
        {
            return ResxString("PLN_EDIT");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static string TAB_MEP_ADDIN()
        {
            return ResxString("TAB_MEP_ADDIN");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static string T_TIP_PD_BTN()
        {
            return ResxString("T_TIP_PD_BTN");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static string T_TIP_P_BTN_2PICK()
        {
            return ResxString("T_TIP_P_BTN_2PICK");
        }

        internal static string T_TIP_BTN_ROTATE_TEES()
        {
            return ResxString("T_TIP_BTN_ROTATE_TEES");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static string T_TIP_P_BTN_3PICK()
        {
            return ResxString("T_TIP_P_BTN_3PICK");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static string T_TIP_P_BTN_3PICK_Linkd()
        {
            return ResxString("T_TIP_P_BTN_3PICK_Linkd");
        }
    }
}