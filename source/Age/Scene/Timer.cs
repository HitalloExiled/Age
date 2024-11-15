using System.Diagnostics;

namespace Age.Scene;

public sealed class Timer : Node
{
    public event Action? Timeout;
    private long timestamp;

    public override string NodeName { get; } = nameof(Time);

    public TimeSpan WaitTime { get; set; }

    public bool Running { get; private set; }
    public bool OneShot { get; set; }

    public Timer() =>
        this.Flags = NodeFlags.IgnoreUpdates;

    private void UpdateTimestamp() =>
        this.timestamp = Stopwatch.GetTimestamp();

    protected override void Connected(NodeTree tree)
    {
        base.Connected(tree);

        tree.Timers.Add(this);

        this.UpdateTimestamp();
    }

    protected override void Disconnected(NodeTree tree)
    {
        base.Disconnected(tree);

        tree.Timers.Remove(this);
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
