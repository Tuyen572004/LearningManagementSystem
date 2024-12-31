using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Services.UserService;
using LearningManagementSystem.Views;
using Microsoft.UI.Xaml.Controls;
using System;

namespace LearningManagementSystem.ViewModels
{
    public class ClassDetailViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the EnrollmentViewModel.
        /// </summary>
        public EnrollmentClassViewModel EnrollmentViewModel { get; set; }

        /// <summary>
        /// Gets or sets the ResourceViewModel.
        /// </summary>
        public ResourceViewModel ResourceViewModel { get; set; }

        /// <summary>
        /// Indicates whether the current user can edit.
        /// </summary>
        public readonly bool CanEdit;

        /// <summary>
        /// Event handler for ComboBox selection changes.
        /// </summary>
        public SelectionChangedEventHandler ComboBox_SelectionChanged { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDetailViewModel"/> class.
        /// </summary>
        public ClassDetailViewModel()
        {
            EnrollmentViewModel = new EnrollmentClassViewModel();
            ResourceViewModel = new ResourceViewModel();
            CanEdit = !UserService.GetCurrentUser().Result.Role.Equals(RoleEnum.GetStringValue(Role.Student));

            ComboBox_SelectionChanged = new SelectionChangedEventHandler(OnSelectionChanged);
        }

        /// <summary>
        /// Handles the ComboBox selection change event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
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
                    assignmentViewModel.Assignment.CreatedBy = UserService.GetCurrentUser().Result.Id;
                    assignmentViewModel.Assignment.ResourceCategoryId = (int)ResourceCategoryEnum.Assignment;

                    WeakReferenceMessenger.Default.Send(new NavigationMessage(typeof(AddAssignment), assignmentViewModel));
                }
                else if (selectedCategory.Id == (int)ResourceCategoryEnum.Notification)
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
