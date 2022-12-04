using System.Text;

namespace Age.Extensions;

public static class StringExtensions
{
    public static unsafe byte[] ToUTF8Bytes(this string source) => Encoding.UTF8.GetBytes(source);
}
