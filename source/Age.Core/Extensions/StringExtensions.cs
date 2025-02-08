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

    public static void GetTextInfo(this ReadOnlySpan<char> value, out int nonWhitespaceCount, out int linesCount)
    {
        nonWhitespaceCount = 0;
        linesCount         = 1;

        for (var i = 0; i < value.Length; i++)
        {
            if (!char.IsWhiteSpace(value[i]))
            {
                nonWhitespaceCount++;
            }
            else if (value[i] == '\n')
            {
                linesCount++;
            }
        }
    }
}
