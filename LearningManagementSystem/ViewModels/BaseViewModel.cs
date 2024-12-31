using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// Base class for ViewModels that implements the INotifyPropertyChanged interface.
    /// Provides methods to raise property change notifications.
    /// </summary>
    public partial class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Event that is raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed. This value is optional and can be automatically provided by the CallerMemberName attribute.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
