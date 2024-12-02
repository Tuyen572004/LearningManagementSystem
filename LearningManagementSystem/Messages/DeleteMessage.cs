using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LearningManagementSystem.Messages
{

    public class DeleteMessage : ValueChangedMessage<object>
    {

        public DeleteMessage(object resource) : base(resource) { }
    }

    public class DeleteSubmissionMessage: DeleteMessage
    {
        public DeleteSubmissionMessage(object resource) : base(resource) { }
    }

    public class DeleteResourceMessage : DeleteMessage
    {
        public DeleteResourceMessage(object resource) : base(resource) { }
    }
}
