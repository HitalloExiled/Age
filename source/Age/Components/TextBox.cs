using System.Runtime.CompilerServices;
using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Elements;
using Age.Elements.Events;
using Age.Themes;

using Key       = Age.Platforms.Display.Key;
using KeyStates = Age.Platforms.Display.KeyStates;

namespace Age.Components;

public partial class TextBox : Element
{
    public event Action? Changed;

    private readonly DropoutStack<HistoryEntry> redo = new(50);
    private readonly DropoutStack<HistoryEntry> undo = new(50);

    private bool textBufferHasChanged;

    private Text Text => Unsafe.As<Text>(this.ShadowRoot)!;

    public uint CursorPosition
    {
        get => this.Text.CursorPosition;
        set => this.Text.CursorPosition = value;
    }

    public bool Multiline { get; set; }

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

    public string? Value
    {
        get => this.Text.Value;
        set => this.Text.Value = value;
    }

    public TextBox()
    {
        this.IsFocusable = true;

        this.StyleSheet = Theme.Current.TextBox.Outlined;

        this.AttachShadowRoot(new Text());

        this.AddInputEvents();

        this.Blured      += this.OnBlur;
        this.Focused     += this.OnFocused;
        this.MouseDown   += this.OnMouseDown;
        this.Activated   += this.Text.HandleActivate;
        this.Deactivated += this.Text.HandleDeactivate;

        this.Text.Buffer.Changed += this.OnTextBufferChanged;

        this.Seal();
    }

    private void AddInputEvents()
    {
        this.Input   += this.OnInput;
        this.KeyDown += this.OnKeyDown;
    }

    private void ApplyHistory(in HistoryEntry entry)
    {
        this.Text.Buffer.Set(entry.Text);

        this.Text.CursorPosition = entry.CursorPosition;
        this.Text.Selection = entry.Selection;
    }

    private HistoryEntry CreateHistory() =>
        new()
        {
            Text           = this.Text.Buffer.ToString(),
            CursorPosition = this.Text.CursorPosition,
            Selection      = this.Text.Selection
        };

    private uint GetTrimmedCursorPosition() =>
        this.Text.Buffer.IsEmpty ? 0 : uint.Min(this.Text.CursorPosition, (uint)this.Text.Buffer.Length - 1);

