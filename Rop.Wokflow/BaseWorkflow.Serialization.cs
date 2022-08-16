using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rop.Wokflow.NextCases;
using Rop.Wokflow.Pipelines;

namespace Rop.Wokflow
{

    // Serialization Partial
    public partial class BaseWorkflow
    {
        private readonly object _lockserialization = new object();
        public int RunningSteps { get; private set; }
        /// <summary>
        /// Serialization Paused when External Cancel but we won't signal all processed as cancelled
        /// </summary>
        public bool PauseSerialization { get; private set; }
        /// <summary>
        /// Each Possible status change calls ProgressSerialization to change CurrentSerialization
        /// </summary>
        public void ProgressSerialization()
        {
            if (PauseSerialization) return;
            lock (_lockserialization)
            {
                // WARNING! RunningStep and ID shouldn't be includen in comparison
                var dto = Serialize();
                if (dto.Equals(CurrentSerialization)) return;
                dto.RunningSteps = RunningSteps;
                CurrentSerialization = dto;
                Persistence.Save(dto);
                RunningSteps++;
            }
        }
        /// <summary>
        /// IoC of Persistence to save Serialization To Disk/Database
        /// </summary>
        public IWorkflowPersistence Persistence { get; }
        /// <summary>
        /// Current Serialization. We store in memory to compare for changes
        /// </summary>
        public WorkflowDto? CurrentSerialization { get; private set; } = null;

        /// <summary>
        /// Serialize with input parameter is virtual because dto can change type on other Workflows
        /// </summary>
        /// <param name="dto">dto to be filled with Wf status.</param>
        /// <returns>Self dto</returns>
        protected virtual WorkflowDto Serialize(in WorkflowDto dto)
        {
            dto.WorkflowType = this.GetType().Name;
            dto.CompleteTime = CompleteTime;
            dto.StartingTime = StartingTime;
            dto.Description = Description;
            dto.RunningDescription = RunningDescription;
            dto.Status = Status;
            dto.WorkFlowId = WorkFlowId; // No used in comparison
            dto.Result = Result;
            dto.MainPipeline = MainPipeline?.CurrentSerialization;
            dto.RunningSteps = RunningSteps; // No used in comparison
            return dto;
        }
        /// <summary>
        /// Serialize is virtual because dto can change type on other Workflows
        /// </summary>
        /// <returns>Dto</returns>
        protected virtual WorkflowDto Serialize()
        {
            var res = new WorkflowDto();
            return Serialize(res);
        }
    }
}
