using LearningManagementSystem.Models;
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

        public void RemoveUserByID(int userId)
        {
            var sql = "DELETE FROM Users WHERE Id=@Id";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", userId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public int CountUser()
        {
            throw new NotImplementedException();
        }

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

        public void GetFullUser(User user)
        {
            var sql = "SELECT * FROM Users WHERE username=@Username AND passwordhash=@PasswordHash";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    user.Username = reader.GetString(reader.GetOrdinal("Username"));
                    user.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                    user.Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email"));
                    user.Role = reader.GetString(reader.GetOrdinal("Role"));
                    user.CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"));
                }
            }
        }

        public bool CheckUserInfo(User user)
        {
            var sql = "SELECT COUNT(*) AS TotalItems FROM Users WHERE Username=@username AND PasswordHash=@passwordhash";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@passwordhash", user.PasswordHash);

                connection.Open();
                var reader = command.ExecuteReader();
                reader.Read();
                int result = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                return result == 1;
            }
        }

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
    }
}
