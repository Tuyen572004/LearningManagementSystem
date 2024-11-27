using LearningManagementSystem.Helpers;

namespace LearningManagementSystem.ViewModels
{

    public class ListResource : PropertyChangedClass
    {
        public FullObservableCollection<SingularResourceViewModel> SingularResources { get; set; }

        public ListResource()
        {
            SingularResources = new FullObservableCollection<SingularResourceViewModel>();
        }

    }

}
