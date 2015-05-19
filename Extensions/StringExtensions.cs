#region
using System.Text.RegularExpressions;

#endregion

namespace EnlaceFiscal.Extensions {
    public static class StringExtensions {
        public static string RemoveTrailingZeroes(this string str) {
            return Regex.Replace(str, @"\.0+$", "");
        }
    }
}