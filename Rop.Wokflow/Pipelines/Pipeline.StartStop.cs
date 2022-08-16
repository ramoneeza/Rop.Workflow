using Rop.Wokflow.NextCases;
using Rop.Wokflow.Sagas;

namespace Rop.Wokflow.Pipelines
{
    public partial class Pipeline
    {
        public bool Finished => PipelineStatus == Pipelines.PipelineStatus.Terminated;
        public bool Running => PipelineStatus == PipelineStatus.Running;
        public async Task<NextStatus> Run()
        {
            return PipelineStatus switch
            {
                PipelineStatus.Terminated => NextResult ?? NextStatus.Error("No return"),
                PipelineStatus.Loaded =>await MakePipeline(),
                PipelineStatus.Running => await MakePipeline(),
                _ => throw new Exception("Run pipeline only allowed on Loaded status")
            };
        }
        public void LoadFirstStep()
        {
            if (PipelineStatus != PipelineStatus.Init) throw new Exception("Load pipeline only allowed on Init status");
            NextResult = null;
            CurrentSaga = null;
            CurrentStep = FirstStep.ToNextStep();
            PipelineStatus = PipelineStatus.Loaded;
        }

        public void Load(PipelineDto dto)
        {
            if (PipelineStatus != PipelineStatus.Init) throw new Exception("Load pipeline only allowed on Init status");
            Deserialize(dto);
        }

        private void Deserialize(PipelineDto dto)
        {
            FirstStep = dto.FirstStep ?? throw new Exception("Error deserializing FirstStep");
            CurrentStep = dto.CurrentStep;
            if (dto.CurrentSaga is not null)
                CurrentSaga = (dto.CurrentSaga.Type == "Saga")
                    ? Saga.FactoryLoaded(Workflow, this, dto.CurrentSaga)
                    : SubRutine.FactoryLoaded(Workflow, this, dto.CurrentSaga);
            else
                CurrentSaga = null;
            NextResult = dto.NextToReturn;
            CurrentSerialization = dto;
            PipelineStatus = dto.PipelineStatus;
        }
        private Task<NextStatus> MakePipeline()
        {
            return Task.Run(() =>
            {
                NextResult = InnerRun();
                PipelineStatus = PipelineStatus.Terminated;
                CurrentStep = null;
                ProgressSerialization();
                return NextResult;
            },LinkedTokens.LinkedToken);
        }
    }
}
