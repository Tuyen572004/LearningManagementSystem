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
    /// <summary>
    /// Interface for holding a temporary user.
    /// </summary>
    public interface ITemporaryUserHolder
    {
        /// <summary>
        /// The ID for a newly holding user.
        /// </summary>
        public static readonly int NewlyHoldingUserId = -69000;

        /// <summary>
        /// Gets or sets the holding user.
        /// </summary>
        protected User? HoldingUser { get; set; }

        /// <summary>
        /// Sets the holding user.
        /// </summary>
        /// <param name="newUser">The new user to hold.</param>
        public void SetHoldingUser(User? newUser) => HoldingUser = newUser;

        /// <summary>
        /// Gets the holding user by reference.
        /// </summary>
        /// <param name="user">The user reference to set.</param>
        public void GetHoldingUserByReference(ref User? user) => user = HoldingUser;
    }
    /// <summary>
    /// Enum representing the target of a user assignment.
    /// </summary>
    public enum UserAssignmentTarget { Student, Teacher }
    /// <summary>
    /// Message class for user assignment confirmation.
    /// </summary>
    /// <param name="isConfirmed">Indicates if the assignment is confirmed.</param>
    /// <param name="target">The target of the assignment.</param>
    public class UserAssignmentMessage(bool isConfirmed, UserAssignmentTarget target)
    {
        private readonly UserAssignmentTarget _target = target;
        private readonly bool _isConfirm = isConfirmed;

        /// <summary>
        /// Gets a value indicating whether the assignment is confirmed.
        /// </summary>
        public bool IsConfirm => _isConfirm;

        /// <summary>
        /// Gets the target of the assignment.
        /// </summary>
        public UserAssignmentTarget Target => _target;
    }
    /// <summary>
    /// ViewModel for user assignment.
    /// </summary>
    /// <param name="dao">The data access object.</param>
    /// <param name="userService">The user service.</param>
    partial class UserAssignmentViewModel(IDao dao, UserService userService) : CUDViewModel(dao)
    {
        /// <summary>
        /// The assignment confirmation message.
        /// </summary>
        public static readonly string AssignmentConfimation = "AssignmentConfirm";

        private readonly UserService _userService = userService;

        /// <summary>
        /// Gets the order of columns.
        /// </summary>
        public override IEnumerable<string> ColumnOrder => ["Username", "Password", "Email"];

        /// <summary>
        /// Gets the columns to ignore.
        /// </summary>
        public override IEnumerable<string> IgnoringColumns => ["Id", "Role", "CreatedAt", "PasswordHash"];

        /// <summary>
        /// Gets the column converters.
        /// </summary>
        public override IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => base.ColumnConverters;

        /// <summary>
        /// Gets the message of the specified item.
        /// </summary>
        /// <param name="item">The item to get the message of.</param>
        /// <returns>The message of the item.</returns>
        public override InfoBarMessage? GetMessageOf(object item) => null;

        /// <summary>
        /// Gets the row status of the specified item.
        /// </summary>
        /// <param name="item">The item to get the row status of.</param>
        /// <returns>The row status of the item.</returns>
        public override RowStatus? GetRowStatus(object item) => null;

        /// <summary>
        /// Converts the input string to lowercase and removes spaces.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The processed string.</returns>
        static private string ToLowerCaseNoSpaces(string input)
        {
            return input.ToLower().Replace(" ", "");
        }

        /// <summary>
        /// Gets the default password.
        /// </summary>
        static public string DefaultPassword => "passwordUwU";

        /// <summary>
        /// Generates a password for the specified object.
        /// </summary>
        /// <param name="obj">The object to generate a password for.</param>
        /// <returns>The generated password.</returns>
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

        /// <summary>
        /// Converts objects to users and populates the items.
        /// </summary>
        /// <param name="objects">The objects to convert.</param>
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

        /// <summary>
        /// Confirms the assignment of users.
        /// </summary>
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
