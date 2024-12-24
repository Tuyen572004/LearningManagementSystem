using CommunityToolkit.WinUI.UI.Controls;
using LearningManagementSystem.Controls;
using LearningManagementSystem.Models;
using LearningManagementSystem.Views;
using MathNet.Numerics.Distributions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.DataAccess
{
    partial class SqlDao
    {
        public (
            IList<int> addedStudentIds,
            int addedCount,
            IList<(int studentId, IEnumerable<string> error)> invalidStudentIdsInfo
        ) AddStudentsToClass(IEnumerable<int> studentIds, int classId)
        {
            List<int> addedStudentIds = [];
            int addedCount = 0;
            List<(int studentId, IEnumerable<string> error)> invalidStudentIds = [];

            using (var connection = GetConnection())
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // Check if the class exists once
                    var checkClassIdCommand = new NpgsqlCommand(
                        """
                        SELECT COUNT(*) FROM Classes
                        WHERE Id = @ClassId
                        """, connection, transaction);
                    checkClassIdCommand.Parameters.AddWithValue("@ClassId", classId);
                    int existingClassesCount = Convert.ToInt32(checkClassIdCommand.ExecuteScalar() ?? 0);

                    if (existingClassesCount == 0)
                    {
                        foreach (var studentId in studentIds)
                        {
                            invalidStudentIds.Add((studentId, new List<string> { $"Class with Id {classId} doesn't exist at the moment of addition" }));
                        }
                        return (addedStudentIds, addedCount, invalidStudentIds);
                    }
                }
                catch (Exception ex)
                {
                    foreach (var studentId in studentIds)
                    {
                        invalidStudentIds.Add((studentId, new List<string> { "Exception raised when checking class existence: " + ex.Message }));
                    }
                    return (addedStudentIds, addedCount, invalidStudentIds);
                }

                foreach (var studentId in studentIds)
                {
                    try
                    {
                        var checkStudentIdCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM Students
                            WHERE Id = @StudentId
                            """, connection, transaction);
                        checkStudentIdCommand.Parameters.AddWithValue("@StudentId", studentId);
                        int existingStudentsCount = Convert.ToInt32(checkStudentIdCommand.ExecuteScalar() ?? 0);
                        if (existingStudentsCount == 0)
                        {
                            invalidStudentIds.Add((studentId, new List<string> { $"Student with Id {studentId} doesn't exists at the moment of insertion" }));
                            continue;
                        }

                        var checkExistingEnrollmentCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM Enrollments
                            WHERE ClassId = @ClassId AND StudentId = @StudentId
                            """, connection, transaction);
                        checkExistingEnrollmentCommand.Parameters.AddWithValue("@ClassId", classId);
                        checkExistingEnrollmentCommand.Parameters.AddWithValue("@StudentId", studentId);
                        int existingEnrollmentsCount = Convert.ToInt32(checkExistingEnrollmentCommand.ExecuteScalar() ?? 0);
                        if (existingEnrollmentsCount > 0)
                        {
                            invalidStudentIds.Add((studentId, new List<string> { $"Student with Id {studentId} is already enrolled in class with Id {classId}" }));
                            continue;
                        }

                        var insertCommand = new NpgsqlCommand(
                            """
                            INSERT INTO Enrollments (ClassId, StudentId, EnrollmentDate)
                            VALUES (@ClassId, @StudentId, @EnrollmentDate)
                            """, connection, transaction);

                        insertCommand.Parameters.AddWithValue("@ClassId", classId);
                        insertCommand.Parameters.AddWithValue("@StudentId", studentId);
                        insertCommand.Parameters.AddWithValue("@EnrollmentDate", DateTime.Now);

                        int queryResult = insertCommand.ExecuteNonQuery();
                        if (queryResult > 0)
                        {
                            addedStudentIds.Add(studentId);
                            addedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        invalidStudentIds.Add((studentId, new List<string> { "Exception raised when adding to database: " + ex.Message }));
                    }
                }
                transaction.Commit();
            }
            return (addedStudentIds, addedCount, invalidStudentIds);
        }

        public (
            IList<int> removedStudentIds,
            int removedCount,
            IList<(int studentId, IEnumerable<string> error)> invalidStudentIdsInfo
        ) RemoveStudentsFromClass(IEnumerable<int> studentIds, int classId)
        {
            List<int> removedStudentIds = [];
            int removedCount = 0;
            List<(int studentId, IEnumerable<string> error)> invalidStudentIds = [];

            using (var connection = GetConnection())
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // Check if the class exists once
                    var checkClassIdCommand = new NpgsqlCommand(
                        """
                        SELECT COUNT(*) FROM Classes
                        WHERE Id = @ClassId
                        """, connection, transaction);
                    checkClassIdCommand.Parameters.AddWithValue("@ClassId", classId);
                    int existingClassesCount = Convert.ToInt32(checkClassIdCommand.ExecuteScalar() ?? 0);

                    if (existingClassesCount == 0)
                    {
                        foreach (var studentId in studentIds)
                        {
                            invalidStudentIds.Add((studentId, new List<string> { $"Class with Id {classId} doesn't exist at the moment of removal" }));
                        }
                        return (removedStudentIds, removedCount, invalidStudentIds);
                    }
                }
                catch (Exception ex)
                {
                    foreach (var studentId in studentIds)
                    {
                        invalidStudentIds.Add((studentId, new List<string> { "Exception raised when checking class existence: " + ex.Message }));
                    }
                    return (removedStudentIds, removedCount, invalidStudentIds);
                }

                foreach (var studentId in studentIds)
                {
                    try
                    {
                        var checkStudentIdCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM Students
                            WHERE Id = @StudentId
                            """, connection, transaction);
                        checkStudentIdCommand.Parameters.AddWithValue("@StudentId", studentId);
                        int existingStudentsCount = Convert.ToInt32(checkStudentIdCommand.ExecuteScalar() ?? 0);

                        if (existingStudentsCount == 0)
                        {
                            invalidStudentIds.Add((studentId, new List<string> { $"Student with Id {studentId} doesn't exists at the moment of removal" }));
                            continue;
                        }

                        var checkEnrollmentCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM Enrollments
                            WHERE ClassId = @ClassId AND StudentId = @StudentId
                            """, connection, transaction);
                        checkEnrollmentCommand.Parameters.AddWithValue("@ClassId", classId);
                        checkEnrollmentCommand.Parameters.AddWithValue("@StudentId", studentId);
                        int existingEnrollmentsCount = Convert.ToInt32(checkEnrollmentCommand.ExecuteScalar() ?? 0);

                        if (existingEnrollmentsCount == 0)
                        {
                            invalidStudentIds.Add((studentId, new List<string> { $"Student with Id {studentId} is not enrolled in class with Id {classId}" }));
                            continue;
                        }

                        var deleteCommand = new NpgsqlCommand(
                            """
                            DELETE FROM Enrollments
                            WHERE ClassId = @ClassId AND StudentId = @StudentId
                            """, connection, transaction);
                        deleteCommand.Parameters.AddWithValue("@ClassId", classId);
                        deleteCommand.Parameters.AddWithValue("@StudentId", studentId);

                        int queryResult = deleteCommand.ExecuteNonQuery();
                        if (queryResult > 0)
                        {
                            removedStudentIds.Add(studentId);
                            removedCount++;
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        invalidStudentIds.Add((studentId, new List<string> { "Exception raised when removing from database: " + ex.Message }));
                    }
                }
                transaction.Commit();
            }

            return (removedStudentIds, removedCount, invalidStudentIds);
        }

        public (ObservableCollection<StudentVer2>, int) GetStudentsFromClass(
            int classId,
            bool fetchingAll = false,
            int ignoringCount = 0,
            int fetchingCount = 0,
            IEnumerable<SortCriteria> sortCriteria = null
        )
        {
            ObservableCollection<StudentVer2> students = [];
            int totalItems = 0;

            using (var connection = GetConnection())
            {
                connection.Open();

                var query =
                    """
                    SELECT s.Id, s.StudentCode, s.StudentName, s.EnrollmentYear, 
                           s.GraduationYear, s.Email, s.BirthDate, s.PhoneNo, s.UserId,
                           COUNT(*) OVER() as TotalItems
                    FROM Students s
                    JOIN Enrollments e ON s.Id = e.StudentId
                    WHERE e.ClassId = @ClassId
                    """;

                query += '\n';

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
                    query += $"ORDER BY {string.Join(", ", sortQuery)}\n";
                }

                if (!fetchingAll)
                {
                    query += "LIMIT @Take OFFSET @Skip\n";
                }

                var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassId", classId);

                if (!fetchingAll)
                {
                    command.Parameters.AddWithValue("@Take", fetchingCount);
                    command.Parameters.AddWithValue("@Skip", ignoringCount);
                }

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    totalItems = reader.GetInt32(9); // The TotalItems column

                    students.Add(new StudentVer2
                    {
                        Id = reader.GetInt32(0),
                        StudentCode = reader.GetString(1),
                        StudentName = reader.GetString(2),
                        EnrollmentYear = reader.GetInt32(3),
                        GraduationYear = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                        Email = reader.GetString(5),
                        BirthDate = reader.GetDateTime(6),
                        PhoneNo = reader.IsDBNull(7) ? null : reader.GetString(7),
                        UserId = reader.IsDBNull(8) ? null : reader.GetInt32(8)
                    });
                }
            }
            return (students, totalItems);
        }

        public (
            IList<int> addedTeacherIds,
            int addedCount,
            IList<(int teacherId, IEnumerable<string> error)> invalidTeacherIdsInfo
        ) AddTeachersToClass(IEnumerable<int> teacherIds, int classId)
        {
            List<int> addedTeacherIds = [];
            int addedCount = 0;
            List<(int teacherId, IEnumerable<string> error)> invalidTeacherIds = [];

            using (var connection = GetConnection())
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // Check if the class exists once
                    var checkClassIdCommand = new NpgsqlCommand(
                        """
                        SELECT COUNT(*) FROM Classes
                        WHERE Id = @ClassId
                        """, connection, transaction);
                    checkClassIdCommand.Parameters.AddWithValue("@ClassId", classId);
                    int existingClassesCount = Convert.ToInt32(checkClassIdCommand.ExecuteScalar() ?? 0);

                    if (existingClassesCount == 0)
                    {
                        foreach (var teacherId in teacherIds)
                        {
                            invalidTeacherIds.Add((teacherId, new List<string> { $"Class with Id {classId} doesn't exist at the moment of removal" }));
                        }
                        return (addedTeacherIds, addedCount, invalidTeacherIds);
                    }
                }
                catch (Exception ex)
                {
                    foreach (var teacherId in teacherIds)
                    {
                        invalidTeacherIds.Add((teacherId, new List<string> { "Exception raised when checking class existence: " + ex.Message }));
                    }
                    return (addedTeacherIds, addedCount, invalidTeacherIds);
                }


                foreach (var teacherId in teacherIds)
                {
                    try
                    {
                        var checkTeacherIdCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM Teachers
                            WHERE Id = @TeacherId
                            """, connection, transaction);
                        checkTeacherIdCommand.Parameters.AddWithValue("@TeacherId", teacherId);
                        int existingTeachersCount = Convert.ToInt32(checkTeacherIdCommand.ExecuteScalar() ?? 0);
                        if (existingTeachersCount == 0)
                        {
                            invalidTeacherIds.Add((teacherId, new List<string> { $"Teacher with Id {teacherId} doesn't exist at the moment of insertion" }));
                            continue;
                        }

                        var checkExistingAssignmentCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM TeachersPerClass
                            WHERE ClassId = @ClassId AND TeacherId = @TeacherId
                            """, connection, transaction);
                        checkExistingAssignmentCommand.Parameters.AddWithValue("@ClassId", classId);
                        checkExistingAssignmentCommand.Parameters.AddWithValue("@TeacherId", teacherId);
                        int existingAssignmentsCount = Convert.ToInt32(checkExistingAssignmentCommand.ExecuteScalar() ?? 0);
                        if (existingAssignmentsCount > 0)
                        {
                            invalidTeacherIds.Add((teacherId, new List<string> { $"Teacher with Id {teacherId} is already assigned to class with Id {classId}" }));
                            continue;
                        }

                        var insertCommand = new NpgsqlCommand(
                            """
                            INSERT INTO TeachersPerClass (ClassId, TeacherId)
                            VALUES (@ClassId, @TeacherId)
                            """, connection, transaction);

                        insertCommand.Parameters.AddWithValue("@ClassId", classId);
                        insertCommand.Parameters.AddWithValue("@TeacherId", teacherId);

                        int queryResult = insertCommand.ExecuteNonQuery();
                        if (queryResult > 0)
                        {
                            addedTeacherIds.Add(teacherId);
                            addedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        invalidTeacherIds.Add((teacherId, new List<string> { "Exception raised when adding to database: " + ex.Message }));
                    }
                }
                transaction.Commit();
            }
            return (addedTeacherIds, addedCount, invalidTeacherIds);
        }

        public (
            IList<int> removedTeacherIds,
            int removedCount,
            IList<(int teacherId, IEnumerable<string> error)> invalidTeacherIdsInfo
        ) RemoveTeachersFromClass(IEnumerable<int> teacherIds, int classId)
        {
            List<int> removedTeacherIds = [];
            int removedCount = 0;
            List<(int teacherId, IEnumerable<string> error)> invalidTeacherIds = [];

            using (var connection = GetConnection())
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // Check if the class exists once
                    var checkClassIdCommand = new NpgsqlCommand(
                        """
                        SELECT COUNT(*) FROM Classes
                        WHERE Id = @ClassId
                        """, connection, transaction);
                    checkClassIdCommand.Parameters.AddWithValue("@ClassId", classId);
                    int existingClassesCount = Convert.ToInt32(checkClassIdCommand.ExecuteScalar() ?? 0);

                    if (existingClassesCount == 0)
                    {
                        foreach (var teacherId in teacherIds)
                        {
                            invalidTeacherIds.Add((teacherId, new List<string> { $"Class with Id {classId} doesn't exist at the moment of removal" }));
                        }
                        return (removedTeacherIds, removedCount, invalidTeacherIds);
                    }
                }
                catch (Exception ex)
                {
                    foreach (var teacherId in teacherIds)
                    {
                        invalidTeacherIds.Add((teacherId, new List<string> { "Exception raised when checking class existence: " + ex.Message }));
                    }
                    return (removedTeacherIds, removedCount, invalidTeacherIds);
                }

                foreach (var teacherId in teacherIds)
                {
                    try
                    {
                        var checkTeacherIdCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM Teachers
                            WHERE Id = @TeacherId
                            """, connection, transaction);
                        checkTeacherIdCommand.Parameters.AddWithValue("@TeacherId", teacherId);
                        int existingTeachersCount = Convert.ToInt32(checkTeacherIdCommand.ExecuteScalar() ?? 0);

                        if (existingTeachersCount == 0)
                        {
                            invalidTeacherIds.Add((teacherId, new List<string> { $"Teacher with Id {teacherId} doesn't exist at the moment of removal" }));
                            continue;
                        }

                        var checkAssignmentCommand = new NpgsqlCommand(
                            """
                            SELECT COUNT(*) FROM TeachersPerClass
                            WHERE ClassId = @ClassId AND TeacherId = @TeacherId
                            """, connection, transaction);
                        checkAssignmentCommand.Parameters.AddWithValue("@ClassId", classId);
                        checkAssignmentCommand.Parameters.AddWithValue("@TeacherId", teacherId);
                        int existingAssignmentsCount = Convert.ToInt32(checkAssignmentCommand.ExecuteScalar() ?? 0);

                        if (existingAssignmentsCount == 0)
                        {
                            invalidTeacherIds.Add((teacherId, new List<string> { $"Teacher with Id {teacherId} is not assigned to class with Id {classId}" }));
                            continue;
                        }

                        var deleteCommand = new NpgsqlCommand(
                            """
                            DELETE FROM TeachersPerClass
                            WHERE ClassId = @ClassId AND TeacherId = @TeacherId
                            """, connection, transaction);
                        deleteCommand.Parameters.AddWithValue("@ClassId", classId);
                        deleteCommand.Parameters.AddWithValue("@TeacherId", teacherId);

                        int queryResult = deleteCommand.ExecuteNonQuery();
                        if (queryResult > 0)
                        {
                            removedTeacherIds.Add(teacherId);
                            removedCount++;
                        }

                        
                    }
                    catch (Exception ex)
                    {
                        invalidTeacherIds.Add((teacherId, new List<string> { "Exception raised when removing from database: " + ex.Message }));
                    }
                }
                transaction.Commit();
            }
            return (removedTeacherIds, removedCount, invalidTeacherIds);
        }

        public (ObservableCollection<Teacher>, int) GetTeachersFromClass(
            int classId,
            bool fetchingAll = false,
            int ignoringCount = 0,
            int fetchingCount = 0,
            IEnumerable<SortCriteria> sortCriteria = null
        )
        {
            ObservableCollection<Teacher> teachers = [];
            int totalItems = 0;

            using (var connection = GetConnection())
            {
                connection.Open();

                var query =
                    """
                    SELECT t.Id, t.TeacherCode, t.TeacherName, t.Email, t.PhoneNo, t.UserId,
                           COUNT(*) OVER() as TotalItems
                    FROM Teachers t
                    JOIN TeachersPerClass tc ON t.Id = tc.TeacherId
                    WHERE tc.ClassId = @ClassId
                    """;

                query += '\n';

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
                    query += $"ORDER BY {string.Join(", ", sortQuery)}\n";
                }

                if (!fetchingAll)
                {
                    query += "LIMIT @Take OFFSET @Skip\n";
                }

                var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassId", classId);

                if (!fetchingAll)
                {
                    command.Parameters.AddWithValue("@Take", fetchingCount);
                    command.Parameters.AddWithValue("@Skip", ignoringCount);
                }

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    totalItems = reader.GetInt32(6); // The TotalItems column

                    teachers.Add(new Teacher
                    {
                        Id = reader.GetInt32(0),
                        TeacherCode = reader.GetString(1),
                        TeacherName = reader.GetString(2),
                        Email = reader.GetString(3),
                        PhoneNo = reader.IsDBNull(4) ? null : reader.GetString(4),
                        UserId = reader.IsDBNull(5) ? null : reader.GetInt32(5)
                    });
                }
            }
            return (teachers, totalItems);
        }
    }
}
