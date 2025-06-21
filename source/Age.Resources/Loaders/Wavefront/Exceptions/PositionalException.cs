namespace Age.Resources.Loaders.Wavefront.Exceptions;

public abstract class PositionalException : Exception
{
    protected PositionalException()
    { }

    protected PositionalException(string? message) : base(message)
    { }

    protected PositionalException(string? message, Exception? innerException) : base(message, innerException)
    { }

    protected PositionalException(string message, int line, int column, int index) : base(message)
    {
        this.Column = column;
        this.Index  = index;
        this.Line   = line;
    }

    public int Column { get; }
    public int Index  { get; }
    public int Line   { get; }
}
