// 2kbit C# Edition，2kbit的C#分支版本
// Copyright(C) 2022 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

// 致所有构建及修改2kbit代码片段的用户：作者（Abjust）并不承担构建2kbit代码片段（包括修改过的版本）所产生的一切风险，但是用户有权在2kbit的GitHub项目页提出issue，并有权在代码片段修复这些问题后获取这些更新，但是，作者不会对修改过的代码版本做质量保证，也没有义务修正在修改过的代码片段中存在的任何缺陷。

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Net_2kBit.Modules
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
                    try
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
                    catch
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(receiver.GroupId, "无法从HanBot同步黑名单！");
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
                                try
                                {
                                    RestClient client1 = new($"{Global.api}/blacklist");
                                    RestRequest request1 = new("up?uid=" + Global.g_blocklist[i] + "&key=" + Global.api_key, Method.Post);
                                    request.Timeout = 10000;
                                    await client1.ExecuteAsync(request1);
                                }
                                catch
                                {
                                    try
                                    {
                                        await MessageManager.SendGroupMessageAsync(receiver.GroupId, "无法将黑名单反向同步给HanBot！");
                                    }
                                    catch
                                    {
                                        Console.WriteLine("群消息发送失败");
                                    }
                                    break;
                                }
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
                    try
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
                    catch
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(receiver.GroupId, "无法合并黑名单！");
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