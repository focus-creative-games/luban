using Bright.Net.Codecs;
using System.Collections.Generic;

namespace Luban.Common.Protos
{
    public static class ProtocolStub
    {
        public static Dictionary<int, ProtocolCreator> Factories { get; } = new Dictionary<int, ProtocolCreator>
        {
            [GetInputFile.ID] = () => new GetInputFile(),
            [GetOutputFile.ID] = () => new GetOutputFile(),
            [PushLog.ID] = () => new PushLog(),
            [PushException.ID] = () => new PushException(),
            [GenJob.ID] = () => new GenJob(),
            [GetImportFileOrDirectory.ID] = () => new GetImportFileOrDirectory(),
            [QueryFilesExists.ID] = () => new QueryFilesExists(),
        };
    }
}
