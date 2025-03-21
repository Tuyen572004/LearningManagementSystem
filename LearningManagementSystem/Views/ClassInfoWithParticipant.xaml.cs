#nullable enable
using CloudinaryDotNet.Actions;
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClassInfoWithParticipant : Microsoft.UI.Xaml.Controls.Page
    {
        private StudentsInClassViewModel? _studentsInClassViewModel = null;
        private TeachersInClassViewModel? _teachersInClassViewModel = null;
        private SimpleClassViewModel? _simpleClassViewModel = null;
        private int _classId = -1;
        public ClassInfoWithParticipant()
        {
            this.InitializeComponent();

            //var mockClassId = 14;

            //_studentsInClassViewModel = new StudentsInClassViewModel(App.Current.Services.GetService<IDao>()!, mockClassId)
            //{
            //    RowsPerPage = 5
            //};
            //_studentsInClassViewModel.LoadInitialItems();
            //_teachersInClassViewModel = new TeachersInClassViewModel(App.Current.Services.GetService<IDao>()!, mockClassId)
            //{
            //    RowsPerPage = 5
            //};
            //_teachersInClassViewModel.LoadInitialItems();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is int classId)
            //if (14 is int classId)
            {
                _classId = classId;
                _simpleClassViewModel = new SimpleClassViewModel(App.Current.Services.GetService<IDao>()!, classId);
                _simpleClassViewModel.LoadRequiredInformation();
                _studentsInClassViewModel = new StudentsInClassViewModel(App.Current.Services.GetService<IDao>()!, classId)
                {
                    RowsPerPage = 5
                };
                _studentsInClassViewModel.LoadInitialItems();
                _teachersInClassViewModel = new TeachersInClassViewModel(App.Current.Services.GetService<IDao>()!, classId)
                {
                    RowsPerPage = 5
                };
                _teachersInClassViewModel.LoadInitialItems();
            }
        }


        private void EditTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditTeachersInClassPage), _classId);
        }

        private void EditStudentsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditStudentsInClassPage), _classId);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }


        private bool _isProcessingNavigation = false;

        // TODO: Tuyen's part to navigate to the ClassDetailPage
        private async void SeeClassDetailButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isProcessingNavigation)
            {
                return;
            }
            _isProcessingNavigation = true;
            var navigationParam = _simpleClassViewModel?.DetailPageNavigatingParameter();
            if (navigationParam is not null)
            {
                Frame.Navigate(typeof(ClassDetailPage), navigationParam);
            }
            else
            {
                await ShowErrorDialog(
                    "Failed to navigate to the detail page of the class",
                    """
                    The page fails to get sufficient data to navigate!
                    (The Class, the Course, or the Department of the current class may be not correct)
                    """
                    );
            }
            _isProcessingNavigation = false;
        }

        private async Task ShowErrorDialog(string title, string message)
        {
            ContentDialog errorDialog = new()
            {
                XamlRoot = this.XamlRoot,
                Title = title,
                Content = message,
                CloseButtonText = "Ok"
            };

            await errorDialog.ShowAsync();
        }

        //private void TableSelector_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        //{
        //    if (sender.SelectedItem == StudentTableBar)
        //    {
        //        TeacherTableBar.Visibility = Visibility.Collapsed;
        //        StudentTableBar.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        TeacherTableBar.Visibility = Visibility.Visible;
        //        StudentTableBar.Visibility = Visibility.Collapsed;
        //    }
        //}
    }
}
