using Age.Core.IO;
using Age.Platforms.Windows;

#pragma warning disable IDE0051,CS0169,IDE0044,CS0649 // TODO - Remove

namespace Age.Core.OS;

public enum RenderThreadMode
{
    NONE = -1,
    RENDER_THREAD_UNSAFE,
    RENDER_THREAD_SAFE,
    RENDER_SEPARATE_THREAD
};

internal abstract partial class OS
{
    public static OS Singleton { get; } = new OSWindows();

    private bool stdoutEnabled;

    protected abstract CompositeLogger? Logger { get; }

    public abstract bool     IsInLowProcessorUsageMode  { get; }
    public abstract MainLoop MainLoop { get; set; }

    public bool             AllowHidpi                     { get; set; }
    public bool             AllowLayered                   { get; set; }
    public string?          CurrentRenderingDriverName     { get; set; }
    public string?          CurrentRenderingMethod         { get; set; }
    public RenderThreadMode RenderThreadMode               { get; set; }
    public int              DisplayDriverId                { get; set; }
    public bool             VerboseStdout                  { get; set; }
    public bool             LowProcessorUsageMode          { get; set; }
    public int              LowProcessorUsageModeSleepUsec { get; set; }
    public int              ExitCode                       { get; set; }

    public abstract void AddFrameDelay(bool canDraw);
    public abstract void FinalizeCore();
    public abstract void Initialize();
    public abstract void Run();

    public abstract void Alert(string alert, string title);
    public void Print(string message)
    {
        if (!this.stdoutEnabled)
        {
            return;
        }

        this.Logger?.Logv(message, false);
    }
    public void PrintError(string function, string file, int line, string code, string rationale, bool editorNotify, Logger.ErrorType type) => throw new NotImplementedException();
}
