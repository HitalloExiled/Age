using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Age.Platforms.Abstractions.Interfaces;

/// <summary>
/// Provides methods for the creation, copying, deletion, moving, and opening of a single file, and aids in the creation of FileStream objects.
/// </summary>
public interface IFile
{
    /// <inheritdoc cref="File.AppendAllLines(string, IEnumerable{string})" />
    void AppendAllLines(string path, IEnumerable<string> contents);

    /// <inheritdoc cref="File.AppendAllLines(string, IEnumerable{string}, Encoding)" />
    void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding);

    /// <inheritdoc cref="File.AppendAllLinesAsync(string, IEnumerable{string}, CancellationToken)" />
    Task AppendAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.AppendAllLinesAsync(string, IEnumerable{string}, Encoding, CancellationToken)" />
    Task AppendAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.AppendAllText(string, string?)" />
    void AppendAllText(string path, string? contents);

    /// <inheritdoc cref="File.AppendAllText(string, string?, Encoding)" />
    void AppendAllText(string path, string? contents, Encoding encoding);

    /// <inheritdoc cref="File.AppendAllTextAsync(string, string?, CancellationToken)" />
    Task AppendAllTextAsync(string path, string? contents, Encoding encoding, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.AppendAllTextAsync(string, string?, Encoding, CancellationToken)" />
    Task AppendAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.AppendText(string)" />
    StreamWriter AppendText(string path);

    /// <inheritdoc cref="File.Copy(string, string)" />
    void Copy(string sourceFileName, string destFileName);

    /// <inheritdoc cref="File.Copy(string, string, bool)" />
    void Copy(string sourceFileName, string destFileName, bool overwrite);

    /// <inheritdoc cref="File.Create(string)" />
    FileStream Create(string path);

    /// <inheritdoc cref="File.Create(string, int)" />
    FileStream Create(string path, int bufferSize);

    /// <inheritdoc cref="File.Create(string, int, FileOptions)" />
    FileStream Create(string path, int bufferSize, FileOptions options);

    /// <inheritdoc cref="File.CreateSymbolicLink(string, string)" />
    IFileSystemInfo CreateSymbolicLink(string path, string pathToTarget);

    /// <inheritdoc cref="File.CreateText(string)" />
    StreamWriter CreateText(string path);

    /// <inheritdoc cref="File.Decrypt(string)" />
    [SupportedOSPlatform("windows")]
    void Decrypt(string path);

    /// <inheritdoc cref="File.Delete(string)" />
    void Delete(string path);

    /// <inheritdoc cref="File.Encrypt(string)" />
    [SupportedOSPlatform("windows")]
    void Encrypt(string path);

    /// <inheritdoc cref="File.Exists(string?)" />
    bool Exists([NotNullWhen(true)] string? path);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.GetAttributes(SafeFileHandle)" />
    FileAttributes GetAttributes(SafeFileHandle fileHandle);
#endif

    /// <inheritdoc cref="File.GetAttributes(string)" />
    FileAttributes GetAttributes(string path);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.GetCreationTime(SafeFileHandle)" />
    DateTime GetCreationTime(SafeFileHandle fileHandle);
#endif

    /// <inheritdoc cref="File.GetCreationTime(string)" />
    DateTime GetCreationTime(string path);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.GetCreationTimeUtc(SafeFileHandle)" />
    DateTime GetCreationTimeUtc(SafeFileHandle fileHandle);
#endif

    /// <inheritdoc cref="File.GetCreationTimeUtc(string)" />
    DateTime GetCreationTimeUtc(string path);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.GetLastAccessTime(SafeFileHandle)" />
    DateTime GetLastAccessTime(SafeFileHandle fileHandle);
#endif

    /// <inheritdoc cref="File.GetLastAccessTime(string)" />
    DateTime GetLastAccessTime(string path);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.GetLastAccessTimeUtc(SafeFileHandle)" />
    DateTime GetLastAccessTimeUtc(SafeFileHandle fileHandle);
#endif

    /// <inheritdoc cref="File.GetLastAccessTimeUtc(string)" />
    DateTime GetLastAccessTimeUtc(string path);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.GetLastWriteTime(SafeFileHandle)" />
    DateTime GetLastWriteTime(SafeFileHandle fileHandle);
#endif

    /// <inheritdoc cref="File.GetLastAccessTime(string)" />
    DateTime GetLastWriteTime(string path);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.GetLastWriteTimeUtc(SafeFileHandle)" />
    DateTime GetLastWriteTimeUtc(SafeFileHandle fileHandle);
#endif

    /// <inheritdoc cref="File.GetLastWriteTimeUtc(string)" />
    DateTime GetLastWriteTimeUtc(string path);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.GetUnixFileMode(SafeFileHandle)" />
    [UnsupportedOSPlatform("windows")]
    UnixFileMode GetUnixFileMode(SafeFileHandle fileHandle);

    /// <inheritdoc cref="File.GetUnixFileMode(string)" />
    [UnsupportedOSPlatform("windows")]
    UnixFileMode GetUnixFileMode(string path);
#endif

    /// <inheritdoc cref="File.Move(string, string)" />
    void Move(string sourceFileName, string destFileName);

    /// <inheritdoc cref="File.Move(string, string, bool)" />
    void Move(string sourceFileName, string destFileName, bool overwrite);

    /// <inheritdoc cref="File.Open(string, FileMode)" />
    FileStream Open(string path, FileMode mode);

    /// <inheritdoc cref="File.Open(string, FileMode, FileAccess)" />
    FileStream Open(string path, FileMode mode, FileAccess access);

    /// <inheritdoc cref="File.Open(string, FileMode, FileAccess, FileShare)" />
    FileStream Open(string path, FileMode mode, FileAccess access, FileShare share);

    /// <inheritdoc cref="File.Open(string, FileStreamOptions)" />
    FileStream Open(string path, FileStreamOptions options);

    /// <inheritdoc cref="File.OpenHandle(string, FileMode, FileAccess, FileShare, FileOptions, long)" />
    SafeFileHandle OpenHandle(string path, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read, FileOptions options = FileOptions.None, long preallocationSize = 0L);

    /// <inheritdoc cref="File.OpenRead(string)" />
    FileStream OpenRead(string path);

    /// <inheritdoc cref="File.OpenText(string)" />
    StreamReader OpenText(string path);

    /// <inheritdoc cref="File.OpenWrite(string)" />
    FileStream OpenWrite(string path);

    /// <inheritdoc cref="File.ReadAllBytes(string)" />
    byte[] ReadAllBytes(string path);

    /// <inheritdoc cref="File.ReadAllBytesAsync(string, CancellationToken)" />
    Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.ReadAllLines(string)" />
    string[] ReadAllLines(string path);

    /// <inheritdoc cref="File.ReadAllLines(string, Encoding)" />
    string[] ReadAllLines(string path, Encoding encoding);

    /// <inheritdoc cref="File.ReadAllLinesAsync(string, Encoding, CancellationToken)" />
    Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.ReadAllLinesAsync(string, CancellationToken)" />
    Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.ReadAllText(string)" />
    string ReadAllText(string path);

    /// <inheritdoc cref="File.ReadAllText(string, Encoding)" />
    string ReadAllText(string path, Encoding encoding);

    /// <inheritdoc cref="File.ReadAllTextAsync(string, Encoding, CancellationToken)" />
    Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.ReadAllTextAsync(string, CancellationToken)" />
    Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.ReadLines(string)" />
    IEnumerable<string> ReadLines(string path);

    /// <inheritdoc cref="File.ReadLines(string, Encoding)" />
    IEnumerable<string> ReadLines(string path, Encoding encoding);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.ReadLinesAsync(string, CancellationToken)" />
    IAsyncEnumerable<string> ReadLinesAsync(string path, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.ReadLinesAsync(string, Encoding, CancellationToken)" />
    IAsyncEnumerable<string> ReadLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default);
#endif

    /// <inheritdoc cref="File.Replace(string, string, string?)" />
    void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName);

    /// <inheritdoc cref="File.Replace(string, string, string?, bool)" />
    void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName, bool ignoreMetadataErrors);

    /// <inheritdoc cref="File.ResolveLinkTarget(string, bool)" />
    IFileSystemInfo? ResolveLinkTarget(string linkPath, bool returnFinalTarget);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.SetAttributes(SafeFileHandle, FileAttributes)" />
    void SetAttributes(SafeFileHandle fileHandle, FileAttributes fileAttributes);
