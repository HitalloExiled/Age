namespace Age.Core.Exceptions;

public class SingletonViolationException : Exception
{
    public SingletonViolationException()
    { }

    public SingletonViolationException(string? message) : base(message)
    { }

    public SingletonViolationException(string? message, Exception? innerException) : base(message, innerException)
    { }

    public static void ThrowIfNoSingleton<T>(T singleton) where T : class
    {
        if (singleton != null)
        {
            throw new InvalidOperationException($"Only one single instace of {typeof(T).Name} is allowed");
        }
    }
}
