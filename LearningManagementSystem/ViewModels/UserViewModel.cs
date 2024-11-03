using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    public partial class UserViewModel: BaseViewModel
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
