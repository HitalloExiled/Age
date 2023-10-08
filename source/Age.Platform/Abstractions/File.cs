using System.Diagnostics.CodeAnalysis;
using System.Text;
using Age.Platform.Abstractions.Interfaces;
using Microsoft.Win32.SafeHandles;

using _File = System.IO.File;
using System.Runtime.Versioning;

namespace Age.Platform.Abstractions;

[ExcludeFromCodeCoverage]
internal class File : IFile
{
    public void AppendAllLines(string path, IEnumerable<string> contents) =>
        _File.AppendAllLines(path, contents);

    public void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding) =>
        _File.AppendAllLines(path, contents, encoding);

    public Task AppendAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default) =>
        _File.AppendAllLinesAsync(path, contents, cancellationToken);

    public Task AppendAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default) =>
        _File.AppendAllLinesAsync(path, contents, cancellationToken);

    public void AppendAllText(string path, string? contents) =>
        _File.AppendAllText(path, contents);

    public void AppendAllText(string path, string? contents, Encoding encoding) =>
        _File.AppendAllText(path, contents, encoding);

    public Task AppendAllTextAsync(string path, string? contents, Encoding encoding, CancellationToken cancellationToken = default) =>
        _File.AppendAllTextAsync(path, contents, encoding, cancellationToken);

    public Task AppendAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default) =>
        _File.AppendAllTextAsync(path, contents, cancellationToken);

    public StreamWriter AppendText(string path) =>
        _File.AppendText(path);

    public void Copy(string sourceFileName, string destFileName) =>
        _File.Copy(sourceFileName, destFileName);

    public void Copy(string sourceFileName, string destFileName, bool overwrite) =>
        _File.Copy(sourceFileName, destFileName, overwrite);

    public FileStream Create(string path) =>
        new(_File.Create(path));

    public FileStream Create(string path, int bufferSize) =>
        new(_File.Create(path, bufferSize));

    public FileStream Create(string path, int bufferSize, FileOptions options) =>
        new(_File.Create(path, bufferSize, options));

    public IFileSystemInfo CreateSymbolicLink(string path, string pathToTarget) =>
        FileSystemInfo.Wrap(_File.CreateSymbolicLink(path, pathToTarget));

    public StreamWriter CreateText(string path) =>
        _File.CreateText(path);

    [SupportedOSPlatform("windows")]
    public void Decrypt(string path) =>
        _File.Decrypt(path);

    public void Delete(string path) =>
        _File.Delete(path);

    [SupportedOSPlatform("windows")]
    public void Encrypt(string path) =>
        _File.Encrypt(path);

    public bool Exists([NotNullWhen(true)] string? path) =>
        _File.Exists(path);

#if NET7_0_OR_GREATER
    public FileAttributes GetAttributes(SafeFileHandle fileHandle) =>
        _File.GetAttributes(fileHandle);
#endif

    public FileAttributes GetAttributes(string path) =>
        _File.GetAttributes(path);

#if NET7_0_OR_GREATER
    public DateTime GetCreationTime(SafeFileHandle fileHandle) =>
        _File.GetCreationTime(fileHandle);
#endif

    public DateTime GetCreationTime(string path) =>
        _File.GetCreationTime(path);

#if NET7_0_OR_GREATER
    public DateTime GetCreationTimeUtc(SafeFileHandle fileHandle) =>
        _File.GetCreationTimeUtc(fileHandle);
#endif

    public DateTime GetCreationTimeUtc(string path) =>
        _File.GetCreationTimeUtc(path);

#if NET7_0_OR_GREATER
    public DateTime GetLastAccessTime(SafeFileHandle fileHandle) =>
        _File.GetLastAccessTime(fileHandle);
#endif

    public DateTime GetLastAccessTime(string path) =>
        _File.GetLastAccessTime(path);

#if NET7_0_OR_GREATER
    public DateTime GetLastAccessTimeUtc(SafeFileHandle fileHandle) =>
        _File.GetLastAccessTimeUtc(fileHandle);
#endif

    public DateTime GetLastAccessTimeUtc(string path) =>
        _File.GetLastAccessTimeUtc(path);

