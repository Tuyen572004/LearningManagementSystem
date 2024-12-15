using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.DataAccess
{
    public partial interface IDao
    {
        void UpdateNotification(Notification notification);
        void DeleteAttachmentByNotificationId(int id);

        void AddNotification(Notification notification);

        Notification FindNotificationById(int id);

        void DeleteNotificationById(int id);
    }
}
