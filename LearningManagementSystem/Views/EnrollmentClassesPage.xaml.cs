using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Windows.Input;

namespace LearningManagementSystem.Views
{
    public sealed partial class EnrollmentClassesPage : Page
    {
        private EnrollmentClassesViewModel ViewModel { get; set; }
        public EnrollmentClassesPage()
        {
            this.InitializeComponent();
            ViewModel = new EnrollmentClassesViewModel();
            ViewModel.LoadEnrolledClasses();
            ViewModel.NavigateCommand = new RelayCommand<EnrollmentViewModel>(NavigateToClassDetail);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void NavigateToClassDetail(EnrollmentViewModel classViewModel)
        {
            Frame.Navigate(typeof(ClassDetailPage), classViewModel);
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
