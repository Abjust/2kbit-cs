﻿// 2kbot b2.3.0
// Copyright(C) 2022 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

// 致所有构建及修改2kbot代码片段的用户：作者（Abjust）并不承担构建2kbot代码片段（包括修改过的版本）所产生的一切风险，但是用户有权在2kbot的GitHub项目页提出issue，并有权在代码片段修复这些问题后获取这些更新，但是，作者不会对修改过的代码版本做质量保证，也没有义务修正在修改过的代码片段中存在的任何缺陷。

using MySql.Data.MySqlClient;

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