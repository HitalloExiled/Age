using Age.Rendering.Commands;

namespace Age.Rendering;

public record CanvasItem
{
    public DrawCommand? Command { get; private set; }

    public void AddCommand(DrawCommand command)
    {
        if (Command != null)
        {
            Command.Next = command;
        }
        else
        {
            Command = command;
        }
    }

    public IEnumerable<DrawCommand> EnumerateCommands()
    {
        var command = Command;

        while (command != null)
        {
            yield return command;

            command = command.Next;
        }
    }
}
