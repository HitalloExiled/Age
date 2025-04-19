namespace Age.Core;

public static class FileReader
{
    public static NativeArray<byte> ReadAllBytes(string path)
    {
        using var stream = File.OpenRead(path);

        var buffer = new NativeArray<byte>((int)stream.Length);

        stream.ReadExactly(buffer);

        return buffer;
    }

    public static RefArray<byte> ReadAllBytesAsRef(string path)
    {
        using var stream = File.OpenRead(path);

        var buffer = new RefArray<byte>((int)stream.Length);

        stream.ReadExactly(buffer);

        return buffer;
    }
}
