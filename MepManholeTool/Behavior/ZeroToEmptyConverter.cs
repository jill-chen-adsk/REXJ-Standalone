using System ;
using System.Globalization ;
using System.Windows.Data ;
using MepManholeTool.Models ;

namespace MepManholeTool.Behavior
{
  public class ZeroToEmptyConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return string.Empty;

      if (double.TryParse(value.ToString(), out double doubleVal))
      {
        if (doubleVal == 0) return string.Empty;
        return doubleVal.ToString("0");
      }

      return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (string.IsNullOrWhiteSpace(value as string))
        return 0;
      if (double.TryParse(value.ToString(), out double result))
        return result;
      return 0;
    }
  }
}