using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Elements;
using Age.Extensions;
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

        this.Blured     += this.OnBlurer;
        this.MouseDown  += this.OnMouseDown;
        this.Focused    += this.OnFocused;
        this.Input      += this.OnInput;
        this.KeyDown    += this.OnKeyDown;
        this.MouseMoved += this.OnMouseMove;
    }

    private void ApplyHistory(in HistoryEntry entry)
    {
        this.text.Value     = entry.Text;
        this.CursorPosition = entry.CursorPosition;
        this.text.Selection = entry.Selection;
    }

    private void AdjustScroll()
    {
        if (this.text.Value?.Length > 0)
        {
            var boxModel        = this.GetBoxModel();
            var cursorBoundings = this.text.GetCursorBoundings();

            var leftBounds   = boxModel.Boundings.Left   + boxModel.Border.Left   + boxModel.Padding.Left;
            var rightBounds  = boxModel.Boundings.Right  - boxModel.Border.Right  - boxModel.Padding.Right;
            var topBounds    = boxModel.Boundings.Top    + boxModel.Border.Top    + boxModel.Padding.Top;
            var bottomBounds = boxModel.Boundings.Bottom - boxModel.Border.Bottom - boxModel.Padding.Bottom;

            var scroll = this.Scroll;

            if (cursorBoundings.Left - this.Scroll.X < leftBounds)
            {
                var characterBounds = this.text.GetCharacterBoundings(this.CursorPosition);

                scroll.X = (uint)(characterBounds.Left - leftBounds);
            }
            else if (cursorBoundings.Right - this.Scroll.X > rightBounds)
            {
                var position        = this.CursorPosition.ClampSubtract(1);
                var characterBounds = this.text.GetCharacterBoundings(position);

                scroll.X = (uint)(characterBounds.Right + cursorBoundings.Size.Height - rightBounds);
            }

            if (cursorBoundings.Top - this.Scroll.Y < topBounds)
            {
                var characterBounds = this.text.GetCharacterBoundings(this.CursorPosition);

                scroll.Y = (uint)(characterBounds.Top - topBounds);
            }
            else if (cursorBoundings.Bottom - this.Scroll.Y > bottomBounds)
            {
                var position        = this.CursorPosition.ClampSubtract(1);
                var characterBounds = this.text.GetCharacterBoundings(position);

                scroll.Y = (uint)(characterBounds.Bottom + cursorBoundings.Size.Height - bottomBounds);
            }

            this.Scroll = scroll;
        }
        else
        {
            this.Scroll = default;
        }
    }

    private HistoryEntry CreateHistory() =>
        new()
        {
            Text           = this.text.Value,
            CursorPosition = this.CursorPosition,
            Selection      = this.text.Selection
        };

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

                    var lineInfo     = new LineInfo(this.text.Value, this.CursorPosition);
                    var position     = lineInfo.Start;
                    var previousLine = lineInfo.PreviousLine();

                    if (!previousLine.IsEmpty && !(this.CursorPosition == this.text.Value.Length && this.text.Value[^1] == '\n'))
                    {
                        var column = this.CursorPosition - lineInfo.Start;

                        position = column < previousLine.Length ? previousLine.Start + column : previousLine.End;
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

                    var lineInfo = new LineInfo(this.text.Value, this.CursorPosition);
                    var position = lineInfo.End + 1;
                    var nextLine = lineInfo.NextLine();

                    if (!nextLine.IsEmpty)
                    {
                        var column  = this.CursorPosition - lineInfo.Start;

                        position = column < nextLine.Length
                            ? nextLine.Start + column
                            : nextLine.End == this.text.Value.Length - 1
                                ? (uint)this.text.Value.Length
                                : nextLine.End;
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
                        : new LineInfo(this.text.Value, this.CursorPosition).Start;

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
                        var lineInfo = new LineInfo(this.text.Value, this.CursorPosition);

                        position = lineInfo.End == this.text.Value.Length - 1 && this.text.Value[^1] != '\n' ? lineInfo.End + 1 : lineInfo.End;
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

    private void OnMouseMove(in MouseEvent mouseEvent)
    {
        if (this.text.Value != null && !mouseEvent.Indirect && mouseEvent.IsHoldingPrimaryButton)
        {
            this.text.Layout.UpdateSelection(mouseEvent.X, mouseEvent.Y);
        }
     }

    private void SaveHistory() =>
        this.undo.Push(this.CreateHistory());

    public void DeleteSelected()
    {
        this.text.DeleteSelected();
        this.AdjustScroll();
    }

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
