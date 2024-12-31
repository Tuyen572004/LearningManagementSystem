using LearningManagementSystem.Services.UserService;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for editing a user.
    /// </summary>
    public class EditUserViewModel : UserViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether the password should be reset.
        /// </summary>
        public bool resetPassword { get; set; }

        /// <summary>
        /// Gets or sets the user service.
        /// </summary>
        public UserService MyUserService { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditUserViewModel"/> class.
        /// </summary>
        public EditUserViewModel()
        {
            resetPassword = false;
            MyUserService = new UserService();
        }
    }
}
