using System.Reflection.Metadata;


namespace Rop.Wokflow.NextCases;

public class FirstStepSaga:IFirstStepCallOrSaga,IEquatable<FirstStepSaga>
{   
    public string[] Pipelines { get; }
    public string ReturnStep { get; }
    public object? Parameter { get; }
    public FirstStepSaga(IEnumerable<string> firstSteps,string returnStep, object? parameter)
    {
        Pipelines =firstSteps?.ToArray()??Array.Empty<string>();
        ReturnStep = returnStep;
        Parameter = parameter;
    }
    public static FirstStepSaga Factory(NextStatusSaga next)
    {
        return new FirstStepSaga(next.Pipelines,next.NextStep, next.Parameter);
    }

    public bool Equals(FirstStepSaga? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Pipelines.Equals(other.Pipelines) && ReturnStep == other.ReturnStep && Equals(Parameter, other.Parameter);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not FirstStepSaga saga) return false;
        return Equals(saga);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Pipelines, ReturnStep, Parameter);
    }
}