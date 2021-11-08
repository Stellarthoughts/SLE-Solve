using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Lab5
{
    public class Reader
    {
        public static T[,] MatrixFromString<T>(string text)
        {
            text = NormalizeWhiteSpaceForLoop(text);
            text = text.Replace("\r", "\n");
            text = text.Replace("\t", " ");
            text = text.Trim('\n');
            text = text.Trim(' ');
            string[] split = text.Split("\n");
            List<string> digits = new List<string>();
            foreach (string line in split)
            {
                string[] lineSplit = line.Split(" ");
                foreach(string word in lineSplit)
                    digits.Add(word);
            }

            int numbersInLine = digits.Count / split.Length;
            T[,] read = new T[split.Length, numbersInLine];

            for (int i = 0; i < split.Length; i++)
                for(int j = 0; j < numbersInLine; j++)
                    read[i, j] = (T) (object) Double.Parse(digits[i * numbersInLine + j], CultureInfo.InvariantCulture); 
            
            return read;
        }

        // https://stackoverflow.com/questions/6442421/c-sharp-fastest-way-to-remove-extra-white-spaces
        public static string NormalizeWhiteSpaceForLoop(string input)
        {
            int len = input.Length,
                index = 0,
                i = 0;
            var src = input.ToCharArray();
            bool skip = false;
            char ch;
            for (; i < len; i++)
            {
                ch = src[i];
                switch (ch)
                {
                    case '\u0020':
                    case '\u00A0':
                    case '\u1680':
                    case '\u2000':
                    case '\u2001':
                    case '\u2002':
                    case '\u2003':
                    case '\u2004':
                    case '\u2005':
                    case '\u2006':
                    case '\u2007':
                    case '\u2008':
                    case '\u2009':
                    case '\u200A':
                    case '\u202F':
                    case '\u205F':
                    case '\u3000':
                    case '\u2028':
                    case '\u2029':
                    case '\u0009':
                    case '\u000A':
                    case '\u000B':
                    case '\u000C':
                    case '\u000D':
                    case '\u0085':
                        if (skip) continue;
                        src[index++] = ch;
                        skip = true;
                        continue;
                    default:
                        skip = false;
                        src[index++] = ch;
                        continue;
                }
            }

            return new string(src, 0, index);
        }
    }
}
