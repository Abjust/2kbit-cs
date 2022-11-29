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