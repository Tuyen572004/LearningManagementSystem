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
    public partial class Teacher : ICloneable, INotifyDataErrorInfoExtended, INotifyPropertyChanged, ITemporaryUserHolder
    {
        public static readonly int MAX_TEACHER_CODE_LENGTH = 10;
        public static readonly int MAX_TEACHER_NAME_LENGTH = 100;
        public static readonly int MAX_EMAIL_LENGTH = 100;
        public static readonly int MAX_PHONE_NO_LENGTH = 100;

        private readonly List<string> _needErrorChecked =
            INotifyDataErrorInfoExtended.GetPropertiesOf<Teacher>()
            .Except(["Id", "UserId", "IsValid"])
            .ToList();
        List<string> INotifyDataErrorInfoExtended.PropertyNames => _needErrorChecked;

        private readonly Dictionary<string, List<string>> _errors = [];
        Dictionary<string, List<string>> INotifyDataErrorInfoExtended.RawErrors => _errors;
        public bool IsValid => !(this as INotifyDataErrorInfoExtended).HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        void INotifyDataErrorInfoExtended.OnErrorsChanged(string propertyName)
        {
            //if (propertyName == INotifyDataErrorInfoExtended.NoErrorPropertyName)
            //{
            //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
            //}
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

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

        private User? _holdingUser = null;
        User? ITemporaryUserHolder.HoldingUser { get => _holdingUser; set => _holdingUser = value; }


        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();

        [GeneratedRegex(@"^\+?\d{0,3}[-.\s]?\(?\d+\)?[-.\s]?\d+([-.\s]?\d+)*$")]
        private static partial Regex PhoneRegex();
    }
}
