public class RunData
{
    public bool IsRunning { get; private set; }

    public void StartRun()
    {
        Reset();
        IsRunning = true;
    }
    public void Reset()
    {
        IsRunning = false;
    }
}
