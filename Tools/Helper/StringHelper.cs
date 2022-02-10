using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SIX.SCS.Tools.Helper;

public static class StringHelper
{
    /// <summary>
    ///     Extended version of the string.Contains() method,
    ///     accepting a [CompareInfo] object to perform different kind of comparisons
    ///     and a [CultureInfo] object instance to apply them to the given culture casing rules.
    /// </summary>
    public static bool ContainsIgnoreCase(this string source, string value)
    {
        var culture = CultureInfo.CurrentCulture;
        const CompareOptions options = CompareOptions.IgnoreCase;

        return culture.CompareInfo.IndexOf(source, value, options) >= 0;
    }

    /// <summary>
    ///     Crops the string if the given size is smaller than the string length
    /// </summary>
    /// <param name="aString">the given string to be cropped</param>
    /// <param name="size">the maximum size of the string</param>
    /// <returns>the cropped string</returns>
    public static string Crop(this string aString, int size)
    {
        return aString.Length > size ? aString.Remove(size) : aString;
    }

    /// <summary>
    ///     Create a string list for a given string which includes line separators
    /// </summary>
    /// <param name="aString">the given string which shall be split</param>
    /// <returns>the list of strings</returns>
    public static List<string> SplitByLineSeparator(this string aString)
    {
        if (aString == null)
        {
            throw new ArgumentException($"{nameof(aString)} must not be null");
        }

        return aString.Replace("\r", "").Split('\n').ToList();
    }
}