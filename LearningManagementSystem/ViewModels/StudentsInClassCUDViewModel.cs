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
    partial class StudentsInClassCUDViewModel(IDao dao, int classId) : CUDViewModel(dao)
    {
        private readonly int _classId = classId;

        /// <summary>
        /// Gets the order of columns to be displayed.
        /// </summary>
        public override IEnumerable<string> ColumnOrder => ["Id", "UserId", "StudentCode", "StudentName", "Email", "BirthDate", "PhoneNo"];

        /// <summary>
        /// Gets the columns to be ignored.
        /// </summary>
        public override IEnumerable<string> IgnoringColumns => ["IsValid"];

        /// <summary>
        /// Gets the column converters.
        /// </summary>
        public override IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [
            ("BirthDate", new DateTimeToStringConverter()),
            ];

        /// <summary>
        /// Gets the message of the specified item.
        /// </summary>
        /// <param name="item">The item to get the message of.</param>
        /// <returns>The message of the item.</returns>
        public override InfoBarMessage? GetMessageOf(object item)
        {
            if (item is StudentVer2 student)
            {
                if ((student as INotifyDataErrorInfoExtended).HasErrors)
                {
                    var errors = (student as INotifyDataErrorInfoExtended).GetErrors(null) as List<string> ?? [];
                    var newMessage = string.Join("\n", errors);
                    return new()
                    {
                        Title = (student.Id > 0) ? $"Error with student Id {student.Id}" : $"Error with the newly created student",
                        Message = newMessage,
                        Severity = InfoBarMessageSeverity.Error
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the row status of the specified item.
        /// </summary>
        /// <param name="item">The item to get the row status of.</param>
        /// <returns>The row status of the item.</returns>
        public override RowStatus? GetRowStatus(object item)
        {
            if (item is StudentVer2 student)
            {
                if ((student as INotifyDataErrorInfoExtended).HasErrors)
                {
                    return RowStatus.Error;
                }
                if (student.Id == -1)
                {
                    return RowStatus.New;
                }
                return RowStatus.Normal;
            }
            return null;
        }

        /// <summary>
        /// Gets the filter for transferring items.
        /// </summary>
        public override GroupValidator? TransferingItemFilter => (newItem, existingItems) =>
        {
            var existingStudents = existingItems.OfType<StudentVer2>();
            var newStudent = newItem as StudentVer2;
            var existingStudent = existingStudents.FirstOrDefault(s => s.Id == newStudent?.Id);
            if (existingStudent != null)
            {
                return false;
            }
            return true;
        };

        /// <summary>
        /// Checks if the item can be added.
        /// </summary>
        public override ItemChecker? ItemCheckForAdd => (object item) => item is StudentVer2 student && student.Id >= 0;

        /// <summary>
        /// Inserts the specified items into the DAO.
        /// </summary>
        public override ItemDaoUpdater? ItemDaoInserter => (IEnumerable<object> items) =>
        {
            var insertingStudents = items.Cast<StudentVer2>();
            var insertingIds = insertingStudents.Select(student => student.Id);
            var (addedIds, addedCount, invalidIdsInfo) = _dao.AddStudentsToClass(insertingIds, _classId);
            var addedStudents = insertingStudents.Where(student => addedIds.Contains(student.Id)).Cast<object>();
            var invalidStudentsInfo = invalidIdsInfo.Select(info => (insertingStudents.First(student => student.Id == info.studentId) as object, info.error));

            return (addedStudents.ToList(), addedCount, new List<(object, IEnumerable<string>)>(invalidStudentsInfo));
        };

        /// <summary>
        /// Deletes the specified items from the DAO.
        /// </summary>
        public override ItemDaoUpdater? ItemDaoDeleter => (IEnumerable<object> items) =>
        {
            var deletingStudents = items.Cast<StudentVer2>();
            var deletingIds = deletingStudents.Select(student => student.Id);
            var (deletedIds, deletedCount, invalidIdsInfo) = _dao.RemoveStudentsFromClass(deletingIds, _classId);
            var deletedStudents = deletingStudents.Where(student => deletedIds.Contains(student.Id)).Cast<object>();
            var invalidStudentsInfo = invalidIdsInfo.Select(info => (deletingStudents.First(student => student.Id == info.studentId) as object, info.error));
            return (deletedStudents.ToList(), deletedCount, new List<(object, IEnumerable<string>)>(invalidStudentsInfo));
        };
    }
}
