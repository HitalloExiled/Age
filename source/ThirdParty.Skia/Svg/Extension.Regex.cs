using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ThirdParty.Skia.Svg;

internal static partial class Extension
{
    private static readonly Regex groupPattern = CreateGroupPattern();

    [GeneratedRegex(@"(?<!\\)\((?!\?:)[^[\]]*?(?<!\\)\)")]
    private static partial Regex CreateGroupPattern();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Regex.ValueMatchEnumerator EnumerateRegexGroups(this ReadOnlySpan<char> source) =>
        groupPattern.EnumerateMatches(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> Slice(this ReadOnlySpan<char> source, in ValueMatch match) =>
        source.Slice(match.Index, match.Length);
}
