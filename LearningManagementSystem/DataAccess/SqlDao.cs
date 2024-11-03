using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml;
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
            //server = "localhost";
            //database = "LMSdb";
            //username = "root";
            //password = "nopassword";
            //string connectionString = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={password}";
            //connection = new MySqlConnection(connectionString);

            server = "localhost";
            database = "learning_management_system";
            username = "root";
            password = "LMSMySqlServer@123";
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
                var sql = "update Courses set CourseCode=@CourseCode, CourseDescription=@CourseDescription, DepartmentId=@DepartmentId where Id=@Id";
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
                    from Departments
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
                var sql = "select count(*) as TotalItems from Courses";
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
                var sql = "select count(*) as TotalItems from Departments";
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
                var sql = "select count(*) as TotalItems from Users where Username=@username and PasswordHash=@passwordhash";
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
                var sql = "select count(*) as TotalItems from Users where Username=@username";

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
                    insert into Users (Username,PasswordHash,Email,users.Role,CreatedAt)
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

        public ObservableCollection<StudentVer2> GetStudentsByClassId(int classId)
        {
            try
            {
                bool connectionState = OpenConnection();
                if (connectionState)
                {
                    Console.WriteLine("Connection is open");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            CloseConnection();
            return [];
        }

        public (ObservableCollection<StudentVer2>, int) GetStudentsById(
            int ignoringCount = 0,
            int fetchingCount = 0,
            IEnumerable<int> chosenIds = null
            )
        {
            ObservableCollection<StudentVer2> result = [];
            int queryCount = 0;
            if (OpenConnection() == true)
            {
                var Query = """
                select count(*) over() as TotalItem, Id, StudentCode, StudentName, Email, BirthDate, PhoneNo, UserId, EnrollmentYear, GraduationYear
                from Students
                """;

                if (chosenIds is not null && chosenIds.Any())
                {
                    Query += $"\nwhere Id in ({string.Join(", ", chosenIds)})";
                }

                Query += "\nlimit @Take offset @Skip";

                var Command = new MySqlCommand(Query, connection);
                Command.Parameters.Add("@Skip", MySqlDbType.Int32).Value = ignoringCount;
                Command.Parameters.Add("@Take", MySqlDbType.Int32).Value = fetchingCount;

                var finalquery = Query;

                var QueryResultReader = Command.ExecuteReader();

                bool isTotalItemFetched = false;

                while (QueryResultReader.Read())
                {
                    if (!isTotalItemFetched)
                    {
                        queryCount = QueryResultReader.GetInt32("TotalItem");
                        isTotalItemFetched = true;
                    }

                    int graduationYearColumn = QueryResultReader.GetOrdinal("GraduationYear");
                    int userIdColumn = QueryResultReader.GetOrdinal("UserId");
                    StudentVer2 newStudent = new()
                    {
                        Id = QueryResultReader.GetInt32("Id"),
                        StudentCode = QueryResultReader.GetString("StudentCode"),
                        StudentName = QueryResultReader.GetString("StudentName"),
                        Email = QueryResultReader.GetString("Email"),
                        BirthDate = QueryResultReader.GetDateTime("BirthDate"),
                        PhoneNo = QueryResultReader.GetString("PhoneNo"),
                        UserId = (QueryResultReader.IsDBNull(userIdColumn)
                            ? null
                            : QueryResultReader.GetInt32("UserId")
                            ),
                        EnrollmentYear = QueryResultReader.GetInt32("EnrollmentYear"),
                        GraduationYear = (QueryResultReader.IsDBNull(graduationYearColumn)
                            ? null
                            : QueryResultReader.GetInt32("GraduationYear")
                            )
                    };
                     
                    result.Add(newStudent);
                };
            }
            CloseConnection();
            return (result, queryCount);
        }


        public (ObservableCollection<StudentVer2>, int) GetStudents(
            bool fetchingAll = false,
            int ignoringCount = 0,
            int fetchingCount = 0,
            List<(StudentField field, Ordering order)> sortCriteria = null,
            List<(StudentField field, object keyword)> searchKeyword = null,
            List<(StudentField field, object leftBound, object rightBound, bool containedLeftBound, bool withinBounds, bool containedRightBound)> filterCriteria = null
        )
        {
            ObservableCollection<StudentVer2> result = [];
            int totalItem = 0;
            //if (OpenConnection())
            //{
            //    var sql = """
            //        select count(*) over() as TotalItem, Id, StudentCode, StudentName, Email, Birthday, PhoneNo, UserId
            //        from Students
            //        """;

            //    bool startingWhere = false;
            //    var whereFormat = isStarting =>
            //    {
            //        if (isStarting)
            //        {
            //            return "where {0}\n";
            //        }
            //        else
            //        {
            //            return "    and {0}\n";
            //        }
            //    };

            //    string temp;
            //    foreach ((StudentField field, object keyword) in searchKeyword) {
            //        var criteriaType = field.InferType();
            //        switch (criteriaType)
            //        {
            //            case typeof(int):
            //                temp = field.ToSqlAttributeName() + " = " + (keyword as int) as string;
            //                sql = sql + string.Format(whereFormat(startingWhere), temp);
            //                break;
            //            case typeof(string):
            //                temp = field.ToSqlAttributeName() + " like \"" + (keyword as string) + "\"%";
            //                sql = sql + string.Format(whereFormat(startingWhere), temp);
            //                break;
            //            case typeof(DateTime):
            //                temp = field.ToSqlAttributeName() + " = \"" + ((keyword as DateTime) as Date).ToString() + "\"";
            //                sql = sql + string.Format(whereFormat(startingWhere), temp);
            //                break;
            //            default:
            //                throw Exception("Invalid field type found!");
            //        }
            //        startingWhere = false;
            //    }

            //    foreach (
            //        (
            //            StudentField field, object leftBound, object rightBound,
            //            bool containedLeftBound, bool withinBounds, bool containedRightBound
            //        ) in filterCriteria)
            //    {
            //        var criteriaType = field.InferType();
            //        switch (criteriaType)
            //        {
            //            case typeof(int):
            //                temp = field.ToSqlAttributeName() + " = " + (keyword as int) as string;
            //                sql = sql + string.Format(whereFormat(startingWhere), temp);
            //                break;
            //            case typeof(string):
            //                temp = field.ToSqlAttributeName() + " like \"" + (keyword as string) + "\"%";
            //                sql = sql + string.Format(whereFormat(startingWhere), temp);
            //                break;
            //            case typeof(DateTime):
            //                temp = field.ToSqlAttributeName() + " = \"" + ((keyword as DateTime) as Date).ToString() + "\"";
            //                sql = sql + string.Format(whereFormat(startingWhere), temp);
            //                break;
            //            default:
            //                throw Exception("Invalid field type found!");
            //        }
            //        startingWhere = false;
            //    }

            //}
            //CloseConnection();
            return (result, totalItem);
        }
    }
}
