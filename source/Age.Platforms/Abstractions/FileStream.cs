using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;

using _FileStream = System.IO.FileStream;

namespace Age.Platforms.Abstractions;

#pragma warning disable CS0618

/// <inheritdoc cref="_FileStream" />
[ExcludeFromCodeCoverage]
public class FileStream : Stream
{
    private readonly nint           handle;
    private readonly bool           isAsync;
    private readonly string         name;
    private readonly SafeFileHandle safeFileHandle;
    private readonly Stream         stream;

    private bool disposed;

    /// <inheritdoc />
    public override bool CanRead => this.stream.CanRead;

    /// <inheritdoc />
    public override bool CanSeek => this.stream.CanSeek;

    /// <inheritdoc />
    public override bool CanWrite => this.stream.CanWrite;

    [Obsolete("FileStream.Handle has been deprecated. Use FileStream's SafeFileHandle property instead.")]
    public virtual IntPtr Handle => this.handle;

    /// <inheritdoc cref="_FileStream.IsAsync" />
    public virtual bool IsAsync => this.isAsync;

    /// <inheritdoc />
    public override long Length => this.stream.Length;

    /// <inheritdoc cref="_FileStream.Name" />
    public virtual string Name => this.name;

    /// <inheritdoc />
    public override long Position
    {
        get => this.stream.Position;
        set => this.stream.Position = value;
    }

    /// <inheritdoc cref="_FileStream.SafeFileHandle" />
    public virtual SafeFileHandle SafeFileHandle => this.safeFileHandle;

    /// <inheritdoc cref="_FileStream(SafeFileHandle, FileAccess)" />
    public FileStream(_FileStream fileStream)
    {
        this.handle         = fileStream.Handle;
        this.isAsync        = fileStream.IsAsync;
        this.name           = fileStream.Name;
        this.safeFileHandle = fileStream.SafeFileHandle;
        this.stream         = fileStream;
    }

    public FileStream(Stream stream, string name, bool isAsync)
    {
        this.handle         = default;
        this.isAsync        = isAsync;
        this.name           = name;
        this.safeFileHandle = new();
        this.stream         = stream;
    }

    /// <inheritdoc cref="_FileStream(SafeFileHandle, FileAccess)" />
    public FileStream(SafeFileHandle handle, FileAccess access) : this(new _FileStream(handle, access)) { }

    /// <inheritdoc cref="_FileStream(SafeFileHandle, FileAccess, int)" />
    public FileStream(SafeFileHandle handle, FileAccess access, int bufferSize) : this(new _FileStream(handle, access, bufferSize)) { }

