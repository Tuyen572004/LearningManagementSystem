#nullable enable
using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    public enum Ordering
    {
        Ascending,
        Descending
    }
    public enum StudentField
    {
        Id,
        StudentCode,
        StudentName,
        Email,
        BirthDate,
        PhoneNo,
        UserId,
        EnrolledYear,
        GraduationYear
    }

    public static partial class StudentFieldExtensions
    {
        //public static Type InferType(this StudentField field)
        //{
        //    return field switch
        //    {
        //        StudentField.Id or StudentField.UserId => typeof(int),
        //        StudentField.EnrolledYear or StudentField.GraduationYear => typeof(int),
        //        StudentField.BirthDate => typeof(DateTime),
        //        StudentField.StudentCode or StudentField.StudentName
        //            or StudentField.Email or StudentField.PhoneNo => typeof(string),
        //        _ => throw new ArgumentException("Invalid field passed")
        //    };
        //}

        public static string ToSqlAttributeName(this StudentField field)
        {
            return field switch
            {
                StudentField.Id => "Id",
                StudentField.StudentCode => "StudentCode",
                StudentField.StudentName => "StudentName",
                StudentField.Email => "Email",
                StudentField.BirthDate => "BirthDate",
                StudentField.PhoneNo => "PhoneNo",
                StudentField.UserId => "UserId",
                StudentField.EnrolledYear => "EnrolledYear",
                StudentField.GraduationYear => "GraduationYear",
                _ => throw new ArgumentException("Invalid field passed")
            };
        }

        //public static object? ConstructObject(this StudentField field, params object[] args)
        //{
        //    return Activator.CreateInstance(field.InferType(), args);
        //}

        //public static T? ConstructValueObject<T>(this StudentField field, params object[] args) where T : struct
        //{
        //    var obj = Activator.CreateInstance(field.InferType(), args);
        //    if (obj is null)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return (T)obj;
        //    }
        //}

        //public static T? ConstructReferenceObject<T>(this StudentField field, params object[] args) where T : class
        //{
        //    return Activator.CreateInstance(field.InferType(), args) as T;
        //}


        // Usage: val field = StudentField.StudentCode;
        //        val parsedObject = field.ConstructObject("123456") as field.InferType(); 

        public static object? GetValueByField(this StudentVer2 student, StudentField field)
        {
            return field switch
            {
                StudentField.Id => student.Id,
                StudentField.StudentCode => student.StudentCode,
                StudentField.StudentName => student.StudentName,
                StudentField.Email => student.Email,
                StudentField.BirthDate => student.BirthDate,
                StudentField.PhoneNo => student.PhoneNo,
                StudentField.UserId => student.UserId,
                StudentField.EnrolledYear => student.EnrolledYear,
                StudentField.GraduationYear => student.GraduationYear, // This field is nullable
                _ => throw new ArgumentException("Invalid field passed")
            };
        }
    }

    public class StudentQueryBuilder(StudentsViewModel caller)
    {
        private readonly StudentsViewModel _caller = caller;
        private bool _hasExplicitPageNumber = false;

        public StudentQueryBuilder Reset()
        {
            _caller.CurrentPage = 1;
            _caller.RowsPerPage = StudentsViewModel.DEFAULT_ROWS_PER_PAGE;
            _caller.SortCriteria.Clear();
            _caller.SearchKeyword.Clear();
            _caller.FilterCriteria.Clear();
            return this;
        }
        public StudentQueryBuilder ChangePageNumber(int pageNumber)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentException("Page number must be greater than 0");
            }
            _caller.CurrentPage = pageNumber;
            _hasExplicitPageNumber = true;
            return this;
        }
        public StudentQueryBuilder ChangeRowsPerPage(int rowsPerPage)
        {
            _caller.RowsPerPage = rowsPerPage;
            return this;
        }
        public StudentQueryBuilder ResetSortCriterias()
        {
            _caller.SortCriteria.Clear();
            return this;
        }
        public StudentQueryBuilder ModifySortCriterias(params (StudentField, Ordering)[] criterias)
        {
            foreach (var (field, order) in criterias)
            {
                var matchingFieldIndexInOrigin = _caller.SortCriteria.FindIndex(criteria => criteria.field == field);
                if (matchingFieldIndexInOrigin != -1)
                {
                    _caller.SortCriteria[matchingFieldIndexInOrigin] = (field, order);
                }
                else
                {
                    _caller.SortCriteria.Add((field, order));
                }

            }
            return this;
        }
        public StudentQueryBuilder AddSortCriterias(params (StudentField, Ordering)[] criterias)
        {
            foreach (var (field, order) in criterias)
            {
                var matchingFieldIndexInOrigin = _caller.SortCriteria.FindIndex(criteria => criteria.field == field);
                if (matchingFieldIndexInOrigin != -1)
                {
                    _caller.SortCriteria.RemoveAt(matchingFieldIndexInOrigin);
                }
                _caller.SortCriteria.Add((field, order));
            }
            return this;
        }
        public StudentQueryBuilder ResetSearchKeywords()
        {
            _caller.SearchKeyword.Clear();
            return this;
        }
        public StudentQueryBuilder AddSearchKeywords(params (StudentField, object keyword)[] keywords)
        {
            foreach (var (field, keyword) in keywords)
            {
                _caller.SearchKeyword.Add((field, keyword));
            }
            return this;
        }
        public StudentQueryBuilder ModifySearchKeywords(params (StudentField, object keyword)[] keywords)
        {
            List<StudentField> modifiedFields = [];
            foreach (var (field, keyword) in keywords)
            {
                if (!modifiedFields.Contains(field))
                {
                    var matchingFieldIndexInOrigin = _caller.SearchKeyword.FindIndex(criteria => criteria.field == field);
                    if (matchingFieldIndexInOrigin != -1)
                    {
                        _caller.SearchKeyword[matchingFieldIndexInOrigin] = (field, keyword);
                    }
                    modifiedFields.Add(field);
                }

                _caller.SearchKeyword.Add((field, keyword));
            }
            return this;
        }
        public StudentQueryBuilder ResetFilterCriterias()
        {
            _caller.FilterCriteria.Clear();
            return this;
        }
        public StudentQueryBuilder AddFilterCriteria<T>(
            StudentField field,
            T? leftBound,
            T? rightBound,
            bool containedLeftBound = true,
            bool withinBound = true,
            bool containedRightBound = false
            ) where T : IComparable
        {
            if (leftBound != null && rightBound != null && leftBound.CompareTo(rightBound) > 0)
            {
                throw new ArgumentException("LeftBound object can't be greater than the RightBound");
            }

            _caller.FilterCriteria.Add(
                (field, leftBound, rightBound, containedLeftBound, withinBound, containedRightBound)
                );
            return this;
        }
        public StudentQueryBuilder RemoveFilterCriterias(params StudentField[] fields)
        {
            _caller.FilterCriteria.RemoveAll(criteria => fields.Contains(criteria.field));
            return this;
        }


        public void Execute(bool getAll = false)
        {
            if (!_hasExplicitPageNumber)
            {
                _caller.CurrentPage = 1;
            }
            _caller.GetStudents(getAll);
        }
    }

    public partial class StudentsViewModel(IDao dao) : BaseViewModel, IStudentProvider
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

        // Dictionary only allows one corresponding value for each StudenField
        public List<(StudentField field, Ordering order)> SortCriteria { get; internal set; } = [];
        public List<(StudentField field, object keyword)> SearchKeyword { get; internal set; } = [];
        public List<(
            StudentField field, object? leftBound, object? rightBound,
            bool containedLeftBound, bool withinBounds, bool containedRightBound
            )> FilterCriteria
        { get; internal set; } = [];

        public ObservableCollection<StudentVer2> ManagingStudents { get; private set; } = [];

        public StudentQueryBuilder MakeQuery()
        {
            return new StudentQueryBuilder(this);
        }

        internal void GetStudents(bool getAll = false)
        {
            var (resultList, queryCount) = _dao.GetStudents(
                getAll,
                (CurrentPage - 1) * RowsPerPage,
                RowsPerPage,
                SortCriteria,
                SearchKeyword,
                FilterCriteria
                );
            ItemCount = queryCount;
            ManagingStudents = resultList;

            // You must "roar" by yourself :'))
            RaisePropertyChanged(nameof(ManagingStudents));
        }

        public StudentsViewModel NavigateToPage(int pageNumber)
        {
            if (pageNumber < 1 || pageNumber > PageCount)
            {
                return this;
                // throw new ArgumentException("Invalid page number");
            }
            CurrentPage = pageNumber;
            GetStudents();
            return this;
        }
    }

    public static partial class StudentEnumeratorExtensions
    {
        public static IEnumerable<StudentVer2> ConditionallyFiltered(
            this IEnumerable<StudentVer2> iterable,
            List<(StudentField, object, object, bool, bool, bool)> filterCriterias
            )
        {
            foreach (
                (StudentField field, object leftBound, object rightBound,
                bool containedLeftBound, bool withinBounds, bool containedRightBound)
            in filterCriterias)
            {
                if (leftBound is not null && leftBound as IComparable is null)
                {
                    throw new ArgumentException("LeftBound must be comparable");
                }

                if (rightBound is not null && rightBound as IComparable is null)
                {
                    throw new ArgumentException("RightBound must be comparable");
                }

                iterable = iterable.Where(student => 
                {
                    if (leftBound is null && rightBound is null)
                    {
                        return true;
                    }

                    var comparingValue = student.GetValueByField(field);
                    if (comparingValue is null)
                    {
                        return false;
                    }


                    bool validWithLeftBound = true; // This check if the comparingValue is >[=] leftBound
                    if (leftBound is not null)
                    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        int comparingResult = (leftBound as IComparable).CompareTo(comparingValue);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                        if (
                            comparingResult > 0 || // leftBound > comparingValue
                            comparingResult == 0 && !containedLeftBound
                        )
                        {
                            validWithLeftBound = false;
                        }

                    }

                    bool validWithRightBound = true; // This check if the comparingValue is <[=] rightBound
                    if (rightBound is not null)
                    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        int comparingResult = (rightBound as IComparable).CompareTo(comparingValue);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                        if (
                            comparingResult < 0 || // rightBound < comparingValue
                            comparingResult == 0 && !containedRightBound
                        )
                        {
                            validWithRightBound = false;
                        }
                    }

                    return withinBounds && (validWithLeftBound && validWithRightBound) ||
                        (!withinBounds && (!validWithLeftBound || !validWithRightBound));
                });

            }
            return iterable;
        }
    }
}
