using Rop.Wokflow.NextCases;
using Rop.Wokflow.Sagas;

namespace Rop.Wokflow.Pipelines
{
    public partial class Pipeline:ISubProcess
    {
        public BaseWorkflow Workflow { get; }
        public ISubProcess? Parent { get; }
        public ILinkedTokens LinkedTokens { get; }
        public IHasFirstStep FirstStep { get; private set; }
        public NextStatusNext? CurrentStep { get; set; } = null;
        public NextStatus? NextResult { get; private set; }
        public ISaga? CurrentSaga { get; private set; }
        public PipelineStatus PipelineStatus { get; private set; }
        public Pipeline(BaseWorkflow workflow, ISaga? parent, IHasFirstStep firstStep)
        {
            Workflow = workflow;
            Parent = parent;
            FirstStep = firstStep;
            PipelineStatus = PipelineStatus.Init;
            LinkedTokens = new LinkedTokens(this);
            CurrentSerialization = Serialize();
        }

        public static Pipeline FactoryLoaded(BaseWorkflow workflow, ISaga? parentSaga, PipelineDto dto)
        {
            var firststep = dto.FirstStep;
            var pipeline=new Pipeline(workflow, parentSaga, firststep);
            pipeline.Load(dto);
            if (pipeline.PipelineStatus==PipelineStatus.Init)
                pipeline.LoadFirstStep();
            return pipeline;
        }

        public void Dispose()
        {
            CurrentSaga?.Dispose();
            LinkedTokens?.Dispose();
        }

        public static Pipeline FactoryLoadedFirstStep(BaseWorkflow workflow, SubRutine subRutine)
        {
            var firststep =NextCases.FirstStep.Factory(subRutine.FirstStep.CallStep,subRutine.FirstStep.Parameter);
            var pipeline=new Pipeline(workflow, subRutine, firststep);
            pipeline.LoadFirstStep();
            return pipeline;
        }

        public static Pipeline FactoryLoadedFirstStep(BaseWorkflow workflow, Saga saga, string s, object? parameter)
        {
            var firststep =NextCases.FirstStep.Factory(s,parameter);
            var pipeline=new Pipeline(workflow, saga, firststep);
            pipeline.LoadFirstStep();
            return pipeline;
        }
    }
}
