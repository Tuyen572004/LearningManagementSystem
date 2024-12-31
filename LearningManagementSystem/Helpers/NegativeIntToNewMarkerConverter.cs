using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Helpers
{
    /// <summary>
    /// Converts a negative integer to the string "new" and non-negative integers to their string representation.
    /// </summary>
    public partial class NegativeIntToNewMarkerConverter : IValueConverter
    {
        /// <summary>
        /// Converts a negative integer to the string "new" and non-negative integers to their string representation.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>A string "new" if the value is a negative integer, otherwise the string representation of the integer.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int number)
            {
                return (number < 0) ? "new" : number.ToString();
            }
            return String.Empty;
        }

        /// <summary>
        /// Not implemented. Throws a <see cref="NotImplementedException"/> if called.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>Throws a <see cref="NotImplementedException"/>.</returns>
        /// <exception cref="NotImplementedException">Always thrown.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
