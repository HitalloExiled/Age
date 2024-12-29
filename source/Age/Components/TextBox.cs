using System.ComponentModel.DataAnnotations;
using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Elements;
using Age.Extensions;
using Age.Scene;
using Age.Themes;

using Key       = Age.Platforms.Display.Key;
using KeyStates = Age.Platforms.Display.KeyStates;

namespace Age.Components;

public partial class TextBox : Element
{
    public event Action? Changed;

    private readonly DropoutStack<HistoryEntry> redo = new(50);
    private readonly TextNode                   text = new();
    private readonly DropoutStack<HistoryEntry> undo = new(50);

    private string? previousText;

    public override string NodeName { get; } = nameof(TextBox);

    public uint CursorPosition
    {
        get => this.text.CursorPosition;
        set
        {
            if (this.text.CursorPosition != value)
            {
                this.text.CursorPosition = value;

                this.AdjustScroll();
            }
        }
    }

    public string? Value
    {
        get => this.text.Value;
        set
        {
            if (value != this.text.Value)
            {
                this.text.Value = value;

                if (value != null)
                {
                    this.CursorPosition = (uint)value.Length;
                }

                this.Changed?.Invoke();
            }
        }
    }

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

    private void ApplyHistory(in HistoryEntry entry)
    {
        this.text.Value          = entry.Text;
        this.CursorPosition = entry.CursorPosition;
        this.text.Selection      = entry.Selection;
    }

    private HistoryEntry GetHistory() =>
        new()
        {
            Text           = this.text.Value,
            CursorPosition = this.CursorPosition,
            Selection      = this.text.Selection
        };

    private void AdjustScroll()
    {
        if (this.text.Value?.Length > 0)
        {
            var boundings       = this.GetBoundings();
            var cursorBoundings = this.text.GetCursorBounds();
            var paddingLeft     = 4;
            var paddingRight    = 4;

            var leftBounds  = boundings.Left + paddingLeft;
            var rightBounds = boundings.Right - paddingRight;

            if (cursorBoundings.Left < leftBounds)
            {
                var characterBounds = this.text.GetCharacterBounds(this.CursorPosition);

                var offsetX = this.Scroll.X - (leftBounds - characterBounds.Left);

                this.Scroll = this.Scroll with { X = offsetX };
            }
            else if (cursorBoundings.Right > rightBounds)
            {
                var position        = this.CursorPosition.ClampSubtract(1);
                var characterBounds = this.text.GetCharacterBounds(position);

                var offsetX = this.Scroll.X + (characterBounds.Right + cursorBoundings.Size.Width - rightBounds);

                this.Scroll = this.Scroll with { X = offsetX };
            }
        }
        else
        {
            this.Scroll = this.Scroll with { X = 0 };
        }
    }

    private void OnBlurer(in MouseEvent mouseEvent)
    {
        this.text.HideCaret();

        if (this.previousText != this.text.Value)
        {
            this.Changed?.Invoke();
        }
    }

    private void OnFocused(in MouseEvent mouseEvent)
    {
        this.text.ShowCaret();

        this.previousText = this.text.Value;
    }

    private void OnInput(char character)
    {
        if (char.IsControl(character))
        {
            return;
        }

        this.SaveHistory();

        if (this.text.Selection != null)
        {
            this.text.DeleteSelected();
        }

        if (this.text.Value?.Length is 0 or null || this.CursorPosition == this.text.Value.Length)
        {
            this.text.Value += character;
        }
        else
        {
            var start = this.text.Value.AsSpan(..(int)this.CursorPosition);

            Span<char> middle = [character];

            var end = this.text.Value.AsSpan((int)this.CursorPosition..);

            this.text.Value = string.Concat(start, middle, end);
        }

        this.CursorPosition++;
    }

