// 2kbot b2.3.1
// Copyright(C) 2022 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

// 致所有构建及修改2kbot代码片段的用户：作者（Abjust）并不承担构建2kbot代码片段（包括修改过的版本）所产生的一切风险，但是用户有权在2kbot的GitHub项目页提出issue，并有权在代码片段修复这些问题后获取这些更新，但是，作者不会对修改过的代码版本做质量保证，也没有义务修正在修改过的代码片段中存在的任何缺陷。

using Mirai.Net.Sessions.Http.Managers;
using MySql.Data.MySqlClient;
using RestSharp;

namespace Net_2kBot.Modules
{
    public class Admin
    {
        // 禁言功能
        public static async void Mute(string executor, string victim, string group, int minutes)
        {
            if (Global.ops != null && Global.ops.Contains($"{group}_{executor}") || Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                if (Global.ops != null && Global.ops.Contains($"{group}_{victim}") == false || Global.g_ops != null && Global.g_ops.Contains(victim) == false)
                {
                    try
                    {
                        await GroupManager.MuteAsync(victim, group, minutes * 60);
                        await MessageManager.SendGroupMessageAsync(group, $"已尝试将 {victim} 禁言 {minutes} 分钟");
                    }
                    catch
                    {
                        try
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(group, "执行失败！正在调用api...");
                            }
                            catch
                            {
                                Console.WriteLine("执行失败！正在调用api...");
                            }
                            RestClient client = new($"{Global.api}/guser");
                            RestRequest request = new($"nobb?uid={victim}&gid={group}&tim={minutes * 60}&key={Global.api_key}", Method.Post);
                            request.Timeout = 10000;
                            RestResponse response = await client.ExecuteAsync(request);
                            Console.WriteLine(response.Content);
                        }
                        catch
                        {
                            Console.WriteLine("你甚至连api都调用不了");
                        }
                    }
                }
                else
                {
                    await MessageManager.SendGroupMessageAsync(group, "此人是机器人管理员，无法禁言");
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
            }
        }
        // 解禁功能
        public static async void Unmute(string executor, string victim, string group)
        {
            if (Global.ops != null && Global.ops.Contains($"{group}_{executor}") || Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                try
                {
                    await GroupManager.UnMuteAsync(victim, group);
                    await MessageManager.SendGroupMessageAsync(group, $"已尝试将 {victim} 解除禁言");
                }
                catch
                {
                    try
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, "执行失败！正在调用api...");
                        }
                        catch
                        {
                            Console.WriteLine("执行失败！正在调用api...");
                        }
                        RestClient client = new($"{Global.api}/guser");
                        RestRequest request = new($"nobb?uid={victim}&gid={group}&tim=0&key={Global.api_key}", Method.Post);
                        request.Timeout = 10000;
                        RestResponse response = await client.ExecuteAsync(request);
                        Console.WriteLine(response.Content);
                    }
                    catch
                    {
                        Console.WriteLine("你甚至连api都调用不了");
                    }
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
            }
        }
        // 踢人功能
        public static async void Kick(string executor, string victim, string group)
        {
            if (Global.ops != null && Global.ops.Contains($"{group}_{executor}") || Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                if (Global.ops != null && Global.ops.Contains($"{group}_{victim}") == false || Global.g_ops != null && Global.g_ops.Contains(victim) == false)
                {
                    try
                    {
                        await GroupManager.KickAsync(victim, group);
                        await MessageManager.SendGroupMessageAsync(group, $"已尝试将 {victim} 踢出");
                    }
                    catch
                    {
                        try
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(group, "执行失败！正在调用api...");
                            }
                            catch
                            {
                                Console.WriteLine("执行失败！正在调用api...");
                            }
                            RestClient client = new($"{Global.api}/guser");
                            RestRequest request = new($"del?key={Global.api_key}&uid={victim}&gid={group}", Method.Post);
                            request.Timeout = 10000;
                            RestResponse response = await client.ExecuteAsync(request);
                            Console.WriteLine(response.Content);
                        }
                        catch
                        {
                            Console.WriteLine("你甚至连api都调用不了");
                        }
                    }
                }
                else
                {
                    await MessageManager.SendGroupMessageAsync(group, "此人是机器人管理员，无法踢出");
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
            }
        }
        // 加黑功能
        public static async void Block(string executor, string victim, string group)
        {
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            await msc.OpenAsync();
            if (Global.ops != null && Global.ops.Contains($"{group}_{executor}") || Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                if (Global.ops != null && Global.ops.Contains($"{group}_{victim}") == false || Global.g_ops != null && Global.g_ops.Contains(victim) == false)
                {
                    if (Global.blocklist?.Contains(victim) == false)
                    {
                        cmd.CommandText = $"INSERT INTO blocklist (qid,gid) VALUES ({victim},{group});";
                        await cmd.ExecuteNonQueryAsync();
                        await msc.CloseAsync();
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, $"已将 {victim} 加入本群黑名单");
                        }
                        catch
                        {
                            Console.WriteLine($"已将 {victim} 加入 {group} 黑名单");
                        }
                        try
                        {
                            await GroupManager.KickAsync(victim, group);
                        }
                        catch
                        {
                            try
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(group, "在尝试将黑名单对象踢出时执行失败！正在调用api...");
                                }
                                catch
                                {
                                    Console.WriteLine("在尝试将黑名单对象踢出时执行失败！正在调用api...");
                                }
                                RestClient client = new($"{Global.api}/guser");
                                RestRequest request = new($"del?key={Global.api_key}&uid={victim}&gid={group}", Method.Post);
                                request.Timeout = 10000;
                                RestResponse response = await client.ExecuteAsync(request);
                                Console.WriteLine(response.Content);
                            }
                            catch
                            {
                                Console.WriteLine("你甚至连api都调用不了");
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, $"{victim} 已经在本群黑名单内");
                        }
                        catch
                        {
                            Console.WriteLine($"{victim} 已经在 {group} 黑名单内");
                        }
                    }
                    await msc.CloseAsync();
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"{victim} 是机器人管理员，不能加黑");
                    }
                    catch
                    {
                        Console.WriteLine($"{victim} 是机器人管理员，不能加黑");
                    }
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
            }
        }
        // 解黑功能
        public static async void Unblock(string executor, string victim, string group)
        {
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            await msc.OpenAsync();
            if (Global.ops != null && Global.ops.Contains($"{group}_{executor}") || Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                if (Global.blocklist != null && Global.blocklist.Contains($"{group}_{victim}"))
                {
                    cmd.CommandText = $"DELETE FROM blocklist WHERE qid = {victim} AND gid = {group});";
                    await cmd.ExecuteNonQueryAsync();
                    await msc.CloseAsync();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"已将 {victim} 移出本群黑名单");
                    }
                    catch
                    {
                        Console.WriteLine($"已将 {victim} 移出 {group} 黑名单");
                    }

                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"{victim} 不在本群黑名单内");
                    }
                    catch
                    {
                        Console.WriteLine($"{victim} 不在 {group} 黑名单内");
                    }
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
            }
        }
        // 全局加黑功能
        public static async void G_Block(string executor, string victim, string group)
        {
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            await msc.OpenAsync();
            if (Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                if (Global.g_ops != null && Global.g_ops.Contains(victim) == false)
                {
                    if (Global.g_blocklist == null || Global.g_blocklist != null && Global.g_blocklist.Contains(victim) == false)
                    {
                        cmd.CommandText = $"INSERT INTO g_blocklist (qid) VALUES ({victim});";
                        await cmd.ExecuteNonQueryAsync();
                        await msc.CloseAsync();
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, $"已将 {victim} 加入全局黑名单");
                        }
                        catch
                        {
                            Console.WriteLine($"已将 {victim} 加入全局黑名单");
                        }
                        try
                        {
                            await GroupManager.KickAsync(victim, group);
                        }
                        catch
                        {
                            try
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(group, "在尝试将黑名单对象踢出时执行失败！正在调用api...");
                                }
                                catch
                                {
                                    Console.WriteLine("在尝试将黑名单对象踢出时执行失败！正在调用api...");
                                }
                                RestClient client = new($"{Global.api}/guser");
                                RestRequest request = new($"del?key={Global.api_key}&uid={victim}&gid={group}", Method.Post);
                                request.Timeout = 10000;
                                RestResponse response = await client.ExecuteAsync(request);
                                Console.WriteLine(response.Content);
                            }
                            catch
                            {
                                Console.WriteLine("你甚至连api都调用不了");
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, $"{victim} 已经在全局黑名单内");
                        }
                        catch
                        {
                            Console.WriteLine($"{victim} 已经在全局黑名单内");
                        }
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"{victim} 是全局机器人管理员，不能全局加黑");
                    }
                    catch
                    {
                        Console.WriteLine($"{victim} 是全局机器人管理员，不能全局加黑");
                    }
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是全局机器人管理员");
            }
        }
        // 全局解黑功能
        public static async void G_Unblock(string executor, string victim, string group)
        {
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            await msc.OpenAsync();
            if (Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                if (Global.g_blocklist != null && Global.g_blocklist.Contains(victim))
                {
                    cmd.CommandText = $"DELETE FROM g_blocklist WHERE qid = {victim});";
                    await cmd.ExecuteNonQueryAsync();
                    await msc.CloseAsync();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"已将 {victim} 移出全局黑名单");
                    }
                    catch
                    {
                        Console.WriteLine($"已将 {victim} 移出全局黑名单");
                    }

                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"{victim} 不在全局黑名单内");
                    }
                    catch
                    {
                        Console.WriteLine($"{victim} 不在全局黑名单内");
                    }
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是全局机器人管理员");
            }
        }
        // 给OP功能
        public static async void Op(string executor, string victim, string group)
        {
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            await msc.OpenAsync();
            if (Global.ops != null && Global.ops.Contains($"{group}_{executor}") || Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                if (Global.ops == null || Global.ops != null && Global.ops.Contains($"{group}_{victim}") == false)
                {
                    cmd.CommandText = $"INSERT INTO ops (qid,gid) VALUES ({victim},{group});";
                    await cmd.ExecuteNonQueryAsync();
                    await msc.CloseAsync();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"已将 {victim} 设置为本群机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine($"已将 {victim} 设置为 {group} 机器人管理员");
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"{victim} 已经是本群机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine($"{victim} 已经是 {group} 机器人管理员");
                    }
                }
            }
            else
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            }
        }
        // 取消OP功能
        public static async void Deop(string executor, string victim, string group)
        {
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            await msc.OpenAsync();
            if (Global.ops != null && Global.ops.Contains($"{group}_{executor}") || Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                if (Global.ops != null && Global.ops.Contains($"{group}_{victim}"))
                {
                    cmd.CommandText = $"DELETE FROM ops WHERE qid = {victim} AND gid = {group});";
                    await cmd.ExecuteNonQueryAsync();
                    await msc.CloseAsync();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"已取消 {victim} 在本群的机器人管理员权限");
                    }
                    catch
                    {
                        Console.WriteLine($"已取消 {victim} 在 {group} 的机器人管理员权限");
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"{victim} 不是本群机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine($"{victim} 不是 {group} 机器人管理员");
                    }
                }
            }
            else
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            }
        }
        // 给全局OP功能
        public static async void G_Op(string executor, string victim, string group)
        {
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            await msc.OpenAsync();
            if (Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                if (Global.g_ops != null && Global.g_ops.Contains(victim) == false)
                {
                    cmd.CommandText = $"INSERT INTO g_ops (qid) VALUES ({victim});";
                    await cmd.ExecuteNonQueryAsync();
                    await msc.CloseAsync();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"已将 {victim} 设置为全局机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine($"已将 {victim} 设置为全局机器人管理员");
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"{victim} 已经是全局机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine($"{victim} 已经是全局机器人管理员");
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
        // 取消OP功能
        public static async void G_Deop(string executor, string victim, string group)
        {
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            await msc.OpenAsync();
            if (Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                if (Global.g_ops != null && Global.g_ops.Contains(victim))
                {
                    cmd.CommandText = $"DELETE FROM g_ops WHERE qid = {victim});";
                    await cmd.ExecuteNonQueryAsync();
                    await msc.CloseAsync();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"已取消 {victim} 的全局机器人管理员权限");
                    }
                    catch
                    {
                        Console.WriteLine($"已取消 {victim} 的全局机器人管理员权限");
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"{victim} 不是全局机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine($"{victim} 不是全局机器人管理员");
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
        // 屏蔽消息功能
        public static async void Ignore(string executor, string victim, string group)
        {
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            await msc.OpenAsync();
            if (Global.ops != null && Global.ops.Contains($"{group}_{executor}") || Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                if (Global.ignores?.Contains($"{group}_{victim}") == false)
                {
                    cmd.CommandText = $"INSERT INTO ignores (qid,gid) VALUES ({victim},{group});";
                    await cmd.ExecuteNonQueryAsync();
                    await msc.CloseAsync();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"已在本群屏蔽 {victim} 的消息");
                    }
                    catch
                    {
                        Console.WriteLine($"已在 {group} 屏蔽 {victim} 的消息");
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"{victim} 的消息已经在本群被机器人屏蔽");
                    }
                    catch
                    {
                        Console.WriteLine($"{victim} 的消息已经在 {group} 被机器人屏蔽");
                    }
                }
            }
            else
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            }
        }
        // 全局屏蔽消息功能
        public static async void G_Ignore(string executor, string victim, string group)
        {
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            await msc.OpenAsync();
            if (Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                if (Global.g_ignores?.Contains($"{group}_{victim}") == false)
                {
                    cmd.CommandText = $"INSERT INTO g_ignores (qid) VALUES ({victim});";
                    await cmd.ExecuteNonQueryAsync();
                    await msc.CloseAsync();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"已屏蔽 {victim} 在所有群的消息");
                    }
                    catch
                    {
                        Console.WriteLine($"已屏蔽 {victim} 在所有群的消息");
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, $"{victim} 的消息已经在所有群被机器人屏蔽了");
                    }
                    catch
                    {
                        Console.WriteLine($"{victim} 的消息已经所有群被机器人屏蔽了");
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
        // 带 清 洗
        public static async void Purge(string executor, string group)
        {
            if (Global.ops != null && Global.ops.Contains($"{group}_{executor}") || Global.g_ops != null && Global.g_ops.Contains(executor))
            {
                if (Global.g_blocklist != null)
                {
                    foreach (string item in Global.g_blocklist)
                    {
                        try
                        {
                            await GroupManager.KickAsync(item, group);
                        }
                        catch { }
                    }
                }
                if (Global.blocklist != null)
                {
                    foreach (string item in Global.blocklist)
                    {
                        string[] blocklist = item.Split("_");
                        if (blocklist[0] == group)
                        {
                            try
                            {
                                await GroupManager.KickAsync(blocklist[1], group);
                            }
                            catch { }
                        }
                    }
                }
                await MessageManager.SendGroupMessageAsync(group, "带清洗发动成功！");
            }
            else
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            }
        }
    }
}