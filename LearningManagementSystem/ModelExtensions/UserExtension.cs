#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    public partial class User
    {

        /// <summary>
        /// Gets or sets the password for the user.
        /// This password is only used to show the user's password in the UI
        /// when it is auto-generated the first time
        /// and then would be hashed and stored in the database.
        /// </summary>
        public string? Password { get; set; } = null;
    }
}
