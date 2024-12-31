
using System;
using Microsoft.UI.Xaml.Data;

namespace LearningManagementSystem.Helpers
{
    /// <summary>
    /// Converts a <see cref="DateTime"/> object to a <see cref="TimeSpan"/> object and vice versa.
    /// </summary>
    public class DateTimeToTimeSpanConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="DateTime"/> object to a <see cref="TimeSpan"/> object.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> object to convert.</param>
        /// <param name="targetType">The type of the target property. This parameter is not used.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. This parameter is not used.</param>
        /// <param name="language">The language of the conversion. This parameter is not used.</param>
        /// <returns>A <see cref="TimeSpan"/> object representing the time part of the <see cref="DateTime"/> object, or <see cref="TimeSpan.Zero"/> if the input is null or invalid.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                // Return only the time part as a TimeSpan
                return dateTime.TimeOfDay;
            }
            return TimeSpan.Zero; // Default value if it's null or invalid
        }

        /// <summary>
        /// Converts a <see cref="TimeSpan"/> object back to a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> object to convert.</param>
        /// <param name="targetType">The type of the target property. This parameter is not used.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. This parameter is not used.</param>
        /// <param name="language">The language of the conversion. This parameter is not used.</param>
        /// <returns>A <see cref="DateTime"/> object with the date part set to today and the time part set to the <see cref="TimeSpan"/> value, or <see cref="DateTime.MinValue"/> if the input is null or invalid.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan timeSpan)
            {
                // Convert TimeSpan back to DateTime, keeping the date part unchanged
                return DateTime.Today.Add(timeSpan); // or any other date if needed
            }
            return DateTime.MinValue; // Default value if it's null or invalid
        }
    }
}




