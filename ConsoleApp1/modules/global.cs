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
        public static readonly string owner_qq = "2548452533";
        public static readonly string api = "http://152.136.205.97/bot_api";
        public static readonly string api_key = "k3kGvRtDPI";
        public static readonly string qq = "2810482259";
        public static readonly string verify_key = "989898123oO";
        public const string database_host = "localhost";
        public const string database_user = "root";
        public const string database_passwd = "Abjust.253389";
        public const string database_name = "2kbot";
        public static readonly string connectstring = $"server={database_host};userid={database_user};password={database_passwd};database={database_name}";
    }
}