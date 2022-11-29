using MySql.Data.MySqlClient;
using System.Data;

namespace Net_2kBot.Modules
{
    public class BreadFactory
    {
        public static List<string>? group_ids;
        public static int speed;
        public static async Task BreadProduce()
        {
            // 连接数据库
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                List<string> groupids = new();
                cmd.CommandText = "SELECT * FROM bread;";
                MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    string groupid = reader.GetString("gid");
                    groupids?.Add(groupid);
                    group_ids = groupids;
                }
                await reader.CloseAsync();
                while (true)
                {
                    Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    if (group_ids != null)
                    {
                        foreach (string groupid in group_ids)
                        {
                            cmd.CommandText = $"SELECT * FROM bread WHERE gid = {groupid};";
                            reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                            await reader.ReadAsync();
                            int formula = (int)Math.Ceiling(Math.Pow(4, reader.GetInt32("factory_level")) * Math.Pow(0.25, reader.GetInt32("bread_diversity")));
                            speed = 300 - (20 * (reader.GetInt32("factory_level") -1));
                            int maxstorage = (int)(32 * Math.Pow(4, reader.GetInt32("factory_level") - 1) * Math.Pow(2, reader.GetInt32("storage_upgraded")));
                            Random r = new();
                            int random = r.Next(1, formula);
                            if (Global.time_now - reader.GetInt64("last_produce") >= speed)
                            {
                                await reader.CloseAsync();
                                cmd.CommandText = $"SELECT * FROM bread WHERE gid = {groupid};";
                                reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                                await reader.ReadAsync();
                                using (var msc1 = new MySqlConnection(Global.connectstring))
                                {
                                    await msc1.OpenAsync();
                                    MySqlCommand cmd1 = new()
                                    {
                                        Connection = msc1
                                    };
                                    if (reader.GetInt32("breads") + random < maxstorage)
                                    {
                                        cmd1.CommandText = $"UPDATE bread SET breads = {reader.GetInt32("breads") + random} WHERE gid = {groupid};";
                                        await cmd1.ExecuteNonQueryAsync();
                                    }
                                    else if (reader.GetInt32("breads") + random >= maxstorage)
                                    {
                                        cmd1.CommandText = $"UPDATE bread SET breads = {maxstorage} WHERE gid = {groupid};";
                                        await cmd1.ExecuteNonQueryAsync();
                                    }
                                    cmd1.CommandText = $"UPDATE bread SET last_produce = {new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()} WHERE gid = {groupid};";
                                    await cmd1.ExecuteNonQueryAsync();
                                }
                                await reader.CloseAsync();
                            }
                        }
                    }
                    Thread.Sleep(500);
                }
            }
        }
    }
}