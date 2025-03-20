using System;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace Quiz.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Colors.Transparent; // Цвет не меняется, если значение null

            if (value is bool isValid)
            {
                return isValid ? Color.FromArgb("#1f6e40") : Color.FromArgb("#781f1f"); // Зеленый, если true; красный, если false
            }

            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
