using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LearningManagementSystem.Messages
{
    public class BusyMessage : ValueChangedMessage<bool>
    {
        public BusyMessage(bool isBusy) : base(isBusy)
        {
        }
    }
}
