namespace Age.Core;

public class UnsafeLockException : Exception
{
    public UnsafeLockException()
    { }

    public UnsafeLockException(string message) : base(message)
    { }

    public UnsafeLockException(string? message, Exception? innerException) : base(message, innerException)
    { }
}
