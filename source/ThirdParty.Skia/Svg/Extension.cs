using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ThirdParty.Skia.Svg;

public static partial class Extension
{
    private static readonly Regex groupPattern = CreateGroupPattern();

    [GeneratedRegex(@"(?<!\\)\((?!\?:)[^[\]]*?(?<!\\)\)")]
    private static partial Regex CreateGroupPattern();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T? GetValueRefOrAddDefault<T, K>(this Dictionary<K, T> source, K key, out bool exists) where K : notnull =>
        ref CollectionsMarshal.GetValueRefOrAddDefault(source, key, out exists);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetValueRefOrNullRef<T, K>(this Dictionary<K, T> source, K key) where K : notnull =>
        ref CollectionsMarshal.GetValueRefOrNullRef(source, key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Regex.ValueMatchEnumerator EnumerateRegexGroups(this ReadOnlySpan<char> source) =>
        groupPattern.EnumerateMatches(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> Slice(this ReadOnlySpan<char> source, in ValueMatch match) =>
        source.Slice(match.Index, match.Length);
}
