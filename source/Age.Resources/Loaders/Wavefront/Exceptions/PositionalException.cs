namespace Age.Resources.Loaders.Wavefront.Exceptions;

public abstract class PositionalException(string message, int line, int column, int index) : Exception(message)
{
    public int Column { get; } = column;
    public int Index  { get; } = index;
    public int Line   { get; } = line;
}
