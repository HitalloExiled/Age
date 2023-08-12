using Age.Platform.Windows.Display;

Console.WriteLine("Here we go again!");

using var windowManager = new WindowManager();

var window = windowManager.CreateWindow("Window1", 600, 400, 0, 0);

window.Show();

ThreadPool.QueueUserWorkItem(
    async (object? state) =>
    {
        await Task.Delay(5000);

        window.Minimize();

        await Task.Delay(5000);

        window.Restore();
    }
);

while (!window.Closed)
{
    window.DoEvents();
}
