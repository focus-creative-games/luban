using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Luban.Job.Cfg.DataExporters
{
    class YamlExportor : IDataFuncVisitor<YamlNode>
    {
        public static YamlExportor Ins { get; } = new YamlExportor();

        public YamlNode WriteAsArray(List<Record> datas)
        {

            var seqNode = new YamlSequenceNode();
            foreach (var d in datas)
            {
                seqNode.Add(d.Data.Apply(this));
            }
            return seqNode;
        }


        private static YamlScalarNode ToPlainNode(string x)
        {
            return new YamlScalarNode(x) { Style = ScalarStyle.Plain };
        }

        private static YamlScalarNode ToText(string x)
        {
            return new YamlScalarNode(x) { Style = ScalarStyle.SingleQuoted };
        }

        public YamlNode Accept(DBool type)
        {
            return ToPlainNode(type.Value ? "true" : "false");
        }

        public YamlNode Accept(DByte type)
        {
            return ToPlainNode(type.Value.ToString());
        }

        public YamlNode Accept(DShort type)
        {
            return ToPlainNode(type.Value.ToString());
        }

        public YamlNode Accept(DFshort type)
        {
            return ToPlainNode(type.Value.ToString());
        }

        public YamlNode Accept(DInt type)
        {
            return ToPlainNode(type.Value.ToString());
        }

        public YamlNode Accept(DFint type)
        {
            return ToPlainNode(type.Value.ToString());
        }

        public YamlNode Accept(DLong type)
        {
            return ToPlainNode(type.Value.ToString());
        }

        public YamlNode Accept(DFlong type)
        {
            return ToPlainNode(type.Value.ToString());
        }

        public YamlNode Accept(DFloat type)
        {
            return ToPlainNode(type.Value.ToString());
        }

        public YamlNode Accept(DDouble type)
        {
            return ToPlainNode(type.Value.ToString());
        }

        public YamlNode Accept(DEnum type)
        {
            return ToPlainNode(type.Value.ToString());
        }

        public YamlNode Accept(DString type)
        {
            return ToText(type.Value);
        }

        public YamlNode Accept(DBytes type)
        {
            throw new NotSupportedException();
        }

        public YamlNode Accept(DText type)
        {
            var m = new YamlMappingNode();
            m.Add(DText.KEY_NAME, ToText(type.Key));
            m.Add(DText.TEXT_NAME, ToText(type.TextOfCurrentAssembly));
            return m;
        }

        public YamlNode Accept(DBean type)
        {
            var m = new YamlMappingNode();

            if (type.Type.IsAbstractType)
            {
                m.Add(DefBean.JSON_TYPE_NAME_KEY, ToText(DataUtil.GetImplTypeName(type)));
            }
            var defFields = type.ImplType.HierarchyFields;
            int index = 0;
            foreach (var d in type.Fields)
            {
                var defField = (DefField)defFields[index++];

                // 特殊处理 bean 多态类型
                // 另外，不生成  xxx:null 这样
                if (d == null || !defField.NeedExport)
                {
                    //x.WriteNullValue();
                }
                else
                {
                    m.Add(defField.Name, d.Apply(this));
                }
            }

            return m;
        }

        public YamlSequenceNode ToSeqNode(List<DType> datas)
        {
            var seqNode = new YamlSequenceNode();
            foreach (var d in datas)
            {
                seqNode.Add(d.Apply(this));
            }
            return seqNode;
        }

        public YamlNode Accept(DArray type)
        {
            return ToSeqNode(type.Datas);
        }

        public YamlNode Accept(DList type)
        {
            return ToSeqNode(type.Datas);
        }

        public YamlNode Accept(DSet type)
        {
            return ToSeqNode(type.Datas);
        }

        public YamlNode Accept(DMap type)
        {
            var seqNode = new YamlSequenceNode();
            foreach (var d in type.Datas)
            {
                var e = new YamlSequenceNode();
                e.Add(d.Key.Apply(this));
                e.Add(d.Value.Apply(this));
                seqNode.Add(e);
            }
            return seqNode;
        }

        public YamlNode Accept(DVector2 type)
        {
            var m = new YamlMappingNode();
            m.Add("x", type.Value.X.ToString());
            m.Add("y", type.Value.Y.ToString());
            return m;
        }

        public YamlNode Accept(DVector3 type)
        {
            var m = new YamlMappingNode();
            m.Add("x", type.Value.X.ToString());
            m.Add("y", type.Value.Y.ToString());
            m.Add("z", type.Value.Z.ToString());
            return m;
        }

        public YamlNode Accept(DVector4 type)
        {
            var m = new YamlMappingNode();
            m.Add("x", type.Value.X.ToString());
            m.Add("y", type.Value.Y.ToString());
            m.Add("z", type.Value.Z.ToString());
            m.Add("w", type.Value.W.ToString());
            return m;
        }

        public YamlNode Accept(DDateTime type)
        {
            return ToPlainNode(type.UnixTimeOfCurrentAssembly.ToString());
        }
    }
}
