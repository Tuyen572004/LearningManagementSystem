#nullable enable
using CommunityToolkit.Mvvm.Messaging;
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
    /// <summary>
    /// ViewModel for Create, Update, Delete operations on Teacher entities.
    /// </summary>
    partial class TeacherCUDViewModel : CUDViewModel, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeacherCUDViewModel"/> class.
        /// </summary>
        /// <param name="dao">The data access object.</param>
        public TeacherCUDViewModel(IDao dao) : base(dao)
        {
            WeakReferenceMessenger.Default.Register<UserAssignmentMessage>(this, (r, m) =>
            {
                if (m.Target == UserAssignmentTarget.Teacher && m.IsConfirm)
                {
                    RefreshManagingItems();
                }
            });
        }

        /// <summary>
        /// Disposes the instance and unregisters all messages.
        /// </summary>
        public void Dispose()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        /// <summary>
        /// Gets the columns to be ignored.
        /// </summary>
        public override IEnumerable<string> IgnoringColumns => [];

        /// <summary>
        /// Gets the order of the columns.
        /// </summary>
        public override IEnumerable<string> ColumnOrder => ["Id", "UserId", "IsValid", "TeacherCode", "TeacherName", "Email", "PhoneNo"];

        /// <summary>
        /// Gets the read-only columns.
        /// </summary>
        public override IEnumerable<string> ReadOnlyColumns => ["Id", "UserId", "IsValid"];

        /// <summary>
        /// Gets the column converters.
        /// </summary>
        public override IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [
            ("Id", new NegativeIntToNewMarkerConverter()),
            ("UserId", new NegativeIntToNewMarkerConverter()),
            ("PhoneNo", new NullableStringToStringConverter()),
            ];

        /// <summary>
        /// Gets the message of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The message.</returns>
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
        /// <param name="item">The item.</param>
        /// <returns>The row status.</returns>
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
        /// Gets the validation for old items on editing.
        /// </summary>
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

        /// <summary>
        /// Gets the item transformer on editing.
        /// </summary>
        public override ItemTransformer? ItemTransformerOnEditting => base.ItemTransformerOnEditting;

        /// <summary>
        /// Gets the existing item transformer on editing.
        /// </summary>
        public override ItemTransformer? ExistingItemTransformerOnEditting => (object newItem, ref object existingItem) =>
        {
            (existingItem as Teacher)?.Copy(newItem as Teacher);
        };

        /// <summary>
        /// Gets an empty item.
        /// </summary>
        /// <param name="identitySeed">The identity seed.</param>
        /// <returns>The empty item.</returns>
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

        /// <summary>
        /// Checks if the item can be added.
        /// </summary>
        public override ItemChecker? ItemCheckForAdd => (object item) => item is Teacher teacher && teacher.Id < 0;

        /// <summary>
        /// Checks if the item can be updated.
        /// </summary>
        public override ItemChecker? ItemCheckForUpdate => (object item) => item is Teacher teacher && teacher.Id >= 0;

        /// <summary>
        /// Inserts the specified items.
        /// </summary>
        public override ItemDaoUpdater? ItemDaoInserter => (IEnumerable<object> items) =>
        {
            var (addedTeachers, addedCount, invalidTeachersInfo) = _dao.AddTeachers(items.Cast<Teacher>());
            return (
                addedTeachers.Cast<object>().ToList(),
                addedCount,
                new List<(object, IEnumerable<string>)>(invalidTeachersInfo.Select(x => (x.teacher as object, x.errors)))
            );
        };

        /// <summary>
        /// Updates the specified items.
        /// </summary>
        public override ItemDaoUpdater? ItemDaoUpdater => (IEnumerable<object> items) =>
        {
            var (updatedTeachers, updatedCount, invalidTeachersInfo) = _dao.UpdateTeachers(items.Cast<Teacher>());
            return (
                updatedTeachers.Cast<object>().ToList(),
                updatedCount,
                new List<(object, IEnumerable<string>)>(invalidTeachersInfo.Select(x => (x.teacher as object, x.errors)))
            );
        };

        /// <summary>
        /// Deletes the specified items.
        /// </summary>
        public override ItemDaoUpdater? ItemDaoDeleter => (IEnumerable<object> items) =>
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
