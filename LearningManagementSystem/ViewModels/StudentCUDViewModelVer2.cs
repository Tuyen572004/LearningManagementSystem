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
    /// ViewModel for handling Create, Update, and Delete operations for Student entities.
    /// </summary>
    partial class StudentCUDViewModelVer2 : CUDViewModel, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StudentCUDViewModelVer2"/> class.
        /// </summary>
        /// <param name="dao">The data access object for interacting with the database.</param>
        public StudentCUDViewModelVer2(IDao dao) : base(dao)
        {
            WeakReferenceMessenger.Default.Register<UserAssignmentMessage>(this, (r, m) =>
            {
                if (m.Target == UserAssignmentTarget.Student && m.IsConfirm)
                {
                    RefreshManagingItems();
                }
            });
        }

        /// <summary>
        /// Disposes the ViewModel and unregisters all messages.
        /// </summary>
        public void Dispose()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        /// <summary>
        /// Gets the order of columns for display.
        /// </summary>
        public override IEnumerable<string> ColumnOrder => ["Id", "UserId", "IsValid", "StudentCode", "StudentName", "Email", "BirthDate", "PhoneNo"];

        /// <summary>
        /// Gets the columns to be ignored.
        /// </summary>
        public override IEnumerable<string> IgnoringColumns => [];

        /// <summary>
        /// Gets the column converters for specific columns.
        /// </summary>
        public override IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [
            ("Id", new NegativeIntToNewMarkerConverter()),
            ("UserId", new NegativeIntToNewMarkerConverter()),
            ("GraduationYear", new NullableIntToStringConverter()),
            ("BirthDate", new DateTimeToStringConverter()),
            ];

        /// <summary>
        /// Gets the read-only columns.
        /// </summary>
        public override IEnumerable<string> ReadOnlyColumns => ["Id", "UserId", "IsValid"];

        /// <summary>
        /// Gets the message associated with a specific item.
        /// </summary>
        /// <param name="item">The item to get the message for.</param>
        /// <returns>An <see cref="InfoBarMessage"/> if there are errors, otherwise null.</returns>
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
        /// Gets the row status of a specific item.
        /// </summary>
        /// <param name="item">The item to get the row status for.</param>
        /// <returns>A <see cref="RowStatus"/> indicating the status of the row.</returns>
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
        /// Validates if the new item can be transferred to the managing items.
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
        /// Validates if the edited item exists in the managing items.
        /// </summary>
        public override GroupSelector? OldItemValidationOnEditing => (newItem, existingItems) =>
        {
            var existingStudents = existingItems.OfType<StudentVer2>();
            var newStudent = newItem as StudentVer2;
            var existingStudent = existingStudents.FirstOrDefault(s => s.Id == newStudent?.Id);
            if (existingStudent != null)
            {
                return existingStudent;
            }
            return null;
        };

        /// <summary>
        /// Transforms the edited item before updating the managing items.
        /// </summary>
        public override ItemTransformer? ItemTransformerOnEditting => (object oldItem, ref object newItem) =>
        {
            if (newItem is not StudentVer2 newStudent || oldItem is not StudentVer2 oldStudent)
            {
                return;
            }
            if (newStudent.BirthDate.Date == DateTimeToStringConverter.ERROR_DATETIME)
            {
                newStudent.BirthDate = oldStudent.BirthDate;
            }
        };

        /// <summary>
        /// Updates the existing item with the new item.
        /// </summary>
        public override ItemTransformer? ExistingItemTransformerOnEditting => (object newItem, ref object existingItem) =>
        {
            (existingItem as StudentVer2)?.Copy(newItem as StudentVer2);
        };

        /// <summary>
        /// Returns an empty item for adding.
        /// </summary>
        /// <param name="identitySeed">The identity seed for generating unique IDs.
        /// (See <see cref="CUDViewModel.EmptyItem(ref object?)"/> for more information about this parameter) </param>
        /// <returns>An empty <see cref="StudentVer2"/> object.</returns>
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
            var newEmptyStudent = StudentVer2.Empty();
            newEmptyStudent.Id = (int)identitySeed;
            return newEmptyStudent;
        }

        /// <summary>
        /// Checks if the item is valid for adding.
        /// </summary>
        public override ItemChecker? ItemCheckForAdd => (object item) => item is StudentVer2 student && student.Id < 0;

        /// <summary>
        /// Checks if the item is valid for updating.
        /// </summary>
        public override ItemChecker? ItemCheckForUpdate => (object item) => item is StudentVer2 student && student.Id >= 0;

        /// <summary>
        /// Inserts the items into the database.
        /// </summary>
        public override ItemDaoUpdater? ItemDaoInserter => (IEnumerable<object> items) =>
        {
            var (addedStudents, addedCount, invalidStudentInfo) = _dao.AddStudents(items.OfType<StudentVer2>());
            return (
                addedStudents.OfType<object>().ToList(),
                addedCount,
                new List<(object, IEnumerable<string>)>(invalidStudentInfo.Select(info => (info.student as object, info.error)))
            );
        };

        /// <summary>
        /// Updates the items in the database.
        /// </summary>
        public override ItemDaoUpdater? ItemDaoUpdater => (IEnumerable<object> items) =>
        {
            var (updatedStudents, updatedCount, invalidStudentInfo) = _dao.UpdateStudents(items.OfType<StudentVer2>());
            return (
                updatedStudents.OfType<object>().ToList(),
                updatedCount,
                new List<(object, IEnumerable<string>)>(invalidStudentInfo.Select(info => (info.student as object, info.error)))
            );
        };

        /// <summary>
        /// Deletes the items from the database.
        /// </summary>
        public override ItemDaoUpdater? ItemDaoDeleter => (IEnumerable<object> items) =>
        {
            var (deletedStudents, deletedCount, invalidStudentInfo) = _dao.DeleteStudents(items.OfType<StudentVer2>());
            return (
                deletedStudents.OfType<object>().ToList(),
                deletedCount,
                new List<(object, IEnumerable<string>)>(invalidStudentInfo.Select(info => (info.student as object, info.error)))
            );
        };

    }
}
