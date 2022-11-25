using Manganese.Text;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Utils.Scaffolds;

namespace Net_2kBot.Modules
{
    public static class Help
    {
        public static async void Execute(MessageReceiverBase @base)
        {
            if (@base is GroupMessageReceiver receiver)
            {
                //菜单
                if (receiver.MessageChain.GetPlainMessage() == "菜单" || receiver.MessageChain.GetPlainMessage() == "/menu")
                {
                    try
                    {
                        await receiver.SendMessageAsync(@"2kbot菜单
                        1.群管系统
                        2.复读机
                        3.精神心理疾病科普
                        4.量表测试
                        5.叫人功能
                        详情请用/help指令");
                    }
                    catch
                    {
                        Console.WriteLine("菜单消息发送失败");
                    }
                }
                //帮助
                var indexs = new List<string>
                {
                    "1",
                    "2",
                    "3",
                    "4",
                    "5",
                };
                var contents = new List<string>
                {
                    @"群管功能
禁言：/mute <QQ号或at> [时间] （以分钟算）
解禁：/unmute <QQ号或at>
踢出：/kick <QQ号或at>
加黑：/block <QQ号或at>
解黑：/unblock <QQ号或at>
屏蔽消息（加灰）：/ignore <QQ号或at>
给予管理员：/op <QQ号或at>
剥夺管理员：/deop <QQ号或at>
清理群内所有黑名单人员：/purge
从Hanbot同步黑名单：/sync
将黑名单反向同步给Hanbot：/rsync
合并黑名单：/merge
（上述功能都需要机器人管理员）
注：在block、unblock、ignore、op、deop前加上g表示应用全局（例子：/gblock）",
                    "该指令用于复述文本\r\n用法：/echo <文本>",
                    "该指令用于叫人\r\n用法：/call <QQ号或at> [次数]",
                    "发送“精神疾病”或者“心理疾病”并按照后续出现的选项发送相应文字即可获得科普文本",
                    "发送“量表”或者“测试”并按照后续出现的选项发送相应文字即可获得链接"
                };
                if (receiver.MessageChain.GetPlainMessage().StartsWith("/help") == true)
                {
                    string[] result = receiver.MessageChain.GetPlainMessage().Split(" ");
                    if (result.Length > 1)
                    {
                        foreach (string q in indexs)
                        {
                            try
                            {
                                if (result[1] == q)
                                {
                                    try
                                    {
                                        await receiver.SendMessageAsync((contents[indexs.IndexOf(q)]));
                                    }
                                    catch
                                    {
                                        Console.WriteLine("帮助消息发送失败");
                                    }
                                }
                                else if (result[1].ToInt32() > indexs.Count)
                                {
                                    try
                                    {
                                        await receiver.SendMessageAsync("未找到相关帮助");
                                    }
                                    catch
                                    {
                                        Console.WriteLine("帮助消息发送失败");
                                    }
                                    break;
                                }
                            }
                            catch
                            {
                                try
                                {
                                    await receiver.SendMessageAsync("请写数字，不要写别的好吗？");
                                }
                                catch
                                {
                                    Console.WriteLine("帮助消息发送失败");
                                }
                                break;
                            }
                        }
                    }
                    else if (receiver.MessageChain.GetPlainMessage() == "/help")
                    {
                        try
                        {
                            await receiver.SendMessageAsync(@"目前有对于以下功能的帮助文档：
                            [1]群管功能
                            [2]/echo
                            [3]/call
                            [4]精神心理疾病科普
                            [5]量表测试");
                        }
                        catch
                        {
                            Console.WriteLine("帮助消息发送失败");
                        }
                    }
                }
            }
        }
    }
}