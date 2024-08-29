using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Age.Platforms.Abstractions.Interfaces;

using _FileInfo = System.IO.FileInfo;

namespace Age.Platforms.Abstractions;

[ExcludeFromCodeCoverage]
internal class FileInfo(_FileInfo fileInfo) : FileSystemInfo(fileInfo), IFileInfo
{
    private readonly _FileInfo fileInfo = fileInfo;

    public IDirectoryInfo? Directory => this.fileInfo.Directory != null ? new DirectoryInfo(this.fileInfo.Directory) : null;

    public string? DirectoryName => this.fileInfo.DirectoryName;

    public bool IsReadOnly
    {
        get => this.fileInfo.IsReadOnly;
        set => this.fileInfo.IsReadOnly = value;
    }

    public long Length => this.fileInfo.Length;

    public StreamWriter AppendText() =>
        this.fileInfo.AppendText();

    public IFileInfo CopyTo(string destFileName) =>
        new FileInfo(this.fileInfo.CopyTo(destFileName));

    public IFileInfo CopyTo(string destFileName, bool overwrite) =>
        new FileInfo(this.fileInfo.CopyTo(destFileName, overwrite));

    public FileStream Create() =>
        new(this.fileInfo.Create());

    public StreamWriter CreateText() =>
        this.fileInfo.CreateText();

    [SupportedOSPlatform("windows")]
    public void Decrypt() =>
        this.fileInfo.Decrypt();

    [SupportedOSPlatform("windows")]
    public void Encrypt() =>
        this.fileInfo.Encrypt();

    public void MoveTo(string destFileName) =>
        this.fileInfo.MoveTo(destFileName);

    public void MoveTo(string destFileName, bool overwrite) =>
        this.fileInfo.MoveTo(destFileName, overwrite);

    public FileStream Open(FileMode mode) =>
        new(this.fileInfo.Open(mode));

    public FileStream Open(FileMode mode, FileAccess access) =>
        new(this.fileInfo.Open(mode, access));

    public FileStream Open(FileMode mode, FileAccess access, FileShare share) =>
        new(this.fileInfo.Open(mode, access, share));

    public FileStream Open(FileStreamOptions options) =>
        new(this.fileInfo.Open(options));

    public FileStream OpenRead() =>
        new(this.fileInfo.OpenRead());

    public StreamReader OpenText() =>
        this.fileInfo.OpenText();

    public FileStream OpenWrite() =>
        new(this.fileInfo.OpenWrite());

    public IFileInfo Replace(string destinationFileName, string? destinationBackupFileName) =>
        new FileInfo(this.fileInfo.Replace(destinationFileName, destinationBackupFileName));

    public IFileInfo Replace(string destinationFileName, string? destinationBackupFileName, bool ignoreMetadataErrors) =>
        new FileInfo(this.fileInfo.Replace(destinationFileName, destinationBackupFileName, ignoreMetadataErrors));
}
