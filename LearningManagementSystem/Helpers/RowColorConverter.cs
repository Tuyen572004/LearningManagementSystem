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
    /// <summary>
    /// Represents the status of a row.
    /// </summary>
    public enum RowStatus
    {
        Normal,
        New,
        Commited,
        Warning,
        Error
    }
    /// <summary>
    /// Interface to determine the status of a row.
    /// </summary>
    public interface IRowStatusDeterminer
    {
        /// <summary>
        /// Gets the status of the specified row item.
        /// </summary>
        /// <param name="item">The row item.</param>
        /// <returns>The status of the row.</returns>
        public RowStatus? GetRowStatus(object item);
    }
    /// <summary>
    /// Converts row status to corresponding color.
    /// </summary>
    public partial class RowColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a row status to a SolidColorBrush.
        /// </summary>
        /// <param name="value">The row item.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter, expected to be an IRowStatusDeterminer.</param>
        /// <param name="language">The language.</param>
        /// <returns>A SolidColorBrush corresponding to the row status.</returns>
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

        /// <summary>
        /// Not implemented. Converts back from SolidColorBrush to row status.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Throws NotImplementedException.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
