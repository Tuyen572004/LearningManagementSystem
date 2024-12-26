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
    partial class TeachersInClassViewModel(IDao dao, int classId) : ReaderViewModel(dao)
    {
        private readonly int _classId = classId;
        protected override (ObservableCollection<object> fetchedItem, int queryCount) GetItems(
            int ignoreCount = 0,
            int fetchCount = 0,
            List<SortCriteria> sortCriteria = null,
            SearchCriteria searchCriteria = null)
        {
            var (resultList, queryCount) = _dao.GetTeachersFromClass(
                classId: _classId,
                ignoringCount: ignoreCount,
                fetchingCount: fetchCount,
                sortCriteria: sortCriteria
            );

            return (new(resultList), queryCount);
        }

        public override IEnumerable<string> ColumnOrder => ["Id", "UserId", "TeacherCode", "TeacherName", "Email", "PhoneNo"];
        public override IEnumerable<string> IgnoringColumns => ["IsValid"];
        public override IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [];
    }
}
