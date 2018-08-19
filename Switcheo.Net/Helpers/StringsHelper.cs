using System;

namespace Switcheo.Net.Helpers
{
    public static class StringsHelper
    {
        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return Char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }

        public static string UpperFirst(this string str)
        {
            return char.ToUpper(str[0]) +
                ((str.Length > 1) ? str.Substring(1).ToLower() : string.Empty);
        }
    }
}
