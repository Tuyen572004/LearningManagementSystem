using LearningManagementSystem.DataAccess;
using LearningManagementSystem.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClassInfoWithParticipant : Page
    {
        private readonly StudentsInClassViewModel _studentsInClassViewModel;
        private readonly TeachersInClassViewModel _teachersInClassViewModel;
        public ClassInfoWithParticipant()
        {
            this.InitializeComponent();

            var mockClassId = 14;

            _studentsInClassViewModel = new StudentsInClassViewModel(App.Current.Services.GetService<IDao>()!, mockClassId)
            {
                RowsPerPage = 5
            };
            _studentsInClassViewModel.LoadInitialItems();
            _teachersInClassViewModel = new TeachersInClassViewModel(App.Current.Services.GetService<IDao>()!, mockClassId)
            {
                RowsPerPage = 5
            };
            _teachersInClassViewModel.LoadInitialItems();
        }

        private void TableSelector_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            if (sender.SelectedItem == StudentTableBar)
            {
                TeacherTableBar.Visibility = Visibility.Collapsed;
                StudentTableBar.Visibility = Visibility.Visible;
            }
            else
            {
                TeacherTableBar.Visibility = Visibility.Visible;
                StudentTableBar.Visibility = Visibility.Collapsed;
            }
        }
    }
}
