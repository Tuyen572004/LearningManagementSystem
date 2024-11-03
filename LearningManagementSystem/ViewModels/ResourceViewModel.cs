using LearningManagementSystem.Command;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Helper;
using LearningManagementSystem.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LearningManagementSystem.ViewModels
{
    public class ResourceViewModel : BaseViewModel
    {
        public ListResource ListResource { get; set; }

        private IDao _dao;

        public ResourceViewModel()
        {
            _dao = new SqlDao();
            ListResource = new ListResource();
            LoadResourceTitles();
        }

        private void LoadResourceTitles()
        {
            var categories = _dao.findAllResourceCategories();
            foreach (var category in categories)
            {
                ListResource.SingularResources.Add(new SingularResourceViewModel(category));
            }
        }

        public void LoadMoreItems(ResourceCategory category, int classId)
        {
            int categoryId = category.Id;
            if (categoryId == 1)
            {
                var assignments = _dao.findAssignmentsByClassId(classId);
                ListResource.SingularResources.First(x => x.ResourceCategory.Id == categoryId).Resources = assignments;
            }
            else if (categoryId == 2)
            {
                var notifications = _dao.findNotificationsByClassId(classId);
                ListResource.SingularResources.First(x => x.ResourceCategory.Id == categoryId).Resources = notifications;
            }
            else if (categoryId == 3)
            {
                var resources = _dao.findDocumentsByClassId(classId);
                ListResource.SingularResources.First(x => x.ResourceCategory.Id == categoryId).Resources = resources;
            }
        }
    }
}
