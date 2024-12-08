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

namespace LearningManagementSystem.ViewModels
{
    public class ClassDetailViewModel : BaseViewModel
    {
        public EnrollmentClassViewModel EnrollmentViewModel { get; set; }

        public ResourceViewModel ResourceViewModel { get; set; }

        private readonly IDao _dao = new SqlDao();

        private readonly CloudinaryService _cloudinaryService = new CloudinaryService();

        private readonly UserService _userService = new UserService();

        public readonly bool IsTeacher;

        public SelectionChangedEventHandler ComboBox_SelectionChanged { get; }


        public ClassDetailViewModel()
        {
            EnrollmentViewModel = new EnrollmentClassViewModel();
            ResourceViewModel = new ResourceViewModel();
            IsTeacher = _userService.GetCurrentUser().Result.Role.Equals(RoleEnum.GetStringValue(Role.Teacher));

            ComboBox_SelectionChanged = new SelectionChangedEventHandler(OnSelectionChanged);
            WeakReferenceMessenger.Default.Register<DeleteResourceMessage>(this,async (r, m) =>
            {
                await DeleteResource(m.Value as BaseResourceViewModel);
            });
        }

        ~ClassDetailViewModel()
        {
            WeakReferenceMessenger.Default.Unregister<DeleteMessage>(this);
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
                    assignmentViewModel.Assignment.TeacherId = _userService.GetCurrentUser().Result.Id;
                    assignmentViewModel.Assignment.ResourceCategoryId = (int)ResourceCategoryEnum.Assignment;

                    WeakReferenceMessenger.Default.Send(new NavigationMessage(typeof(AddAssignment), assignmentViewModel));
                }
            }
        }

        private async Task DeleteResource(BaseResourceViewModel resource)
        {
            if (resource != null)
            {
                var categoryId = resource.BaseResource.ResourceCategoryId;
                if (categoryId == (int)ResourceCategoryEnum.Assignment)
                {
                    var id = resource.BaseResource.Id;
                    Assignment assignment = _dao.GetAssignmentById(id);
                    if (assignment != null)
                    {
                        if (assignment.FilePath != null)
                        {
                            await _cloudinaryService.DeleteFileByUriAsync(assignment.FilePath);
                        }
                        _dao.DeleteAssignmentById(assignment.Id);
                        ResourceViewModel.SingularResources.FirstOrDefault(r => r.ResourceCategory.Id == (int)ResourceCategoryEnum.Assignment).Resources.Remove(resource);
                    }
                }
                //else if (resource.ResourceCategoryId == (int)ResourceCategoryEnum.Notification)
                //{
                //    _dao.DeleteNotificationById(resource.Id);
                //}
                //else if (resource.ResourceCategoryId == (int)ResourceCategoryEnum.Document)
                //{
                //    _dao.DeleteDocumentById(resource.Id);
                //}
            }
        }
    }
}
