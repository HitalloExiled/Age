using Age.Core.Collections;

namespace Age.Core;

public static class FileReader
{
    public static NativeArray<byte> ReadAllBytes(string path)
    {
        NativeArray<byte> buffer = default;

        try
        {
            using var stream = File.OpenRead(path);

            buffer = new NativeArray<byte>((int)stream.Length);

            stream.ReadExactly(buffer.AsSpan());

            return buffer;
        }
        catch (Exception)
        {
            buffer.Dispose();

            throw;
        }
    }
}
