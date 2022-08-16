using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rop.Wokflow.NextCases;

namespace Rop.Wokflow.Pipelines
{
    public partial class Pipeline
    {
        private readonly object _lockserialization = new object();
        public bool PauseSerialization => Workflow.PauseSerialization;
        public PipelineDto CurrentSerialization { get; private set; }
        private PipelineDto Serialize()
        {
            var dto = new PipelineDto()
            {
                CurrentSaga = CurrentSaga?.CurrentSerialization,
                CurrentStep = CurrentStep,
                FirstStep = FirstStep,
                NextToReturn = NextResult,
                PipelineStatus = PipelineStatus
            };
            return dto;
        }
        public void ProgressSerialization()
        {
            if (PauseSerialization) return;
            lock (_lockserialization)
            {


                var dto = Serialize();
                if (dto != CurrentSerialization)
                {
                    CurrentSerialization = dto;
                    if (Parent is not null)
                        Parent.ProgressSerialization();
                    else
                        Workflow.ProgressSerialization();
                }
            }
        }
        
    }
}
