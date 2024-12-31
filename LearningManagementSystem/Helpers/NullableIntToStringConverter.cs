using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Helpers
{
    /// <summary>
    /// Converts a nullable integer to a string and vice versa.
    /// </summary>
    public partial class NullableIntToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a nullable integer to a string.
        /// </summary>
        /// <param name="value">The nullable integer value to convert.</param>
        /// <param name="targetType">The target type (not used).</param>
        /// <param name="parameter">An optional parameter (not used).</param>
        /// <param name="language">The language (not used).</param>
        /// <returns>A string representation of the integer, or an empty string if the value is null.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int intValue)
            {
                return intValue.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Converts a string back to a nullable integer.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <param name="targetType">The target type (not used).</param>
        /// <param name="parameter">An optional parameter (not used).</param>
        /// <param name="language">The language (not used).</param>
        /// <returns>A nullable integer if the conversion is successful, otherwise null.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string stringValue)
            {
                if (int.TryParse(stringValue, out int intValue))
                {
                    return intValue;
                }
            }
            return null;
        }
    }
}
