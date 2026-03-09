using System;
using System.Globalization;

namespace Farms.Utilities
{
    public static class objExtend
{
    public static string toTimeHour(this int v)
    {
        var timespane = TimeSpan.FromSeconds(v);
        float hour = timespane.Hours;
        float minute = (float)timespane.Minutes / 60.0f;
        return (hour + minute).ToString(".0#") + "H";
    }

    public static string ClearDot(this string v)
    {
        string str = v.ClearWhiteSpace();
        int charEnd = 0;
        bool charShortcut = false;
        if (str.Contains('.'))
        {
            for (int i = str.Length - 1; i >= 0; --i)
            {
                if (str[i] == '.')
                {
                    break;
                }

                charEnd++;
            }
        }

        if (str.Contains("K"))
        {
            charShortcut = true;
            charEnd--;
            str = str.Replace("K", "000");
        }

        if (str.Contains("M"))
        {
            charShortcut = true;
            charEnd--;
            str = str.Replace("M", "000000");
        }

        if (str.Contains("B"))
        {
            charShortcut = true;
            charEnd--;
            str = str.Replace("B", "000000000");
        }

        if (str.Contains("T"))
        {
            charShortcut = true;
            charEnd--;
            str = str.Replace("T", "000000000000");
        }

        if (str.Contains("a"))
        {
            charShortcut = true;
            charEnd -= 2;
            str = str.Remove(str.Length - 2, 1);
            str = str.Insert(str.Length - 1, "000000000000");
            string[] alphab =
                { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t" };
            var index = System.Array.FindIndex(alphab, x => x == str.Substring(str.Length - 1, 1));
            str = str.Substring(0, str.Length - 1);
            for (int i = 0; i < index + 1; ++i)
            {
                str += "000";
            }
        }

        str = (charShortcut && charEnd > 0) ? str.Substring(0, str.Length - charEnd) : str;
        return str.Replace(".", string.Empty);
    }

    public static string ToKMBTA(this string numstr)
    {
        numstr = numstr.ClearDot();
        var num = System.Numerics.BigInteger.Parse(numstr);
        if (numstr.Length > 15)
        {
            string[] alphab =
                { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t" };
            string format = "#,##0";
            for (int i = 0; i < (numstr.Length / 3) - (numstr.Length % 3 == 0 ? 1 : 0); ++i)
            {
                format += ",";
            }

            format += "a" + alphab[((numstr.Length / 3) - (numstr.Length % 3 == 0 ? 1 : 0)) - 5];
            return num.ToString(format, CultureInfo.InvariantCulture);
        }
        else if (numstr.Length > 12)
        {
            return num.ToString("0,,,,.###T", CultureInfo.InvariantCulture);
        }
        else if (numstr.Length > 9)
        {
            return num.ToString("0,,,.##B", CultureInfo.InvariantCulture);
        }
        else if (numstr.Length > 6)
        {
            return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
        }
        else if (numstr.Length > 3)
        {
            return num.ToString("0,.#K", CultureInfo.InvariantCulture);
        }
        else
        {
            return num.ToString(CultureInfo.InvariantCulture);
        }
    }

    public static void CopyAllTo<T>(this T source, T target)
    {
        var type = typeof(T);
        foreach (var sourceProperty in type.GetProperties())
        {
            var targetProperty = type.GetProperty(sourceProperty.Name);
            targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
        }

        foreach (var sourceField in type.GetFields())
        {
            var targetField = type.GetField(sourceField.Name);
            targetField.SetValue(target, sourceField.GetValue(source));
        }
    }

    public static string RemoveRichTextDynamicTag(string input, string tag)
    {
        var index = -1;
        while (true)
        {
            index = input.IndexOf($"<{tag}=");
            //Debug.Log($"{{{index}}} - <noparse>{input}");
            if (index != -1)
            {
                var endIndex = input.Substring(index, input.Length - index).IndexOf('>');
                if (endIndex > 0)
                    input = input.Remove(index, endIndex + 1);
                continue;
            }

            input = RemoveRichTextTag(input, tag, false);
            return input;
        }
    }

    public static string RemoveRichTextTag(string input, string tag, bool isStart = true)
    {
        while (true)
        {
            var index = input.IndexOf(isStart ? $"<{tag}>" : $"</{tag}>");
            if (index != -1)
            {
                input = input.Remove(index, 2 + tag.Length + (!isStart).GetHashCode());
                continue;
            }

            if (isStart)
                input = RemoveRichTextTag(input, tag, false);
            return input;
        }
    }
}
    public static class StringUtils
    {
        public static string toString(this System.Numerics.BigInteger v)
        {
            return v.ToString();
        }

        public static double toDouble(this string v)
        {
            return double.Parse(v);
        }

        public static float toFloat(this string v)
        {
            return float.Parse(v);
        }

        public static int toInt(this string v)
        {
            int result = 0;
            int.TryParse(v, out result);
            return result;
        }

        public static long toLong(this string v)
        {
            return long.Parse(v);
        }

        public static ulong toULong(this string v)
        {
            return ulong.Parse(v);
        }

        public static System.Numerics.BigInteger toBigInt(this string v)
        {
            return System.Numerics.BigInteger.Parse(v.ClearDot());
        }

        public static string ConvertMoneyAndAddDot(this int i)
        {
            string money = i.ToString();
            int pCounterDot = (money.Length - 1) / 3;
            if (i < 0)
            {
                pCounterDot = (money.Length - 2) / 3;
            }

            var pIndex = money.Length;
            var newStrMoney = "";
            while (pCounterDot > 0)
            {
                newStrMoney = "." + money.Substring(pIndex - 3, 3) + newStrMoney;
                pCounterDot--;
                pIndex -= 3;
            }

            newStrMoney = money.Substring(0, pIndex) + newStrMoney;
            return newStrMoney;
        }

        public static string ConvertMoneyAndAddDot(long i)
        {
            string money = i.ToString();
            int pCounterDot = (money.Length - 1) / 3;
            if (i < 0)
            {
                pCounterDot = (money.Length - 2) / 3;
            }

            var pIndex = money.Length;
            var newStrMoney = "";
            while (pCounterDot > 0)
            {
                newStrMoney = "." + money.Substring(pIndex - 3, 3) + newStrMoney;
                pCounterDot--;
                pIndex -= 3;
            }

            newStrMoney = money.Substring(0, pIndex) + newStrMoney;
            return newStrMoney;
        }

        //support for 0 -> 999.999.999.999
        public static string ConvertMoneyAndAddText(long i)
        {
            var newString = string.Empty;
            long intOrigin = i;
            long intExcess = 0;
            int level = 0;
            if (intOrigin < 10000)
            {
                return ConvertMoneyAndAddDot(intOrigin);
            }

            while (intOrigin >= 1000)
            {
                intExcess = intOrigin % 1000;
                intOrigin /= 1000;
                level++;
            }

            newString += intOrigin.ToString();
            if (intExcess >= 10)
            {
                string stringExecess = ((long)(intExcess / 10)).ToString();
                newString += "." + (stringExecess.Length == 1 ? "0" + stringExecess : stringExecess);
            }

            switch (level)
            {
                case 1:
                    newString += "K";
                    break;
                case 2:
                    newString += "M";
                    break;
                case 3:
                    newString += "B";
                    break;
                default:
                    return i.ToString();
            }

            return newString;
        }

        static CultureInfo elGR;

        public static string AddDotMoney(long i)
        {
            string money = i.ToString();
            int pCounterDot = (money.Length - 1) / 3;
            if (i < 0)
            {
                pCounterDot = (money.Length - 2) / 3;
            }

            int pIndex = money.Length;
            string newStrMoney = "";
            while (pCounterDot > 0)
            {
                newStrMoney = "." + money.Substring(pIndex - 3, 3) + newStrMoney;
                pCounterDot--;
                pIndex -= 3;
            }

            newStrMoney = money.Substring(0, pIndex) + newStrMoney;
            return newStrMoney;
        }

        public static int ConvertToInt(this string v)
        {
            return int.Parse(v);
        }

        public static long ConvertToLong(this string v)
        {
            return long.Parse(v);
        }

        public static int ConvertStringDotToInt(this string v)
        {
            return v.ClearDot().ConvertToInt();
        }

        public static string ClearWhiteSpace(this string s)
        {
            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s.Substring(i, 1) == " ")
                {
                    s = s.Remove(i, 1);
                }
            }

            return s;
        }

        public static string InsertWhiteSpaceEvery(this string s, int pCount)
        {
            int index = 0;
            int count = 0;
            while (index < s.Length)
            {
                if (count == pCount)
                {
                    s = s.Insert(index, " ");
                    //index++;
                    count = 0;
                }
                else
                {
                    count++;
                }

                index++;
            }

            return s;
        }

        public static bool IsHaveWhiteSpace(this string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s.Substring(i, 1) == " ")
                {
                    return true;
                }
            }

            return false;
        }

        public static bool StartWithNumberic(this string s)
        {
            int n;
            return int.TryParse(s.Substring(0, 1), out n);
        }

        public static string FormartString1(string format, int index)
        {
            return string.Format(format, index);
        }

        public static string ReplaceTag(string input, string tag, string valueTag)
        {
            return input.Replace(tag, valueTag);
        }
    }
}