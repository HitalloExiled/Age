using System.Diagnostics.CodeAnalysis;
using Age.Core;
using Age.Core.Extensions;

namespace Age.Platforms.Display;

public sealed partial class WindowManager : Disposable
{
    private readonly List<Window> windows = [];

    public string Id { get; }

    [AllowNull]
    public static WindowManager Instance { get; private set; }

    public ReadOnlySpan<Window> Windows => this.windows.AsSpan();

    public partial WindowManager(string id);

    protected override partial void OnDisposed(bool disposing);
}
