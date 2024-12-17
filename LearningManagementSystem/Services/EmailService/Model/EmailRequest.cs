using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Services.EmailService.Model
{
    public class EmailRequest
    {
        public Account sender { get; set; }
        public List<Account> to { get; set; }
        public string subject { get; set; }
        public string htmlContent { get; set; }
    }
}
