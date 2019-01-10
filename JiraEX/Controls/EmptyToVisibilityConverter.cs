using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace JiraEX.Controls
{
    public class EmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;
            else
            {
                ICollection list = value as ICollection;
                if (list != null)
                {
                    if (list.Count == 0)
                        return Visibility.Visible;
                    else
                        return Visibility.Collapsed;
                }
                else
                    return Visibility.Visible;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)

        {
            throw new NotImplementedException();
        }
    }
}
