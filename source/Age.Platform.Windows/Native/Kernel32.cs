using System.Runtime.InteropServices;
using Age.Platform.Windows.Native.Types;

namespace Age.Platform.Windows.Native;

internal static partial class Kernel32
{
    /// <summary>
    /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count. When the reference count reaches zero, the module is unloaded from the address space of the calling process and the handle is no longer valid.
    /// </summary>
    /// <param name="hLibModule">
    /// A handle to the loaded library module. The <see cref="LoadLibrary"/>, <see cref="LoadLibraryEx"/>,
    /// <see cref="GetModuleHandle"/>, or <see cref="GetModuleHandleEx"/> function returns this handle.
    /// </param>
    /// <returns>
    /// <para>If the function succeeds, the return value is nonzero.</para>
    /// <para>If the function fails, the return value is zero. To get extended error information, call the <see cref="GetLastError"/> function.</para>
    /// </returns>
    /// <remarks>
    /// <para>The system maintains a per-process reference count for each loaded module. A module that was loaded at process initialization due to load-time dynamic linking has a reference count of one. The reference count for a module is incremented each time the module is loaded by a call to LoadLibrary. The reference count is also incremented by a call to <see cref="LoadLibraryEx"/> unless the module is being loaded for the first time and is being loaded as a data or image file.</para>
    /// <para>The reference count is decremented each time the <see cref="FreeLibrary"/> or <see cref="FreeLibraryAndExitThread"/> function is called for the module. When a module's reference count reaches zero or the process terminates, the system unloads the module from the address space of the process. Before unloading a library module, the system enables the module to detach from the process by calling the module's <see cref="DllMain"/> function, if it has one, with the DLL_PROCESS_DETACH value. Doing so gives the library module an opportunity to clean up resources allocated on behalf of the current process. After the entry-point function returns, the library module is removed from the address space of the current process.</para>
    /// <para>It is not safe to call <see cref="FreeLibrary"/> from <see cref="DllMain"/>. For more information, see the Remarks section in <see cref="DllMain"/>.</para>
    /// <para>Calling <see cref="FreeLibrary"/> does not affect other processes that are using the same module.</para>
    /// <para>Use caution when calling <see cref="FreeLibrary"/> with a handle returned by <see cref="GetModuleHandle"/>. The GetModuleHandle function does not increment a module's reference count, so passing this handle to <see cref="FreeLibrary"/> can cause a module to be unloaded prematurely.</para>
    /// <para>A thread that must unload the DLL in which it is executing and then terminate itself should call <see cref="FreeLibraryAndExitThread"/> instead of calling <see cref="FreeLibrary"/> and ExitThread separately. Otherwise, a race condition can occur. For details, see the Remarks section of <see cref="FreeLibraryAndExitThread"/>.</para>
    /// </remarks>
    /// <example>For an example, see <see href="https://learn.microsoft.com/en-us/windows/desktop/Dlls/using-run-time-dynamic-linking">Using Run-Time Dynamic Linking</see>.</example>
    [LibraryImport(nameof(Kernel32))]
    public static partial BOOL FreeLibrary(HMODULE hLibModule);

