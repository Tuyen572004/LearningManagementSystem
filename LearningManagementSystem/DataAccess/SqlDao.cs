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
            password = "matkhaugitutim";
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
            if (this.OpenConnection() == true)
            {
                var sql = "update courses set CourseCode=@CourseCode, CourseDescription=@CourseDescription, DepartmentId=@DepartmentId where Id=@Id";
                var command = new MySqlCommand(sql, connection);

                command.Parameters.Add("@CourseCode", MySqlDbType.String).Value = course.CourseCode;
                command.Parameters.Add("@CourseDescription", MySqlDbType.String).Value = course.CourseDescription;
                command.Parameters.Add("@DepartmentId", MySqlDbType.Int32).Value = course.DepartmentId;
                command.Parameters.Add("@Id", MySqlDbType.Int32).Value = course.Id;

                int count = command.ExecuteNonQuery();

                this.CloseConnection();
            }
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

        public int CountCourse()
        {
            if (this.OpenConnection() == true)
            {
                var sql = "select count(*) as TotalItems from courses";
                var command = new MySqlCommand(sql, connection);

                var reader = command.ExecuteReader();

                reader.Read();

                int result = reader.GetInt32("TotalItems");
                

                this.CloseConnection();
                return result;
            }
            else return 0;
        }

        public int CountDepartments()
        {
            if (this.OpenConnection() == true)
            {
                var sql = "select count(*) as TotalItems from departments";
                var command = new MySqlCommand(sql, connection);

                var reader = command.ExecuteReader();

                reader.Read();

                int result = reader.GetInt32("TotalItems");


                this.CloseConnection();
                return result;
            }
            else return 0;
        }

        public bool CheckUserInfo(User user)
        {
            if (this.OpenConnection() == true)
            {
                var sql = "select count(*) as TotalItems from users where Username=@username and PasswordHash=@passwordhash";
                var command = new MySqlCommand(sql, connection);

                command.Parameters.Add("@username", MySqlDbType.String).Value = user.Username;
                command.Parameters.Add("@passwordhash", MySqlDbType.String).Value = user.PasswordHash;

                //String commandtext = command.CommandText;
                //foreach (MySql.Data.MySqlClient.MySqlParameter p in command.Parameters)
                //{
                //    commandtext = commandtext.Replace("@username", user.Username);
                //    commandtext = commandtext.Replace("@passwordhash", '%'+user.PasswordHash);
                //}
                var reader = command.ExecuteReader();

                reader.Read();

                int result = reader.GetInt32("TotalItems");


                this.CloseConnection();
                return result == 1;
            }
            else return false;
        }

        public bool IsExistsUsername(string username)
        {
            if (this.OpenConnection() == true)
            {
                var sql = "select count(*) as TotalItems from users where Username=@username";

                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@username", MySqlDbType.String).Value = username;
                var reader = command.ExecuteReader();

                reader.Read();

                int result = reader.GetInt32("TotalItems");


                this.CloseConnection();
                return result == 1;
            }
            else return true;
        }

        public bool AddUser(User user)
        {
            if (this.OpenConnection() == true)
            {
                var sql = """
                    insert into users (Username,PasswordHash,Email,users.Role,CreatedAt)
                    values (@username,@passwordhash,null,@role,null);
                    """;

                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@username", MySqlDbType.String).Value = user.Username;
                command.Parameters.Add("@passwordhash", MySqlDbType.String).Value = user.PasswordHash;
                command.Parameters.Add("@role", MySqlDbType.Int32).Value = user.Role;

                int id = Convert.ToInt32(command.ExecuteScalar());
                user.Id = id;

                this.CloseConnection();

                return id > 0 ? true : false;
            }
            this.CloseConnection();
            return false;
        }
    
    }
}
