using System.Runtime.Versioning;

namespace Age.Platforms.Abstractions.Interfaces;

/// <inheritdoc cref="FileInfo" />
public interface IFileInfo : IFileSystemInfo
{
    /// <inheritdoc cref="FileInfo.Directory" />
    IDirectoryInfo? Directory { get; }

    /// <inheritdoc cref="FileInfo.DirectoryName" />
    string? DirectoryName { get; }

    /// <inheritdoc cref="FileInfo.IsReadOnly" />
    bool IsReadOnly { get; set; }

    /// <inheritdoc cref="FileInfo.Length" />
    long Length { get; }

    /// <inheritdoc cref="FileInfo.AppendText" />
    StreamWriter AppendText();

    /// <inheritdoc cref="FileInfo.CopyTo(string)" />
    IFileInfo CopyTo(string destFileName);

    /// <inheritdoc cref="FileInfo.CopyTo(string, bool)" />
    IFileInfo CopyTo(string destFileName, bool overwrite);

    /// <inheritdoc cref="FileInfo.Create" />
    FileStream Create();

    /// <inheritdoc cref="FileInfo.CreateText" />
    StreamWriter CreateText();

    /// <inheritdoc cref="FileInfo.Decrypt" />
    [SupportedOSPlatform("windows")]
    void Decrypt();

    /// <inheritdoc cref="FileInfo.Encrypt" />
    [SupportedOSPlatform("windows")]
    void Encrypt();

    /// <inheritdoc cref="FileInfo.MoveTo(string)" />
    void MoveTo(string destFileName);

    /// <inheritdoc cref="FileInfo.MoveTo(string, bool)" />
    void MoveTo(string destFileName, bool overwrite);

    /// <inheritdoc cref="FileInfo.Open(FileMode)" />
    FileStream Open(FileMode mode);

    /// <inheritdoc cref="FileInfo.Open(FileMode, FileAccess)" />
    FileStream Open(FileMode mode, FileAccess access);

    /// <inheritdoc cref="FileInfo.Open(FileMode, FileAccess, FileShare)" />
    FileStream Open(FileMode mode, FileAccess access, FileShare share);

    /// <inheritdoc cref="FileInfo.Open(FileStreamOptions)" />
    FileStream Open(FileStreamOptions options);

    /// <inheritdoc cref="FileInfo.OpenRead" />
    FileStream OpenRead();

    /// <inheritdoc cref="FileInfo.OpenText" />
    StreamReader OpenText();

    /// <inheritdoc cref="FileInfo.OpenWrite" />
    FileStream OpenWrite();

    /// <inheritdoc cref="FileInfo.Replace(string, string?)" />
    IFileInfo Replace(string destinationFileName, string? destinationBackupFileName);

    /// <inheritdoc cref="FileInfo.Replace(string, string?, bool)" />
    IFileInfo Replace(string destinationFileName, string? destinationBackupFileName, bool ignoreMetadataErrors);
}
