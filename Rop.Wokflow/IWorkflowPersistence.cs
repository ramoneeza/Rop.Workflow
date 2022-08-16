namespace Rop.Wokflow;

public interface IWorkflowPersistence
{
    public WorkflowDto? Load(int workflowid);
    public int Save(WorkflowDto workflow);
}