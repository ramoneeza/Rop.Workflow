using System.Linq.Expressions;
using Rop.Wokflow.NextCases;
using Rop.Wokflow.Sagas;

namespace Rop.Wokflow.Pipelines
{
    public partial class Pipeline
    {
        private NextStatus InnerRun()
        {
            try
            {
                while (!LinkedTokens.IsTerminatedOrCancelled)
                {
                    NextStatus res = ContinueStepDeferSagaOrCall();
                    if (res.MustReturn) return res;
                    if (res is NextStatusNext next)
                    {
                        CurrentStep = next;
                        ProgressSerialization();
                    }
                    // Must be loop
                }
                return NextStatus.Canceled();
            }
            catch (Exception ex)
            {
                return (LinkedTokens.IsLocalCancelled)?NextStatus.Canceled():NextStatus.Error(ex.Message);
            }
        }

        private NextStatus ContinueStepDeferSagaOrCall()
        {
            switch (CurrentStep)
            {
                case NextStatusCall call:
                    return ContinueCall(call);
                case NextStatusSaga saga:
                    return ContinueSaga(saga);
                case NextStatusDefer defer:
                    return ContinueDefer(defer);
                case NextStatusNext next:
                    return ContinueStep(next);
                default:
                    return NextStatus.Error("CurrentStep incorrect on InnerRun");
            }
        }
        private NextStatus ContinueStep(NextStatusNext next)
        {
            if (string.IsNullOrEmpty(next.NextStep)) return new NextStatusJoin();
            var step = Workflow.StepRepository.Get(next.NextStep);
            return step.Run(Workflow,LinkedTokens.LinkedToken, next.Parameter);
        }

        private NextStatus ContinueCall(NextStatusCall call)
        {
            if (CurrentSaga is null) CurrentSaga = SubRutine.FactoryLoaded(Workflow, this, call.ToFirstStepCall());
            return ContinueCallOrSaga(call);
        }
        private NextStatus ContinueSaga(NextStatusSaga saga)
        {
            if (CurrentSaga is null) CurrentSaga = Saga.FactoryLoaded(Workflow, this, saga.ToFirstStepSaga());
            return ContinueCallOrSaga(saga);
        }

        private NextStatus ContinueCallOrSaga(NextStatusSaga saga)
        {
            if (CurrentSaga is null) throw new Exception("CurrentSaga null on ContinueCallOrSaga");
            try
            {
                var task = CurrentSaga.Run();
                task.Wait(LinkedTokens.LinkedToken);
                var res = (task.IsCompletedSuccessfully)
                    ? task.Result
                    : (LinkedTokens.IsLocalCancelled ? NextStatus.Canceled() : NextStatus.Error("Error en Saga"));
                CurrentSaga = null;
                switch (res.NextSt)
                {
                    case NextEnum.Return:
                        return NextStatus.Next(saga.NextStep, res.Parameter);
                    case NextEnum.CancelSaga:
                        return NextStatus.Next(saga.NextStep, res.Parameter);
                    case NextEnum.Call:
                    case NextEnum.Defer:
                    case NextEnum.Saga:
                        return NextStatus.Error($"Error on CallResult");
                    default:
                        return res;
                }

            }
            catch (Exception ex)
            {
                if (LinkedTokens.IsTerminatedOrCancelled)
                    return NextStatus.Canceled();
                return NextStatus.Error(ex.Message);
            }
            finally
            {
                CurrentSaga = null;
            }
        }
        private NextStatus ContinueDefer(NextStatusDefer nd)
        {
            var defer = new DeferArgs(Workflow, nd.DeferType, nd.Parameter);
            Workflow.PushDefer(defer);
            if (!defer.IsCompleted && !LinkedTokens.IsLinkedCancelled)
            {
                defer.Signal.WaitOne(LinkedTokens.LinkedToken);
            }
            if (defer.IsCompleted)
            {
                return NextStatus.Next(nd.NextStep, defer.Argument);
            }
            else
            {
                return NextStatus.CanceledOrJoin(LinkedTokens.IsLinkedCancelled);
            }
        }

        

    }
}
