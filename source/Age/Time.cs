
namespace Age;
public static class Time
{
    public static double   DeltaTime { get; internal set; }
    public static ulong    Frames    { get; internal set; }
    public static int      Scale     { get; internal set; } = 1;
    public static DateTime Start     { get; internal set; }

    public static double   Fps     => 1 / DeltaTime;
    public static TimeSpan Running => DateTime.Now - Start;
}
