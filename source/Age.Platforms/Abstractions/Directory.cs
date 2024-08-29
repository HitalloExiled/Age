using System.Diagnostics.CodeAnalysis;
#if NET7_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using Age.Platforms.Abstractions.Interfaces;

using _Directory = System.IO.Directory;
using _DirectoryInfo = System.IO.DirectoryInfo;

namespace Age.Platforms.Abstractions;

[ExcludeFromCodeCoverage]
internal class Directory : IDirectory
{
    public IDirectoryInfo CreateDirectory(string path) =>
        new DirectoryInfo(_Directory.CreateDirectory(path));

#if NET7_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    public IDirectoryInfo CreateDirectory(string path, UnixFileMode unixCreateMode) =>
        new DirectoryInfo(_Directory.CreateDirectory(path, unixCreateMode));
#endif

    public IFileSystemInfo CreateSymbolicLink(string path, string pathToTarget) =>
        new DirectoryInfo((_DirectoryInfo)_Directory.CreateSymbolicLink(path, pathToTarget));

#if NET7_0_OR_GREATER
    public IDirectoryInfo CreateTempSubdirectory(string? prefix = null) =>
        new DirectoryInfo(_Directory.CreateTempSubdirectory(prefix));
#endif

    public void Delete(string path) =>
        _Directory.Delete(path);

    public void Delete(string path, bool recursive) =>
        _Directory.Delete(path, recursive);

    public IEnumerable<string> EnumerateDirectories(string path) =>
        _Directory.EnumerateDirectories(path);

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern) =>
        _Directory.EnumerateDirectories(path, searchPattern);

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions) =>
        _Directory.EnumerateDirectories(path, searchPattern, enumerationOptions);

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        _Directory.EnumerateDirectories(path, searchPattern, searchOption);

    public IEnumerable<string> EnumerateFiles(string path) =>
        _Directory.EnumerateFiles(path);

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern) =>
        _Directory.EnumerateFiles(path, searchPattern);

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, EnumerationOptions enumerationOptions) =>
        _Directory.EnumerateFiles(path, searchPattern, enumerationOptions);

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        _Directory.EnumerateFiles(path, searchPattern, searchOption);

    public IEnumerable<string> EnumerateFileSystemEntries(string path) =>
        _Directory.EnumerateFileSystemEntries(path);

    public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern) =>
        _Directory.EnumerateFileSystemEntries(path, searchPattern);

    public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, EnumerationOptions enumerationOptions) =>
        _Directory.EnumerateFileSystemEntries(path, searchPattern, enumerationOptions);

    public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) =>
        _Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);

    public bool Exists([NotNullWhen(true)] string? path) =>
        _Directory.Exists(path);

    public DateTime GetCreationTime(string path) =>
        _Directory.GetCreationTime(path);

    public DateTime GetCreationTimeUtc(string path) =>
        _Directory.GetCreationTimeUtc(path);

    public string GetCurrentDirectory() =>
        _Directory.GetCurrentDirectory();

    public string[] GetDirectories(string path) =>
        _Directory.GetDirectories(path);

    public string[] GetDirectories(string path, string searchPattern) =>
        _Directory.GetDirectories(path, searchPattern);

    public string[] GetDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions) =>
        _Directory.GetDirectories(path, searchPattern, enumerationOptions);

    public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) =>
        _Directory.GetDirectories(path, searchPattern, searchOption);

    public string GetDirectoryRoot(string path) =>
        _Directory.GetDirectoryRoot(path);

    public string[] GetFiles(string path) =>
        _Directory.GetFiles(path);

    public string[] GetFiles(string path, string searchPattern) =>
        _Directory.GetFiles(path, searchPattern);

    public string[] GetFiles(string path, string searchPattern, EnumerationOptions enumerationOptions) =>
        _Directory.GetFiles(path, searchPattern, enumerationOptions);

    public string[] GetFiles(string path, string searchPattern, SearchOption searchOption) =>
        _Directory.GetFiles(path, searchPattern, searchOption);

    public string[] GetFileSystemEntries(string path) =>
        _Directory.GetFileSystemEntries(path);

    public string[] GetFileSystemEntries(string path, string searchPattern) =>
        _Directory.GetFileSystemEntries(path, searchPattern);

    public string[] GetFileSystemEntries(string path, string searchPattern, EnumerationOptions enumerationOptions) =>
        _Directory.GetFileSystemEntries(path, searchPattern, enumerationOptions);

    public string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption) =>
        _Directory.GetFileSystemEntries(path, searchPattern, searchOption);

    public DateTime GetLastAccessTime(string path) =>
        _Directory.GetLastAccessTime(path);

    public DateTime GetLastAccessTimeUtc(string path) =>
        _Directory.GetLastAccessTimeUtc(path);

    public DateTime GetLastWriteTime(string path) =>
        _Directory.GetLastWriteTime(path);

    public DateTime GetLastWriteTimeUtc(string path) =>
        _Directory.GetLastWriteTimeUtc(path);

    public string[] GetLogicalDrives() =>
        _Directory.GetLogicalDrives();

    public IDirectoryInfo? GetParent(string path) =>
        _Directory.GetParent(path) is _DirectoryInfo directoryInfo ? new DirectoryInfo(directoryInfo) : null;

    public void Move(string sourceDirName, string destDirName) =>
        _Directory.Move(sourceDirName, destDirName);

    public IFileSystemInfo? ResolveLinkTarget(string linkPath, bool returnFinalTarget) =>
        FileSystemInfo.WrapOrNull(_Directory.ResolveLinkTarget(linkPath, returnFinalTarget));

    public void SetCreationTime(string path, DateTime creationTime) =>
        _Directory.SetCreationTime(path, creationTime);

    public void SetCreationTimeUtc(string path, DateTime creationTimeUtc) =>
        _Directory.SetCreationTimeUtc(path, creationTimeUtc);

    public void SetCurrentDirectory(string path) =>
        _Directory.SetCurrentDirectory(path);

    public void SetLastAccessTime(string path, DateTime lastAccessTime) =>
        _Directory.SetLastAccessTime(path, lastAccessTime);

    public void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
        _Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc);

    public void SetLastWriteTime(string path, DateTime lastWriteTime) =>
        _Directory.SetLastWriteTime(path, lastWriteTime);

    public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
        _Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
}
