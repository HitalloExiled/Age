using System.Runtime.CompilerServices;

namespace ThirdParty.Tests;

public static class Extension
{
    extension(Path)
    {
        private static string GetFileLocation([CallerFilePath] string? callerFilePath = default) =>
            callerFilePath!;

        public static string RootLocation => Path.GetDirectoryName(GetFileLocation())!;
    }
}
