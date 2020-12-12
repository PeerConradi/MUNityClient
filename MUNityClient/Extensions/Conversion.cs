using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUNityClient.Extensions
{
    public static class Conversion
    {
        public static string ToRoman(this int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        public static string ToPathname(int[] input)
        {
            var path = "";

            for (int i=0;i<input.Length; i++)
            {
                // [1].a.ii
                if (i == 0 || i % 3 == 0)
                {
                    path += "." + (input[i] + 1).ToString();
                }
                else if (i == 1 || i % 3 == 1)
                {
                    path += "." + input[i].ToLetter();
                }
                else
                {
                    path += "." + (input[i] + 1).ToRoman().ToLower();
                }
            }
            if (path.StartsWith('.'))
                path = path.Substring(1);

            return path;
        }

        public static string ToLetter(this int number)
        {
            var letters = "abcdefghijklmnopqrstuvwxyz";
            if (number < letters.Length)
            {
                return letters[number].ToString();
            }
            throw new ArgumentOutOfRangeException("Not supported by now! Only 26 letters");
        }
    }
}
