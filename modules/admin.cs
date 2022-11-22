using Mirai.Net.Sessions.Http.Managers;
using RestSharp;
using System.Diagnostics;

namespace Net_2kBot.Modules
{
    public class Admin
    {
        // 禁言功能
        public async void Mute(string executor, string victim, string group, int minutes)
        {
            if (Global.ops != null && Global.ops.Contains(executor))
            {
                if (Global.ops.Contains(victim) == false)
                {
                    try
                    {
                        await GroupManager.MuteAsync(victim, group, minutes * 60);
                        await MessageManager.SendGroupMessageAsync(group, "已尝试将 " + victim + " 禁言 " + minutes + " 分钟");
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
                            RestClient client = new("http://101.42.94.97/guser");
                            RestRequest request = new("nobb?uid=" + victim + "&gid=" + group + "&tim=" + minutes * 60 + "&key=" + Global.api_key, Method.Post);
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
        public async void Unmute(string executor, string victim, string group)
        {
            if (Global.ops != null && Global.ops.Contains(executor))
            {
                try
                {
                    await GroupManager.UnMuteAsync(victim, group);
                    await MessageManager.SendGroupMessageAsync(group, "已尝试将 " + victim + " 解除禁言");
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
                        RestClient client = new("http://101.42.94.97/guser");
                        RestRequest request = new("nobb?uid=" + victim + "&gid=" + group + "&tim=0&key=" + Global.api_key, Method.Post);
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
        public async void Kick(string executor, string victim, string group)
        {
            if (Global.ops != null && Global.ops.Contains(executor))
            {
                if (Global.ops.Contains(victim) == false)
                {
                    try
                    {
                        await GroupManager.KickAsync(victim, group);
                        await MessageManager.SendGroupMessageAsync(group, "已尝试将 " + victim + " 踢出");
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
                            RestClient client = new("http://101.42.94.97/guser");
                            RestRequest request = new("del?key=" + Global.api_key + "&uid=" + victim + "&gid=" + group, Method.Post);
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
        public async void Block(string executor, string victim, string group)
        {
            if (Global.ops != null && Global.ops.Contains(executor))
            {
                if (Global.ops.Contains(victim) == false)
                {
                    if (Global.blocklist != null && Global.blocklist.Contains(victim) == false)
                    {
                        using StreamWriter file = new("blocklist.txt", append: true);
                        await file.WriteLineAsync("\r\n" + victim);
                        file.Close();
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, "已将 " + victim + " 加入黑名单");
                        }
                        catch
                        {
                            Console.WriteLine("已将 " + victim + " 加入黑名单");
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
                                RestClient client = new("http://101.42.94.97/guser");
                                RestRequest request = new("del?key=" + Global.api_key + "&uid=" + victim + "&gid=" + group, Method.Post);
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
                            await MessageManager.SendGroupMessageAsync(group, victim + " 已经在黑名单内");
                        }
                        catch
                        {
                            Console.WriteLine(victim + " 已经在黑名单内");
                        }
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, victim + " 是机器人管理员，不能加黑");
                    }
                    catch
                    {
                        Console.WriteLine(victim + " 是机器人管理员，不能加黑");
                    }
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
            }
        }
        //给OP功能
        public async void Op(string executor, string victim, string group)
        {
            if (Global.ops != null && Global.ops.Contains(executor))
            {
                if (Global.ops.Contains(victim) == false)
                {
                    using StreamWriter file = new("ops.txt", append: true);
                    await file.WriteLineAsync("\r\n" + victim);
                    file.Close();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "已将 " + victim + " 设置为机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine("已将 " + victim + " 设置为机器人管理员");
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, victim + " 已经是机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine(victim + " 已经是机器人管理员");
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
        // 解黑功能
        public async void Unblock(string executor, string victim, string group)
        {
            if (Global.ops != null && Global.ops.Contains(executor))
            {
                if (Global.blocklist != null && Global.blocklist.Contains(victim))
                {
                    var blocklist_old = Global.blocklist;
                    var blocklist_new = Global.blocklist.Where(line => !line.Contains(victim));
                    File.WriteAllLines("blocklist.txt", blocklist_new);
                    await MessageManager.SendGroupMessageAsync(group, "已将 " + victim + " 移出黑名单");
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, victim + " 不在黑名单内");
                    }
                    catch
                    {
                        Console.WriteLine(victim + " 不在黑名单内");
                    }
                }
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(group, "你不是机器人管理员");
            }
        }
        //取消OP功能
        public async void Deop(string executor, string victim, string group)
        {
            if (Global.ops != null && Global.ops.Contains(executor))
            {
                if (Global.ops.Contains(victim) == true)
                {
                    var ops_old = Global.ops;
                    var ops_new = Global.ops.Where(line => !line.Contains(victim));
                    File.WriteAllLines("ops.txt", ops_new);
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "已取消 " + victim + " 的机器人管理员权限");
                    }
                    catch
                    {
                        Console.WriteLine("已取消 " + victim + " 的机器人管理员权限");
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, victim + " 不是机器人管理员");
                    }
                    catch
                    {
                        Console.WriteLine(victim + " 不是机器人管理员");
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
        //屏蔽消息功能
        public async void Ignore(string executor, string victim, string group)
        {
            if (Global.ops != null && Global.ops.Contains(executor))
            {
                if (Global.ops.Contains(victim) == false)
                {
                    using StreamWriter file = new("ignores.txt", append: true);
                    await file.WriteLineAsync("\r\n" + victim);
                    file.Close();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, "已将 " + victim + " 的消息屏蔽");
                    }
                    catch
                    {
                        Console.WriteLine("已将 " + victim + " 的消息屏蔽");
                    }
                }
                else
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, victim + " 的消息已经被机器人屏蔽");
                    }
                    catch
                    {
                        Console.WriteLine(victim + " 的消息已经被机器人屏蔽");
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
    }
}