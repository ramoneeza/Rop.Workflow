namespace Rop.Wokflow;

public class WfLinkedTokens : ILinkedTokens
{
    private readonly IWorkflow _workflow;
    public CancellationTokenSource MainCts { get; }
    public CancellationToken InternalToken { get; }
    public CancellationToken SuperiorToken { get; }
    public CancellationToken LinkedToken { get; }

    public bool IsTerminatedOrCancelled => _workflow.Status == WorkflowStatus.Terminated || InternalToken.IsCancellationRequested;

    public void CancelLocal()
    {
        throw new NotImplementedException();
    }

    public WfLinkedTokens(IWorkflow workflow)
    {
        _workflow = workflow;
        MainCts = new CancellationTokenSource();
        InternalToken = MainCts.Token;
        SuperiorToken = MainCts.Token;
        LinkedToken = MainCts.Token;
    }

    public void Dispose()
    {
        MainCts.Dispose();
    }
}