namespace Rop.Wokflow.NextCases;

public class FirstStep:IHasFirstStep,IEquatable<FirstStep>
{
    public string StartStep { get; }
    public object Parameter { get; }
    public FirstStep(string nextStep, object parameter)
    {
        StartStep = nextStep;
        Parameter = parameter;
    }

    public bool Equals(FirstStep? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StartStep == other.StartStep && Equals(Parameter, other.Parameter);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not FirstStep fs) return false;
        return Equals(fs);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartStep, Parameter);
    }

    public static IHasFirstStep Factory(string next, object? parameter)
    {
        if (parameter is null)
            return FirstStepNull.Factory(next);
        else
            return new FirstStep(next,parameter);
    }

    public static IHasFirstStep Factory(NextStatusNext next)
    {
        return Factory(next.NextStep, next.Parameter);
    }
}
