using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;




namespace LearningManagementSystem.DataAccess
{
    public class DatabaseConfig
    {
        public string DB_SERVER { get; set; }
        public string DB_DATABASE { get; set; }
        public string DB_USERNAME { get; set; }
        public string DB_PASSWORD { get; set; }
    }
    public class SqlDao : IDao
    {

        public MySqlConnection connection;

        //public NpgsqlConnection connection;

        public string server { get; set; }
        public string database { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public SqlDao()
        {
            Initialize();
        }

        public void Initialize()
        {

            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            string configContent = File.ReadAllText(configFilePath);
            var configJson = JsonDocument.Parse(configContent);
            var databaseConfig = configJson.RootElement.GetProperty("Database").ToString();
            DatabaseConfig config = JsonSerializer.Deserialize<DatabaseConfig>(databaseConfig);

            server = config.DB_SERVER;
            database = config.DB_DATABASE;
            username = config.DB_USERNAME;
            password = config.DB_PASSWORD;

            // Use the credentials to create a connection string or connect to the database
            string connectionString = $"SERVER={server};DATABASE={database};UID={username};PASSWORD={password}";


            connection = new MySqlConnection(connectionString);


            //var Host = "postgresql://posgre:sOL87JmxSGdTuGCwXDNNc8ehNbyctMVH@dpg-ct3vdm52ng1s73a13mr0-a/lmsdb_7mch";
            //var Username = "posgre";
            //var Password = "s0L87JmxSGdTuGCwXDNNc8ehNbyctMVH";
            //var Database = "lmsdb_7mch";

            //var connectionString = $"Host={Host};Username={Username};Password={Password};Database={Database}";

            //connection = new NpgsqlConnection(connectionString);

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
            var result = new List<Course>();

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
            if (this.OpenConnection() == true)
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


        // ---------------------------  DEPARTMENT------------------ //

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

        public Course findCourseByClassId(int classId)
        {
            var result = new Course();
            if (this.OpenConnection() == true)
            {
                var sql = """
                    select c.Id, c.CourseCode, c.CourseDescription, c.DepartmentId
                    from Courses c
                    join Classes cl on c.Id = cl.CourseId
                    where cl.Id=@classId
                    """;

                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@classId", MySqlDbType.Int32).Value = classId;
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result = new Course
                    {
                        Id = reader.GetInt32("Id"),
                        CourseCode = reader.GetString("CourseCode"),
                        CourseDescription = reader.GetString("CourseDescription"),
                        DepartmentId = reader.GetInt32("DepartmentId")
                    };
                }

                this.CloseConnection();
            }
            return result;
        }

        // --------------------------------- CLASS -----------------------------

        public Class findClassById(int classId)
        {
            var result = new Class();
            if (this.OpenConnection() == true)
            {
                var sql = """
                    select Id, CourseId, ClassCode, CycleId, ClassStartDate, ClassEndDate
                    from Classes
                    where id=@classId
                    """;

                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@ClassId", MySqlDbType.Int32).Value = classId;
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result = new Class
                    {
                        Id = reader.GetInt32("Id"),
                        CourseId = reader.GetInt32("CourseId"),
                        ClassCode = reader.GetString("ClassCode"),
                        CycleId = reader.GetInt32("CycleId"),
                        ClassStartDate = reader.GetDateTime("ClassStartDate"),
                        ClassEndDate = reader.GetDateTime("ClassEndDate")
                    };
                }

                this.CloseConnection();
            }
            return result;
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

        // --------------------------- RESOURCE --------------------------- //
        public List<ResourceCategory> findAllResourceCategories()
        {
            var result = new List<ResourceCategory>();

            if (this.OpenConnection() == true)
            {
                var sql = """
                    select Id, Name, Summary
                    from ResourceCategories
                    """;

                var command = new MySqlCommand(sql, connection);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new ResourceCategory
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name"),
                        Summary = reader.GetString("Summary")
                    });
                }

