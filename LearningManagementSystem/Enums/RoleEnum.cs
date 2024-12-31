using System;

namespace LearningManagementSystem.Enums
{
    /// <summary>
    /// Represents the different roles available in the Learning Management System.
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// Represents a student role.
        /// </summary>
        Student,

        /// <summary>
        /// Represents a teacher role.
        /// </summary>
        Teacher,

        /// <summary>
        /// Represents an admin role.
        /// </summary>
        Admin
    }

    public static class RoleEnum
    {
        /// <summary>
        /// Gets the string value associated with the specified role.
        /// </summary>
        /// <param name="role">The role to get the string value for.</param>
        /// <returns>The string value of the specified role.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the role is not recognized.</exception>
        public static string GetStringValue(Role role)
        {
            switch (role)
            {
                case Role.Student:
                    return "student";
                case Role.Teacher:
                    return "teacher";
                case Role.Admin:
                    return "admin";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
