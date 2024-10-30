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
        
        public MySqlConnection connection;
        public string server { get; set; }
        public string database { get; set; }
        public string username { get; set; }
        public string password{ get; set; }
        public SqlDao()
        {
            Initialize();
        }

        public void Initialize()
        {
            server = "localhost";
            database = "LMSdb";
            username = "root";
            password = "nopassword";
            string connectionString = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={password}";
            connection = new MySqlConnection(connectionString);
        }

        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server. Contact administrator");
                        break;
                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public void DeleteCourse(int courseId)
        {
            throw new NotImplementedException();
        }

        public Tuple<int, List<Course>> GetAllCourses(int page = 1, int pageSize = 10, string keyword = "", bool nameAscending = false)
        {
            var result=new List<Course>();

            if (this.OpenConnection() == true)
            {
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
            if (this.OpenConnection() == true)
            {
                var sql = """
                    insert into Courses (CourseCode, CourseDescription, DepartmentId)
                    values (@CourseCode, @CourseDescription, @DepartmentId);
                    SELECT LAST_INSERT_ID();
                    """;

                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@CourseCode", MySqlDbType.String).Value = course.CourseCode;
                command.Parameters.Add("@CourseDescription", MySqlDbType.String).Value = course.CourseDescription;
                command.Parameters.Add("@DepartmentId", MySqlDbType.Int32).Value = course.DepartmentId;

                int id = Convert.ToInt32(command.ExecuteScalar());
                course.Id = id;

                this.CloseConnection();

                return id > 0 ? 1 : -1;
            }
            this.CloseConnection();
            return -1;
        }

        public void RemoveCourseByID(int id)
        {
            if (this.OpenConnection()==true)
            {
                var sql = "delete from Courses where Id=@Id";
                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@Id", MySqlDbType.Int32).Value = id;

                int count = command.ExecuteNonQuery();

                this.CloseConnection();
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

        public Tuple<int, List<Department>> GetAllDepartments(int page = 1, int pageSize = 10, string keyword = "", bool nameAscending = false)
        {
            var result = new List<Department>();

            if (this.OpenConnection() == true)
            {
                var sql = """
                    select count(*) over() as TotalItems, Id, DepartmentCode, DepartmentDesc
                    from departments
                    where DepartmentCode like @Keyword
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
                    result.Add(new Department
                    {
                        Id = reader.GetInt32("Id"),
                        DepartmentCode = reader.GetString("DepartmentCode"),
                        DepartmentDesc = reader.GetString("DepartmentDesc"),
                    });
                }

                this.CloseConnection();
                return new Tuple<int, List<Department>>(totalItems, result);
            }
            else
            {
                this.CloseConnection();
                return new Tuple<int, List<Department>>(-1, null);
            }
        }

        public int InsertDepartment(Department department)
        {
            throw new NotImplementedException();
        }

        public void UpdateDepartment(Department department)
        {
            throw new NotImplementedException();
        }

        public void RemoveDepartmentByID(int departmentId)
        {
            throw new NotImplementedException();
        }

        public int FindDepartmentID(Department department)
        {
            if (this.OpenConnection() == true)
            {
                var sql = "select Id from Departments where DepartmentCode=@DepartmentCode";
                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@DepartmentCode", MySqlDbType.String).Value = department.DepartmentCode;

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    department.Id = reader.GetInt32("Id");
                }

                this.CloseConnection();
                return department.Id;
            }
            else return -1;
        }
    }
    public class DBConnection
    {
        private DBConnection() { }

        private static DBConnection _instance = null;
        private MySqlConnection _connection = null;

        public static DBConnection Instance()
        {
            if (_instance == null)
            {
                _instance = new DBConnection();
            }
            return _instance;
        }

        public string Server { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public MySqlConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    string connString = $"Server={Server}; database={DatabaseName}; UID={Username}; password={Password}";
                    _connection = new MySqlConnection(connString);
                }
                return _connection;
            }
        }

        public bool IsConnect()
        {
            if (Connection.State == System.Data.ConnectionState.Closed)
            {
                Connection.Open();
            }
            return Connection.State == System.Data.ConnectionState.Open;
        }

        public void Close()
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
        }
    }
}
