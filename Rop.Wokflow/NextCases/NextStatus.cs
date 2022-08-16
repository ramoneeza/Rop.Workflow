using Newtonsoft.Json;

namespace Rop.Wokflow.NextCases;

public abstract class NextStatus:IEquatable<NextStatus>
{
    public NextEnum NextSt { get; }
    public object? Parameter { get; }
    [JsonIgnore]
    public bool MustReturn
    {
        get
        {
            switch (this)
            {
                case NextStatusError:
                case NextStatusTerminate:
                case NextStatusReturn:
                case NextStatusJoin:
                case NextStatusCanceled:
                    return true;
                case NextStatusLoop:
                case NextStatusNext:
                    return false;
                default:
                    return true;
            }
        }
    }

    protected NextStatus(NextEnum nextSt, object? parameter)
    {
        NextSt = nextSt;
        Parameter = parameter;
    }

    public static NextStatusNext Next(string nextstep, object? parameter = null) => new NextStatusNext(nextstep, parameter);
    public static NextStatusCall Call(string callstep, string nextstep, object? parameter = null) => new NextStatusCall(callstep, nextstep, parameter);
    public static NextStatusSaga Saga(string[] pipelines, string nextstep, object? parameter = null) => new NextStatusSaga(pipelines, nextstep, parameter);
    public static NextStatusLoop Loop(object? parameter = null) => new NextStatusLoop(parameter);
    public static NextStatusDefer Defer(string defertype, string nextstep, object? parameter = null) => new NextStatusDefer(defertype, nextstep, parameter);

    public static NextStatusDefer Defer<T>(T defertype, string nextstep, object? parameter = null)
        where T : struct, Enum => new NextStatusDefer(defertype.ToString(), nextstep, parameter);

    public static NextStatusReturn Return(object? parameter=null) => new NextStatusReturn(parameter);
    public static NextStatusTerminate Terminate() => new NextStatusTerminate();
    public static NextStatusError Error(string message) => new NextStatusError(message);
    public static NextStatusCanceled Canceled(object? parameter = null) => new NextStatusCanceled(parameter);
    public static NextStatusJoin Join() => new NextStatusJoin();
    public static NextStatus CanceledOrJoin(CancellationToken ct) => (ct.IsCancellationRequested) ? Canceled() : Join();
    public static NextStatus CanceledOrJoin(bool iscancelledorended) => iscancelledorended ? Canceled() : Join();
    public static NextStatus CancelSaga(object? parameter=null) =>new NextStatusCancelSaga(parameter);

    public virtual bool Equals(NextStatus? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return NextSt == other.NextSt && Equals(Parameter, other.Parameter);
    }
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((NextStatus)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)NextSt, Parameter);
    }
}
