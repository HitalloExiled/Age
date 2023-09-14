namespace Age.Core;

public class LoadSymbolException : Exception
{
    public LoadSymbolException(string symbol) : base($"Failed to load symbol {symbol}")
    { }
}
