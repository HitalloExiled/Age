namespace Age.Resources.Loaders.Wavefront.Exceptions;

public class SyntaxErrorException(string message, int line, int column, int index) : PositionalException(message, line, column, index);
