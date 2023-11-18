using System.Reflection;
using Luban.CustomBehaviour;
using Luban.Validator;

namespace Luban.DataTarget;

public class DataTargetManager
{
    public static DataTargetManager Ins { get; } = new();

    public void Init()
    {

    }

    public IDataExporter CreateDataExporter(string name)
    {
        return CustomBehaviourManager.Ins.CreateBehaviour<IDataExporter, DataExporterAttribute>(name);
    }

    public IDataTarget CreateDataTarget(string name)
    {
        return CustomBehaviourManager.Ins.CreateBehaviour<IDataTarget, DataTargetAttribute>(name);
    }
}
