using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Messages
{
    /// <summary>
    /// Represents a message that contains dialog information including a title and content.
    /// </summary>
    public class DialogMessage : ValueChangedMessage<string>
    {
        /// <summary>
        /// Gets the title of the dialog message.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogMessage"/> class with the specified title and content.
        /// </summary>
        /// <param name="title">The title of the dialog message.</param>
        /// <param name="content">The content of the dialog message.</param>
        public DialogMessage(string title, string content) : base(content)
        {
            Title = title;
        }
    }
}
