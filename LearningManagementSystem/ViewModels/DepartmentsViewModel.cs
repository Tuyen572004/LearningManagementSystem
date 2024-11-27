using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.EModels;
using System;

namespace LearningManagementSystem.ViewModels
{
    public class DepartmentsViewModel : PropertyChangedClass
    {
        public string Keyword { get; set; } = "";
        public bool NameAscending { get; set; } = false;
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 0;
        public int TotalItems { get; set; } = 0;

        private IDao _dao = null;


        public FullObservableCollection<Department> Departments { get; set; }

        public DepartmentsViewModel()
        {
            _dao = new ESqlDao();
            GetAllDepartments();
        }

        // Find department Id by YY with input is YY - XXXXX
        public int CountDepartments()
        {
            return _dao.CountDepartments();
        }
        public int FindDepartmentID(string input)
        {
            var token=input.Split(" - ");

            return _dao.FindDepartmentID(new Department { DepartmentCode = token[0] });

        }
        public void GetAllDepartments()
        {
            var (totalItems, departments) = _dao.GetAllDepartments(CurrentPage, PageSize, Keyword, NameAscending);
            if (totalItems >= 0)
            {
                Departments = new FullObservableCollection<Department>(departments);
                TotalItems = totalItems;
                TotalPages = (TotalItems / PageSize) + ((TotalItems % PageSize == 0) ? 0 : 1);
            }
        }
    
    }
}
