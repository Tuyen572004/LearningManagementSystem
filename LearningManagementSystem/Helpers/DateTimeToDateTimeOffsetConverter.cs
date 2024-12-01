using Microsoft.UI.Xaml.Data;
using System;


namespace LearningManagementSystem.Helpers
{
    public class DateTimeToDateTimeOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                return new DateTimeOffset(dateTime);  // Convert DateTime to DateTimeOffset
            }
            return null;  // Return null if value is not a DateTime
        }

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

