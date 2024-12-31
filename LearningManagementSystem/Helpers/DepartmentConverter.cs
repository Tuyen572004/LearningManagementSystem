using LearningManagementSystem.Models;
using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace LearningManagementSystem.Helpers
{
    /// <summary>
    /// Converts a Department object to a string representation and vice versa.
    /// </summary>
    public class DepartmentConverter : IValueConverter
    {
        /// <summary>
        /// Converts a Department object to a string representation.
        /// </summary>
        /// <param name="value">The Department object to convert.</param>
        /// <param name="targetType">The type of the target property. This parameter is not used.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. This parameter is not used.</param>
        /// <param name="language">The language of the conversion. This parameter is not used.</param>
        /// <returns>A string representation of the Department object, or an empty string if the value is not a Department object.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Department department)
            {
                return $"{department.DepartmentCode} - {department.DepartmentDesc}";
            }
            return string.Empty;
        }

        /// <summary>
        /// Converts a string representation back to a Department object.
        /// This method is not implemented and will throw a NotImplementedException if called.
        /// </summary>
        /// <param name="value">The string representation to convert back.</param>
        /// <param name="targetType">The type of the target property. This parameter is not used.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. This parameter is not used.</param>
        /// <param name="language">The language of the conversion. This parameter is not used.</param>
        /// <returns>Throws a NotImplementedException.</returns>
        /// <exception cref="NotImplementedException">Always thrown as this method is not implemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}