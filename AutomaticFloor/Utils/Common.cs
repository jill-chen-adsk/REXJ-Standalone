using System.Windows.Forms;

namespace ADSK.JExtRAC.AutomaticFloor.Utils
{
    public class Common
    {
        public static void NumberCheck(object sender, KeyPressEventArgs e, bool allowNegativeValue = false)
        {
            if (!allowNegativeValue)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                    e.Handled = true;
                if (sender is TextBox tb && e.KeyChar == '.' && tb.Text.IndexOf('.') > -1)
                    e.Handled = true;
                if (sender is ComboBox cb && e.KeyChar == '.' && cb.Text.IndexOf('.') > -1)
                    e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-')
                    e.Handled = true;
                if (sender is TextBox tb2)
                {
                    if (e.KeyChar == '.' && tb2.Text.IndexOf('.') > -1) e.Handled = true;
                    if (e.KeyChar == '-' && tb2.Text.IndexOf('-') > -1) e.Handled = true;
                }
                if (sender is ComboBox cb2)
                {
                    if (e.KeyChar == '.' && cb2.Text.IndexOf('.') > -1) e.Handled = true;
                    if (e.KeyChar == '-' && cb2.Text.IndexOf('-') > -1) e.Handled = true;
                }
            }
        }
    }

    public enum eFloorType
    {
        Arch = 0,
        Struct,
        Slab
    }
}
