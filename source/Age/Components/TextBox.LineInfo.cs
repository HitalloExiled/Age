namespace Age.Components;

public partial class TextBox
{
    public ref struct LineInfo
    {
        private readonly ReadOnlySpan<char> text;

        public uint Start;
        public uint End;

        public readonly bool               IsEmpty => this.text.IsEmpty;
        public readonly uint               Length  => !this.text.IsEmpty ? this.End + 1 - this.Start : default;
        public readonly ReadOnlySpan<char> Text    => !this.text.IsEmpty ? this.text[(int)this.Start..(int)(this.End + 1)] : default;

        private LineInfo(ReadOnlySpan<char> text) =>
            this.text = text;

        public LineInfo(ReadOnlySpan<char> text, uint cursor) : this(text)
        {
            if (text.Length > 0)
            {
                if (cursor > text.Length - 1)
                {
                    cursor = (uint)(text.Length - 1);
                }

                this.Start = GetCursorStartOfLine(text, cursor);
                this.End   = GetCursorEndOfLine(text, cursor);
            }
        }

        public readonly LineInfo PreviousLine()
        {
            if (this.Start > 0)
            {
                var start = this.Start - 1;

                return new(this.text)
                {
                    Start = GetCursorStartOfLine(this.text, start),
                    End   = start,
                };
            }
            else
            {
                return default;
            }
        }

        public readonly LineInfo NextLine()
        {
            if (this.End < this.text.Length - 1)
            {
                var end = this.End + 1;

                return new(this.text)
                {
                    Start = end,
                    End   = GetCursorEndOfLine(this.text, end),
                };
            }
            else
            {
                return default;
            }
        }

        private static uint GetCursorStartOfLine(ReadOnlySpan<char> text, uint position)
        {
            var current = (int)position;

            if (text[current] == '\n')
            {
                current--;
            }

            while (current > -1)
            {
                if (text[current] == '\n')
                {
                    return (uint)current + 1;
                }

                current--;
            }

            return 0;
        }

        private static uint GetCursorEndOfLine(ReadOnlySpan<char> text, uint position)
        {
            var current = (int)position;

            while (current < text.Length)
            {
                if (text[current] == '\n')
                {
                    return (uint)current;
                }

                current++;
            }

            return (uint)(text.Length - 1);
        }
    }
}
