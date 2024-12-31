using CommunityToolkit.WinUI.UI.Controls;
using LearningManagementSystem.Controls;
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
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
        public (
            IList<StudentVer2> addStudents,
            int addCount,
            IList<(StudentVer2 student, IEnumerable<string> error)> invalidStudentsInfo
        ) AddStudents(IEnumerable<StudentVer2> students)
        {
            List<StudentVer2> addedStudents = [];
            int addedCount = 0;
            List<(StudentVer2 student, IEnumerable<string> error)> invalidStudents = [];

            using (var connection = GetConnection())
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                foreach (var student in students)
                {
                    try
                    {
                        var checkStudentIdCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM "students"
                            WHERE "studentcode" = @StudentCode
                            """, connection, transaction);
                        checkStudentIdCommand.Parameters.AddWithValue("@StudentCode", student.StudentCode);
                        int existsCount = Convert.ToInt32(checkStudentIdCommand.ExecuteScalar() ?? 0);
                        if (existsCount > 0)
                        {
                            invalidStudents.Add((student, new List<string> { "Student with StudentCode " + student.StudentCode + " already exists" }));
                            continue;
                        }

                        if (student.UserId == ITemporaryUserHolder.NewlyHoldingUserId)
                        {
                            User holdingUser = null;
                            (student as ITemporaryUserHolder).GetHoldingUserByReference(ref holdingUser);
                            if (holdingUser is null)
                            {
                                student.UserId = null;
                            }
                            else
                            {
                                var insertUserCommand = new NpgsqlCommand(
                                    """
                                    INSERT INTO "users" ("username", "passwordhash", "role", "email", "createdat")
                                    VALUES (@Username, @PasswordHash, @Role, @Email, @CreatedAt)
                                    RETURNING "id"
                                    """, connection, transaction);
                                insertUserCommand.Parameters.AddWithValue("@Username", holdingUser.Username);
                                insertUserCommand.Parameters.AddWithValue("@PasswordHash", holdingUser.PasswordHash);
                                insertUserCommand.Parameters.AddWithValue("@Role", holdingUser.Role);
                                insertUserCommand.Parameters.AddWithValue("@Email", holdingUser.Email);
                                insertUserCommand.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                                var userId = insertUserCommand.ExecuteScalar();
                                if (userId != null)
                                {
                                    student.UserId = Convert.ToInt32(userId);
                                }
                                else
                                {
                                    invalidStudents.Add((student, new List<string> { "Failed to insert holding user." }));
                                    continue;
                                }
                            }
                        }

                        var insertCommand = new NpgsqlCommand(
                            """
                            INSERT INTO "students" ("studentcode", "studentname", "email", "birthdate", "phoneno", "userid", "enrollmentyear", "graduationyear")
                            VALUES (@StudentCode, @StudentName, @Email, @BirthDate, @PhoneNo, @UserId, @EnrollmentYear, @GraduationYear)
                            """, connection, transaction);

                        insertCommand.Parameters.AddWithValue("@StudentCode", student.StudentCode);
                        insertCommand.Parameters.AddWithValue("@StudentName", student.StudentName);
                        insertCommand.Parameters.AddWithValue("@Email", student.Email);
                        insertCommand.Parameters.AddWithValue("@BirthDate", student.BirthDate);
                        insertCommand.Parameters.AddWithValue("@PhoneNo", student.PhoneNo);
                        insertCommand.Parameters.AddWithValue("@UserId", student.UserId ?? (object)DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@EnrollmentYear", student.EnrollmentYear);
                        insertCommand.Parameters.AddWithValue("@GraduationYear", student.GraduationYear ?? (object)DBNull.Value);

                        int queryResult = insertCommand.ExecuteNonQuery();
                        if (queryResult > 0)
                        {
                            addedStudents.Add(student);
                            addedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        invalidStudents.Add((student, new List<string> { "Exception raised when adding to database: " + ex.Message }));
                    }
                }
                transaction.Commit();
            }
            return (addedStudents, addedCount, invalidStudents);
        }

        /// <summary>
        /// Counts the total number of students in the database.
        /// </summary>
        /// <returns>The total number of students.</returns>
        public int CountStudent()
        {
            int count = 0;
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new NpgsqlCommand(
                    """
                    SELECT COUNT(*) FROM students
                    """, connection);
                count = Convert.ToInt32(command.ExecuteScalar() ?? 0);
            }
            return count;
        }

        /// <summary>
        /// Updates a list of students in the database.
        /// </summary>
        /// <param name="students">The list of students to update.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item>
        /// <description>A list of successfully updated students.</description>
        /// </item>
        /// <item>
        /// <description>The count of successfully updated students.</description>
        /// </item>
        /// <item>
        /// <description>A list of students that failed to update along with the corresponding error messages.</description>
        /// </item>
        /// </list>
        /// </returns>
        public (
            IList<StudentVer2> updateStudents,
            int updatedCount,
            IList<(StudentVer2 student, IEnumerable<string> error)> invalidStudentsInfo
        ) UpdateStudents(IEnumerable<StudentVer2> students)
        {
            List<StudentVer2> updatedStudents = [];
            int updatedCount = 0;
            List<(StudentVer2 student, IEnumerable<string> error)> invalidStudents = [];

            using (var connection = GetConnection())
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                foreach (var student in students)
                {
                    try
                    {
                        var checkExistenceCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM "students"
                            WHERE "id" = @Id
                            """, connection, transaction);
                        checkExistenceCommand.Parameters.AddWithValue("@Id", student.Id);

                        var existsCount = Convert.ToInt32(checkExistenceCommand.ExecuteScalar() ?? 0);
                        if (existsCount == 0)
                        {
                            invalidStudents.Add((student, new[] { $"Student with Id {student.Id} not found" }));
                            continue;
                        }

                        if (student.UserId == ITemporaryUserHolder.NewlyHoldingUserId)
                        {
                            User holdingUser = null;
                            (student as ITemporaryUserHolder).GetHoldingUserByReference(ref holdingUser);
                            if (holdingUser is null)
                            {
                                student.UserId = null;
                            }
                            else
                            {
                                var insertUserCommand = new NpgsqlCommand(
                                    """
                                    INSERT INTO "users" ("username", "passwordhash", "role", "email", "createdat")
                                    VALUES (@Username, @PasswordHash, @Role, @Email, @CreatedAt)
                                    RETURNING "id"
                                    """, connection, transaction);
                                insertUserCommand.Parameters.AddWithValue("@Username", holdingUser.Username);
                                insertUserCommand.Parameters.AddWithValue("@PasswordHash", holdingUser.PasswordHash);
                                insertUserCommand.Parameters.AddWithValue("@Role", holdingUser.Role);
                                insertUserCommand.Parameters.AddWithValue("@Email", holdingUser.Email);
                                insertUserCommand.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                                var userId = insertUserCommand.ExecuteScalar();
                                if (userId != null)
                                {
                                    student.UserId = Convert.ToInt32(userId);
                                }
                                else
                                {
                                    invalidStudents.Add((student, new List<string> { "Failed to insert holding user." }));
                                    continue;
                                }
                            }
                        }
                        else if (student.UserId is not null)
                        {
                            var checkUserIdCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM "users"
                            WHERE "id" = @UserId
                            """, connection, transaction);
                            checkUserIdCommand.Parameters.AddWithValue("@UserId", student.UserId);

                            var userIdExistsCount = Convert.ToInt32(checkUserIdCommand.ExecuteScalar() ?? 0);
                            if (userIdExistsCount == 0)
                            {
                                invalidStudents.Add((student, new[] { $"User with Id {student.UserId} not found" }));
                                continue;
                            }
                        }

                        var checkNewCodeCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM "students"
                            WHERE "studentcode" = @StudentCode AND "id" != @Id
                            """, connection, transaction);
                        checkNewCodeCommand.Parameters.AddWithValue("@StudentCode", student.StudentCode);
                        checkNewCodeCommand.Parameters.AddWithValue("@Id", student.Id);

                        var newCodeExistsCount = Convert.ToInt32(checkNewCodeCommand.ExecuteScalar() ?? 0);
                        if (newCodeExistsCount > 0)
                        {
                            invalidStudents.Add((student, new[] { $"Student with StudentCode {student.StudentCode} already exists" }));
                            continue;
                        }

                        var updateCommand = new NpgsqlCommand(
                            """
                            UPDATE "students"
                            SET "studentcode" = @StudentCode, "studentname" = @StudentName, "email" = @Email, 
                                "birthdate" = @BirthDate, "phoneno" = @PhoneNo, "userid" = @UserId, 
                                "enrollmentyear" = @EnrollmentYear, "graduationyear" = @GraduationYear
                            WHERE "id" = @Id
                            """, connection, transaction);
                        updateCommand.Parameters.AddWithValue("@Id", student.Id);
                        updateCommand.Parameters.AddWithValue("@StudentCode", student.StudentCode);
                        updateCommand.Parameters.AddWithValue("@StudentName", student.StudentName);
                        updateCommand.Parameters.AddWithValue("@Email", student.Email);
                        updateCommand.Parameters.AddWithValue("@BirthDate", student.BirthDate);
                        updateCommand.Parameters.AddWithValue("@PhoneNo", student.PhoneNo);
                        updateCommand.Parameters.AddWithValue("@UserId", student.UserId ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@EnrollmentYear", student.EnrollmentYear);
                        updateCommand.Parameters.AddWithValue("@GraduationYear", student.GraduationYear ?? (object)DBNull.Value);

                        int queryResult = updateCommand.ExecuteNonQuery();
                        if (queryResult > 0)
                        {
                            updatedStudents.Add(student);
                            updatedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        invalidStudents.Add((student, new[] { "Exception raised when updating to database: " + ex.Message }));
                    }
                }
                transaction.Commit();
            }
            return (updatedStudents, updatedCount, invalidStudents);
        }

        /// <summary>
        /// Deletes a list of students from the database.
        /// </summary>
        /// <param name="students">The list of students to delete.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item>
        /// <description>A list of successfully deleted students.</description>
        /// </item>
        /// <item>
        /// <description>The count of successfully deleted students.</description>
        /// </item>
        /// <item>
        /// <description>A list of students that failed to delete along with the corresponding error messages.</description>
        /// </item>
        /// </list>
        /// </returns>
        public (
            IList<StudentVer2> deleteStudents,
            int deletedCount,
            IList<(StudentVer2 student, IEnumerable<string> error)> invalidStudentsInfo
        ) DeleteStudents(IEnumerable<StudentVer2> students)
        {
            List<StudentVer2> deletedStudents = [];
            int deletedCount = 0;
            List<(StudentVer2 student, IEnumerable<string> error)> invalidStudents = [];

            using (var connection = GetConnection())
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                foreach (var student in students)
                {
                    try
                    {
                        if (student.Id == -1)
                        {
                            invalidStudents.Add((student, new[] { "Newly created student can't be deleted" }));
                            continue;
                        }

                        var checkExistenceCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM "students"
                            WHERE "id" = @Id
                            """, connection, transaction);
                        checkExistenceCommand.Parameters.AddWithValue("@Id", student.Id);

                        var existsCount = Convert.ToInt32(checkExistenceCommand.ExecuteScalar() ?? 0);
                        if (existsCount == 0)
                        {
                            invalidStudents.Add((student, new[] { $"Student with Id {student.Id} not found" }));
                            continue;
                        }

                        var checkReferenceCommand = new NpgsqlCommand(
                            """
                            SELECT 
                                (SELECT COUNT(*) FROM "enrollments" WHERE "studentid" = @StudentId) 
                            AS "TotalReferences"
                            """, connection, transaction);
                        checkReferenceCommand.Parameters.AddWithValue("@StudentId", student.Id);

                        var referenceCount = Convert.ToInt32(checkReferenceCommand.ExecuteScalar() ?? 0);
                        if (referenceCount > 0)
                        {
                            invalidStudents.Add((student, new[] { $"Student with Id {student.Id} has {referenceCount} references" }));
                            continue;
                        }

                        var deletingStudentCommand = new NpgsqlCommand(
                            """
                            DELETE FROM "students" 
                            WHERE "id" = @Id
                            """, connection, transaction);
                        deletingStudentCommand.Parameters.AddWithValue("@Id", student.Id);

                        int queryResult = deletingStudentCommand.ExecuteNonQuery();
                        if (queryResult > 0)
                        {
                            deletedStudents.Add(student);
                            deletedCount++;
                        }

                        // Student contains the user's reference, so student should be deleted first!
                        if (student.UserId is not null)
                        {
                            var deletingUserCommand = new NpgsqlCommand(
                                """
                                DELETE FROM "users" 
                                WHERE "id" = @UserId
                                """, connection, transaction);
                            deletingUserCommand.Parameters.AddWithValue("@UserId", student.UserId);

                            int userQueryResult = deletingUserCommand.ExecuteNonQuery();
                            if (userQueryResult == 0)
                            {
                                invalidStudents.Add((student, new[] { $"User with Id {student.UserId} referenced by this student not found" }));
                                continue;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        invalidStudents.Add((student, new[] { "Exception raised when deleting from database: " + ex.Message }));
                    }
                }
                transaction.Commit();
            }
            return (deletedStudents, deletedCount, invalidStudents);
        }

        /// <summary>
        /// Retrieves a collection of students from the database based on the specified criteria.
        /// </summary>
        /// <param name="fetchingAll">If true, fetches all students; otherwise, fetches a limited number of students.</param>
        /// <param name="ignoringCount">The number of students to skip.</param>
        /// <param name="fetchingCount">The number of students to fetch.</param>
        /// <param name="chosenIds">A collection of student IDs to filter the results.</param>
        /// <param name="sortCriteria">A collection of sorting criteria to order the results.</param>
        /// <param name="searchCriteria">The search criteria to filter the results.</param>
        /// <returns>A tuple containing an observable collection of students and the total count of students.</returns>
        public (ObservableCollection<StudentVer2>, int) GetStudents(
            bool fetchingAll = false,
            int ignoringCount = 0,
            int fetchingCount = 0,
            IEnumerable<int> chosenIds = null,
            IEnumerable<SortCriteria> sortCriteria = null,
            SearchCriteria searchCriteria = null
        )
        {
            ObservableCollection<StudentVer2> result = [];
            int queryCount = 0;

            using (var connection = GetConnection())
            {
                connection.Open();
                var query =
                    """
                    select count(*) over() as "totalitem", "id", "studentcode", "studentname", "email", "birthdate", "phoneno", "userid", "enrollmentyear", "graduationyear"
                    from "students"
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
                    string searchCondition = "";
                    searchCondition = searchCriteria.Field switch
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
                    }
                    ).ToList()
                    ?? [];

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

                    int graduationYearColumn = queryResultReader.GetOrdinal("graduationyear");
                    int userIdColumn = queryResultReader.GetOrdinal("userid");
                    StudentVer2 newStudent = new()
                    {
                        Id = queryResultReader.GetInt32("id"),
                        StudentCode = queryResultReader.GetString("studentcode"),
                        StudentName = queryResultReader.GetString("studentname"),
                        Email = queryResultReader.GetString("email"),
                        BirthDate = queryResultReader.GetDateTime("birthdate"),
                        PhoneNo = queryResultReader.GetString("phoneno"),
                        UserId = (queryResultReader.IsDBNull(userIdColumn)
                            ? null
                            : queryResultReader.GetInt32("userid")
                            ),
                        EnrollmentYear = queryResultReader.GetInt32("enrollmentyear"),
                        GraduationYear = (queryResultReader.IsDBNull(graduationYearColumn)
                            ? null
                            : queryResultReader.GetInt32("graduationyear")
                            )
                    };

                    result.Add(newStudent);
                };
            }
            return (result, queryCount);
        }

        /// <summary>
        /// Retrieves a student by their user ID.
        /// </summary>
        /// <param name="id">The user ID of the student to retrieve.</param>
        /// <returns>A <see cref="Student"/> object containing the student's details.</returns>
        public Student GetStudentByUserId(int id)
        {
            var result = new Student();
            var sql = """
                SELECT Id, StudentCode, StudentName, Email, BirthDate, PhoneNo, UserId
                FROM Students
                WHERE UserId=@UserId
                """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@UserId", id);
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result = new Student
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        StudentCode = reader.GetString(reader.GetOrdinal("StudentCode")),
                        StudentName = reader.GetString(reader.GetOrdinal("StudentName")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        BirthDate = reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                        PhoneNo = reader.GetString(reader.GetOrdinal("PhoneNo")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId"))
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// Finds students by their IDs and returns a list of students with their names and user IDs.
        /// </summary>
        /// <param name="ids">A list of student IDs to search for.</param>
        /// <returns>A list of <see cref="StudentVer2"/> objects containing the student names and user IDs.</returns>
        public List<StudentVer2> findStudentsByIdIn(List<int> ids)
        {
            List<StudentVer2> result = [];
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                    SELECT studentname, userid FROM students WHERE id = ANY(@Ids)
                ";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Ids", ids.ToArray());
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new StudentVer2
                    {
                        StudentName = reader.GetString(0),
                        UserId = reader.IsDBNull(1) ? null : reader.GetInt32(1)
                    });
                }
            }
            return result;
        }
    }
}
