namespace Age.Resources.Loaders.Wavefront.Exceptions;

public class ParseException : PositionalException
{
    protected ParseException()
    { }

    protected ParseException(string? message) : base(message)
    { }

    protected ParseException(string? message, Exception? innerException) : base(message, innerException)
    { }

    public ParseException(string message, int line, int column, int index) : base(message, line, column, index)
    { }
}
