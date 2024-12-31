namespace LearningManagementSystem.Services.EmailService.Model
{
    /// <summary>
    /// Represents the response received after sending an email.
    /// </summary>
    public class EmailResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the email message.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets the status of the email message.
        /// </summary>
        public string Status { get; set; }
    }
}
