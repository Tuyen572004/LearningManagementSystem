using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Services.EmailService.Model
{
    /// <summary>
    /// Represents an account with a name and email address.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Gets or sets the name of the account.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the email address of the account.
        /// </summary>
        public string email { get; set; }
    }
}
