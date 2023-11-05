using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Versioning;

namespace Age.Platforms.Abstractions.Interfaces;

/// <summary>
/// Provides the base interface for both <see cref="IFileInfo"/> and <see cref="IDirectoryInfo"/> objects.
/// </summary>
public interface IFileSystemInfo : ISerializable
{
    /// <inheritdoc cref="FileSystemInfo.LinkTarget" />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    string? LinkTarget { get; }

    /// <inheritdoc cref="FileSystemInfo.LastWriteTimeUtc" />
    DateTime LastWriteTimeUtc { get; set; }

    /// <inheritdoc cref="FileSystemInfo.LastWriteTime" />
    DateTime LastWriteTime { get; set; }

    /// <inheritdoc cref="FileSystemInfo.LastAccessTimeUtc" />
    DateTime LastAccessTimeUtc { get; set; }

    /// <inheritdoc cref="FileSystemInfo.LastAccessTime" />
    DateTime LastAccessTime { get; set; }

    /// <inheritdoc cref="FileSystemInfo.FullName" />
    string FullName { get; }

    /// <inheritdoc cref="FileSystemInfo.Extension" />
    string Extension { get; }

    /// <inheritdoc cref="FileSystemInfo.Exists" />
    abstract bool Exists { get; }

    /// <inheritdoc cref="FileSystemInfo.CreationTime" />
    DateTime CreationTime { get; set; }

    /// <inheritdoc cref="FileSystemInfo.Name" />
    abstract string Name { get; }

    /// <inheritdoc cref="FileSystemInfo.Attributes" />
    FileAttributes Attributes { get; set; }

    /// <inheritdoc cref="FileSystemInfo.CreationTimeUtc" />
    DateTime CreationTimeUtc { get; set; }

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="FileSystemInfo.UnixFileMode" />
    [UnsupportedOSPlatform("windows")]
    UnixFileMode UnixFileMode { get; set; }
#endif

    /// <inheritdoc cref="FileSystemInfo.CreateAsSymbolicLink(string)" />
    void CreateAsSymbolicLink(string pathToTarget);

    /// <inheritdoc cref="FileSystemInfo.Delete" />
    abstract void Delete();

    /// <inheritdoc cref="FileSystemInfo.Refresh" />
    void Refresh();

    /// <inheritdoc cref="FileSystemInfo.ResolveLinkTarget(bool)" />
    IFileSystemInfo? ResolveLinkTarget(bool returnFinalTarget);
}
