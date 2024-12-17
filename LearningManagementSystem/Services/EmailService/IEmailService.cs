using LearningManagementSystem.Services.EmailService.Model;
using System.Threading.Tasks;

namespace LearningManagementSystem.Services.EmailService
{

    public interface IEmailService
    {
        Task sendEmail(EmailRequest emailRequest);
    }
}
