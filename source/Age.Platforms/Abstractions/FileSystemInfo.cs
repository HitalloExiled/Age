using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using Age.Platforms.Abstractions.Interfaces;

using _DirectoryInfo  = System.IO.DirectoryInfo;
using _FileInfo       = System.IO.FileInfo;
using _FileSystemInfo = System.IO.FileSystemInfo;

namespace Age.Platforms.Abstractions;

[ExcludeFromCodeCoverage]
internal abstract class FileSystemInfo(_FileSystemInfo fileSystemInfo) : MarshalByRefObject, IFileSystemInfo
{
    private readonly _FileSystemInfo fileSystemInfo = fileSystemInfo;

    /// <inheritdoc cref="_FileSystemInfo.LinkTarget" />
    public string? LinkTarget => this.fileSystemInfo.LinkTarget;

    /// <inheritdoc cref="_FileSystemInfo.LastWriteTimeUtc" />
    public DateTime LastWriteTimeUtc
    {
        get => this.fileSystemInfo.LastWriteTimeUtc;
        set => this.fileSystemInfo.LastWriteTimeUtc = value;
    }

    /// <inheritdoc cref="_FileSystemInfo.LastWriteTime" />
    public DateTime LastWriteTime
    {
        get => this.fileSystemInfo.LastWriteTime;
        set => this.fileSystemInfo.LastWriteTime = value;
    }

    /// <inheritdoc cref="_FileSystemInfo.LastAccessTimeUtc" />
    public DateTime LastAccessTimeUtc
    {
        get => this.fileSystemInfo.LastAccessTimeUtc;
        set => this.fileSystemInfo.LastAccessTimeUtc = value;
    }

    /// <inheritdoc cref="_FileSystemInfo.LastAccessTime" />
    public DateTime LastAccessTime
    {
        get => this.fileSystemInfo.LastAccessTime;
        set => this.fileSystemInfo.LastAccessTime = value;
    }

    /// <inheritdoc cref="_FileSystemInfo.FullName" />
    public string FullName => this.fileSystemInfo.FullName;

    /// <inheritdoc cref="_FileSystemInfo.Extension" />
    public string Extension => this.fileSystemInfo.Extension;

    /// <inheritdoc cref="_FileSystemInfo.Exists" />
    public bool Exists => this.fileSystemInfo.Exists;

    /// <inheritdoc cref="_FileSystemInfo.CreationTime" />
    public DateTime CreationTime
    {
        get => this.fileSystemInfo.CreationTime;
        set => this.fileSystemInfo.CreationTime = value;
    }

    /// <inheritdoc cref="_FileSystemInfo.Name" />
    public string Name => this.fileSystemInfo.Name;

    /// <inheritdoc cref="_FileSystemInfo.Attributes" />
    public FileAttributes Attributes
    {
        get => this.fileSystemInfo.Attributes;
        set => this.fileSystemInfo.Attributes = value;
    }

    /// <inheritdoc cref="_FileSystemInfo.CreationTimeUtc" />
    public DateTime CreationTimeUtc
    {
        get => this.fileSystemInfo.CreationTimeUtc;
        set => this.fileSystemInfo.CreationTimeUtc = value;
    }

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="_FileSystemInfo.UnixFileMode" />
    public UnixFileMode UnixFileMode
    {
        get => this.fileSystemInfo.UnixFileMode;
        [UnsupportedOSPlatform("windows")]
        #pragma warning disable CA1416
        set => this.fileSystemInfo.UnixFileMode = value;
        #pragma warning restore CA1416
    }
#endif

    internal static DirectoryInfo Wrap(_DirectoryInfo directoryInfo) =>
        new(directoryInfo);

    internal static FileInfo Wrap(_FileInfo fileInfo) =>
        new(fileInfo);

    internal static FileSystemInfo Wrap(_FileSystemInfo fileSystemInfo) =>
        fileSystemInfo is _DirectoryInfo directoryInfo ? new DirectoryInfo(directoryInfo) : new FileInfo((_FileInfo)fileSystemInfo);

    internal static FileSystemInfo? WrapOrNull(_FileSystemInfo? fileSystemInfo) =>
        fileSystemInfo == null ? null : Wrap(fileSystemInfo);

    /// <inheritdoc cref="_FileSystemInfo.CreateAsSymbolicLink(string)" />
    public void CreateAsSymbolicLink(string pathToTarget) =>
        this.fileSystemInfo.CreateAsSymbolicLink(pathToTarget);

    /// <inheritdoc cref="_FileSystemInfo.Delete" />
    public void Delete() =>
        this.fileSystemInfo.Delete();

    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void GetObjectData(SerializationInfo info, StreamingContext context) =>
        this.fileSystemInfo.GetObjectData(info, context);

    /// <inheritdoc cref="_FileSystemInfo.Refresh" />
    public void Refresh() =>
        this.fileSystemInfo.Refresh();

    /// <inheritdoc cref="_FileSystemInfo.ResolveLinkTarget(bool)" />
    public IFileSystemInfo? ResolveLinkTarget(bool returnFinalTarget) =>
        WrapOrNull(this.fileSystemInfo.ResolveLinkTarget(returnFinalTarget));
}
