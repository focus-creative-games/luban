using Luban.Defs;
using Luban.Utils;

namespace Luban.Validator;

public class DataValidatorContext
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    [field: ThreadStatic]
    public static DataValidatorVisitor CurrentVisitor { get; set; }

    public static string CurrentRecordPath => TypeUtil.MakeFullName(CurrentVisitor.Path);

    public DefAssembly Assembly { get; }

    public DataValidatorContext(DefAssembly ass)
    {
        this.Assembly = ass;
    }

    public void ValidateTables(IEnumerable<DefTable> tables)
    {
        var tasks = new List<Task>();
        foreach (var t in tables)
        {
            tasks.Add(Task.Run(() =>
            {
                var records = GenerationContext.Current.GetTableAllDataList(t);
                var visitor = new DataValidatorVisitor(this);
                try
                {
                    CurrentVisitor = visitor;
                    visitor.ValidateTable(t, records);
                }
                finally
                {
                    CurrentVisitor = null;
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }
}
