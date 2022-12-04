using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Age.Platforms.Windows.Interop;

// cspell:ignore lpfnWndProc, lpszMenuName, lpszClassName, lpwcx

internal delegate nint WndProc(nint hWnd, uint uMsg, nint wParam, nint lParam);

[NativeMarshalling(typeof(WndClassExMarshaller))]
internal struct WndClassEx
{
    public uint cbSize;
    public uint style;
    public WndProc lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public long hInstance;
    public int hIcon;
    public int? hCursor;
    public int? hbrBackground;
    public string? lpszMenuName;
    public string? lpszClassName;
    public int hIconSm;
}

[CustomMarshaller(typeof(WndClassEx), MarshalMode.ManagedToUnmanagedIn, typeof(WndClassExMarshaller))]
[CustomMarshaller(typeof(WndClassEx), MarshalMode.ElementOut, typeof(Out))]
internal static unsafe class WndClassExMarshaller
{
    public struct WndClassExUnmanaged
    {
        public uint cbSize;
        public uint style;
        public nint lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public nint lpszMenuName;
        public nint lpszClassName;
        public nint hIconSm;
    }

    public static class Out
    {
        public static WndClassEx ConvertToManaged(WndClassExUnmanaged unmanaged) => new()
        {
            cbSize        = unmanaged.cbSize,
            style         = unmanaged.style,
            lpfnWndProc   = Marshal.GetDelegateForFunctionPointer<WndProc>(unmanaged.lpfnWndProc),
            cbClsExtra    = unmanaged.cbClsExtra,
            cbWndExtra    = unmanaged.cbWndExtra,
            hInstance     = unmanaged.hInstance.ToInt64(),
            hIcon         = unmanaged.hIcon.ToInt32(),
            hCursor       = unmanaged.hCursor.ToInt32(),
            hbrBackground = unmanaged.hbrBackground.ToInt32(),
            lpszMenuName  = unmanaged.lpszMenuName.ToString(),
            lpszClassName = unmanaged.lpszClassName.ToString(),
            hIconSm       = unmanaged.hIconSm.ToInt32(),
        };
    }

    public static unsafe WndClassExUnmanaged ConvertToUnmanaged(WndClassEx managed) => new()
    {
        cbSize        = managed.cbSize,
        style         = managed.style,
        lpfnWndProc   = Marshal.GetFunctionPointerForDelegate(managed.lpfnWndProc),
        cbClsExtra    = managed.cbClsExtra,
        cbWndExtra    = managed.cbWndExtra,
        hInstance     = new IntPtr(managed.hInstance),
        hIcon         = managed.hIcon,
        hCursor       = managed.hCursor ?? default(nint),
        hbrBackground = managed.hbrBackground ?? default(nint),
        lpszMenuName  = Marshal.StringToHGlobalUni(managed.lpszMenuName),
        lpszClassName = Marshal.StringToHGlobalUni(managed.lpszClassName),
        hIconSm       = managed.hIconSm,
    };

    // public static void Free(WndClassExUnmanaged unmanaged)
    // {
    //     // Marshal.FreeHGlobal(unmanaged.cbSize);
    //     // Marshal.FreeHGlobal(unmanaged.style);
    //     Marshal.FreeHGlobal(unmanaged.lpfnWndProc);
    //     // Marshal.FreeHGlobal(unmanaged.cbClsExtra);
    //     // Marshal.FreeHGlobal(unmanaged.cbWndExtra);
    //     Marshal.FreeHGlobal(unmanaged.hInstance);
    //     Marshal.FreeHGlobal(unmanaged.hIcon);
    //     Marshal.FreeHGlobal(unmanaged.hCursor);
    //     Marshal.FreeHGlobal(unmanaged.hbrBackground);
    //     Marshal.FreeHGlobal(unmanaged.lpszMenuName);
    //     Marshal.FreeHGlobal(unmanaged.lpszClassName);
    //     Marshal.FreeHGlobal(unmanaged.hIconSm);
    // }
}

internal static partial class User32
{
    public const int CS_DBLCLKS         = 0x0008;
    public const int CS_HREDRAW         = 0x0002;
    public const int CS_OWNDC           = 0x0020;
    public const int CS_VREDRAW         = 0x0001;
    public const int MB_ICONEXCLAMATION = 0x00000030;
    public const int MB_OK              = 0x00000000;

    [LibraryImport("User32.dll", SetLastError = true, EntryPoint = "MessageBoxW")]
    private static partial int MessageBoxW(nint h, nint m, nint c, int type);

    [LibraryImport("user32.dll", SetLastError = true, EntryPoint = "RegisterClassExW")]
    public static partial ushort RegisterClassExW(WndClassEx lpwcx);

    public static int MessageBox(int? h, string m, string c, int type) =>
        MessageBoxW(
            h ?? default(nint),
            Marshal.StringToHGlobalUni(m),
            Marshal.StringToHGlobalUni(c),
            type
        );

}
