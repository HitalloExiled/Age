namespace Age.Core.String;

internal record PrintHandlerList(object UserData, PrintString.PrintHandlerFunc ErrorHandler);

internal static class PrintString
{
    private static readonly LinkedList<PrintHandlerList> printHandlerList = new();
    private static readonly object locker                                 = new();

    public delegate void PrintHandlerFunc(object data, string content, bool error, bool rich);

    public static void PrintLine(string content)
    {
        if (!CoreGlobals.PrintLineEnabled)
        {
            return;
        }

        OS.OS.Singleton.Print($"{content}\n");

        lock(locker)
        {
            foreach (var item in printHandlerList)
            {
                item.ErrorHandler.Invoke(item.UserData, content, false, false);
            }
        }
    }
}
