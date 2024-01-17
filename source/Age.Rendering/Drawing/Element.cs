using Age.Rendering.Commands;

namespace Age.Rendering.Drawing;

public class Element : Node
{
    public DrawCommand? Command { get; private set; }

    public void AddCommand(DrawCommand command)
    {
        if (this.Command != null)
        {
            this.Command.Next = command;
        }
        else
        {
            this.Command = command;
        }
    }

    public IEnumerable<DrawCommand> EnumerateCommands()
    {
        var command = this.Command;

        while (command != null)
        {
            yield return command;

            command = command.Next;
        }
    }
}
