namespace Rop.Wokflow.NextCases;

public class NextStatusDefer : NextStatusNext,IEquatable<NextStatusDefer>
{
    public string DeferType { get; }

    public NextStatusDefer(string defertype, string nextstep, object? parameter = null) : base(nextstep, NextEnum.Defer,
        parameter)
    {
        DeferType = defertype;
    }

    public override bool Equals(NextStatus? other)
    {
        if (other is NextStatusDefer defer) return Equals(defer);
        return false;
    }

    public override bool Equals(NextStatusNext? other)
    {
        if (other is NextStatusDefer defer) return Equals(defer);
        return false;
    }

    public bool Equals(NextStatusDefer? other)
    {
        if (!base.Equals(other)) return false;
        return DeferType == other.DeferType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(),DeferType);
    }
}