    /// <inheritdoc cref="_FileStream(SafeFileHandle, FileAccess, int, bool)" />
    public FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) : this(new _FileStream(handle, access, bufferSize, isAsync)) { }

    /// <inheritdoc cref="_FileStream(IntPtr, FileAccess)" />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This constructor has been deprecated. Use FileStream(SafeFileHandle handle, FileAccess access) instead.")]
    public FileStream(IntPtr handle, FileAccess access) : this(new _FileStream(handle, access)) { }

    /// <inheritdoc cref="_FileStream(IntPtr, FileAccess, bool)" />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This constructor has been deprecated. Use FileStream(SafeFileHandle handle, FileAccess access) and optionally make a new SafeFileHandle with ownsHandle=false if needed instead.")]
    public FileStream(IntPtr handle, FileAccess access, bool ownsHandle) : this(new _FileStream(handle, access)) { }

    /// <inheritdoc cref="_FileStream(IntPtr, FileAccess, bool, int)" />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This constructor has been deprecated. Use FileStream(SafeFileHandle handle, FileAccess access, int bufferSize) and optionally make a new SafeFileHandle with ownsHandle=false if needed instead.")]
    public FileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize) : this(new _FileStream(handle, access, ownsHandle, bufferSize)) { }

    /// <inheritdoc cref="_FileStream(IntPtr, FileAccess, bool, int, bool)" />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This constructor has been deprecated. Use FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) and optionally make a new SafeFileHandle with ownsHandle=false if needed instead.")]
    public FileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync) : this(new _FileStream(handle, access, ownsHandle, bufferSize)) { }

    /// <inheritdoc cref="_FileStream(string, FileMode)" />
    public FileStream(string path, FileMode mode) : this(new _FileStream(path, mode)) { }

    /// <inheritdoc cref="_FileStream(string, FileMode, FileAccess)" />
    public FileStream(string path, FileMode mode, FileAccess access) : this(new _FileStream(path, mode, access)) { }

    /// <inheritdoc cref="_FileStream(string, FileMode, FileAccess, FileShare)" />
    public FileStream(string path, FileMode mode, FileAccess access, FileShare share) : this(new _FileStream(path, mode, access, share)) { }

    /// <inheritdoc cref="_FileStream(string, FileMode, FileAccess, FileShare, int)" />
    public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize) : this(new _FileStream(path, mode, access, share, bufferSize)) { }

    /// <inheritdoc cref="_FileStream(string, FileMode FileAccess, FileShare, int, bool)" />
    public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) : this(new _FileStream(path, mode, access, share, bufferSize, useAsync)) { }

    /// <inheritdoc cref="_FileStream(string, FileMode, FileAccess, FileShare, int, FileOptions)" />
    public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options) : this(new _FileStream(path, mode, access, share, bufferSize, options)) { }

    /// <inheritdoc cref="_FileStream(string, FileStreamOptions)" />
    public FileStream(string path, FileStreamOptions options) : this(new _FileStream(path, options)) { }

    ~FileStream() =>
        this.Dispose(false);

    /// <inheritdoc cref="_FileStream.BeginRead(byte[], int, int, AsyncCallback?, object?)" />
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        this.stream.BeginRead(buffer, offset,  count, callback, state);

    /// <inheritdoc cref="_FileStream.BeginWrite(byte[], int, int, AsyncCallback?, object?)" />
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        this.stream.BeginWrite(buffer, offset, count, callback, state);

    /// <inheritdoc cref="_FileStream.CopyToAsync(Stream, int, CancellationToken)" />
    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
        this.stream.CopyToAsync(destination, bufferSize, cancellationToken);

    /// <inheritdoc cref="_FileStream.Dispose(bool)" />
    protected override void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.stream.Dispose();
            }

            this.disposed = false;
            base.Dispose(disposing);
        }
    }

    /// <inheritdoc cref="_FileStream.DisposeAsync" />
    public override async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await this.stream.DisposeAsync();

        await base.DisposeAsync();
    }

    /// <inheritdoc cref="_FileStream.EndRead(IAsyncResult)" />
    public override int EndRead(IAsyncResult asyncResult) =>
        this.stream.EndRead(asyncResult);

    /// <inheritdoc cref="_FileStream.EndWrite(IAsyncResult)" />
    public override void EndWrite(IAsyncResult asyncResult) =>
        this.stream.EndWrite(asyncResult);

    /// <inheritdoc cref="_FileStream.Flush()" />
    public override void Flush() =>
        this.stream.Flush();

    /// <inheritdoc cref="_FileStream.Flush(bool)" />
    public virtual void Flush(bool flushToDisk)
    {
        if (this.stream is _FileStream fileStream)
        {
            fileStream.Flush(flushToDisk);
        }
    }

    /// <inheritdoc cref="_FileStream.FlushAsync(CancellationToken)" />
    public override Task FlushAsync(CancellationToken cancellationToken) =>
        this.stream.FlushAsync(cancellationToken);

    /// <inheritdoc cref="_FileStream.Lock(long, long)" />
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("tvos")]
    [UnsupportedOSPlatform("freebsd")]
    public virtual void Lock(long position, long length)
    {
        if (this.stream is _FileStream fileStream)
        {
            fileStream.Lock(position, length);
        }
    }

    /// <inheritdoc cref="_FileStream.Read(byte[], int, int)" />
    public override int Read(byte[] buffer, int offset, int count) =>
        this.stream.Read(buffer, offset, count);

    /// <inheritdoc cref="_FileStream.Read(Span{byte})" />
    public override int Read(Span<byte> buffer) =>
        this.stream.Read(buffer);

    /// <inheritdoc cref="_FileStream.ReadAsync(byte[], int, int, CancellationToken)" />
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        this.stream.ReadAsync(buffer, offset, count, cancellationToken);

    /// <inheritdoc cref="_FileStream.ReadAsync(Memory{byte}, CancellationToken)" />
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        this.stream.ReadAsync(buffer, cancellationToken);

    /// <inheritdoc cref="_FileStream.ReadByte" />
    public override int ReadByte() =>
        this.stream.ReadByte();

    /// <inheritdoc cref="_FileStream.Seek(long, SeekOrigin)" />
    public override long Seek(long offset, SeekOrigin origin) =>
        this.stream.Seek(offset, origin);

    /// <inheritdoc cref="_FileStream.SetLength(long)" />
    public override void SetLength(long value) =>
        this.stream.SetLength(value);

    /// <inheritdoc cref="_FileStream.Unlock(long, long)" />
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("tvos")]
    [UnsupportedOSPlatform("freebsd")]
    public virtual void Unlock(long position, long length)
    {
        if (this.stream is _FileStream fileStream)
        {
            fileStream.Unlock(position, length);
        }
    }

    /// <inheritdoc cref="_FileStream.Write(byte[], int, int)" />
    public override void Write(byte[] buffer, int offset, int count) =>
        this.stream.Write(buffer, offset, count);

    /// <inheritdoc cref="_FileStream.Write(ReadOnlySpan{byte})" />
    public override void Write(ReadOnlySpan<byte> buffer) =>
        this.stream.Write(buffer);

    /// <inheritdoc cref="_FileStream.WriteAsync(byte[], int, int, CancellationToken)" />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        this.stream.WriteAsync(buffer, offset, count, cancellationToken);

    /// <inheritdoc cref="_FileStream.WriteAsync(ReadOnlyMemory{byte}, CancellationToken)" />
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
        this.stream.WriteAsync(buffer, cancellationToken);

    /// <inheritdoc cref="_FileStream.WriteByte(byte)" />
    public override void WriteByte(byte value) =>
        this.stream.WriteByte(value);
}
