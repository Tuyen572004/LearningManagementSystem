using LearningManagementSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace LearningManagementSystem.DataAccess
{
    public class SqlDao : IDao
    {
        public static Tuple<bool, MySqlConnection> OpenConnection()
        {
            var dbCon = DBConnection.Instance();
            dbCon.Server = "localhost";
            dbCon.DatabaseName = "LMSdb";
            dbCon.Username = "root";
            dbCon.Password = "nopassword";

            return new Tuple<bool, MySqlConnection> (dbCon.IsConnect(), dbCon.Connection);
        }
        public SqlDao() { }
        public void DeleteCourse(int courseId)
        {
            throw new NotImplementedException();
        }

        public Tuple<int, List<Course>> GetAllCourses(int page = 1, int pageSize = 10, string keyword = "", bool nameAscending = false)
        {
            var result=new List<Course>();

            var checkConnection = OpenConnection();
            if (checkConnection.Item1 == true)
            {
                var connection = checkConnection.Item2;
                var sql = """
                    select count(*) over() as TotalItems, Id, CourseCode, CourseDescription, DepartmentId
                    from Courses
                    where CourseCode like @Keyword
                    order by Id asc
                    limit @Take offset @Skip
                    """;

                var skip = (page - 1) * pageSize;
                var take = pageSize;

                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@Skip", MySqlDbType.Int32).Value = skip;
                command.Parameters.Add("@Take", MySqlDbType.Int32).Value = take;
                command.Parameters.Add("@Keyword", MySqlDbType.String).Value = $"%{keyword}%";

                var reader = command.ExecuteReader();

                int totalItems = -1;
                while (reader.Read())
                {
                    if (totalItems == -1)
                    {
                        totalItems = reader.GetInt32("TotalItems");
                    }
                    result.Add(new Course
                    {
                        Id = reader.GetInt32("Id"),
                        CourseCode = reader.GetString("CourseCode"),
                        CourseDescription = reader.GetString("CourseDescription"),
                        DepartmentId = reader.GetInt32("DepartmentId")
                    });
                }

                connection.Close();
                return new Tuple<int, List<Course>>(totalItems, result);
            }
            else return null;
        }

        public int InsertCourse(Course course)
        {
            var checkConnection = OpenConnection();
            if (checkConnection.Item1 == true)
            {
                var connection = checkConnection.Item2;
                var sql = """
                    insert into Courses (CourseCode, CourseDescription, DepartmentId)
                    values (@CourseCode, @CourseDescription, @DepartmentId)
                    """;

                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@CourseCode", MySqlDbType.String).Value = course.CourseCode;
                command.Parameters.Add("@CourseDescription", MySqlDbType.String).Value = course.CourseDescription;
                command.Parameters.Add("@DepartmentId", MySqlDbType.Int32).Value = course.DepartmentId;

                int id = (int)(decimal)command.ExecuteScalar();
                course.Id = id;

                int count= command.ExecuteNonQuery();

                connection.Close();

                return count;

            }
            return -1;
        }

        public void RemoveCourseById(string id)
        {
            var checkConnection = OpenConnection();
            if (checkConnection.Item1 == true)
            {
                var connection = checkConnection.Item2;

                var sql= "delete from Courses where Id=@Id";
                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@Id", MySqlDbType.Int32).Value = id;

                int count = command.ExecuteNonQuery();

                connection.Close();
            }
        }

            public void UpdateCourse(Course course)
        {
            throw new NotImplementedException();
        }


        public Course GetCourseById(int courseId)
        {
            throw new NotImplementedException();
        }

        public Department GetDepartmentById(int departmentId)
        {
            throw new NotImplementedException();
        }

        // --------------------------------------------- //
        public ObservableCollection<Class> GetEnrolledClassesByStudentId(int studentId)
        {
            throw new NotImplementedException();
        }

        public Teacher GetTeacherById(int teacherId)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<Teacher> GetTeachersByClassId(int classId)
        {
            throw new NotImplementedException();
        }

  
    }
}
