using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Text;

namespace KRSP.Utils
{
    public class TextUtilities
    {
        public static List<string> InstalledFonts()
        {
            List<String> familyNames = new List<string>();
            InstalledFontCollection installedFonts = new InstalledFontCollection();
            FontFamily[] families = installedFonts.Families;
            foreach (FontFamily family in families)
            {
                familyNames.Add(family.Name);
            }
            return familyNames;
        }

        public static string FixUnits(string original, string[] units)
        {
            string result = original;
            foreach (string unit in units)
            {
                //check for isolated units
                result = ReplaceEx(result, " " + unit + " ", " " + unit + " ");
                result = ReplaceEx(result, " " + unit + ".", " " + unit + ".");
                result = ReplaceEx(result, " " + unit + ",", " " + unit + ",");

                //check for units next to a numeral
                for (int i = 0; i < 10; i++)
                {
                    result = ReplaceEx(result, i + unit + " ", i + unit + " ");
                    result = ReplaceEx(result, i + unit + ".", i + unit + ".");
                    result = ReplaceEx(result, i + unit + ",", i + unit + ",");
                }
            }
            return result;
        }

        public static string FixMultiplier(string original)
        {
            string multiplier = "x";
            string result = original;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    result = ReplaceEx(result, i + multiplier + j, i + multiplier + j);
                }
            }
            return result;
        }

        public static string ReplaceEx(string original, string pattern, string replacement)
        {
            int count, pos0, pos1;
            count = pos0 = pos1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();

            int inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];

            while ((pos1 = upperString.IndexOf(upperPattern, pos0)) != -1)
            {
                for (int i = pos0; i < pos1; ++i) chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i) chars[count++] = replacement[i];
                pos0 = pos1 + pattern.Length;
            }

            if (pos0 == 0) { return original; }
            for (int i = pos0; i < original.Length; ++i) chars[count++] = original[i];
            return new string(chars, 0, count);
        }

        public static string SentenceCase(string original)
        {
            string result = original.ToLower();
            Regex r = new Regex(@"(^[a-z])|\.\s+(.)", RegexOptions.ExplicitCapture);
            return r.Replace(result, s => s.Value.ToUpper());
        }

        public static string[] SplitByString(string original, string delim)
        {

            int offset = 0;
            int index = 0;
            int[] offsets = new int[original.Length + 1];

            while (index < original.Length)
            {
                int indexOf = original.IndexOf(delim, index);
                if (indexOf != 1)
                {
                    offsets[offset++] = indexOf;
                    index = indexOf + delim.Length;
                }
                else
                {
                    index = original.Length;
                }
            }

            string[] result = new string[offset + 1];
            if (offset == 0)
            {
                result[0] = original;
            }
            else
            {
                offset--;
                result[0] = original.Substring(0, offsets[0]);
                for (int i = 0; i < offset; i++)
                {
                    result[i + 1] = original.Substring(offsets[i] + delim.Length, offsets[i] - delim.Length);
                }
                result[offset + 1] = original.Substring(offsets[offset] + delim.Length);
            }

            return result;
        }
    }
}
