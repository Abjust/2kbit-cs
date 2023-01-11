// 2kbit C# Edition，2kbit的C#分支版本
// Copyright(C) 2022 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

// 致所有构建及修改2kbit代码片段的用户：作者（Abjust）并不承担构建2kbit代码片段（包括修改过的版本）所产生的一切风险，但是用户有权在2kbit的GitHub项目页提出issue，并有权在代码片段修复这些问题后获取这些更新，但是，作者不会对修改过的代码版本做质量保证，也没有义务修正在修改过的代码片段中存在的任何缺陷。

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
using Net_2kBit.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reactive.Linq;

namespace Net_2kBit
{
    public static class BotMain
    {
        public static async Task Main()
        {
            // 初始化全局变量
            if (!System.IO.File.Exists("global.txt"))
            {
                string[] lines =
                {
                    "owner_qq=", "api=", "api_key=","bot_qq=","verify_key=","database_host=","database_user=","database_passwd=","database_name="
                };
                System.IO.File.Create("global.txt").Close();
                await System.IO.File.WriteAllLinesAsync("global.txt", lines);
                Console.WriteLine("全局变量文件已创建！现在，你需要前往项目文件夹或者程序文件夹找到global.txt并按照要求编辑");
                Environment.Exit(0);
            }
            else
            {
                foreach (string line in System.IO.File.ReadLines("global.txt"))
                {
                    string[] split = line.Split("=");
                    if (split.Length == 2)
                    {
                        switch (split[0])
                        {
                            case "owner_qq":
                                Global.owner_qq = split[1];
                                break;
                            case "api":
                                Global.api = split[1];
                                break;
                            case "api_key":
                                Global.api_key = split[1];
                                break;
                            case "bot_qq":
                                Global.bot_qq = split[1];
                                break;
                            case "verify_key":
                                Global.verify_key = split[1];
                                break;
                            case "database_host":
                                Global.database_host = split[1];
                                break;
                            case "database_user":
                                Global.database_user = split[1];
                                break;
                            case "database_passwd":
                                Global.database_passwd = split[1];
                                break;
                            case "database_name":
                                Global.database_name = split[1];
                                break;
                        }
                    }
                }
                Global.connectstring = $"server={Global.database_host};userid={Global.database_user};password={Global.database_passwd};database={Global.database_name}";
            }
            // 启动机器人程序
            MiraiBot bot = new()
            {
                Address = "localhost:8080",
                QQ = Global.bot_qq,
                VerifyKey = Global.verify_key
            };
            // 注意: `LaunchAsync`是一个异步方法，请确保`Main`方法的返回值为`Task`
            await bot.LaunchAsync();
            // 启动成功提示
            Console.WriteLine("2kbit已启动！");
            // 初始化
            // 连接数据库
            using (var msc = new MySqlConnection(Global.connectstring))
            {
                await msc.OpenAsync();
                MySqlCommand cmd = new()
                {
                    Connection = msc
                };
                // 若数据表不存在则创建
                cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS blocklist (
 `id` INT NOT NULL AUTO_INCREMENT,
 `qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',
 `gid` VARCHAR(10) NOT NULL COMMENT 'Q群号',
 PRIMARY KEY (`id`));
CREATE TABLE IF NOT EXISTS ops (
 `id` INT NOT NULL AUTO_INCREMENT,
 `qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',
 `gid` VARCHAR(10) NOT NULL COMMENT 'Q群号',
 PRIMARY KEY (`id`));
CREATE TABLE IF NOT EXISTS ignores (
 `id` INT NOT NULL AUTO_INCREMENT,
 `qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',
 `gid` VARCHAR(10) NOT NULL COMMENT 'Q群号',
 PRIMARY KEY (`id`));
CREATE TABLE IF NOT EXISTS g_blocklist (
 `id` INT NOT NULL AUTO_INCREMENT,
 `qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',
 PRIMARY KEY (`id`));
CREATE TABLE IF NOT EXISTS g_ops (
 `id` INT NOT NULL AUTO_INCREMENT,
 `qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',
 PRIMARY KEY (`id`));
CREATE TABLE IF NOT EXISTS g_ignores (
 `id` INT NOT NULL AUTO_INCREMENT,
 `qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',
 PRIMARY KEY (`id`));
CREATE TABLE IF NOT EXISTS repeatctrl (
 `id` INT NOT NULL AUTO_INCREMENT,
 `qid` VARCHAR(10) NOT NULL COMMENT 'QQ号',
 `gid` VARCHAR(10) NOT NULL COMMENT 'Q群号',
 `last_repeat` bigint NOT NULL DEFAULT '946656000' COMMENT '上次复读时间',
 `last_repeatctrl` bigint NOT NULL DEFAULT '946656000' COMMENT '上次复读控制时间',
 `repeat_count` TINYINT UNSIGNED NOT NULL DEFAULT 0 COMMENT '复读计数',
PRIMARY KEY (`id`));
CREATE TABLE IF NOT EXISTS bread (
  `id` int NOT NULL AUTO_INCREMENT,
  `gid` varchar(10) NOT NULL COMMENT 'Q群号',
  `factory_level` int NOT NULL DEFAULT '1' COMMENT '面包厂等级',
  `storage_upgraded` int NOT NULL DEFAULT '0' COMMENT '库存升级次数',
  `speed_upgraded` int NOT NULL DEFAULT '0' COMMENT '生产速度升级次数',
  `output_upgraded` int NOT NULL DEFAULT '0' COMMENT '产量升级次数',
  `factory_mode` tinyint NOT NULL DEFAULT '0' COMMENT '面包厂生产模式',
  `factory_exp` int NOT NULL DEFAULT '0' COMMENT '面包厂经验',
  `breads` int NOT NULL DEFAULT '0' COMMENT '面包库存',
  `exp_gained_today` int NOT NULL DEFAULT '0' COMMENT '近24小时获取经验数',
  `last_expfull` bigint NOT NULL DEFAULT '946656000' COMMENT '上次达到经验上限时间',
  `last_expgain` bigint NOT NULL DEFAULT '946656000' COMMENT '近24小时首次获取经验时间',
  `last_produce` bigint NOT NULL DEFAULT '946656000' COMMENT '上次完成一轮生产周期时间',
  PRIMARY KEY (`id`));
CREATE TABLE IF NOT EXISTS material (
  `id` int NOT NULL AUTO_INCREMENT,
  `gid` varchar(10) NOT NULL COMMENT 'Q群号',
  `flour` int NOT NULL DEFAULT 0 COMMENT '面粉数量',
  `egg` int NOT NULL DEFAULT 0 COMMENT '鸡蛋数量',
  `yeast` int NOT NULL DEFAULT 0 COMMENT '酵母数量',
  `last_produce` bigint NOT NULL DEFAULT '946656000' COMMENT '上次完成一轮生产周期时间',
  PRIMARY KEY (`id`));
CREATE TABLE IF NOT EXISTS woodenfish (
  `id` int NOT NULL AUTO_INCREMENT,
  `uid` varchar(10) NOT NULL COMMENT '赛博账号',
  `time` bigint NOT NULL COMMENT '上次计算时间',
  `level` int NOT NULL DEFAULT '1' COMMENT '木鱼等级',
  `exp` bigint unsigned NOT NULL DEFAULT '1' COMMENT '木鱼经验',
  `gongde` bigint unsigned NOT NULL COMMENT '功德',
  `ban` int NOT NULL DEFAULT '0' COMMENT '封禁状态',
  `dt` bigint NOT NULL DEFAULT '946656000' COMMENT '封禁结束时间',
  `end_time` bigint NOT NULL DEFAULT '946656000' COMMENT '最近一次敲木鱼时间',
  `hit_count` int NOT NULL DEFAULT '0' COMMENT '敲木鱼次数',
  `info_time` bigint NOT NULL DEFAULT '0' COMMENT '最近一次信息查询时间',
  `info_count` int NOT NULL DEFAULT '0' COMMENT '信息查询次数',
  `info_ctrl` bigint NOT NULL DEFAULT '946656000' COMMENT '信息查询限制结束时间',
  `total_ban` int NOT NULL DEFAULT '0' COMMENT '累计封禁次数',
  PRIMARY KEY (`id`));
INSERT IGNORE INTO material (id, gid) SELECT id, gid FROM bread";
                await cmd.ExecuteNonQueryAsync();
                // 更新数据表
                try
                {
                    cmd.CommandText = @"
ALTER TABLE bread
ADD COLUMN `speed_upgraded` INT NOT NULL DEFAULT 0 COMMENT '生产速度升级次数' AFTER `storage_upgraded`,
ADD COLUMN `output_upgraded` INT NOT NULL DEFAULT 0 COMMENT '产量升级次数' AFTER `speed_upgraded`,
CHANGE COLUMN `bread_diversity` `factory_mode` TINYINT NOT NULL DEFAULT '0' COMMENT '面包厂生产模式' ;";
                    await cmd.ExecuteNonQueryAsync();
                }
                catch { }
            }
            // 在这里添加你的代码，比如订阅消息/事件之类的
            Update.Execute();
            // 戳一戳效果
            bot.EventReceived
            .OfType<NudgeEvent>()
            .Subscribe(receiver =>
            {
                if (receiver.Target == Global.bot_qq && receiver.Subject.Kind == "Group")
                {
                    Zuan.Execute(receiver.FromId, receiver.Subject.Id, @event: receiver);
                }
            });
            // bot被加好友
            bot.EventReceived
            .OfType<NewFriendRequestedEvent>()
            .Subscribe(async e =>
            {
                await e.ApproveAsync();
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
                if ((Global.blocklist != null && Global.blocklist.Contains($"{e.GroupId}_{e.FromId}")) || (Global.g_blocklist != null && Global.g_blocklist.Contains(e.FromId)))
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
            // 侦测入群
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
            // bot对接收私聊消息的处理
            bot.MessageReceived
           .OfType<FriendMessageReceiver>()
           .Subscribe(async x =>
           {
               if (x.FriendId != Global.owner_qq)
               {
                   MessageChain? messageChain = new MessageChainBuilder()
                   .Plain($"消息来自：{x.FriendName} ({x.FriendId})\n消息内容：")
                   .Build();
                   foreach (MessageBase message in x.MessageChain)
                   {
                       messageChain.Add(message);
                   }
                   await MessageManager.SendFriendMessageAsync(Global.owner_qq, messageChain);
                   await MessageManager.SendFriendMessageAsync(Global.owner_qq, "你可以使用/send <目标QQ> <消息>来发送私聊消息");
               }
               else if (x.FriendId == Global.owner_qq && x.MessageChain.GetPlainMessage().StartsWith("/send"))
               {
                   string[] result = x.MessageChain.GetPlainMessage().Split(" ");
                   if (result.Length > 2)
                   {
                       try
                       {
                           string results = "";
                           for (int i = 2; i < result.Length; i++)
                           {
                               if (i == 2)
                               {
                                   results = result[i];
                               }
                               else
                               {
                                   results = results + " " + result[i];
                               }
                           }
                           try
                           {
                               await MessageManager.SendFriendMessageAsync(result[1], results);
                           }
                           catch
                           {
                               await x.SendMessageAsync("私聊消息发送失败");
                           }
                       }
                       catch
                       {
                           await x.SendMessageAsync("参数错误");
                       }
                   }
                   else
                   {
                       await x.SendMessageAsync("参数错误");
                   }
               }
           });
            // bot对接收群消息的处理
            bot.MessageReceived
            .OfType<GroupMessageReceiver>()
            .Subscribe(async x =>
            {
                if ((Global.ignores == null || !Global.ignores.Contains($"{x.GroupId}_{x.Sender.Id}")) && (Global.g_ignores == null || !Global.g_ignores.Contains(x.Sender.Id)))
                {
                    // 面包厂相关
                    string[] text1 = x.MessageChain.GetPlainMessage().Split(" ");
                    if (text1.Length == 2)
                    {
                        int number;
                        switch (text1[0])
                        {
                            case "/givebread":
                                if (int.TryParse(text1[1], out number))
                                {
                                    Bread.Give(x.GroupId, x.Sender.Id, number);
                                }
                                break;
                            case "/getbread":
                                if (int.TryParse(text1[1], out number))
                                {
                                    Bread.Get(x.GroupId, x.Sender.Id, number);
                                }
                                break;
                            case "/change_mode":
                                switch (text1[1])
                                {
                                    case "infinite": Bread.ChangeMode(x.GroupId, 2); break;
                                    case "diversity": Bread.ChangeMode(x.GroupId, 1); break;
                                    case "normal": Bread.ChangeMode(x.GroupId, 0); break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (text1[0])
                        {
                            case "/query": Bread.Query(x.GroupId, x.Sender.Id); break;
                            case "/upgrade_factory": Bread.UpgradeFactory(x.GroupId); break;
                            case "/build_factory": Bread.BuildFactory(x.GroupId); break;
                            case "/upgrade_storage": Bread.UpgradeStorage(x.GroupId); break;
                            case "/upgrade_speed": Bread.UpgradeSpeed(x.GroupId); break;
                            case "/upgrade_output": Bread.UpgradeOutput(x.GroupId); break;
                        }
                    }
                    // 计算经验
                    Bread.GetExp(x);
                    // 复读机
                    Repeat.Execute(x);
                    // 数学计算
                    if (x.MessageChain.GetPlainMessage().StartsWith("/calc"))
                    {
                        Mathematics.Execute(x);
                    }
                    // 发送公告
                    if (x.MessageChain.GetPlainMessage().StartsWith("/announce"))
                    {
                        IEnumerable<Group> groups = AccountManager.GetGroupsAsync().GetAwaiter().GetResult();
                        Announce.Execute(x, x.Sender.Id, groups);
                    }
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
                                    if (ja.Count == 4)
                                    {
                                        string target = ja[2]["target"]!.ToString();
                                        string t = ja[3]["text"]!.ToString().Replace(" ", "");
                                        int time = t.ToInt32();
                                        try
                                        {
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
                    Zuan.Execute(x.Sender.Id, x.GroupId, x.MessageChain);
                    // 群管功能
                    // 禁言
                    if (x.MessageChain.GetPlainMessage().StartsWith("/mute") && !x.MessageChain.GetPlainMessage().StartsWith("/muteme"))
                    {
                        string result1 = x.MessageChain.ToJsonString();
                        JArray ja = (JArray)JsonConvert.DeserializeObject(result1)!;  //正常获取jobject
                        string[] text = ja[1]["text"]!.ToString().Split(" ");
                        if (text.Length != 1)
                        {
                            switch (text.Length)
                            {
                                case 3:
                                    Admin.Mute(x.Sender.Id, text[1], x.GroupId, x.Sender.Permission.ToString(), text[2].ToInt32());
                                    break;
                                case 2:
                                    switch (ja.Count)
                                    {
                                        case 4:
                                            string t = ja[3]["text"]!.ToString().Replace(" ", "");
                                            string target = ja[2]["target"]!.ToString();
                                            if (t == "")
                                            {
                                                Admin.Mute(x.Sender.Id, target, x.GroupId, x.Sender.Permission.ToString(), 10);
                                            }
                                            else
                                            {
                                                int time = t.ToInt32();
                                                Admin.Mute(x.Sender.Id, target, x.GroupId, x.Sender.Permission.ToString(), time);
                                            }
                                            break;
                                        case 3:
                                            string target1 = ja[2]["target"]!.ToString();
                                            Admin.Mute(x.Sender.Id, target1, x.GroupId, x.Sender.Permission.ToString(), 10);
                                            break;
                                        case 2:
                                            Admin.Mute(x.Sender.Id, text[1], x.GroupId, x.Sender.Permission.ToString(), 10);
                                            break;
                                    }
                                    break;
                                default:
                                    {
                                        try
                                        {
                                            await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
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
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
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
                                    Admin.Unmute(x.Sender.Id, target, x.GroupId, x.Sender.Permission.ToString());
                                    break;
                                case 2:
                                    Admin.Unmute(x.Sender.Id, text[1], x.GroupId, x.Sender.Permission.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
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
                                    Admin.Kick(x.Sender.Id, target, x.GroupId, x.Sender.Permission.ToString());
                                    break;
                                case 2:
                                    Admin.Kick(x.Sender.Id, text[1], x.GroupId, x.Sender.Permission.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
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
                                    Admin.Block(x.Sender.Id, target, x.GroupId, x.Sender.Permission.ToString());
                                    break;
                                case 2:
                                    Admin.Block(x.Sender.Id, text[1], x.GroupId, x.Sender.Permission.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
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
                                    Admin.Unblock(x.Sender.Id, target, x.GroupId, x.Sender.Permission.ToString());
                                    break;
                                case 2:
                                    Admin.Unblock(x.Sender.Id, text[1], x.GroupId, x.Sender.Permission.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
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
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
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
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
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
                                    Admin.Op(x.Sender.Id, target, x.GroupId, x.Sender.Permission.ToString());
                                    break;
                                case 2:
                                    Admin.Op(x.Sender.Id, text[1], x.GroupId, x.Sender.Permission.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
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
                        IEnumerable<Member> members = new List<Member>();
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
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
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
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
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
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
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
                                    Admin.Ignore(x.Sender.Id, target, x.GroupId, x.Sender.Permission.ToString());
                                    break;
                                case 2:
                                    Admin.Ignore(x.Sender.Id, text[1], x.GroupId, x.Sender.Permission.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
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
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
                            }
                            catch { }
                        }
                    }
                    // 禁言自己
                    if (x.MessageChain.GetPlainMessage().StartsWith("/muteme"))
                    {
                        string[] text = x.MessageChain.GetPlainMessage().Split(" ");
                        if (text.Length == 2)
                        {
                            Admin.MuteMe(x.Sender.Id, x.GroupId, text[1].ToInt32());
                        }
                        else if (text.Length == 1)
                        {
                            Admin.MuteMe(x.Sender.Id, x.GroupId, 10);
                        }
                        else
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(x.GroupId, "参数错误");
                            }
                            catch { }
                        }
                    }
                    // 发动带清洗
                    if (x.MessageChain.GetPlainMessage() == ("/purge"))
                    {
                        Admin.Purge(x.Sender.Id, x.GroupId, x.Sender.Permission.ToString());
                    }
                    // 同步黑名单
                    if (x.MessageChain.GetPlainMessage() == ("/sync"))
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId, "因HanBot API存在问题，同步功能被暂时禁用！");
                    }
                    // 反向同步黑名单
                    if (x.MessageChain.GetPlainMessage() == ("/rsync"))
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId, "因HanBot API存在问题，同步功能被暂时禁用！");
                    }
                    // 合并黑名单并双向同步
                    if (x.MessageChain.GetPlainMessage() == ("/merge"))
                    {
                        await MessageManager.SendGroupMessageAsync(x.GroupId, "因HanBot API存在问题，同步功能被暂时禁用！");
                    }
                    // 电子木鱼
                    if (x.MessageChain.GetPlainMessage() == "我的木鱼")
                    {
                        WoodenFish.Info(x.GroupId, x.Sender.Id);
                    }
                    if (x.MessageChain.GetPlainMessage() == "给我木鱼")
                    {
                        WoodenFish.Register(x.GroupId, x.Sender.Id);
                    }
                    if (x.MessageChain.GetPlainMessage() == "敲木鱼")
                    {
                        WoodenFish.Hit(x.GroupId, x.Sender.Id);
                    }
                    if (x.MessageChain.GetPlainMessage() == "1")
                    {
                        WoodenFish.Laugh(x.GroupId, x.Sender.Id);
                    }
                    // 精神状况监控（beta）
                    List<string> words = new()
                        {
                            "我想自杀",
                            "我想自残",
                            "我想死",
                            "想自杀",
                            "想自残",
                            "想死",
                            "我想zs",
                            "我想zc",
                            "我想s",
                            "想zs",
                            "想zc",
                            "想s",
                            "zs",
                            "zc",
                            "心烦",
                            "我累了",
                            "我一点都不难过",
                            "我很开心啊"
                        };
                    foreach (string word in words)
                    {
                        if (x.MessageChain.GetPlainMessage().ToLower().Contains(word))
                        {
                            Global.time_now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                            using (var msc = new MySqlConnection(Global.connectstring))
                            {
                                await msc.OpenAsync();
                                MySqlCommand cmd = new()
                                {
                                    Connection = msc
                                };
                                cmd.CommandText = "SELECT COUNT(*) qid FROM mentalhealth WHERE qid = @qid;";
                                cmd.Parameters.AddWithValue("@qid", x.Sender.Id);
                                int i = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                                // 如不存在便创建
                                if (i == 0)
                                {
                                    cmd.CommandText = "INSERT INTO mentalhealth (qid) VALUES (@qid);";
                                    await cmd.ExecuteNonQueryAsync();
                                }
                                cmd.CommandText = "SELECT * FROM mentalhealth WHERE qid = @qid;";
                                MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                                while (reader.Read())
                                {
                                    using (var msc1 = new MySqlConnection(Global.connectstring))
                                    {
                                        await msc1.OpenAsync();
                                        MySqlCommand cmd1 = new()
                                        {
                                            Connection = msc1
                                        };
                                        cmd1.CommandText = "UPDATE mentalhealth SET attempts = @attempts, last_record = @time_now WHERE qid = @qid";
                                        cmd1.Parameters.AddWithValue("@attempts", reader.GetInt32("attempts") + 1);
                                        cmd1.Parameters.AddWithValue("@time_now", Global.time_now);
                                        cmd1.Parameters.AddWithValue("@qid", x.Sender.Id);
                                        await cmd1.ExecuteNonQueryAsync();
                                        break;
                                    }
                                }
                                await reader.CloseAsync();
                                List<string> poems = new()
                                {
                                    @"
假如生活欺骗了你，
不要悲伤，不要心急！
忧郁的日子里须要镇静：
相信吧，快乐的日子将会来临！
心儿永远向往着未来；
现在却常是忧郁：
一切都是瞬息，一切都将会过去；
而那过去了的，就会成为亲切的怀恋。
",
                                    @"
整个自然都是艺术，不过你不领悟；
一切偶然都是规定，只是你没有看清；
一切不协，是你不理解的和谐；
一切局部的祸，乃是全体的福。
高傲可鄙，只因它不近情理。
凡存在的都合理，乃是清楚的道理。
",
                                    " 感谢科学，它不仅使生活充满快乐与欢欣，并且给生活以支柱和自尊心。",
                                    " 生活本来是个沉重的话题，它带给我们压力责任和使命，但如果我们用好的心态去面对，用宽广的胸怀去接纳，那快乐就会不期而至，伴随左右。心态决定想法，想法决定做法，做法决定结果。",
                                    " 应当赶紧地，充分地生活，因为意外的疾病或悲惨的事故随时都可以突然结束他的生命。",
                                    " 人是用心去活，而不是用脸去活，用心去活无非是认真的去感受生活中的点滴，享受生活的每一次给予。",
                                    " 生活的真谛就是懂得享受生活，而享受生活的真正目的就是使自己的心情达到一种舒畅或平静的状态，做事完全是自觉、自愿而且带着兴趣的。随心所欲并不是指金钱的方向和改变，而是指心灵的自由。",
                                    " 根本不必回头去看咒骂你的人是谁？如果有一条疯狗咬你一口，难道你也要趴下去反咬他一口吗？",
                                    " 世界的设计创造应以人为中心，而不是以谋取金钱，人并非以金钱为对象而生活，人的对象往往是人。",
                                    " 享受生活不需要寻找特殊的日子，因为每一天都是特殊的，享受生活就是享受今天。",
                                    " 应该相信，自己是生活的战胜者。",
                                    @"
从一粒沙看世界，
从一朵花看天堂，
把永恒纳进一个时辰，
把无限握在自己手心。",
                                    " 天若有情人亦老，人间正道是沧桑。"
                                };
                                Random r = new();
                                int random = r.Next(poems.Count);
                                MessageChain messageChain = new MessageChainBuilder()
                                    .At(x.Sender.Id)
                                    .Plain(poems[random])
                                    .Build();
                                await MessageManager.SendGroupMessageAsync(x.GroupId, messageChain);
                                break;
                            }
                        }
                    }
                    // 版本
                    if (x.MessageChain.GetPlainMessage() == "版本")
                    {
                        List<string> splashes = new()
                        {
                            "也试试HanBot罢！Also try HanBot!",
                            "誓死捍卫微软苏维埃！",
                            "打倒MF独裁分子！",
                            "要把反革命分子的恶臭思想，扫进历史的垃圾堆！",
                            "PHP是世界上最好的编程语言（雾）",
                            "社会主义好，社会主义好~",
                            "Minecraft很好玩，但也可以试试Terraria！",
                            "So Nvidia, f**k you!",
                            "战无不胜的马克思列宁主义万岁！",
                            "Bug是杀不完的，你杀死了一个Bug，就会有千千万万个Bug站起来！",
                            "跟张浩扬博士一起来学Jvav罢！",
                            "哼哼哼，啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊",
                            "你知道吗？其实你什么都不知道！",
                            "Tips:这是一条烫...烫..烫知识（）",
                            "你知道成功的秘诀吗？我告诉你成功的秘诀就是：我操你妈的大臭逼",
                            "有时候ctmd不一定是骂人 可能是传统美德",
                            "python不一定是编程语言 也可能是屁眼通红",
                            "这条标语虽然没有用，但是是有用的，因为他被加上了标语",
                            "使用C#编写！"
                        };
                        Random r = new();
                        int random = r.Next(splashes.Count);
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId,
                            $"机器人版本：b_23w04a\r\n上次更新日期：2023/1/11\r\n更新内容：添加了电子木鱼功能；添加了精神状况监控功能；添加了整点报时（仅22点整）\r\n---------\r\n{splashes[random]}");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                    // 获取源码
                    if (x.MessageChain.GetPlainMessage() == "源码" || (x.MessageChain.GetPlainMessage() == "获取源码") || (x.MessageChain.GetPlainMessage() == "怎样做这样的机器人"))
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(x.GroupId, "请前往https://github.com/Abjust/2kbit-cs获取2kbit C# Edition的源码！");
                        }
                        catch
                        {
                            Console.WriteLine("群消息发送失败");
                        }
                    }
                }
            });
            // 运行各项自动化任务
            var Tasks = new Task[]
            {
                Task.Run(async () => await BreadFactory.MaterialProduce()),
                Task.Run(async () => await BreadFactory.BreadProduce()),
                Task.Run(async () => await WoodenFish.ExpUpgrade()),
                Task.Run(async () => await Announce.Notification())
            };
            await Task.WhenAll(Tasks);
            Console.ReadLine();
        }
    }
}