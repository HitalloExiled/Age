namespace Age.Platforms.Abstractions.Interfaces;

public interface IFileSystem
{
    /// <inheritdoc cref="System.IO.Directory" />
    IDirectory Directory { get; }

    /// <inheritdoc cref="System.IO.File" />
    IFile File { get; }

    /// <inheritdoc cref="System.IO.DirectoryInfo(string)" />
    IDirectoryInfo GetDirectoryInfo(string path);

    /// <inheritdoc cref="System.IO.FileInfo(string)" />
    IFileInfo GetFileInfo(string path);
}
