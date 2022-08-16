using Rop.Wokflow.NextCases;
using Rop.Wokflow.Pipelines;

namespace Rop.Wokflow
{
    // Start/Stop Engine of a Workflow
    public partial class BaseWorkflow
    {
        // The main Pipeline Running the FirstStep
        public Pipeline? MainPipeline { get; private set; } = null;
        public async Task<NextStatus> Run()
        {
            if (Status == WorkflowStatus.Terminated) 
                return Result??NextStatus.Error("No Result");
            if (Status != WorkflowStatus.Loaded || MainPipeline is null) 
                throw new Exception("Workflow not loaded");
            Status = WorkflowStatus.Running;
            var workflowtask = InnerRun();
            return await workflowtask;
        }

        private Task<NextStatus> InnerRun()
        {
            return Task.Run<NextStatus>(() =>
            {
                try
                {
                    var r = MainPipeline!.Run();
                    r.Wait(this.CancellationToken);
                    if (r.IsCompletedSuccessfully)
                    {
                        return r.Result;
                    }
                    else
                    {
                        return NextStatus.Canceled();
                    }
                }
                catch (Exception ex)
                {
                    return NextStatus.Error(ex.Message);
                }
                finally
                {
                    CompleteTime=DateTime.Now;
                    Status = WorkflowStatus.Terminated;
                }
            },CancellationToken);
        }

        public void LoadFirstStep()
        {
            if (Status != WorkflowStatus.Init) throw new Exception("Workflow already running");
            MainPipeline = new Pipeline(this, null,FirstStep);
            MainPipeline.LoadFirstStep();
            Status = WorkflowStatus.Loaded;
        }

        public void Load(WorkflowDto dto)
        {
            if (Status != WorkflowStatus.Init) throw new Exception("Workflow already running");
            _currentdefers.Clear();
            Deserialize(dto);
            if (dto.Status!=WorkflowStatus.Terminated) Status = WorkflowStatus.Loaded;
        }

        protected virtual void Deserialize(WorkflowDto dto)
        {
            WorkFlowId=dto.WorkFlowId;
            if (dto.WorkflowType != WorkflowType.Name) throw new Exception("Mismatch workflowtype");
            RunningDescription = dto.RunningDescription;
            StartingTime = dto.StartingTime;
            CompleteTime = dto.CompleteTime;
            RunningSteps = dto.RunningSteps;
            CurrentSerialization = dto;
            MainPipeline = dto.MainPipeline != null ? Pipeline.FactoryLoaded(this,null,dto.MainPipeline) : null;
            Result = dto.Result;
        }

        public void Cancel()
        {
            if (Status == WorkflowStatus.Running)
            {
                this._linkedTokens.MainCts.Cancel();
            }
        }

        public void Suspend()
        {
            PauseSerialization = true;
            Cancel();
            PauseSerialization = false;
        }

    }
}
