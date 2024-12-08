using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml;
using LearningManagementSystem.Helpers;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using System.Configuration;
using System.Text.Json;
using System.IO;
using LearningManagementSystem.Controls;
using CommunityToolkit.WinUI.UI.Controls;




namespace LearningManagementSystem.DataAccess
{
    public class Config
    {
        public string DB_SERVER { get; set; }
        public string DB_DATABASE { get; set; }
        public string DB_USERNAME { get; set; }
        public string DB_PASSWORD { get; set; }
    }
    public class SqlDao : IDao
    {

        public MySqlConnection connection;

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
            Config config = JsonSerializer.Deserialize<Config>(configContent);

            server = config.DB_SERVER;
            database = config.DB_DATABASE;
            username = config.DB_USERNAME;
            password = config.DB_PASSWORD;

            // Use the credentials to create a connection string or connect to the database
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

                //String commandtext = deletingStudentCommand.CommandText;
                //foreach (MySql.Data.MySqlClient.MySqlParameter p in deletingStudentCommand.Parameters)
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
            IEnumerable<int> chosenIds = null,
            IEnumerable<SortCriteria> sortCriteria = null,
            SearchCriteria searchCriteria = null,
            List<(StudentField field, object leftBound, object rightBound, bool containedLeftBound, bool withinBounds, bool containedRightBound)> filterCriteria = null
        )
        {
            ObservableCollection<StudentVer2> result = [];
            int queryCount = 0;


            if (OpenConnection() == true)
            {
                var Query =
                """
                select count(*) over() as TotalItem, Id, StudentCode, StudentName, Email, BirthDate, PhoneNo, UserId, EnrollmentYear, GraduationYear
                from Students
                """;

                Query += '\n';

                bool startingWhere = true;
                string whereCondition(string condition)
                {
                    if (startingWhere)
                    {
                        startingWhere = false;
                        return String.Format("where {0}\n", condition);
                    }
                    else
                    {
                        return String.Format("    and {0}\n", condition);
                    }
                }

                if (chosenIds is not null && chosenIds.Any())
                {
                    // Query += $"\nwhere Id in ({string.Join(", ", chosenIds)})";

                    Query += whereCondition($"Id in ({string.Join(", ", chosenIds)})");

                }

                if (searchCriteria is not null)
                {
                    string searchCondition = "";
                    searchCondition = searchCriteria.Field switch
                    {
                        _ => $"{searchCriteria.Field}",
                    };


                    searchCondition = searchCriteria.Pattern switch
                    {
                        "is not null" or "is null" => $"{searchCondition} {searchCriteria.Pattern}",
                        _ => $"ifnull(cast({searchCondition} as char), '') {searchCriteria.Pattern}",
                    };

                    Query += whereCondition(searchCondition);
                }

                List<string> sortQuery = sortCriteria
                    ?.Select(s => 
                    {
                        string sortField = s.ColumnTag;
                        string sortDirection = s.SortDirection == DataGridSortDirection.Ascending ? "asc" : "desc";
                        return $"{sortField} {sortDirection}";
                    }
                    ).ToList()
                    ?? [];

                if (sortQuery.Count != 0)
                {
                    Query += $"order by {string.Join(", ", sortQuery)}\n";
                }


                if (!fetchingAll)
                {
                    Query += "limit @Take offset @Skip\n";
                }
                

                var Command = new MySqlCommand(Query, connection);

                if (!fetchingAll)
                {
                    Command.Parameters.Add("@Skip", MySqlDbType.Int32).Value = ignoringCount;
                    Command.Parameters.Add("@Take", MySqlDbType.Int32).Value = fetchingCount;
                }

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


            //if (OpenConnection())
            //{
            //    var sql = """
            //        select count(*) over() as TotalItem, Id, StudentCode, StudentName, Email, Birthday, PhoneNo, UserId
            //        from Students
            //        """;

            //    bool startingWhere = false;
            //    var whereFormat = static (isStarting) =>
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
            //    foreach ((StudentField field, object keyword) in searchKeyword)
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
            
        }
        public (
            IList<StudentVer2> addStudents,
            int addCount,
            IList<(StudentVer2 student, IEnumerable<String> error)> invalidStudentsInfo
            ) AddStudents(IEnumerable<StudentVer2> students)
        {
            List<StudentVer2> addedStudents = [];
            int addedCount = 0;
            List<(StudentVer2 student, IEnumerable<String> error)> invalidStudents = [];

            if (OpenConnection() == false)
            {
                CloseConnection();
                return (
                    addedStudents,
                    addedCount,
                    students.Select(s => (s, (new List<String> { "Can't connect to database"}).AsEnumerable() ?? [])).ToList()
                    );
            }

            using var transaction = connection.BeginTransaction();
            foreach (var student in students)
            {
                try
                {
                    var checkStudentIdCommand = new MySqlCommand(
                        """
                        SELECT COUNT(*) FROM Students
                        WHERE StudentCode = @StudentCode
                        """, connection, transaction);
                    checkStudentIdCommand.Parameters.AddWithValue("@StudentCode", student.StudentCode);
                    int existsCount = Convert.ToInt32(checkStudentIdCommand.ExecuteScalar() ?? 0);
                    if (existsCount > 0)
                    {
                        invalidStudents.Add((student, ["Student with StudentCode " + student.StudentCode + " already exists"]));
                        continue;
                    }

                    var insertCommand = new MySqlCommand(
                        """
                        INSERT INTO Students (StudentCode, StudentName, Email, BirthDate, PhoneNo, UserId, EnrollmentYear, GraduationYear)
                        VALUES (@StudentCode, @StudentName, @Email, @BirthDate, @PhoneNo, @UserId, @EnrollmentYear, @GraduationYear)
                        """, connection, transaction);

                    insertCommand.Parameters.AddWithValue("@StudentCode", student.StudentCode);
                    insertCommand.Parameters.AddWithValue("@StudentName", student.StudentName);
                    insertCommand.Parameters.AddWithValue("@Email", student.Email);
                    insertCommand.Parameters.AddWithValue("@BirthDate", student.BirthDate);
                    insertCommand.Parameters.AddWithValue("@PhoneNo", student.PhoneNo);
                    insertCommand.Parameters.AddWithValue("@UserId", student.UserId);
                    insertCommand.Parameters.AddWithValue("@EnrollmentYear", student.EnrollmentYear);
                    insertCommand.Parameters.AddWithValue("@GraduationYear", student.GraduationYear);

                    int queryResult = insertCommand.ExecuteNonQuery();
                    if (queryResult > 0)
                    {
                        addedStudents.Add(student);
                        addedCount++;
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    invalidStudents.Add((student, ["Exception raised when adding to database: " + ex.Message]));
                }
            }
            transaction.Dispose();
            CloseConnection();
            return (addedStudents, addedCount, invalidStudents);
        }
    

        public (
            IList<StudentVer2> updateStudents,
            int updatedCount,
            IList<(StudentVer2 student, IEnumerable<string> error)> invalidStudentsInfo
            ) UpdateStudents(IEnumerable<StudentVer2> students)
        {
            List<StudentVer2> updatedStudents = [];
            int updatedCount = 0;
            List<(StudentVer2 student, IEnumerable<string> error)> invalidStudents = [];

            if (!OpenConnection())
            {
                CloseConnection();
                return (updatedStudents, updatedCount, invalidStudents);
            }

            using var transaction = connection.BeginTransaction();
            foreach (var student in students)
            {
                try
                {
                    var checkExistenceCommand = new MySqlCommand(
                        """
                        SELECT COUNT(*) FROM Students
                        WHERE Id = @Id
                        """, connection, transaction);
                    checkExistenceCommand.Parameters.AddWithValue("@Id", student.Id);

                    var existsCount = Convert.ToInt32(checkExistenceCommand.ExecuteScalar() ?? 0);
                    if (existsCount == 0)
                    {
                        invalidStudents.Add((student, [$"Student with Id {student.Id} not found"]));
                        continue;
                    }

                    if (student.UserId is not null)
                    {
                        var checkUserIdCommand = new MySqlCommand(
                        """
                        SELECT COUNT(*) FROM Users
                        WHERE Id = @UserId
                        """, connection, transaction);
                        checkUserIdCommand.Parameters.AddWithValue("@UserId", student.UserId);

                        var userIdExistsCount = Convert.ToInt32(checkUserIdCommand.ExecuteScalar() ?? 0);
                        if (userIdExistsCount == 0)
                        {
                            invalidStudents.Add((student, [$"User with Id {student.UserId} not found"]));
                            continue;
                        }
                    }

                    var checkNewCodeCommand = new MySqlCommand(
                        """
                        SELECT COUNT(*) FROM Students
                        WHERE StudentCode = @StudentCode AND Id != @Id
                        """, connection, transaction);
                    checkNewCodeCommand.Parameters.AddWithValue("@StudentCode", student.StudentCode);
                    checkNewCodeCommand.Parameters.AddWithValue("@Id", student.Id);

                    var newCodeExistsCount = Convert.ToInt32(checkNewCodeCommand.ExecuteScalar() ?? 0);
                    if (newCodeExistsCount > 0)
                    {
                        invalidStudents.Add((student, [$"Student with StudentCode {student.StudentCode} already exists"]));
                        continue;
                    }


                    var updateCommand = new MySqlCommand(
                        """
                        UPDATE Students
                        SET StudentCode = @StudentCode, StudentName = @StudentName, Email = @Email, BirthDate = @BirthDate, PhoneNo = @PhoneNo, UserId = @UserId, EnrollmentYear = @EnrollmentYear, GraduationYear = @GraduationYear
                        WHERE Id = @Id
                        """, connection, transaction);
                    updateCommand.Parameters.AddWithValue("@Id", student.Id);
                    updateCommand.Parameters.AddWithValue("@StudentCode", student.StudentCode);
                    updateCommand.Parameters.AddWithValue("@StudentName", student.StudentName);
                    updateCommand.Parameters.AddWithValue("@Email", student.Email);
                    updateCommand.Parameters.AddWithValue("@BirthDate", student.BirthDate);
                    updateCommand.Parameters.AddWithValue("@PhoneNo", student.PhoneNo);
                    updateCommand.Parameters.AddWithValue("@UserId", student.UserId);
                    updateCommand.Parameters.AddWithValue("@EnrollmentYear", student.EnrollmentYear);
                    updateCommand.Parameters.AddWithValue("@GraduationYear", student.GraduationYear);

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
            transaction.Dispose();

            CloseConnection();
            return (updatedStudents, updatedCount, invalidStudents);
        }

        public (
            IList<StudentVer2> deleteStudents,
            int deletedCount,
            IList<(StudentVer2 student, IEnumerable<string> error)> invalidStudentsInfo
            ) DeleteStudents(IEnumerable<StudentVer2> students)
        {
            List<StudentVer2> deletedStudents = [];
            int deletedCount = 0;
            List<(StudentVer2 student, IEnumerable<string> error)> invalidStudents = [];

            if (!OpenConnection())
            {
                CloseConnection();
                return (deletedStudents, deletedCount, invalidStudents);
            }

            using var transaction = connection.BeginTransaction();
            foreach (var student in students)
            {
                try
                {
                    if (student.Id == -1)
                    {
                        invalidStudents.Add((student, [$"Newly created student can't be deleted"]));
                        continue;
                    }

                    var checkExistenceCommand = new MySqlCommand(
                        """
                        SELECT COUNT(*) FROM Students
                        WHERE Id = @Id
                        """, connection, transaction);
                    checkExistenceCommand.Parameters.AddWithValue("@Id", student.Id);

                    var existsCount = Convert.ToInt32(checkExistenceCommand.ExecuteScalar() ?? 0);
                    if (existsCount == 0)
                    {
                        invalidStudents.Add((student, [ $"Student with Id {student.Id} not found" ]));
                        continue;
                    }

                    var checkReferenceCommand = new MySqlCommand(
                        """
                        SELECT 
                            (SELECT COUNT(*) FROM Enrollments WHERE StudentId = @StudentId) 
                        AS TotalReferences
                        """, connection, transaction);
                    // (SELECT COUNT(*) FROM Submissions WHERE StudentId = @StudentId) +
                    checkReferenceCommand.Parameters.AddWithValue("@StudentId", student.Id);

                    var referenceCount = Convert.ToInt32(checkReferenceCommand.ExecuteScalar() ?? 0);
                    if (referenceCount > 0)
                    {
                        invalidStudents.Add((student, [$"Student with Id {student.Id} has {referenceCount} references"]));
                        continue;
                    }

                    if (student.UserId is not null)
                    {
                        var deletingUserCommand = new MySqlCommand(
                            """
                            DELETE FROM Users 
                            WHERE Id = @UserId
                            """, connection, transaction);
                        deletingUserCommand.Parameters.AddWithValue("@UserId", student.UserId);

                        int userQueryResult = deletingUserCommand.ExecuteNonQuery();
                        if (userQueryResult == 0)
                        {
                            invalidStudents.Add((student, [$"User with Id {student.UserId} referenced by this student not found"]));
                            continue;
                        }
                    }

                    var deletingStudentCommand = new MySqlCommand(
                        """
                        DELETE FROM Students 
                        WHERE Id = @Id
                        """, connection, transaction);
                    deletingStudentCommand.Parameters.AddWithValue("@Id", student.Id);

                    int queryResult = deletingStudentCommand.ExecuteNonQuery();
                    if (queryResult > 0)
                    {
                        deletedStudents.Add(student);
                        deletedCount++;
                    }
                }
                catch (Exception ex)
                {
                    invalidStudents.Add((student, new[] { "Exception raised when deleting from database: " + ex.Message }));
                }
            }
            transaction.Commit();
            transaction.Dispose();

            CloseConnection();
            return (deletedStudents, deletedCount, invalidStudents);
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
                    select Id, ClassId, ResourceCategoryId, Title, Description, DueDate
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
                        Description = reader.GetString("Description"),
                        DueDate = reader.GetDateTime("DueDate")
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
                    select Id, ClassId, ResourceCategoryId, DocumentName, DocumentPath, UploadDate, Title
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
                        DocumentName = reader.GetString("DocumentName"),
                        DocumentPath = reader.GetString("DocumentPath"),
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
            if (this.OpenConnection() == true)
            {
                var sql = """
                    select Id, AssignmentId, UserId, SubmissionDate, FilePath,FileName,FileType
                    from Submissions
                    where Id=@Id
                    """;

                var command = new MySqlCommand(sql, connection);
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
                        FileType = reader.GetString("FileType")
                    };
                }

                this.CloseConnection();
            }
            return result;
        }

        public Student GetStudentById(int studentId)
        {
            throw new NotImplementedException();
        }

        public Assignment GetAssignmentById(int assignmentId)
        {
            throw new NotImplementedException();
        }

        public Student GetStudentByUserId(int id)
        {
            var result = new Student();
            if (this.OpenConnection() == true)
            {
                var sql = """
                    select Id, StudentCode, StudentName, Email, BirthDate, PhoneNo, UserId
                    from Students
                    where UserId=@UserId
                    """;

                var command = new MySqlCommand(sql, connection);
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

                this.CloseConnection();
            }
            return result;
        }


        public void SaveSubmission(Submission submission)
        {
            if (this.OpenConnection() == true)
            {
                var sql = """
                insert into Submissions (AssignmentId, UserId, SubmissionDate, FilePath, FileName, FileType)
                values (@AssignmentId, @UserId, @SubmissionDate, @FilePath, @FileName, @FileType)
                """;

                var command = new MySqlCommand(sql, connection);
                command.Parameters.Add("@AssignmentId", MySqlDbType.Int32).Value = submission.AssignmentId;
                command.Parameters.Add("@UserId", MySqlDbType.Int32).Value = submission.UserId;
                command.Parameters.Add("@SubmissionDate", MySqlDbType.DateTime).Value = submission.SubmissionDate;
                command.Parameters.Add("@FilePath", MySqlDbType.String).Value = submission.FilePath;
                command.Parameters.Add("@FileName", MySqlDbType.String).Value = submission.FileName;
                command.Parameters.Add("@FileType", MySqlDbType.String).Value = submission.FileType;

                command.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        public List<Submission> GetSubmissionsByAssignmentId(int id)
        {
            var result = new List<Submission>();
            if (this.OpenConnection() == true)
            {
                var sql = """
                    select s.Id, s.AssignmentId, s.UserId, s.SubmissionDate, s.FilePath, s.FileName, s.FileType, u.Username
                    from Submissions s
                    join Users u on s.UserId = u.Id
                    where s.AssignmentId=@AssignmentId
                    """;

                var command = new MySqlCommand(sql, connection);
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
                        FileType = reader.GetString("FileType")
                    });
                }

                this.CloseConnection();
            }
            return result;
        }

        public List<Submission> GetSubmissionsByAssignmentIdAndUserId(int id1, int id2)
        {
            var result = new List<Submission>();
            if(this.OpenConnection() == true)
            {
                var sql = """
                    select s.Id, s.AssignmentId, s.UserId, s.SubmissionDate, s.FilePath, s.FileName, s.FileType
                    from Submissions s
                    join Users u on s.UserId = u.Id
                    where s.AssignmentId=@AssignmentId and s.UserId=@UserId
                    """;

                var command = new MySqlCommand(sql, connection);
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
                        FileType = reader.GetString("FileType")
                    });
                }

