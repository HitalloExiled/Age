using Age.Core.Collections;

namespace Age.Core;

public static class FileReader
{
    public static NativeArray<byte> ReadAllBytes(string path)
    {
        NativeArray<byte>? buffer = null;

        try
        {
            using var stream = File.OpenRead(path);

            buffer = new NativeArray<byte>((int)stream.Length);

            stream.ReadExactly(buffer);

            return buffer;
        }
        catch (Exception)
        {
            buffer?.Dispose();

            throw;
        }
    }

    public static RefArray<byte> ReadAllBytesAsRef(string path)
    {
        RefArray<byte> buffer = default;

        try
        {
            using var stream = File.OpenRead(path);

            buffer = new RefArray<byte>((int)stream.Length);

            stream.ReadExactly(buffer);

            return buffer;
        }
        catch (Exception)
        {
            buffer.Dispose();

            throw;
        }
    }
}
