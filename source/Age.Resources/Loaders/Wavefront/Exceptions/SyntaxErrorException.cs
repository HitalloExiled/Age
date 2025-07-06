namespace Age.Resources.Loaders.Wavefront.Exceptions;

public class SyntaxErrorException : PositionalException
{
    protected SyntaxErrorException()
    { }

    protected SyntaxErrorException(string? message) : base(message)
    { }

    protected SyntaxErrorException(string? message, Exception? innerException) : base(message, innerException)
    { }

    public SyntaxErrorException(string message, int line, int column, int index) : base(message, line, column, index)
    { }
}
