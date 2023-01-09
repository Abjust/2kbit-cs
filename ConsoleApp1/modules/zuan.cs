// 2kbit C# Edition，2kbit的C#分支版本
// Copyright(C) 2022 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

// 致所有构建及修改2kbit代码片段的用户：作者（Abjust）并不承担构建2kbit代码片段（包括修改过的版本）所产生的一切风险，但是用户有权在2kbit的GitHub项目页提出issue，并有权在代码片段修复这些问题后获取这些更新，但是，作者不会对修改过的代码版本做质量保证，也没有义务修正在修改过的代码片段中存在的任何缺陷。

using Mirai.Net.Data.Events;
using Mirai.Net.Data.Events.Concretes.Message;
using Mirai.Net.Data.Messages;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using System.Runtime.InteropServices;

namespace Net_2kBit.Modules
{
    public class Zuan
    {
        public const int zuan_cd = 120;
        public const int zuan_threshold = 3;
        public static int zuan_count = 0;
        public static long last_zuan;
        public static long last_zuanctrl;
        public static async void Execute(string sender, string group, [Optional] MessageChain message, [Optional] EventBase @event)
        {
            string[] words =
                            {
                                 "cnmd",
                                 "你更是歌姬吧嗷",
                                 "你个狗比玩意",
                                 "你是不是被抛上去3次，却只被接住2次？",
                                 "你真是小母牛坐灯泡，牛逼一闪又一闪",
                                 "小嘴像抹了开塞露一样",
                                 "小东西长得真随机",
                                 "我只想骂人，但不想骂你",
                                 "但凡你有点用，也不至于一点用处都没有",
                                 "你还真把自己当个人看了，你也配啊",
                                 "那么丑的脸，就可以看出你是金针菇",
                                 "阁下长得真是天生励志",
                                 "装逼对你来说就像一日三餐的事",
                                 "我怎么敢碰你呢，我怕我买洗手液买穷自己",
                                 "狗咬了你，你还能咬回狗吗",
                                 "你是独一无二的，至少全人类都不希望再有第二个",
                                 "你的智商和喜马拉雅山的氧气一样，稀薄",
                                 "别人的脸是七分天注定，三分靠打扮，你的脸是一分天注定，九分靠滤镜",
                                 "偶尔也要活得强硬一点，软得像滩烂泥一样有什么意思",
                                 "任何人工智能都敌不过阁下这款天然呆",
                                 "我骂你是为了你好，你应该从中学到些什么，比如说自知之明",
                                 "你要好好做自己，反正别的你也做不好",
                                 "如果国家把长相分等级的话，你的长相，都可以吃低保了",
                                 "你没权利看不惯我的生活方式，但你有权抠瞎自己的双眼",
                                 "如果你觉得我哪里不对，请一定要告诉我，反正我也不会改，你别憋出病来",
                                 "你（  ）什么时候（  ）啊",
                                 "四吗玩意，说我是歌姬吧，你怎么不撒泡尿照照镜子看看你自己，狗比玩意",
                                 "握草泥马呀—\r\n我操尼玛啊啊啊啊—\r\n我—操—你—妈—\r\n听到没，我—操—你—妈—"
                            };
            MessageChain messageChain = new MessageChainBuilder()
                .At(Global.bot_qq)
                .Plain(" 你就是歌姬吧")
                .Build();
            Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            if ((message != null && message[1] == messageChain[0] && message[2] == messageChain[1]) || (@event != null && @event is NudgeEvent))
            {
                if (Global.time_now - last_zuanctrl >= zuan_cd)
                {
                    if (Global.time_now - last_zuan <= zuan_cd)
                    {
                        if (zuan_count <= zuan_threshold)
                        {
                            Random r = new();
                            int index = r.Next(words.Length);
                            MessageChain? messageChain1 = new MessageChainBuilder()
                            .At(sender)
                            .Plain(" ")
                            .Plain(words[index])
                            .Build();
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(group, messageChain1);
                                last_zuan = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                                zuan_count += 1;
                            }
                            catch
                            {
                                Console.WriteLine("祖安失败（恼）");
                            }
                        }
                        else
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(group, $"祖安太多了，收手罢（恼）（祖安功能将被禁用 {zuan_cd} 秒）");
                                last_zuanctrl = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                                zuan_count = 0;
                            }
                            catch
                            {
                                Console.WriteLine("祖安失败（恼）");
                            }
                        }
                    }
                    else
                    {
                        Random r = new();
                        int index = r.Next(words.Length);
                        MessageChain? messageChain1 = new MessageChainBuilder()
                        .At(sender)
                        .Plain(" ")
                        .Plain(words[index])
                        .Build();
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(group, messageChain1);
                            last_zuan = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                            zuan_count = 1;
                        }
                        catch
                        {
                            Console.WriteLine("祖安失败（恼）");
                        }
                    }
                }
            }
        }
    }
}