using Microsoft.UI.Xaml.Data;
using System;

namespace LearningManagementSystem.Helpers
{

    public class NumberToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "";
            }

            if (value is double number)
            {
                return number.ToString("N");
            }

            return "";
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null; 
            }

            if (value is string text && double.TryParse(text, out double number))
            {
                return number; 
            }

            return null; 
        }
    }

}
