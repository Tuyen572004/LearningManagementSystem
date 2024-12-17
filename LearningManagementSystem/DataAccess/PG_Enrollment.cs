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
        //------------------------ TUYEN ------------
        public List<int> findStudentIdByClassId(int id)
        {
            List<int> result = new List<int>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                    SELECT studentid FROM enrollments WHERE classid = @ClassId
                ";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClassId", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetInt32(reader.GetOrdinal("studentid")));
                        }
                    }
                }
            }
            return result;
        }
    }
}
