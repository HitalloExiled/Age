namespace Age.Core.Collections;

public class InlineListException : Exception
{
    public InlineListException() { }
    public InlineListException(string message) : base(message) { }
    public InlineListException(string message, Exception inner) : base(message, inner) { }

    public static void ThrowsIfExceeds(int size, int capacity)
    {
        if (size > capacity)
        {
            throw new InlineListException($"FixedArray exceeds capacity of {capacity} elements.");
        }
    }
}
