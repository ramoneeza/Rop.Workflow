using Newtonsoft.Json;

namespace Rop.Wokflow.NextCases;

public class NextStatusCall : NextStatusSaga,IEquatable<NextStatusCall>
{
    [JsonIgnore]
    public override string[] Pipelines => base.Pipelines;
    public string CallStep => Pipelines[0];
    public NextStatusCall(string callstep, string nextStep, object? parameter) : base(new []{callstep},nextStep, NextEnum.Call, parameter)
    {
    }

    public FirstStepCall ToFirstStepCall()
    {
        return FirstStepCall.Factory(CallStep, NextStep, Parameter);
    }

    public override bool Equals(NextStatus? other)
    {
        if (other is NextStatusCall call) return call.Equals(other);
        return false;
    }

    public override bool Equals(NextStatusNext? other)
    {
        if (other is NextStatusCall call) return call.Equals(other);
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public bool Equals(NextStatusCall? other)
    {
        return base.Equals(other);
    }
}
