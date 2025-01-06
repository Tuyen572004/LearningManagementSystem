#nullable enable
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    /// <summary>
    /// Represents a teacher in the learning management system.
    /// </summary>
    public partial class Teacher : ICloneable, INotifyDataErrorInfoExtended, INotifyPropertyChanged, ITemporaryUserHolder
    {
        /// <summary>
        /// Maximum length for the teacher code.
        /// </summary>
        public static readonly int MAX_TEACHER_CODE_LENGTH = 10;

        /// <summary>
        /// Maximum length for the teacher name.
        /// </summary>
        public static readonly int MAX_TEACHER_NAME_LENGTH = 100;

        /// <summary>
        /// Maximum length for the email address.
        /// </summary>
        public static readonly int MAX_EMAIL_LENGTH = 100;

        /// <summary>
        /// Maximum length for the phone number.
        /// </summary>
        public static readonly int MAX_PHONE_NO_LENGTH = 100;

        /// <summary>
        /// List of property names that need error checking.
        /// </summary>
        private readonly List<string> _needErrorChecked =
            INotifyDataErrorInfoExtended.GetPropertiesOf<Teacher>()
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
        /// Gets a value indicating whether the teacher instance is valid.
        /// </summary>
        public bool IsValid => !(this as INotifyDataErrorInfoExtended).HasErrors;

        /// <summary>
        /// Event that is triggered when validation errors change.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Triggers the ErrorsChanged event for a specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property for which errors have changed.</param>
        void INotifyDataErrorInfoExtended.OnErrorsChanged(string propertyName)
        {
            //if (propertyName == INotifyDataErrorInfoExtended.NoErrorPropertyName)
            //{
            //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
            //}
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets the validation errors for a specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve errors for.</param>
        /// <returns>A collection of error messages for the specified property.</returns>
        public IEnumerable<string> GetErrorsOfSingleProperty(string propertyName)
        {
            List<string> currentErrors = [];
            switch (propertyName)
            {
                case nameof(Id):
                    break;
                case nameof(TeacherCode):
                    if (string.IsNullOrWhiteSpace(TeacherCode))
                    {
                        currentErrors.Add("Teacher Code is required.");
                    }
                    else if (TeacherCode.Length > MAX_TEACHER_CODE_LENGTH)
                    {
                        currentErrors.Add($"Teacher Code must be less than {MAX_TEACHER_CODE_LENGTH} characters.");
                    }
                    break;
                case nameof(TeacherName):
                    if (string.IsNullOrWhiteSpace(TeacherName))
                    {
                        currentErrors.Add("Teacher Name is required.");
                    }
                    else if (TeacherName.Length > MAX_TEACHER_NAME_LENGTH)
                    {
                        currentErrors.Add($"Teacher Name must be less than {MAX_TEACHER_NAME_LENGTH} characters.");
                    }
                    break;
                case nameof(Email):
                    if (string.IsNullOrWhiteSpace(Email))
                    {
                        currentErrors.Add("Email is required.");
                    }
                    else if (!EmailRegex().IsMatch(Email))
                    {
                        currentErrors.Add("Email is not in the valid format.");
                    }
                    else if (Email.Length > MAX_EMAIL_LENGTH)
                    {
                        currentErrors.Add($"Email must be less than {MAX_EMAIL_LENGTH} characters.");
                    }
                    break;
                case nameof(PhoneNo):
                    if (PhoneNo != null)
                    {
                        if (!PhoneRegex().IsMatch(PhoneNo))
                        {
                            currentErrors.Add("Phone No is not in the valid format.");
                        }
                        else if (PhoneNo.Length > MAX_PHONE_NO_LENGTH)
                        {
                            currentErrors.Add($"Phone No must be less than {MAX_PHONE_NO_LENGTH} characters.");
                        }
                    }
                    break;
                default:
                    break;
            }
            return currentErrors;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new Teacher
            {
                Id = Id,
                TeacherCode = TeacherCode,
                TeacherName = TeacherName,
                Email = Email,
                PhoneNo = PhoneNo,
                UserId = UserId
            };
        }

        /// <summary>
        /// Copies the properties of another teacher instance to this instance.
        /// </summary>
        /// <param name="other">The other teacher instance to copy properties from.</param>
        /// <returns>This instance with copied properties.</returns>
        public Teacher Copy(Teacher? other)
        {
            if (other == null)
            {
                return this;
            }
            TeacherCode = other.TeacherCode;
            TeacherName = other.TeacherName;
            Email = other.Email;
            PhoneNo = other.PhoneNo;
            UserId = other.UserId;
            return this;
        }

        /// <summary>
        /// Creates an empty teacher instance with default values.
        /// </summary>
        /// <returns>An empty teacher instance.</returns>
        public static Teacher Empty()
        {
            return new()
            {
                Id = -1,
                TeacherCode = "",
                TeacherName = "",
                Email = "",
                PhoneNo = null,
                UserId = null
            };
        }

        /// <summary>
        /// Holds the user temporarily.
        /// </summary>
        private User? _holdingUser = null;
        /// <summary>
        /// Gets or sets the user that is being held temporarily.
        /// </summary>
        User? ITemporaryUserHolder.HoldingUser
        {
            get => _holdingUser;
            set => _holdingUser = value;
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();

        [GeneratedRegex(@"^\+?\d{0,3}[-.\s]?\(?\d+\)?[-.\s]?\d+([-.\s]?\d+)*$")]
        private static partial Regex PhoneRegex();
    }
}
