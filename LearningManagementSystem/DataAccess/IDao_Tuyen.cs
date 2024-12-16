using LearningManagementSystem.Models;

namespace LearningManagementSystem.DataAccess
{
    public partial interface IDao
    {
        void UpdateNotification(Notification notification);
        void DeleteAttachmentByNotificationId(int id);

        void AddNotification(Notification notification);

        Notification FindNotificationById(int id);

        void DeleteNotificationById(int id);
        bool checkIfAssignmentIsSubmitted(int id);
    }
}
