using Age.Core.Error;
using Age.Core.OS;

namespace Age.Platforms.Windows;

#pragma warning disable IDE0060 // Todo - Remove

internal class AgeWindows
{
    // platform\windows\godot_windows.cpp[191:231] - Todo Analyze
    private static OS os = null!;

    public static int Initialize(string[] args)
    {
        os = new OSWindows();
        // platform\windows\godot_windows.cpp[154:162] - Todo

        var error = Main.Main.Setup(args, true);

        if (error != ErrorType.OK)
        {
            if (error == ErrorType.ERR_HELP)
            {
                return 0;
            }

            return 255;
        }

        if (Main.Main.Start())
        {
            os.Run();
        }

        Main.Main.Cleanup();

        return OSWindows.Singleton.ExitCode;
    }
}
