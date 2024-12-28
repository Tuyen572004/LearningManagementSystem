#nullable enable
using CommunityToolkit.Mvvm.Messaging;
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
    public sealed partial class UserAssignmentForStudentsPage : Page
    {
        private readonly UserAssignmentViewModel _userAssignmentViewModel;
        private readonly SimplifiedStudentHolderViewModel _holderViewModel;
        internal UserAssignmentViewModel? UserAssignmentViewModel => _userAssignmentViewModel;
        public UserAssignmentForStudentsPage()
        {
            this.InitializeComponent();

            _userAssignmentViewModel = new(
                App.Current.Services.GetService<IDao>()!,
                new Services.UserService.UserService()
                )
            {
                RowsPerPage = 100,
            };
            _holderViewModel = new(App.Current.Services.GetService<IDao>()!)
            {
                RowsPerPage = 100,
            };

            UserTable.PagingOptionsBar.CollapsePagingDetails();
            StudentTable.PagingOptionsBar.CollapsePagingDetails();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is IEnumerable<object> items)
            {
                _userAssignmentViewModel.ConvertObjectsToUsersThenPopulate(items);
                _holderViewModel.PopulateStudents(items);
            }
        }

        private void AssignentConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            _userAssignmentViewModel.OnConfirmAssignment();
            if (Frame.CanGoBack)
            {
                WeakReferenceMessenger.Default.Send(new UserAssignmentMessage(true, UserAssignmentTarget.Student));
                Frame.GoBack();
            }
        }

        private void AssignmentCancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
