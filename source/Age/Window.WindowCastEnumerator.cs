using System.Collections;

using PlatformWindow = Age.Platforms.Display.Window;

namespace Age;

public sealed partial class Window
{
    private readonly struct WindowCastEnumerator(IEnumerable<PlatformWindow> windows) : IEnumerator<Window>, IEnumerable<Window>
    {
        private readonly IEnumerator<PlatformWindow> enumerator = windows.GetEnumerator();

        public readonly Window Current => (Window)this.enumerator.Current;

        readonly object IEnumerator.Current => this.Current;

        public readonly void Dispose() =>
            this.enumerator.Dispose();

        public IEnumerator<Window> GetEnumerator() => this;

        public readonly bool MoveNext() =>
            this.enumerator.MoveNext();

        public readonly void Reset() =>
            this.enumerator.Reset();

        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}
