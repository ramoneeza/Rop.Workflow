namespace Rop.Wokflow.NextCases;

public class NextStatusError : NextStatus
{
    public string Error { get; }

    public NextStatusError(string message) : base(NextEnum.Error, null)
    {
        Error = message;
    }
}