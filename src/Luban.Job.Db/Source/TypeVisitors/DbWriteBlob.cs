using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Db.TypeVisitors
{
    class DbWriteBlob
    {
        public static DbCsCompatibleSerializeVisitor Ins { get; } = new DbCsCompatibleSerializeVisitor();

    }
}
