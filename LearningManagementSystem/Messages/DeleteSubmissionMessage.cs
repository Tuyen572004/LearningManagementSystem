using CommunityToolkit.Mvvm.Messaging.Messages;
using LearningManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Messages
{
    public class DeleteSubmissionMessage : ValueChangedMessage<SubmissionViewModel>
    {
        public DeleteSubmissionMessage(SubmissionViewModel submissionViewModel) : base(submissionViewModel)
        {
        }
    }
}
