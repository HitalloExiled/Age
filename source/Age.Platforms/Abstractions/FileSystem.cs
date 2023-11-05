using System.Diagnostics.CodeAnalysis;
using Age.Platforms.Abstractions.Interfaces;

using _DirectoryInfo = System.IO.DirectoryInfo;
using _FileInfo      = System.IO.FileInfo;

namespace Age.Platforms.Abstractions;

[ExcludeFromCodeCoverage]
public class FileSystem : IFileSystem
{
    public IDirectory Directory { get; } = new Directory();
    public IFile      File      { get; } = new File();

    public IDirectoryInfo GetDirectoryInfo(string path) =>
        new DirectoryInfo(new _DirectoryInfo(path));

    public IFileInfo GetFileInfo(string path) =>
        new FileInfo(new _FileInfo(path));
}
