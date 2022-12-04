using System.Runtime.CompilerServices;
using static Age.Core.Error.ErrorMacro;

namespace Age.Core.Config;

#pragma warning disable IDE0052 // TODO - Remove

internal class Engine
{
    public static Engine Singleton { get; private set; } = null!;

    private readonly Dictionary<Type, object>   singletons = new();
    private readonly Dictionary<string, double> startupBenchmarkJson = new();

    private long    startupBenchmarkFrom;
    private string? startupBenchmarkSection;
    private long    startupBenchmarkTotalFrom;

    public uint   Fps                          { get; set; }
    public int    FrameDelay                   { get; set; }
    public long   FramesDrawn                  { get; set; }
    public uint   FrameTicks                   { get; set; }
    public bool   IsEditorHint                 { get; set; }
    public bool   IsProjectManagerHint         { get; set; }
    public int    MaxFps                       { get; set; }
    public int    MaxPhysicsStepsPerFrame      { get; set; }
    public double PhysicsInterpolationFraction { get; set; }
    public double PhysicsJitterFix             { get; set; }
    public int    PhysicsTicksPerSecond        { get; set; }
    public long   ProcessFrames                { get; set; }
    public double ProcessStep                  { get; set; }
    public bool   ProjectManagerHint           { get; set; }
    public double TimeScale                    { get; set; }

    public Engine() => Singleton = this;

    public void StartupBegin() => this.startupBenchmarkTotalFrom = DateTime.Now.Ticks;

    public void StartupBenchmarkBeginMeasure(string what)
    {
        this.startupBenchmarkSection = what;
	    this.startupBenchmarkFrom    = DateTime.Now.Ticks;
    }

    public void StartupBenchmarkEndMeasure()
    {
        var total = DateTime.Now.Ticks - this.startupBenchmarkFrom / 1000000D;

        this.startupBenchmarkJson[this.startupBenchmarkSection!] = total;
    }

    public void AddSingleton<T>(T instance, [CallerArgumentExpression("instance")] string expression = null!) where T : notnull
    {
        var type = typeof(T);

        if (ERR_FAIL_COND_MSG(this.singletons.ContainsKey(type), "Can't register singleton that already exists: " + expression))
        {
            return;
        }

        this.singletons.Add(type, instance);
    }
}
