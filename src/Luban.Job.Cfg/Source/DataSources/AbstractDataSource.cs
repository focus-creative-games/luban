using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using System.Collections.Generic;
using System.IO;

namespace Luban.Job.Cfg.DataSources
{
    abstract class AbstractDataSource
    {
        public abstract DType ReadOne(TBean type);

        public abstract List<DType> ReadMulti(TBean type);

        public abstract void Load(string rawUrl, string sheetName, Stream stream, bool exportDebugData);
    }
}