#if NET7_0_OR_GREATER
    public DateTime GetLastWriteTime(SafeFileHandle fileHandle) =>
        _File.GetLastWriteTime(fileHandle);
#endif

    public DateTime GetLastWriteTime(string path) =>
        _File.GetLastWriteTime(path);

#if NET7_0_OR_GREATER
    public DateTime GetLastWriteTimeUtc(SafeFileHandle fileHandle) =>
        _File.GetLastWriteTimeUtc(fileHandle);
#endif

    public DateTime GetLastWriteTimeUtc(string path) =>
        _File.GetLastWriteTimeUtc(path);

#if NET7_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    public UnixFileMode GetUnixFileMode(SafeFileHandle fileHandle) =>
        _File.GetUnixFileMode(fileHandle);

    [UnsupportedOSPlatform("windows")]
    public UnixFileMode GetUnixFileMode(string path) =>
        _File.GetUnixFileMode(path);
#endif

    public void Move(string sourceFileName, string destFileName) =>
        _File.Move(sourceFileName, destFileName);

    public void Move(string sourceFileName, string destFileName, bool overwrite) =>
        _File.Move(sourceFileName, destFileName, overwrite);

    public FileStream Open(string path, FileMode mode) =>
        new(_File.Open(path, mode));

    public FileStream Open(string path, FileMode mode, FileAccess access) =>
        new(_File.Open(path, mode, access));

    public FileStream Open(string path, FileMode mode, FileAccess access, FileShare share) =>
        new(_File.Open(path, mode, access, share));

    public FileStream Open(string path, FileStreamOptions options) =>
        new(_File.Open(path, options));

    public SafeFileHandle OpenHandle(string path, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read, FileOptions options = FileOptions.None, long preallocationSize = 0) =>
        _File.OpenHandle(path, mode, access, share, options, preallocationSize);

    public FileStream OpenRead(string path) =>
        new(_File.OpenRead(path));

    public StreamReader OpenText(string path) =>
        _File.OpenText(path);

    public FileStream OpenWrite(string path) =>
        new(_File.OpenWrite(path));

    public byte[] ReadAllBytes(string path) =>
        _File.ReadAllBytes(path);

    public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default) =>
        _File.ReadAllBytesAsync(path, cancellationToken);

    public string[] ReadAllLines(string path) =>
        _File.ReadAllLines(path);

    public string[] ReadAllLines(string path, Encoding encoding) =>
        _File.ReadAllLines(path, encoding);

    public Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default) =>
        _File.ReadAllLinesAsync(path, encoding, cancellationToken);

    public Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default) =>
        _File.ReadAllLinesAsync(path, cancellationToken);

    public string ReadAllText(string path) =>
        _File.ReadAllText(path);

    public string ReadAllText(string path, Encoding encoding) =>
        _File.ReadAllText(path, encoding);

    public Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default) =>
        _File.ReadAllTextAsync(path, encoding, cancellationToken);

    public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default) =>
        _File.ReadAllTextAsync(path, cancellationToken);

    public IEnumerable<string> ReadLines(string path) =>
        _File.ReadLines(path);

    public IEnumerable<string> ReadLines(string path, Encoding encoding) =>
        _File.ReadLines(path, encoding);

#if NET7_0_OR_GREATER
    public IAsyncEnumerable<string> ReadLinesAsync(string path, CancellationToken cancellationToken = default) =>
        _File.ReadLinesAsync(path, cancellationToken);

    public IAsyncEnumerable<string> ReadLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default) =>
        _File.ReadLinesAsync(path, encoding, cancellationToken);
#endif

    public void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName) =>
        _File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);

    public void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName, bool ignoreMetadataErrors) =>
        _File.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors);

    public IFileSystemInfo? ResolveLinkTarget(string linkPath, bool returnFinalTarget) =>
        FileSystemInfo.WrapOrNull(_File.ResolveLinkTarget(linkPath, returnFinalTarget));

#if NET7_0_OR_GREATER
    public void SetAttributes(SafeFileHandle fileHandle, FileAttributes fileAttributes) =>
        _File.SetAttributes(fileHandle, fileAttributes);
