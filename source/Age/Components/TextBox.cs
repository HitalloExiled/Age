using System.Text;
using Age.Elements;
using Age.Themes;

using Key       = Age.Platforms.Display.Key;
using KeyStates = Age.Platforms.Display.KeyStates;

namespace Age.Components;

public class TextBox : Element
{
    private readonly TextNode text = new();

    public override string NodeName { get; } = nameof(TextBox);

    public TextBox()
    {
        this.IsFocusable = true;

        this.States = Theme.Current.TextBox.Outlined;

        this.AppendChild(this.text);

        this.Clicked += this.OnClicked;
        this.Blured  += this.OnBlurer;
        this.KeyDown += this.OnKeyDown;
    }

    private void OnKeyDown(in KeyEvent keyEvent)
    {
        switch (keyEvent.Key)
        {
            case Key.Delete:
                this.text.DeleteSelected();
                break;

            case Key.Enter:
                this.text.Value += "\n ";
                this.text.CursorPosition = this.text.Value.Length;
                break;

            case Key.Backspace:
                if (this.text.Value != null)
                {
                    this.text.Value = this.text.Value[..^1];
                    this.text.CursorPosition--;
                }
                break;

            case Key.Left:
                if (this.text.CursorPosition > 1)
                {
                    this.text.CursorPosition--;
                }

                break;

            case Key.Right:
                if (this.text.CursorPosition < this.text.Value?.Length)
                {
                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {

                    }

                    this.text.CursorPosition++;
                }

                break;

            default:
                {
                    var character = (char)keyEvent.Key;

                    if (!char.IsControl(character) && char.IsAscii(character))
                    {
                        if (!keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                        {
                            character = char.ToLower(character);
                        }

                        if (this.text.Value == null || this.text.CursorPosition == this.text.Value.Length)
                        {
                            this.text.Value += character;
                        }
                        else
                        {
                            Span<char> buffer = stackalloc char[this.text.Value.Length + 1];

                            this.text.Value.AsSpan()[..this.text.CursorPosition].CopyTo(buffer);

                            buffer[this.text.CursorPosition] = character;

                            this.text.Value.AsSpan()[this.text.CursorPosition..].CopyTo(buffer[(this.text.CursorPosition + 1)..]);

                            this.text.Value = new(buffer);
                        }

                        this.text.CursorPosition++;
                    }
                }

                break;
        }
    }

    private void OnBlurer(in MouseEvent mouseEvent)
    {
        if (this.text.Value == null)
        {
            this.text.ClearSelection();
        }
    }

    private void OnClicked(in MouseEvent mouseEvent)
    {
        if (this.text.Value == null)
        {
            this.text.CursorPosition = 0;
        }
    }
}
