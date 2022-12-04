using System.Runtime.CompilerServices;
using Age.Core.IO;

namespace Age.Core.Error;

internal record ErrorHandlerList(object UserData, ErrorMacro.ErrorHandlerFunc ErrorHandler);

internal static class ErrorMacro
{
    public delegate void ErrorHandlerFunc(object userData, string function, string file, int line, string error, string message, bool editorNotify, ErrorHandlerType type);

    public enum ErrorHandlerType
    {
        ERR_HANDLER_ERROR,
        ERR_HANDLER_WARNING,
        ERR_HANDLER_SCRIPT,
        ERR_HANDLER_SHADER,
    };

    private static readonly LinkedList<ErrorHandlerList> errorHandlerList = new();
    private static readonly object locker = new();

    private static void ErrPrintError(string function, string file, int line, string error, string message = "") =>
        ErrPrintError(function, file, line, error, message, default, default);

    private static void ErrPrintError(string function, string file, int line, string error, string message, bool editorNotify, ErrorHandlerType type)
    {
        if (OS.OS.Singleton != null)
        {
            OS.OS.Singleton.PrintError(function, file, line, error, message, editorNotify, (Logger.ErrorType)type);
        }
        else
        {
            var details = !string.IsNullOrEmpty(message) ? message : error;

            Console.WriteLine($"ERROR: {details}\n   at: {function} ({file}:{line})\n");
        }

        lock(locker)
        {
            foreach (var item in errorHandlerList)
            {
                item.ErrorHandler.Invoke(item.UserData, function, file, line, error, message, editorNotify, type);
            }
        }
    }

    private static void ErrPrintIndexError(string function, string file, int line, long index, long size, string message = "", bool fatal = false)
    {
        var err = $"{(fatal ? "FATAL: " : "")} + Index {index} = {index} is out of bounds ({size} = {size}).";

        ErrPrintError(function, file, line, err, message);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool ERR_FAIL_COND(
        bool condition,
        string message = "",
        [CallerArgumentExpression("condition")] string expression = "",
        [CallerMemberName]                      string caller     = "",
        [CallerFilePath]                        string file       = "",
        [CallerLineNumber]                      int    line       = 0
    )
    {
        if (condition)
        {
            ErrPrintError(caller, file, line, $"Condition '{expression}' is true.", message);

            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static T ERR_FAIL_V<T>(
        T                         value,
        string                    message = "",
        [CallerMemberName] string caller  = "",
        [CallerFilePath]   string file    = "",
        [CallerLineNumber] int    line    = 0
    )
    {
        ErrPrintError(caller, file, line, $"Method/function failed. Returning: {value}", message);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_COND(
        bool condition,
        [CallerArgumentExpression("condition")] string expression = "",
        [CallerMemberName]                      string caller     = "",
        [CallerFilePath]                        string file       = "",
        [CallerLineNumber]                      int    line       = 0
    ) => ERR_FAIL_COND(condition, "", expression, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_COND_MSG(
        bool condition,
        string message,
        [CallerArgumentExpression("condition")] string expression = "",
        [CallerMemberName]                      string caller     = "",
        [CallerFilePath]                        string file       = "",
        [CallerLineNumber]                      int    line       = 0
    ) => ERR_FAIL_COND(condition, message, expression, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_COND_V(
        bool condition,
        [CallerArgumentExpression("condition")] string expression = "",
        [CallerMemberName]                      string caller     = "",
        [CallerFilePath]                        string file       = "",
        [CallerLineNumber]                      int    line       = 0
    ) => ERR_FAIL_COND(condition, "", expression, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_COND_V_MSG(
        bool condition,
        string message,
        [CallerArgumentExpression("condition")] string expression = "",
        [CallerMemberName]                      string caller     = "",
        [CallerFilePath]                        string file       = "",
        [CallerLineNumber]                      int    line       = 0
    ) => ERR_FAIL_COND(condition, message, expression, caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool ERR_FAIL_INDEX_V(
        int index,
        int size,
        [CallerMemberName] string caller = "",
        [CallerFilePath]   string file   = "",
        [CallerLineNumber] int    line   = 0
    )
    {
        if (index < 0 || index >= size)
        {
            ErrPrintIndexError(caller, file, line, index, size);

            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T ERR_FAIL_V<T>(
        T                         value,
        [CallerMemberName] string caller = "",
        [CallerFilePath]   string file   = "",
        [CallerLineNumber] int    line   = 0
    ) => ERR_FAIL_V(value, "", caller, file, line);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T ERR_FAIL_V_MSG<T>(
        T                                              value,
        string                                         message,
        [CallerMemberName]                      string caller     = "",
        [CallerFilePath]                        string file       = "",
        [CallerLineNumber]                      int    line       = 0
    ) => ERR_FAIL_V(value, message, caller, file, line);

    public static void AddErrorHandler(ErrorHandlerList item)
    {
        lock(locker)
        {
            if (errorHandlerList.Contains(item))
            {
                errorHandlerList.Remove(item);
            }

            errorHandlerList.AddLast(item);
        }
    }
}
