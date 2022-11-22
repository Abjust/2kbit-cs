namespace Net_2kBot.Modules
{
    public class Global
    {
        public static long last_call;
        public static long time_now;
        public const int call_cd = 40;
        public static long last_repeat;
        public static long last_repeatctrl;
        public const int repeat_cd = 300;
        public const int repeat_threshold = 5;
        public const int repeat_interval = 10;
        public static int repeat_count = 0;
        public static string[]? ops;
        public static string[]? blocklist;
        public static string[]? ignores;
        public static readonly string path = Directory.GetCurrentDirectory();
        public static readonly string api_key = "";
        public static readonly string qq = "";
        public static readonly string verify_key = "";
    };
}
