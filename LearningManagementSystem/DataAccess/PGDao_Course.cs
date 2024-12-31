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
        /// <summary>
        /// Finds the course by its course code and sets the course ID.
        /// </summary>
        /// <param name="course">The course object containing the course code.</param>
        /// <returns>The ID of the course.</returns>
        public int FindCourseByID(Course course)
        {
            var sql = "SELECT Id FROM Courses WHERE CourseCode=@CourseCode";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@CourseCode", course.CourseCode);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    course.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                }
                return course.Id;
            }
        }
        /// <summary>
        /// Finds the total number of classes for a given course ID.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <returns>The total number of classes.</returns>
        public int findTotalClassesByCourseId(int courseId)
        {
            var sql = "SELECT COUNT(*) FROM classes WHERE courseid=@CourseId";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@CourseId", courseId);
                connection.Open();
                var result = (long)command.ExecuteScalar();
                return (int)result;
            }
        }
        /// <summary>
        /// Finds the total number of students enrolled in a given course ID.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <returns>The total number of students.</returns>
        public int findTotalStudentsByCourseId(int courseId)
        {
            var sql = "SELECT COUNT(*) FROM enrollments e JOIN classes c ON e.classid = c.id WHERE c.courseid=@CourseId";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@CourseId", courseId);
                connection.Open();
                var result = (long)command.ExecuteScalar();
                return (int)result;
            }
        }
        /// <summary>
        /// Finds the course associated with a given class ID.
        /// </summary>
        /// <param name="classId">The ID of the class.</param>
        /// <returns>The course associated with the class ID.</returns>
        public Course findCourseByClassId(int classId)
        {
            var result = new Course();
            var sql = """
                SELECT c.Id, c.CourseCode, c.CourseDescription, c.DepartmentId
                FROM Courses c
                JOIN Classes cl ON c.Id = cl.CourseId
                WHERE cl.Id=@classId
                """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@classId", classId);
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result = new Course
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        CourseCode = reader.GetString(reader.GetOrdinal("CourseCode")),
                        CourseDescription = reader.GetString(reader.GetOrdinal("CourseDescription")),
                        DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the course by its ID.
        /// </summary>
        /// <param name="courseId">The ID of the course.</param>
        /// <returns>The course with the specified ID.</returns>
        public Course GetCourseById(int courseId)
        {
            var result = new Course();
            var sql = "SELECT Id, CourseCode, CourseDescription, DepartmentId FROM Courses WHERE Id=@Id";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", courseId);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = new Course
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        CourseCode = reader.GetString(reader.GetOrdinal("CourseCode")),
                        CourseDescription = reader.GetString(reader.GetOrdinal("CourseDescription")),
                        DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves all course descriptions from the database.
        /// </summary>
        /// <returns>A list of course descriptions.</returns>
        public List<string> GetAllCourseDecriptions()
        {
            var result = new List<string>();
            var sql = "SELECT Coursedescription FROM Courses";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.GetString(reader.GetOrdinal("coursedescription")));
                }
            }
            return result;
        }


        /// <summary>
        /// Retrieves a paginated list of courses from the database.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of courses per page.</param>
        /// <param name="keyword">The keyword to filter courses by description.</param>
        /// <param name="sortBy">The column to sort by.</param>
        /// <param name="sortOrder">The sort order (ASC or DESC).</param>
        /// <returns>A tuple containing the total number of courses and a list of courses.</returns>
        public Tuple<int, List<Course>> GetAllCourses(int page = 1, int pageSize = 10, string keyword = "", string sortBy = "Id", string sortOrder = "ASC")
        {
            var result = new List<Course>();
            var sql = $"""
                SELECT COUNT(*) OVER() AS TotalItems, Id, CourseCode, CourseDescription, DepartmentId
                FROM Courses
                WHERE CourseDescription LIKE @Keyword
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
                    result.Add(new Course
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        CourseCode = reader.GetString(reader.GetOrdinal("CourseCode")),
                        CourseDescription = reader.GetString(reader.GetOrdinal("CourseDescription")),
                        DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                    });
                }
                return new Tuple<int, List<Course>>(totalItems, result);
            }
        }

        /// <summary>
        /// Inserts a new course into the database.
        /// </summary>
        /// <param name="course">The course to insert.</param>
        /// <returns>The ID of the inserted course.</returns>
        public int InsertCourse(Course course)
        {
            var sql = """
                INSERT INTO Courses (CourseCode, CourseDescription, DepartmentId)
                VALUES (@CourseCode, @CourseDescription, @DepartmentId)
                RETURNING Id;
                """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@CourseCode", course.CourseCode);
                command.Parameters.AddWithValue("@CourseDescription", course.CourseDescription ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DepartmentId", course.DepartmentId);

                connection.Open();
                int id = (int)command.ExecuteScalar();
                course.Id = id;

                return id > 0 ? 1 : -1;
            }
        }

        /// <summary>
        /// Removes a course from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the course to remove.</param>
        /// <returns>0 if the course was successfully removed, 1 if an error occurred.</returns>
        public int RemoveCourseByID(int id)
        {
            var sql = "DELETE FROM Courses WHERE Id=@Id";
            try
            {
                using (var connection = GetConnection())
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return 1;
                throw;
            }
            return 0;
        }

        /// <summary>
        /// Updates the course details in the database.
        /// </summary>
        /// <param name="course">The course object containing updated details.</param>
        public void UpdateCourse(Course course)
        {
            var sql = "UPDATE Courses SET CourseCode=@CourseCode, CourseDescription=@CourseDescription, DepartmentId=@DepartmentId WHERE Id=@Id";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@CourseCode", course.CourseCode);
                command.Parameters.AddWithValue("@CourseDescription", course.CourseDescription ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DepartmentId", course.DepartmentId);
                command.Parameters.AddWithValue("@Id", course.Id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Counts the total number of courses in the database.
        /// </summary>
        /// <returns>The total number of courses.</returns>
        public int CountCourse()
        {
            var sql = "SELECT COUNT(*) AS TotalItems FROM Courses";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                reader.Read();
                int result = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                return result;
            }
        }
    }
}
