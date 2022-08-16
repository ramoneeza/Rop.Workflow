using Rop.Wokflow.NextCases;

namespace Rop.Wokflow;

public interface IWorkflow
{
    int WorkFlowId { get; }
    IHasFirstStep FirstStep { get; }
    Type WorkflowType { get; }
    string Description { get; }
    string RunningDescription { get; }
    WorkflowStatus Status { get; }
    DateTime? StartingTime { get; }
    DateTime? CompleteTime { get; }
    int RunningSteps { get; }
    // Defer
    
    bool TryGetDefer(out IDeferArgs? defer);
    bool WaitDefer(int timeout,out IDeferArgs? defer);
    bool WaitDefer(int timeout, CancellationToken ct, out IDeferArgs? defer);

    bool WaitDefer(CancellationToken ct, out IDeferArgs? defer);
    //Serialization
    bool PauseSerialization { get; }
    WorkflowDto? CurrentSerialization { get; }
    void ProgressSerialization();
    //Start/Stop
    CancellationToken CancellationToken { get; }
    Task<NextStatus> Run();
    void LoadFirstStep();
    void Load(WorkflowDto dto);
    void Cancel();
    void Suspend();
};