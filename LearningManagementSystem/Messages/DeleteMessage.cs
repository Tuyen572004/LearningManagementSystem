using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LearningManagementSystem.Messages
{

    /// <summary>
    /// Represents a message that indicates a delete action.
    /// </summary>
    public class DeleteMessage : ValueChangedMessage<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteMessage"/> class.
        /// </summary>
        /// <param name="resource">The resource to be deleted.</param>
        public DeleteMessage(object resource) : base(resource) { }
    }

    /// <summary>
    /// Represents a message that indicates a delete action for a submission.
    /// </summary>
    public class DeleteSubmissionMessage : DeleteMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteSubmissionMessage"/> class.
        /// </summary>
        /// <param name="resource">The submission resource to be deleted.</param>
        public DeleteSubmissionMessage(object resource) : base(resource) { }
    }

    /// <summary>
    /// Represents a message that indicates a delete action for a resource.
    /// </summary>
    public class DeleteResourceMessage : DeleteMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteResourceMessage"/> class.
        /// </summary>
        /// <param name="resource">The resource to be deleted.</param>
        public DeleteResourceMessage(object resource) : base(resource) { }
    }
}
