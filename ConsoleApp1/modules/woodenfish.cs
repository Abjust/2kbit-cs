// 2kbit C# Edition，2kbit的C#分支版本
// Copyright(C) 2022 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

// 致所有构建及修改2kbit代码片段的用户：作者（Abjust）并不承担构建2kbit代码片段（包括修改过的版本）所产生的一切风险，但是用户有权在2kbit的GitHub项目页提出issue，并有权在代码片段修复这些问题后获取这些更新，但是，作者不会对修改过的代码版本做质量保证，也没有义务修正在修改过的代码片段中存在的任何缺陷。

using Mirai.Net.Data.Messages;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using MySql.Data.MySqlClient;

namespace Net_2kBit.Modules
{
    public static class WoodenFish
    {
        // 我的木鱼
        public static async void Info(string group, string executor)
        {
            Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            string status = "";
            string word = "";
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                cmd.CommandText = "SELECT COUNT(*) uid FROM woodenfish WHERE uid = @uid;";
                cmd.Parameters.AddWithValue("@uid", executor);
                int i = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                if (i == 1)
                {
                    cmd.CommandText = "SELECT * FROM woodenfish WHERE uid = @uid;";
                    MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        if (reader.GetInt64("info_ctrl") < Global.time_now)
                        {
                            if (reader.GetInt32("ban") == 0)
                            {
                                using (var msc1 = new MySqlConnection(Global.connectstring))
                                {
                                    await msc1.OpenAsync();
                                    MySqlCommand cmd1 = new()
                                    {
                                        Connection = msc1
                                    };
                                    status = "正常";
                                    word = "【敲电子木鱼，见机甲佛祖，取赛博真经】";
                                    cmd1.Parameters.Add("@uid", MySqlDbType.String);
                                    if (Math.Log10(reader.GetInt64("gongde")) >= 1)
                                    {
                                        cmd1.CommandText = "UPDATE woodenfish SET e = @e, gongde = 0 WHERE uid = @uid";
                                        cmd1.Parameters.AddWithValue("@e", Math.Log10(Math.Pow(10, reader.GetDouble("e")) + reader.GetInt64("gongde")));
                                        cmd1.Parameters["@uid"].Value = executor;
                                        await cmd1.ExecuteNonQueryAsync();
                                    }
                                    if (Math.Log10(reader.GetDouble("e")) >= 1)
                                    {
                                        cmd1.CommandText = "UPDATE woodenfish SET ee = @ee, e = 0 WHERE uid = @uid";
                                        cmd1.Parameters.AddWithValue("@ee", Math.Log10(Math.Pow(10, reader.GetDouble("ee")) + reader.GetDouble("e")));
                                        cmd1.Parameters["@uid"].Value = executor;
                                        await cmd1.ExecuteNonQueryAsync();
                                    }
                                }
                            }
                            else if (reader.GetInt32("ban") == 1)
                            {
                                status = "永久封禁中";
                                word = "【我说那个佛祖啊，我刚刚在刷功德的时候，你有在偷看罢？】";
                            }
                            else if (reader.GetInt32("ban") == 2)
                            {
                                if (Global.time_now < reader.GetInt64("dt"))
                                {
                                    DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                                    time = time.AddSeconds(reader.GetInt64("dt"));
                                    status = $"暂时封禁中（直至：{TimeZoneInfo.ConvertTime(time, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"))}）";
                                    word = "【待封禁结束后，可发送“我的木鱼”解封】";
                                }
                                else
                                {
                                    status = "正常";
                                    word = "【敲电子木鱼，见机甲佛祖，取赛博真经】";
                                    using (var msc1 = new MySqlConnection(Global.connectstring))
                                    {
                                        await msc1.OpenAsync();
                                        MySqlCommand cmd1 = new()
                                        {
                                            Connection = msc1
                                        };
                                        cmd1.CommandText = "UPDATE woodenfish SET ban=0, time = @time_now WHERE uid = @uid";
                                        cmd1.Parameters.AddWithValue("@time_now", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
                                        cmd1.Parameters.AddWithValue("@uid", executor);
                                        await cmd1.ExecuteNonQueryAsync();
                                    }
                                }
                            }
                            await reader.CloseAsync();
                            cmd.CommandText = "SELECT * FROM woodenfish WHERE uid = @uid;";
                            reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                            while (reader.Read())
                            {
                                using (var msc1 = new MySqlConnection(Global.connectstring))
                                {
                                    await msc1.OpenAsync();
                                    MySqlCommand cmd1 = new()
                                    {
                                        Connection = msc1
                                    };
                                    cmd1.Parameters.Add("@uid", MySqlDbType.String);
                                    if (Global.time_now - reader.GetInt64("info_time") <= 10)
                                    {
                                        cmd1.CommandText = "UPDATE woodenfish SET info_count = @count WHERE uid = @uid";
                                        cmd1.Parameters.AddWithValue("@count", reader.GetInt32("info_count") + 1);
                                        cmd1.Parameters["@uid"].Value = executor;
                                        await cmd1.ExecuteNonQueryAsync();
                                    }
                                    else
                                    {
                                        cmd1.CommandText = "UPDATE woodenfish SET info_time = @time_now, info_count = 1 WHERE uid = @uid";
                                        cmd1.Parameters.AddWithValue("@time_now", Global.time_now);
                                        cmd1.Parameters["@uid"].Value = executor;
                                        await cmd1.ExecuteNonQueryAsync();
                                    }
                                }
                                string gongde;
                                if (reader.GetDouble("ee") >= 1)
                                {
                                    gongde = $"ee{Math.Truncate(10000 * reader.GetDouble("ee")) / 10000}（约 {Math.Round(Math.Pow(10, Math.Truncate(10000 * reader.GetDouble("ee")) / 10000)) / 100} 亿）";
                                }
                                else if (reader.GetDouble("e") >= 1)
                                {
                                    gongde = $"e{Math.Truncate(10000 * reader.GetDouble("e")) / 10000}（约 {Math.Round(Math.Pow(10, Math.Truncate(10000 * reader.GetDouble("e")) / 10000)) / 10000} 万）";
                                }
                                else
                                {
                                    gongde = $"{reader.GetInt64("gongde")}";
                                }
                                if (Global.time_now - (reader.GetInt64("info_time")) <= 10 && reader.GetInt32("info_count") > 5)
                                {
                                    try
                                    {
                                        MessageChain messageChain = new MessageChainBuilder()
                                            .At(executor)
                                            .Plain(" 宁踏马3分钟之内别想用我的木鱼辣（恼）")
                                            .Build();
                                        await MessageManager.SendGroupMessageAsync(group, messageChain);
                                        using (var msc1 = new MySqlConnection(Global.connectstring))
                                        {
                                            msc1.Open();
                                            MySqlCommand cmd1 = new()
                                            {
                                                Connection = msc1
                                            };
                                            cmd1.CommandText = "UPDATE woodenfish SET info_ctrl = @time, info_count = 0 WHERE uid = @uid";
                                            cmd1.Parameters.AddWithValue("@uid", executor);
                                            cmd1.Parameters.AddWithValue("@time", Global.time_now + 180);
                                            await cmd1.ExecuteNonQueryAsync();
                                        }
                                    }
                                    catch
                                    {
                                        Console.WriteLine("群消息发送失败");
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        MessageChain messageChain = new MessageChainBuilder()
                                            .At(executor)
                                            .Plain($@"
赛博账号：{executor}
账号状态：{status}
木鱼等级：{reader.GetInt32("level")}
涅槃值：{reader.GetDouble("nirvana")}
当前速度：{(int)Math.Round(60 * Math.Pow(0.95, reader.GetInt32("level")))} 秒/周期
当前功德：{gongde}
{word}")
                                            .Build();
                                        await MessageManager.SendGroupMessageAsync(group, messageChain);
                                    }
                                    catch
                                    {
                                        Console.WriteLine("群消息发送失败");
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "宁踏马害没注册？快发送“给我木鱼”注册罢！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        // 给我木鱼
        public static async void Register(string group, string executor)
        {
            Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                cmd.CommandText = "SELECT COUNT(*) uid FROM woodenfish WHERE uid = @uid;";
                cmd.Parameters.AddWithValue("@uid", executor);
                int i = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                if (i == 0)
                {
                    Random r = new();
                    cmd.CommandText = "INSERT INTO woodenfish(uid, time) VALUES (@uid, @time_now)";
                    cmd.Parameters.AddWithValue("@time_now", Global.time_now);
                    await cmd.ExecuteNonQueryAsync();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "注册成功辣！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "宁踏马不是注册过了吗？");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        // 敲木鱼
        public static async void Hit(string group, string executor)
        {
            Thread.Sleep(250);
            Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                cmd.CommandText = "SELECT COUNT(*) uid FROM woodenfish WHERE uid = @uid;";
                cmd.Parameters.AddWithValue("@uid", executor);
                int i = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                if (i == 1)
                {
                    cmd.CommandText = "SELECT * FROM woodenfish WHERE uid = @uid;";
                    MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        if (reader.GetInt32("ban") == 0)
                        {
                            Random r = new();
                            int random = r.Next(1, 6);
                            using (var msc1 = new MySqlConnection(Global.connectstring))
                            {
                                await msc1.OpenAsync();
                                MySqlCommand cmd1 = new()
                                {
                                    Connection = msc1
                                };
                                cmd1.Parameters.Add("@uid", MySqlDbType.String);
                                cmd1.Parameters.Add("@gongde", MySqlDbType.Int64);
                                if (Global.time_now - reader.GetInt64("end_time") <= 3)
                                {
                                    cmd1.CommandText = "UPDATE woodenfish SET hit_count = @count WHERE uid = @uid";
                                    cmd1.Parameters.AddWithValue("@count", reader.GetInt32("hit_count") + 1);
                                    cmd1.Parameters["@uid"].Value = executor;
                                    await cmd1.ExecuteNonQueryAsync();
                                }
                                else
                                {
                                    cmd1.CommandText = "UPDATE woodenfish SET end_time = @time_now, hit_count = 1 WHERE uid = @uid";
                                    cmd1.Parameters.AddWithValue("@time_now", Global.time_now);
                                    cmd1.Parameters["@uid"].Value = executor;
                                    await cmd1.ExecuteNonQueryAsync();
                                }
                                await reader.CloseAsync();
                                cmd.CommandText = "SELECT * FROM woodenfish WHERE uid = @uid;";
                                reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                                while (reader.Read())
                                {
                                    Thread.Sleep(250);
                                    if (Global.time_now - reader.GetInt64("end_time") <= 3 && reader.GetInt32("hit_count") > 5 && reader.GetInt32("total_ban") < 9)
                                    {
                                        try
                                        {
                                            MessageChain messageChain = new MessageChainBuilder()
                                                .At(executor)
                                                .Plain($" DoS佛祖是吧？这就给你封了（恼）（你被封禁 1 小时，功德扣掉 30%）")
                                                .Build();
                                            await MessageManager.SendGroupMessageAsync(group, messageChain);
                                        }
                                        catch
                                        {
                                            Console.WriteLine("群消息发送失败");
                                        }
                                        cmd1.CommandText = "UPDATE woodenfish SET ban = 2, total_ban = @ban, gongde = @gongde, dt = @time, hit_count = 0 WHERE uid = @uid";
                                        cmd1.Parameters["@uid"].Value = executor;
                                        cmd1.Parameters.AddWithValue("@ban", reader.GetInt32("total_ban") + 1);
                                        cmd1.Parameters["@gongde"].Value = (long)(reader.GetInt64("gongde") - reader.GetInt64("gongde") * 0.3);
                                        cmd1.Parameters.AddWithValue("@ee", reader.GetDouble("ee") - reader.GetDouble("ee") * 0.3);
                                        cmd1.Parameters.AddWithValue("@e", reader.GetDouble("e") - reader.GetDouble("e") * 0.3);
                                        cmd1.Parameters.AddWithValue("@time", Global.time_now + 3600);
                                        await cmd1.ExecuteNonQueryAsync();
                                        break;
                                    }
                                    else if (Global.time_now - reader.GetInt64("end_time") <= 5 && reader.GetInt32("hit_count") > 5 && reader.GetInt32("total_ban") >= 9)
                                    {
                                        try
                                        {
                                            MessageChain messageChain = new MessageChainBuilder()
                                            .At(executor)
                                            .Plain($" 多次DoS佛祖，死不悔改，罪加一等（恼）（你被永久封禁，等级、涅槃值重置，功德清零）")
                                            .Build();
                                            await MessageManager.SendGroupMessageAsync(group, messageChain);
                                            cmd1.CommandText = "UPDATE woodenfish SET ban = 1, level = 0, nirvana = 1, gongde = 0, ee = 0, e = 0, total_ban = 10, hit_count = 0 WHERE uid = @uid";
                                            cmd1.Parameters["@uid"].Value = executor;
                                            await cmd1.ExecuteNonQueryAsync();
                                            break;
                                        }
                                        catch
                                        {
                                            Console.WriteLine("群消息发送失败");
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            MessageChain messageChain = new MessageChainBuilder()
                                                .At(executor)
                                                .Plain($" 功德 +{random}")
                                                .Build();
                                            await MessageManager.SendGroupMessageAsync(group, messageChain);
                                            cmd1.CommandText = "UPDATE woodenfish SET gongde = @gongde WHERE uid = @uid";
                                            cmd1.Parameters["@gongde"].Value = reader.GetInt64("gongde") + random;
                                            cmd1.Parameters["@uid"].Value = executor;
                                            await cmd1.ExecuteNonQueryAsync();
                                            break;
                                        }
                                        catch
                                        {
                                            Console.WriteLine("群消息发送失败");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                MessageChain messageChain = new MessageChainBuilder()
                                    .At(executor)
                                    .Plain(" 宁踏马被佛祖封号辣（恼）")
                                    .Build();
                                await MessageManager.SendGroupMessageAsync(group, messageChain);
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "宁踏马害没注册？快发送“给我木鱼”注册罢！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        // 扣1佛祖陪你一起笑
        public static async void Laugh(string group, string executor)
        {
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                cmd.CommandText = "SELECT COUNT(*) uid FROM woodenfish WHERE uid = @uid;";
                cmd.Parameters.AddWithValue("@uid", executor);
                int i = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                if (i == 1)
                {
                    cmd.CommandText = "SELECT * FROM woodenfish WHERE uid = @uid;";
                    MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        if (reader.GetInt32("ban") == 0 && Math.Pow(10, reader.GetInt64("e")) >= 100)
                        {
                            using (var msc1 = new MySqlConnection(Global.connectstring))
                            {
                                await msc1.OpenAsync();
                                MySqlCommand cmd1 = new()
                                {
                                    Connection = msc1
                                };
                                MessageChain messageChain = new MessageChainBuilder()
                                .At(executor)
                                .Plain(" 哈*100")
                                .Build();
                                await MessageManager.SendGroupMessageAsync(group, messageChain);
                                cmd1.CommandText = "UPDATE woodenfish SET e = @e WHERE uid = @uid";
                                cmd1.Parameters.AddWithValue("@e", Math.Log10(Math.Pow(10, reader.GetInt64("e")) - 100));
                                cmd1.Parameters.AddWithValue("@uid", executor);
                                await cmd1.ExecuteNonQueryAsync();
                            }
                        }
                        else if (Math.Pow(10, reader.GetInt64("e")) < 100)
                        {
                            MessageChain messageChain = new MessageChainBuilder()
                                .At(executor)
                                .Plain(" 宁踏马功德不够，笑个毛啊（恼）")
                                .Build();
                            await MessageManager.SendGroupMessageAsync(group, messageChain);
                        }
                        else
                        {
                            MessageChain messageChain = new MessageChainBuilder()
                                .At(executor)
                                .Plain(" 宁踏马被佛祖封号辣（恼）")
                                .Build();
                            await MessageManager.SendGroupMessageAsync(group, messageChain);
                        }
                    }
                }
            }
        }
        // 升级木鱼
        public static async void Upgrade(string group, string executor)
        {
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                cmd.CommandText = "SELECT COUNT(*) uid FROM woodenfish WHERE uid = @uid;";
                cmd.Parameters.AddWithValue("@uid", executor);
                int i = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                if (i == 1)
                {
                    cmd.CommandText = "SELECT * FROM woodenfish WHERE uid = @uid;";
                    MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        if (reader.GetInt32("ban") == 0 && Math.Pow(10, reader.GetDouble("ee")) + reader.GetDouble("e") >= reader.GetInt32("level") + 2)
                        {
                            if (reader.GetDouble("e") >= reader.GetInt32("level") + 2)
                            {
                                using (var msc1 = new MySqlConnection(Global.connectstring))
                                {
                                    await msc1.OpenAsync();
                                    MySqlCommand cmd1 = new()
                                    {
                                        Connection = msc1
                                    };
                                    cmd1.CommandText = "UPDATE woodenfish SET level = @level, e = @e WHERE uid = @uid";
                                    cmd1.Parameters.AddWithValue("@level", reader.GetInt32("level") + 1);
                                    cmd1.Parameters.AddWithValue("@e", reader.GetDouble("e") - reader.GetInt32("level") + 2);
                                    cmd1.Parameters.AddWithValue("@uid", executor);
                                    await cmd1.ExecuteNonQueryAsync();
                                }
                            }
                            else
                            {
                                using (var msc1 = new MySqlConnection(Global.connectstring))
                                {
                                    await msc1.OpenAsync();
                                    MySqlCommand cmd1 = new()
                                    {
                                        Connection = msc1
                                    };
                                    cmd1.CommandText = "UPDATE woodenfish SET level = @level, ee = @ee, e = 0 WHERE uid = @uid";
                                    cmd1.Parameters.AddWithValue("@level", reader.GetInt32("level") + 1);
                                    cmd1.Parameters.AddWithValue("@ee", Math.Log10(Math.Pow(10, reader.GetDouble("ee")) + reader.GetDouble("e") - reader.GetInt32("level") + 2));
                                    cmd1.Parameters.AddWithValue("@uid", executor);
                                    await cmd1.ExecuteNonQueryAsync();
                                    MessageChain messageChain = new MessageChainBuilder()
                                        .At(executor)
                                        .Plain(" 木鱼升级成功辣（喜）")
                                        .Build();
                                    await MessageManager.SendGroupMessageAsync(group, messageChain);
                                }
                            }
                        }
                        else if (Math.Pow(10, reader.GetDouble("ee")) + reader.GetDouble("e") < reader.GetInt32("level") + 2)
                        {
                            MessageChain messageChain = new MessageChainBuilder()
                                .At(executor)
                                .Plain(" 宁踏马功德不够，升级个毛啊（恼）")
                                .Build();
                            await MessageManager.SendGroupMessageAsync(group, messageChain);
                        }
                        else
                        {
                            MessageChain messageChain = new MessageChainBuilder()
                                .At(executor)
                                .Plain(" 宁踏马被佛祖封号辣（恼）")
                                .Build();
                            await MessageManager.SendGroupMessageAsync(group, messageChain);
                        }
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "宁踏马害没注册？快发送“给我木鱼”注册罢！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        // 涅槃重生
        public static async void Nirvana(string group, string executor)
        {
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                cmd.CommandText = "SELECT COUNT(*) uid FROM woodenfish WHERE uid = @uid;";
                cmd.Parameters.AddWithValue("@uid", executor);
                int i = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                if (i == 1)
                {
                    cmd.CommandText = "SELECT * FROM woodenfish WHERE uid = @uid;";
                    MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        if (reader.GetInt32("ban") == 0 && reader.GetDouble("ee") >= 10 * reader.GetDouble("nirvana"))
                        {
                            using (var msc1 = new MySqlConnection(Global.connectstring))
                            {
                                await msc1.OpenAsync();
                                MySqlCommand cmd1 = new()
                                {
                                    Connection = msc1
                                };
                                cmd1.CommandText = "UPDATE woodenfish SET nirvana = @nirvana, level = 0, ee = 0, e = 0, gongde = 0 WHERE uid = @uid";
                                cmd1.Parameters.AddWithValue("@nirvana", reader.GetDouble("nirvana") + 0.05);
                                cmd1.Parameters.AddWithValue("@uid", executor);
                                await cmd1.ExecuteNonQueryAsync();
                                MessageChain messageChain = new MessageChainBuilder()
                                .At(executor)
                                .Plain(" 涅槃重生，功德圆满（喜）")
                                .Build();
                                await MessageManager.SendGroupMessageAsync(group, messageChain);
                            }
                        }
                        else if (reader.GetDouble("ee") < 10 * reader.GetDouble("nirvana"))
                        {
                            MessageChain messageChain = new MessageChainBuilder()
                                .At(executor)
                                .Plain(" 宁踏马功德不够，涅槃重生个毛啊（恼）")
                                .Build();
                            await MessageManager.SendGroupMessageAsync(group, messageChain);
                        }
                        else
                        {
                            MessageChain messageChain = new MessageChainBuilder()
                                .At(executor)
                                .Plain(" 宁踏马被佛祖封号辣（恼）")
                                .Build();
                            await MessageManager.SendGroupMessageAsync(group, messageChain);
                        }
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "宁踏马害没注册？快发送“给我木鱼”注册罢！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        // 功德榜
        public static async void Leaderboard(string group, string executor)
        {
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                cmd.CommandText = "SELECT COUNT(*) uid FROM woodenfish;";
                int i = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                if (i >= 1)
                {
                    List<string> uids = new();
                    cmd.CommandText = "SELECT * FROM woodenfish ORDER BY ee DESC;";
                    MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        if (reader.GetDouble("ee") >= 1)
                        {
                            string uid = reader.GetString("uid");
                            uids?.Add(uid);
                        }
                    }
                    await reader.CloseAsync();
                    cmd.CommandText = "SELECT * FROM woodenfish ORDER BY e DESC;";
                    reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        if (reader.GetDouble("e") >= 1 && reader.GetDouble("ee") < 1)
                        {
                            string uid = reader.GetString("uid");
                            uids?.Add(uid);
                        }
                    }
                    await reader.CloseAsync();
                    cmd.CommandText = "SELECT * FROM woodenfish ORDER BY gongde DESC;";
                    reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        if (reader.GetDouble("e") < 1 && reader.GetDouble("ee") < 1)
                        {
                            string uid = reader.GetString("uid");
                            uids?.Add(uid);
                        }
                    }
                    await reader.CloseAsync();
                    MessageChain messageChain = new MessageChainBuilder()
                        .At(executor)
                        .Plain("\n功德榜\n赛博账号 --- 功德")
                        .Build();
                    cmd.Parameters.Add("@uid", MySqlDbType.String);
                    foreach (string uid in uids!)
                    {
                        cmd.CommandText = "SELECT * FROM woodenfish WHERE uid = @uid;";
                        cmd.Parameters["@uid"].Value = uid;
                        reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            if (reader.GetDouble("ee") >= 1)
                            {
                                MessageChain messageChain1 = new MessageChainBuilder()
                                .Plain($"\n{uid} --- ee{Math.Truncate(10000 * reader.GetDouble("ee")) / 10000}")
                                .Build();
                                foreach (MessageBase message in messageChain1)
                                {
                                    messageChain.Add(message);
                                }
                                break;
                            }
                            else if (reader.GetDouble("e") >= 1 && reader.GetDouble("ee") < 1)
                            {
                                MessageChain messageChain1 = new MessageChainBuilder()
                                .Plain($"\n{uid} --- e{Math.Truncate(10000 * reader.GetDouble("e")) / 10000}")
                                .Build();
                                foreach (MessageBase message in messageChain1)
                                {
                                    messageChain.Add(message);
                                }
                                break;
                            }
                            else if (reader.GetInt64("gongde") >= 1 && reader.GetDouble("e") < 1 && reader.GetDouble("ee") < 1)
                            {
                                MessageChain messageChain1 = new MessageChainBuilder()
                                .Plain($"\n{uid} --- {reader.GetInt64("gongde")}")
                                .Build();
                                foreach (MessageBase message in messageChain1)
                                {
                                    messageChain.Add(message);
                                }
                                break;
                            }
                        }
                        await reader.CloseAsync();
                    }
                    await reader.CloseAsync();
                    await MessageManager.SendGroupMessageAsync(group, messageChain);
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "目前还没有人注册赛博账号！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        // 封禁榜
        public static async void BanLeaderboard(string group, string executor)
        {
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                cmd.CommandText = "SELECT COUNT(*) uid FROM woodenfish;";
                int i = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                if (i >= 1)
                {
                    List<string> uids = new();
                    cmd.CommandText = "SELECT * FROM woodenfish ORDER BY total_ban DESC;";
                    MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        string uid = reader.GetString("uid");
                        uids?.Add(uid);
                    }
                    await reader.CloseAsync();
                    MessageChain messageChain = new MessageChainBuilder()
                        .At(executor)
                        .Plain("\n封禁榜\n赛博账号 --- 累计封禁次数")
                        .Build();
                    cmd.Parameters.Add("@uid", MySqlDbType.String);
                    foreach (string uid in uids!)
                    {
                        cmd.CommandText = "SELECT * FROM woodenfish WHERE uid = @uid;";
                        cmd.Parameters["@uid"].Value = uid;
                        reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            if (reader.GetInt32("total_ban") >= 1)
                            {
                                MessageChain messageChain1 = new MessageChainBuilder()
                                .Plain($"\n{uid} --- {reader.GetInt64("total_ban")}")
                                .Build();
                                foreach (MessageBase message in messageChain1)
                                {
                                    messageChain.Add(message);
                                }
                            }
                        }
                        await reader.CloseAsync();
                    }
                    await reader.CloseAsync();
                    await MessageManager.SendGroupMessageAsync(group, messageChain);
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "目前还没有人注册赛博账号！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        public static async Task GetExp()
        {
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                List<string> uids = new();
                cmd.CommandText = "SELECT * FROM woodenfish;";
                MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    string uid = reader.GetString("uid");
                    if (uids == null || !uids.Contains(uid))
                    {
                        uids?.Add(uid);
                    }
                }
                await reader.CloseAsync();
                cmd.Parameters.Add("@uid", MySqlDbType.String);
                while (true)
                {
                    Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    if (uids != null)
                    {
                        foreach (string uid in uids)
                        {
                            cmd.CommandText = "SELECT * FROM woodenfish WHERE uid = @uid;";
                            cmd.Parameters["@uid"].Value = uid;
                            reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                            while (await reader.ReadAsync())
                            {
                                int cycle_speed;
                                if (60 * Math.Pow(0.95, reader.GetInt32("level")) <= 1)
                                {
                                    cycle_speed = 1;
                                }
                                else
                                {
                                    cycle_speed = (int)Math.Round(60 * Math.Pow(0.95, reader.GetInt32("level")));
                                }
                                if (reader.GetInt32("ban") == 0 && Global.time_now - reader.GetInt64("time") >= cycle_speed)
                                {
                                    using (var msc1 = new MySqlConnection(Global.connectstring))
                                    {
                                        await msc1.OpenAsync();
                                        MySqlCommand cmd1 = new()
                                        {
                                            Connection = msc1
                                        };
                                        cmd1.CommandText = "UPDATE woodenfish SET time = @time_now, e = @e WHERE uid = @uid";
                                        cmd1.Parameters.AddWithValue("@time_now", Global.time_now);
                                        cmd1.Parameters.AddWithValue("@e", reader.GetInt64("e") * reader.GetDouble("nirvana") + Math.Log10(reader.GetInt32("level")));
                                        cmd1.Parameters.AddWithValue("@uid", uid);
                                        await cmd1.ExecuteNonQueryAsync();
                                    }
                                    break;
                                }
                            }
                            await reader.CloseAsync();
                        }
                    }
                    Thread.Sleep(500);
                }
            }
        }
    }
}