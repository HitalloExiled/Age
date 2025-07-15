using Age.Core.Collections;
using Age.Core.Extensions;
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
    private readonly Text                       text = new();
    private readonly DropoutStack<HistoryEntry> undo = new(50);

    private bool textBufferHasChanged;

    public override string NodeName => nameof(TextBox);

    public bool Readonly
    {
        get;
        set
        {
            if (field != value)
            {
                if (value)
                {
                    this.RemoveInputEvents();
                }
                else
                {
                    this.AddInputEvents();
                }

                field = value;
            }
        }
    }

    public uint CursorPosition
    {
        get => this.text.CursorPosition;
        set => this.text.CursorPosition = value;
    }

    public string? Value
    {
        get => this.text.Value;
        set => this.text.Value = value;
    }

    public bool Multiline { get; set; }

    public TextBox()
    {
        this.NodeFlags       = NodeFlags.Immutable;
        this.IsFocusable = true;

        this.StyleSheet = Theme.Current.TextBox.Outlined;

        this.AttachShadowTree(true);

        this.ShadowTree.AppendChild(this.text);

        this.AddInputEvents();

        this.Blured      += this.OnBlur;
        this.Focused     += this.OnFocused;
        this.MouseDown   += this.OnMouseDown;
        this.Activated   += this.text.InvokeActivate;
        this.Deactivated += this.text.InvokeDeactivate;

        this.text.Buffer.Changed += this.OnTextBufferChanged;
    }

    private void AddInputEvents()
    {
        this.Input   += this.OnInput;
        this.KeyDown += this.OnKeyDown;
    }

    private void ApplyHistory(in HistoryEntry entry)
    {
        this.text.Buffer.Set(entry.Text);

        this.text.CursorPosition = entry.CursorPosition;
        this.text.Selection = entry.Selection;
    }

    private HistoryEntry CreateHistory() =>
        new()
        {
            Text           = this.text.Buffer.ToString(),
            CursorPosition = this.text.CursorPosition,
            Selection      = this.text.Selection
        };

    private uint GetTrimmedCursorPosition() =>
        this.text.Buffer.IsEmpty ? 0 : uint.Min(this.text.CursorPosition, (uint)this.text.Buffer.Length - 1);

    private void OnBlur(in MouseEvent mouseEvent)
    {
        this.text.ClearSelection();
        this.text.HideCaret();

        if (this.textBufferHasChanged)
        {
            this.textBufferHasChanged = false;
            this.Changed?.Invoke();
        }
    }

    private void OnFocused(in MouseEvent mouseEvent)
    {
        if (!this.Readonly)
        {
            this.text.ShowCaret();
        }
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

        if (this.text.Buffer.IsEmpty || this.text.CursorPosition == this.text.Buffer.Length)
        {
            this.text.Buffer.Append([character]);
        }
        else
        {
            this.text.Buffer.Insert((int)this.text.CursorPosition, [character]);
        }

        this.text.CursorPosition++;
    }

    private void OnKeyDown(in KeyEvent keyEvent)
    {
        switch (keyEvent.Key)
        {
            case Key.Delete:
                if (!this.text.Buffer.IsEmpty)
                {
                    this.SaveHistory();

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
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
                        if (this.text.CursorPosition < this.text.Buffer.Length)
                        {
                            this.text.Delete(new(this.text.CursorPosition, this.text.CursorPosition + 1));
                        }
                    }

                    this.text.ShowCaret();
                }

                break;

            case Key.Backspace:
                if (this.text.CursorPosition > 0 && !this.text.Buffer.IsEmpty)
                {
                    this.SaveHistory();

                    if (this.text.Selection != null)
                    {
                        this.DeleteSelected();
                    }
                    else
                    {
                        if (this.text.CursorPosition == this.text.Buffer.Length)
                        {
                            this.text.Buffer.Remove(this.text.Buffer.Length - 1);
                        }
                        else
                        {
                            this.text.Buffer.Remove((int)this.text.CursorPosition - 1);
                        }

                        this.text.CursorPosition--;
                    }
                }

                break;

            case Key.Enter:
                if (this.Multiline)
                {
                    this.SaveHistory();

                    this.DeleteSelected();

                    if (this.text.CursorPosition == this.text.Buffer.Length)
                    {
                        this.text.Buffer.Append(['\n']);
                    }
                    else
                    {
                        this.text.Buffer.Insert((int)this.text.CursorPosition, ['\n']);
                    }

                    this.text.CursorPosition++;
                }
                else
                {
                    this.Blur();
                }

                break;

            case Key.Left:
                if (this.text.CursorPosition > 0)
                {
                    this.SaveHistory();

                    var cursorPosition = this.text.CursorPosition;

                    this.text.CursorPosition--;

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(this.text.CursorPosition) ?? new(cursorPosition, this.text.CursorPosition);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }
                }

                break;

            case Key.Right:
                if (this.text.CursorPosition < this.text.Buffer.Length)
                {
                    this.SaveHistory();

                    var cursorPosition = this.text.CursorPosition;

                    this.text.CursorPosition++;

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(this.text.CursorPosition) ?? new(cursorPosition, this.text.CursorPosition);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }
                }

                break;

            case Key.Up:
                if (this.Multiline && !this.text.Buffer.IsEmpty && this.text.CursorPosition > 0)
                {
                    this.SaveHistory();

                    var cursorPosition = this.text.CursorPosition;
                    var cursor         = this.GetTrimmedCursorPosition();
                    var currentLine    = this.text.GetCharacterLine(cursor);
                    var previousLine   = this.text.GetCharacterPreviousLine(cursor);
                    var position       = currentLine.Start;

                    if (previousLine.HasValue && !(cursorPosition == this.text.Buffer.Length && this.text.Buffer[^1] == '\n'))
                    {
                        var column = cursorPosition - currentLine.Start;

                        position = column < previousLine.Value.Length ? previousLine.Value.Start + column : previousLine.Value.End;
                    }

                    this.text.CursorPosition = position;

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(this.text.CursorPosition) ?? new(cursorPosition, this.text.CursorPosition);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }
                }

                break;

            case Key.Down:
                if (this.Multiline && this.text.CursorPosition < this.text.Buffer.Length)
                {
                    this.SaveHistory();

                    var cursorPosition = this.text.CursorPosition;
                    var cursor         = this.GetTrimmedCursorPosition();
                    var currentLine    = this.text.GetCharacterLine(cursor);
                    var nextLine       = this.text.GetCharacterNextLine(cursor);
                    var position       = currentLine.End + 1;

                    if (nextLine.HasValue)
                    {
                        var column = this.text.CursorPosition - currentLine.Start;

                        position = column < nextLine.Value.Length
                            ? nextLine.Value.Start + column
                            : nextLine.Value.End + 1 == this.text.Buffer.Length
                                ? (uint)this.text.Buffer.Length
                                : nextLine.Value.End;
                    }

                    this.text.CursorPosition = position;

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(this.text.CursorPosition) ?? new(cursorPosition, this.text.CursorPosition);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }
                }

                break;

            case Key.Home:
                if (!this.text.Buffer.IsEmpty)
                {
                    this.SaveHistory();

                    var cursorPosition = this.text.CursorPosition;

                    this.text.CursorPosition = (!this.Multiline || keyEvent.Modifiers.HasFlags(KeyStates.Control))
                        ? 0u
                        : this.text.GetCharacterLine(this.GetTrimmedCursorPosition()).Start;

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(this.text.CursorPosition) ?? new(cursorPosition, this.text.CursorPosition);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }
                }

                break;

            case Key.End:
                if (!this.text.Buffer.IsEmpty)
                {
                    this.SaveHistory();

                    var cursorPosition = this.text.CursorPosition;

                    uint position;

                    if (!this.Multiline || keyEvent.Modifiers.HasFlags(KeyStates.Control))
                    {
                        position = (uint)this.text.Buffer.Length;
                    }
                    else
                    {
                        var currentLine = this.text.GetCharacterLine(this.GetTrimmedCursorPosition());

                        position = currentLine.End == this.text.Buffer.Length - 1 && this.text.Buffer[^1] != '\n' ? currentLine.End + 1 : currentLine.End;
                    }

                    this.text.CursorPosition = position;

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        this.text.Selection = this.text.Selection?.WithEnd(this.text.CursorPosition) ?? new(cursorPosition, this.text.CursorPosition);
                    }
                    else if (this.text.Selection != null)
                    {
                        this.text.ClearSelection();
                    }
                }

                break;

            case Key.A:
                if (!this.text.Buffer.IsEmpty && keyEvent.Modifiers.HasFlags(KeyStates.Control))
                {
                    this.text.Selection = new(0, (uint)this.text.Buffer.Length);

                    this.text.CursorPosition = (uint)this.text.Buffer.Length;
                }

                break;

            case Key.V:
                {
                    if (keyEvent.Modifiers.HasFlags(KeyStates.Control) && this.Tree is RenderTree renderTree)
                    {
                        this.SaveHistory();

                        this.text.DeleteSelected();

                        if (renderTree.Window.GetClipboardData() is string text)
                        {
                            if (this.text.Buffer.IsEmpty || this.text.CursorPosition == this.text.Buffer.Length)
                            {
                                this.text.Buffer.Append(text);
                            }
                            else
                            {
                                this.text.Buffer.Insert((int)this.text.CursorPosition, text);
                            }

                            this.text.CursorPosition += (uint)text.Length;
                        }
                    }
                }

                break;

            case Key.X:
                {
                    if (keyEvent.Modifiers.HasFlags(KeyStates.Control) && this.Tree is RenderTree renderTree)
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
                if (keyEvent.Modifiers.HasFlags(KeyStates.Control))
                {
                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        this.Redo();
                    }
                    else
                    {
                        this.Undo();
                    }
                }

                break;
        }
    }

    private void OnMouseDown(in MouseEvent mouseEvent)
    {
        if (this.text.Buffer != null && !mouseEvent.Indirect)
        {
            this.text.SetCaret(mouseEvent.X, mouseEvent.Y);
        }
    }

    private void OnTextBufferChanged()
    {
        if (this.IsFocused)
        {
            this.text.CursorPosition = (uint)this.text.Buffer.Length;
        }

        this.textBufferHasChanged = true;
    }

    private void RemoveInputEvents()
    {
        this.Input   -= this.OnInput;
        this.KeyDown -= this.OnKeyDown;
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
