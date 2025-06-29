namespace Age.Commands;

public sealed partial record TextCommand
{
    [Flags]
    public enum SurrogateKind
    {
        None,
        High,
        Low,
    }
}
