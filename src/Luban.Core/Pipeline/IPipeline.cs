using Luban.Defs;
using Luban.RawDefs;

namespace Luban.Pipeline;

public interface IPipeline
{
    void Run(PipelineArguments args);
}
