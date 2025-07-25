using Age.Elements;

namespace Age.Components;

public partial class TextBox
{
    private record struct HistoryEntry
    {
        public string?        Text;
        public uint           CursorPosition;
        public TextSelection? Selection;
    }
}
