#nullable enable
using Azure.Identity;
using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services.UserService;
using Microsoft.UI.Xaml.Data;
using NPOI.OpenXmlFormats.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    public interface ITemporaryUserHolder
    {
        public static readonly int NewlyHoldingUserId = -69000;
        protected User? HoldingUser { get; set; }

        public void SetHoldingUser(User? newUser) => HoldingUser = newUser;
        // Prevent Automatic Column Generation on this property in DataGrid
        public void GetHoldingUserByReference(ref User? user) => user = HoldingUser; 
    }
    public enum UserAssignmentTarget { Student, Teacher }
    public class UserAssignmentMessage(bool isConfirmed, UserAssignmentTarget target)
    {
        private readonly UserAssignmentTarget _target = target;
        private readonly bool _isConfirm = isConfirmed;
        public bool IsConfirm => _isConfirm;
        public UserAssignmentTarget Target => _target;

    }
    partial class UserAssignmentViewModel(IDao dao, UserService userService): CUDViewModel(dao)
    {
        public static readonly string AssignmentConfimation = "AssignmentConfirm";

        private readonly UserService _userService = userService;
        public override IEnumerable<string> ColumnOrder => ["Username", "Password", "Email"];
        public override IEnumerable<string> IgnoringColumns => ["Id", "Role", "CreatedAt", "PasswordHash"];
        public override IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => base.ColumnConverters;

        public override InfoBarMessage? GetMessageOf(object item) => null;

        public override RowStatus? GetRowStatus(object item) => null;

        static private string ToLowerCaseNoSpaces(string input)
        {
            return input.ToLower().Replace(" ", "");
        }
        public static string GetEmailLocalPart(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return string.Empty;
            }

            var parts = email.Split('@');
            if (parts.Length < 2)
            {
                return string.Empty;
            }

            return parts[0];
        }
        static public string DefaultPassword => "passwordUwU";
        static public readonly string InvalidContent = "INVALID";
        static public readonly string ExistedContent = "EXISTED";
        static private string GeneratePasswordForObject(object obj)
        {
            if (obj is StudentVer2 student)
            {
                return ToLowerCaseNoSpaces(student.StudentCode) + ToLowerCaseNoSpaces(GetEmailLocalPart(student.Email));
            }
            else if (obj is Teacher teacher)
            {
                return ToLowerCaseNoSpaces(teacher.TeacherCode) + ToLowerCaseNoSpaces(GetEmailLocalPart(teacher.Email));
            }
            return DefaultPassword;
        }

        private readonly List<object> _originalObjects = [];
        private readonly Dictionary<int, User> _userMapper = [];
        private readonly HashSet<User> _invalidUsers = [];
        // Note that students and teachers should not both in the same list, as their ids may conflict.
        public void ConvertObjectsToUsersThenPopulate(IEnumerable<object> objects)
        {
            List<object> newUsers = [];
            _userMapper.Clear();
            _invalidUsers.Clear();

            INotifyDataErrorInfoExtended? objectAsErrorNotifier;
            bool isUserNameValid;
            bool isEmailValid;
            foreach (var obj in objects)
            {
                
                //if (obj is INotifyDataErrorInfoExtended errorNotifier)
                //{
                //    errorNotifier.RevalidateAllProperties();
                //    if (errorNotifier.HasErrors)
                //    {
                //        User newUser = new()
                //        {
                //            Username = InvalidContent,
                //            PasswordHash = "",
                //            Password = InvalidContent,
                //            Role = "student",
                //            Email = InvalidContent,
                //        };
                //        newUsers.Add(newUser);
                //        _invalidUsers.Add(newUser);
                //        continue;
                //    }
                //}
                objectAsErrorNotifier = obj as INotifyDataErrorInfoExtended;
                
                if (obj is StudentVer2 student)
                {
                    var password = GeneratePasswordForObject(student);
                    User newUser;
                    if (student.UserId == null || student.UserId == ITemporaryUserHolder.NewlyHoldingUserId)
                    {
                        isUserNameValid = objectAsErrorNotifier is null 
                            || objectAsErrorNotifier.GetErrorsOfSingleProperty("StudentName").ToList().Count == 0;
                        isEmailValid = objectAsErrorNotifier is null
                            || objectAsErrorNotifier.GetErrorsOfSingleProperty("Email").ToList().Count == 0;
                        newUser = new()
                        {
                            Username = isUserNameValid ? student.StudentName : InvalidContent,
                            PasswordHash = _userService.EncryptPassword(password),
                            Password = password,
                            Role = "student",
                            Email = isEmailValid ? student.Email : InvalidContent,
                        };
                        if (!isEmailValid || !isEmailValid)
                        {
                            _invalidUsers.Add(newUser);
                        }
                    }
                    else
                    {
                        newUser = new()
                        {
                            Username = ExistedContent,
                            PasswordHash = "",
                            Password = ExistedContent,
                            Role = "student",
                            Email = ExistedContent,
                        };
                        _invalidUsers.Add(newUser);
                    }
                    newUsers.Add(newUser);
                    _userMapper.Add(student.Id, newUser);
                }
                else if (obj is Teacher teacher)
                {
                    var password = GeneratePasswordForObject(teacher);
                    User newUser;
                    if (teacher.UserId == null || teacher.UserId == ITemporaryUserHolder.NewlyHoldingUserId)
                    {
                        isUserNameValid = objectAsErrorNotifier is null
                            || objectAsErrorNotifier.GetErrorsOfSingleProperty("TeacherName").ToList().Count == 0;
                        isEmailValid = objectAsErrorNotifier is null
                            || objectAsErrorNotifier.GetErrorsOfSingleProperty("Email").ToList().Count == 0;
                        newUser = new()
                        {
                            Username = isUserNameValid ? teacher.TeacherName : InvalidContent,
                            PasswordHash = _userService.EncryptPassword(password),
                            Password = password,
                            Role = "teacher",
                            Email = isEmailValid ? teacher.Email : InvalidContent,
                        };
                        if (!isEmailValid || !isEmailValid)
                        {
                            _invalidUsers.Add(newUser);
                        }
                    }
                    else
                    {
                        newUser = new()
                        {
                            Username = ExistedContent,
                            PasswordHash = "",
                            Password = ExistedContent,
                            Role = "teacher",
                            Email = ExistedContent,
                        };
                        _invalidUsers.Add(newUser);
                    }
                    newUsers.Add(newUser);
                    _userMapper.Add(teacher.Id, newUser);
                }
                else
                {
                    continue;
                }
            }
            _originalObjects.Clear();
            _originalObjects.AddRange(objects);
            PopulateItems(newUsers);
        }

        public void OnConfirmAssignment()
        {
            foreach (var originalObject in _originalObjects)
            {
                if (originalObject is not ITemporaryUserHolder userHolder)
                {
                    continue;
                }
                if (originalObject is StudentVer2 student)
                {
                    _userMapper.TryGetValue(student.Id, out var matchingUser);
                    if (matchingUser is null || _invalidUsers.Contains(matchingUser))
                    {
                        continue;
                    }
                    userHolder.SetHoldingUser(matchingUser);

                    student.UserId ??= ITemporaryUserHolder.NewlyHoldingUserId;
                }
                else if (originalObject is Teacher teacher)
                {
                    _userMapper.TryGetValue(teacher.Id, out var matchingUser);
                    if (matchingUser is null || _invalidUsers.Contains(matchingUser))
                    {
                        continue;
                    }
                    userHolder.SetHoldingUser(matchingUser);

                    teacher.UserId ??= ITemporaryUserHolder.NewlyHoldingUserId;
                }
            }
        }
    }
}
