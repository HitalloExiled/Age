using Age.Core.IO;

namespace Age.Platforms;

// Cover WindowsTerminalLogger

internal class TerminalLogger : Logger
{
    public override void Logv(string message, bool error)
    {
        if (ShouldLog(error))
        {
            Console.Write(message);
        }
    }
}
