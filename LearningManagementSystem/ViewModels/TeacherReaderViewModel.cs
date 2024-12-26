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
    partial class TeacherReaderViewModel(IDao dao): ReaderViewModel(dao)
    {
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

        public override IEnumerable<string> IgnoringColumns => ["IsValid"];
        public override IEnumerable<string> ColumnOrder => ["Id", "UserId", "TeacherCode", "TeacherName", "Email", "PhoneNo"];
        public override IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [];

        public override List<string> SearchFields => [
            "Id", "UserId", "TeacherCode", "TeacherName", "Email", "PhoneNo"
            ];
    }
}
