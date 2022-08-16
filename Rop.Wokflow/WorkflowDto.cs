using Rop.Wokflow.Pipelines;
using System.Collections;
using Rop.Wokflow.NextCases;

namespace Rop.Wokflow
{
    public class WorkflowDto:IEquatable<WorkflowDto>
    {
        public int WorkFlowId { get; set; } = -1;
        public string WorkflowType { get; set; }
        public string Description { get; set; }
        public string RunningDescription { get; set; }
        public WorkflowStatus Status { get; set; }
        public DateTime? StartingTime { get; set; }
        public DateTime? CompleteTime { get; set; }
        public int RunningSteps { get; set; }
        public PipelineDto? MainPipeline { get; set; }
        public NextStatus? Result { get; set; }
        public virtual bool Equals(WorkflowDto? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return
                WorkflowType == other.WorkflowType &&
                Description == other.Description &&
                RunningDescription == other.RunningDescription &&
                Status == other.Status &&
                Nullable.Equals(StartingTime, other.StartingTime) &&
                Nullable.Equals(CompleteTime, other.CompleteTime) &&
                Equals(MainPipeline, other.MainPipeline) &&
                Equals(Result, other.Result);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WorkflowDto)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(WorkflowType, Description, RunningDescription, (int)Status, StartingTime, CompleteTime, MainPipeline, Result);
        }
    }
}
