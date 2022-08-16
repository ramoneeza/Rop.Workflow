namespace Rop.Wokflow.NextCases;

public class NextStatusCancelSaga : NextStatus
{
    public NextStatusCancelSaga(object? parameter = null) : base(NextEnum.CancelSaga, parameter){}
}