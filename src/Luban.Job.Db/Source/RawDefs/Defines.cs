using Luban.Job.Common.RawDefs;
using System.Collections.Generic;

namespace Luban.Job.Db.RawDefs
{
    public class Defines : DefinesCommon
    {
        public List<Table> DbTables { get; set; } = new List<Table>();
    }
}
