using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Utils.Scaffolds;

namespace Net_2kBot.Modules
{
    public static class Zuan
    {
        public static async void Execute(MessageReceiverBase @base)
        {
            if (@base is GroupMessageReceiver receiver)
            {
                if (receiver.MessageChain[1].ToString() == $"AtMessage {{ Type = At, Target = {Global.qq} }}" && receiver.MessageChain[2].ToString() == "PlainMessage { Type = Plain, Text =  你就是歌姬吧 }")
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
                         "你（  ）什么时候（  ）啊"
                        };
                    Random r = new();
                    int index = r.Next(words.Length);
                    MessageChain? messageChain1 = new MessageChainBuilder()
                    .At(receiver.Sender.Id)
                    .Plain(" ")
                    .Plain(words[index])
                    .Build();
                    try
                    {
                        await receiver.SendMessageAsync(messageChain1);
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