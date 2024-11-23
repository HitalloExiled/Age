namespace Age.Core.Extensions;

public static class StringExtensions
{
    public static int CountNonWhitespaceCharacters(this ReadOnlySpan<char> value)
    {
        var count = 0;

        for (var i = 0; i < value.Length; i++)
        {
            if (!char.IsWhiteSpace(value[i]))
            {
                count++;
            }
        }

        return count;
    }
}
