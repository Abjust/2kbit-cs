using Manganese.Text;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Utils.Scaffolds;
using MySql.Data.MySqlClient;

namespace Net_2kBot.Modules
{
    public class Repeat
    {
        public static async void Execute(MessageReceiverBase @base)
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
            string[] repeatwords =
                {
                    "114514",
                    "1919810",
                    "1145141919810",
                    "ccc",
                    "c",
                    "草",
                    "tcl",
                    "?",
                    "。",
                    "？",
                    "e",
                    "额",
                    "呃",
                    "6",
                    "666"
                };
            if (@base is GroupMessageReceiver receiver)
            {
                // 复读机
                if (receiver.MessageChain.GetPlainMessage().StartsWith("/echo"))
                {
                    string[] result = receiver.MessageChain.GetPlainMessage().Split(" ");
                    if (result.Length > 1)
                    {
                        try
                        {
                            string results = "";
                            if ((Global.ignores == null || Global.ignores.Contains($"{receiver.GroupId}_{receiver.Sender.Id}") == false) && (Global.g_ignores == null || Global.g_ignores.Contains(receiver.Sender.Id) == false))
                            {
                                for (int i = 1; i < result.Length; i++)
                                {
                                    if (i == 1)
                                    {
                                        results = result[i];
                                    }
                                    else
                                    {
                                        results = results + " " + result[i];
                                    }
                                }
                            }
                            try
                            {
                                await receiver.SendMessageAsync(results);
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                        catch
                        {
                            try
                            {
                                await receiver.SendMessageAsync("油饼食不食？");
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
                            await receiver.SendMessageAsync("你个sb难道没发觉到少了些什么？");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                }
                // 主动复读
                else if ((Global.ignores == null || Global.ignores.Contains($"{receiver.GroupId}_{receiver.Sender.Id}") == false) && (Global.g_ignores == null || Global.g_ignores.Contains(receiver.Sender.Id) == false))
                {
                    foreach (string item in repeatwords)
                    {
                        if (item.Equals(receiver.MessageChain.GetPlainMessage()))
                        {
                            Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                            // 判断数据是否存在
                            cmd.CommandText = $"SELECT COUNT(*) qid FROM repeatctrl WHERE qid = {receiver.Sender.Id} AND gid = {receiver.GroupId};";
                            int i = Convert.ToInt32(cmd.ExecuteScalar());
                            // 如不存在便创建
                            while (i == 0)
                            {
                                cmd.CommandText = $"INSERT INTO repeatctrl (qid,gid) VALUES ({receiver.Sender.Id},{receiver.GroupId});";
                                cmd.ExecuteNonQuery();
                                break;
                            }
                            cmd.CommandText = $"SELECT * FROM repeatctrl WHERE qid = {receiver.Sender.Id} AND gid = {receiver.GroupId};";
                            MySqlDataReader reader = cmd.ExecuteReader();
                            reader.Read();
                            if (Global.time_now - reader.GetInt64("last_repeatctrl") >= Global.repeat_cd)
                            {
                                if (Global.time_now - reader.GetInt64("last_repeat") <= Global.repeat_interval)
                                {
                                    if (reader.GetInt32("repeat_count") <= Global.repeat_threshold)
                                    {
                                        cmd1.CommandText = $"UPDATE repeatctrl SET last_repeat = {Global.time_now} WHERE qid = {receiver.Sender.Id} AND gid = {receiver.GroupId};";
                                        cmd1.ExecuteNonQuery();
                                        cmd1.CommandText = $"UPDATE repeatctrl SET repeat_count = {reader.GetInt32("repeat_count") + 1} WHERE qid = {receiver.Sender.Id} AND gid = {receiver.GroupId};";
                                        cmd1.ExecuteNonQuery();
                                        await receiver.SendMessageAsync(receiver.MessageChain.GetPlainMessage());
                                        reader.Close();
                                        msc.Close();
                                        msc1.Close();
                                    }
                                    else
                                    {
                                        try
                                        {
                                            var messageChain = new MessageChainBuilder()
                                           .At(receiver.Sender.Id)
                                           .Plain($" 你话太多了（恼）（你的消息将在 {Global.repeat_cd} 秒内不被复读）")
                                           .Build();
                                            await receiver.SendMessageAsync(messageChain);
                                            reader.Close();
                                            msc.Close();
                                            msc1.Close();
                                        }
                                        catch
                                        {
                                            Console.WriteLine("群消息发送失败");
                                        }
                                        cmd1.CommandText = $"UPDATE repeatctrl SET last_repeatctrl = {Global.time_now}, repeat_count = 0 WHERE qid = {receiver.Sender.Id} AND gid = {receiver.GroupId};";
                                        cmd1.ExecuteNonQuery();
                                        reader.Close();
                                        msc.Close();
                                        msc1.Close();
                                    }
                                }
                                else
                                {
                                    cmd1.CommandText = $"UPDATE repeatctrl SET repeat_count = 0, last_repeat = {Global.time_now} WHERE qid = {receiver.Sender.Id} AND gid = {receiver.GroupId};";
                                    cmd1.ExecuteNonQuery();
                                    cmd1.CommandText = $"UPDATE repeatctrl SET repeat_count = {reader.GetString("repeat_count").ToInt32() + 1} WHERE qid = {receiver.Sender.Id} AND gid = {receiver.GroupId};";
                                    cmd1.ExecuteNonQuery();
                                    await receiver.SendMessageAsync(receiver.MessageChain.GetPlainMessage());
                                    reader.Close();
                                    msc.Close();
                                    msc1.Close();
                                }
                            }
                            reader.Close();
                            msc.Close();
                            msc1.Close();
                        }
                    }
                }
            }
        }
    }
}