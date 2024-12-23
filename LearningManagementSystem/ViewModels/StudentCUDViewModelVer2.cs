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
    partial class StudentCUDViewModelVer2(IDao dao) : CUDViewModel(dao)
    {
        public override IEnumerable<string> ColumnOrder => ["Id", "UserId", "IsValid", "StudentCode", "StudentName", "Email", "BirthDate", "PhoneNo"];
        public override IEnumerable<string> IgnoringColumns => [];
        public override IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [
            ("Id", new NegativeIntToNewMarkerConverter()),
            ("GraduationYear", new NullableIntToStringConverter()),
            ("BirthDate", new DateTimeToStringConverter()),
            ];
        public override IEnumerable<string> ReadOnlyColumns => ["Id", "UserId", "IsValid"];

        // Required override from CUDViewModel
        public override InfoBarMessage? GetMessageOf(object item)
        {
            if (item is StudentVer2 student)
            {
                if ((student as INotifyDataErrorInfoExtended).HasErrors)
                {
                    var errors = student.GetErrors(null) as List<String> ?? [];
                    var newMessage = String.Join("\n", errors);
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

        // Functional overrides
        public override GroupValidator? TransferingItemFilter => (newItem, existingItems) => {
            var existingStudents = existingItems.OfType<StudentVer2>();
            var newStudent = newItem as StudentVer2;
            var existingStudent = existingStudents.FirstOrDefault(s => s.Id == newStudent?.Id);
            if (existingStudent != null)
            {
                return false;
            }
            return true;
        };

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

        public override ItemTransformer? ExistingItemTransformerOnEditting => (object newItem, ref object existingItem) =>
        {
            (existingItem as StudentVer2)?.Copy(newItem as StudentVer2);
        };

        public override object? EmptyItem() => StudentVer2.Empty();

        public override ItemChecker? ItemCheckForAdd => (object item) => item is StudentVer2 student && student.Id < 0;

        public override ItemChecker? ItemCheckForUpdate => (object item) => item is StudentVer2 student && student.Id >= 0;

        public override ItemDaoModifier? ItemDaoInserter => (IEnumerable<object> items) =>
        {
            var (addedStudents, addedCount, invalidStudentInfo) = _dao.AddStudents(items.OfType<StudentVer2>());
            return (
                addedStudents.OfType<object>().ToList(),
                addedCount,
                new List<(object, IEnumerable<string>)>(invalidStudentInfo.Select(info => (info.student as object, info.error)))
            );
        };

        public override ItemDaoModifier? ItemDaoUpdater => (IEnumerable<object> items) =>
        {
            var (updatedStudents, updatedCount, invalidStudentInfo) = _dao.UpdateStudents(items.OfType<StudentVer2>());
            return (
                updatedStudents.OfType<object>().ToList(),
                updatedCount,
                new List<(object, IEnumerable<string>)>(invalidStudentInfo.Select(info => (info.student as object, info.error)))
            );
        };

        public override ItemDaoModifier? ItemDaoDeleter => (IEnumerable<object> items) =>
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
