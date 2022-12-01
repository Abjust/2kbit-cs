// 2kbot b2.3.1
// Copyright(C) 2022 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

// 致所有构建及修改2kbot代码片段的用户：作者（Abjust）并不承担构建2kbot代码片段（包括修改过的版本）所产生的一切风险，但是用户有权在2kbot的GitHub项目页提出issue，并有权在代码片段修复这些问题后获取这些更新，但是，作者不会对修改过的代码版本做质量保证，也没有义务修正在修改过的代码片段中存在的任何缺陷。

using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;

namespace Net_2kBot.Modules
{
    public class Call
    {
        // 叫人功能
        public static async void Execute(string victim, string group, int times)
        {
            if (times >= 10)
            {
                times = 10;
            }
            var messageChain = new MessageChainBuilder()
                               .At(victim)
                               .Plain(" 机器人正在呼叫你")
                               .Build();
            Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            if (Global.time_now - Global.last_call >= Global.call_cd)
            {
                for (int i = 0; i < times; i++)
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(group, messageChain);
                        Global.last_call = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            else
            {
                Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                try
                {
                    await MessageManager.SendGroupMessageAsync(group, $"CD未到，请别急！CD还剩： {Global.call_cd - (Global.time_now - Global.last_call)} 秒");
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            }
        }
    }
}