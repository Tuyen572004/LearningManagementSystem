
using CloudinaryDotNet.Actions;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using System.Linq;
using System.Resources;

namespace LearningManagementSystem.ViewModels
{
    public class ResourceViewModel : BaseViewModel
    {
        public FullObservableCollection<SingularResourceViewModel> SingularResources { get; set; }

        private IDao _dao;

        public ResourceViewModel()
        {
            _dao = new SqlDao();
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

            resources = _dao.findDocumentsByClassId(classId);
            resourcesViewModel = convertToViewModels(resources);
            SingularResources.FirstOrDefault(x => x.ResourceCategory.Id == (int)ResourceCategoryEnum.Document).Resources = resourcesViewModel;
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
    }
}
