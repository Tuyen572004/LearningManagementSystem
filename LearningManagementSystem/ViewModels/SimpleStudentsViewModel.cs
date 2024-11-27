#nullable enable
using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LearningManagementSystem.Helpers;

namespace LearningManagementSystem.ViewModels
{
    public partial class SimpleStudentsViewModel(IDao dao) : PropertyChangedClass, IStudentProvider
    {
        public static readonly int DEFAULT_ROWS_PER_PAGE = 10;
        private readonly IDao _dao = dao;
        public int CurrentPage { get; internal set; } = 1;
        private int _rowsPerPage = DEFAULT_ROWS_PER_PAGE;

        public int RowsPerPage
        {
            get => _rowsPerPage;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Rows per page must be greater than 0");
                }
                _rowsPerPage = value;
            }
        }
        public int ItemCount { get; private set; } = 0;
        public int PageCount { get => ItemCount / RowsPerPage + ((ItemCount % RowsPerPage > 0) ? 1 : 0); }
        public ObservableCollection<StudentVer2> ManagingStudents { get; private set; } = [];
        public List<int>? ManagingIds { get; private set; } = null;
        public SimpleStudentsViewModel NavigateToPage(int pageNumber)
        {
            if (pageNumber < 1 || pageNumber > PageCount)
            {
                return this;
                // throw new ArgumentException("Invalid page number");
            }
            CurrentPage = pageNumber;
            GetStudents(ManagingIds);
            return this;
        }
        public void GetStudents(IEnumerable<int>? ids = null)
        {
            ManagingIds = ids?.ToList();
            var (resultList, queryCount) = _dao.GetStudentsById(
                ignoringCount: (CurrentPage - 1) * RowsPerPage,
                fetchingCount: RowsPerPage,
                chosenIds: ids
                );
            ItemCount = queryCount;
            ManagingStudents = resultList;

            // You must "roar" by yourself :'))
            RaisePropertyChanged(nameof(ManagingStudents));
        }

    }
}
