using System;
using System.Globalization;

namespace Quiz.Converters
{
    public class RadioButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if the value (SelectedLevel) matches the parameter (RadioButton value)
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Return the RadioButton value if it is checked
            return (bool)value ? parameter.ToString() : null;
        }
    }
}
