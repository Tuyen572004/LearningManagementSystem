//using LearningManagementSystem.DataAccess;
//using LearningManagementSystem.Enums;
//using LearningManagementSystem.Helper;
//using LearningManagementSystem.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Windows.Services.Maps;

//namespace LearningManagementSystem.ViewModels
//{
//    public class DetailClassViewModel : BaseViewModel
//    {
//        // a map with key as resource category and value as list of resources
//        public Dictionary<ResourceCategory, List<BaseResource>> ResourceMap { get; set; }

//        private IDao _dao;

//        public DetailClassViewModel()
//        {
//            ResourceMap = new Dictionary<ResourceCategory, List<BaseResource>>();
//            _dao = new SqlDao();
//        }
//        public void LoadResourcesByClassId(int classId)
//        {
//            var ResourceCategory = _dao.findAllResourceCategories();
//            foreach (var category in ResourceCategory)
//            {
//                if (category.Id == (int)ResourceCategoryEnum.Assignment)
//                {
//                    ResourceMap.Add(category, _dao.findAssignmentsByClassId(classId));
//                }
//                if (category.Id == (int)ResourceCategoryEnum.Notification)
//                {
//                    ResourceMap.Add(category, _dao.findNotificationsByClassId(classId));
//                }
//                if (category.Id == (int)ResourceCategoryEnum.Document)
//                {
//                    ResourceMap.Add(category, _dao.findDocumentsByClassId(classId));
//                }
//            }


//        }
//    }


//}
//}
