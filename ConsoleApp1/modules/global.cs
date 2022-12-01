// 2kbot b2.3.0
// Copyright(C) 2022 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

// 致所有构建及修改2kbot代码片段的用户：作者（Abjust）并不承担构建2kbot代码片段（包括修改过的版本）所产生的一切风险，但是用户有权在2kbot的GitHub项目页提出issue，并有权在代码片段修复这些问题后获取这些更新，但是，作者不会对修改过的代码版本做质量保证，也没有义务修正在修改过的代码片段中存在的任何缺陷。

// 提交Git并将代码片段推送到GitHub仓库之前，请将该文件的隐私部分（如数据库信息、机器人QQ号）删除，以免带来不必要的麻烦

namespace Net_2kBot.Modules
{
    public class Global
    {
        public static List<string>? ops;
        public static List<string>? g_ops;
        public static List<string>? blocklist;
        public static List<string>? g_blocklist;
        public static List<string>? ignores;
        public static List<string>? g_ignores;
        public static long last_call;
        public static long time_now;
        public const int call_cd = 40;
        public const int repeat_cd = 300;
        public const int repeat_threshold = 5;
        public const int repeat_interval = 10;
        public const int breadfactory_maxlevel = 5;
        public static readonly string path = Directory.GetCurrentDirectory();
        public static readonly string owner_qq = "";
        public static readonly string api = "";
        public static readonly string api_key = "";
        public static readonly string qq = "";
        public static readonly string verify_key = "";
        public const string database_host = "";
        public const string database_user = "";
        public const string database_passwd = "";
        public const string database_name = "";
        public static readonly string connectstring = $"server={database_host};userid={database_user};password={database_passwd};database={database_name}";
    }
}
