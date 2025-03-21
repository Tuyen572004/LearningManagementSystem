﻿using LearningManagementSystem.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.DataAccess
{
    public partial class SqlDao
    {
        /// <summary>
        /// Retrieves a paginated list of users with optional keyword search and sorting.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <param name="keyword">The keyword to search for in usernames.</param>
        /// <param name="sortBy">The column to sort by.</param>
        /// <param name="sortOrder">The sort order (ASC or DESC).</param>
        /// <returns>A tuple containing the total number of users and a list of users for the specified page.</returns>
        public Tuple<int, List<User>> GetAllUsers(int page = 1, int pageSize = 10, string keyword = "", string sortBy = "Id", string sortOrder = "ASC")
        {
            var result = new List<User>();
            var sql = $"""
                    SELECT COUNT(*) OVER() AS TotalItems, id, username , passwordhash , email, role, createdat
                    FROM Users
                    WHERE username LIKE @Keyword
                    ORDER BY {sortBy} {sortOrder}
                    LIMIT @Take OFFSET @Skip
                    """;

            var skip = (page - 1) * pageSize;
            var take = pageSize;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Skip", skip);
                command.Parameters.AddWithValue("@Take", take);
                command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

                connection.Open();
                var reader = command.ExecuteReader();

                int totalItems = -1;
                while (reader.Read())
                {
                    if (totalItems == -1)
                    {
                        totalItems = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                    }
                    result.Add(new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Username = reader.GetString(reader.GetOrdinal("username")),
                        PasswordHash = reader.GetString(reader.GetOrdinal("passwordhash")),
                        Email = reader.GetString(reader.GetOrdinal("email")),
                        Role = reader.GetString(reader.GetOrdinal("role")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat"))
                    });
                }
                return new Tuple<int, List<User>>(totalItems, result);
            }
        }

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>The user with the specified username.</returns>
        public User GetUserByUserName(string username)
        {
            var result = new User();
            var sql = "SELECT * FROM Users WHERE username=@Username";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                        Role = reader.GetString(reader.GetOrdinal("Role")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// Inserts a new user into the database.
        /// </summary>
        /// <param name="user">The user to insert.</param>
        /// <returns>The ID of the inserted user.</returns>
        public int InsertUser(User user)
        {
            var sql = """
                    INSERT INTO Users (username, passwordhash, email, role, createdat)
                    VALUES (@Username, @PasswordHash, @Email, @Role, @CreatedAt)
                    RETURNING Id;
                    """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);

                connection.Open();
                int id = (int)command.ExecuteScalar();
                user.Id = id;

                return id > 0 ? 1 : -1;
            }
        }

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <param name="user">The user to update.</param>
        public void UpdateUser(User user)
        {
            var sql = "UPDATE Users SET Username=@Username, PasswordHash=@PasswordHash, Email=@Email, Role=@Role WHERE Id=@Id";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.Parameters.AddWithValue("@Id", user.Id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Removes a user from the database by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to remove.</param>
        /// <returns>1 if the user was removed successfully, otherwise 0.</returns>
        public int RemoveUserByID(int userId)
        {
            var sql = "DELETE FROM Users WHERE Id=@Id";
            try
            {
                using (var connection = GetConnection())
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", userId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// Counts the total number of users in the database.
        /// </summary>
        /// <returns>The total number of users.</returns>
        public int CountUser()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a list of all usernames in the database.
        /// </summary>
        /// <returns>A list of all usernames.</returns>
        public List<string> GetAllUsernames()
        {
            var result = new List<string>();
            var sql = "SELECT Username FROM Users";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.GetString(reader.GetOrdinal("username")));
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves the full details of a user by their username and password hash.
        /// </summary>
        /// <param name="user">The user to retrieve.</param>
        public void GetFullUser(User user)
        {
            var sql = "SELECT * FROM Users WHERE email=@email AND passwordhash=@PasswordHash";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    user.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                    user.Username = reader.GetString(reader.GetOrdinal("Username"));
                    user.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                    user.Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email"));
                    user.Role = reader.GetString(reader.GetOrdinal("Role"));
                    user.CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"));
                }
            }
        }

        /// <summary>
        /// Checks if a user with the specified username and password hash exists in the database.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>True if the user exists, otherwise false.</returns>
        public bool CheckUserInfo(User user)
        {
            var sql = "SELECT COUNT(*) AS TotalItems FROM Users WHERE email=@email AND PasswordHash=@passwordhash";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@passwordhash", user.PasswordHash);

                connection.Open();
                var reader = command.ExecuteReader();
                reader.Read();
                int result = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                return result == 1;
            }
        }

        /// <summary>
        /// Checks if a username exists in the database.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>True if the username exists, otherwise false.</returns>
        public bool IsExistsUsername(string username)
        {
            var sql = "SELECT COUNT(*) AS TotalItems FROM Users WHERE Username=@username";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@username", username);

                connection.Open();
                var reader = command.ExecuteReader();
                reader.Read();
                int result = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                return result == 1;
            }
        }

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <returns>True if the user was added successfully, otherwise false.</returns>
        public bool AddUser(User user)
        {
            var sql = """
                    INSERT INTO Users (Username, PasswordHash, Email, Role, CreatedAt)
                    VALUES (@username, @passwordhash, null, @role, null)
                    RETURNING Id;
                    """;
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@passwordhash", user.PasswordHash);
                command.Parameters.AddWithValue("@role", user.Role);

                connection.Open();
                int id = (int)command.ExecuteScalar();
                user.Id = id;
                return id > 0;
            }
        }

        /// <summary>
        /// Sets the UserId to null for a teacher in the database.
        /// </summary>
        /// <param name="user">The user whose UserId to set to null.</param>
        /// <returns>1 if the operation was successful, otherwise 0.</returns>
        public int SetNullUserIDInTeacher(User user)
        {
            var sql = "UPDATE Teachers SET UserId=null WHERE UserId=@Id";
            try
            {
                using (var connection = GetConnection())
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// Sets the UserId to null for a student in the database.
        /// </summary>
        /// <param name="user">The user whose UserId to set to null.</param>
        /// <returns>1 if the operation was successful, otherwise 0.</returns>
        public int SetNullUserIDInStudent(User user)
        {
            var sql = "UPDATE Students SET UserId=null WHERE UserId=@Id";
            try
            {
                using (var connection = GetConnection())
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// Finds a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to find.</param>
        /// <returns>The user with the specified ID.</returns>
        public User findUserById(int? id)
        {
            var result = new User();
            var sql = "SELECT * FROM Users WHERE Id=@Id";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                        Role = reader.GetString(reader.GetOrdinal("Role")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                    };
                }
            }
            return result;
        }
    }
}
