using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;
using System.Windows;

namespace LearningManagementSystem.Helpers
{
    /// <summary>
    /// Converts a boolean value to a Visibility enumeration value.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a Visibility enumeration value.
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <param name="targetType">The type of the target property. This parameter is not used.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. This parameter is not used.</param>
        /// <param name="language">The culture to use in the converter. This parameter is not used.</param>
        /// <returns>Returns Visibility.Visible if the value is true; otherwise, Visibility.Collapsed.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a Visibility enumeration value back to a boolean value.
        /// </summary>
        /// <param name="value">The Visibility value to convert.</param>
        /// <param name="targetType">The type of the target property. This parameter is not used.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. This parameter is not used.</param>
        /// <param name="language">The culture to use in the converter. This parameter is not used.</param>
        /// <returns>Returns true if the value is Visibility.Visible; otherwise, false.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }
}
