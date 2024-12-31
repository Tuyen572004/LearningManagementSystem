using Microsoft.UI.Xaml.Data;
using System;

namespace LearningManagementSystem.Helpers
{

    /// <summary>
    /// Converts a number to its text representation and vice versa.
    /// </summary>
    public class NumberToTextConverter : IValueConverter
    {
        /// <summary>
        /// Converts a number to its text representation.
        /// </summary>
        /// <param name="value">The value to convert, expected to be a double.</param>
        /// <param name="targetType">The type of the target property. This parameter is not used.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. This parameter is not used.</param>
        /// <param name="language">The language of the conversion. This parameter is not used.</param>
        /// <returns>A string representation of the number, or an empty string if the value is null or not a double.</returns>
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

        /// <summary>
        /// Converts a text representation of a number back to a double.
        /// </summary>
        /// <param name="value">The value to convert, expected to be a string.</param>
        /// <param name="targetType">The type of the target property. This parameter is not used.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. This parameter is not used.</param>
        /// <param name="language">The language of the conversion. This parameter is not used.</param>
        /// <returns>A double representation of the text, or null if the value is null or not a valid double.</returns>
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
