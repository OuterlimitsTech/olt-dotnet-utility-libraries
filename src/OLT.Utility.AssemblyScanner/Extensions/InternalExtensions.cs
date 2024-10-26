using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace OLT.Utility;

internal static class InternalExtensions
{
    public static IEnumerable<TResult> SelectDistinct<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) 
        => SelectDistinct(source, selector, null);

    public static IEnumerable<TResult> SelectDistinct<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, IEqualityComparer<TResult>? distinctComparer) 
        =>  source.Select(selector).Distinct(distinctComparer);

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static bool StartsWithAny([AllowNull, NotNullWhen(true)] this string source, params string[] values)
           => StartsWithAny(source, false, values);

    public static bool StartsWithAny([AllowNull, NotNullWhen(true)] this string source, bool ignoreCase, params string[] values)
    {
        if (!source.IsNullOrEmpty() && values?.Length > 0)
        {
            foreach (var value in values.Where(v => !v.IsNullOrEmpty()))
            {
                if (source.StartsWith(value, ignoreCase, null))
                    return true;
            }
        }

        return false;
    }


    public static bool Equals(this string? source, string value, bool ignoreCase)
    {
        return CultureInfo.CurrentCulture.CompareInfo.Compare(source, value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None) == 0;
    }

    public static bool EqualsAny([AllowNull, NotNullWhen(true)] this string source, bool ignoreCase, params string[] values)
    {
        if (source is not null && values?.Length > 0)
        {
            foreach (var value in values.Where(v => v is not null))
            {
                return source.Equals(value, ignoreCase);
            }
        }

        return false;
    }


#if NETSTANDARD2_0

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        var keys = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (keys.Contains(keySelector(element))) continue;
            keys.Add(keySelector(element));
            yield return element;
        }
    }

#endif
}
