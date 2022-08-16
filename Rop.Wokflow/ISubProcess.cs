using Rop.Wokflow.NextCases;

namespace Rop.Wokflow;

public interface ISubProcess:IDisposable
{
    BaseWorkflow Workflow { get; }
    ISubProcess? Parent { get; }
    bool Finished { get; }
    NextStatus? NextResult { get; }
    void ProgressSerialization();
    Task<NextStatus> Run();
    // CascadeTokens
    ILinkedTokens LinkedTokens { get; }
    public bool IsCanceledOrEnded => Workflow.Finished || LinkedTokens.IsLinkedCancelled;
    public bool IsLocalCanceled => LinkedTokens.IsLocalCancelled;
    public bool IsLinkedCancelled => LinkedTokens.IsLinkedCancelled;
    public CancellationToken InternalToken => LinkedTokens.InternalToken;
    public CancellationToken SuperiorToken => LinkedTokens.SuperiorToken;
    public CancellationToken LinkedToken => LinkedTokens.LinkedToken;
}