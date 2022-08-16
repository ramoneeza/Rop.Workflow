using Rop.Wokflow.NextCases;

namespace Rop.Wokflow;

public abstract partial class BaseWorkflow : IWorkflow
{
    /// <summary>
    /// WorkflowId adquires his Id when Insert on Database.
    /// </summary>
    public int WorkFlowId { get; private set; } = -1;
    public IHasFirstStep FirstStep { get; private set; }
    public virtual Type WorkflowType { get; }
    /// <summary>
    /// Description must be override with the description of current Workflow Type
    /// </summary>
    public abstract string Description { get; }
    /// <summary>
    /// Description of current progress.
    /// Update Via Attribute on Step or Direct change
    /// </summary>
    public string RunningDescription { get; protected internal set; } = "";
    /// <summary>
    /// Each Step is Cached into this StepRepository.
    /// </summary>
    public StepRepository StepRepository { get; }
    /// <summary>
    /// Current Workflow Status.
    /// </summary>
    public WorkflowStatus Status { get; protected internal set; }
    /// <summary>
    /// When Workflow starts
    /// </summary>
    public DateTime? StartingTime { get; protected set; }
    /// <summary>
    /// When Workflow ends
    /// </summary>
    public DateTime? CompleteTime { get; protected set; }
    /// <summary>
    /// Final Result of Workflow
    /// </summary>
    public NextStatus? Result { get; private set; }
    /// <summary>
    /// Main Constructor
    /// </summary>
    /// <param name="firststep">
    ///First Step to Launch on MainPipeline
    /// </param>
    /// <param name="persistence">
    ///Ioc for Persistence
    /// </param>

    protected BaseWorkflow(string firststep, IWorkflowPersistence persistence)
    {
        WorkflowType = this.GetType();
        Persistence = persistence;
        MainPipeline = null;
        StartingTime = DateTime.Now;
        _linkedTokens = new WfLinkedTokens(this);
        StepRepository = new StepRepository(this);
        Status = WorkflowStatus.Init;
        FirstStep =FirstStepNull.Factory(firststep);
        RunningSteps = 0;
    }
    /// <summary>
    /// MainPipeline Must Be Disposed.
    /// Virtual as Derived workflows may be need more dispose.
    /// </summary>
    public virtual void Dispose()
    {
        MainPipeline?.Dispose();
        MainPipeline = null;
        _linkedTokens.Dispose();
    }
}