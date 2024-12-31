using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LearningManagementSystem.Messages
{
    /// <summary>
    /// Represents a message indicating the busy state.
    /// </summary>
    public class BusyMessage : ValueChangedMessage<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusyMessage"/> class.
        /// </summary>
        /// <param name="isBusy">A boolean value indicating whether the system is busy.</param>
        public BusyMessage(bool isBusy) : base(isBusy)
        {
        }
    }
}
