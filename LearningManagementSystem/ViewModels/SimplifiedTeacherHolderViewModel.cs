using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
#nullable enable
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for managing a simplified list of teachers.
    /// </summary>
    /// <param name="dao">Data access object for interacting with the database.</param>
    partial class SimplifiedTeacherHolderViewModel(IDao dao) : CUDViewModel(dao)
    {

        /// <summary>
        /// Gets the order of columns to be displayed.
        /// </summary>
        public override IEnumerable<string> ColumnOrder => ["TeacherCode", "TeacherName"];

        /// <summary>
        /// Gets the columns to be ignored.
        /// </summary>
        public override IEnumerable<string> IgnoringColumns => ["Id", "UserId", "IsValid", "Email", "PhoneNo"];

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
        /// Populates the list of teachers.
        /// </summary>
        /// <param name="objects">The collection of objects to populate as teachers.</param>
        public void PopulateTeachers(IEnumerable<object> objects)
        {
            PopulateItems(objects.OfType<Teacher>());
        }
    }
}
