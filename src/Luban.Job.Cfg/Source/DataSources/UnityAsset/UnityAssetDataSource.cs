using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.RepresentationModel;
using System.Linq;

namespace Luban.Job.Cfg.DataSources.UnityAsset
{
    class UnityAssetDataSource : AbstractDataSource
    {
        private YamlNode _root;
        public override void Load(string rawUrl, string sheetOrFieldName, Stream stream)
        {
            var ys = new YamlStream();
            ys.Load(new StreamReader(stream));
            var rootNode = (YamlMappingNode)ys.Documents[0].RootNode;

            // unity asset 格式为 包含一个doc的 yaml文件
            // doc顶层为map，只包含一个字段，字段key为类型名。
            if (rootNode.Children.Count != 1)
            {
                throw new Exception($"asset doc 应该只包含一个顶层字段");
            }

            this._root = rootNode.First().Value;

            if (!string.IsNullOrEmpty(sheetOrFieldName))
            {
                if (sheetOrFieldName.StartsWith("*"))
                {
                    sheetOrFieldName = sheetOrFieldName[1..];
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
            var data = (DBean)type.Apply(UnityAssetDataCreator.Ins, yamlNode, (DefAssembly)type.Bean.AssemblyBase);
            var tags = DataUtil.ParseTags(tagName);
            return new Record(data, RawUrl, tags);
        }
    }
}
