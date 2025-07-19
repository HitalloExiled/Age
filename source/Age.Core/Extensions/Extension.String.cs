namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(scoped ReadOnlySpan<char> value)
    {
        public int CountNonWhitespaceCharacters()
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

        public void GetTextInfo(out int visibleCharacterCount, out int linesCount)
        {
            visibleCharacterCount = 0;
            linesCount = 1;

            for (var i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]) && !char.IsLowSurrogate(value[i]))
                {
                    visibleCharacterCount++;
                }
                else if (value[i] == '\n' && i + 1 < value.Length)
                {
                    linesCount++;
                }
            }
        }
    }
}
