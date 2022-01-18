using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyUrl.Framework.Converters
{
    public class Base10ToBase62
    {
        public static string Convert(long num)
        {
            const int radix = 62;
            const int bitsInLong = 64;
            const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            if (num == 0)
                return "0";

            int index = bitsInLong - 1;
            long currentNumber = Math.Abs(num);
            char[] charArray = new char[bitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new String(charArray, index + 1, bitsInLong - index - 1);
            if (num < 0)
            {
                result = "-" + result;
            }

            return result;
        }
    }
}
