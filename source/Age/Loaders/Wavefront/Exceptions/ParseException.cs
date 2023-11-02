namespace Age.Loaders.Wavefront.Exceptions;

public class ParseException(string message, int line, int column, int index) : PositionalException(message, line, column, index);
