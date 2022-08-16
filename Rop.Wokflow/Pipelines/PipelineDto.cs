using Rop.Wokflow.NextCases;
using Rop.Wokflow.Sagas;

namespace Rop.Wokflow.Pipelines;

public class PipelineDto:IEquatable<PipelineDto>
{
    public IHasFirstStep FirstStep { get; init; }
    public NextStatusNext? CurrentStep { get; init; }
    public NextStatus? NextToReturn { get; init; }
    public SagaDto? CurrentSaga { get; init; }
    public PipelineStatus PipelineStatus { get; init; }

    public bool Equals(PipelineDto? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return FirstStep.Equals(other.FirstStep) && 
               Equals(CurrentStep, other.CurrentStep) && 
               Equals(NextToReturn, other.NextToReturn) && 
               Equals(CurrentSaga, other.CurrentSaga) && 
               PipelineStatus == other.PipelineStatus;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (!(obj is PipelineDto p)) return false;
        return Equals(p);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FirstStep, CurrentStep, NextToReturn, CurrentSaga, (int)PipelineStatus);
    }
}