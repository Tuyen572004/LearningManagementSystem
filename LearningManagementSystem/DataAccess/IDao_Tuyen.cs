using LearningManagementSystem.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        List<int> findStudentIdByClassId(int id);

        List<StudentVer2> findStudentsByIdIn(List<int> ids);
        User findUserById(int? userId);
        Teacher GetTeacherByUserId(int id);
        ObservableCollection<Class> GetEnrolledClassesByTeacherId(int id);
    }
}
