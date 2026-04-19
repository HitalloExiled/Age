namespace ThirdParty.Wayland;

public class WaylandException : Exception
{
    public WaylandException()
    { }

    public WaylandException(string? message) : base(message)
    { }

    public WaylandException(string? message, Exception? innerException) : base(message, innerException)
    { }

    public static void Check(int status, string? message = null)
    {
        if (status < 0)
        {
            throw new WaylandException(message);
        }
    }
}
