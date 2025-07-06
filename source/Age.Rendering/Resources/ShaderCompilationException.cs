namespace Age.Rendering.Resources;

public class ShaderCompilationException : Exception
{
    public ShaderCompilationException()
    { }

    public ShaderCompilationException(string? message, Exception? innerException) : base(message, innerException)
    { }

    public ShaderCompilationException(string? message) : base(message)
    { }
}
