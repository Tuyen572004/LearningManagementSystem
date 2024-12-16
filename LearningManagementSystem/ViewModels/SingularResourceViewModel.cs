using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LearningManagementSystem.ViewModels
{
    public class SingularResourceViewModel : BaseViewModel
    {
        public ResourceCategory ResourceCategory { get; set; }
        public FullObservableCollection<BaseResourceViewModel> Resources { get; set; }

        private readonly IDao _dao = App.Current.Services.GetService<IDao>();

        private readonly UserService _userService = new UserService();
        private readonly User _user;


        public SingularResourceViewModel(ResourceCategory resourceCategory)
        {
            ResourceCategory = resourceCategory;
            Resources = new FullObservableCollection<BaseResourceViewModel>();
        }


    }
}
