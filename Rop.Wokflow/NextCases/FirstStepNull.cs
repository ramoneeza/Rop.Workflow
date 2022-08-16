namespace Rop.Wokflow.NextCases;

public class FirstStepNull : IHasFirstStep,IEquatable<FirstStepNull>
{
    public string StartStep { get; }

    public FirstStepNull(string startStep)
    {
        StartStep = startStep;
    }


    public bool Equals(FirstStepNull? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StartStep == other.StartStep;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((FirstStepNull)obj);
    }

    public override int GetHashCode()
    {
        return StartStep.GetHashCode();
    }

    public static FirstStepNull Factory(string startStep)
    {
        return new FirstStepNull(startStep);
    }
}