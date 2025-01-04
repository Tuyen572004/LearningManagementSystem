using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Helpers
{
    /// <summary>
    /// Converts a nullable string to a non-nullable string and vice versa.
    /// </summary>
    public partial class NullableStringToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a nullable string to a non-nullable string.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>A non-nullable string if the input is a string; otherwise, an empty string.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string stringValue)
            {
                return stringValue.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Converts a non-nullable string back to a nullable string.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>A trimmed string if the input is a non-empty string; otherwise, null.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string stringValue)
            {
                var processedString = stringValue.Trim();
                if (processedString.Length > 0)
                {
                    return processedString;
                }
            }
            return null;
        }
    }
}