#endif

    /// <inheritdoc cref="File.SetAttributes(string, FileAttributes)" />
    void SetAttributes(string path, FileAttributes fileAttributes);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.SetCreationTime(SafeFileHandle, DateTime)" />
    void SetCreationTime(SafeFileHandle fileHandle, DateTime creationTime);
#endif

    /// <inheritdoc cref="File.SetCreationTime(string, DateTime)" />
    void SetCreationTime(string path, DateTime creationTime);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.SetCreationTimeUtc(SafeFileHandle, DateTime)" />
    void SetCreationTimeUtc(SafeFileHandle fileHandle, DateTime creationTimeUtc);
#endif

    /// <inheritdoc cref="File.SetCreationTimeUtc(string, DateTime)" />
    void SetCreationTimeUtc(string path, DateTime creationTimeUtc);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.SetLastAccessTime(SafeFileHandle, DateTime)" />
    void SetLastAccessTime(SafeFileHandle fileHandle, DateTime lastAccessTime);
#endif

    /// <inheritdoc cref="File.SetLastAccessTime(string, DateTime)" />
    void SetLastAccessTime(string path, DateTime lastAccessTime);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.SetLastAccessTime(SafeFileHandle, DateTime)" />
    void SetLastAccessTimeUtc(SafeFileHandle fileHandle, DateTime lastAccessTimeUtc);
