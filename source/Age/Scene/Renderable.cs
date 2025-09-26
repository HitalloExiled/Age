using Age.Commands;

namespace Age.Scene;

public abstract class Renderable : Node
{
    public bool Visible { get; set; } = true;
}

public abstract class Renderable<T> : Renderable where T : Command
{
    internal List<T> Commands { get; set; } = [];

    internal T? SingleCommand
    {
        get => this.Commands.Count == 1 ? this.Commands[0] : null;
        set
        {
            if (value == null)
            {
                this.Commands.Clear();
            }
            else if (this.Commands.Count == 1)
            {
                this.Commands[0] = value;
            }
            else
            {
                this.Commands.Clear();
                this.Commands.Add(value);
            }
        }
    }
}
