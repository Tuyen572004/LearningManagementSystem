#nullable enable
using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for reading and managing teacher data with support for paging, sorting, and searching.
    /// </summary>
    /// <param name="dao">Data access object for interacting with the data source.</param>
    partial class TeacherReaderViewModel(IDao dao) : ReaderViewModel(dao)
    {
        /// <summary>
        /// Fetches teacher items from the data source based on the specified parameters.
        /// </summary>
        /// <param name="ignoreCount">Number of items to skip.</param>
        /// <param name="fetchCount">Number of items to fetch.</param>
        /// <param name="sortCriteria">Criteria for sorting the items.</param>
        /// <param name="searchCriteria">Criteria for searching the items.</param>
        /// <returns>A tuple containing the fetched items and the total query count.</returns>
        protected override (ObservableCollection<object> fetchedItem, int queryCount) GetItems(
            int ignoreCount = 0,
            int fetchCount = 0,
            List<SortCriteria>? sortCriteria = null,
            SearchCriteria? searchCriteria = null
            )
        {
            var (resultList, queryCount) = _dao.GetTeachers(
                ignoringCount: ignoreCount,
                fetchingCount: fetchCount,
                sortCriteria: sortCriteria,
                searchCriteria: searchCriteria
            );

            return (new(resultList), queryCount);
        }

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
        /// Gets the fields available for searching.
        /// </summary>
        public override List<string> SearchFields => [
            "Id", "UserId", "TeacherCode", "TeacherName", "Email", "PhoneNo"
            ];
    }
}
