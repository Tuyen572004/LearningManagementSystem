
using System;
using Microsoft.UI.Xaml.Data;

namespace LearningManagementSystem.Helpers
{
    public class DateTimeToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                // Return only the time part as a TimeSpan
                return dateTime.TimeOfDay;
            }
            return TimeSpan.Zero; // Default value if it's null or invalid
        }

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




