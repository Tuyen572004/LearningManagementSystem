using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services.UserService;
using Microsoft.Extensions.DependencyInjection;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for managing singular resources.
    /// </summary>
    public class SingularResourceViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the resource category.
        /// </summary>
        public ResourceCategory ResourceCategory { get; set; }

        /// <summary>
        /// Gets or sets the collection of resources.
        /// </summary>
        public FullObservableCollection<BaseResourceViewModel> Resources { get; set; }

        private readonly IDao _dao = App.Current.Services.GetService<IDao>();

        private readonly UserService _userService = new UserService();
        private readonly User _user;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingularResourceViewModel"/> class with a specified resource category.
        /// </summary>
        /// <param name="resourceCategory">The resource category to initialize with.</param>
        public SingularResourceViewModel(ResourceCategory resourceCategory)
        {
            ResourceCategory = resourceCategory;
            Resources = new FullObservableCollection<BaseResourceViewModel>();
        }
    }
}
