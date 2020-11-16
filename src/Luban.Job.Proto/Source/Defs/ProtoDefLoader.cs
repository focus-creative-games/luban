using Luban.Job.Common.Utils;
using Luban.Job.Common.Defs;
using Luban.Server.Common;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Luban.Common.Utils;
using Luban.Job.Proto.RawDefs;

namespace Luban.Job.Proto.Defs
{
    class ProtoDefLoader : CommonDefLoader
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly List<PProto> _protos = new List<PProto>();
        private readonly List<PRpc> _rpcs = new List<PRpc>();

        public ProtoDefLoader(RemoteAgent agent) : base(agent)
        {
            RegisterRootDefineHandler("service", AddService);
            RegisterModuleDefineHandler("proto", AddProto);
            RegisterModuleDefineHandler("rpc", AddRpc);
        }

        public Defines BuildDefines()
        {
            return new Defines();
        }

        private readonly List<string> rpcAttrs = new List<string> { "id" };
        private readonly List<string> rpcRequiredAttrs = new List<string> { "name", "arg", "res" };
        private void AddRpc(XElement e)
        {
            ValidAttrKeys(e, rpcAttrs, rpcRequiredAttrs);
            var r = new PRpc()
            {
                Name = XmlUtil.GetRequiredAttribute(e, "name"),
                Namespace = CurNamespace,
                ArgType = XmlUtil.GetRequiredAttribute(e, "arg"),
                ResType = XmlUtil.GetRequiredAttribute(e, "res"),
            };
            s_logger.Trace("add rpc:{@rpc}", r);
            _rpcs.Add(r);
        }


        private readonly List<string> protoAttrs = new List<string> { "id" };
        private readonly List<string> protoRequiredAttrs = new List<string> { "name" };

        private void AddProto(XElement e)
        {
            ValidAttrKeys(e, protoAttrs, protoRequiredAttrs);

            var p = new PProto()
            {
                Name = XmlUtil.GetRequiredAttribute(e, "name"),
                Namespace = CurNamespace,
                Id = XmlUtil.GetOptionIntAttribute(e, "id"),
            };

            foreach (XElement fe in e.Elements())
            {
                switch (fe.Name.LocalName)
                {
                    case "var":
                    {
                        p.Fields.Add(CreateField(fe)); ;
                        break;
                    }
                    default:
                    {
                        throw new Exception($"定义文件:{CurImportFile} 不支持 tag:{fe.Name}");
                    }
                }

            }

            s_logger.Trace("add proto:{@proto}", p);
            _protos.Add(p);
        }

        private readonly List<string> serviceAttrs = new List<string> { "name" };
        private void AddService(XElement e)
        {
            var name = XmlUtil.GetRequiredAttribute(e, "name");
            s_logger.Trace("service {service}", name);
            ValidAttrKeys(e, serviceAttrs, serviceAttrs);
            foreach (XElement ele in e.Elements())
            {
                s_logger.Trace("service {service_name} node: {name} {value}", name, ele.Name, ele.Attribute("value")?.Value);
            }
        }

        protected override void AddBean(XElement e, string parent)
        {
            throw new NotImplementedException();
        }
    }
}
