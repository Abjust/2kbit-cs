using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Utils.Scaffolds;

namespace Net_2kBot.Modules
{
    public static class Repeat
    {
        public static async void Execute(MessageReceiverBase @base)
        {
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
                    "呃"
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
                            if (Global.ignores != null && Global.ignores.Contains(receiver.Sender.Id) == false)
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
                else if (Global.ignores != null && Global.ignores.Contains(receiver.Sender.Id) == false)
                {
                    foreach (string item in repeatwords)
                    {
                        if (item.Equals(receiver.MessageChain.GetPlainMessage()))
                        {
                            Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                            if (Global.time_now - Global.last_repeatctrl >= Global.repeat_cd)
                            {
                                try
                                {
                                    if (Global.time_now - Global.last_repeat <= Global.repeat_interval)
                                    {
                                        if (Global.repeat_count <= Global.repeat_threshold)
                                        {
                                            Global.last_repeat = Global.time_now;
                                            await receiver.SendMessageAsync(receiver.MessageChain.GetPlainMessage());
                                            Global.repeat_count++;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                await receiver.SendMessageAsync("警告：2kbot已执行动态管理机制！（主动复读功能将被暂时禁用 " + Global.repeat_cd + " 秒）");
                                            }
                                            catch
                                            {
                                                Console.WriteLine("群消息发送失败");
                                            }
                                            Global.last_repeatctrl = Global.time_now;
                                            Global.repeat_count = 0;
                                        }
                                    }
                                    else
                                    {
                                        Global.repeat_count = 0;
                                        Global.last_repeat = Global.time_now;
                                        await receiver.SendMessageAsync(receiver.MessageChain.GetPlainMessage());
                                        Global.repeat_count++;
                                    }
                                }
                                catch
                                {
                                    break;
                                }
                            }
                            
                        }
                    }
                }
            }
        }
    }
}