    /// <summary>
    /// Retrieves the address of an exported function (also known as a procedure) or variable from the specified dynamic-link library (DLL).
    /// </summary>
    /// <param name="hModule">
    /// <para>A handle to the DLL module that contains the function or variable. The <see cref="LoadLibrary"/>, <see cref="LoadLibraryEx"/>, <see cref="LoadPackagedLibrary"/>, or <see cref="GetModuleHandle"/> function returns this handle.</para>
    /// <para>The <see cref="GetProcAddress"/> function does not retrieve addresses from modules that were loaded using the <see cref="LOAD_LIBRARY_AS_DATAFILE"/> flag. For more information, see <see cref="LoadLibraryEx"/>.</para>
    /// </param>
    /// <param name="lpProcName">The function or variable name, or the function's ordinal value. If this parameter is an ordinal value, it must be in the low-order word; the high-order word must be zero.</param>
    /// <returns>
    /// <para>If the function succeeds, the return value is the address of the exported function or variable.</para>
    /// <para>If the function fails, the return value is NULL. To get extended error information, call GetLastError.</para>
    /// </returns>
    /// <remarks><para>The spelling and case of a function name pointed to by lpProcName must be identical to that in the EXPORTS statement of the source DLL's module-definition (.def) file. The exported names of functions may differ from the names you use when calling these functions in your code. This difference is hidden by macros used in the SDK header files. For more information, see <see href="https://learn.microsoft.com/en-us/windows/desktop/Intl/conventions-for-function-prototypes">Conventions for Function Prototypes</see>.</para>
    /// <para>The lpProcName parameter can identify the DLL function by specifying an ordinal value associated with the function in the EXPORTS statement. <see cref="GetProcAddress"/> verifies that the specified ordinal is in the range 1 through the highest ordinal value exported in the .def file. The function then uses the ordinal as an index to read the function's address from a function table.</para>
    /// <para>If the .def file does not number the functions consecutively from 1 to N (where N is the number of exported functions), an error can occur where <see cref="GetProcAddress"/> returns an invalid, non-NULL address, even though there is no function with the specified ordinal.</para>
    /// <para>If the function might not exist in the DLL module—for example, if the function is available only on Windows Vista but the application might be running on Windows XP—specify the function by name rather than by ordinal value and design your application to handle the case when the function is not available, as shown in the following code fragment.</para>
    /// </remarks>
    /// <example>For an example, see <see href="https://learn.microsoft.com/en-us/windows/desktop/Dlls/using-run-time-dynamic-linking">Using Run-Time Dynamic Linking</see>.</example>
    [LibraryImport(nameof(Kernel32))]
    public static partial FARPROC GetProcAddress(HMODULE hModule, LPCSTR lpProcName);

    /// <inheritdoc cref="GetProcAddress" />
    /// <param name="procName"><inheritdoc cref="GetProcAddress" path="/param[@name='lpProcName']" /></param>
    public static FARPROC GetProcAddress(HMODULE hModule, string procName)
    {
        using var lpProcName = new LPCSTR(procName);

        return GetProcAddress(hModule, lpProcName);
    }

    /// <summary>
    /// Loads the specified module into the address space of the calling process. The specified module may cause other modules to be loaded.
    /// </summary>
    /// <param name="lpLibFileName">
    /// A string that specifies the file name of the module to load. This name is not related to the name stored in a library module itself, as specified by the LIBRARY keyword in the module-definition (.def) file.
    /// <para>The module can be a library module (a .dll file) or an executable module (an .exe file). If the specified module is an executable module, static imports are not loaded; instead, the module is loaded as if DONT_RESOLVE_DLL_REFERENCES was specified. See the dwFlags parameter for more information.</para>
    /// <para>If the string specifies a module name without a path and the file name extension is omitted, and the module name does not contain any point character (.), then the function appends the default library extension ".DLL" to the module name. To prevent the function from appending ".DLL" to the module name, include a trailing point character (.) in the module name string.</para>
    /// <para>If the string specifies a fully qualified path, the function searches only that path for the module. When specifying a path, be sure to use backslashes (\), not forward slashes (/). For more information about paths, see Naming Files, Paths, and Namespaces.</para>
    /// <para>If the string specifies a module name without a path and more than one loaded module has the same base name and extension, the function returns a handle to the module that was loaded first.</para>
    /// <para>If the string specifies a module name without a path and a module of the same name is not already loaded, or if the string specifies a module name with a relative path, the function searches for the specified module. The function also searches for modules if loading the specified module causes the system to load other associated modules (that is, if the module has dependencies). The directories that are searched and the order in which they are searched depend on the specified path and the dwFlags parameter. For more information, see Remarks.</para>
    /// <para>If the function cannot find the module or one of its dependencies, the function fails.</para>
    /// </param>
    /// <param name="hFile">This parameter is reserved for future use. It must be NULL.</param>
    /// <param name="dwFlags">The action to be taken when loading the module. If no flags are specified, the behavior of this function is identical to that of the LoadLibrary function.</param>
    /// <returns>
    /// <para>If the function succeeds, the return value is a handle to the loaded module.</para>
    /// <para>If the function fails, the return value is NULL. To get extended error information, call <see cref="Kernel32.GetLastError"/>.</para>
    /// </returns>
    /// <remarks>The LoadLibraryEx function is very similar to the LoadLibrary function. The differences consist of a set of optional behaviors that LoadLibraryEx provides:
    /// <para>LoadLibraryEx can load a DLL module without calling the <see cref="DllMain"/> function of the DLL.</para>
    /// <para>LoadLibraryEx can load a module in a way that is optimized for the case where the module will never be executed, loading the module as if it were a data file.</para>
    /// <para>LoadLibraryEx can find modules and their associated modules by using either of two search strategies or it can search a process-specific set of directories.</para>
    /// <para>You select these optional behaviors by setting the dwFlags parameter; if dwFlags is zero, LoadLibraryEx behaves identically to LoadLibrary.</para>
    /// <para>The calling process can use the handle returned by LoadLibraryEx to identify the module in calls to the GetProcAddress, <see cref="FindResource"/>, and <see cref="LoadResource"/> functions.</para>
    /// <para>To enable or disable error messages displayed by the loader during DLL loads, use the <see cref="SetErrorMode"/> function.</para>
    /// <para>It is not safe to call LoadLibraryEx from <see cref="DllMain"/>. For more information, see the Remarks section in <see cref="DllMain"/>.</para>
    /// <para>Visual C++:  The Visual C++ compiler supports a syntax that enables you to declare thread-local variables: _declspec(thread). If you use this syntax in a DLL, you will not be able to load the DLL explicitly using LoadLibraryEx on versions of Windows prior to Windows Vista. If your DLL will be loaded explicitly, you must use the thread local storage functions instead of _declspec(thread). For an example, see Using Thread Local Storage in a Dynamic Link Library.</para>
    /// </remarks>
    /// <example>For an example, see <see href="https://learn.microsoft.com/en-us/windows/desktop/NetMgmt/looking-up-text-for-error-code-numbers">Looking Up Text for Error Code Numbers</see>.</example>
    [LibraryImport(nameof(Kernel32))]
    public static partial HMODULE LoadLibraryExW(LPCWSTR lpLibFileName, HANDLE hFile, LOAD_LIBRARY_FLAGS dwFlags);

