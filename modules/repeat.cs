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
                            try
                            {
                                await receiver.SendMessageAsync(receiver.MessageChain.GetPlainMessage());
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