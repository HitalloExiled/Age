using System.Diagnostics.CodeAnalysis;

namespace Age.Core;

[ExcludeFromCodeCoverage]
public class LoadSymbolException : Exception
{
    public LoadSymbolException()
    { }

    public LoadSymbolException(string? message, Exception? innerException) : base(message, innerException)
    { }

    public LoadSymbolException(string symbol) : base($"Failed to load symbol {symbol}")
    { }
}