    /// <inheritdoc cref="LoadLibraryExW" />
    /// <param name="libFileName"><inheritdoc cref="LoadLibraryExW" path="/param[@name='lpLibFileName']" /></param>
    public static HMODULE LoadLibraryExW(string libFileName, HANDLE hFile, LOAD_LIBRARY_FLAGS dwFlags)
    {
        using var lpLibFileName = new LPCWSTR(libFileName);

        return LoadLibraryExW(lpLibFileName, hFile, dwFlags);
    }

    /// <summary>
    /// <para>Loads the specified module into the address space of the calling process. The specified module may cause other modules to be loaded.</para>
    /// <para>For additional load options, use the <see cref="LoadLibraryEx"/> function.</para>
    /// </summary>
    /// <param name="lpLibFileName">
    /// <para>The name of the module. This can be either a library module (a .dll file) or an executable module (an .exe file). If the specified module is an executable module, static imports are not loaded; instead, the module is loaded as if by <see cref="LoadLibraryEx"/> with the <see cref="DONT_RESOLVE_DLL_REFERENCES"/> flag.</para>
    /// <para>The name specified is the file name of the module and is not related to the name stored in the library module itself, as specified by the LIBRARY keyword in the module-definition (.def) file.</para>
    /// <para>If the string specifies a full path, the function searches only that path for the module.</para>
    /// <para>If the string specifies a relative path or a module name without a path, the function uses a standard search strategy to find the module; for more information, see the Remarks.</para>
    /// <para>If the function cannot find the module, the function fails. When specifying a path, be sure to use backslashes (\), not forward slashes (/). For more information about paths, see <see href="https://learn.microsoft.com/en-us/windows/desktop/FileIO/naming-a-file">Naming a File or Directory</see>.</para>
    /// <para>If the string specifies a module name without a path and the file name extension is omitted, the function appends the default library extension ".DLL" to the module name. To prevent the function from appending ".DLL" to the module name, include a trailing point character (.) in the module name string.</para>
    /// </param>
    /// <returns>
    /// <para>If the function succeeds, the return value is a handle to the module.</para>
    /// <para>If the function fails, the return value is NULL. To get extended error information, call <see cref="GetLastError"/>.</para>
    /// </returns>
    /// <remarks>
    /// <para>To enable or disable error messages displayed by the loader during DLL loads, use the <see cref="SetErrorMode"/> function.</para>
    /// <para>LoadLibrary can be used to load a library module into the address space of the process and return a handle that can be used in <see cref="GetProcAddress"/> to get the address of a DLL function. LoadLibrary can also be used to load other executable modules. For example, the function can specify an .exe file to get a handle that can be used in <see cref="FindResource"/> or <see cref="LoadResource"/>. However, do not use LoadLibrary to run an .exe file. Instead, use the <see cref="CreateProcess"/> function.</para>
    /// <para>If the specified module is a DLL that is not already loaded for the calling process, the system calls the DLL's <see cref="DllMain"/> function with the DLL_PROCESS_ATTACH value. If <see cref="DllMain"/> returns TRUE, LoadLibrary returns a handle to the module. If <see cref="DllMain"/> returns FALSE, the system unloads the DLL from the process address space and LoadLibrary returns NULL. It is not safe to call LoadLibrary from <see cref="DllMain"/>. For more information, see the Remarks section in <see cref="DllMain"/>.</para>
    /// <para>Module handles are not global or inheritable. A call to LoadLibrary by one process does not produce a handle that another process can use — for example, in calling GetProcAddress. The other process must make its own call to LoadLibrary for the module before calling GetProcAddress.</para>
    /// <para>If lpFileName does not include a path and there is more than one loaded module with the same base name and extension, the function returns a handle to the module that was loaded first.</para>
    /// <para>If no file name extension is specified in the lpFileName parameter, the default library extension .dll is appended. However, the file name string can include a trailing point character (.) to indicate that the module name has no extension. When no path is specified, the function searches for loaded modules whose base name matches the base name of the module to be loaded. If the name matches, the load succeeds. Otherwise, the function searches for the file.</para>
    /// <para>The first directory searched is the directory containing the image file used to create the calling process (for more information, see the <see cref="CreateProcess"/> function). Doing this allows private dynamic-link library (DLL) files associated with a process to be found without adding the process's installed directory to the PATH environment variable. If a relative path is specified, the entire relative path is appended to every token in the DLL search path list. To load a module from a relative path without searching any other path, use <see cref="GetFullPathName"/> to get a nonrelative path and call LoadLibrary with the nonrelative path. For more information on the DLL search order, see <see href="https://learn.microsoft.com/en-us/windows/desktop/Dlls/dynamic-link-library-search-order">Dynamic-Link Library Search Order</see>.</para>
    /// <para>The search path can be altered using the <see cref="SetDllDirectory"/> function. This solution is recommended instead of using <see cref="SetCurrentDirectory"/> or hard-coding the full path to the DLL.</para>
    /// <para>If a path is specified and there is a redirection file for the application, the function searches for the module in the application's directory. If the module exists in the application's directory, LoadLibrary ignores the specified path and loads the module from the application's directory. If the module does not exist in the application's directory, LoadLibrary loads the module from the specified directory. For more information, see <see href="https://learn.microsoft.com/en-us/windows/desktop/Dlls/dynamic-link-library-redirection">Dynamic Link Library Redirection</see>.</para>
    /// <para>If you call LoadLibrary with the name of an assembly without a path specification and the assembly is listed in the system compatible manifest, the call is automatically redirected to the side-by-side assembly.</para>
    /// <para>The system maintains a per-process reference count on all loaded modules. Calling LoadLibrary increments the reference count. Calling the <see cref="FreeLibrary"/> or <see cref="FreeLibraryAndExitThread"/> function decrements the reference count. The system unloads a module when its reference count reaches zero or when the process terminates (regardless of the reference count).</para>
    /// <para>Windows Server 2003 and Windows XP:  The Visual C++ compiler supports a syntax that enables you to declare thread-local variables: _declspec(thread). If you use this syntax in a DLL, you will not be able to load the DLL explicitly using LoadLibrary on versions of Windows prior to Windows Vista. If your DLL will be loaded explicitly, you must use the thread local storage functions instead of _declspec(thread). For an example, see <see href="https://learn.microsoft.com/en-us/windows/desktop/Dlls/using-thread-local-storage-in-a-dynamic-link-library">Using Thread Local Storage in a Dynamic Link Library</see>.</para>
    /// </remarks>
    /// <example>For an example, see <see href="https://learn.microsoft.com/en-us/windows/desktop/Dlls/using-run-time-dynamic-linking">Using Run-Time Dynamic Linking</see>.</example>
    [LibraryImport(nameof(Kernel32))]
    public static partial HMODULE LoadLibraryW(LPCWSTR lpLibFileName);

    /// <inheritdoc cref="LoadLibraryW(LPCWSTR)" />
    /// <param name="libFileName"><inheritdoc cref="LoadLibraryW(LPCWSTR)" path="/param[@name='lpLibFileName']" /></param>
    public static HMODULE LoadLibraryW(string libFileName)
    {
        using var lpLibFileName = new LPCWSTR(libFileName);

        return LoadLibraryW(lpLibFileName);
    }
}
