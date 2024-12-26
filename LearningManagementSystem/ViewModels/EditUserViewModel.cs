using LearningManagementSystem.Services.UserService;

namespace LearningManagementSystem.ViewModels
{
    public class EditUserViewModel : UserViewModel
    {
        public bool resetPassword { get; set; }

        public UserService MyUserService { get; set; }
        public EditUserViewModel()
        {
            resetPassword = false;
            MyUserService = new UserService();
        }


    }
}
