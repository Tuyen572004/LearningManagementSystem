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
            int ignoreFetch = 0,
            List<SortCriteria>? sortCriteria = null,
            SearchCriteria? searchCriteria = null
            )
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> IgnoringColumns => base.IgnoringColumns;
        public override IEnumerable<string> ColumnOrder => base.ColumnOrder;
        public override IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => base.ColumnConverters;

        public override List<string> SearchFields => base.SearchFields;
    }
}
