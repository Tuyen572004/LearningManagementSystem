#nullable enable
using ICSharpCode.SharpZipLib.Core;
using LearningManagementSystem.Controls;
using LearningManagementSystem.ViewModels;
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
    public partial class StudentVer2 : ICloneable, INotifyDataErrorInfoExtended, INotifyPropertyChanged, ITemporaryUserHolder
    {
        /// <summary>
        /// Maximum length for the student code.
        /// </summary>
        public static readonly int MAX_STUDENT_CODE_LENGTH = 10;

        /// <summary>
        /// Maximum length for the student name.
        /// </summary>
        public static readonly int MAX_STUDENT_NAME_LENGTH = 100;

        /// <summary>
        /// Maximum length for the email address.
        /// </summary>
        public static readonly int MAX_EMAIL_LENGTH = 100;

        /// <summary>
        /// Maximum length for the phone number.
        /// </summary>
        public static readonly int MAX_PHONE_NO_LENGTH = 30;

        /// <summary>
        /// List of property names that need error checking.
        /// </summary>
        private readonly List<string> _needErrorChecked =
            INotifyDataErrorInfoExtended.GetPropertiesOf<StudentVer2>()
            .Except(["Id", "UserId", "IsValid"])
            .ToList();

        /// <summary>
        /// Gets the list of property names that need error checking.
        /// </summary>
        List<string> INotifyDataErrorInfoExtended.PropertyNames => _needErrorChecked;

        /// <summary>
        /// Dictionary to store validation errors for each property.
        /// </summary>
        private readonly Dictionary<string, List<string>> _errors = [];

        /// <summary>
        /// Gets the dictionary of raw validation errors.
        /// </summary>
        Dictionary<string, List<string>> INotifyDataErrorInfoExtended.RawErrors => _errors;

        /// <summary>
        /// Gets a value indicating whether the student is valid (has no validation errors).
        /// </summary>
        public bool IsValid => !(this as INotifyDataErrorInfoExtended).HasErrors;

        /// <summary>
        /// Occurs when the validation errors for a property have changed.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Raises the ErrorsChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that has validation errors.</param>
        void INotifyDataErrorInfoExtended.OnErrorsChanged(string propertyName)
        {
            if (propertyName == INotifyDataErrorInfoExtended.NoErrorPropertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
            }
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the ErrorsChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that has validation errors.</param>
        public void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets the validation errors for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property to get validation errors for.</param>
        /// <returns>A collection of validation errors for the specified property.</returns>
        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return _errors.Values.SelectMany(sublist => sublist).ToList();
            }
            return _errors.GetValueOrDefault(propertyName) ?? [];
        }

        /// <summary>
        /// Gets the validation errors for a single property.
        /// </summary>
        /// <param name="propertyName">The name of the property to get validation errors for.</param>
        /// <returns>A collection of validation errors for the specified property.</returns>
        public IEnumerable<string> GetErrorsOfSingleProperty(string propertyName)
        {
            List<string> currentErrors = [];
            switch (propertyName)
            {
                case nameof(Id):
                    break;
                case nameof(StudentCode):
                    if (StudentCode.Length == 0)
                    {
                        currentErrors.Add("Student code must not be empty.");
                    }
                    else if (StudentCode.Length > MAX_STUDENT_CODE_LENGTH)
                    {
                        currentErrors.Add($"Student code must be less than {MAX_STUDENT_CODE_LENGTH} characters.");
                    }
                    break;
                case nameof(StudentName):
                    if (StudentName.Length == 0)
                    {
                        currentErrors.Add("Student name must not be empty.");
                    }
                    else if (StudentName.Length > MAX_STUDENT_NAME_LENGTH)
                    {
                        currentErrors.Add($"Student name must be less than {MAX_STUDENT_NAME_LENGTH} characters.");
                    }
                    break;
                case nameof(Email):
                    if (Email.Length == 0)
                    {
                        currentErrors.Add("Email must not be empty.");
                    }
                    else
                    {
                        if (!EmailRegex().IsMatch(Email))
                        {
                            currentErrors.Add("Email is not in a valid format.");
                        }
                    }
                    if (Email.Length > MAX_EMAIL_LENGTH)
                    {
                        currentErrors.Add($"Email must be less than {MAX_EMAIL_LENGTH} characters.");
                    }
                    break;
                case nameof(BirthDate):
                    if (BirthDate > DateTime.Now)
                    {
                        currentErrors.Add("Birth date must be less than current date.");
                    }
                    break;
                case nameof(PhoneNo):
                    if (PhoneNo.Length == 0)
                    {
                        currentErrors.Add("Phone number must not be empty.");
                    }
                    else
                    {
                        if (!PhoneRegex().IsMatch(PhoneNo))
                        {
                            currentErrors.Add("Phone number is not in a valid format.");
                        }
                    }
                    if (PhoneNo.Length > MAX_PHONE_NO_LENGTH)
                    {
                        currentErrors.Add($"Phone number must be less than {MAX_PHONE_NO_LENGTH} characters.");
                    }
                    break;
                case nameof(UserId):
                    break;
                case nameof(EnrollmentYear):
                    if (EnrollmentYear < 1900 || EnrollmentYear > DateTime.Now.Year)
                    {
                        currentErrors.Add("Enrollment year must be between 1900 and current year.");
                    }
                    break;
                case nameof(GraduationYear):
                    if (GraduationYear != null && (GraduationYear < 1900 || GraduationYear > DateTime.Now.Year))
                    {
                        currentErrors.Add("Graduation year must be between 1900 and current year.");
                    }
                    if (GraduationYear != null && GraduationYear < EnrollmentYear)
                    {
                        currentErrors.Add("Graduation year must be greater than or equal to enrollment year.");
                    }
                    break;
                default:
                    break;
            }
            return currentErrors;
        }

        /// <summary>
        /// Validates a single property and updates the validation errors.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
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

        /// <summary>
        /// Revalidates all properties and updates the validation errors.
        /// </summary>
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
        /// <summary>
        /// Changes the validation errors for a specific property and raises the ErrorsChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property to change errors for.</param>
        /// <param name="errors">The list of errors to set for the property.</param>
        public void ChangeErrors(string propertyName, List<string> errors)
        {
            _errors[propertyName] = errors;
            OnErrorsChanged(propertyName);
        }


        /// <summary>
        /// Creates a new instance of the <see cref="StudentVer2"/> class that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="StudentVer2"/> object that is a copy of this instance.</returns>
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

        /// <summary>
        /// Copies the values from another <see cref="StudentVer2"/> instance to this instance.
        /// </summary>
        /// <param name="other">The <see cref="StudentVer2"/> instance to copy values from.</param>
        /// <returns>This <see cref="StudentVer2"/> instance with copied values.</returns>
        public StudentVer2 Copy(StudentVer2? other)
        {
            if (other == null)
            {
                return this;
            }
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

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
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

        /// <summary>
        /// Creates an empty <see cref="StudentVer2"/> instance with default values.
        /// </summary>
        /// <returns>An empty <see cref="StudentVer2"/> instance.</returns>
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

        /// <summary>
        /// Gets or sets the user holding the temporary user information.
        /// </summary>
        private User? _holdingUser = null;
        /// <summary>
        /// Gets or sets the user holding the temporary user information.
        /// </summary>
        User? ITemporaryUserHolder.HoldingUser { get => _holdingUser; set => _holdingUser = value; }

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
