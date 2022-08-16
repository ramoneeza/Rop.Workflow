using Newtonsoft.Json;

namespace Rop.Wokflow.NextCases;

public class NextStatusNext : NextStatus,IEquatable<NextStatusNext>
{
    public string NextStep { get; }
    protected NextStatusNext(string nextStep, NextEnum st, object? parameter) : base(st, parameter)
    {
        NextStep = nextStep;
    }
    public NextStatusNext(string nextStep, object? parameter = null) : this(nextStep, NextEnum.Next, parameter)
    {
    }

    public override bool Equals(NextStatus? other)
    {
        if (other is NextStatusNext derived)
            return Equals(derived);
        else
            return base.Equals(other);
    }
    public virtual bool Equals(NextStatusNext? other)
    {
        if (!base.Equals(other)) return false;
        return NextStep == other.NextStep;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(),NextStep);
    }
}