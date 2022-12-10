using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;

namespace Net_2kBot.Modules
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
    }
}