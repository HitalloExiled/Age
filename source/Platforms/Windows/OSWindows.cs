#define WINDOWS_DEBUG_OUTPUT_ENABLED
#define WINDOWS_SUBSYSTEM_CONSOLE

using System.Diagnostics;
using Age.Core.Error;
using Age.Core.IO;
using Age.Core.OS;
using Age.Platforms.Windows.Interop;
using static Age.Core.Error.ErrorMacro;

namespace Age.Platforms.Windows;

#pragma warning disable IDE0052,IDE0044 // TODO - Remove

internal partial class OSWindows : OS
{
    public static new OS Singleton { get; private set; } = null!;

    private readonly List<Logger> loggers = new();

    private CompositeLogger? logger;

    protected override CompositeLogger? Logger => this.logger;

    private ErrorHandlerList errorHandlerList = null!;

    public override bool IsInLowProcessorUsageMode { get; }
    public override MainLoop MainLoop { get; set; } = null!;

    public OSWindows()
    {
        Singleton = this;

        // TODO - platform\windows\os_windows.cpp[1485:1490]

        DisplayServerWindows.RegisterWindowsDriver();

        // TODO - platform\windows\os_windows.cpp[1499:1504]

        this.logger = new CompositeLogger(new() { new TerminalLogger() });
    }

    #if WINDOWS_DEBUG_OUTPUT_ENABLED
    private static void ErrorHandler(object userData, string function, string file, int line, string error, string message, bool editorNotify, ErrorHandlerType type)
    {
        var output = !string.IsNullOrEmpty(message) ? message : $"{file}:{line} - {error}{(editorNotify ? " (User)" : "")}\n";

        Debug.Write(output);
    }
    #endif

    private static void RedirectIOToConsole()
    {
        if (Kernel32.AttachConsole(-1))
        {
            // TODO - platform\windows\os_windows.cpp[103:105]
        }
    }

    public override void AddFrameDelay(bool canDraw) => throw new NotImplementedException();

    public override void Alert(string alert, string title) =>
        Console.WriteLine($"{title}: {alert}");

    public override void FinalizeCore() => throw new NotImplementedException();
    public override void Initialize()
    {
        #if WINDOWS_DEBUG_OUTPUT_ENABLED
        this.errorHandlerList = new (this, ErrorHandler);
        AddErrorHandler(this.errorHandlerList);
        #endif

        #if WINDOWS_SUBSYSTEM_CONSOLE
        RedirectIOToConsole();
        #endif
    }

    public override void Run() => throw new NotImplementedException();
}
