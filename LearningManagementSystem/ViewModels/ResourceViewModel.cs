using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services.CloudinaryService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    public class ResourceViewModel : BaseViewModel
    {
        public FullObservableCollection<SingularResourceViewModel> SingularResources { get; set; }

        public bool IsBusy { get; set; }

        private IDao _dao;

        private readonly ICloudinaryService _cloudinaryService = App.Current.Services.GetService<ICloudinaryService>();

        public ResourceViewModel()
        {
            _dao = App.Current.Services.GetService<IDao>();;
            SingularResources = new FullObservableCollection<SingularResourceViewModel>();
            LoadResourceTitles();

            
        }

        private void LoadResourceTitles()
        {
            var categories = _dao.findAllResourceCategories();
            foreach (var category in categories)
            {
                SingularResources.Add(new SingularResourceViewModel(category));
            }
        }

        public void LoadMoreItems(int classId)
        {
            var resources = new FullObservableCollection<BaseResource>();
            FullObservableCollection<BaseResourceViewModel> resourcesViewModel;

            resources = _dao.findAssignmentsByClassId(classId);
            resourcesViewModel = convertToViewModels(resources);
            SingularResources.FirstOrDefault(x => x.ResourceCategory.Id == (int)ResourceCategoryEnum.Assignment).Resources = resourcesViewModel;

            resources = _dao.findNotificationsByClassId(classId);
            resourcesViewModel = convertToViewModels(resources);
            SingularResources.FirstOrDefault(x => x.ResourceCategory.Id == (int)ResourceCategoryEnum.Notification).Resources = resourcesViewModel;

            
        }


        private FullObservableCollection<BaseResourceViewModel> convertToViewModels(FullObservableCollection<BaseResource> baseResources)
        {
            var baseResourceViewModels = new FullObservableCollection<BaseResourceViewModel>();
            foreach (var resource in baseResources)
            {
                baseResourceViewModels.Add(new BaseResourceViewModel(resource));
            }
            return baseResourceViewModels;
        }

        public async Task DeleteResource(BaseResourceViewModel resource)
        {
            IsBusy = true;
            if (resource == null)
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", "Resource cannot be null"));
                return;
            }

            try
            {
                var categoryId = resource.BaseResource.ResourceCategoryId;

                if (categoryId == (int)ResourceCategoryEnum.Assignment)
                {
                    await DeleteAssignmentResource(resource);
                }
                else if (categoryId == (int)ResourceCategoryEnum.Notification)
                {
                    await DeleteNotificationResource(resource);
                }
                else
                {
                    IsBusy = false;
                    WeakReferenceMessenger.Default.Send(new DialogMessage("Error", "Unknown resource category"));
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Failed to delete resource: {ex.Message}"));
            }
        }

        private async Task DeleteAssignmentResource(BaseResourceViewModel resource)
        {
            var id = resource.BaseResource.Id;
            var assignment = _dao.GetAssignmentById(id);

            if (assignment == null)
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", "Assignment not found"));
                return;
            }

            if (_dao.checkIfAssignmentIsSubmitted(assignment.Id))
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", "Assignment has been submitted, cannot be deleted"));
                return;
            }

            if (!string.IsNullOrEmpty(assignment.FilePath))
            {
                await _cloudinaryService.DeleteFileByUriAsync(assignment.FilePath);
            }

            _dao.DeleteAssignmentById(assignment.Id);
            RemoveResourceFromViewModel(ResourceCategoryEnum.Assignment, resource);
        }

        private async Task DeleteNotificationResource(BaseResourceViewModel resource)
        {
            var id = resource.BaseResource.Id;
            var notification = _dao.FindNotificationById(id);

            if (notification == null)
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", "Notification not found"));
                return;
            }

            if (!string.IsNullOrEmpty(notification.FilePath))
            {
                await _cloudinaryService.DeleteFileByUriAsync(notification.FilePath);
            }

            _dao.DeleteNotificationById(notification.Id);
            RemoveResourceFromViewModel(ResourceCategoryEnum.Notification, resource);
        }

        private void RemoveResourceFromViewModel(ResourceCategoryEnum category, BaseResourceViewModel resource)
        {
            var singularResource = SingularResources
                .FirstOrDefault(r => r.ResourceCategory.Id == (int)category);

            singularResource?.Resources.Remove(resource);
        }

    }
}
