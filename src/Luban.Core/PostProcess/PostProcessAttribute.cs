using Luban.CustomBehaviour;

namespace Luban.PostProcess;

[Flags]
public enum TargetFileType
{
    None = 0,
    Code = 0x1,
    DataExport = 0x2,
    DataConvert = 0x4,
}

public class PostProcessAttribute : BehaviourBaseAttribute
{
    public TargetFileType TargetFileType { get; }

    public PostProcessAttribute(string name, TargetFileType targetFileType) : base(name)
    {
        TargetFileType = targetFileType;
    }
}
