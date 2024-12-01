using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.WinUI.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
