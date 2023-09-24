using System.Diagnostics.CodeAnalysis;

namespace Age.Core;

[ExcludeFromCodeCoverage]
public class LoadSymbolException(string symbol) : Exception($"Failed to load symbol {symbol}")
{
}
