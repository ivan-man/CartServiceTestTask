using System;
using System.Text.RegularExpressions;

namespace Common.Extensions
{
    public static class NamingStyleExtension
    {
        /// <summary>
        /// Convert CamelCase string into space_cases.
        /// </summary>
        /// <param name="input">Origin string.</param>
        /// <returns>space_cases string.</returns>
        public static string ToSnakeCaseFromCamel(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return Regex.Replace(input, "(\\B[A-Z])", "_$1").ToLower();
        }
    }
}
