namespace Rop.Wokflow;

public interface IDeferArgs
{
    IWorkflow Workflow { get; }
    string OperationName { get; }
    object? Argument { get; set; }
    void DeferCompleted();
}