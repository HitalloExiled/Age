using System.Diagnostics.CodeAnalysis;
using Age.Platforms.Abstractions.Interfaces;

using _DirectoryInfo = System.IO.DirectoryInfo;

namespace Age.Platforms.Abstractions;

[ExcludeFromCodeCoverage]
internal class DirectoryInfo(_DirectoryInfo directoryInfo) : FileSystemInfo(directoryInfo), IDirectoryInfo
{
    private readonly _DirectoryInfo directoryInfo = directoryInfo;

    public IDirectoryInfo? Parent => this.directoryInfo.Parent != null ? new DirectoryInfo(this.directoryInfo.Parent) : null;

    public IDirectoryInfo Root => new DirectoryInfo(this.directoryInfo.Root);

    public void Create() =>
        this.directoryInfo.Create();

    public IDirectoryInfo CreateSubdirectory(string path) =>
        new DirectoryInfo(this.directoryInfo.CreateSubdirectory(path));

    public void Delete(bool recursive) =>
        this.directoryInfo.Delete(recursive);

    public IEnumerable<IDirectoryInfo> EnumerateDirectories() =>
        this.directoryInfo.EnumerateDirectories().Select(Wrap);

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern) =>
        this.directoryInfo.EnumerateDirectories(searchPattern).Select(Wrap);

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, EnumerationOptions enumerationOptions) =>
        this.directoryInfo.EnumerateDirectories(searchPattern, enumerationOptions).Select(Wrap);

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption) =>
        this.directoryInfo.EnumerateDirectories(searchPattern, searchOption).Select(Wrap);

    public IEnumerable<IFileInfo> EnumerateFiles() =>
        this.directoryInfo.EnumerateFiles().Select(Wrap);

    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern) =>
        this.directoryInfo.EnumerateFiles(searchPattern).Select(Wrap);

    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, EnumerationOptions enumerationOptions) =>
        this.directoryInfo.EnumerateFiles(searchPattern, enumerationOptions).Select(Wrap);

    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption) =>
        this.directoryInfo.EnumerateFiles(searchPattern, searchOption).Select(Wrap);

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos() =>
        this.directoryInfo.EnumerateFileSystemInfos().Select(Wrap);

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern) =>
        this.directoryInfo.EnumerateFileSystemInfos(searchPattern).Select(Wrap);

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, EnumerationOptions enumerationOptions) =>
        this.directoryInfo.EnumerateFileSystemInfos(searchPattern, enumerationOptions).Select(Wrap);

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption) =>
        this.directoryInfo.EnumerateFileSystemInfos(searchPattern, searchOption).Select(Wrap);

    public IDirectoryInfo[] GetDirectories(string searchPattern, EnumerationOptions enumerationOptions) =>
        this.directoryInfo.GetDirectories(searchPattern, enumerationOptions).Select(Wrap).ToArray();

    public IDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption) =>
        this.directoryInfo.GetDirectories(searchPattern, searchOption).Select(Wrap).ToArray();

    public IDirectoryInfo[] GetDirectories(string searchPattern) =>
        this.directoryInfo.GetDirectories(searchPattern).Select(Wrap).ToArray();

    public IDirectoryInfo[] GetDirectories() =>
        this.directoryInfo.GetDirectories().Select(Wrap).ToArray();

    public IFileInfo[] GetFiles(string searchPattern) =>
        this.directoryInfo.GetFiles(searchPattern).Select(Wrap).ToArray();

    public IFileInfo[] GetFiles(string searchPattern, EnumerationOptions enumerationOptions) =>
        this.directoryInfo.GetFiles(searchPattern, enumerationOptions).Select(Wrap).ToArray();

    public IFileInfo[] GetFiles(string searchPattern, SearchOption searchOption) =>
        this.directoryInfo.GetFiles(searchPattern).Select(Wrap).ToArray();

    public IFileInfo[] GetFiles() =>
        this.directoryInfo.GetFiles().Select(Wrap).ToArray();

    public IFileSystemInfo[] GetFileSystemInfos() =>
        this.directoryInfo.GetFileSystemInfos().Select(Wrap).ToArray();

    public IFileSystemInfo[] GetFileSystemInfos(string searchPattern) =>
        this.directoryInfo.GetFileSystemInfos(searchPattern).Select(Wrap).ToArray();

    public IFileSystemInfo[] GetFileSystemInfos(string searchPattern, EnumerationOptions enumerationOptions) =>
        this.directoryInfo.GetFileSystemInfos(searchPattern, enumerationOptions).Select(Wrap).ToArray();

    public IFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption) =>
        this.directoryInfo.GetFileSystemInfos(searchPattern, searchOption).Select(Wrap).ToArray();

    public void MoveTo(string destDirName) =>
        this.directoryInfo.MoveTo(destDirName);
}
