using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using Microsoft.UI.Xaml.Data;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for managing students in a class.
    /// </summary>
    /// <param name="dao">Data access object for interacting with the data source.</param>
    /// <param name="classId">The ID of the class.</param>
    partial class StudentsInClassViewModel(IDao dao, int classId) : ReaderViewModel(dao)
    {
        private readonly int _classId = classId;

        /// <summary>
        /// Fetches students from the specified class based on the given parameters.
        /// </summary>
        /// <param name="ignoreCount">Number of items to skip.</param>
        /// <param name="fetchCount">Number of items to fetch.</param>
        /// <param name="sortCriteria">Criteria for sorting the items.</param>
        /// <param name="searchCriteria">Criteria for searching the items.</param>
        /// <returns>A tuple containing the fetched items and the total query count.</returns>
        protected override (ObservableCollection<object> fetchedItem, int queryCount) GetItems(
            int ignoreCount = 0,
            int fetchCount = 0,
            List<SortCriteria> sortCriteria = null,
            SearchCriteria searchCriteria = null)
        {
            var (resultList, queryCount) = _dao.GetStudentsFromClass(
                classId: _classId,
                ignoringCount: ignoreCount,
                fetchingCount: fetchCount,
                sortCriteria: sortCriteria
            );

            return (new(resultList), queryCount);
        }

        /// <summary>
        /// Gets the order of the columns.
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
    }
}