#endif

    public void SetAttributes(string path, FileAttributes fileAttributes) =>
        _File.SetAttributes(path, fileAttributes);

#if NET7_0_OR_GREATER
    public void SetCreationTime(SafeFileHandle fileHandle, DateTime creationTime) =>
        _File.SetCreationTime(fileHandle, creationTime);
#endif

    public void SetCreationTime(string path, DateTime creationTime) =>
        _File.SetCreationTime(path, creationTime);

#if NET7_0_OR_GREATER
    public void SetCreationTimeUtc(SafeFileHandle fileHandle, DateTime creationTimeUtc) =>
        _File.SetCreationTimeUtc(fileHandle, creationTimeUtc);
#endif

    public void SetCreationTimeUtc(string path, DateTime creationTimeUtc) =>
        _File.SetCreationTimeUtc(path, creationTimeUtc);

#if NET7_0_OR_GREATER
    public void SetLastAccessTime(SafeFileHandle fileHandle, DateTime lastAccessTime) =>
        _File.SetLastAccessTime(fileHandle, lastAccessTime);
#endif

    public void SetLastAccessTime(string path, DateTime lastAccessTime) =>
        _File.SetLastAccessTime(path, lastAccessTime);

#if NET7_0_OR_GREATER
    public void SetLastAccessTimeUtc(SafeFileHandle fileHandle, DateTime lastAccessTimeUtc) =>
        _File.SetLastAccessTimeUtc(fileHandle, lastAccessTimeUtc);
#endif

    public void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
        _File.SetLastAccessTimeUtc(path, lastAccessTimeUtc);

#if NET7_0_OR_GREATER
    public void SetLastWriteTime(SafeFileHandle fileHandle, DateTime lastWriteTime) =>
        _File.SetLastWriteTime(fileHandle, lastWriteTime);
#endif

    public void SetLastWriteTime(string path, DateTime lastWriteTime) =>
        _File.SetLastWriteTime(path, lastWriteTime);

#if NET7_0_OR_GREATER
    public void SetLastWriteTimeUtc(SafeFileHandle fileHandle, DateTime lastWriteTimeUtc) =>
        _File.SetLastWriteTimeUtc(fileHandle, lastWriteTimeUtc);
#endif

    public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
        _File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);

#if NET7_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    public void SetUnixFileMode(SafeFileHandle fileHandle, UnixFileMode mode) =>
        _File.SetUnixFileMode(fileHandle, mode);

    [UnsupportedOSPlatform("windows")]
    public void SetUnixFileMode(string path, UnixFileMode mode) =>
        _File.SetUnixFileMode(path, mode);
#endif

    public void WriteAllBytes(string path, byte[] bytes) =>
        _File.WriteAllBytes(path, bytes);

    public Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default) =>
        _File.WriteAllBytesAsync(path, bytes, cancellationToken);

    public void WriteAllLines(string path, IEnumerable<string> contents) =>
        _File.WriteAllLines(path, contents);

    public void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding) =>
        _File.WriteAllLines(path, contents, encoding);

    public void WriteAllLines(string path, string[] contents) =>
        _File.WriteAllLines(path, contents);

    public void WriteAllLines(string path, string[] contents, Encoding encoding) =>
        _File.WriteAllLines(path, contents, encoding);

    public Task WriteAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default) =>
        _File.WriteAllLinesAsync(path, contents, encoding, cancellationToken);

    public Task WriteAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default) =>
        _File.WriteAllLinesAsync(path, contents, cancellationToken);

    public void WriteAllText(string path, string? contents) =>
        _File.WriteAllText(path, contents);

    public void WriteAllText(string path, string? contents, Encoding encoding) =>
        _File.WriteAllText(path, contents, encoding);

    public Task WriteAllTextAsync(string path, string? contents, Encoding encoding, CancellationToken cancellationToken = default) =>
        _File.WriteAllTextAsync(path, contents, cancellationToken);

    public Task WriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default) =>
        _File.WriteAllTextAsync(path, contents, cancellationToken);

}