    private void OnBlur(in MouseEvent mouseEvent)
    {
        this.Text.ClearSelection();
        this.Text.HideCaret();

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
            this.Text.ShowCaret();
        }
    }

    private void OnInput(char character)
    {
        if (char.IsControl(character))
        {
            return;
        }

        this.SaveHistory();

        if (this.Text.Selection != null)
        {
            this.Text.DeleteSelected();
        }

        if (this.Text.Buffer.IsEmpty || this.Text.CursorPosition == this.Text.Buffer.Length)
        {
            this.Text.Buffer.Append([character]);
        }
        else
        {
            this.Text.Buffer.Insert((int)this.Text.CursorPosition, [character]);
        }

        this.Text.CursorPosition++;
    }

    private void OnKeyDown(in KeyEvent keyEvent)
    {
        var window = this.Scene!.Viewport!.Window!;

        switch (keyEvent.Key)
        {
            case Key.Delete:
                if (!this.Text.Buffer.IsEmpty)
                {
                    this.SaveHistory();

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        var currentLine = this.Text.GetCharacterLine(this.GetTrimmedCursorPosition());

                        if (this.Text.Cut(new(currentLine.Start, currentLine.End + 1)) is string text)
                        {
                            window.SetClipboardData(text);
                        }
                    }
                    else if (this.Text.Selection != null)
                    {
                        this.DeleteSelected();
                    }
                    else
                    {
                        if (this.Text.CursorPosition < this.Text.Buffer.Length)
                        {
                            this.Text.Delete(new(this.Text.CursorPosition, this.Text.CursorPosition + 1));
                        }
                    }

                    this.Text.ShowCaret();
                }

                break;

            case Key.Backspace:
                if (this.Text.CursorPosition > 0 && !this.Text.Buffer.IsEmpty)
                {
                    this.SaveHistory();

                    if (this.Text.Selection != null)
                    {
                        this.DeleteSelected();
                    }
                    else
                    {
                        if (this.Text.CursorPosition == this.Text.Buffer.Length)
                        {
                            this.Text.Buffer.Remove(this.Text.Buffer.Length - 1);
                        }
                        else
                        {
                            this.Text.Buffer.Remove((int)this.Text.CursorPosition - 1);
                        }

                        this.Text.CursorPosition--;
                    }
                }

                break;

            case Key.Enter:
                if (this.Multiline)
                {
                    this.SaveHistory();

                    this.DeleteSelected();

                    if (this.Text.CursorPosition == this.Text.Buffer.Length)
                    {
                        this.Text.Buffer.Append(['\n']);
                    }
                    else
                    {
                        this.Text.Buffer.Insert((int)this.Text.CursorPosition, ['\n']);
                    }

                    this.Text.CursorPosition++;
                }
                else
                {
                    this.Blur();
                }

                break;

            case Key.Left:
                if (this.Text.CursorPosition > 0)
                {
                    this.SaveHistory();

                    var cursorPosition = this.Text.CursorPosition;

                    this.Text.CursorPosition--;

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        this.Text.Selection = this.Text.Selection?.WithEnd(this.Text.CursorPosition) ?? new(cursorPosition, this.Text.CursorPosition);
                    }
                    else if (this.Text.Selection != null)
                    {
                        this.Text.ClearSelection();
                    }
                }

                break;

            case Key.Right:
                if (this.Text.CursorPosition < this.Text.Buffer.Length)
                {
                    this.SaveHistory();

                    var cursorPosition = this.Text.CursorPosition;

                    this.Text.CursorPosition++;

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        this.Text.Selection = this.Text.Selection?.WithEnd(this.Text.CursorPosition) ?? new(cursorPosition, this.Text.CursorPosition);
                    }
                    else if (this.Text.Selection != null)
                    {
                        this.Text.ClearSelection();
                    }
                }

                break;

            case Key.Up:
                if (this.Multiline && !this.Text.Buffer.IsEmpty && this.Text.CursorPosition > 0)
                {
                    this.SaveHistory();

                    var cursorPosition = this.Text.CursorPosition;
                    var cursor         = this.GetTrimmedCursorPosition();
                    var currentLine    = this.Text.GetCharacterLine(cursor);
                    var previousLine   = this.Text.GetCharacterPreviousLine(cursor);
                    var position       = currentLine.Start;

                    if (previousLine.HasValue && !(cursorPosition == this.Text.Buffer.Length && this.Text.Buffer[^1] == '\n'))
                    {
                        var column = cursorPosition - currentLine.Start;

                        position = column < previousLine.Value.Length ? previousLine.Value.Start + column : previousLine.Value.End;
                    }

                    this.Text.CursorPosition = position;

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        this.Text.Selection = this.Text.Selection?.WithEnd(this.Text.CursorPosition) ?? new(cursorPosition, this.Text.CursorPosition);
                    }
                    else if (this.Text.Selection != null)
                    {
                        this.Text.ClearSelection();
                    }
                }

                break;

            case Key.Down:
                if (this.Multiline && this.Text.CursorPosition < this.Text.Buffer.Length)
                {
                    this.SaveHistory();

                    var cursorPosition = this.Text.CursorPosition;
                    var cursor         = this.GetTrimmedCursorPosition();
                    var currentLine    = this.Text.GetCharacterLine(cursor);
                    var nextLine       = this.Text.GetCharacterNextLine(cursor);
                    var position       = currentLine.End + 1;

                    if (nextLine.HasValue)
                    {
                        var column = this.Text.CursorPosition - currentLine.Start;

                        position = column < nextLine.Value.Length
                            ? nextLine.Value.Start + column
                            : nextLine.Value.End + 1 == this.Text.Buffer.Length
                                ? (uint)this.Text.Buffer.Length
                                : nextLine.Value.End;
                    }

                    this.Text.CursorPosition = position;

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        this.Text.Selection = this.Text.Selection?.WithEnd(this.Text.CursorPosition) ?? new(cursorPosition, this.Text.CursorPosition);
                    }
                    else if (this.Text.Selection != null)
                    {
                        this.Text.ClearSelection();
                    }
                }

                break;

            case Key.Home:
                if (!this.Text.Buffer.IsEmpty)
                {
                    this.SaveHistory();

                    var cursorPosition = this.Text.CursorPosition;

                    this.Text.CursorPosition = (!this.Multiline || keyEvent.Modifiers.HasFlags(KeyStates.Control))
                        ? 0u
                        : this.Text.GetCharacterLine(this.GetTrimmedCursorPosition()).Start;

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        this.Text.Selection = this.Text.Selection?.WithEnd(this.Text.CursorPosition) ?? new(cursorPosition, this.Text.CursorPosition);
                    }
                    else if (this.Text.Selection != null)
                    {
                        this.Text.ClearSelection();
                    }
                }

                break;

            case Key.End:
                if (!this.Text.Buffer.IsEmpty)
                {
                    this.SaveHistory();

                    var cursorPosition = this.Text.CursorPosition;

                    uint position;

                    if (!this.Multiline || keyEvent.Modifiers.HasFlags(KeyStates.Control))
                    {
                        position = (uint)this.Text.Buffer.Length;
                    }
                    else
                    {
                        var currentLine = this.Text.GetCharacterLine(this.GetTrimmedCursorPosition());

                        position = currentLine.End == this.Text.Buffer.Length - 1 && this.Text.Buffer[^1] != '\n' ? currentLine.End + 1 : currentLine.End;
                    }

                    this.Text.CursorPosition = position;

                    if (keyEvent.Modifiers.HasFlags(KeyStates.Shift))
                    {
                        this.Text.Selection = this.Text.Selection?.WithEnd(this.Text.CursorPosition) ?? new(cursorPosition, this.Text.CursorPosition);
                    }
                    else if (this.Text.Selection != null)
                    {
                        this.Text.ClearSelection();
                    }
                }

                break;

            case Key.A:
                if (!this.Text.Buffer.IsEmpty && keyEvent.Modifiers.HasFlags(KeyStates.Control))
                {
                    this.Text.Selection = new(0, (uint)this.Text.Buffer.Length);

                    this.Text.CursorPosition = (uint)this.Text.Buffer.Length;
                }

                break;

            case Key.V:
                {
                    if (keyEvent.Modifiers.HasFlags(KeyStates.Control))
                    {
                        this.SaveHistory();

                        this.Text.DeleteSelected();

                        if (window.GetClipboardData() is string text)
                        {
                            if (this.Text.Buffer.IsEmpty || this.Text.CursorPosition == this.Text.Buffer.Length)
                            {
                                this.Text.Buffer.Append(text);
                            }
                            else
                            {
                                this.Text.Buffer.Insert((int)this.Text.CursorPosition, text);
                            }

                            this.Text.CursorPosition += (uint)text.Length;
                        }
                    }
                }

                break;

            case Key.X:
                {
                    if (keyEvent.Modifiers.HasFlags(KeyStates.Control))
                    {
                        this.SaveHistory();

                        if (this.Text.CutSelected() is string text)
                        {
                            window.SetClipboardData(text);
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
        if (this.Text.Buffer != null && !mouseEvent.Indirect)
        {
            this.Text.SetCaret(mouseEvent.X, mouseEvent.Y);
        }
    }

    private void OnTextBufferChanged()
    {
        if (this.IsFocused)
        {
            this.Text.CursorPosition = (uint)this.Text.Buffer.Length;
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
        this.Text.DeleteSelected();

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
