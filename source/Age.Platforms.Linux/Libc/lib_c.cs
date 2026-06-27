using System.Runtime.InteropServices;

namespace Age.Platforms.Linux.Libc;

internal unsafe static partial class lib_c
{
    private const string LIBRARY = "libc.so.6";

    public const int   MAP_SHARED        = 0x01;
    public const uint  MFD_ALLOW_SEALING = 0x0002;
    public const uint  MFD_CLOEXEC       = 0x0001;
    public const uint  MFD_HUGETLB       = 0x0004;
    public const short POLLHUP           = 0x020;
    public const short POLLIN            = 0x001;
    public const int   PROT_READ         = 0x1;
    public const int   PROT_WRITE        = 0x2;

    public const int MAP_PRIVATE = 0x02;

    public static void* MAP_FAILED => (void*)(nint)(-1);

    [LibraryImport(LIBRARY)]
    public static partial int close(int __fd);

    [LibraryImport(LIBRARY)]
    public static partial int ftruncate(int __fd, __off_t __length);

    [LibraryImport(LIBRARY)]
    public static partial int memfd_create(byte* name, uint flags);

    [LibraryImport(LIBRARY)]
    public static partial void* mmap(void* __addr, size_t __len, int __prot, int __flags, int __fd, __off_t __offset);

    [LibraryImport(LIBRARY)]
    public static partial int munmap(void* __addr, size_t __len);

    [LibraryImport(LIBRARY)]
    public static partial int pipe(int* pipefd);

    [LibraryImport(LIBRARY)]
    public static partial int poll(pollfd* __fds, nfds_t __nfds, int __timeout);

    [LibraryImport(LIBRARY)]
    public static partial nint read(int __fd, void* __buf, nuint __nbytes);

    [LibraryImport(LIBRARY)]
    public static partial nint write(int __fd, void* __buf, nuint __n);
}
