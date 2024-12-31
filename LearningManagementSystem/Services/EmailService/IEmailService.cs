using LearningManagementSystem.Services.EmailService.Model;
using System.Threading.Tasks;

namespace LearningManagementSystem.Services.EmailService
{

    /// <summary>
    /// Interface for email service to send emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email based on the provided email request.
        /// </summary>
        /// <param name="emailRequest">The email request containing the email details.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task sendEmail(EmailRequest emailRequest);
    }
}
