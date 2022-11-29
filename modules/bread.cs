using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Data.Shared;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using MySql.Data.MySqlClient;
using System.Reflection.PortableExecutable;

namespace Net_2kBot.Modules
{
    public static class Bread
    {
        // 建造面包厂
        public static async void BuildFactory(string group)
        {
            // 连接数据库
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                // 判断数据是否存在
                cmd.CommandText = $"SELECT COUNT(*) gid FROM bread WHERE gid = {group};";
                int i = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                if (i == 0)
                {
                    cmd.CommandText = $"INSERT INTO bread (qid) VALUES ({group});";
                    await cmd.ExecuteNonQueryAsync();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "成功为本群建造面包厂！");
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
                        await MessageManager.SendGroupMessageAsync(group, "本群已经有面包厂了！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        // 给2kbot面包
        public static async void Give(string group, int number)
        {
            // 连接数据库
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
                try
                {
                    MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    if (number + reader.GetInt32("breads") <= 32 * Math.Pow(4, reader.GetInt32("factory_level") - 1))
                    {
                        using (var msc1 = new MySqlConnection(Global.connectstring))
                        {
                            await msc1.OpenAsync();
                            MySqlCommand cmd1 = new()
                            {
                                Connection = msc1
                            };
                            cmd1.CommandText = $"UPDATE bread SET breads = {reader.GetInt32("breads") + number} WHERE gid = {group};";
                            await cmd1.ExecuteNonQueryAsync();
                            await reader.CloseAsync();
                            cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
                            reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                            await reader.ReadAsync();
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(group, $"现在库存有 {reader.GetInt32("breads")} 块面包辣！");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, "抱歉，库存已经满了。。。");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                    await reader.CloseAsync();
                }
                catch
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "本群还没有面包厂！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        // 给我面包
        public static async void Get(string group, int number)
        {
            // 连接数据库
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
                try
                {
                    MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    if (reader.GetInt32("breads") >= number)
                    {
                        if (reader.GetInt32("bread_diversity") == 1)
                        {
                            List<string> bread_types = new()
                        {
                        "🍞",
                        "🥖",
                        "🥐",
                        "🥯",
                        "🍩"
                        };
                            Random r = new();
                            int random = r.Next(bread_types.Count);
                            MessageChain? messageChain = new MessageChainBuilder()
                           .Plain($"{bread_types[random]}*{number}")
                           .Build();
                            using (var msc1 = new MySqlConnection(Global.connectstring))
                            {
                                await msc1.OpenAsync();
                                MySqlCommand cmd1 = new()
                                {
                                    Connection = msc1
                                };
                                cmd1.CommandText = $"UPDATE bread SET breads = {reader.GetInt32("breads") - number} WHERE gid = {group};";
                                await cmd1.ExecuteNonQueryAsync();
                            }    
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(group, messageChain);
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
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
                                cmd1.CommandText = $"UPDATE bread SET breads = {reader.GetInt32("breads") - number} WHERE gid = {group};";
                                await cmd1.ExecuteNonQueryAsync();
                            }
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(group, $"🍞*{number}");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, "抱歉，面包不够了。。。");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                    await reader.DisposeAsync();
                }
                catch
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "本群还没有面包厂！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        // 查询面包库存
        public static async void Query(string group)
        {
            // 连接数据库
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                try
                {
                    MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"现在库存有 {reader.GetInt32("breads")} 块面包");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                    await reader.CloseAsync();
                }
                catch
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "本群还没有面包厂！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }  
        }
        // 多样化生产
        public static async void Diversity(string group, int status)
        {
            // 连接数据库
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                // 判断数据是否存在
                cmd.CommandText = $"SELECT COUNT(*) gid FROM bread WHERE gid = {group};";
                int i = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                if (i == 1)
                {
                    cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
                    MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    if (reader.GetInt32("breads") == 0)
                    {
                        if (status == 1)
                        {
                            using (var msc1 = new MySqlConnection(Global.connectstring))
                            {
                                await msc1.OpenAsync();
                                MySqlCommand cmd1 = new()
                                {
                                    Connection = msc1
                                };
                                cmd1.CommandText = $"UPDATE bread SET bread_diversity = 1 WHERE gid = {group};";
                                await cmd1.ExecuteNonQueryAsync();
                            }
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(group, "已为本群启用多样化生产！");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
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
                                cmd1.CommandText = $"UPDATE bread SET bread_diversity = 0 WHERE gid = {group};";
                                await cmd1.ExecuteNonQueryAsync();
                            }
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(group, "已为本群禁用多样化生产！");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, "你必须先清空库存，才能修改生产模式！");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "本群还没有面包厂！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        // 获取经验
        public static async void GetExp(MessageReceiverBase @base)
        {
            if (@base is GroupMessageReceiver receiver)
            {
                if ((Global.ignores == null || Global.ignores.Contains($"{receiver.GroupId}_{receiver.Sender.Id}") == false) && (Global.g_ignores == null || Global.g_ignores.Contains(receiver.Sender.Id) == false))
                {
                    // 连接数据库
                    using (var msc = new MySqlConnection(Global.connectstring))
                    {
                        await msc.OpenAsync();
                        MySqlCommand cmd = new()
                        {
                            Connection = msc
                        };
                        // 判断数据是否存在
                        cmd.CommandText = $"SELECT COUNT(*) gid FROM bread WHERE gid = {receiver.GroupId};";
                        int i = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        if (i == 1)
                        {
                            cmd.CommandText = $"SELECT * FROM bread WHERE gid = {receiver.GroupId};";
                            MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                            await reader.ReadAsync();
                            int maxexp_formula = (int)(300 * Math.Pow(2, reader.GetInt32("factory_level") - 1));
                            Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                            if (Global.time_now - reader.GetInt64("last_expfull") >= 86400)
                            {
                                if (Global.time_now - reader.GetInt64("last_expgain") >= 86400)
                                {
                                    using (var msc1 = new MySqlConnection(Global.connectstring))
                                    {
                                        await msc1.OpenAsync();
                                        MySqlCommand cmd1 = new()
                                        {
                                            Connection = msc1
                                        };
                                        cmd1.CommandText = $"UPDATE bread SET exp_gained_today = 0, last_expgain = {Global.time_now} WHERE gid = {receiver.GroupId};";
                                        await cmd1.ExecuteNonQueryAsync();
                                        if (reader.GetInt32("exp_gained_today") <= maxexp_formula)
                                        {
                                            cmd1.CommandText = $"UPDATE bread SET factory_exp = {reader.GetInt32("factory_exp") + 1}, exp_gained_today = {reader.GetInt32("exp_gained_today") + 1} WHERE gid = {receiver.GroupId};";
                                            await cmd1.ExecuteNonQueryAsync();
                                        }
                                        else
                                        {
                                            cmd1.CommandText = $"UPDATE bread SET last_expfull = {Global.time_now}, exp_gained_today = 0 WHERE gid = {receiver.GroupId};";
                                            await cmd1.ExecuteNonQueryAsync();
                                            try
                                            {
                                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, "本群已达到今日获取经验上限！");
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
                                    if (reader.GetInt32("exp_gained_today") < maxexp_formula)
                                    {
                                        using (var msc1 = new MySqlConnection(Global.connectstring))
                                        {
                                            await msc1.OpenAsync();
                                            MySqlCommand cmd1 = new()
                                            {
                                                Connection = msc1
                                            };
                                            cmd1.CommandText = $"UPDATE bread SET factory_exp = {reader.GetInt32("factory_exp") + 1}, exp_gained_today = {reader.GetInt32("exp_gained_today") + 1} WHERE gid = {receiver.GroupId};";
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
                                            cmd1.CommandText = $"UPDATE bread SET last_expfull = {Global.time_now}, exp_gained_today = 0 WHERE gid = {receiver.GroupId};";
                                            await cmd1.ExecuteNonQueryAsync();
                                        }   
                                        try
                                        {
                                            await MessageManager.SendGroupMessageAsync(receiver.GroupId, "本群已达到今日获取经验上限！");
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
                }
            }
        }
        // 升级工厂
        public static async void UpgradeFactory(string group)
        {
            // 连接数据库
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                // 判断数据是否存在
                cmd.CommandText = $"SELECT COUNT(*) gid FROM bread WHERE gid = {group};";
                int i = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                if (i == 1)
                {
                    cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
                    MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    int exp_formula = (int)(900 * Math.Pow(2, reader.GetInt32("factory_level") - 1));
                    if (reader.GetInt32("factory_exp") >= exp_formula)
                    {
                        using (var msc1 = new MySqlConnection(Global.connectstring))
                        {
                            await msc1.OpenAsync();
                            MySqlCommand cmd1 = new()
                            {
                                Connection = msc1
                            };
                            cmd1.CommandText = $"UPDATE bread SET factory_level = {reader.GetInt32("factory_level") + 1}, factory_exp = {reader.GetInt32("factory_exp") - exp_formula} WHERE gid = {group};";
                            await cmd1.ExecuteNonQueryAsync();
                        }
                        await reader.CloseAsync();
                        cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
                        reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                        await reader.ReadAsync();
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, $"恭喜，本群面包厂升级成功辣！现在面包厂等级是 {reader.GetInt32("factory_level")} 级");
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
                            await MessageManager.SendGroupMessageAsync(group, $"很抱歉，目前本群还需要 {exp_formula - reader.GetInt32("factory_exp")} 经验才能升级");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "本群还没有面包厂！");
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
