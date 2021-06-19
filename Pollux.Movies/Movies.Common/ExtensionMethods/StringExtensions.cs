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
    }
}
