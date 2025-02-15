using Age.Core.Collections;
using Age.Elements;
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
        set => this.text.CursorPosition = value;
    }

    public string? Value
    {
        get => this.text.Buffer.ToString();
        set => this.text.Buffer.Set(value);
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

        this.text.Buffer.Changed += this.OnTextBufferChanged;
    }

    private void ApplyHistory(in HistoryEntry entry)
    {
        this.text.Buffer.Set(entry.Text);

        this.CursorPosition = entry.CursorPosition;
        this.text.Selection = entry.Selection;
    }

    private HistoryEntry CreateHistory() =>
        new()
        {
            Text           = this.text.Buffer.ToString(),
            CursorPosition = this.CursorPosition,
            Selection      = this.text.Selection
        };

    private uint GetTrimmedCursorPosition() =>
        this.text.Buffer.IsEmpty ? 0 : uint.Min(this.CursorPosition, (uint)this.text.Buffer.Length - 1);

    private void OnBlur(in MouseEvent mouseEvent)
    {
        this.text.ClearSelection();
        this.text.HideCaret();

        if (!this.text.Buffer.Equals(this.previousText))
        {
            this.Changed?.Invoke();
        }
    }

    private void OnFocused(in MouseEvent mouseEvent)
    {
        this.text.ShowCaret();

        this.previousText = this.text.Buffer.ToString();
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

        if (this.text.Buffer.IsEmpty || this.CursorPosition == this.text.Buffer.Length)
        {
            this.text.Buffer.Append([character]);
        }
        else
        {
            this.text.Buffer.Insert([character], (int)this.CursorPosition);
        }

        this.CursorPosition++;
    }

    private void OnKeyDown(in KeyEvent keyEvent)
    {
        switch (keyEvent.Key)
        {
            case Key.Delete:
                if (this.text.Buffer?.Length > 0)
                {
                    this.SaveHistory();

                    if (keyEvent.Modifiers.HasFlag(KeyStates.Shift))
                    {
                        var currentLine = this.text.GetCharacterLine(this.GetTrimmedCursorPosition());

                        if (this.Tree is RenderTree tree && this.text.Cut(new(currentLine.Start, currentLine.End + 1)) is string text)
                        {
                            tree.Window.SetClipboardData(text);
                        }
                    }
                    else if (this.text.Selection != null)
                    {
                        this.DeleteSelected();
                    }
                    else
                    {
                        if (this.CursorPosition < this.text.Buffer.Length)
                        {
                            this.text.Delete(new(this.CursorPosition, this.CursorPosition + 1));
                        }
                    }
                }

                break;

            case Key.Backspace:
                if (this.CursorPosition > 0 && this.text.Buffer?.Length > 0)
                {
                    this.SaveHistory();

                    if (this.text.Selection != null)
                    {
                        this.DeleteSelected();
                    }
                    else
                    {
                        if (this.CursorPosition == this.text.Buffer.Length)
                        {
                            this.text.Buffer.Remove(this.text.Buffer.Length - 1);
                        }
                        else
                        {
                            this.text.Buffer.Remove((int)this.CursorPosition - 1);
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

                    if (this.CursorPosition == this.text.Buffer.Length)
                    {
                        this.text.Buffer.Append(['\n']);
                    }
                    else
                    {
                        this.text.Buffer.Insert(['\n'], (int)this.CursorPosition);
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
                if (this.CursorPosition < this.text.Buffer?.Length)
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
                if (this.Multiline && this.text.Buffer != null && this.CursorPosition > 0)
                {
                    this.SaveHistory();

                    var cursor       = this.GetTrimmedCursorPosition();
                    var currentLine  = this.text.GetCharacterLine(cursor);
                    var previousLine = this.text.GetCharacterPreviousLine(cursor);
                    var position     = currentLine.Start;

                    if (previousLine.HasValue && !(this.CursorPosition == this.text.Buffer.Length && this.text.Buffer[^1] == '\n'))
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
                if (this.Multiline && this.CursorPosition < this.text.Buffer?.Length)
                {
                    this.SaveHistory();

                    var cursor      = this.GetTrimmedCursorPosition();
                    var currentLine = this.text.GetCharacterLine(cursor);
                    var nextLine    = this.text.GetCharacterNextLine(cursor);
                    var position    = currentLine.End + 1;

                    if (nextLine.HasValue)
                    {
                        var column  = this.CursorPosition - currentLine.Start;

                        position = column < nextLine.Value.Length
                            ? nextLine.Value.Start + column
                            : nextLine.Value.End + 1 == this.text.Buffer.Length
                                ? (uint)this.text.Buffer.Length
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
                if (this.text.Buffer?.Length > 0)
                {
                    this.SaveHistory();

                    var position = (!this.Multiline || keyEvent.Modifiers.HasFlag(KeyStates.Control))
                        ? 0u
                        : this.text.GetCharacterLine(this.GetTrimmedCursorPosition()).Start;

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
                if (this.text.Buffer?.Length > 0)
                {
                    this.SaveHistory();

                    uint position;

                    if (!this.Multiline || keyEvent.Modifiers.HasFlag(KeyStates.Control))
                    {
                        position = (uint)this.text.Buffer.Length;
                    }
                    else
                    {
                        var currentLine = this.text.GetCharacterLine(this.GetTrimmedCursorPosition());

                        position = currentLine.End == this.text.Buffer.Length - 1 && this.text.Buffer[^1] != '\n' ? currentLine.End + 1 : currentLine.End;
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
                if (keyEvent.Modifiers.HasFlag(KeyStates.Control) && this.text.Buffer != null)
                {
                    this.text.Selection = new(0, (uint)this.text.Buffer.Length);

                    this.CursorPosition = (uint)this.text.Buffer.Length;
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
                            if (this.text.Buffer.IsEmpty || this.CursorPosition == this.text.Buffer.Length)
                            {
                                this.text.Buffer.Append(text);
                            }
                            else
                            {
                                this.text.Buffer.Insert(text, (int)this.CursorPosition);
                            }

                            this.CursorPosition += (uint)text.Length;
                        }
                    }
                }

                break;

            case Key.X:
                {
                    if (keyEvent.Modifiers.HasFlag(KeyStates.Control) && this.Tree is RenderTree renderTree)
                    {
                        this.SaveHistory();

                        if (this.text.CutSelected() is string text)
                        {
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
        if (this.text.Buffer != null && !mouseEvent.Indirect)
        {
            this.text.Layout.SetCaret(mouseEvent.X, mouseEvent.Y);
        }
    }

    private void OnTextBufferChanged()
    {
        if (this.IsFocused)
        {
            this.CursorPosition = (uint)this.text.Buffer.Length;
        }

        this.Changed?.Invoke();
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
