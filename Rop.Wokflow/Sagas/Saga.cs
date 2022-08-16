using Rop.Wokflow.NextCases;
using Rop.Wokflow.Pipelines;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Xml;
using Rop.Wokflow;
using Rop.Wokflow.Sagas;

namespace Rop.Wokflow.Sagas
{
    public class Saga : ISaga
    {
        public ILinkedTokens LinkedTokens { get; }
        public BaseWorkflow Workflow { get; }
        public Pipeline Parent { get; }
        ISubProcess ISubProcess.Parent => Parent;
        IFirstStepCallOrSaga ISaga.FirstStep => FirstStep;
        public FirstStepSaga FirstStep { get; private set; }
        public SagaStatus Status { get; private set; }
        public Pipeline[] Pipelines { get; private set; } = Array.Empty<Pipeline>();
        public bool Finished => Status == SagaStatus.Terminated;
        public NextStatus? NextResult { get; private set; }
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
            Pipelines = FirstStep.Pipelines.Select(s => Pipeline.FactoryLoadedFirstStep(Workflow, this, s, FirstStep.Parameter)).ToArray();
        }
        public void Load(SagaDto dto)
        {
            Deserialize(dto);
            Pipelines = dto.Pipelines is not null ? 
                dto.Pipelines.Select(s => Pipeline.FactoryLoaded(Workflow, this, s)).ToArray() 
                : FirstStep.Pipelines.Select(s => Pipeline.FactoryLoadedFirstStep(Workflow, this, s, FirstStep.Parameter)).ToArray();
        }
        private NextStatus InnerRun()
        {
            NextResult = null;
            var returnlist = new List<object?>();
            try
            {
                var tasks = Pipelines.Select(p => p.Run()).ToList();
                while (!LinkedTokens.IsTerminatedOrCancelled)
                {
                    var arraytask = tasks.Where(t => !t.IsCompleted).Cast<Task>().ToArray();
                    if (!arraytask.Any()) break;
                    Task.WaitAny(arraytask, LinkedTokens.LinkedToken);
                    var terminated = arraytask.Where(t => t.IsCompleted).Cast<Task<NextStatus>>().ToArray();
                    if (!terminated.Any()) continue;
                    foreach (var t in terminated)
                    {
                        if (LinkedTokens.IsTerminatedOrCancelled) break;
                        var p = t.Result;
                        switch (p.NextSt)
                        {
                            case NextEnum.CancelSaga:
                                NextResult = p;
                                LinkedTokens.CancelLocal();
                                break;
                            case NextEnum.Canceled:
                            case NextEnum.Error:
                            case NextEnum.Terminate:
                                NextResult = p;
                                LinkedTokens.CancelLocal();
                                break;
                            case NextEnum.Return:
                                returnlist.Add(p.Parameter);
                                NextResult = p;
                                break;
                            case NextEnum.Join:
                                break;
                            default:
                                NextResult = NextStatus.Error("Bad Result on Saga");
                                LinkedTokens.CancelLocal();
                                break;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NextResult ??= LinkedTokens.IsLocalCancelled ? NextStatus.CancelSaga() : NextStatus.Error(ex.Message);
            }
            NextResult ??= LinkedTokens.IsLocalCancelled ? NextStatus.CancelSaga() : NextStatus.Error("No Result on Saga");

            if (NextResult.NextSt == NextEnum.Return)
            {
                    NextResult = new NextStatusReturn(returnlist.ToArray());
            }
            Status = SagaStatus.Terminated;
            Pipelines=Array.Empty<Pipeline>();
            return NextResult;
        }
        public Saga(BaseWorkflow workflow,Pipeline parent,FirstStepSaga firstStep)
        {
            Workflow = workflow;
            Parent = parent;
            FirstStep = firstStep;
            Status = SagaStatus.Init;
            LinkedTokens = new LinkedTokens(this);
        }
        private readonly object _lockserialization=new object();
        public void ProgressSerialization()
        {
            lock (_lockserialization)
            {
                var dto = new SagaDto()
                {
                    Type = "Saga",
                    FirstStep = FirstStep,
                    NextResult = NextResult,
                    Pipelines =(Pipelines.Any())?Pipelines.Select(p => p.CurrentSerialization).ToArray():null,
                    Status = Status
                };
                if (dto.Equals(CurrentSerialization)) return;
                CurrentSerialization = dto;
                Parent.ProgressSerialization();
            }
        }
        
        public void Deserialize(SagaDto dto)
        {
            FirstStep =(dto.FirstStep as FirstStepSaga) ?? throw new Exception("FirstArgument deserialization exception");
            NextResult = dto.NextResult;
            Pipelines = (dto.Pipelines?.Select(p =>Pipeline.FactoryLoaded(Workflow, this,p)).ToArray())??Array.Empty<Pipeline>();
            Status = dto.Status;
        }

        public static ISaga FactoryLoaded(BaseWorkflow workflow, Pipeline pipeline, FirstStepSaga firstStepSaga)
        {
            var saga = new Saga(workflow, pipeline, firstStepSaga);
            saga.LoadFirstStep();
            return saga;
        }
        public static ISaga FactoryLoaded(BaseWorkflow workflow, Pipeline pipeline, SagaDto dto)
        {
            var firststep=(dto.FirstStep as FirstStepSaga)??throw new Exception("Error desserializando saga");
            var saga = new Saga(workflow, pipeline, firststep);
            saga.Load(dto);
            return saga;
        }

        public void Dispose()
        {
            LinkedTokens.Dispose();
        }
    }
}
