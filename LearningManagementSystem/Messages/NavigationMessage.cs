using System;

namespace LearningManagementSystem.Messages
{
    /// <summary>
    /// Represents a message used for navigation purposes within the Learning Management System.
    /// </summary>
    public class NavigationMessage /*: ValueChangedMessage<object>*/
    {
        /// <summary>
        /// Gets or sets the type of the destination page.
        /// </summary>
        public Type DestinationPage { get; set; }

        /// <summary>
        /// Gets or sets the parameter to be passed to the destination page.
        /// </summary>
        public object Parameter { get; set; }

        //public NavigationMessage(Type DestinationPage, object parameter) : base(parameter) {
        //    this.DestinationPage = DestinationPage;
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationMessage"/> class.
        /// </summary>
        /// <param name="destinationPage">The type of the destination page.</param>
        /// <param name="parameter">The parameter to be passed to the destination page.</param>
        public NavigationMessage(Type destinationPage, object parameter)
        {
            DestinationPage = destinationPage;
            Parameter = parameter;
        }
    }
}
