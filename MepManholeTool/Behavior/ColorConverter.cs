using System;
using System.Globalization;
using System.Windows ;
using System.Windows.Controls ;
using System.Windows.Data;
using System.Windows.Media;

namespace MepManholeTool.Behavior
{
    public class ColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double required = (double)values[0];
            double curBottom = (double)values[1];
            
            int rowIndex = -1;
            if (values.Length > 2 && values[2] is DependencyObject depObj)
            {
                DataGridRow row = FindParent<DataGridRow>(depObj);
                if (row != null)
                {
                    rowIndex = row.GetIndex();
                }
            }
            
            if (rowIndex != 0 && (curBottom > required)) return new SolidColorBrush(Colors.Orange);
            return new SolidColorBrush();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
        
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
        
            return parent as T;
        }
    }
}