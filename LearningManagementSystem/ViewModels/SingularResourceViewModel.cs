using LearningManagementSystem.Helpers;
using LearningManagementSystem.EModels;

namespace LearningManagementSystem.ViewModels
{
    public class SingularResourceViewModel : PropertyChangedClass
    {
        public Resourcecategory ResourceCategory { get; set; }
        public FullObservableCollection<BaseResource> Resources { get; set; }

        public SingularResourceViewModel(Resourcecategory resourceCategory)
        {
            ResourceCategory = resourceCategory;
            Resources = new FullObservableCollection<BaseResource>();
        }
    }
}
