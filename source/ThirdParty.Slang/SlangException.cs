namespace ThirdParty.Slang;

public class SlangException : Exception
{
    public SlangException()
    { }

    public SlangException(string? message) : base(message)
    { }

    public SlangException(string? message, Exception? innerException) : base(message, innerException)
    { }

    public static void ThrowIfInvalid(SlangResult slangResult)
    {
        if (slangResult != 0)
        {
            throw new SlangException();
        }
    }
}
