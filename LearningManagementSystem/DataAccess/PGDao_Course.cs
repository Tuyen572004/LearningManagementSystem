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

        public Course findCourseByClassId(int classId)
        {
            var result = new Course();
            if (this.OpenConnection())
            {
                var sql = """
                    SELECT c.Id, c.CourseCode, c.CourseDescription, c.DepartmentId
                    FROM Courses c
                    JOIN Classes cl ON c.Id = cl.CourseId
                    WHERE cl.Id=@classId
                    """;

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@classId", classId);
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

                this.CloseConnection();
            }
            return result;
        }

        public Course GetCourseById(int courseId)
        {
            Random random = new Random();
            courseId = random.Next(1, 20);
            if (courseId % 8 == 1)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE101",
                    CourseDescription = "Introduction to Computer Science",
                    DepartmentId = 1
                };
            }
            if (courseId % 8 == 2)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE102",
                    CourseDescription = "Data Structures and Algorithms",
                    DepartmentId = 1
                };
            }
            if (courseId % 8 == 3)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE103",
                    CourseDescription = "Operating Systems",
                    DepartmentId = 1
                };
            }
            if (courseId % 8 == 4)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE104",
                    CourseDescription = "Computer Networks",
                    DepartmentId = 1
                };
            }
            if (courseId % 8 == 5)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE105",
                    CourseDescription = "Database Management Systems",
                    DepartmentId = 1
                };
            }
            if (courseId % 8 == 6)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE106",
                    CourseDescription = "Software Engineering",
                    DepartmentId = 1
                };
            }
            if (courseId % 8 == 7)
            {
                return new Course
                {
                    Id = courseId,
                    CourseCode = "CSE107",
                    CourseDescription = "Web Development",
                    DepartmentId = 1
                };
            }

            return new Course
            {
                Id = courseId,
                CourseCode = "CSE108",
                CourseDescription = "Computer Graphics",
                DepartmentId = 1
            };
        } // MOCK

        public List<string> GetAllCourseDecriptions()
        {
            var result = new List<string>();
            try
            {
                if (this.OpenConnection())
                {
                    var sql = "SELECT Coursedescription FROM Courses";
                    var command = new NpgsqlCommand(sql, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        result.Add(reader.GetString(reader.GetOrdinal("coursedescription")));
                    }
                    if (result.Count > 0)
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    this.CloseConnection();
                }
            }
            return result;
        }

        public void DeleteCourse(int courseId)
        {
            throw new NotImplementedException();
        }

        public Tuple<int, List<Course>> GetAllCourses(int page = 1, int pageSize = 10, string keyword = "", string sortBy = "Id", string sortOrder = "ASC")
        {
            var result = new List<Course>();

            if (this.OpenConnection())
            {
                var sql = $"""
                    SELECT COUNT(*) OVER() AS TotalItems, Id, CourseCode, CourseDescription, DepartmentId
                    FROM Courses
                    WHERE CourseDescription LIKE @Keyword
                    ORDER BY {sortBy} {sortOrder}
                    LIMIT @Take OFFSET @Skip
                    """;

                var skip = (page - 1) * pageSize;
                var take = pageSize;

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Skip", skip);
                command.Parameters.AddWithValue("@Take", take);
                command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

                var reader = command.ExecuteReader();

                // var fullCommand = GetFullCommandText(command);

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

                this.CloseConnection();
                return new Tuple<int, List<Course>>(totalItems, result);
            }
            else
            {
                this.CloseConnection();
                return new Tuple<int, List<Course>>(-1, null);
            }
        }

        public int InsertCourse(Course course)
        {
            if (this.OpenConnection())
            {
                var sql = """
                    INSERT INTO Courses (CourseCode, CourseDescription, DepartmentId)
                    VALUES (@CourseCode, @CourseDescription, @DepartmentId)
                    RETURNING Id;
                    """;

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@CourseCode", course.CourseCode);
                command.Parameters.AddWithValue("@CourseDescription", course.CourseDescription ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DepartmentId", course.DepartmentId);

                int id = (int)command.ExecuteScalar();
                course.Id = id;

                this.CloseConnection();

                return id > 0 ? 1 : -1;
            }
            this.CloseConnection();
            return -1;
        }

        public void RemoveCourseByID(int id)
        {
            if (this.OpenConnection())
            {
                var sql = "DELETE FROM Courses WHERE Id=@Id";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                command.ExecuteNonQuery();

                this.CloseConnection();
            }
        }

        public void UpdateCourse(Course course)
        {
            if (this.OpenConnection())
            {
                var sql = "UPDATE Courses SET CourseCode=@CourseCode, CourseDescription=@CourseDescription, DepartmentId=@DepartmentId WHERE Id=@Id";
                var command = new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue("@CourseCode", course.CourseCode);
                command.Parameters.AddWithValue("@CourseDescription", course.CourseDescription ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DepartmentId", course.DepartmentId);
                command.Parameters.AddWithValue("@Id", course.Id);

                command.ExecuteNonQuery();

                this.CloseConnection();
            }
        }

        public int CountCourse()
        {
            if (this.OpenConnection())
            {
                var sql = "SELECT COUNT(*) AS TotalItems FROM Courses";
                var command = new NpgsqlCommand(sql, connection);

                var reader = command.ExecuteReader();

                reader.Read();

                int result = reader.GetInt32(reader.GetOrdinal("TotalItems"));

                this.CloseConnection();
                return result;
            }
            else return 0;
        }

    }
}
