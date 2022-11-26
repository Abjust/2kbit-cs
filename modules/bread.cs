using Mirai.Net.Data.Messages;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using MySql.Data.MySqlClient;

namespace Net_2kBot.Modules
{
    public static class Bread
    {
        // 给2kbot面包
        public static async void Give(string group, int number)
        {
            // 连接数据库
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
            cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
            try
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (number + reader.GetInt32("breads") <= 32 * Math.Pow(4, reader.GetInt32("factory_level") - 1))
                {
                    cmd1.CommandText = $"UPDATE bread SET breads = {reader.GetInt32("breads") + number} WHERE gid = {group};";
                    cmd1.ExecuteNonQuery();
                    reader.Close();
                    cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"现在库存有 {reader.GetInt32("breads")} 块面包辣！");
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
                        await MessageManager.SendGroupMessageAsync(group, "抱歉，库存已经满了。。。");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
                reader.Close();
            }
            catch
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, "本群还没有面包厂！（你可尝试再发送一次）");
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            }
            msc.Close();
            msc1.Close();
        }
        // 给我面包
        public static async void Get(string group, int number)
        {
            // 连接数据库
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
            cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
            try
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (reader.GetInt32("breads") >= number)
                {
                    if (reader.GetInt32("bread_diversity") == 1)
                    {
                        List<string> bread_types = new List<string>
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
                        cmd1.CommandText = $"UPDATE bread SET breads = {reader.GetInt32("breads") - number} WHERE gid = {group};";
                        cmd1.ExecuteNonQuery();
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
                        cmd1.CommandText = $"UPDATE bread SET breads = {reader.GetInt32("breads") - number} WHERE gid = {group};";
                        cmd1.ExecuteNonQuery();
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
                reader.Close();
            }
            catch
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, "本群还没有面包厂！（你可尝试再发送一次）");
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            }
            msc.Close();
            msc1.Close();
        }
        // 查询面包库存
        public static async void Query(string group)
        {
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            msc.Open();
            cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
            try
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, $"现在库存有 {reader.GetInt32("breads")} 块面包");
                } 
                catch 
                {
                    Console.WriteLine("群消息发送失败");
                }
                reader.Close();
            }
            catch
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, "本群还没有面包厂！（你可尝试再发送一次）");
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            }
            msc.Close();
        }
        // 多样化生产
        public static async void Diversity(string group, int status)
        {
            // 连接数据库
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
            // 判断数据是否存在
            cmd.CommandText = $"SELECT COUNT(*) gid FROM bread WHERE gid = {group};";
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            if (i == 1)
            {
                cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (reader.GetInt32("breads") == 0)
                {
                    if (status == 1)
                    {
                        cmd1.CommandText = $"UPDATE bread SET bread_diversity = 1 WHERE gid = {group};";
                        cmd1.ExecuteNonQuery();
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
                        cmd1.CommandText = $"UPDATE bread SET bread_diversity = 0 WHERE gid = {group};";
                        cmd1.ExecuteNonQuery();
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
                msc.Close();
                msc1.Close();
            }
            else
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, "本群还没有面包厂！（你可尝试再发送一次）");
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            }
        }
        // 获取经验
        public static async void GetExp(string group)
        {
            // 连接数据库
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
            // 判断数据是否存在
            cmd.CommandText = $"SELECT COUNT(*) gid FROM bread WHERE gid = {group};";
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            if (i == 1)
            {
                cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                int maxexp_formula = (int)(300 * Math.Pow(2, reader.GetInt32("factory_level") - 1));
                Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                if (Global.time_now - reader.GetInt64("last_expfull") >= 86400)
                {
                    if (Global.time_now - reader.GetInt64("last_expgain") >= 86400)
                    {
                        cmd1.CommandText = $"UPDATE bread SET exp_gained_today = 0, last_expgain = {Global.time_now} WHERE gid = {group};";
                        cmd1.ExecuteNonQuery();
                        if (reader.GetInt32("exp_gained_today") <= maxexp_formula)
                        {
                            cmd1.CommandText = $"UPDATE bread SET factory_exp = {reader.GetInt32("factory_exp") + 1}, exp_gained_today = {reader.GetInt32("exp_gained_today") + 1} WHERE gid = {group};";
                            cmd1.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd1.CommandText = $"UPDATE bread SET last_expfull = {Global.time_now}, exp_gained_today = 0 WHERE gid = {group};";
                            cmd1.ExecuteNonQuery();
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(group, "本群已达到今日获取经验上限！");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    else
                    {
                        if (reader.GetInt32("exp_gained_today") < maxexp_formula)
                        {
                            cmd1.CommandText = $"UPDATE bread SET factory_exp = {reader.GetInt32("factory_exp") + 1}, exp_gained_today = {reader.GetInt32("exp_gained_today") + 1} WHERE gid = {group};";
                            cmd1.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd1.CommandText = $"UPDATE bread SET last_expfull = {Global.time_now}, exp_gained_today = 0 WHERE gid = {group};";
                            cmd1.ExecuteNonQuery();
                            try 
                            {
                                await MessageManager.SendGroupMessageAsync(group, "本群已达到今日获取经验上限！");
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
        // 升级工厂
        public static async void UpgradeFactory(string group)
        {
            // 连接数据库
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
            // 判断数据是否存在
            cmd.CommandText = $"SELECT COUNT(*) gid FROM bread WHERE gid = {group};";
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            if (i == 1)
            {
                cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                int exp_formula = (int)(900 * Math.Pow(2, reader.GetInt32("factory_level") -1));
                if (reader.GetInt32("factory_exp") >= exp_formula)
                {
                    cmd1.CommandText = $"UPDATE bread SET factory_level = {reader.GetInt32("factory_level") + 1}, factory_exp = {reader.GetInt32("factory_exp") - exp_formula} WHERE gid = {group};";
                    cmd1.ExecuteNonQuery();
                    reader.Close();
                    cmd.CommandText = $"SELECT * FROM bread WHERE gid = {group};";
                    reader = cmd.ExecuteReader();
                    reader.Read();
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
                reader.Close();
            }
            msc.Close();
            msc1.Close();
        }
    }
}
