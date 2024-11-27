using LearningManagementSystem.EModels;
using System;
using LearningManagementSystem.Helpers;

namespace LearningManagementSystem.ViewModels
{
    public partial class UserViewModel: PropertyChangedClass
    {
        // Get information in User
        // Get enrolled courses in Enrollment
        // Get taught courses in TeachersPerClass
        private User ManagingUser { get; set; }

        public UserViewModel()
        {
            ManagingUser = new User
            {
                Id = -1,
                Username = "",
                Email = "",
                Role = "Undetermine",
                CreatedAt = DateTime.Now,
            };
        }
    }
}
