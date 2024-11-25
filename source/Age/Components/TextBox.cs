using Age.Elements;
using Age.Themes;

using Key       = Age.Platforms.Display.Key;
using KeyStates = Age.Platforms.Display.KeyStates;

namespace Age.Components;

public class TextBox : Element
{
    private readonly TextNode text = new();

    public override string NodeName { get; } = nameof(TextBox);

    public bool Multiline { get; set; }

    public TextBox()
    {
        this.IsFocusable = true;

        this.States = Theme.Current.TextBox.Outlined;

        this.AppendChild(this.text);

        this.Blured  += this.OnBlurer;
        this.Focused += this.OnFocused;
        this.Input   += this.OnInput;
        this.KeyDown += this.OnKeyDown;
    }

    private void OnBlurer(in MouseEvent mouseEvent) =>
        this.text.HideCaret();

    private void OnFocused(in MouseEvent mouseEvent) =>
        this.text.ShowCaret();

    private void OnInput(char character)
    {
        if (character == '\b')
        {
            if (this.text.Value?.Length > 0)
            {
                if (this.text.Selection != null)
                {
                    this.text.DeleteSelected();
                }
                else
                {
                    if (this.text.CursorPosition == this.text.Value.Length)
                    {
                        this.text.Value = this.text.Value[..^1];
                    }
                    else if (this.text.CursorPosition > 0)
                    {
                        var start = this.text.Value.AsSpan(..((int)this.text.CursorPosition - 1));

                        var end = this.text.Value.AsSpan((int)this.text.CursorPosition..);

                        this.text.Value = string.Concat(start, end);
                    }

                    this.text.CursorPosition--;
                }
            }
        }
        else
        {
            if (this.text.Selection != null)
            {
                this.text.DeleteSelected();
            }

            if (this.text.Value?.Length is 0 or null || this.text.CursorPosition == this.text.Value.Length)
            {
                this.text.Value += character;
            }
            else
            {
                var start = this.text.Value.AsSpan(..(int)this.text.CursorPosition);

                Span<char> middle = [character];

                var end = this.text.Value.AsSpan((int)this.text.CursorPosition..);

                this.text.Value = string.Concat(start, middle, end);
            }

            this.text.CursorPosition++;
        }
    }

    private void OnKeyDown(in KeyEvent keyEvent)
    {
        switch (keyEvent.Key)
        {
            case Key.Delete:
                if (this.text.Value?.Length > 0)
                {
                    if (this.text.Selection != null)
                    {
                        this.text.DeleteSelected();
                    }
                    else
                    {
                        if (this.text.CursorPosition < this.text.Value.Length)
                        {
                            var start = this.text.Value.AsSpan(..(int)this.text.CursorPosition);

                            var end = this.text.Value.AsSpan(((int)this.text.CursorPosition + 1)..);

                            this.text.Value = string.Concat(start, end);
                        }
                    }
                }
                break;

            case Key.Enter:
                if (this.Multiline)
                {
                    this.text.Value += "\n\r";
                    this.text.CursorPosition += 2;
                }
                else
                {
                    this.Blur();
                }

                break;

            case Key.Left:
                if (this.text.CursorPosition > 0)
                {
                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(this.text.CursorPosition - 1) ?? new(this.text.CursorPosition, this.text.CursorPosition - 1);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }

                    this.text.CursorPosition--;
                }

                break;

            case Key.Right:
                if (this.text.CursorPosition < this.text.Value?.Length)
                {
                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(this.text.CursorPosition + 1) ?? new(this.text.CursorPosition, this.text.CursorPosition + 1);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }

                    this.text.CursorPosition++;
                }

                break;

            case Key.Home:
                if (this.text.Value?.Length > 0)
                {
                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(0) ?? new(this.text.CursorPosition, 0);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }

                    this.text.CursorPosition = 0;
                }

                break;

            case Key.End:
                if (this.text.Value?.Length > 0)
                {
                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd((uint)this.text.Value.Length) ?? new(this.text.CursorPosition, (uint)this.text.Value.Length);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }

                    this.text.CursorPosition = (uint)this.text.Value.Length;
                }

                break;

            default:
                break;
        }
    }
}
