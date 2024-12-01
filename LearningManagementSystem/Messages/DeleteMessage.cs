using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LearningManagementSystem.Messages
{

    public class DeleteMessage : ValueChangedMessage<object>
    {

        public DeleteMessage(object resource) : base(resource) { }
    }
}
