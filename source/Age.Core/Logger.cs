namespace Age.Core;

public static partial class Logger
{
    public static LogLevel Level { get; set; } = LogLevel.Info;
    private static SpinLock spinLock;

    public static void Log(string message, LogLevel level)
    {
        if (level <= Level)
        {
            var timestamp = DateTime.Now;

            var lockTaken = false;

            spinLock.Enter(ref lockTaken);

            Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.Write($"[{timestamp:HH:mm:ss.fff}]");

            Console.ForegroundColor = level switch
            {
                LogLevel.None    => ConsoleColor.White,
                LogLevel.Fatal   => ConsoleColor.DarkRed,
                LogLevel.Error   => ConsoleColor.Red,
                LogLevel.Warning => ConsoleColor.DarkYellow,
                LogLevel.Info    => ConsoleColor.DarkBlue,
                LogLevel.Debug   => ConsoleColor.Green,
                LogLevel.Trace   => ConsoleColor.Cyan,
                _ => ConsoleColor.White,
            };

            Console.Write($" {level}");

            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine($": {message}");

            if (lockTaken)
            {
                spinLock.Exit(false);
            }
        }
    }

    public static void Fatal(string message) =>
        Log(message, LogLevel.Fatal);

    public static void Error(string message) =>
        Log(message, LogLevel.Error);

    public static void Warn(string message) =>
        Log(message, LogLevel.Warning);

    public static void Info(string message) =>
        Log(message, LogLevel.Info);

    public static void Debug(string message) =>
        Log(message, LogLevel.Debug);

    public static void Trace(string message) =>
        Log(message, LogLevel.Trace);
}
