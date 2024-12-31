using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Services.EmailService.Model
{
    /// <summary>
    /// Represents an email request with sender, recipients, subject, and HTML content.
    /// </summary>
    public class EmailRequest
    {
        /// <summary>
        /// Gets or sets the sender of the email.
        /// </summary>
        public Account sender { get; set; }

        /// <summary>
        /// Gets or sets the list of recipients of the email.
        /// </summary>
        public List<Account> to { get; set; }

        /// <summary>
        /// Gets or sets the subject of the email.
        /// </summary>
        public string subject { get; set; }

        /// <summary>
        /// Gets or sets the HTML content of the email.
        /// </summary>
        public string htmlContent { get; set; }
    }
}
