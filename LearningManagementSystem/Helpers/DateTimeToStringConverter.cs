using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LearningManagementSystem.Helpers
{
    public partial class DateTimeToStringConverter: IValueConverter
    {
        static public readonly DateTime ERROR_DATETIME = new(0609, 06, 09);
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

        [GeneratedRegex(@"(?<!\d)(\d)(?=/)")]
        private static partial Regex SingleDigitRegex();

        [GeneratedRegex(@"^([1-9]|0[1-9]|[12][0-9]|3[01])/([1-9]|0[1-9]|1[0-2])/\d{4}$")]
        private static partial Regex DateRegex();
    }
}
