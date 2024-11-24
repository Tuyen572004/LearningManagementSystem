using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Messages
{
    public class DialogMessage : ValueChangedMessage<string>
    {
        public string Title { get; }

        public DialogMessage(string title, string content) : base(content)
        {
            Title = title;
        }
    }
}
