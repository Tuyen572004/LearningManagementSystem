using System;

namespace LearningManagementSystem.Models
{
    public class Notification : BaseResource
    {
        public string NotificationText { get; set; }
        public DateTime PostDate { get; set; }

    }
}
