using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Luban.Job.Cfg.DataSources.Xml
{
    class XmlDataSource : AbstractDataSource
    {
        private XElement _doc;
        public override void Load(string rawUrl, string sheetName, Stream stream, bool exportDebugData)
        {
            _doc = XElement.Load(stream);
        }

        public override List<DType> ReadMulti(TBean type)
        {
            throw new NotSupportedException();
        }

        public override DType ReadOne(TBean type)
        {
            return type.Apply(XmlDataCreator.Ins, _doc, (DefAssembly)type.Bean.AssemblyBase);
        }
    }
}
