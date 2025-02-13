using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Elements;
using Age.Scene;
using Age.Themes;

using Key = Age.Platforms.Display.Key;
using KeyStates = Age.Platforms.Display.KeyStates;

namespace Age.Components;

public partial class TextBox : Element
{
    public event Action? Changed;

    private readonly DropoutStack<HistoryEntry> redo = new(50);
    private readonly TextNode                   text = new();
    private readonly DropoutStack<HistoryEntry> undo = new(50);

    private string? previousText;

    private uint TrimmedCursorPosition => string.IsNullOrEmpty(this.text.Value) ? 0 : uint.Min(this.CursorPosition, (uint)this.text.Value.Length - 1);

    public override string NodeName { get; } = nameof(TextBox);

    public uint CursorPosition
    {
        get => this.text.CursorPosition;
        set => this.text.CursorPosition = value;
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

        this.Blured      += this.OnBlur;
        this.Focused     += this.OnFocused;
        this.Input       += this.OnInput;
        this.KeyDown     += this.OnKeyDown;
        this.MouseDown   += this.OnMouseDown;
        this.Activated   += this.text.InvokeActivate;
        this.Deactivated += this.text.InvokeDeactivate;
    }

    private void ApplyHistory(in HistoryEntry entry)
    {
        this.text.Value     = entry.Text;
        this.CursorPosition = entry.CursorPosition;
        this.text.Selection = entry.Selection;
    }

    private HistoryEntry CreateHistory() =>
        new()
        {
            Text           = this.text.Value,
            CursorPosition = this.CursorPosition,
            Selection      = this.text.Selection
        };

    private void OnBlur(in MouseEvent mouseEvent)
    {
        this.text.ClearSelection();
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
                if (this.CursorPosition > 0 && this.text.Value?.Length > 0)
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
                        else
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

                    this.DeleteSelected();

                    if (this.CursorPosition == this.text.Value?.Length)
                    {
                        this.text.Value += '\n';
                    }
                    else
                    {
                        var start = this.text.Value.AsSpan(..(int)this.CursorPosition);

                        var end = this.text.Value.AsSpan((int)this.CursorPosition..);

                        this.text.Value = string.Concat(start, ['\n'], end);
                    }

                    this.CursorPosition++;
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

            case Key.Up:
                if (this.Multiline && this.text.Value != null && this.CursorPosition > 0)
                {
                    this.SaveHistory();

                    var cursor       = this.TrimmedCursorPosition;
                    var currentLine  = this.text.GetCharacterLine(cursor);
                    var previousLine = this.text.GetCharacterPreviousLine(cursor);
                    var position     = currentLine.Start;

                    if (previousLine.HasValue && !(this.CursorPosition == this.text.Value.Length && this.text.Value[^1] == '\n'))
                    {
                        var column = this.CursorPosition - currentLine.Start;

                        position = column < previousLine.Value.Length ? previousLine.Value.Start + column : previousLine.Value.End;
                    }

                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(position) ?? new(this.CursorPosition, position);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }

                    this.CursorPosition = position;
                }

                break;

            case Key.Down:
                if (this.Multiline && this.CursorPosition < this.text.Value?.Length)
                {
                    this.SaveHistory();

                    var cursor      = this.TrimmedCursorPosition;
                    var currentLine = this.text.GetCharacterLine(cursor);
                    var nextLine    = this.text.GetCharacterNextLine(cursor);
                    var position    = currentLine.End + 1;

                    if (nextLine.HasValue)
                    {
                        var column  = this.CursorPosition - currentLine.Start;

                        position = column < nextLine.Value.Length
                            ? nextLine.Value.Start + column
                            : nextLine.Value.End == this.text.Value.Length - 1
                                ? (uint)this.text.Value.Length
                                : nextLine.Value.End;
                    }

                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(position) ?? new(this.CursorPosition, position);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }

                    this.CursorPosition = position;
                }

                break;

            case Key.Home:
                if (this.text.Value?.Length > 0)
                {
                    this.SaveHistory();

                    var position = (!this.Multiline || keyEvent.Modifiers.HasFlag(KeyStates.Control))
                        ? 0u
                        : this.text.GetCharacterLine(this.TrimmedCursorPosition).Start;

                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(position) ?? new(this.CursorPosition, position);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }

                    this.CursorPosition = position;
                }

                break;

            case Key.End:
                if (this.text.Value?.Length > 0)
                {
                    this.SaveHistory();

                    uint position;

                    if (!this.Multiline || keyEvent.Modifiers.HasFlag(KeyStates.Control))
                    {
                        position = (uint)this.text.Value.Length;
                    }
                    else
                    {
                        var currentLine = this.text.GetCharacterLine(this.TrimmedCursorPosition);

                        position = currentLine.End == this.text.Value.Length - 1 && this.text.Value[^1] != '\n' ? currentLine.End + 1 : currentLine.End;
                    }

                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(position) ?? new(this.CursorPosition, position);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }

                    this.CursorPosition = position;
                }

                break;

            case Key.A:
                if (keyEvent.Modifiers.HasFlag(KeyStates.Control) && this.text.Value != null)
                {
                    this.text.Selection = new(0, (uint)this.text.Value.Length);

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

    private void OnMouseDown(in MouseEvent mouseEvent)
    {
        if (this.text.Value != null && !mouseEvent.Indirect)
        {
            this.text.Layout.SetCaret(mouseEvent.X, mouseEvent.Y);
        }
    }

    private void SaveHistory() =>
        this.undo.Push(this.CreateHistory());

    public void DeleteSelected() =>
        this.text.DeleteSelected();

    public void Redo()
    {
        if (this.redo.Count > 0)
        {
            this.undo.Push(this.CreateHistory());

            this.ApplyHistory(this.redo.Pop());
        }
    }

    public void Undo()
    {
        if (this.undo.Count > 0)
        {
            this.redo.Push(this.CreateHistory());

            this.ApplyHistory(this.undo.Pop());
        }
    }
}
