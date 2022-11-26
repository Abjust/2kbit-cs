using MySql.Data.MySqlClient;

namespace Net_2kBot.Modules
{
    public class BreadFactory
    {
        public static long last_produce = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        public static List<string>? group_ids;
        public static async Task BreadProduce()
        {
            // 连接数据库
            Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            MySqlConnection msc = new(Global.connectstring);
            MySqlConnection msc1 = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            MySqlCommand cmd1 = new()
            {
                Connection = msc1
            };
            msc.Open();
            msc1.Open();
            while (true)
            {
                List<string> groupids = new();
                Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                cmd.CommandText = "SELECT * FROM bread;";
                MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    string groupid = reader.GetString("gid");
                    groupids?.Add(groupid);
                    group_ids = groupids;
                }
                reader.Close();
                if (group_ids != null)
                {
                    foreach (string groupid in group_ids)
                    {
                        cmd.CommandText = $"SELECT * FROM bread WHERE gid = {groupid};";
                        reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                        reader.Read();
                        int formula = (int)(Math.Ceiling(Math.Pow(4, reader.GetInt32("factory_level")) * Math.Pow(0.25, reader.GetInt32("bread_diversity"))) + 1);
                        int maxstorage = (int)(32 * Math.Pow(4, reader.GetInt32("factory_level") - 1));
                        Random r = new();
                        int random = r.Next(1, formula);
                        reader.Close();
                        if (Global.time_now - last_produce >= 300)
                        {
                            cmd.CommandText = $"SELECT * FROM bread WHERE gid = {groupid};";
                            reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                            reader.Read();
                            if (reader.GetInt32("breads") + random < maxstorage)
                            {
                                cmd1.CommandText = $"UPDATE bread SET breads = {reader.GetInt32("breads") + random} WHERE gid = {groupid};";
                                cmd1.ExecuteNonQuery();
                            }
                            else if (reader.GetInt32("breads") + random >= maxstorage)
                            {
                                cmd1.CommandText = $"UPDATE bread SET breads = {maxstorage} WHERE gid = {groupid};";
                                cmd1.ExecuteNonQuery();
                            }
                            reader.Close();
                            msc.Close();
                            msc1.Close();
                        }
                    }
                }
                reader.Close();
                if (Global.time_now - last_produce >= 300)
                {
                    last_produce = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                }
                msc.Close();
                msc1.Close();
                Thread.Sleep(500);
            }
        }
    }
}
