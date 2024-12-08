using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Helpers
{
    public enum RowStatus
    {
        Normal,
        New,
        Commited,
        Warning,
        Error
    }
    public interface IRowStatusDeterminer
    {
        public RowStatus? GetRowStatus(object item);
    }
    public partial class RowColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is IRowStatusDeterminer determiner)
            {
                return determiner.GetRowStatus(value) switch
                {
                    RowStatus.New => new SolidColorBrush(Colors.LightGreen),
                    RowStatus.Commited => new SolidColorBrush(Colors.LightBlue),
                    RowStatus.Warning => new SolidColorBrush(Colors.LightYellow),
                    RowStatus.Error => new SolidColorBrush(Colors.LightCoral),
                    _ => new SolidColorBrush(Colors.Transparent)
                };
            }
            return new SolidColorBrush(Colors.Green);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
