using MySql.Data.MySqlClient;

namespace Net_2kBot.Modules
{
    public static class Update
    {
        public static void Execute()
        {
            string a;
            string b;
            // 连接数据库
            MySqlConnection msc = new(Global.connectstring);
            MySqlCommand cmd;
            msc.Open();
            // 更新op列表
            cmd = new MySqlCommand($"SELECT * FROM ops WHERE qid IS NOT NULL AND gid IS NOT NULL;", msc);
            MySqlDataReader reader = cmd.ExecuteReader();
            List<string> ops = new();
            while (reader.Read())
            {
                a = reader.GetString("gid");
                b = reader.GetString("qid");
                ops.Add($"{a}_{b}");
                Global.ops = ops;
            }
            reader.Close();
            // 更新黑名单
            cmd = new MySqlCommand($"SELECT * FROM blocklist WHERE qid IS NOT NULL AND gid IS NOT NULL;", msc);
            reader = cmd.ExecuteReader();
            List<string> blocklist = new();
            while (reader.Read())
            {
                a = reader.GetString("gid");
                b = reader.GetString("qid");
                blocklist.Add($"{a}_{b}");
                Global.blocklist = blocklist;
            }
            reader.Close();
            // 更新屏蔽列表
            cmd = new MySqlCommand($"SELECT * FROM ignores WHERE qid IS NOT NULL AND gid IS NOT NULL;", msc);
            reader = cmd.ExecuteReader();
            List<string> ignores = new();
            while (reader.Read())
            {
                a = reader.GetString("gid");
                b = reader.GetString("qid");
                ignores.Add($"{a}_{b}");
                Global.ignores = ignores;
            }
            reader.Close();
            // 更新全局op列表
            cmd = new MySqlCommand($"SELECT * FROM g_ops WHERE qid IS NOT NULL;", msc);
            reader = cmd.ExecuteReader();
            List<string> g_ops = new();
            while (reader.Read())
            {
                a = reader.GetString("qid");
                g_ops.Add(a);
                Global.g_ops = g_ops;
            }
            reader.Close();
            // 更新全局黑名单
            cmd = new MySqlCommand($"SELECT * FROM g_blocklist WHERE qid IS NOT NULL;", msc);
            reader = cmd.ExecuteReader();
            List<string> g_blocklist = new();
            while (reader.Read())
            {
                a = reader.GetString("qid");
                g_blocklist.Add(a);
                Global.g_blocklist = g_blocklist;
            }
            reader.Close();
            // 更新全局屏蔽列表
            cmd = new MySqlCommand($"SELECT * FROM g_ignores WHERE qid IS NOT NULL;", msc);
            reader = cmd.ExecuteReader();
            List<string> g_ignores = new();
            while (reader.Read())
            {
                a = reader.GetString("qid");
                g_ignores.Add(a);
                Global.g_ignores = g_ignores;
            }
            reader.Close();
            msc.Close();
        }
    }
}