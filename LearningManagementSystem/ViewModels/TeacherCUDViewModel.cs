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
    partial class TeacherCUDViewModel(IDao dao) : CUDViewModel(dao)
    {
        public override IEnumerable<string> IgnoringColumns => [];
        public override IEnumerable<string> ColumnOrder => ["Id", "UserId", "IsValid", "TeacherCode", "TeacherName", "Email", "PhoneNo"];
        public override IEnumerable<string> ReadOnlyColumns => ["Id", "UserId", "IsValid"];
        public override IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [
            ("Id", new NegativeIntToNewMarkerConverter()),
            ("UserId", new NegativeIntToNewMarkerConverter()),
            ];

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

        public override GroupSelector? OldItemValidationOnEditing => (newItem, existingItems) =>
        {
            var existingTeachers = existingItems.Cast<Teacher>();
            var newTeacher = newItem as Teacher;
            var existingTeacher = existingTeachers.FirstOrDefault(s => s.Id == newTeacher?.Id);
            if (existingTeacher != null)
            {
                return existingTeacher;
            }
            return null;
        };

        public override ItemTransformer? ItemTransformerOnEditting => base.ItemTransformerOnEditting;

        public override ItemTransformer? ExistingItemTransformerOnEditting => (object newItem, ref object existingItem) =>
        {
            (existingItem as Teacher)?.Copy(newItem as Teacher);
        };

        public override object? EmptyItem(ref object? identitySeed)
        {
            if (identitySeed is int identitySeedInt)
            {
                identitySeed = identitySeedInt - 1;
            }
            else
            {
                identitySeed = -1;
            }
            var newTeacher = Teacher.Empty();
            newTeacher.Id = (int)identitySeed;
            return newTeacher;
        }

        public override ItemChecker? ItemCheckForAdd => (object item) => item is Teacher teacher && teacher.Id < 0;

        public override ItemChecker? ItemCheckForUpdate => (object item) => item is Teacher teacher && teacher.Id >= 0;

        public override ItemDaoModifier? ItemDaoInserter => (IEnumerable<object> items) =>
        {
            var (addedTeachers, addedCount, invalidTeachersInfo) = _dao.AddTeachers(items.Cast<Teacher>());
            return (
                addedTeachers.Cast<object>().ToList(),
                addedCount,
                new List<(object, IEnumerable<string>)>(invalidTeachersInfo.Select(x => (x.teacher as object, x.errors)))
            );
        };

        public override ItemDaoModifier? ItemDaoUpdater => (IEnumerable<object> items) =>
        {
            var (updatedTeachers, updatedCount, invalidTeachersInfo) = _dao.UpdateTeachers(items.Cast<Teacher>());
            return (
                updatedTeachers.Cast<object>().ToList(),
                updatedCount,
                new List<(object, IEnumerable<string>)>(invalidTeachersInfo.Select(x => (x.teacher as object, x.errors)))
            );
        };

        public override ItemDaoModifier? ItemDaoDeleter => (IEnumerable<object> items) =>
        {
            var (deletedTeachers, deletedCount, invalidTeachersInfo) = _dao.DeleteTeachers(items.Cast<Teacher>());
            return (
                deletedTeachers.Cast<object>().ToList(),
                deletedCount,
                new List<(object, IEnumerable<string>)>(invalidTeachersInfo.Select(x => (x.teacher as object, x.errors)))
            );
        };
    }
}
