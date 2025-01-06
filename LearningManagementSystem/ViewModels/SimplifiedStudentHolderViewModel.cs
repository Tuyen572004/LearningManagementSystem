#nullable enable
using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for holding and managing simplified student data.
    /// </summary>
    /// <param name="dao">Data access object for database operations.</param>
    partial class SimplifiedStudentHolderViewModel(IDao dao) : CUDViewModel(dao)
    {

        /// <summary>
        /// Gets the order of columns to be displayed.
        /// </summary>
        public override IEnumerable<string> ColumnOrder => ["StudentCode", "StudentName", "Email"];

        /// <summary>
        /// Gets the columns to be ignored during operations.
        /// </summary>
        public override IEnumerable<string> IgnoringColumns => ["Id", "UserId", "IsValid", "PhoneNo", "BirthDate", "GraduationYear", "EnrollmentYear"];

        /// <summary>
        /// Gets the message associated with a specific item.
        /// </summary>
        /// <param name="item">The item to get the message for.</param>
        /// <returns>Returns null as no specific message is associated.</returns>

        public override InfoBarMessage? GetMessageOf(object item) => null;

        /// <summary>
        /// Gets the row status of a specific item.
        /// </summary>
        /// <param name="item">The item to get the row status for.</param>
        /// <returns>Returns null as no specific row status is associated.</returns>
        public override RowStatus? GetRowStatus(object item) => null;

        /// <summary>
        /// Populates the ViewModel with a collection of student objects.
        /// </summary>
        /// <param name="objects">The collection of objects to populate.</param>
        public void PopulateStudents(IEnumerable<object> objects)
        {
            PopulateItems(objects.OfType<StudentVer2>());
        }
    }
}