#endif

    /// <inheritdoc cref="File.SetLastAccessTimeUtc(string, DateTime)" />
    void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.SetLastWriteTime(SafeFileHandle, DateTime)" />
    void SetLastWriteTime(SafeFileHandle fileHandle, DateTime lastWriteTime);
#endif

    /// <inheritdoc cref="File.SetLastWriteTime(string, DateTime)" />
    void SetLastWriteTime(string path, DateTime lastWriteTime);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.SetLastWriteTimeUtc(SafeFileHandle, DateTime)" />
    void SetLastWriteTimeUtc(SafeFileHandle fileHandle, DateTime lastWriteTimeUtc);
#endif

    /// <inheritdoc cref="File.SetLastWriteTimeUtc(string, DateTime)" />
    void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);

#if NET7_0_OR_GREATER
    /// <inheritdoc cref="File.SetUnixFileMode(SafeFileHandle, UnixFileMode)" />
    [UnsupportedOSPlatform("windows")]
    void SetUnixFileMode(SafeFileHandle fileHandle, UnixFileMode mode);

    /// <inheritdoc cref="File.SetUnixFileMode(string, UnixFileMode)" />
    [UnsupportedOSPlatform("windows")]
    void SetUnixFileMode(string path, UnixFileMode mode);
#endif

    /// <inheritdoc cref="File.WriteAllBytes(string, byte[])" />
    void WriteAllBytes(string path, byte[] bytes);

    /// <inheritdoc cref="File.WriteAllBytesAsync(string, byte[], CancellationToken)" />
    Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.WriteAllLines(string, IEnumerable{string})" />
    void WriteAllLines(string path, IEnumerable<string> contents);

    /// <inheritdoc cref="File.WriteAllLines(string, IEnumerable{string}, Encoding)" />
    void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding);

    /// <inheritdoc cref="File.WriteAllLines(string, string[])" />
    void WriteAllLines(string path, string[] contents);

    /// <inheritdoc cref="File.WriteAllLines(string, string[], Encoding)" />
    void WriteAllLines(string path, string[] contents, Encoding encoding);

    /// <inheritdoc cref="File.WriteAllTextAsync(string, string?, Encoding, CancellationToken)" />
    Task WriteAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.WriteAllLinesAsync(string, IEnumerable{string}, CancellationToken)" />
    Task WriteAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.WriteAllText(string, string?)" />
    void WriteAllText(string path, string? contents);

    /// <inheritdoc cref="File.WriteAllText(string, string?, Encoding)" />
    void WriteAllText(string path, string? contents, Encoding encoding);

    /// <inheritdoc cref="File.WriteAllTextAsync(string, string?, Encoding, CancellationToken)" />
    Task WriteAllTextAsync(string path, string? contents, Encoding encoding, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="File.WriteAllTextAsync(string, string?, CancellationToken)" />
    Task WriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default);
}