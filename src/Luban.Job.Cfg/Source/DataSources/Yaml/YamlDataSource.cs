using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.RepresentationModel;

namespace Luban.Job.Cfg.DataSources.Yaml
{
    class YamlDataSource : AbstractDataSource
    {
        private YamlNode _root;
        public override void Load(string rawUrl, string sheetOrFieldName, Stream stream)
        {
            var ys = new YamlStream();
            ys.Load(new StreamReader(stream));
            var rootNode = ys.Documents[0].RootNode;

            this._root = rootNode;

            if (!string.IsNullOrEmpty(sheetOrFieldName))
            {
                if (sheetOrFieldName.StartsWith("*"))
                {
                    sheetOrFieldName = sheetOrFieldName.Substring(1);
                }
                if (!string.IsNullOrEmpty(sheetOrFieldName))
                {
                    foreach (var subField in sheetOrFieldName.Split('.'))
                    {
                        this._root = _root[new YamlScalarNode(subField)];
                    }
                }
            }
        }

        public override List<Record> ReadMulti(TBean type)
        {
            var records = new List<Record>();
            foreach (var ele in (YamlSequenceNode)_root)
            {
                var rec = ReadRecord(ele, type);
                if (rec != null)
                {
                    records.Add(rec);
                }
            }
            return records;
        }

        private static readonly YamlScalarNode s_tagNameNode = new(TAG_KEY);

        public override Record ReadOne(TBean type)
        {
            return ReadRecord(_root, type);
        }

        private Record ReadRecord(YamlNode yamlNode, TBean type)
        {
            string tagName;
            if (((YamlMappingNode)yamlNode).Children.TryGetValue(s_tagNameNode, out var tagNode))
            {
                tagName = (string)tagNode;
            }
            else
            {
                tagName = null;
            }
            if (DataUtil.IsIgnoreTag(tagName))
            {
                return null;
            }
            var data = (DBean)type.Apply(YamlDataCreator.Ins, yamlNode, (DefAssembly)type.Bean.AssemblyBase);
            var tags = DataUtil.ParseTags(tagName);
            return new Record(data, RawUrl, tags);
        }
    }
}
