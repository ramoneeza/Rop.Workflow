using Rop.Wokflow.NextCases;
using Rop.Wokflow.Pipelines;

namespace Rop.Wokflow.Sagas;

public class SagaDto:IEquatable<SagaDto>
{
    public string Type { get; init; }
    public IFirstStepCallOrSaga FirstStep { get; init; }
    public NextStatus? NextResult { get; init; }
    public PipelineDto[]? Pipelines { get; init; }
    public SagaStatus Status { get; init; }

    public bool Equals(SagaDto? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type && 
               FirstStep.Equals(other.FirstStep) && 
               Equals(NextResult, other.NextResult) && 
               PipSequenceEqual(other) &&
               Status == other.Status;
    }

    private bool PipSequenceEqual(SagaDto other)
    {
        if (Pipelines is null && other.Pipelines is null) return true;
        if (Pipelines is null) return false;
        if (other.Pipelines is null) return false;
        return Pipelines.SequenceEqual(other.Pipelines);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SagaDto)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, FirstStep, NextResult, Pipelines, (int)Status);
    }
}