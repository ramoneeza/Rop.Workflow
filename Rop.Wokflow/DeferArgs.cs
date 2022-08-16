namespace Rop.Wokflow;

public class DeferArgs : IDeferArgs
{
    public IWorkflow Workflow { get; }
    public string OperationName { get; }
    public object? Argument { get; set; }
    public void DeferCompleted()
    {
        IsCompleted = true;
        Signal.Set();
    }
    public bool IsCompleted { get; private set; }

    internal AutoResetEvent Signal { get; }
    public DeferArgs(IWorkflow wf, string deferType,object? argument)
    {
        Workflow = wf;
        OperationName = deferType;
        Argument=argument;
        Signal = new AutoResetEvent(false);
        IsCompleted = false;
    }
}