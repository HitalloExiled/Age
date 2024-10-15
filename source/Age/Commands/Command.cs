namespace Age.Commands;

public abstract record Command
{
    public required int Id { get; set; }
}
