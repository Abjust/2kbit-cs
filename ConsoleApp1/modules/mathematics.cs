// 2kbot，一款用C#编写的基于mirai和mirai.net的自由机器人软件
// Copyright(C) 2022 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

// 致所有构建及修改2kbot代码片段的用户：作者（Abjust）并不承担构建2kbot代码片段（包括修改过的版本）所产生的一切风险，但是用户有权在2kbot的GitHub项目页提出issue，并有权在代码片段修复这些问题后获取这些更新，但是，作者不会对修改过的代码版本做质量保证，也没有义务修正在修改过的代码片段中存在的任何缺陷。

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;

namespace Net_2kBot.Modules
{
    public static class Mathematics
    {
        public static async void Execute(MessageReceiverBase @base)
        {
            if (@base is GroupMessageReceiver receiver)
            {
                if (receiver.MessageChain.GetPlainMessage() == "/calc")
                {
                    try
                    {
                        await MessageManager.SendGroupMessageAsync(receiver.GroupId, @"2kbot数学计算器
指令前缀为：/calc
在前缀后跟上空格可输入算式，例如：
基本运算
加法：<数字>+<数字>
减法：<数字>-<数字>
乘法：<数字>*<数字>
除法：<数字>/<数字>
乘方：<数字>^<数字>
三角函数运算
正弦：sin<数字>
余弦：cos<数字>
正切：tan<数字>
余切：cot<数字>
");
                    }
                    catch { }
                }
                else
                {
                    string formula = receiver.MessageChain.GetPlainMessage().Split(" ")[1];
                    bool is_int = true;
                    bool negative = false;
                    double double_ans = 0;
                    decimal decimal_ans = 0;
                    long int_ans = 0;
                    double operand;
                    string[] operands;
                    // 基本运算
                    if (formula.Contains("+"))
                    {
                        operands = formula.Split("+");
                        try
                        {
                            foreach (string number in operands)
                            {
                                if (double.Parse(number.Replace("n","")) % 1 != 0)
                                {
                                    is_int = false;
                                    break;
                                }
                            }
                            for (int i = 0; i < operands.Length; i++)
                            {
                                if (is_int)
                                {
                                    if (i == 0)
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            int_ans = 0 - int.Parse(operands[i].Replace("n", ""));
                                        }
                                        else
                                        {
                                            int_ans = int.Parse(operands[i]);
                                        }
                                    }
                                    else
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            int_ans += 0 - int.Parse(operands[i]);
                                        }
                                        else
                                        {
                                            int_ans += int.Parse(operands[i]);
                                        }
                                    }
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            decimal_ans = (decimal)(0 - double.Parse(operands[i]));
                                        }
                                        else
                                        {
                                            decimal_ans = (decimal)double.Parse(operands[i]);
                                        }
                                    }
                                    else
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            decimal_ans += (decimal)(0 - double.Parse(operands[i]));
                                        }
                                        else
                                        {
                                            decimal_ans += (decimal)(double.Parse(operands[i]));
                                        }
                                    }
                                }
                            }
                            if (is_int)
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{int_ans}");
                                }
                                catch
                                {
                                    Console.WriteLine("群消息发送失败");
                                }
                            }
                            else
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{decimal_ans}");
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
                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, "计算失败");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    else if (formula.Contains("-"))
                    {
                        operands = formula.Split("-");
                        try
                        {
                            foreach (string number in operands)
                            {
                                if (double.Parse(number.Replace("n", "")) % 1 != 0)
                                {
                                    is_int = false;
                                    break;
                                }
                            }
                            for (int i = 0; i < operands.Length; i++)
                            {
                                if (is_int)
                                {
                                    if (i == 0)
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            int_ans = 0 - int.Parse(operands[i].Replace("n", ""));
                                        }
                                        else
                                        {
                                            int_ans = int.Parse(operands[i]);
                                        }
                                    }
                                    else
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            int_ans -= 0 - int.Parse(operands[i]);
                                        }
                                        else
                                        {
                                            int_ans -= int.Parse(operands[i]);
                                        }
                                    }
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            decimal_ans = (decimal)(0 - double.Parse(operands[i]));
                                        }
                                        else
                                        {
                                            decimal_ans = (decimal)double.Parse(operands[i]);
                                        }
                                    }
                                    else
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            decimal_ans -= (decimal)(0 - double.Parse(operands[i]));
                                        }
                                        else
                                        {
                                            decimal_ans -= (decimal)(double.Parse(operands[i]));
                                        }
                                    }
                                }
                            }
                            if (is_int)
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{int_ans}");
                                }
                                catch
                                {
                                    Console.WriteLine("群消息发送失败");
                                }
                            }
                            else
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{decimal_ans}");
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
                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, "计算失败");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    else if (formula.Contains("*"))
                    {
                        operands = formula.Split("*");
                        try
                        {
                            foreach (string number in operands)
                            {
                                if (double.Parse(number.Replace("n", "")) % 1 != 0)
                                {
                                    is_int = false;
                                    break;
                                }
                            }
                            for (int i = 0; i < operands.Length; i++)
                            {
                                if (is_int)
                                {
                                    if (i == 0)
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            int_ans = 0 - int.Parse(operands[i].Replace("n", ""));
                                        }
                                        else
                                        {
                                            int_ans = int.Parse(operands[i]);
                                        }
                                    }
                                    else
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            int_ans *= 0 - int.Parse(operands[i]);
                                        }
                                        else
                                        {
                                            int_ans *= int.Parse(operands[i]);
                                        }
                                    }
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            decimal_ans = (decimal)(0 - double.Parse(operands[i]));
                                        }
                                        else
                                        {
                                            decimal_ans = (decimal)double.Parse(operands[i]);
                                        }
                                    }
                                    else
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            decimal_ans *= (decimal)(0 - double.Parse(operands[i]));
                                        }
                                        else
                                        {
                                            decimal_ans *= (decimal)(double.Parse(operands[i]));
                                        }
                                    }
                                }
                            }
                            if (is_int)
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{int_ans}");
                                }
                                catch
                                {
                                    Console.WriteLine("群消息发送失败");
                                }
                            }
                            else
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{decimal_ans}");
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
                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, "计算失败");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    else if (formula.Contains("/"))
                    {
                        operands = formula.Split("/");
                        try
                        {
                            foreach (string number in operands)
                            {
                                if (double.Parse(number.Replace("n", "")) % 1 != 0)
                                {
                                    is_int = false;
                                    break;
                                }
                            }
                            for (int i = 0; i < operands.Length; i++)
                            {
                                if (is_int)
                                {
                                    if (i == 0)
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            int_ans = 0 - int.Parse(operands[i].Replace("n", ""));
                                        }
                                        else
                                        {
                                            int_ans = int.Parse(operands[i]);
                                        }
                                    }
                                    else
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            int_ans /= 0 - int.Parse(operands[i]);
                                        }
                                        else
                                        {
                                            int_ans /= int.Parse(operands[i]);
                                        }
                                    }
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            decimal_ans = (decimal)(0 - double.Parse(operands[i]));
                                        }
                                        else
                                        {
                                            decimal_ans = (decimal)double.Parse(operands[i]);
                                        }
                                    }
                                    else
                                    {
                                        if (operands[i].Contains("n"))
                                        {
                                            decimal_ans /= (decimal)(0 - double.Parse(operands[i]));
                                        }
                                        else
                                        {
                                            decimal_ans /= (decimal)(double.Parse(operands[i]));
                                        }
                                    }
                                }
                            }
                            if (is_int)
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{int_ans}");
                                }
                                catch
                                {
                                    Console.WriteLine("群消息发送失败");
                                }
                            }
                            else
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{decimal_ans}");
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
                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, "计算失败");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    else if (formula.Contains("^"))
                    {
                        operands = formula.Split("^");
                        try
                        {
                            foreach (string number in operands)
                            {
                                if (!number.Contains("n") && double.Parse(number) % 1 != 0)
                                {
                                    is_int = false;
                                    break;
                                }
                                else if (operands[0].Contains("n"))
                                {
                                    negative = true;
                                }
                                else if (operands[1].Contains("n"))
                                {
                                    negative = true;
                                    is_int = false;
                                    break;
                                }
                            }
                            for (int i = 0; i < operands.Length; i++)
                            {
                                if (is_int)
                                {
                                    if (i == 0)
                                    {
                                        if (!negative)
                                        {
                                            int_ans = int.Parse(operands[i]);
                                        }
                                        else
                                        {
                                            int_ans = 0 - int.Parse(operands[i].Replace("n", ""));
                                        }
                                    }
                                    else
                                    {
                                        int_ans = (long)Math.Pow(int_ans, int.Parse(operands[i]));
                                    }
                                }
                                else if (negative && !is_int)
                                {
                                    if (i == 0)
                                    {
                                        double_ans = double.Parse(operands[i].Replace("n", ""));
                                    }
                                    else
                                    {
                                        double_ans = Math.Pow(double_ans, 0 - int.Parse(operands[i].Replace("n", "")));
                                    }
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        decimal_ans = (decimal)double.Parse(operands[i]);
                                    }
                                    else
                                    {
                                        decimal_ans = (decimal)Math.Pow((double)decimal_ans, double.Parse(operands[i]));
                                    }
                                }
                            }
                            if (is_int)
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{int_ans}");
                                }
                                catch
                                {
                                    Console.WriteLine("群消息发送失败");
                                }
                            }
                            else if (negative && !is_int)
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{double_ans}");
                                }
                                catch
                                {
                                    Console.WriteLine("群消息发送失败");
                                }
                            }
                            else
                            {
                                try
                                {
                                    await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{decimal_ans}");
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
                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, "计算失败");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    // 三角函数运算
                    else if (formula.StartsWith("sin"))
                    {
                        try
                        {
                            if (formula.Split("sin")[1].Contains("n"))
                            {
                                operand = 0 - double.Parse(formula.Split("sin")[1].Replace("n",""));
                            }
                            else
                            {
                                operand = double.Parse(formula.Split("sin")[1]);
                            }
                            decimal_ans = (decimal)Math.Sin(operand * Math.PI / 180);
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{decimal_ans}");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                        catch
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, "计算失败");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    else if (formula.StartsWith("cos"))
                    {
                        try
                        {
                            if (formula.Split("cos")[1].Contains("n"))
                            {
                                operand = 0 - double.Parse(formula.Split("cos")[1].Replace("n", ""));
                            }
                            else
                            {
                                operand = double.Parse(formula.Split("cos")[1]);
                            }
                            decimal_ans = (decimal)Math.Cos(operand * Math.PI / 180);
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{decimal_ans}");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                        catch
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, "计算失败");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    else if (formula.StartsWith("tan"))
                    {
                        try
                        {
                            if (formula.Split("tan")[1].Contains("n"))
                            {
                                operand = 0 - double.Parse(formula.Split("tan")[1].Replace("n", ""));
                            }
                            else
                            {
                                operand = double.Parse(formula.Split("tan")[1]);
                            }
                            decimal_ans = (decimal)Math.Tan(operand * Math.PI / 180);
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{decimal_ans}");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                        catch
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, "计算失败");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    else if (formula.StartsWith("cot"))
                    {
                        try
                        {
                            if (formula.Split("cot")[1].Contains("n"))
                            {
                                operand = 0 - double.Parse(formula.Split("cot")[1].Replace("n", ""));
                            }
                            else
                            {
                                operand = double.Parse(formula.Split("cot")[1]);
                            }
                            decimal_ans = 1 / (decimal)Math.Tan(operand * Math.PI / 180);
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, $"这个算式的答案是：{decimal_ans}");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                        catch
                        {
                            try
                            {
                                await MessageManager.SendGroupMessageAsync(receiver.GroupId, "计算失败");
                            }
                            catch
                            {
                                Console.WriteLine("群消息发送失败");
                            }
                        }
                    }
                    // 如果不是算式
                    else
                    {
                        try
                        {
                            await MessageManager.SendGroupMessageAsync(receiver.GroupId, "参数错误");
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
