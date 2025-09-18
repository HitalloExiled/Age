using System.Diagnostics;

namespace Age.Scene;

public sealed class Timer : Renderable
{
    public event Action? Timeout;
    private long timestamp;

    public override string NodeName => nameof(Time);

    public bool Running { get; private set; }

    public bool     OneShot  { get; set; }
    public TimeSpan WaitTime { get; set; }

    public Timer() =>
        this.NodeFlags = NodeFlags.IgnoreUpdates;

    private void UpdateTimestamp() =>
        this.timestamp = Stopwatch.GetTimestamp();

    protected override void OnConnected()
    {
        base.OnConnected();

        this.Window!.Tree.Timers.Add(this);

        this.UpdateTimestamp();
    }

    protected override void OnDisconnected()
    {
        base.OnDisconnected();

        this.Window!.Tree.Timers.Remove(this);
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
