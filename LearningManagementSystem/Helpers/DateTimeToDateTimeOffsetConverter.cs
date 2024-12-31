using Microsoft.UI.Xaml.Data;
using System;


namespace LearningManagementSystem.Helpers
{
    /// <summary>
    /// Converts a <see cref="DateTime"/> to a <see cref="DateTimeOffset"/> and vice versa.
    /// </summary>
    public class DateTimeToDateTimeOffsetConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="DateTime"/> to a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> value to convert.</param>
        /// <param name="targetType">The type of the target property. This parameter is not used.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. This parameter is not used.</param>
        /// <param name="language">The language of the conversion. This parameter is not used.</param>
        /// <returns>A <see cref="DateTimeOffset"/> that represents the converted <see cref="DateTime"/> value, or null if the value is not a <see cref="DateTime"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                return new DateTimeOffset(dateTime);  // Convert DateTime to DateTimeOffset
            }
            return null;  // Return null if value is not a DateTime
        }

        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> back to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">The <see cref="DateTimeOffset"/> value to convert back.</param>
        /// <param name="targetType">The type of the target property. This parameter is not used.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. This parameter is not used.</param>
        /// <param name="language">The language of the conversion. This parameter is not used.</param>
        /// <returns>A <see cref="DateTime"/> that represents the converted <see cref="DateTimeOffset"/> value, or the default <see cref="DateTime"/> if the value is not a <see cref="DateTimeOffset"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.DateTime;  // Convert DateTimeOffset back to DateTime
            }
            return default(DateTime);  // Return default DateTime if value is not a DateTimeOffset
        }
    }
}

