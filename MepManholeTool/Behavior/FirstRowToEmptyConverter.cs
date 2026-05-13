using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace MepManholeTool.Behavior
{
    [ValueConversion(typeof(object[]), typeof(string))]
    public class FirstRowToEmptyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values == null || values.Length != 3) return string.Empty;
                
                var value = values[0]; // The actual value to display
                var itemsSource = values[1] as IList; // The collection (MasuSetting)
                var currentItem = values[2]; // The current data context (RoutingParameter)
                
                if (itemsSource == null || currentItem == null) return string.Empty;

                // Check if current item is the first item in the collection
                if (itemsSource.Count > 0 && itemsSource[0] == currentItem)
                {
                    return string.Empty;
                }

                // Handle null values or nullable types
                if (value == null) return string.Empty;

                // If not first row, format the value normally
                if (double.TryParse(value.ToString(), out double doubleVal))
                {
                    return doubleVal.ToString("0");
                }

                return value.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
