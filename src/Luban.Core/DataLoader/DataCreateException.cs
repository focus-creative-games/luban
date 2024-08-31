using Luban.Defs;

namespace Luban.DataLoader;

public class DataCreateException : System.Exception
{
    private List<(DefBean, DefField)> VariablePath { get; } = new();

    public string OriginDataLocation { get; set; }

    public string DataLocationInFile { get; }

    public string OriginErrorMsg { get; }

    public string OriginStackTrace { get; }

    public DataCreateException(Exception e, string dataLocation) : base(e.Message, e)
    {
        this.OriginStackTrace = e.StackTrace;
        this.OriginErrorMsg = e.Message;
        this.DataLocationInFile = dataLocation;
    }

    public override string Message => this.OriginErrorMsg;

    public void Push(DefBean bean, DefField f)
    {
        VariablePath.Add((bean, f));
    }

    public string VariableFullPathStr
    {
        get
        {
            var path = new List<(DefBean, DefField)>(VariablePath);
            path.Reverse();
            return string.Join(" => ", path.Select(b => $"{{{b.Item1.FullName}}}.{b.Item2.Name}"));
        }
    }
}
