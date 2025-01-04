#nullable enable
using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    partial class TeachersInClassCUDViewModel(IDao dao, int classId) : CUDViewModel(dao)
    {
        private readonly int _classId = classId;

        /// <summary>
        /// Gets the columns to be ignored.
        /// </summary>
        public override IEnumerable<string> IgnoringColumns => ["IsValid"];

        /// <summary>
        /// Gets the order of the columns.
        /// </summary>
        public override IEnumerable<string> ColumnOrder => ["Id", "UserId", "TeacherCode", "TeacherName", "Email", "PhoneNo"];

        /// <summary>
        /// Gets the column converters.
        /// </summary>
        public override IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [];

        /// <summary>
        /// Gets the message of the specified item.
        /// </summary>
        /// <param name="item">The item to get the message of.</param>
        /// <returns>The message of the item, or null if the item is not a Teacher or has no errors.</returns>
        public override InfoBarMessage? GetMessageOf(object item)
        {
            if (item is not Teacher teacher)
            {
                return null;
            }
            if ((teacher as INotifyDataErrorInfoExtended).HasErrors)
            {
                var errors = (teacher as INotifyDataErrorInfoExtended).GetErrors(null).Cast<string>().ToList() ?? [];
                var newMessage = string.Join("\n", errors);
                return new()
                {
                    Title = (teacher.Id > 0) ? $"Error with teacher Id {teacher.Id}" : $"Error with the newly created teacher",
                    Message = newMessage,
                    Severity = InfoBarMessageSeverity.Error
                };
            }
            return null;
        }

        /// <summary>
        /// Gets the row status of the specified item.
        /// </summary>
        /// <param name="item">The item to get the row status of.</param>
        /// <returns>The row status of the item, or null if the item is not a Teacher or does not implement INotifyDataErrorInfoExtended.</returns>
        public override RowStatus? GetRowStatus(object item)
        {
            if (item is not Teacher teacher)
            {
                return null;
            }
            if (teacher is not INotifyDataErrorInfoExtended dataNotifier)
            {
                return null;
            }

            if (!dataNotifier.HasErrors)
            {
                if (teacher.Id == -1)
                {
                    return RowStatus.New;
                }
                return RowStatus.Normal;
            }
            return RowStatus.Error;
        }

        /// <summary>
        /// Gets the filter for transferring items.
        /// </summary>
        public override GroupValidator? TransferingItemFilter => (newItem, existingItems) =>
        {
            var existingTeachers = existingItems.Cast<Teacher>();
            var newTeacher = newItem as Teacher;
            var existingTeacher = existingTeachers.FirstOrDefault(s => s.Id == newTeacher?.Id);
            if (existingTeacher != null)
            {
                return false;
            }
            return true;
        };

        /// <summary>
        /// Gets the checker for adding items.
        /// </summary>
        public override ItemChecker? ItemCheckForAdd => (object item) => item is Teacher teacher && teacher.Id >= 0;

        /// <summary>
        /// Gets the inserter for adding items to the DAO.
        /// </summary>
        public override ItemDaoUpdater? ItemDaoInserter => (IEnumerable<object> items) =>
        {
            var insertingTeachers = items.Cast<Teacher>();
            var insertingIds = insertingTeachers.Select(teacher => teacher.Id);
            var (addedIds, addedCount, invalidIdsInfo) = _dao.AddTeachersToClass(insertingIds, _classId);
            var addedTeachers = insertingTeachers.Where(teacher => addedIds.Contains(teacher.Id)).Cast<object>();
            var invalidTeachersInfo = invalidIdsInfo.Select(info => (insertingTeachers.First(teacher => teacher.Id == info.teacherId) as object, info.error));

            return (addedTeachers.ToList(), addedCount, new List<(object, IEnumerable<string>)>(invalidTeachersInfo));
        };

        /// <summary>
        /// Gets the deleter for removing items from the DAO.
        /// </summary>
        public override ItemDaoUpdater? ItemDaoDeleter => (IEnumerable<object> items) =>
        {
            var deletingTeachers = items.Cast<Teacher>();
            var deletingIds = deletingTeachers.Select(teacher => teacher.Id);
            var (deletedIds, deletedCount, invalidIdsInfo) = _dao.RemoveTeachersFromClass(deletingIds, _classId);
            var deletedTeachers = deletingTeachers.Where(teacher => deletedIds.Contains(teacher.Id)).Cast<object>();
            var invalidTeachersInfo = invalidIdsInfo.Select(info => (deletingTeachers.First(teacher => teacher.Id == info.teacherId) as object, info.error));

            return (deletedTeachers.ToList(), deletedCount, new List<(object, IEnumerable<string>)>(invalidTeachersInfo));
        };
    }
}
