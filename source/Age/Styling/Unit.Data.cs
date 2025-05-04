using System.Runtime.InteropServices;

namespace Age.Styling;

public partial record struct Unit
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Data
    {
        [FieldOffset(0)]
        public int Pixel;

        [FieldOffset(0)]
        public float Percentage;

        [FieldOffset(0)]
        public float Em;
    }
}
