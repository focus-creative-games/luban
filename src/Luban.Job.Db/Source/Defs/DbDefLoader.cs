using Luban.Common.Utils;
using Luban.Job.Common.Defs;
using Luban.Job.Db.RawDefs;
using Luban.Server.Common;
using System.Collections.Generic;
using System.Xml.Linq;

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
            var defines = new Defines()
            {
                DbTables = _tables,
            };
            BuildCommonDefines(defines);
            return defines;
        }

        private readonly List<string> _tableOptionalAttrs = new List<string> { "memory", "comment" };
        private readonly List<string> _tableRequireAttrs = new List<string> { "name", "id", "key", "value" };

        private void AddTable(string defineFile, XElement e)
        {
            ValidAttrKeys(defineFile, e, _tableOptionalAttrs, _tableRequireAttrs);
            var p = new Table()
            {
                Id = XmlUtil.GetRequiredIntAttribute(e, "id"),
                Name = XmlUtil.GetRequiredAttribute(e, "name"),
                Namespace = CurNamespace,
                KeyType = XmlUtil.GetRequiredAttribute(e, "key"),
                ValueType = XmlUtil.GetRequiredAttribute(e, "value"),
                IsPersistent = !XmlUtil.GetOptionBoolAttribute(e, "memory"),
                Comment = XmlUtil.GetOptionalAttribute(e, "comment"),
            };

            s_logger.Trace("add Db:{@Db}", p);
            _tables.Add(p);
        }



    }
}
