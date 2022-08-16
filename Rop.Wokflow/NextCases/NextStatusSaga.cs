

namespace Rop.Wokflow.NextCases;

public class NextStatusSaga : NextStatusNext,IEquatable<NextStatusSaga>
{
    public virtual string[] Pipelines { get; }
    protected NextStatusSaga(IEnumerable<string> pipelines,string nextStep, NextEnum st, object? parameter) : base(nextStep, st, parameter)
    {
        Pipelines = pipelines.ToArray();
    }

    public NextStatusSaga(IEnumerable<string> pipelines, string nextStep, object? parameter) :this(pipelines, nextStep, NextEnum.Saga, parameter)
    {
    }
    public FirstStepSaga ToFirstStepSaga()
    {
         return new FirstStepSaga(Pipelines, NextStep, Parameter);
    }

    public override bool Equals(NextStatus? other)
    {
        if (other is NextStatusSaga saga) return Equals(saga);
        return false;
    }

    public override bool Equals(NextStatusNext? other)
    {
        if (other is NextStatusSaga saga) return Equals(saga);
        return false;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(),Pipelines);
    }

    public bool Equals(NextStatusSaga? other)
    {
        if (!base.Equals(other)) return false;
        return Pipelines.SequenceEqual(other.Pipelines);
    }
}