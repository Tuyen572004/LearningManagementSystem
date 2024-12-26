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
        public override IEnumerable<string> IgnoringColumns => ["IsValid"];
        public override IEnumerable<string> ColumnOrder => ["Id", "UserId", "TeacherCode", "TeacherName", "Email", "PhoneNo"];
        public override IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [];


        // Required override from CUDViewModel
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

        // Functional overrides
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

        public override ItemChecker? ItemCheckForAdd => (object item) => item is Teacher teacher && teacher.Id >= 0;

        public override ItemDaoModifier? ItemDaoInserter => (IEnumerable<object> items) =>
        {
            var insertingTeachers = items.Cast<Teacher>();
            var insertingIds = insertingTeachers.Select(teacher => teacher.Id);
            var (addedIds, addedCount, invalidIdsInfo) = _dao.AddTeachersToClass(insertingIds, _classId);
            var addedTeachers = insertingTeachers.Where(teacher => addedIds.Contains(teacher.Id)).Cast<object>();
            var invalidTeachersInfo = invalidIdsInfo.Select(info => (insertingTeachers.First(teacher => teacher.Id == info.teacherId) as object, info.error));

            return (addedTeachers.ToList(), addedCount, new List<(object, IEnumerable<string>)>(invalidTeachersInfo));
        };

        public override ItemDaoModifier? ItemDaoDeleter => (IEnumerable<object> items) =>
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
