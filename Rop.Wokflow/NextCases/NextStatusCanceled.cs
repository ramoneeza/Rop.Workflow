namespace Rop.Wokflow.NextCases;

public class NextStatusCanceled: NextStatus
{
    public NextStatusCanceled(object? parameter=null):base(NextEnum.Canceled, parameter)
    {
    }
}