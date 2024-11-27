using LearningManagementSystem.ViewModels;
using LearningManagementSystem.Helpers;

namespace LearningManagementSystem.EModels
{
    public class BaseResource : PropertyChangedClass
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int ResourceCategoryId { get; set; }

        public string Title { get; set; }
    }
}
