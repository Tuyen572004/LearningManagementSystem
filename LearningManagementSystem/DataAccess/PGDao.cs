using CommunityToolkit.WinUI.UI.Controls;
using LearningManagementSystem.Controls;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using Npgsql;
using NpgsqlTypes;
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
        public string DB_HOST { get; set; }
        public string DB_DATABASE { get; set; }
        public string DB_USERNAME { get; set; }
        public string DB_PASSWORD { get; set; }
    }

    public class SqlDao : IDao
    {
        public NpgsqlConnection connection;

        public string host { get; set; }
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
            var databaseConfig = configJson.RootElement.GetProperty("PG_Database").ToString();
            DatabaseConfig config = JsonSerializer.Deserialize<DatabaseConfig>(databaseConfig);

            host = config.DB_HOST;
            database = config.DB_DATABASE;
            username = config.DB_USERNAME;
            password = config.DB_PASSWORD;

            string connectionString = $"Host={host};Username={username};Password={password};Database={database}";
            connection = new NpgsqlConnection(connectionString);
        }

        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Cannot connect to server: {ex.Message}");
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
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public void DeleteCourse(int courseId)
        {
            throw new NotImplementedException();
        }

        public string GetFullCommandText(NpgsqlCommand command)
        {
            string commandText = command.CommandText;
            foreach (NpgsqlParameter param in command.Parameters)
            {
                commandText = commandText.Replace(param.ParameterName, param.Value.ToString());
            }
            return commandText;
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

        // ---------------------------  DEPARTMENT------------------ //

        public Tuple<int, List<Department>> GetAllDepartments(int page = 1, int pageSize = 10, string keyword = "", bool nameAscending = false)
        {
            var result = new List<Department>();

            if (this.OpenConnection())
            {
                var sql = """
                    SELECT COUNT(*) OVER() AS TotalItems, Id, DepartmentCode, DepartmentDesc
                    FROM Departments
                    WHERE DepartmentCode LIKE @Keyword
                    ORDER BY Id ASC
                    LIMIT @Take OFFSET @Skip
                    """;

                var skip = (page - 1) * pageSize;
                var take = pageSize;

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Skip", skip);
                command.Parameters.AddWithValue("@Take", take);
                command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

                var reader = command.ExecuteReader();

                int totalItems = -1;
                while (reader.Read())
                {
                    if (totalItems == -1)
                    {
                        totalItems = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                    }
                    result.Add(new Department
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        DepartmentCode = reader.GetString(reader.GetOrdinal("DepartmentCode")),
                        DepartmentDesc = reader.GetString(reader.GetOrdinal("DepartmentDesc"))
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
            if (this.OpenConnection())
            {
                var sql = "SELECT Id FROM Departments WHERE DepartmentCode=@DepartmentCode";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@DepartmentCode", department.DepartmentCode);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    department.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                }

                this.CloseConnection();
                return department.Id;
            }
            else return -1;
        }

        public void GetFullUser(User user)
        {
            if (this.OpenConnection())
            {
                var sql = "SELECT * FROM Users WHERE username=@Username AND passwordhash=@PasswordHash";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    user.Username = reader.GetString(reader.GetOrdinal("Username"));
                    user.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                    user.Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email"));
                    user.Role = reader.GetString(reader.GetOrdinal("Role"));
                    user.CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"));
                }
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

        public int CountDepartments()
        {
            if (this.OpenConnection())
            {
                var sql = "SELECT COUNT(*) AS TotalItems FROM Departments";
                var command = new NpgsqlCommand(sql, connection);

                var reader = command.ExecuteReader();

                reader.Read();

                int result = reader.GetInt32(reader.GetOrdinal("TotalItems"));

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

        // --------------------------------- CLASS -----------------------------

        public Class findClassById(int classId)
        {
            var result = new Class();
            if (this.OpenConnection())
            {
                var sql = """
                    SELECT Id, CourseId, ClassCode, CycleId, ClassStartDate, ClassEndDate
                    FROM Classes
                    WHERE Id=@classId
                    """;

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ClassId", classId);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result = new Class
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        ClassCode = reader.GetString(reader.GetOrdinal("ClassCode")),
                        CycleId = reader.GetInt32(reader.GetOrdinal("CycleId")),
                        ClassStartDate = reader.GetDateTime(reader.GetOrdinal("ClassStartDate")),
                        ClassEndDate = reader.GetDateTime(reader.GetOrdinal("ClassEndDate"))
                    };
                }

                this.CloseConnection();
            }
            return result;
        }

        public bool CheckUserInfo(User user)
        {
            if (this.OpenConnection())
            {
                var sql = "SELECT COUNT(*) AS TotalItems FROM Users WHERE Username=@username AND PasswordHash=@passwordhash";
                var command = new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@passwordhash", user.PasswordHash);

                var reader = command.ExecuteReader();

                reader.Read();

                int result = reader.GetInt32(reader.GetOrdinal("TotalItems"));

                this.CloseConnection();
                return result == 1;
            }
            else return false;
        }

        public bool IsExistsUsername(string username)
        {
            if (this.OpenConnection())
            {
                var sql = "SELECT COUNT(*) AS TotalItems FROM Users WHERE Username=@username";

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@username", username);
                var reader = command.ExecuteReader();

                reader.Read();

                int result = reader.GetInt32(reader.GetOrdinal("TotalItems"));

                this.CloseConnection();
                return result == 1;
            }
            else return true;
        }

        public bool AddUser(User user)
        {
            if (this.OpenConnection())
            {
                var sql = """
                    INSERT INTO Users (Username, PasswordHash, Email, Role, CreatedAt)
                    VALUES (@username, @passwordhash, null, @role, null)
                    RETURNING Id;
                    """;

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@passwordhash", user.PasswordHash);
                command.Parameters.AddWithValue("@role", user.Role);

                int id = (int)command.ExecuteScalar();
                user.Id = id;

                this.CloseConnection();

                return id > 0 ? true : false;
            }
            this.CloseConnection();
            return false;
        }

        //public ObservableCollection<StudentVer2> GetStudentsByClassId(int classId)
        //{
        //    try
        //    {
        //        bool connectionState = OpenConnection();
        //        if (connectionState)
        //        {
        //            Console.WriteLine("Connection is open");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    CloseConnection();
        //    return new ObservableCollection<StudentVer2>();
        //}

        //public (ObservableCollection<StudentVer2>, int) GetStudentsById(
        //    int ignoringCount = 0,
        //    int fetchingCount = 0,
        //    IEnumerable<int> chosenIds = null
        //    )
        //{
        //    ObservableCollection<StudentVer2> result = new ObservableCollection<StudentVer2>();
        //    int queryCount = 0;
        //    if (OpenConnection())
        //    {
        //        var Query = """
        //        SELECT COUNT(*) OVER() AS TotalItem, Id, StudentCode, StudentName, Email, BirthDate, PhoneNo, UserId, EnrollmentYear, GraduationYear
        //        FROM Students
        //        """;

        //        if (chosenIds != null && chosenIds.Any())
        //        {
        //            Query += $"\nWHERE Id IN ({string.Join(", ", chosenIds)})";
        //        }

        //        Query += "\nLIMIT @Take OFFSET @Skip";

        //        var Command = new NpgsqlCommand(Query, connection);
        //        Command.Parameters.AddWithValue("@Skip", ignoringCount);
        //        Command.Parameters.AddWithValue("@Take", fetchingCount);

        //        var QueryResultReader = Command.ExecuteReader();

        //        bool isTotalItemFetched = false;

        //        while (QueryResultReader.Read())
        //        {
        //            if (!isTotalItemFetched)
        //            {
        //                queryCount = QueryResultReader.GetInt32(QueryResultReader.GetOrdinal("TotalItem"));
        //                isTotalItemFetched = true;
        //            }

        //            int graduationYearColumn = QueryResultReader.GetOrdinal("GraduationYear");
        //            int userIdColumn = QueryResultReader.GetOrdinal("UserId");
        //            StudentVer2 newStudent = new()
        //            {
        //                Id = QueryResultReader.GetInt32(QueryResultReader.GetOrdinal("Id")),
        //                StudentCode = QueryResultReader.GetString(QueryResultReader.GetOrdinal("StudentCode")),
        //                StudentName = QueryResultReader.GetString(QueryResultReader.GetOrdinal("StudentName")),
        //                Email = QueryResultReader.GetString(QueryResultReader.GetOrdinal("Email")),
        //                BirthDate = QueryResultReader.GetDateTime(QueryResultReader.GetOrdinal("BirthDate")),
        //                PhoneNo = QueryResultReader.GetString(QueryResultReader.GetOrdinal("PhoneNo")),
        //                UserId = QueryResultReader.IsDBNull(userIdColumn) ? (int?)null : QueryResultReader.GetInt32(QueryResultReader.GetOrdinal("UserId")),
        //                EnrollmentYear = QueryResultReader.GetInt32(QueryResultReader.GetOrdinal("EnrollmentYear")),
        //                GraduationYear = QueryResultReader.IsDBNull(graduationYearColumn) ? (int?)null : QueryResultReader.GetInt32(QueryResultReader.GetOrdinal("GraduationYear"))
        //            };

        //            result.Add(newStudent);
        //        };
        //    }
        //    CloseConnection();
        //    return (result, queryCount);
        //}

        //public (ObservableCollection<StudentVer2>, int) GetStudents(
        //    bool fetchingAll = false,
        //    int ignoringCount = 0,
        //    int fetchingCount = 0,
        //    List<(StudentField field, Ordering order)> sortCriteria = null,
        //    List<(StudentField field, object keyword)> searchKeyword = null,
        //    List<(StudentField field, object leftBound, object rightBound, bool containedLeftBound, bool withinBounds, bool containedRightBound)> filterCriteria = null
        //)
        //{
        //    ObservableCollection<StudentVer2> result = new ObservableCollection<StudentVer2>();
        //    int totalItem = 0;
        //    return (result, totalItem);
        //}

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

                var query = """
                    select count(*) over() as "totalitem", "id", "studentcode", "studentname", "email", "birthdate", "phoneno", "userid", "enrollmentyear", "graduationyear"
                    from "students"
                    """;

                if (chosenIds is not null && chosenIds.Any())
                {
                    query += $"\nwhere \"id\" in ({string.Join(", ", chosenIds)})";
                }

                query += "\nlimit @Take offset @Skip";

                var command = new NpgsqlCommand(query, connection);
                command.Parameters.Add("@Skip", NpgsqlDbType.Integer).Value = ignoringCount;
                command.Parameters.Add("@Take", NpgsqlDbType.Integer).Value = fetchingCount;

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
                        return String.Format("where {0}\n", condition);
                    }
                    else
                    {
                        return String.Format("    and {0}\n", condition);
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
            CloseConnection();
            return (result, queryCount);
        }


        public (
            IList<StudentVer2> addStudents,
            int addCount,
            IList<(StudentVer2 student, IEnumerable<string> error)> invalidStudentsInfo
        ) AddStudents(IEnumerable<StudentVer2> students)
        {
            List<StudentVer2> addedStudents = [];
            int addedCount = 0;
            List<(StudentVer2 student, IEnumerable<string> error)> invalidStudents = [];

            if (OpenConnection() == false)
            {
                CloseConnection();
                return (
                    addedStudents,
                    addedCount,
                    students.Select(s => (s, (new List<string> { "Can't connect to database" }).AsEnumerable() ?? [])).ToList()
                );
            }

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
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    invalidStudents.Add((student, new List<string> { "Exception raised when adding to database: " + ex.Message }));
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

                    if (student.UserId is not null)
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

            if (this.OpenConnection())
            {
                var sql = """
            SELECT Id, Name, Summary
            FROM ResourceCategories
            """;

                var command = new NpgsqlCommand(sql, connection);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new ResourceCategory
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Summary = reader.GetString(reader.GetOrdinal("Summary"))
                    });
                }

                this.CloseConnection();
            }
            return result;
        }

        public FullObservableCollection<BaseResource> findNotificationsByClassId(int classId)
        {
            var result = new FullObservableCollection<BaseResource>();
            if (this.OpenConnection())
            {
                var sql = """
            SELECT Id, ClassId, ResourceCategoryId, NotificationText, PostDate, Title
            FROM Notifications
            WHERE ClassId=@ClassId
            """;
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ClassId", classId);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Notification
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        ClassId = reader.GetInt32(reader.GetOrdinal("ClassId")),
                        ResourceCategoryId = reader.GetInt32(reader.GetOrdinal("ResourceCategoryId")),
                        NotificationText = reader.GetString(reader.GetOrdinal("NotificationText")),
                        PostDate = reader.GetDateTime(reader.GetOrdinal("PostDate")),
                        Title = reader.GetString(reader.GetOrdinal("Title"))
                    });
                }
                this.CloseConnection();
            }

            return result;
        }

        public FullObservableCollection<BaseResource> findAssignmentsByClassId(int classId)
        {
            var result = new FullObservableCollection<BaseResource>();
            if (this.OpenConnection())
            {
                var sql = """
            SELECT Id, ClassId, ResourceCategoryId, Title, TeacherId, Description, DueDate, FilePath, FileName, FileType
            FROM Assignments
            WHERE ClassId=@ClassId
            """;
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ClassId", classId);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Assignment
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        ClassId = reader.GetInt32(reader.GetOrdinal("ClassId")),
                        ResourceCategoryId = reader.GetInt32(reader.GetOrdinal("ResourceCategoryId")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        TeacherId = reader.GetInt32(reader.GetOrdinal("TeacherId")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                        FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath")),
                        FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                        FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType"))
                    });
                }
                this.CloseConnection();
            }
            return result;
        }

        public FullObservableCollection<BaseResource> findDocumentsByClassId(int classId)
        {
            var result = new FullObservableCollection<BaseResource>();
            if (this.OpenConnection())
            {
                var sql = """
            SELECT Id, ClassId, ResourceCategoryId, FileName, FilePath, FileType, UploadDate, Title
            FROM Documents
            WHERE ClassId=@ClassId
            """;
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ClassId", classId);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Document
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        ClassId = reader.GetInt32(reader.GetOrdinal("ClassId")),
                        ResourceCategoryId = reader.GetInt32(reader.GetOrdinal("ResourceCategoryId")),
                        FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                        FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath")),
                        FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType")),
                        UploadDate = reader.GetDateTime(reader.GetOrdinal("UploadDate")),
                        Title = reader.GetString(reader.GetOrdinal("Title"))
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
                    ClassCode = "CQ_1",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 2,
                    CourseId = 2,
                    ClassCode = "CQ_2",
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
                SELECT Id, AssignmentId, UserId, SubmissionDate, FilePath, FileName, FileType, Grade
                FROM Submissions
                WHERE Id=@Id
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            result = new Submission
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                AssignmentId = reader.GetInt32(reader.GetOrdinal("AssignmentId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                SubmissionDate = reader.GetDateTime(reader.GetOrdinal("SubmissionDate")),
                                FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath")),
                                FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                                FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType")),
                                Grade = reader.IsDBNull(reader.GetOrdinal("Grade")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("Grade"))
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

        public Assignment GetAssignmentById(int assignmentId)
        {
            var result = new Assignment();
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                SELECT Id, ClassId, ResourceCategoryId, Title, Description, DueDate, FilePath, FileName, FileType
                FROM Assignments
                WHERE Id=@Id
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", assignmentId);
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            result.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            result.ClassId = reader.GetInt32(reader.GetOrdinal("ClassId"));
                            result.ResourceCategoryId = reader.GetInt32(reader.GetOrdinal("ResourceCategoryId"));
                            result.Title = reader.GetString(reader.GetOrdinal("Title"));
                            result.Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"));
                            result.DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate"));
                            result.FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath"));
                            result.FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName"));
                            result.FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType"));
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
        public Student GetStudentByUserId(int id)
        {
            var result = new Student();
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                SELECT Id, StudentCode, StudentName, Email, BirthDate, PhoneNo, UserId
                FROM Students
                WHERE UserId=@UserId
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", id);
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
                INSERT INTO Submissions (AssignmentId, UserId, SubmissionDate, FilePath, FileName, FileType, Grade)
                VALUES (@AssignmentId, @UserId, @SubmissionDate, @FilePath, @FileName, @FileType, @Grade)
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AssignmentId", submission.AssignmentId);
                        command.Parameters.AddWithValue("@UserId", submission.UserId);
                        command.Parameters.AddWithValue("@SubmissionDate", submission.SubmissionDate);
                        command.Parameters.AddWithValue("@FilePath", submission.FilePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileName", submission.FileName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileType", submission.FileType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Grade", submission.Grade ?? (object)DBNull.Value);

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
                SELECT s.Id, s.AssignmentId, s.UserId, s.SubmissionDate, s.FilePath, s.FileName, s.FileType, u.Username, s.Grade
                FROM Submissions s
                JOIN Users u ON s.UserId = u.Id
                WHERE s.AssignmentId=@AssignmentId
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AssignmentId", id);
                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            result.Add(new Submission
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                AssignmentId = reader.GetInt32(reader.GetOrdinal("AssignmentId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                SubmissionDate = reader.GetDateTime(reader.GetOrdinal("SubmissionDate")),
                                FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath")),
                                FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                                FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType")),
                                Grade = reader.IsDBNull(reader.GetOrdinal("Grade")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("Grade"))
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
                SELECT s.Id, s.AssignmentId, s.UserId, s.SubmissionDate, s.FilePath, s.FileName, s.FileType, s.Grade
                FROM Submissions s
                JOIN Users u ON s.UserId = u.Id
                WHERE s.AssignmentId=@AssignmentId AND s.UserId=@UserId
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AssignmentId", id1);
                        command.Parameters.AddWithValue("@UserId", id2);
                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            result.Add(new Submission
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                AssignmentId = reader.GetInt32(reader.GetOrdinal("AssignmentId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                SubmissionDate = reader.GetDateTime(reader.GetOrdinal("SubmissionDate")),
                                FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath")),
                                FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                                FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType")),
                                Grade = reader.IsDBNull(reader.GetOrdinal("Grade")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("Grade"))
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
                UPDATE Submissions SET AssignmentId=@AssignmentId, UserId=@UserId, SubmissionDate=@SubmissionDate, FilePath=@FilePath, FileName=@FileName, FileType=@FileType, Grade=@Grade
                WHERE Id=@Id
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AssignmentId", submission.AssignmentId);
                        command.Parameters.AddWithValue("@UserId", submission.UserId);
                        command.Parameters.AddWithValue("@SubmissionDate", submission.SubmissionDate);
                        command.Parameters.AddWithValue("@FilePath", submission.FilePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileName", submission.FileName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileType", submission.FileType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Grade", submission.Grade ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Id", submission.Id);

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
                INSERT INTO Assignments (ClassId, ResourceCategoryId, Title, Description, TeacherId, DueDate, FilePath, FileName, FileType)
                VALUES (@ClassId, @ResourceCategoryId, @Title, @Description, @TeacherId, @DueDate, @FilePath, @FileName, @FileType)
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ClassId", assignment.ClassId);
                        command.Parameters.AddWithValue("@ResourceCategoryId", assignment.ResourceCategoryId);
                        command.Parameters.AddWithValue("@Title", assignment.Title);
                        command.Parameters.AddWithValue("@Description", assignment.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TeacherId", assignment.TeacherId);
                        command.Parameters.AddWithValue("@DueDate", assignment.DueDate);
                        command.Parameters.AddWithValue("@FilePath", assignment.FilePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileName", assignment.FileName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileType", assignment.FileType ?? (object)DBNull.Value);

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
                UPDATE Assignments 
                SET ClassId=@ClassId, ResourceCategoryId=@ResourceCategoryId, Title=@Title, Description=@Description, TeacherId=@TeacherId, DueDate=@DueDate, FilePath=@FilePath, FileName=@FileName, FileType=@FileType
                WHERE Id=@Id
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ClassId", assignment.ClassId);
                        command.Parameters.AddWithValue("@ResourceCategoryId", assignment.ResourceCategoryId);
                        command.Parameters.AddWithValue("@Title", assignment.Title);
                        command.Parameters.AddWithValue("@Description", assignment.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TeacherId", assignment.TeacherId);
                        command.Parameters.AddWithValue("@DueDate", assignment.DueDate);
                        command.Parameters.AddWithValue("@FilePath", assignment.FilePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileName", assignment.FileName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileType", assignment.FileType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Id", assignment.Id);

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
                    var sql = "DELETE FROM Submissions WHERE Id=@Id";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
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
                    var sql = "DELETE FROM Attachments WHERE AssignmentId=@AssignmentId";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AssignmentId", id);
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
                INSERT INTO Assignments (ClassId, ResourceCategoryId, Title, Description, TeacherId, DueDate, FilePath, FileName, FileType)
                VALUES (@ClassId, @ResourceCategoryId, @Title, @Description, @TeacherId, @DueDate, @FilePath, @FileName, @FileType)
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ClassId", assignment.ClassId);
                        command.Parameters.AddWithValue("@ResourceCategoryId", assignment.ResourceCategoryId);
                        command.Parameters.AddWithValue("@Title", assignment.Title);
                        command.Parameters.AddWithValue("@Description", assignment.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TeacherId", assignment.TeacherId);
                        command.Parameters.AddWithValue("@DueDate", assignment.DueDate);
                        command.Parameters.AddWithValue("@FilePath", assignment.FilePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileName", assignment.FileName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileType", assignment.FileType ?? (object)DBNull.Value);

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
                    var sql = "DELETE FROM Assignments WHERE Id=@Id";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
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

        public Tuple<int, List<User>> GetAllUsers(int page = 1, int pageSize = 10, string keyword = "", string sortBy = "Id", string sortOrder = "ASC")
        {
            var result = new List<User>();

            if (this.OpenConnection())
            {
                var sql = $"""
                    SELECT COUNT(*) OVER() AS TotalItems, id, username , passwordhash , email, role, createdat
                    FROM Users
                    WHERE username LIKE @Keyword
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

                int totalItems = -1;
                while (reader.Read())
                {
                    if (totalItems == -1)
                    {
                        totalItems = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                    }
                    result.Add(new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Username = reader.GetString(reader.GetOrdinal("username")),
                        PasswordHash = reader.GetString(reader.GetOrdinal("passwordhash")),
                        Email = reader.GetString(reader.GetOrdinal("email")),
                        Role = reader.GetString(reader.GetOrdinal("role")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat"))
                    });
                }

                this.CloseConnection();
                return new Tuple<int, List<User>>(totalItems, result);
            }
            else
            {
                this.CloseConnection();
                return new Tuple<int, List<User>>(-1, null);
            }
        }

        public int InsertUser(User user)
        {
            if (this.OpenConnection())
            {
                var sql = """
                    INSERT INTO Users (username, passwordhash, email, role, createdat)
                    VALUES (@Username, @PasswordHash, @Email, @Role, @CreatedAt)
                    RETURNING Id;
                    """;

                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);

                int id = (int)command.ExecuteScalar();
                user.Id = id;

                this.CloseConnection();

                return id > 0 ? 1 : -1;
            }
            this.CloseConnection();
            return -1;
        }

        public void UpdateUser(User user)
        {
            if (this.OpenConnection())
            {
                var sql = "UPDATE Users SET Username=@Username, PasswordHash=@PasswordHash, Email=@Email, Role=@Role WHERE Id=@Id";
                var command = new NpgsqlCommand(sql, connection);

                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.Parameters.AddWithValue("@Id", user.Id);

                command.ExecuteNonQuery();

                this.CloseConnection();
            }
        }

        public void RemoveUserByID(int userId)
        {
            if (this.OpenConnection())
            {
                var sql = "DELETE FROM Users WHERE Id=@Id";
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", userId);

                command.ExecuteNonQuery();

                this.CloseConnection();
            }
        }

        public int CountUser()
        {
            throw new NotImplementedException();
        }

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

        public List<string> GetAllUsernames()
        {
            var result = new List<string>();
            try
            {
                if (this.OpenConnection())
                {
                    var sql = "SELECT Username FROM Users";
                    var command = new NpgsqlCommand(sql, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        result.Add(reader.GetString(reader.GetOrdinal("username")));
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
    }
}