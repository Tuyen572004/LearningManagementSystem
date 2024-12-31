using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LearningManagementSystem.Helpers
{
    /// <summary>
    /// Converts DateTime objects to string representations and vice versa.
    /// </summary>
    public partial class DateTimeToStringConverter : IValueConverter
    {
        /// <summary>
        /// Represents an error DateTime value.
        /// </summary>
        static public readonly DateTime ERROR_DATETIME = new(0609, 06, 09);

        /// <summary>
        /// Converts a DateTime object to a string representation in the format "dd/MM/yyyy".
        /// </summary>
        /// <param name="value">The DateTime object to convert.</param>
        /// <param name="targetType">The target type (not used).</param>
        /// <param name="parameter">An optional parameter (not used).</param>
        /// <param name="language">The language (not used).</param>
        /// <returns>A string representation of the DateTime object, or an empty string if the conversion fails.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                try
                {
                    return dateTime.ToString("dd/MM/yyyy");
                }
                catch (Exception)
                {
                    return string.Empty;
                }

            }
            return string.Empty;
        }

        /// <summary>
        /// Converts a string representation of a date in the format "dd/MM/yyyy" back to a DateTime object.
        /// </summary>
        /// <param name="value">The string representation of the date.</param>
        /// <param name="targetType">The target type (not used).</param>
        /// <param name="parameter">An optional parameter (not used).</param>
        /// <param name="language">The language (not used).</param>
        /// <returns>A DateTime object if the conversion is successful, or ERROR_DATETIME if the conversion fails.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {

            if (value is string dateString)
            {
                var regex = DateRegex();
                try
                {
                    if (regex.IsMatch(dateString))
                    {
                        SingleDigitRegex().Replace(dateString, "0$1");

                        if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                        {
                            return dateTime;
                        }
                    }
                }
                catch (Exception)
                {
                    return ERROR_DATETIME;
                }
            }
            return ERROR_DATETIME;
        }

        /// <summary>
        /// Regular expression to match single digit day or month and add leading zero.
        /// </summary>
        /// <returns>A Regex object for matching single digit day or month.</returns>
        [GeneratedRegex(@"(?<!\d)(\d)(?=/)")]
        private static partial Regex SingleDigitRegex();

        /// <summary>
        /// Regular expression to validate date string in the format "dd/MM/yyyy".
        /// </summary>
        /// <returns>A Regex object for validating date string.</returns>
        [GeneratedRegex(@"^([1-9]|0[1-9]|[12][0-9]|3[01])/([1-9]|0[1-9]|1[0-2])/\d{4}$")]
        private static partial Regex DateRegex();
    }
}
