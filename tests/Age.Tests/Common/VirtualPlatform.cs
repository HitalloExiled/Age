using System.Text;
using Age.Platform.Abstractions.Interfaces;

using FileStream = Age.Platform.Abstractions.FileStream;
using Directory  = System.IO.Directory;

namespace Age.Tests.Common;

public record VirtualFile
{
    public string Name    { get; }
    public byte[] Content { get; }

    public VirtualFile(string name, byte[] content)
    {
        this.Name    = name;
        this.Content = content;
    }

    public VirtualFile(string name, string content)
    {
        this.Name    = name;
        this.Content = Encoding.UTF8.GetBytes(content);
    }
}

public record VirtualDirectory(string Path, VirtualDirectory[]? Directories = null, VirtualFile[]? Files = null);

public record FileEntry()
{
    public required IFileInfo      Info   { get; init; }
    public required string         Name   { get; init; }
    public required DirectoryEntry Parent { get; init; }
    public required string         Path   { get; init; }
}

public record DirectoryEntry()
{
    private DirectoryEntry? parent;

    public List<DirectoryEntry> Directories { get; } = new();
    public List<FileEntry>      Files       { get; } = new();

    public required IDirectoryInfo Info { get; init; }

    public required DirectoryEntry? Parent
    {
        get => this.parent;
        init
        {
            this.parent = value;
            this.parent?.Directories.Add(this);
        }
    }

    public required string Path { get; init; }
}

public class VirtualFileSystemStream(Stream stream, string path, bool isAsync) : FileStream(stream, path, isAsync)
{
    public VirtualFileSystemStream(byte[] content, string path, bool isAsync) : this(new MemoryStream(content), path, isAsync)
    { }
}

public class VirtualPlatform
{
    private readonly Mock<IFileSystem> fileSystemMock = new();
    private readonly Dictionary<string, DirectoryEntry> directories = new();

    public IFileSystem FileSystem => this.fileSystemMock.Object;

    public VirtualPlatform()
    {
        var directoryMock = new Mock<IDirectory>();
        var fileMock      = new Mock<IFile>();

        directoryMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);

        fileMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
        fileMock.Setup(x => x.Open(It.IsAny<string>(), It.IsAny<FileMode>())).Throws(new FileNotFoundException());
        fileMock.Setup(x => x.OpenRead(It.IsAny<string>())).Throws(new FileNotFoundException());

        this.fileSystemMock.Setup(x => x.Directory).Returns(directoryMock.Object);
        this.fileSystemMock.Setup(x => x.File).Returns(fileMock.Object);
    }

    private void ConfigureMock(Mock<IDirectoryInfo> mock, DirectoryEntry entry)
    {
        mock.Setup(x => x.Exists).Returns(true);
        mock.Setup(x => x.FullName).Returns(entry.Path);
        mock.Setup(x => x.EnumerateDirectories()).Returns(entry.Directories.Select(x => x.Info));
        mock.Setup(x => x.EnumerateFiles()).Returns(entry.Files.Select(x => x.Info));

        this.fileSystemMock.Setup(x => x.GetDirectoryInfo(entry.Path)).Returns(mock.Object);
        this.fileSystemMock.Setup(x => x.Directory.GetDirectories(entry.Path)).Returns(() => entry.Directories.Select(x => x.Info.Name).ToArray());
        this.fileSystemMock.Setup(x => x.Directory.GetFiles(entry.Path)).Returns(() => entry.Files.Select(x => x.Info.Name).ToArray());
    }

    private DirectoryEntry ResolveFileTree(string path)
    {
        if (!this.directories.TryGetValue(path, out var entry))
        {
            var parentEntry = Path.GetDirectoryName(path) is string parent ? this.ResolveFileTree(parent) : null;

            var mock = new Mock<IDirectoryInfo>();

            this.directories[path] = entry = new()
            {
                Path   = path,
                Parent = parentEntry,
                Info   = mock.Object,
            };

            this.ConfigureMock(mock, entry);
        }

        return entry;
    }

    private DirectoryEntry SetupVirtualDirectory(VirtualDirectory directory, DirectoryEntry? parent = null)
    {
        var mock = new Mock<IDirectoryInfo>();

        var path = Path.IsPathRooted(directory.Path) ? Path.GetFullPath(directory.Path) : Path.Join(parent?.Path ?? "/", directory.Path);

        var entry = new DirectoryEntry()
        {
            Path   = path,
            Parent = parent,
            Info   = mock.Object,
        };

        if (directory.Directories != null)
        {
            foreach (var vd in directory.Directories)
            {
                entry.Directories.Add(this.SetupVirtualDirectory(vd, entry));
            }
        }

        if (directory.Files != null)
        {
            foreach (var vf in directory.Files)
            {
                entry.Files.Add(this.SetupVirtualFile(vf, entry));
            }
        }

        this.ConfigureMock(mock, entry);

        return entry;
    }

    private FileEntry SetupVirtualFile(VirtualFile file, DirectoryEntry directory)
    {
        var fileInfoMock = new Mock<IFileInfo>();

        var entry = new FileEntry()
        {
            Path   = Path.Join(directory.Path, file.Name),
            Name   = file.Name,
            Parent = directory,
            Info   = fileInfoMock.Object,
        };

        fileInfoMock.Setup(x => x.Exists).Returns(true);
        fileInfoMock.Setup(x => x.Directory).Returns(directory.Info);
        fileInfoMock.Setup(x => x.DirectoryName).Returns(directory.Info.Name);

        var stream = new VirtualFileSystemStream(new MemoryStream(file.Content), entry.Path, false);

        this.fileSystemMock.Setup(x => x.File.Exists(entry.Path)).Returns(true);
        this.fileSystemMock.Setup(x => x.File.Open(entry.Path, It.IsAny<FileMode>())).Returns(stream);
        this.fileSystemMock.Setup(x => x.File.OpenRead(entry.Path)).Returns(stream);

        return entry;
    }

    public void Setup(VirtualDirectory directory, string? parent = null)
    {
        parent ??= Directory.GetCurrentDirectory();

        var parentEntry = this.ResolveFileTree(parent);

        this.SetupVirtualDirectory(directory, parentEntry);
    }
}
