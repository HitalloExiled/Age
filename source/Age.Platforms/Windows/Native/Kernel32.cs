using System.Runtime.InteropServices;
using System.Text;
using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal unsafe static partial class Kernel32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-freelibrary"></see>
    /// </summary>
    [LibraryImport(nameof(Kernel32))]
    public static partial BOOL FreeLibrary(HMODULE hLibModule);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-getprocaddress"></see>
    /// </summary>
    [LibraryImport(nameof(Kernel32))]
    public static partial FARPROC GetProcAddress(HMODULE hModule, LPCSTR lpProcName);

    /// <inheritdoc cref="GetProcAddress" />
    /// <param name="procName"><inheritdoc cref="GetProcAddress" path="/param[@name='lpProcName']" /></param>
    public static FARPROC GetProcAddress(HMODULE hModule, string procName)
    {
        fixed (byte* lpProcName = Encoding.UTF8.GetBytes(procName))
        {
            return GetProcAddress(hModule, lpProcName);
        }
    }

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-loadlibraryexw"></see>
    /// </summary>
    [LibraryImport(nameof(Kernel32))]
    public static partial HMODULE LoadLibraryExW(LPCWSTR lpLibFileName, HANDLE hFile, LOAD_LIBRARY_FLAGS dwFlags);

    /// <inheritdoc cref="LoadLibraryExW" />
    public static HMODULE LoadLibraryExW(string libFileName, HANDLE hFile, LOAD_LIBRARY_FLAGS dwFlags)
    {
        fixed (char* lpLibFileName = libFileName)
        {
            return LoadLibraryExW(lpLibFileName, hFile, dwFlags);
        }
    }

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-loadlibraryw"></see>
    /// </summary>
    [LibraryImport(nameof(Kernel32))]
    public static partial HMODULE LoadLibraryW(LPCWSTR lpLibFileName);

    /// <inheritdoc cref="LoadLibraryW(LPCWSTR)" />
    public static HMODULE LoadLibraryW(string libFileName)
    {
        fixed (char* lpLibFileName = libFileName)
        {
            return LoadLibraryW(lpLibFileName);
        }
    }
}
