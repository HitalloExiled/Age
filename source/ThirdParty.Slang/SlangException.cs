using System.Text;

namespace ThirdParty.Slang;

public class SlangException : Exception
{
    public SlangException()
    { }

    public SlangException(string? message) : base(message)
    { }

    public SlangException(string? message, Exception? innerException) : base(message, innerException)
    { }

    public static void Check(SlangResult slangResult, string message)
    {
        if (slangResult < 0)
        {
            throw new SlangException(message);
        }
    }

    public static void Check(SlangResult slangResult, in Blob blob)
    {
        if (slangResult < 0)
        {
            throw new SlangException(Encoding.UTF8.GetString(blob));
        }
    }
}
