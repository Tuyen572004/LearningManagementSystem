#nullable enable
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
        static public string DefaultPassword => "passwordUwU";
        static private string GeneratePasswordForObject(object obj)
        {
            if (obj is StudentVer2 student)
            {
                return ToLowerCaseNoSpaces(student.StudentName) + student.StudentCode;
            }
            else if (obj is Teacher teacher)
            {
                return ToLowerCaseNoSpaces(teacher.TeacherName) + teacher.TeacherCode;
            }
            return DefaultPassword;
        }

        private readonly List<object> _originalObjects = [];
        private readonly Dictionary<int, User> _userMapper = [];
        public void ConvertObjectsToUsersThenPopulate(IEnumerable<object> objects)
        {
            List<object> newUsers = [];
            _userMapper.Clear();
            foreach (var obj in objects)
            {
                if (obj is StudentVer2 student)
                {
                    var password = GeneratePasswordForObject(student);
                    User newUser = new()
                    {
                        Username = student.StudentName,
                        PasswordHash = _userService.EncryptPassword(password),
                        Password = password,
                        Role = "student",
                        Email = student.Email,
                    };
                    newUsers.Add(newUser);
                    _userMapper.Add(student.Id, newUser);
                }
                else if (obj is Teacher teacher)
                {
                    var password = GeneratePasswordForObject(teacher);
                    User newUser = new()
                    {
                        Username = teacher.TeacherName,
                        PasswordHash = _userService.EncryptPassword(password),
                        Password = password,
                        Role = "teacher",
                        Email = teacher.Email,
                    };
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
                    userHolder.SetHoldingUser(matchingUser);

                    student.UserId ??= ITemporaryUserHolder.NewlyHoldingUserId;
                }
                else if (originalObject is Teacher teacher)
                {
                    _userMapper.TryGetValue(teacher.Id, out var matchingUser);
                    userHolder.SetHoldingUser(matchingUser);

                    teacher.UserId ??= ITemporaryUserHolder.NewlyHoldingUserId;
                }
            }
        }
    }
}
