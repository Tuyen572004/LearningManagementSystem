using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Services;
using LearningManagementSystem.Views;
using Microsoft.UI.Xaml.Controls;
using LearningManagementSystem.Models;
using Microsoft.UI.Xaml;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LearningManagementSystem.ViewModels
{
    public class ClassDetailViewModel : BaseViewModel
    {
        public EnrollmentClassViewModel EnrollmentViewModel { get; set; }

        public ResourceViewModel ResourceViewModel { get; set; }

        public readonly bool IsTeacher;

        public SelectionChangedEventHandler ComboBox_SelectionChanged { get; }


        public ClassDetailViewModel()
        {
            EnrollmentViewModel = new EnrollmentClassViewModel();
            ResourceViewModel = new ResourceViewModel();
            IsTeacher = UserService.GetCurrentUser().Result.Role.Equals(RoleEnum.GetStringValue(Role.Teacher));

            ComboBox_SelectionChanged = new SelectionChangedEventHandler(OnSelectionChanged);
           
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected item
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox?.SelectedItem as SingularResourceViewModel;

            // Do something with the selected item
            if (selectedItem != null)
            {
                var selectedCategory = selectedItem.ResourceCategory;
                if (selectedCategory.Id == (int)ResourceCategoryEnum.Assignment)
                {
                    AssignmentViewModel assignmentViewModel = new AssignmentViewModel();
                    assignmentViewModel.Assignment.ClassId = EnrollmentViewModel.Class.Id;
                    assignmentViewModel.Assignment.TeacherId = UserService.GetCurrentUser().Result.Id;
                    assignmentViewModel.Assignment.ResourceCategoryId = (int)ResourceCategoryEnum.Assignment;

                    WeakReferenceMessenger.Default.Send(new NavigationMessage(typeof(AddAssignment), assignmentViewModel));
                }
                else if(selectedCategory.Id == (int)ResourceCategoryEnum.Notification)
                {
                    NotificationViewModel notificationViewModel = new NotificationViewModel();
                    notificationViewModel.Notification.ClassId = EnrollmentViewModel.Class.Id;
                    notificationViewModel.Notification.CreatedBy = UserService.GetCurrentUser().Result.Id;
                    notificationViewModel.Notification.ResourceCategoryId = (int)ResourceCategoryEnum.Notification;

                    WeakReferenceMessenger.Default.Send(new NavigationMessage(typeof(AddNotification), notificationViewModel));
                }
            }
        }

       
    }
}
