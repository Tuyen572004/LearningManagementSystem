using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    partial class StudentReaderViewModelVer2(IDao dao): ReaderViewModel(dao) 
    {
        protected override (ObservableCollection<object> fetchedItem, int queryCount) GetItems(
            int ignoreCount = 0,
            int fetchCount = 0,
            List<SortCriteria> sortCriteria = null,
            SearchCriteria searchCriteria = null)
        {
            var (resultList, queryCount) =  _dao.GetStudents(
                ignoringCount: ignoreCount,
                fetchingCount: fetchCount,
                sortCriteria: sortCriteria,
                searchCriteria: searchCriteria
            );

            return (new(resultList), queryCount);
        }

        public override IEnumerable<string> ColumnOrder => ["Id", "UserId", "StudentCode", "StudentName", "Email", "BirthDate", "PhoneNo"];
        public override IEnumerable<string> IgnoringColumns => ["HasErrors"];
        public override IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [
            ("BirthDate", new DateTimeToStringConverter()),
        ];

        public override List<string> SearchFields => [
                "Id", "UserId", "StudentCode", "StudentName", "Email",
                "BirthDate", "PhoneNo", "EnrollmentYear", "GraduationYear"
            ];
    }
}
