using System;
using System.Diagnostics;

namespace Normalizer.Utils
{
    internal static class ArgumentUtils
    {
        public static string AssertThrowNonEmpty(this string value, string? name = null)
        {
            var isEmpty = string.IsNullOrWhiteSpace(value);
            Debug.Assert(!isEmpty, "Not expected an empty or whitespace string.");
            if (isEmpty)
            {
                var argName = name ?? "arg";
                throw new ArgumentException($"{argName} cannot be empty or whitespace.");
            }

            return value;
        }
    }
}
