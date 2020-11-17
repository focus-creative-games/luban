using Luban.Job.Common.Utils;
using Luban.Job.Common.Defs;
using System.Collections.Generic;
using System.Xml.Linq;
using Luban.Server.Common;
using Luban.Common.Utils;
using Luban.Job.Db.RawDefs;

namespace Luban.Job.Db.Defs
{
    class DbDefLoader : CommonDefLoader
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly List<Table> _tables = new List<Table>();

        public DbDefLoader(RemoteAgent agent) : base(agent)
        {
            RegisterModuleDefineHandler("table", AddTable);
        }

        public Defines BuildDefines()
        {
            return new Defines()
            {
                TopModule = TopModule,
                Consts = _consts,
                Enums = _enums,
                Beans = _beans,
                DbTables = _tables,
            };
        }



        private readonly List<string> _tableOptionalAttrs = new List<string> { "memory" };
        private readonly List<string> _tableRequireAttrs = new List<string> { "name", "id", "key", "value" };

        private void AddTable(XElement e)
        {
            ValidAttrKeys(e, _tableOptionalAttrs, _tableRequireAttrs);
            var p = new Table()
            {
                Id = XmlUtil.GetRequiredIntAttribute(e, "id"),
                Name = XmlUtil.GetRequiredAttribute(e, "name"),
                Namespace = CurNamespace,
                KeyType = XmlUtil.GetRequiredAttribute(e, "key"),
                ValueType = XmlUtil.GetRequiredAttribute(e, "value"),
                IsPersistent = !XmlUtil.GetOptionBoolAttribute(e, "memory"),
            };

            s_logger.Trace("add Db:{@Db}", p);
            _tables.Add(p);
        }



    }
}