                this.CloseConnection();

            }
            return result;
        }

        public FullObservableCollection<BaseResource> findNotificationsByClassId(int classId)
        {
            var result = new FullObservableCollection<BaseResource>();
            if (this.OpenConnection() == true)
            {

                var sql = """
                    select Id, ClassId, ResourceCategoryId, NotificationText, PostDate, Title
                    from Notifications
                    where ClassId=@ClassId
                    """;
                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@ClassId", MySqlDbType.Int32).Value = classId;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Notification
                    {
                        Id = reader.GetInt32("Id"),
                        ClassId = reader.GetInt32("ClassId"),
                        ResourceCategoryId = reader.GetInt32("ResourceCategoryId"),
                        NotificationText = reader.GetString("NotificationText"),
                        PostDate = reader.GetDateTime("PostDate"),
                        Title = reader.GetString("Title")
                    });
                }
                this.CloseConnection();
            }

            return result;
        }

        public FullObservableCollection<BaseResource> findAssignmentsByClassId(int classId)
        {
            // same to notification
            var result = new FullObservableCollection<BaseResource>();
            if (this.OpenConnection() == true)
            {

                var sql = """
                    select Id, ClassId, ResourceCategoryId, Title, TeacherId, Description, DueDate, FilePath, FileName, FileType
                    from Assignments
                    where ClassId=@ClassId
                    """;
                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@ClassId", MySqlDbType.Int32).Value = classId;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Assignment
                    {
                        Id = reader.GetInt32("Id"),
                        ClassId = reader.GetInt32("ClassId"),
                        ResourceCategoryId = reader.GetInt32("ResourceCategoryId"),
                        Title = reader.GetString("Title"),
                        TeacherId = reader.GetInt32("TeacherId"),
                        Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                        DueDate = (DateTime)(reader.IsDBNull("DueDate") ? (DateTime?)null : reader.GetDateTime("DueDate")),
                        FilePath = reader.IsDBNull("FilePath") ? null : reader.GetString("FilePath"),
                        FileName = reader.IsDBNull("FileName") ? null : reader.GetString("FileName"),
                        FileType = reader.IsDBNull("FileType") ? null : reader.GetString("FileType")
                    });
                }
                this.CloseConnection();
            }
            return result;

        }

        public FullObservableCollection<BaseResource> findDocumentsByClassId(int classId)
        {
            // same 
            var result = new FullObservableCollection<BaseResource>();
            if (this.OpenConnection() == true)
            {

                var sql = """
                    select Id, ClassId, ResourceCategoryId, FileName, FilePath, FileType, UploadDate, Title
                    from Documents
                    where ClassId=@ClassId
                    """;
                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@ClassId", MySqlDbType.Int32).Value = classId;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Document
                    {
                        Id = reader.GetInt32("Id"),
                        ClassId = reader.GetInt32("ClassId"),
                        ResourceCategoryId = reader.GetInt32("ResourceCategoryId"),
                        FileName = reader.IsDBNull("FileName") ? null : reader.GetString("FileName"),
                        FilePath = reader.IsDBNull("FilePath") ? null : reader.GetString("FilePath"),
                        FileType = reader.IsDBNull("FileType") ? null : reader.GetString("FileType"),
                        UploadDate = reader.GetDateTime("UploadDate"),
                        Title = reader.GetString("Title")
                    });
                }
                this.CloseConnection();
            }
            return result;

        }


        // ------------------- MOCK --------------------


        public Department GetDepartmentById(int departmentId)
        {
            Random random = new Random();
            departmentId = random.Next(1, 20);
            if (departmentId % 5 == 1)
            {
                return new Department
                {
                    Id = departmentId,
                    DepartmentCode = "CSE",
                    DepartmentDesc = "Computer Science and Engineering"
                };
            }
            if (departmentId % 5 == 2)
            {
                return new Department
                {
                    Id = departmentId,
                    DepartmentCode = "ECE",
                    DepartmentDesc = "Electronics and Communication Engineering"
                };
            }
            if (departmentId % 5 == 3)
            {
                return new Department
                {
                    Id = departmentId,
                    DepartmentCode = "EEE",
                    DepartmentDesc = "Electrical and Electronics Engineering"
                };
            }
            if (departmentId % 5 == 4)
            {
                return new Department
                {
                    Id = departmentId,
                    DepartmentCode = "CIV",
                    DepartmentDesc = "Civil Engineering"
                };
            }
            return new Department
            {
                Id = departmentId,
                DepartmentCode = "ME",
                DepartmentDesc = "Mechanical Engineering"
            };
        }

        public ObservableCollection<Class> GetEnrolledClassesByStudentId(int studentId)
        {
            return new ObservableCollection<Class>
            {
                    new Class
                {
                    Id = 1,
                    CourseId = 1,
                    ClassCode = "APCS_1",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 2,
                    CourseId = 2,
                    ClassCode = "APCS_2",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 3,
                    CourseId = 3,
                    ClassCode = "HP_1",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 4,
                    CourseId = 4,
                    ClassCode = "HP_2",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 5,
                    CourseId = 5,
                    ClassCode = "HP_3",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 6,
                    CourseId = 6,
                    ClassCode = "HP_4",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 7,
                    CourseId = 7,
                    ClassCode = "HP_5",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 8,
                    CourseId = 8,
                    ClassCode = "GP_1",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 9,
                    CourseId = 9,
                    ClassCode = "GP_2",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 10,
                    CourseId = 10,
                    ClassCode = "GP_3",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 11,
                    CourseId = 11,
                    ClassCode = "GP_4",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 12,
                    CourseId = 12,
                    ClassCode = "GP_5",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 13,
                    CourseId = 13,
                    ClassCode = "GP_6",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 14,
                    CourseId = 14,
                    ClassCode = "GP_7",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 15,
                    CourseId = 15,
                    ClassCode = "GP_8",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                }

            };
        }

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

        public Submission GetSubmissionById(int id)
        {
            var result = new Submission();
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                        select Id, AssignmentId, UserId, SubmissionDate, FilePath, FileName, FileType, Grade
                        from Submissions
                        where Id=@Id
                        """;

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@Id", MySqlDbType.Int32).Value = id;
                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            result = new Submission
                            {
                                Id = reader.GetInt32("Id"),
                                AssignmentId = reader.GetInt32("AssignmentId"),
                                UserId = reader.GetInt32("UserId"),
                                SubmissionDate = reader.GetDateTime("SubmissionDate"),
                                FilePath = reader.GetString("FilePath"),
                                FileName = reader.GetString("FileName"),
                                FileType = reader.GetString("FileType"),
                                Grade = reader.IsDBNull("Grade") ? null : reader.GetDecimal("Grade")
                            };
                        }
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

        public Student GetStudentById(int studentId)
        {
            throw new NotImplementedException();
        }

        public Assignment GetAssignmentById(int assignmentId)
        {
            var result = new Assignment();
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                        select Id, ClassId, ResourceCategoryId, Title, Description, DueDate, FilePath, FileName, FileType
                        from Assignments
                        where Id=@Id
                        """;

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@Id", MySqlDbType.Int32).Value = assignmentId;
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            result.Id = reader.GetInt32("Id");
                            result.ClassId = reader.GetInt32("ClassId");
                            result.ResourceCategoryId = reader.GetInt32("ResourceCategoryId");
                            result.Title = reader.GetString("Title");
                            result.Description = reader.IsDBNull("Description") ? null : reader.GetString("Description");
                            result.DueDate = reader.GetDateTime("DueDate");
                            result.FilePath = reader.IsDBNull("FilePath") ? null : reader.GetString("FilePath");
                            result.FileName = reader.IsDBNull("FileName") ? null : reader.GetString("FileName");
                            result.FileType = reader.IsDBNull("FileType") ? null : reader.GetString("FileType");
                        }
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

        public Student GetStudentByUserId(int id)
        {
            var result = new Student();
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                        select Id, StudentCode, StudentName, Email, BirthDate, PhoneNo, UserId
                        from Students
                        where UserId=@UserId
                        """;

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@UserId", MySqlDbType.Int32).Value = id;
                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            result = new Student
                            {
                                Id = reader.GetInt32("Id"),
                                StudentCode = reader.GetString("StudentCode"),
                                StudentName = reader.GetString("StudentName"),
                                Email = reader.GetString("Email"),
                                BirthDate = reader.GetDateTime("BirthDate"),
                                PhoneNo = reader.GetString("PhoneNo"),
                                UserId = reader.GetInt32("UserId")
                            };
                        }
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

        public void SaveSubmission(Submission submission)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                        insert into Submissions (AssignmentId, UserId, SubmissionDate, FilePath, FileName, FileType, Grade)
                        values (@AssignmentId, @UserId, @SubmissionDate, @FilePath, @FileName, @FileType, @Grade)
                        """;

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@AssignmentId", MySqlDbType.Int32).Value = submission.AssignmentId;
                        command.Parameters.Add("@UserId", MySqlDbType.Int32).Value = submission.UserId;
                        command.Parameters.Add("@SubmissionDate", MySqlDbType.DateTime).Value = submission.SubmissionDate;
                        command.Parameters.Add("@FilePath", MySqlDbType.String).Value = submission.FilePath;
                        command.Parameters.Add("@FileName", MySqlDbType.String).Value = submission.FileName;
                        command.Parameters.Add("@FileType", MySqlDbType.String).Value = submission.FileType;
                        command.Parameters.Add("@Grade", MySqlDbType.Decimal).Value = submission.Grade;

                        command.ExecuteNonQuery();
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
        }

        public List<Submission> GetSubmissionsByAssignmentId(int id)
        {
            var result = new List<Submission>();
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                        select s.Id, s.AssignmentId, s.UserId, s.SubmissionDate, s.FilePath, s.FileName, s.FileType, u.Username, s.Grade
                        from Submissions s
                        join Users u on s.UserId = u.Id
                        where s.AssignmentId=@AssignmentId
                        """;

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@AssignmentId", MySqlDbType.Int32).Value = id;
                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            result.Add(new Submission
                            {
                                Id = reader.GetInt32("Id"),
                                AssignmentId = reader.GetInt32("AssignmentId"),
                                UserId = reader.GetInt32("UserId"),
                                SubmissionDate = reader.GetDateTime("SubmissionDate"),
                                FilePath = reader.GetString("FilePath"),
                                FileName = reader.GetString("FileName"),
                                FileType = reader.GetString("FileType"),
                                Grade = reader.IsDBNull("Grade") ? null : reader.GetDecimal("Grade")
                            });
                        }
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

        public List<Submission> GetSubmissionsByAssignmentIdAndUserId(int id1, int id2)
        {
            var result = new List<Submission>();
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                        select s.Id, s.AssignmentId, s.UserId, s.SubmissionDate, s.FilePath, s.FileName, s.FileType, s.Grade
                        from Submissions s
                        join Users u on s.UserId = u.Id
                        where s.AssignmentId=@AssignmentId and s.UserId=@UserId
                        """;

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@AssignmentId", MySqlDbType.Int32).Value = id1;
                        command.Parameters.Add("@UserId", MySqlDbType.Int32).Value = id2;
                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            result.Add(new Submission
                            {
                                Id = reader.GetInt32("Id"),
                                AssignmentId = reader.GetInt32("AssignmentId"),
                                UserId = reader.GetInt32("UserId"),
                                SubmissionDate = reader.GetDateTime("SubmissionDate"),
                                FilePath = reader.GetString("FilePath"),
                                FileName = reader.GetString("FileName"),
                                FileType = reader.GetString("FileType"),
                                Grade = reader.IsDBNull("Grade") ? null : reader.GetDecimal("Grade")
                            });
                        }
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

        public void UpdateSubmission(Submission submission)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                        update Submissions set AssignmentId=@AssignmentId, UserId=@UserId, SubmissionDate=@SubmissionDate, FilePath=@FilePath, FileName=@FileName, FileType=@FileType, Grade=@Grade
                        where Id=@Id
                        """;

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@AssignmentId", MySqlDbType.Int32).Value = submission.AssignmentId;
                        command.Parameters.Add("@UserId", MySqlDbType.Int32).Value = submission.UserId;
                        command.Parameters.Add("@SubmissionDate", MySqlDbType.DateTime).Value = submission.SubmissionDate;
                        command.Parameters.Add("@FilePath", MySqlDbType.String).Value = submission.FilePath;
                        command.Parameters.Add("@FileName", MySqlDbType.String).Value = submission.FileName;
                        command.Parameters.Add("@FileType", MySqlDbType.String).Value = submission.FileType;
                        command.Parameters.Add("@Grade", MySqlDbType.Decimal).Value = submission.Grade;
                        command.Parameters.Add("@Id", MySqlDbType.Int32).Value = submission.Id;

                        command.ExecuteNonQuery();
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
        }

        public void DeleteSubmissionById(int id)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = "delete from Submissions where Id=@Id";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@Id", MySqlDbType.Int32).Value = id;
                        command.ExecuteNonQuery();
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
        }

        public void SaveAssignment(Assignment assignment)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                        insert into Assignments (ClassId, ResourceCategoryId, Title, Description, TeacherId, DueDate, FilePath, FileName, FileType)
                        values (@ClassId, @ResourceCategoryId, @Title, @Description, @TeacherId, @DueDate, @FilePath, @FileName, @FileType)
                        """;

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@ClassId", MySqlDbType.Int32).Value = assignment.ClassId;
                        command.Parameters.Add("@ResourceCategoryId", MySqlDbType.Int32).Value = assignment.ResourceCategoryId;
                        command.Parameters.Add("@Title", MySqlDbType.String).Value = assignment.Title;
                        command.Parameters.Add("@Description", MySqlDbType.String).Value = assignment.Description;
                        command.Parameters.Add("@TeacherId", MySqlDbType.Int32).Value = assignment.TeacherId;
                        command.Parameters.Add("@DueDate", MySqlDbType.DateTime).Value = assignment.DueDate;
                        command.Parameters.Add("@FilePath", MySqlDbType.String).Value = assignment.FilePath;
                        command.Parameters.Add("@FileName", MySqlDbType.String).Value = assignment.FileName;
                        command.Parameters.Add("@FileType", MySqlDbType.String).Value = assignment.FileType;

                        command.ExecuteNonQuery();
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
        }

        public void UpdateAssignment(Assignment assignment)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                        update Assignments 
                        set ClassId=@ClassId, ResourceCategoryId=@ResourceCategoryId, Title=@Title, Description=@Description, TeacherId=@TeacherId, DueDate=@DueDate, FilePath=@FilePath, FileName=@FileName, FileType=@FileType
                        where Id=@Id
                        """;

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@ClassId", MySqlDbType.Int32).Value = assignment.ClassId;
                        command.Parameters.Add("@ResourceCategoryId", MySqlDbType.Int32).Value = assignment.ResourceCategoryId;
                        command.Parameters.Add("@Title", MySqlDbType.String).Value = assignment.Title;
                        command.Parameters.Add("@Description", MySqlDbType.String).Value = assignment.Description;
                        command.Parameters.Add("@TeacherId", MySqlDbType.Int32).Value = assignment.TeacherId;
                        command.Parameters.Add("@DueDate", MySqlDbType.DateTime).Value = assignment.DueDate;
                        command.Parameters.Add("@FilePath", MySqlDbType.String).Value = assignment.FilePath;
                        command.Parameters.Add("@FileName", MySqlDbType.String).Value = assignment.FileName;
                        command.Parameters.Add("@FileType", MySqlDbType.String).Value = assignment.FileType;
                        command.Parameters.Add("@Id", MySqlDbType.Int32).Value = assignment.Id;

                        command.ExecuteNonQuery();
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
        }

        public void DeleteAssignmentById(int id)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = "delete from Assignments where Id=@Id";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@Id", MySqlDbType.Int32).Value = id;
                        command.ExecuteNonQuery();
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
        }

        public void DeleteAttachmentByAssignmentId(int id)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = "delete from Assignments where Id=@Id";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@Id", MySqlDbType.Int32).Value = id;
                        command.ExecuteNonQuery();
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
        }

        public void AddAssignment(Assignment assignment)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                        INSERT INTO Assignments 
                        (ClassId, ResourceCategoryId, Title, Description, TeacherId, DueDate, FilePath, FileName, FileType)
                        VALUES (@ClassId, @ResourceCategoryId, @Title, @Description, @TeacherId, @DueDate, @FilePath, @FileName, @FileType)
                        """;

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@ClassId", MySqlDbType.Int32).Value = assignment.ClassId;
                        command.Parameters.Add("@ResourceCategoryId", MySqlDbType.Int32).Value = assignment.ResourceCategoryId;
                        command.Parameters.Add("@Title", MySqlDbType.String).Value = assignment.Title;
                        command.Parameters.Add("@Description", MySqlDbType.String).Value = assignment.Description;
                        command.Parameters.Add("@TeacherId", MySqlDbType.Int32).Value = assignment.TeacherId;
                        command.Parameters.Add("@DueDate", MySqlDbType.DateTime).Value = assignment.DueDate;
                        command.Parameters.Add("@FilePath", MySqlDbType.String).Value = assignment.FilePath;
                        command.Parameters.Add("@FileName", MySqlDbType.String).Value = assignment.FileName;
                        command.Parameters.Add("@FileType", MySqlDbType.String).Value = assignment.FileType;

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    this.CloseConnection();
                }
            }
        }
    }
}
