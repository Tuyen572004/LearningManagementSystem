using System;

namespace LearningManagementSystem.Messages
{
    public class NavigationMessage /*: ValueChangedMessage<object>*/
    {
        public Type DestinationPage { get; set; }
        public object Parameter { get; set; }

        //public NavigationMessage(Type DestinationPage, object parameter) : base(parameter) {
        //    this.DestinationPage = DestinationPage;
        //}

        public NavigationMessage(Type destinationPage, object parameter)
        {
            DestinationPage = destinationPage;
            Parameter = parameter;
        }
    }
}
