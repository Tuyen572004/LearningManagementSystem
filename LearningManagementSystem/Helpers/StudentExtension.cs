#nullable enable
using LearningManagementSystem.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    public partial class StudentVer2 : ICloneable, INotifyDataErrorInfo, INotifyPropertyChanged, IValidatableItem
    {
        public static readonly int MAX_STUDENT_CODE_LENGTH = 10;
        public static readonly int MAX_STUDENT_NAME_LENGTH = 100;
        public static readonly int MAX_EMAIL_LENGTH = 100;
        public static readonly int MAX_PHONE_NO_LENGTH = 30;

        private static readonly List<string> propertyNames = typeof(StudentVer2).GetProperties().Select(p => p.Name).ToList();
        private readonly Dictionary<string, List<string>> _errors = [];
        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        private void OnErrorsChanged(string propertyName)
        {
            if (!propertyNames.Contains(propertyName))
            {
                return;
            }
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return _errors.Values.SelectMany(sublist => sublist).ToList();
            }
            return _errors.GetValueOrDefault(propertyName) ?? [];
        }
        public void ValidateProperty(string? propertyName)
        {
            if (propertyName == null)
            {
                return;
            }
            _errors.Remove(propertyName);

            switch (propertyName)
            {
                case nameof(Id):
                    break;
                case nameof(StudentCode):
                    if (StudentCode.Length == 0)
                    {
                        _errors.Add(propertyName, ["Student code must not be empty."]);
                    }
                    if (StudentCode.Length > MAX_STUDENT_CODE_LENGTH)
                    {
                        _errors.Add(propertyName, [$"Student code must be less than {MAX_STUDENT_CODE_LENGTH} characters."]);
                    }
                    break;
                case nameof(StudentName):
                    if (StudentName.Length == 0)
                    {
                        _errors.Add(propertyName, ["Student name must not be empty."]);
                    }
                    if (StudentName.Length > MAX_STUDENT_NAME_LENGTH)
                    {
                        _errors.Add(propertyName, [$"Student name must be less than {MAX_STUDENT_NAME_LENGTH} characters."]);
                    }
                    break;
                case nameof(Email):
                    if (Email.Length == 0)
                    {
                        _errors.Add(propertyName, ["Email must not be empty."]);
                    }
                    else
                    {
                        if (!EmailRegex().IsMatch(Email))
                        {
                            _errors.Add(propertyName, ["Email is not in a valid format."]);
                        }
                    }
                    if (Email.Length > MAX_EMAIL_LENGTH)
                    {
                        _errors.Add(propertyName, [$"Email must be less than {MAX_EMAIL_LENGTH} characters."]);
                    }
                    break;
                case nameof(BirthDate):
                    if (BirthDate > DateTime.Now)
                    {
                        _errors.Add(propertyName, ["Birth date must be less than current date."]);
                    }
                    break;
                case nameof(PhoneNo):
                    if (PhoneNo.Length == 0)
                    {
                        _errors.Add(propertyName, ["Phone number must not be empty."]);
                    }
                    else
                    {
                        if (!PhoneRegex().IsMatch(PhoneNo))
                        {
                            _errors.Add(propertyName, ["Phone number is not in a valid format."]);
                        }
                    }
                    if (PhoneNo.Length > MAX_PHONE_NO_LENGTH)
                    {
                        _errors.Add(propertyName, [$"Phone number must be less than {MAX_PHONE_NO_LENGTH} characters."]);
                    }
                    break;
                case nameof(UserId):
                    break;
                case nameof(EnrollmentYear):
                    if (EnrollmentYear < 1900 || EnrollmentYear > DateTime.Now.Year)
                    {
                        _errors.Add(propertyName, ["Enrollment year must be between 1900 and current year."]);
                    }
                    break;
                case nameof(GraduationYear):
                    if (GraduationYear != null && (GraduationYear < 1900 || GraduationYear > DateTime.Now.Year))
                    {
                        _errors.Add(propertyName, ["Graduation year must be between 1900 and current year."]);
                    }
                    if (GraduationYear != null && GraduationYear < EnrollmentYear)
                    {
                        _errors.Add(propertyName, ["Graduation year must be greater than or equal to enrollment year."]);
                    }
                    break;
                default:
                    break;
            }
            OnErrorsChanged(propertyName);
        }
        public void RevalidateAllProperties()
        {
            _errors.Clear();
            // ValidateProperty(nameof(Id));
            ValidateProperty(nameof(StudentCode));
            ValidateProperty(nameof(StudentName));
            ValidateProperty(nameof(Email));
            ValidateProperty(nameof(BirthDate));
            ValidateProperty(nameof(PhoneNo));
            // ValidateProperty(nameof(UserId));
            ValidateProperty(nameof(EnrollmentYear));
            ValidateProperty(nameof(GraduationYear));
        }
        public void ChangeErrors(string propertyName, List<string> errors)
        {
            _errors[propertyName] = errors;
            OnErrorsChanged(propertyName);
        }


        public object Clone()
        {
            return new StudentVer2
            {
                Id = this.Id,
                StudentCode = this.StudentCode,
                StudentName = this.StudentName,
                Email = this.Email,
                BirthDate = this.BirthDate,
                PhoneNo = this.PhoneNo,
                UserId = this.UserId,
                EnrollmentYear = this.EnrollmentYear,
                GraduationYear = this.GraduationYear
            };
        }

        public StudentVer2 Copy(StudentVer2 other)
        {
            StudentCode = other.StudentCode;
            StudentName = other.StudentName;
            Email = other.Email;
            BirthDate = other.BirthDate;
            PhoneNo = other.PhoneNo;
            UserId = other.UserId;
            EnrollmentYear = other.EnrollmentYear;
            GraduationYear = other.GraduationYear;
            return this;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            if (obj is StudentVer2 other)
            {
                return 
                    StudentCode == other.StudentCode &&
                    StudentName == other.StudentName &&
                    Email == other.Email &&
                    BirthDate == other.BirthDate &&
                    PhoneNo == other.PhoneNo &&
                    UserId == other.UserId &&
                    EnrollmentYear == other.EnrollmentYear &&
                    GraduationYear == other.GraduationYear;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StudentCode,
                StudentName,
                Email,
                BirthDate,
                PhoneNo,
                UserId,
                EnrollmentYear,
                GraduationYear
            );
        }

        static public StudentVer2 Empty()
        {
            return new()
            {
                Id = -1,
                StudentCode = "",
                StudentName = "",
                Email = "",
                BirthDate = DateTime.Now,
                PhoneNo = "",
                UserId = null,
                EnrollmentYear = DateTime.Now.Year,
                GraduationYear = null
            };
        }

        //static public (bool result, IEnumerable<String> errors) FieldsCheck(StudentVer2 student)
        //{
        //    List<String> errors = [];
        //    if (student.StudentCode.Length == 0)
        //    {
        //        errors.Add("Student code must not be empty.");
        //    }
        //    if (student.StudentName.Length == 0)
        //    {
        //        errors.Add("Student name must not be empty.");
        //    }
        //    if (student.Email.Length == 0)
        //    {
        //        errors.Add("Email must not be empty.");
        //    }
        //    else
        //    {
        //        if (!EmailRegex().IsMatch(student.Email))
        //        {
        //            errors.Add("Email is not in a valid format.");
        //        }
        //    }

        //    if (student.PhoneNo.Length == 0)
        //    {
        //        errors.Add("Phone number must not be empty.");
        //    }
        //    else
        //    {
        //        if (!PhoneRegex().IsMatch(student.PhoneNo))
        //        {
        //            errors.Add("Phone number is not in a valid format.");
        //        }
        //    }

        //    if (student.StudentCode.Length > StudentVer2.MAX_STUDENT_CODE_LENGTH)
        //    {
        //        errors.Add($"Student code must be less than {StudentVer2.MAX_STUDENT_CODE_LENGTH} characters.");
        //    }
        //    if (student.StudentName.Length > StudentVer2.MAX_STUDENT_NAME_LENGTH)
        //    {
        //        errors.Add($"Student name must be less than {StudentVer2.MAX_STUDENT_NAME_LENGTH} characters.");
        //    }
        //    if (student.Email.Length > StudentVer2.MAX_EMAIL_LENGTH)
        //    {
        //        errors.Add($"Email must be less than {StudentVer2.MAX_EMAIL_LENGTH} characters.");
        //    }
        //    if (student.PhoneNo.Length > StudentVer2.MAX_PHONE_NO_LENGTH)
        //    {
        //        errors.Add($"Phone number must be less than {StudentVer2.MAX_PHONE_NO_LENGTH} characters.");
        //    }

        //    if (student.EnrollmentYear < 1900 || student.EnrollmentYear > DateTime.Now.Year)
        //    {
        //        errors.Add("Enrollment year must be between 1900 and current year.");
        //    }
        //    if (student.GraduationYear != null && (student.GraduationYear < 1900 || student.GraduationYear > DateTime.Now.Year))
        //    {
        //        errors.Add("Graduation year must be between 1900 and current year.");
        //    }
        //    return (errors.Count == 0, errors);
        //}

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();

        [GeneratedRegex(@"^\+?\d{0,3}[-.\s]?\(?\d+\)?[-.\s]?\d+([-.\s]?\d+)*$")]
        private static partial Regex PhoneRegex();
    }
}
