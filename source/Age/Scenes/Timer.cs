using System.Diagnostics;

namespace Age.Scenes;

public sealed class Timer : Node
{
    public event Action? Timeout;
    private long timestamp;

    public override string NodeName => nameof(Time);

    public bool Running { get; private set; }

    public bool     OneShot  { get; set; }
    public TimeSpan WaitTime { get; set; }

    public Timer() =>
        this.SuspendUpdates();

    private void UpdateTimestamp() =>
        this.timestamp = Stopwatch.GetTimestamp();

    private protected override void OnConnectedInternal()
    {
        base.OnConnectedInternal();

        this.Scene!.Viewport!.Window!.RenderTree.Timers.Add(this);

        this.UpdateTimestamp();
    }

    private protected override void OnDisconnectingInternal()
    {
        base.OnDisconnectingInternal();

        this.Scene!.Viewport!.Window!.RenderTree.Timers.Remove(this);
    }

    public void Start()
    {
        this.Running = true;

        this.UpdateTimestamp();
    }

    public void Stop() =>
        this.Running = false;

    public override void Update()
    {
        base.Update();

        if (this.Running && Stopwatch.GetElapsedTime(this.timestamp) >= this.WaitTime)
        {
            this.Timeout?.Invoke();

            if (this.OneShot)
            {
                this.Stop();
            }
            else
            {
                this.UpdateTimestamp();
            }
        }
    }
}