    private void OnKeyDown(in KeyEvent keyEvent)
    {
        switch (keyEvent.Key)
        {
            case Key.Delete:
                if (this.text.Value?.Length > 0)
                {
                    this.SaveHistory();

                    if (this.text.Selection != null)
                    {
                        this.DeleteSelected();
                    }
                    else
                    {
                        if (this.CursorPosition < this.text.Value.Length)
                        {
                            var start = this.text.Value.AsSpan(..(int)this.CursorPosition);

                            var end = this.text.Value.AsSpan(((int)this.CursorPosition + 1)..);

                            this.text.Value = string.Concat(start, end);
                        }
                    }
                }

                break;

            case Key.Backspace:
                if (this.text.Value?.Length > 0)
                {
                    this.SaveHistory();

                    if (this.text.Selection != null)
                    {
                        this.DeleteSelected();
                    }
                    else
                    {
                        if (this.CursorPosition == this.text.Value.Length)
                        {
                            this.text.Value = this.text.Value[..^1];
                        }
                        else if (this.CursorPosition > 0)
                        {
                            var start = this.text.Value.AsSpan(..((int)this.CursorPosition - 1));

                            var end = this.text.Value.AsSpan((int)this.CursorPosition..);

                            this.text.Value = string.Concat(start, end);
                        }

                        this.CursorPosition--;
                    }
                }

                break;

            case Key.Enter:
                if (this.Multiline)
                {
                    this.SaveHistory();

                    this.text.Value += "\n\r";
                    this.CursorPosition += 2;
                }
                else
                {
                    this.Blur();
                }

                break;

            case Key.Left:
                if (this.CursorPosition > 0)
                {
                    this.SaveHistory();

                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(this.CursorPosition - 1) ?? new(this.CursorPosition, this.CursorPosition - 1);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }

                    this.CursorPosition--;
                }

                break;

            case Key.Right:
                if (this.CursorPosition < this.text.Value?.Length)
                {
                    this.SaveHistory();

                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(this.CursorPosition + 1) ?? new(this.CursorPosition, this.CursorPosition + 1);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }

                    this.CursorPosition++;
                }

                break;

            case Key.Home:
                if (this.text.Value?.Length > 0)
                {
                    this.SaveHistory();

                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(0) ?? new(this.CursorPosition, 0);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }

                    this.CursorPosition = 0;
                }

                break;

            case Key.End:
                if (this.text.Value?.Length > 0)
                {
                    this.SaveHistory();

                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd((uint)this.text.Value.Length) ?? new(this.CursorPosition, (uint)this.text.Value.Length);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }

                    this.CursorPosition = (uint)this.text.Value.Length;
                }

                break;

            case Key.V:
                {
                    if (keyEvent.Modifiers.HasFlag(KeyStates.Control) && this.Tree is RenderTree renderTree)
                    {
                        this.SaveHistory();

                        this.text.DeleteSelected();

                        if (renderTree.Window.GetClipboardData() is string text)
                        {
                            if (this.text.Value == null || this.CursorPosition == this.text.Value.Length)
                            {
                                this.text.Value += text;
                            }
                            else
                            {
                                var start = this.text.Value.AsSpan(..(int)this.CursorPosition);

                                var middle = text.AsSpan();

                                var end = this.text.Value.AsSpan((int)this.CursorPosition..);

                                this.text.Value = string.Concat(start, middle, end);
                            }

                            this.CursorPosition += (uint)text.Length;
                        }
                    }
                }

                break;

            case Key.X:
                {
                    if (keyEvent.Modifiers.HasFlag(KeyStates.Control) && this.text.Selection != null && this.Tree is RenderTree renderTree)
                    {
                        if (this.text.SelectedText is string text)
                        {
                            this.SaveHistory();

                            this.DeleteSelected();

                            renderTree.Window.SetClipboardData(text);
                        }
                    }
                }

                break;

            case Key.Z:
                if (keyEvent.Modifiers.HasFlag(KeyStates.Control))
                {
                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        this.Redo();
                    }
                    else
                    {
                        this.Undo();
                    }
                }

                break;

            default:
                break;
        }
    }

    private void SaveHistory() =>
        this.undo.Push(this.GetHistory());

    public void DeleteSelected()
    {
        this.text.DeleteSelected();
        this.AdjustScroll();
    }

    public void Redo()
    {
        if (this.redo.Count > 0)
        {
            this.undo.Push(this.GetHistory());

            this.ApplyHistory(this.redo.Pop());
        }
    }

    public void Undo()
    {
        if (this.undo.Count > 0)
        {
            this.redo.Push(this.GetHistory());

            this.ApplyHistory(this.undo.Pop());
        }
    }
}
