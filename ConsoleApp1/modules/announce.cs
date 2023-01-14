// 2kbit C# Edition，2kbit的C#分支版本
// Copyright(C) 2022 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

// 致所有构建及修改2kbit代码片段的用户：作者（Abjust）并不承担构建2kbit代码片段（包括修改过的版本）所产生的一切风险，但是用户有权在2kbit的GitHub项目页提出issue，并有权在代码片段修复这些问题后获取这些更新，但是，作者不会对修改过的代码版本做质量保证，也没有义务修正在修改过的代码片段中存在的任何缺陷。

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Data.Shared;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;

namespace Net_2kBit.Modules
{
    public static class Announce
    {
        public static async void Execute(MessageReceiverBase @base, string executor, IEnumerable<Mirai.Net.Data.Shared.Group> groups)
        {
            if (@base is GroupMessageReceiver receiver)
            {
                string[] result = receiver.MessageChain.GetPlainMessage().Split(" ");
                if (executor == Global.owner_qq)
                {
                    if (result.Length > 1)
                    {
                        try
                        {
                            string results = "";
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
                            foreach (Mirai.Net.Data.Shared.Group group in groups)
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(group, results);
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
                                await receiver.SendMessageAsync("参数错误");
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
                        await receiver.SendMessageAsync("你不是机器人主人");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            }
        }
        public static async Task Notification()
        {
            while (true)
            {
                List<int[]> schedules = new()
                {
                    new int[] {14, 0, 0},
                    new int[] {3, 45, 0}
                };
                List<string> words = new()
                {
                    "同志们，该准备休息了，身体是革命的本钱！",
                    "1145141919810！"
                };
                foreach (int[] schedule in schedules)
                {
                    TimeSpan timeSpan = new TimeSpan(schedule[0], schedule[1], schedule[2]);
                    DateTime utc = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, DateTimeKind.Utc);
                    DateTime time_now = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    if (TimeZoneInfo.ConvertTime(utc, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")) == time_now)
                    {
                        IEnumerable<Group> groups = AccountManager.GetGroupsAsync().GetAwaiter().GetResult();
                        foreach (Group group in groups)
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(group, words[schedules.IndexOf(schedule)]);
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                Thread.Sleep(500);
            }
        }
    }
}