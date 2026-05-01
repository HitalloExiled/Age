using System.Runtime.InteropServices;

namespace Age.Core.Extensions;

public static partial class Extension
{
    extension(Marshal)
    {
        public unsafe static int GetAlignment<T>() where T : unmanaged =>
            GetAlignment(sizeof(T));

        public static int GetAlignment(int stride) =>
            (stride & 7) == 0
                ? 8
                : (stride & 3) == 0
                    ? 4
                    : (stride & 1) == 0
                        ? 2
                        : 1;

        public static int RoundToAlignment(int stride, int alignment) =>
            alignment switch
            {
                1 => stride,
                2 => ((stride + 1) >> 1) * 2,
                4 => ((stride + 3) >> 2) * 4,
                8 => ((stride + 7) >> 3) * 8,
                _ => throw new InvalidOperationException($"Invalid Alignment: {alignment}"),
            };
    }
}
