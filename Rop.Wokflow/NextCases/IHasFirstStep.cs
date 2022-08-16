namespace Rop.Wokflow.NextCases;

public interface IHasFirstStep
{
    string StartStep { get; }

    public NextStatusNext ToNextStep()
    {
        if (this is FirstStepNull fsn) 
            return new NextStatusNext(fsn.StartStep);
        if (this is FirstStep fs) 
            return new NextStatusNext(fs.StartStep,fs.Parameter);
        throw new ArgumentException("No know firststep");
    }
}
public interface IHasReturnStep
{
    string ReturnStep { get; }
}