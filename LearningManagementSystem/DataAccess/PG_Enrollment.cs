using LearningManagementSystem.Models;
using Npgsql;
using NPOI.OpenXmlFormats.Dml.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.DataAccess
{
    public partial class SqlDao
    {
        /// <summary>
        /// Finds the list of student IDs by class ID.
        /// </summary>
        /// <param name="id">The ID of the class.</param>
        /// <returns>A list of student IDs enrolled in the specified class.</returns>
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
