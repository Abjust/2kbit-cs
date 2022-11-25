using Manganese.Text;
using Mirai.Net.Data.Events.Concretes.Group;
using Mirai.Net.Data.Events.Concretes.Message;
using Mirai.Net.Data.Events.Concretes.Request;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Data.Shared;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using MySql.Data.MySqlClient;
using Net_2kBot.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reactive.Linq;

namespace Net_2kBot
{
    public static class BotMain
    {
        public static async Task Main()
        {
            MiraiBot bot = new()
            {
                Address = "localhost:8080",
                QQ = Global.qq,
                VerifyKey = Global.verify_key
            };
            // 注意: `LaunchAsync`是一个异步方法，请确保`Main`方法的返回值为`Task`
            await bot.LaunchAsync();

            // 初始化
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd = new()
            {
                Connection = msc
            };
            msc.Open();
            // 若数据表不存在则创建
            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS `{Global.database_name}`.`blocklist` (`id` INT NOT NULL AUTO_INCREMENT,`qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',`gid` VARCHAR(10) NOT NULL COMMENT 'Q群号',PRIMARY KEY (`id`));";
            cmd.ExecuteNonQuery();
            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS `{Global.database_name}`.`ops` (`id` INT NOT NULL AUTO_INCREMENT,`qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',`gid` VARCHAR(10) NOT NULL COMMENT 'Q群号',PRIMARY KEY (`id`));";
            cmd.ExecuteNonQuery();
            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS `{Global.database_name}`.`ignores` (`id` INT NOT NULL AUTO_INCREMENT,`qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',`gid` VARCHAR(10) NOT NULL COMMENT 'Q群号',PRIMARY KEY (`id`));";
            cmd.ExecuteNonQuery();
            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS `{Global.database_name}`.`g_blocklist` (`id` INT NOT NULL AUTO_INCREMENT,`qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',PRIMARY KEY (`id`));";
            cmd.ExecuteNonQuery();
            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS `{Global.database_name}`.`g_ops` (`id` INT NOT NULL AUTO_INCREMENT,`qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',PRIMARY KEY (`id`));";
            cmd.ExecuteNonQuery();
            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS `{Global.database_name}`.`g_ignores` (`id` INT NOT NULL AUTO_INCREMENT,`qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',PRIMARY KEY (`id`));";
            cmd.ExecuteNonQuery();
            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS `{Global.database_name}`.`repeatctrl` (`id` INT NOT NULL AUTO_INCREMENT,`qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',`gid` VARCHAR(10) NOT NULL COMMENT 'Q群号',`last_repeat` BIGINT NULL COMMENT '上次复读时间',`last_repeatctrl` BIGINT NULL COMMENT '上次复读控制时间',`repeat_count` TINYINT UNSIGNED NOT NULL DEFAULT 0 COMMENT '复读计数',PRIMARY KEY (`id`));";
            cmd.ExecuteNonQuery();
            msc.Close();
            // 在这里添加你的代码，比如订阅消息/事件之类的
            Update.Execute();
            // 持续更新数据
            bot.MessageReceived
            .OfType<GroupMessageReceiver>()
            .Subscribe(_ =>
            {
                Update.Execute();
                Thread.Sleep(500);
            });
            // 戳一戳效果
            bot.EventReceived
            .OfType<NudgeEvent>()
            .Subscribe(async receiver =>
            {
                if (receiver.Target == Global.qq && receiver.Subject.Kind == "Group")
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(receiver.Subject.Id, "狗日的，你tm还有脸戳我？");
                    }
                    catch
                    {
                        Console.WriteLine("狗日的，你tm还有脸戳我？");
                    }
                }
                else if (receiver.Target == Global.qq && receiver.Subject.Kind == "Friend")
                {
                    await MessageManager.SendFriendMessageAsync(receiver.Subject.Id, "cnmlgbd，还跑到私信里来了？");
                }
            });
            // bot加群
            bot.EventReceived
            .OfType<NewInvitationRequestedEvent>()
            .Subscribe(async e =>
            {
                if (e.FromId == Global.owner_qq)
                {
                    // 同意邀请
                    await RequestManager.HandleNewInvitationRequestedAsync(e, NewInvitationRequestHandlers.Approve, "");
                    Console.WriteLine("机器人已同意加入 " + e.GroupId);
                }
                else
                {
                    // 拒绝邀请
                    await RequestManager.HandleNewInvitationRequestedAsync(e, NewInvitationRequestHandlers.Reject, "");
                    Console.WriteLine("机器人已拒绝加入 " + e.GroupId);
                }
            });
            // 侦测加群请求
            bot.EventReceived
            .OfType<NewMemberRequestedEvent>()
            .Subscribe(async e =>
            {
                if (Global.blocklist != null && Global.blocklist.Contains(e.FromId))
                {
                    await e.RejectAsync();
                }
            });
            // 侦测改名
            bot.EventReceived
            .OfType<MemberCardChangedEvent>()
            .Subscribe(async receiver =>
            {
                if (receiver.Current != "")
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, $"QQ号：{receiver.Member.Id}\r\n原昵称：{receiver.Origin}\r\n新昵称：{receiver.Current}");
                    }
                    catch
                    {
                        Console.WriteLine("侦测到改名");
                    }
                }

            });
            // 侦测撤回
            bot.EventReceived
           .OfType<GroupMessageRecalledEvent>()
           .Subscribe(async receiver =>
           {
               var messageChain = new MessageChainBuilder()
                .At(receiver.Operator.Id)
                .Plain(" 你又撤回了什么见不得人的东西？")
                .Build();
               if (receiver.AuthorId != receiver.Operator.Id)
               {
                   if (receiver.Operator.Permission.ToString() != "Administrator" && receiver.Operator.Permission.ToString() != "Owner")
                   {
                       try
                       {
                           await MessageManager.SendGroupMessageAsync(receiver.Group.Id, messageChain);
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
                       await MessageManager.SendGroupMessageAsync(receiver.Group.Id, messageChain);
                   }
                   catch
                   {
                       Console.WriteLine("群消息发送失败");
                   }
               }
           });
            // 侦测踢人
            bot.EventReceived
            .OfType<MemberKickedEvent>()
            .Subscribe(async receiver =>
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, $"{receiver.Member.Name} ({receiver.Member.Id}) 被踢出去辣，好似，开香槟咯！");
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            });
            // 侦测退群
            bot.EventReceived
            .OfType<MemberLeftEvent>()
            .Subscribe(async receiver =>
            {
                try
                {
                    await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, $"{receiver.Member.Name} ({receiver.Member.Id}) 退群力（悲）");
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            });
            //侦测入群
            bot.EventReceived
            .OfType<MemberJoinedEvent>()
            .Subscribe(async receiver =>
            {
                MessageChain? messageChain = new MessageChainBuilder()
               .At(receiver.Member.Id)
               .Plain(" 来辣，让我们一起撅新人！（bushi")
               .Build();
                try
                {
                    await MessageManager.SendGroupMessageAsync(receiver.Member.Group.Id, messageChain);
                }
                catch
                {
                    Console.WriteLine("群消息发送失败");
                }
            });
            // bot对接收消息的处理
            bot.MessageReceived
            .OfType<GroupMessageReceiver>()
            .Subscribe(async x =>
            {
                // 复读机
                Repeat.Execute(x);
                // surprise
                if (x.MessageChain.GetPlainMessage() == "/surprise")
                {
                    MessageChain? chain = new MessageChainBuilder()
                         .VoiceFromPath(Global.path + "/ysxb.slk")
                         .Build();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId, chain);
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
                // 随机图片
                if (x.MessageChain.GetPlainMessage() == "/photo")
                {
                    Random r = new();
                    string url;
                    int chance = 3;
                    int choice = r.Next(chance);
                    if (choice == chance - 1)
                    {
                        url = "https://www.dmoe.cc/random.php";
                    }
                    else
                    {
                        url = "https://source.unsplash.com/random";
                    }
                    MessageChain? chain = new MessageChainBuilder()
                         .ImageFromUrl(url)
                         .Build();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId, "图片在来的路上...");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId, chain);
                    }
                    catch
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "图片好像不见了！再等等吧？");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                }
                // 菜单与帮助
                Help.Execute(x);
                // 遗言
                if (x.MessageChain.GetPlainMessage() == "遗言" || x.MessageChain.GetPlainMessage() == "留言")
                {
                    await MessageManager.SendGroupMessageAsync(x.GroupId,
                        @"对我而言，我曾一直觉得Setup群是个适合我的地方，我的直觉也的确没有错，Setup群确实是个好地方，我在里面学到了不少东西，并且跟群友相谈甚欢。但是，因为群里包括群主在内的不少人和我一样，都饱受抑郁症或者精神心理疾病的困扰，以至于我在面对他们慢慢开始伤害自己的时候，或者说甚至打算终结自己的时候，却显得格外无能。我的一句“赶紧去看医生吧”，此刻显得苍白无力，我理解他们第一次求助，羞于启齿不敢告诉家里人。我不是不能理解群友们的心情，或者自身的悲惨经历。但是对我而言，我真的一时间难以接受这么多负面倾诉。我不是心理咨询师，我对心理学的掌握也有限，其实说是在，我自己也是个病人，我是个双相情感障碍患者，我也是第一次面对这种情况。每次遇到这种情况，我总是想着怎么逃避现实，仿佛精神分裂般，总是觉得事情没有发生，一切都是梦境罢了。我也希望是这样，但是发生的事情终归是发生了，我不可能凭主观意识去改变。
有时候我深感愧疚，不为什么，就为病情。不说世界上的人，就群友来说，群里比我惨的大有人在，有些没事，有些是抑郁症，像我这样得双相情感障碍的基本没有。我会自行反思，自己是不是太矫情、懦弱了，是不是抗压能力太差了呢？我怀疑过自己是假抑郁，认为自己不过是在博同情、骗流量。没错，就连我自己都不相信我自己了，那还有谁会相信这么拙劣的谎言？我感觉自己什么都是装出来的，我没有一样是真的，我只是在不懂装懂，我只是在夸大自己的苦楚和不幸，丝毫没有考虑别人的感受。我就是个精致的利己主义者，自私自利，只考虑自己的感受，特别不要脸。
我知道如果我离开，那就更加坚定我就是只顾自己的人，但是有时候我真的接受不了现实，我真的很想逃离现实，跟社会隔离开来，我不知道为什么我一直想这样，我也控制不了我自己，唉，现实就是那么残酷又无情，或许别人的痛苦是真正的不幸，我得病只是我活该，是我应有的惩罚，如果真是这么说，我也认罪认罚了。说实话，来了群之后，我的事情就特别的多，我不断地给群里的人制造麻烦，做过的错事实在是太多了，实在是不可饶恕。
对不起，Setup群的各位群友们，我觉得我应该就我给你们制造的麻烦，以及我对你们的欺骗谢罪，我可能真的值得离开，如果我离开了，希望你们不要挂念我，我就是个罪人，没什么值得纪念的地方。");
                }
                // 叫人
                if (x.MessageChain.GetPlainMessage().StartsWith("/call"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;//正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (Global.ignores?.Contains($"{x.GroupId}_{x.Sender.Id}") == false && Global.g_ignores?.Contains(x.Sender.Id) == false)
                    {
                        switch (text.Length)
                        {
                            case 3:
                                try
                                {
                                    if (text[2].ToInt32() >= 1)
                                    {
                                        Call.Execute(text[1], x.GroupId, text[2].ToInt32());
                                    }
                                    else if (text[2].ToInt32() < 1)
                                    {
                                        try
                                        {
                                            await MessageManager.SendGroupMessageAsync(x.GroupId, "nmd，这个数字是几个意思？");
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
                                        await MessageManager.SendGroupMessageAsync(x.GroupId, "油饼食不食？");
                                    }
                                    catch
                                    {
                                        Console.WriteLine("群消息发送失败");
                                    }
                                }
                                break;
                            case 2:
                                try
                                {
                                    Console.WriteLine(ja.Count);
                                    if (ja.Count == 4)
                                    {
                                        string target = ja[2]["target"]!.ToString();
                                        string t = ja[3]["text"]!.ToString().Replace(" ", "");
                                        int time = t.ToInt32();
                                        try
                                        {
                                            Console.WriteLine(time);
                                            Console.WriteLine(target);
                                            if (time >= 1)
                                            {
                                                Call.Execute(target, x.GroupId, time);
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    await MessageManager.SendGroupMessageAsync(x.GroupId, "nmd，这个数字是几个意思？");
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
                                                await MessageManager.SendGroupMessageAsync(x.GroupId, "油饼食不食？");
                                            }
                                            catch
                                            {
                                                Console.WriteLine("群消息发送失败");
                                            }
                                        }
                                    }
                                    else if (ja.Count == 3)
                                    {
                                        string target = ja[2]["target"]!.ToString();
                                        Call.Execute(target, x.GroupId, 3);
                                    }
                                    else
                                    {
                                        Call.Execute(text[1], x.GroupId, 3);
                                    }
                                }
                                catch
                                {
                                    Console.WriteLine("群消息发送失败");
                                }
                                break;
                            case < 2:
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                                }
                                catch
                                {
                                    Console.WriteLine("群消息发送失败");
                                }
                                break;
                            default:
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                                }
                                catch
                                {
                                    Console.WriteLine("群消息发送失败");
                                }
                                break;
                        }
                    }
                }
                // 鸣谢
                if (x.MessageChain.GetPlainMessage() == "鸣谢")
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId,
                        "感谢Leobot和Hanbot给我的启发，感谢Leo给我提供C#的技术支持，也感谢Setup群各位群员对我的支持！");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
                // 精神心理疾病科普
                MentalHealth.Execute(x);
                // 处理“你就是歌姬吧”（祖安）
                Zuan.Execute(x);
                // 群管功能
                // 禁言
                if (x.MessageChain.GetPlainMessage().StartsWith("/mute"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;  //正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (text.Length != 1)
                    {
                        switch (text.Length)
                        {
                            case 3:
                                Admin.Mute(x.Sender.Id, text[1], x.GroupId, text[2].ToInt32());
                                break;
                            case 2:
                                switch (ja.Count)
                                {
                                    case 4:
                                        string t = ja[3]["text"]!.ToString().Replace(" ", "");
                                        string target = ja[2]["target"]!.ToString();
                                        if (t == "")
                                        {
                                            Admin.Mute(x.Sender.Id, target, x.GroupId, 10);
                                        }
                                        else
                                        {
                                            int time = t.ToInt32();
                                            Admin.Mute(x.Sender.Id, target, x.GroupId, time);
                                        }
                                        break;
                                    case 3:
                                        string target1 = ja[2]["target"]!.ToString();
                                        Admin.Mute(x.Sender.Id, target1, x.GroupId, 10);
                                        break;
                                    case 2:
                                        Admin.Mute(x.Sender.Id, text[1], x.GroupId, 10);
                                        break;
                                }
                                break;
                            default:
                                {
                                    try
                                    {
                                        await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                                    }
                                    catch
                                    {
                                        Console.WriteLine("群消息发送失败");
                                    }
                                    break;
                                }
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                }
                // 解禁
                if (x.MessageChain.GetPlainMessage().StartsWith("/unmute"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;  //正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        switch (ja.Count)
                        {
                            case 3:
                                string target = ja[2]["target"]!.ToString();
                                Admin.Unmute(x.Sender.Id, target, x.GroupId);
                                break;
                            case 2:
                                Admin.Unmute(x.Sender.Id, text[1], x.GroupId);
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                }
                // 踢人
                if (x.MessageChain.GetPlainMessage().StartsWith("/kick"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;  //正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        switch (ja.Count)
                        {
                            case 3:
                                string target = ja[2]["target"]!.ToString();
                                Admin.Kick(x.Sender.Id, target, x.GroupId);
                                break;
                            case 2:
                                Admin.Kick(x.Sender.Id, text[1], x.GroupId);
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                }
                // 加黑
                if (x.MessageChain.GetPlainMessage().StartsWith("/block"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;  //正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        switch (ja.Count)
                        {
                            case 3:
                                string target = ja[2]["target"]!.ToString();
                                Admin.Block(x.Sender.Id, target, x.GroupId);
                                break;
                            case 2:
                                Admin.Block(x.Sender.Id, text[1], x.GroupId);
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                // 解黑
                if (x.MessageChain.GetPlainMessage().StartsWith("/unblock"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;  //正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        switch (ja.Count)
                        {
                            case 3:
                                string target = ja[2]["target"]!.ToString();
                                Admin.Unblock(x.Sender.Id, target, x.GroupId);
                                break;
                            case 2:
                                Admin.Unblock(x.Sender.Id, text[1], x.GroupId);
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                }
                // 全局加黑
                if (x.MessageChain.GetPlainMessage().StartsWith("/gblock"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;  //正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        switch (ja.Count)
                        {
                            case 3:
                                string target = ja[2]["target"]!.ToString();
                                Admin.G_Block(x.Sender.Id, target, x.GroupId);
                                break;
                            case 2:
                                Admin.G_Block(x.Sender.Id, text[1], x.GroupId);
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                // 全局解黑
                if (x.MessageChain.GetPlainMessage().StartsWith("/gunblock"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;  //正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        switch (ja.Count)
                        {
                            case 3:
                                string target = ja[2]["target"]!.ToString();
                                Admin.G_Unblock(x.Sender.Id, target, x.GroupId);
                                break;
                            case 2:
                                Admin.G_Unblock(x.Sender.Id, text[1], x.GroupId);
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                }
                // 给予机器人管理员
                if (x.MessageChain.GetPlainMessage().StartsWith("/op"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;//正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        switch (ja.Count)
                        {
                            case 3:
                                string target = ja[2]["target"]!.ToString();
                                Admin.Op(x.Sender.Id, target, x.GroupId);
                                break;
                            case 2:
                                Admin.Op(x.Sender.Id, text[1], x.GroupId);
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                // 剥夺机器人管理员
                if (x.MessageChain.GetPlainMessage().StartsWith("/deop"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;  //正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        switch (ja.Count)
                        {
                            case 3:
                                string target = ja[2]["target"]!.ToString();
                                Admin.Deop(x.Sender.Id, target, x.GroupId);
                                break;
                            case 2:
                                Admin.Deop(x.Sender.Id, text[1], x.GroupId);
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                }
                // 给予全局机器人管理员
                if (x.MessageChain.GetPlainMessage().StartsWith("/gop"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;//正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        switch (ja.Count)
                        {
                            case 3:
                                string target = ja[2]["target"]!.ToString();
                                Admin.G_Op(x.Sender.Id, target, x.GroupId);
                                break;
                            case 2:
                                Admin.G_Op(x.Sender.Id, text[1], x.GroupId);
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                // 剥夺全局机器人管理员
                if (x.MessageChain.GetPlainMessage().StartsWith("/gdeop"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;  //正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        switch (ja.Count)
                        {
                            case 3:
                                string target = ja[2]["target"]!.ToString();
                                Admin.G_Deop(x.Sender.Id, target, x.GroupId);
                                break;
                            case 2:
                                Admin.G_Deop(x.Sender.Id, text[1], x.GroupId);
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                }
                // 屏蔽消息
                if (x.MessageChain.GetPlainMessage().StartsWith("/ignore"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;//正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        switch (ja.Count)
                        {
                            case 3:
                                string target = ja[2]["target"]!.ToString();
                                Admin.Ignore(x.Sender.Id, target, x.GroupId);
                                break;
                            case 2:
                                Admin.Ignore(x.Sender.Id, text[1], x.GroupId);
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                // 全局屏蔽消息
                if (x.MessageChain.GetPlainMessage().StartsWith("/gignore"))
                {
                    string result1 = x.MessageChain.ToJsonString();
                    JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;//正常获取jobject
                    string[] text = ja[1]["text"]!.ToString().Split(" ");
                    if (text.Length == 2)
                    {
                        switch (ja.Count)
                        {
                            case 3:
                                string target = ja[2]["target"]!.ToString();
                                Admin.G_Ignore(x.Sender.Id, target, x.GroupId);
                                break;
                            case 2:
                                Admin.G_Ignore(x.Sender.Id, text[1], x.GroupId);
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "缺少参数");
                        }
                        catch { }
                    }
                }
                // 发动带清洗
                if (x.MessageChain.GetPlainMessage() == ("/purge"))
                {
                    Admin.Purge(x.Sender.Id,x.GroupId);
                }
                // 合并黑名单并双向同步
                if (x.MessageChain.GetPlainMessage() == ("/merge"))
                {
                    Syncs.Merge(x, x.GroupId, x.Sender.Id);
                }
                // 重新加载
                if (x.MessageChain.GetPlainMessage() == ("/update"))
                {
                    Update.Execute();
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId, "重新加载列表成功！");
                    }
                    catch { }
                }
                // 同步黑名单
                if (x.MessageChain.GetPlainMessage() == ("/sync"))
                {
                    Syncs.Sync(x, x.GroupId, x.Sender.Id);
                }
                // 反向同步黑名单
                if (x.MessageChain.GetPlainMessage() == ("/rsync"))
                {
                    Syncs.Rsync(x, x.GroupId, x.Sender.Id);
                }
                // 合并黑名单并双向同步
                if (x.MessageChain.GetPlainMessage() == ("/merge"))
                {
                    Syncs.Merge(x, x.GroupId, x.Sender.Id);
                }
                // 版本
                if (x.MessageChain.GetPlainMessage() == "版本")
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId,
                        "机器人版本：b2.0.0\r\n上次更新日期：2022/11/25\r\n更新内容：改用MySQL数据库存储和处理数据，更加高效；对代码片段执行了优化以提升效率并增加可构建性");
                    }
                    catch
                    {
                        Console.WriteLine("群消息发送失败");
                    }
                }
            });
            // 然后在这之后卡住主线程（也可以使用别的方式，文档假设阅读者是个C#初学者）
            Console.ReadLine();
        }
    }
}