                this.CloseConnection();
            }
            return result;
        }

        public void UpdateSubmission(Submission submission)
        {
            // find submission by id
            Submission submission1 = GetSubmissionById(submission.Id);
            if (submission1 != null)
            {
                if (this.OpenConnection() == true)
                {
                    var sql = """
                    update Submissions set AssignmentId=@AssignmentId, UserId=@UserId, SubmissionDate=@SubmissionDate, FilePath=@FilePath, FileName=@FileName, FileType=@FileType
                    where Id=@Id
                    """;

                    var command = new MySqlCommand(sql, connection);
                    command.Parameters.Add("@AssignmentId", MySqlDbType.Int32).Value = submission.AssignmentId;
                    command.Parameters.Add("@UserId", MySqlDbType.Int32).Value = submission.UserId;
                    command.Parameters.Add("@SubmissionDate", MySqlDbType.DateTime).Value = submission.SubmissionDate;
                    command.Parameters.Add("@FilePath", MySqlDbType.String).Value = submission.FilePath;
                    command.Parameters.Add("@FileName", MySqlDbType.String).Value = submission.FileName;
                    command.Parameters.Add("@FileType", MySqlDbType.String).Value = submission.FileType;
                    command.Parameters.Add("@Id", MySqlDbType.Int32).Value = submission.Id;

                    command.ExecuteNonQuery();
                    this.CloseConnection();
                }
            }
            else
            {
                throw new Exception("Submission not found");
            }
        }

        public void DeleteSubmissionById(int id)
        {
            OpenConnection();
            var sql = "delete from Submissions where Id=@Id";
            var command = new MySqlCommand(sql, connection);
            command.Parameters.Add("@Id", MySqlDbType.Int32).Value = id;
            command.ExecuteNonQuery();
            CloseConnection();
        }
    }
}
