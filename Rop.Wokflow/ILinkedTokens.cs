namespace Rop.Wokflow;

public interface ILinkedTokens:IDisposable
{
    CancellationToken InternalToken { get; }
    CancellationToken SuperiorToken { get; }
    CancellationToken LinkedToken { get; }
    public bool IsLinkedCancelled =>LinkedToken.IsCancellationRequested;
    public bool IsLocalCancelled => InternalToken.IsCancellationRequested;
    public bool IsTerminatedOrCancelled { get; }
    void CancelLocal();
}