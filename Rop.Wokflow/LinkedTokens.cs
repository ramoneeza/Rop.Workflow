namespace Rop.Wokflow;

public class LinkedTokens : ILinkedTokens, IDisposable
{
    private IWorkflow _workflow;
    public CancellationTokenSource InternalTokenSource { get; }
    public CancellationToken InternalToken { get; }
    public CancellationToken SuperiorToken { get; }
    public CancellationTokenSource LinkedTokenSource { get; }
    public CancellationToken LinkedToken { get; }
    public void CancelLocal() => InternalTokenSource.Cancel();
    public bool IsTerminatedOrCancelled => _workflow.Status == WorkflowStatus.Terminated || (this as ILinkedTokens).IsLinkedCancelled;

    public LinkedTokens(ISubProcess subprocess)
    {
        _workflow=subprocess.Workflow;
        SuperiorToken = subprocess.Parent?.LinkedToken??_workflow.CancellationToken;
        InternalTokenSource = new CancellationTokenSource();
        InternalToken = InternalTokenSource.Token;
        LinkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(InternalToken, SuperiorToken);
        LinkedToken = LinkedTokenSource.Token;
    }
    public void Dispose()
    {
        LinkedTokenSource.Dispose();
        InternalTokenSource.Dispose();
    }
}