namespace Rop.Wokflow.NextCases;

public class FirstStepCall:IFirstStepCallOrSaga,IEquatable<FirstStepCall>
{
    public object? Parameter { get; init; }
    public string ReturnStep { get; init; }
    public string CallStep { get; init; }
    public FirstStepCall(string callStep,string returnStep, object? parameter)
    {
        CallStep = callStep;
        ReturnStep = returnStep;
        Parameter = parameter;
    }
    
    public bool Equals(FirstStepCall? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(Parameter, other.Parameter) && ReturnStep == other.ReturnStep && CallStep == other.CallStep;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not FirstStepCall call) return false;
        return Equals(call);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Parameter, ReturnStep, CallStep);
    }

    public IHasFirstStep ToPureFirstStep()
    {
        return FirstStep.Factory(CallStep, Parameter);
        
    }
   public static FirstStepCall Factory(NextStatusCall next)
    {
        return new FirstStepCall(next.CallStep,next.NextStep, next.Parameter);
    }
   public static FirstStepCall Factory(string callStep, string returnStep, object? parameter)
   {
       return new FirstStepCall(callStep,returnStep, parameter);
   }
}