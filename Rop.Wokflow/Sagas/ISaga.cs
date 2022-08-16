using Rop.Wokflow.NextCases;
using Rop.Wokflow.Pipelines;

namespace Rop.Wokflow.Sagas;

public interface ISaga:ISubProcess
{
    IFirstStepCallOrSaga FirstStep { get; }
    new Pipeline Parent { get; }
    SagaStatus Status { get; }
    SagaDto CurrentSerialization { get; }
}