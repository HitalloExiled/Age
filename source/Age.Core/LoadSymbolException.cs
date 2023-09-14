using System.Diagnostics.CodeAnalysis;

namespace Age.Core;

[ExcludeFromCodeCoverage]
public class LoadSymbolException : Exception
{
    public LoadSymbolException(string symbol) : base($"Failed to load symbol {symbol}")
    { }
}
