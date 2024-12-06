using Npgsql;
using System;

class Program
{
    static void Main()
    {
        var connString = "Host=dpg-ct3vdm52ng1s73a13mr0-a.singapore-postgres.render.com;Username=posgre;Password=sOL87JmxSGdTuGCwXDNNc8ehNbyctMVH;Database=lmsdb_7mch";

        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand("SELECT * FROM Users", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Id"]}, {reader["Username"]}, {reader["Email"]}, {reader["Role"]}");
                }
            }
        }
    }
}
