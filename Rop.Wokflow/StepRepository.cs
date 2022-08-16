using System.Collections.Concurrent;
using System.Reflection;

namespace Rop.Wokflow;

public class StepRepository
{
    private readonly IWorkflow _workflow;
    private readonly Type _workflowType;
    private readonly ConcurrentDictionary<string,Step> _steps = new (StringComparer.OrdinalIgnoreCase);

    private Step? _factory(string? name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        var dir = _workflowType.GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic);
        if (dir == null) dir=_workflowType.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
        if (dir == null) return null;
        return Step.Factory(name,dir);
    }
    public Step? TryGet(string? name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        if (!_steps.TryGetValue(name, out var step))
        {
            step = _factory(name);
            if (step is null)return null;
            _steps[name] = step;
        }
        return step;
    }
        
    public StepRepository(IWorkflow workflow)
    {
        _workflow = workflow;
        _workflowType = workflow.GetType();
    }

    public Step Get(string name)
    {
        return TryGet(name) ?? throw new Exception($"Step {name} not found");
    }
}