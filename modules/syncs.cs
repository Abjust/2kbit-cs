using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Net_2kBot.Modules
{
    public static class Syncs
    {
        // 从Hanbot同步黑名单
        public static async void Sync(MessageReceiverBase @base, string group, string executor)
        {
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            await msc.OpenAsync();
            if (@base is GroupMessageReceiver receiver)
            {
                if (Global.g_ops != null && Global.g_ops.Contains(executor))
                {
                    RestClient client = new($"{Global.api}/blacklist");
                    RestRequest request = new("look", Method.Get);
                    request.Timeout = 10000;
                    RestResponse response = await client.ExecuteAsync(request);
                    JObject jo = (JObject)JsonConvert.DeserializeObject(response.Content!)!;  //正常获取jobject
                    foreach (string? s in jo["data"]!)
                    {
                        cmd.CommandText = $"INSERT INTO g_blocklist (qid) VALUES ({s});";
                        await cmd.ExecuteNonQueryAsync();
                    }
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(receiver.GroupId, "从Hanbot同步黑名单成功！");
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
                        await MessageManager.SendGroupMessageAsync(group, "你不是全局机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
                await msc.CloseAsync();
            }
        }
        // 将黑名单反向同步到Hanbot
        public static async void Rsync(MessageReceiverBase @base, string group, string executor)
        {
            if (@base is GroupMessageReceiver receiver)
            {
                if (Global.g_ops != null && Global.g_ops.Contains(executor))
                {
                    RestClient client = new($"{Global.api}/blacklist");
                    RestRequest request = new("look", Method.Get);
                    request.Timeout = 10000;
                    RestResponse response = await client.ExecuteAsync(request);
                    JObject jo = (JObject)JsonConvert.DeserializeObject(response.Content!)!;  //正常获取jobject
                    List<string> blocklist2 = new();
                    if (Global.g_blocklist != null)
                    {
                        for (int i = 0; i < Global.g_blocklist.Count; i++)
                        {
                            if (!jo["data"]!.Contains(Global.g_blocklist[i]))
                            {
                                RestClient client1 = new($"{Global.api}/blacklist");
                                RestRequest request1 = new("up?uid=" + Global.g_blocklist[i] + "&key=" + Global.api_key, Method.Post);
                                request.Timeout = 10000;
                                await client1.ExecuteAsync(request1);
                            }
                        }
                        foreach (string? s in jo["data"]!)
                        {
                            if (s != null)
                            {
                                blocklist2.Add(s);
                            }
                        }
                        blocklist2.Remove("");
                        var diff = new HashSet<string>(Global.g_blocklist);
                        diff.SymmetricExceptWith(blocklist2);
                        string diff1 = String.Join(", ", diff);
                        string[] diff2 = diff1.Split(",");
                        foreach (string s in diff2)
                        {
                            RestClient client2 = new($"{Global.api}/blacklist");
                            RestRequest request2 = new($"del?uid={s}&key={Global.api_key}", Method.Delete);
                            request.Timeout = 10000;
                            await client2.ExecuteAsync(request2);
                        }
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(receiver.GroupId, "将黑名单反向同步给Hanbot成功！");
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
                        await MessageManager.SendGroupMessageAsync(group, "你不是全局机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        // 合并黑名单+双向同步
        public static async void Merge(MessageReceiverBase @base, string group, string executor)
        {
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            await msc.OpenAsync();
            if (@base is GroupMessageReceiver receiver)
            {
                if (Global.g_ops != null && Global.g_ops.Contains(executor))
                {
                    RestClient client = new($"{Global.api}/blacklist");
                    RestRequest request = new("look", Method.Get);
                    request.Timeout = 10000;
                    RestResponse response = await client.ExecuteAsync(request);
                    JObject jo = (JObject)JsonConvert.DeserializeObject(response.Content!)!;  //正常获取jobject
                    List<string> blocklist2 = new();
                    if (Global.g_blocklist != null)
                    {
                        foreach (string? s in jo["data"]!)
                        {
                            if (s != null)
                            {
                                blocklist2.Add(s);
                            }
                        }
                        blocklist2.Remove("");
                        IEnumerable<string> union = blocklist2.Union(Global.g_blocklist);
                        foreach (string s in union)
                        {
                            if (!jo["data"]!.Contains(s))
                            {
                                RestClient client1 = new($"{Global.api}/blacklist");
                                RestRequest request1 = new($"up?uid={s}&key={Global.api_key}", Method.Post);
                                request.Timeout = 10000;
                                await client1.ExecuteAsync(request1);
                            }
                            else if (!Global.g_blocklist.Contains(s))
                            {
                                cmd.CommandText = $"INSERT INTO g_blocklist (qid) VALUES ({s});";
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(receiver.GroupId, "合并黑名单成功！");
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
                        await MessageManager.SendGroupMessageAsync(group, "你不是全局机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
                await msc.CloseAsync();
            }
        }
    }
}