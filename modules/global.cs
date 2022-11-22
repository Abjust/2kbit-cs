namespace Net_2kBot.Modules
{
    public static class global
    {
        public static long last_call;
        public static long time_now;
        public static int cd = 40;
        public static string[]? ops;
        public static string[]? blocklist;
        public static string path = Directory.GetCurrentDirectory();
        public static string[]? ignores;
        public static string api_key = "";
        public static string qq = "";
        public static string verify_key = "";
    };
}
