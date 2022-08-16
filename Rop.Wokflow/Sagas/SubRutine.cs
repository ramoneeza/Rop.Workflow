using Rop.Wokflow.NextCases;
using Rop.Wokflow.Pipelines;

namespace Rop.Wokflow.Sagas;

public class SubRutine : ISaga
{
    public BaseWorkflow Workflow { get; }
    public Pipeline Parent { get; }
    ISubProcess ISubProcess.Parent => Parent;

    public ILinkedTokens LinkedTokens { get; }
    public FirstStepCall FirstStep { get; private set; }
    IFirstStepCallOrSaga ISaga.FirstStep => FirstStep;

    public Pipeline? Pipeline { get; private set; } = null;
    public bool Finished => Status == SagaStatus.Terminated;
    public NextStatus? NextResult { get; private set; }

    public SagaStatus Status { get; private set; }
    public SagaDto? CurrentSerialization { get; private set; }
    
    public async Task<NextStatus> Run()
    {
        if (Finished)
        {
            return NextResult??NextStatus.Error("No NextResult");
        }
        Status = SagaStatus.Running;
        var runningTask=Task.Run(() => InnerRun(), LinkedTokens.LinkedToken);
        return await runningTask;
    }
    public void LoadFirstStep()
    {
        NextResult = null;
        Status = SagaStatus.Loaded;
        Pipeline=Pipeline.FactoryLoadedFirstStep(Workflow,this);
    }
    public void Load(SagaDto dto)
    {
        Deserialize(dto);
        if (dto.Pipelines is not null)
            Pipeline = Pipeline.FactoryLoaded(Workflow, this, dto.Pipelines[0]);
        else
            Pipeline = Pipeline.FactoryLoadedFirstStep(Workflow, this);
    }

    private NextStatus InnerRun()
    {
        if (Pipeline is null) throw new Exception("Pipeline not constructed on Subrutine");
        try
        {

            var res = Pipeline.Run();
            //RunningTask = res;
            res.Wait(LinkedTokens.LinkedToken);
            NextResult =(res.IsCompletedSuccessfully)
                ?res.Result
                :(LinkedTokens.IsLocalCancelled?NextStatus.CancelSaga():NextStatus.Error("Error Pipeline on Subrutine"));
            return NextResult;
        }
        catch (Exception ex)
        {
            if (LinkedTokens.IsLocalCancelled) return NextStatus.CancelSaga();
            return NextStatus.Error(ex.Message);
        }
        finally
        {
            Status = SagaStatus.Terminated;
            Pipeline = null;
        }
    }

    public SubRutine(BaseWorkflow workflow,Pipeline parent, FirstStepCall firststep)
    {
        FirstStep=firststep;
        Workflow = workflow;
        Parent=parent;
        LinkedTokens = new LinkedTokens(this);
    }
    private readonly object _lockserialization = new object();
    public SagaDto Serialize()
    {
        lock (_lockserialization)
        {
            return new SagaDto()
            {
                Type = "SubRutine",
                FirstStep = FirstStep,
                NextResult = NextResult,
                Pipelines=(Pipeline is not null)?new[]{Pipeline.CurrentSerialization}:null,
                Status = Status
            };
        }
    }

    public void Deserialize(SagaDto dto)
    {
        FirstStep = (dto.FirstStep as FirstStepCall)??throw new Exception("Error desserializando subrutina");
        NextResult = dto.NextResult;
        Status = dto.Status;
    }
    public void ProgressSerialization()
    {
        var dto = Serialize();
        if (dto==CurrentSerialization) return;
        CurrentSerialization = dto;
        Parent.ProgressSerialization();
    }

    public static ISaga FactoryLoaded(BaseWorkflow workflow, Pipeline pipeline, FirstStepCall firstStepCall)
    {
        var call = new SubRutine(workflow, pipeline, firstStepCall);
        call.LoadFirstStep();
        return call;
    }
    public static ISaga FactoryLoaded(BaseWorkflow workflow, Pipeline pipeline, SagaDto dto)
    {
        var firststep=(dto.FirstStep as FirstStepCall)??throw new Exception("Error desserializando subrutina");
        var call = new SubRutine(workflow, pipeline, firststep);
        call.Load(dto);
        return call;
    }


    public void Dispose()
    {
        Pipeline?.Dispose();
        LinkedTokens.Dispose();
    }
}