namespace Rop.Wokflow.NextCases;

public class NextStatusJoin : NextStatus
{
    public NextStatusJoin(object? parameter = null) : base(NextEnum.Loop, parameter) { }
}