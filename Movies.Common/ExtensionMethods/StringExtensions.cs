using System;
using System.Reflection.Metadata.Ecma335;

namespace Movies.Common.ExtensionMethods
{
    public static class StringExtensions
    {
        /// <summary>
        /// Trims all.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>string with out spaces.</returns>
        public static string TrimAll(this string str)
        {
            var strTrimmed = str.Trim();
            var strFullTrimmed = strTrimmed.Replace(" ", "");
            return strFullTrimmed;
        }

        /// <summary>
        /// Generates a Random letter from a range of ascci numbers.
        /// </summary>
        /// <param name="str">The String</param>
        /// <param name="from">lower range</param>
        /// <param name="to">upper range</param>
        /// <returns>letter in string</returns>
        public static string RandomLetter(this string str, int from, int to)
        {
            var random = new Random();
            int number = random.Next(from, to);
            return ((char)number).ToString();
        }
    }
}
