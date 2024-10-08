
namespace Age.Platforms.Windows.Native;
internal static partial class Kernel32
{
    [Flags]
    public enum LOAD_LIBRARY_FLAGS : uint
    {
        /// <summary>
        /// If this value is used, the system does not call DllMain for process and thread initialization and termination.
        /// Also, the system does not load additional executable modules that are referenced by the specified module.
        /// Do not use this value; it is provided only for backward compatibility.
        /// If you are planning to access only data or resources in the DLL, use LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE or LOAD_LIBRARY_AS_IMAGE_RESOURCE or both.
        /// Otherwise, load the library as a DLL or executable module using the LoadLibrary function.
        /// </summary>
        DONT_RESOLVE_DLL_REFERENCES = 0x00000001,

        /// <summary>
        /// If this value is used, the system does not check AppLocker rules or apply Software Restriction Policies for the DLL.
        /// This action applies only to the DLL being loaded and not to its dependencies.
        /// This value is recommended for use in setup programs that must run extracted DLLs during installation.
        /// </summary>
        LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,

        /// <summary>
        /// If this value is used, the system maps the file into the calling process's virtual address space as if it were a data file.
        /// Nothing is done to execute or prepare to execute the mapped file.
        /// Therefore, you cannot call functions like GetModuleFileName, GetModuleHandle, or GetProcAddress with this DLL.
        /// Using this value causes writes to read-only memory to raise an access violation.
        /// Use this flag when you want to load a DLL only to extract messages or resources from it.
        /// </summary>
        LOAD_LIBRARY_AS_DATAFILE = 0x00000002,

        /// <summary>
        /// Similar to LOAD_LIBRARY_AS_DATAFILE, except that the DLL file is opened with exclusive write access for the calling process.
        /// Other processes cannot open the DLL file for write access while it is in use.
        /// However, the DLL can still be opened by other processes.
        /// </summary>
        LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,

        /// <summary>
        /// If this value is used, the system maps the file into the process's virtual address space as an image file.
        /// However, the loader does not load the static imports or perform the other usual initialization steps.
        /// Use this flag when you want to load a DLL only to extract messages or resources from it.
        /// Unless the application depends on the file having the in-memory layout of an image,
        /// this value should be used with either LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE or LOAD_LIBRARY_AS_DATAFILE.
        /// </summary>
        LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,

        /// <summary>
        /// If this value is used, the application's installation directory is searched for the DLL and its dependencies.
        /// Directories in the standard search path are not searched.
        /// This value cannot be combined with LOAD_WITH_ALTERED_SEARCH_PATH.
        /// </summary>
        LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,

        /// <summary>
        /// This value is a combination of LOAD_LIBRARY_SEARCH_APPLICATION_DIR, LOAD_LIBRARY_SEARCH_SYSTEM32, and LOAD_LIBRARY_SEARCH_USER_DIRS.
        /// Directories in the standard search path are not searched.
        /// This value cannot be combined with LOAD_WITH_ALTERED_SEARCH_PATH.
        /// This value represents the recommended maximum number of directories an application should include in its DLL search path.
        /// </summary>
        LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000,

        /// <summary>
        /// If this value is used, the directory that contains the DLL is temporarily added to the beginning of the list of directories
        /// that are searched for the DLL's dependencies. Directories in the standard search path are not searched.
        /// The lpFileName parameter must specify a fully qualified path.
        /// This value cannot be combined with LOAD_WITH_ALTERED_SEARCH_PATH.
        /// For example, if Lib2.dll is a dependency of C:\Dir1\Lib1.dll, loading Lib1.dll with this value causes the system to search for Lib2.dll only in C:\Dir1.
        /// To search for Lib2.dll in C:\Dir1 and all of the directories in the DLL search path, combine this value with LOAD_LIBRARY_DEFAULT_DIRS.
        /// </summary>
        LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,

        /// <summary>
        /// If this value is used, %windows%\system32 is searched for the DLL and its dependencies.
        /// Directories in the standard search path are not searched.
        /// This value cannot be combined with LOAD_WITH_ALTERED_SEARCH_PATH.
        /// </summary>
        LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,

        /// <summary>
        /// If this value is used, directories added using the AddDllDirectory or the SetDllDirectory function are searched for the DLL and its dependencies.
        /// If more than one directory has been added, the order in which the directories are searched is unspecified.
        /// Directories in the standard search path are not searched.
        /// This value cannot be combined with LOAD_WITH_ALTERED_SEARCH_PATH.
        /// </summary>
        LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400,

        /// <summary>
        /// If this value is used and lpFileName specifies an absolute path,
        /// the system uses the alternate file search strategy discussed in the Remarks section to find associated executable modules
        /// that the specified module causes to be loaded.
        /// If this value is used and lpFileName specifies a relative path, the behavior is undefined.
        /// If this value is not used or if lpFileName does not specify a path,
        /// the system uses the standard search strategy discussed in the Remarks section
        /// to find associated executable modules that the specified module causes to be loaded.
        /// This value cannot be combined with any LOAD_LIBRARY_SEARCH flag.
        /// </summary>
        LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008,

        /// <summary>
        // Specifies that the digital signature of the binary image must be checked at load time.
        // This value requires Windows 8.1, Windows 10 or later.
        /// </summary>
        LOAD_LIBRARY_REQUIRE_SIGNED_TARGET = 0x00000080,

        /// <summary>
        /// If this value is used, loading a DLL for execution from the current directory is only allowed
        /// if it is under a directory in the Safe load list.
        /// </summary>
        LOAD_LIBRARY_SAFE_CURRENT_DIRS = 0x00002000
    }
}
