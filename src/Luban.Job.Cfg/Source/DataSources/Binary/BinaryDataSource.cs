using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace Luban.Job.Cfg.DataSources.Binary
{
    class BinaryDataSource : AbstractDataSource
    {
        public override void Load(string rawUrl, string sheetName, Stream stream)
        {
            throw new NotImplementedException();
        }

        public override List<Record> ReadMulti(TBean type)
        {
            throw new NotImplementedException();
        }

        public override Record ReadOne(TBean type)
        {
            throw new NotImplementedException();
        }
    }
}
