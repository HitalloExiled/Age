using System.Runtime.InteropServices;

namespace Age.Platforms.Windows.Interop;

internal struct REASON_CONTEXT
{
    public uint          Version;
    public uint          Flags;
    public REASON_CONTEXT_DETAILED Detailed;
    public nint          SimpleReasonString;

    public struct REASON_CONTEXT_DETAILED
    {
        public nint LocalizedReasonModule;
        public uint LocalizedReasonId;
        public uint ReasonStringCount;
        public nint ReasonStrings;
    }
}

internal enum POWER_REQUEST_TYPE
{
    PowerRequestDisplayRequired,
    PowerRequestSystemRequired,
    PowerRequestAwayModeRequired,
    PowerRequestExecutionRequired
}

internal static partial class Kernel32
{
    private const string LIB = "Kernel32.dll";

    [LibraryImport(LIB, SetLastError = true, EntryPoint = "AttachConsole")]
    public static partial byte AttachConsoleNative(int dwProcessId);

    [LibraryImport(LIB, EntryPoint = "PowerCreateRequest")]
    private static partial nint PowerCreateRequestNative(ref REASON_CONTEXT context);

    [LibraryImport(LIB, EntryPoint = "PowerSetRequest")]
    private static partial byte PowerSetRequestNative(nint powerRequest, POWER_REQUEST_TYPE requestType);

    [LibraryImport(LIB, EntryPoint = "PowerClearRequest")]
    private static partial byte PowerClearRequestNative(nint powerRequest, POWER_REQUEST_TYPE requestType);

    [LibraryImport(LIB, EntryPoint = "CloseHandle")]
    public static partial byte CloseHandleNative(nint hObject);

    public static bool AttachConsole(int dwProcessId) =>
        AttachConsoleNative(dwProcessId) == 1;

    public static int PowerCreateRequest(ref REASON_CONTEXT context) =>
        PowerCreateRequestNative(ref context).ToInt32();

    public static bool PowerSetRequest(int powerRequest, POWER_REQUEST_TYPE requestType) =>
        PowerSetRequestNative(powerRequest, requestType) == 1;

    public static bool PowerClearRequest(int powerRequest, POWER_REQUEST_TYPE requestType) =>
        PowerClearRequestNative(powerRequest, requestType) == 1;

    public static bool CloseHandle(int hObject) =>
        CloseHandleNative(hObject) == 1;

}
