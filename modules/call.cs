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