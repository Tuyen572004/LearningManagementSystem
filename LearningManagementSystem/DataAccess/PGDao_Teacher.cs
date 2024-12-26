using CommunityToolkit.WinUI.UI.Controls;
using LearningManagementSystem.Controls;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.DataAccess
{
    public partial class SqlDao
    {
        public Teacher GetTeacherById(int teacherId)
        {
            return new Teacher
            {
                Id = teacherId,
                TeacherCode = "T001",
                TeacherName = "John Doe",
                Email = "johndoe@example.com",
                PhoneNo = "1234567890",
                UserId = 1
            };
        }

        public FullObservableCollection<Teacher> GetTeachersByClassId(int classId)
        {
            return new FullObservableCollection<Teacher>
            {
                new Teacher
                {
                    Id = 1,
                    TeacherCode = "T001",
                    TeacherName = "John Doe",
                    Email = "johndoe@example.com",
                    PhoneNo = "1234567890",
                    UserId = 1
                },
                new Teacher
                {
                    Id = 2,
                    TeacherCode = "T002",
                    TeacherName = "Jane Smith",
                    Email = "janesmith@example.com",
                    PhoneNo = "9876543210",
                    UserId = 2
                },
                new Teacher
                {
                    Id = 3,
                    TeacherCode = "T003",
                    TeacherName = "Alice Johnson",
                    Email = "janesmith@example.com",
                    PhoneNo = "9876543210",
                    UserId = 3
                }
            };
        }

        public (
            IList<Teacher> addedTeachers,
            int addedCount,
            IList<(Teacher teacher, IEnumerable<string> errors)> invalidTeachersInfo
        ) AddTeachers(IEnumerable<Teacher> teachers)
        {
            var addedTeachers = new List<Teacher>();
            int addedCount = 0;
            var invalidTeachers = new List<(Teacher teacher, IEnumerable<string> errors)>();
                
            using var connection = GetConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var teacher in teachers)
                {
                    try
                    {
                        // Check if TeacherCode already exists
                        var checkTeacherIdCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM "teachers"
                            WHERE "teachercode" = @TeacherCode
                            """, connection, transaction);
                        checkTeacherIdCommand.Parameters.AddWithValue("@TeacherCode", teacher.TeacherCode);

                        int existsCount = Convert.ToInt32(checkTeacherIdCommand.ExecuteScalar() ?? 0);
                        if (existsCount > 0)
                        {
                            invalidTeachers.Add((
                                teacher,
                                new List<string> { $"Teacher with TeacherCode {teacher.TeacherCode} already exists." }
                            ));
                            continue;
                        }

                        // Insert the new teacher
                        var insertCommand = new NpgsqlCommand(
                            """
                            INSERT INTO "teachers" ("teachercode", "teachername", "email", "phoneno", "userid")
                            VALUES (@TeacherCode, @TeacherName, @Email, @PhoneNo, @UserId)
                            """, connection, transaction);

                        insertCommand.Parameters.AddWithValue("@TeacherCode", teacher.TeacherCode);
                        insertCommand.Parameters.AddWithValue("@TeacherName", teacher.TeacherName);
                        insertCommand.Parameters.AddWithValue("@Email", teacher.Email);
                        insertCommand.Parameters.AddWithValue("@PhoneNo", teacher.PhoneNo ?? (object)DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@UserId", teacher.UserId ?? (object)DBNull.Value);

                        int queryResult = insertCommand.ExecuteNonQuery();
                        if (queryResult > 0)
                        {
                            addedTeachers.Add(teacher);
                            addedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        invalidTeachers.Add((
                            teacher,
                            new List<string> { $"Exception raised when adding to database: {ex.Message}" }
                        ));
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                invalidTeachers.Add((
                    null,
                    [$"Transaction failed: {ex.Message}"]
                ));
            }

            return (addedTeachers, addedCount, invalidTeachers);
        }

        public (
            IList<Teacher> updatedTeachers,
            int updatedCount,
            IList<(Teacher teacher, IEnumerable<string> errors)> invalidTeachersInfo
        ) UpdateTeachers(IEnumerable<Teacher> teachers)
        {
            var updatedTeachers = new List<Teacher>();
            int updatedCount = 0;
            var invalidTeachers = new List<(Teacher teacher, IEnumerable<string> errors)>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                foreach (var teacher in teachers)
                {
                    try
                    {
                        // Check if Teacher exists
                        var checkExistenceCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM "teachers"
                            WHERE "id" = @Id
                            """, connection, transaction);
                        checkExistenceCommand.Parameters.AddWithValue("@Id", teacher.Id);

                        var existsCount = Convert.ToInt32(checkExistenceCommand.ExecuteScalar() ?? 0);
                        if (existsCount == 0)
                        {
                            invalidTeachers.Add((teacher, new[] { $"Teacher with Id {teacher.Id} not found." }));
                            continue;
                        }

                        // Check if UserId exists
                        if (teacher.UserId is not null)
                        {
                            var checkUserIdCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM "users"
                            WHERE "id" = @UserId
                            """, connection, transaction);
                            checkUserIdCommand.Parameters.AddWithValue("@UserId", teacher.UserId);

                            var userIdExistsCount = Convert.ToInt32(checkUserIdCommand.ExecuteScalar() ?? 0);
                            if (userIdExistsCount == 0)
                            {
                                invalidTeachers.Add((teacher, new[] { $"User with Id {teacher.UserId} not found." }));
                                continue;
                            }
                        }

                        // Check if TeacherCode is unique (excluding current Teacher)
                        var checkNewCodeCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM "teachers"
                            WHERE "teachercode" = @TeacherCode AND "id" != @Id
                            """, connection, transaction);
                        checkNewCodeCommand.Parameters.AddWithValue("@TeacherCode", teacher.TeacherCode);
                        checkNewCodeCommand.Parameters.AddWithValue("@Id", teacher.Id);

                        var newCodeExistsCount = Convert.ToInt32(checkNewCodeCommand.ExecuteScalar() ?? 0);
                        if (newCodeExistsCount > 0)
                        {
                            invalidTeachers.Add((teacher, new[] { $"Teacher with TeacherCode {teacher.TeacherCode} already exists." }));
                            continue;
                        }

                        // Update Teacher record
                        var updateCommand = new NpgsqlCommand(
                            """
                            UPDATE "teachers"
                            SET "teachercode" = @TeacherCode, "teachername" = @TeacherName, "email" = @Email, 
                                "phoneno" = @PhoneNo, "userid" = @UserId
                            WHERE "id" = @Id
                            """, connection, transaction);
                        updateCommand.Parameters.AddWithValue("@Id", teacher.Id);
                        updateCommand.Parameters.AddWithValue("@TeacherCode", teacher.TeacherCode);
                        updateCommand.Parameters.AddWithValue("@TeacherName", teacher.TeacherName);
                        updateCommand.Parameters.AddWithValue("@Email", teacher.Email);
                        updateCommand.Parameters.AddWithValue("@PhoneNo", teacher.PhoneNo ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@UserId", teacher.UserId ?? (object)DBNull.Value);

                        int queryResult = updateCommand.ExecuteNonQuery();
                        if (queryResult > 0)
                        {
                            updatedTeachers.Add(teacher);
                            updatedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        invalidTeachers.Add((teacher, new[] { "Exception raised when updating to database: " + ex.Message }));
                    }
                }
                transaction.Commit();
            }
            return (updatedTeachers, updatedCount, invalidTeachers);
        }

        public (
            IList<Teacher> deletedTeachers,
            int deletedCount,
            IList<(Teacher teacher, IEnumerable<string> errors)> invalidTeachersInfo
        ) DeleteTeachers(IEnumerable<Teacher> teachers)
        {
            var deletedTeachers = new List<Teacher>();
            int deletedCount = 0;
            var invalidTeachers = new List<(Teacher teacher, IEnumerable<string> errors)>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                foreach (var teacher in teachers)
                {
                    try
                    {
                        if (teacher.Id == -1)
                        {
                            invalidTeachers.Add((teacher, new[] { "Newly created teacher can't be deleted." }));
                            continue;
                        }

                        // Check if Teacher exists
                        var checkExistenceCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM "teachers"
                            WHERE "id" = @Id
                            """, connection, transaction);
                        checkExistenceCommand.Parameters.AddWithValue("@Id", teacher.Id);

                        var existsCount = Convert.ToInt32(checkExistenceCommand.ExecuteScalar() ?? 0);
                        if (existsCount == 0)
                        {
                            invalidTeachers.Add((teacher, new[] { $"Teacher with Id {teacher.Id} not found." }));
                            continue;
                        }

                        // Check for references to the Teacher in other tables
                        var checkReferenceCommand = new NpgsqlCommand(
                            """
                            SELECT 
                                (SELECT COUNT(*) FROM "teachersperclass" WHERE "teacherid" = @TeacherId) +
                                (SELECT COUNT(*) FROM "assignments" WHERE "teacherid" = @TeacherId) 
                            AS "TotalReferences"
                            """, connection, transaction);
                        checkReferenceCommand.Parameters.AddWithValue("@TeacherId", teacher.Id);

                        var referenceCount = Convert.ToInt32(checkReferenceCommand.ExecuteScalar() ?? 0);
                        if (referenceCount > 0)
                        {
                            invalidTeachers.Add((teacher, new[] { $"Teacher with Id {teacher.Id} has {referenceCount} references." }));
                            continue;
                        }

                        // Delete the associated User if applicable
                        if (teacher.UserId > 0)
                        {
                            var deletingUserCommand = new NpgsqlCommand(
                                """
                                DELETE FROM "users" 
                                WHERE "id" = @UserId
                                """, connection, transaction);
                            deletingUserCommand.Parameters.AddWithValue("@UserId", teacher.UserId);

                            int userQueryResult = deletingUserCommand.ExecuteNonQuery();
                            if (userQueryResult == 0)
                            {
                                invalidTeachers.Add((teacher, new[] { $"User with Id {teacher.UserId} referenced by this teacher not found." }));
                                continue;
                            }
                        }

                        // Delete the Teacher record
                        var deletingTeacherCommand = new NpgsqlCommand(
                            """
                            DELETE FROM "teachers" 
                            WHERE "id" = @Id
                            """, connection, transaction);
                        deletingTeacherCommand.Parameters.AddWithValue("@Id", teacher.Id);

                        int queryResult = deletingTeacherCommand.ExecuteNonQuery();
                        if (queryResult > 0)
                        {
                            deletedTeachers.Add(teacher);
                            deletedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        invalidTeachers.Add((teacher, new[] { "Exception raised when deleting from database: " + ex.Message }));
                    }
                }
                transaction.Commit();
            }
            return (deletedTeachers, deletedCount, invalidTeachers);
        }

        public (ObservableCollection<Teacher>, int) GetTeachers(
            bool fetchingAll = false,
            int ignoringCount = 0,
            int fetchingCount = 0,
            IEnumerable<int> chosenIds = null,
            IEnumerable<SortCriteria> sortCriteria = null,
            SearchCriteria searchCriteria = null
        )
        {
            ObservableCollection<Teacher> result = [];
            int queryCount = 0;

            using (var connection = GetConnection())
            {
                connection.Open();
                var query =
                    """
                    select count(*) over() as "totalitem", "id", "teachercode", "teachername", "email", "phoneno", "userid"
                    from "teachers"
                    """;

                query += '\n';

                bool startingWhere = true;
                string whereCondition(string condition)
                {
                    if (startingWhere)
                    {
                        startingWhere = false;
                        return string.Format("where {0}\n", condition);
                    }
                    else
                    {
                        return string.Format("    and {0}\n", condition);
                    }
                }

                if (chosenIds is not null && chosenIds.Any())
                {
                    query += whereCondition($"\"id\" in ({string.Join(", ", chosenIds)})");
                }

                if (searchCriteria is not null)
                {
                    string searchCondition = searchCriteria.Field switch
                    {
                        _ => $"\"{searchCriteria.Field.ToLower()}\""
                    };

                    searchCondition = searchCriteria.Pattern switch
                    {
                        "is not null" or "is null" => $"{searchCondition} {searchCriteria.Pattern}",
                        _ => $"coalesce(cast({searchCondition} as text), '') {searchCriteria.Pattern}"
                    };

                    query += whereCondition(searchCondition);
                }

                List<string> sortQuery = sortCriteria
                    ?.Select(s =>
                    {
                        string sortField = $"\"{s.ColumnTag.ToLower()}\"";
                        string sortDirection = s.SortDirection == DataGridSortDirection.Ascending ? "asc" : "desc";
                        return $"{sortField} {sortDirection}";
                    })
                    .ToList() ?? [];

                if (sortQuery.Count != 0)
                {
                    query += $"order by {string.Join(", ", sortQuery)}\n";
                }

                if (!fetchingAll)
                {
                    query += "limit @Take offset @Skip\n";
                }

                var command = new NpgsqlCommand(query, connection);

                if (!fetchingAll)
                {
                    command.Parameters.Add("@Skip", NpgsqlDbType.Integer).Value = ignoringCount;
                    command.Parameters.Add("@Take", NpgsqlDbType.Integer).Value = fetchingCount;
                }

                var queryResultReader = command.ExecuteReader();

                bool isTotalItemFetched = false;

                while (queryResultReader.Read())
                {
                    if (!isTotalItemFetched)
                    {
                        queryCount = queryResultReader.GetInt32(queryResultReader.GetOrdinal("totalitem"));
                        isTotalItemFetched = true;
                    }

                    int userIdColumn = queryResultReader.GetOrdinal("userid");
                    Teacher newTeacher = new()
                    {
                        Id = queryResultReader.GetInt32("id"),
                        TeacherCode = queryResultReader.GetString("teachercode"),
                        TeacherName = queryResultReader.GetString("teachername"),
                        Email = queryResultReader.GetString("email"),
                        PhoneNo = queryResultReader.IsDBNull("phoneno")
                            ? null
                            : queryResultReader.GetString("phoneno"),
                        UserId = queryResultReader.IsDBNull(userIdColumn)
                            ? null
                            : queryResultReader.GetInt32("userid")
                    };

                    result.Add(newTeacher);
                }
            }
            return (result, queryCount);
        }
    }

}
