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
        private YamlMappingNode _root;
        public override void Load(string rawUrl, string sheetName, Stream stream, bool exportDebugData)
        {
            var ys = new YamlStream();
            ys.Load(new StreamReader(stream));
            var rootNode = (YamlMappingNode)ys.Documents[0].RootNode;
            if (string.IsNullOrEmpty(sheetName))
            {
                this._root = rootNode;
            }
            else
            {
                if (rootNode.Children.TryGetValue(new YamlScalarNode(sheetName), out var childNode))
                {
                    this._root = (YamlMappingNode)childNode;
                }
                else
                {
                    throw new Exception($"yaml文件:{RawUrl} 不包含子字段:{sheetName}");
                }
            }
        }

        public override List<Record> ReadMulti(TBean type)
        {
            throw new NotImplementedException();
        }

        private readonly static YamlScalarNode s_tagNameNode = new(TAG_KEY);

        public override Record ReadOne(TBean type)
        {
            string tagName;
            if (_root.Children.TryGetValue(s_tagNameNode, out var tagNode))
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
            var data = (DBean)type.Apply(YamlDataCreator.Ins, _root, (DefAssembly)type.Bean.AssemblyBase);
            var tags = DataUtil.ParseTags(tagName);
            return new Record(data, RawUrl, tags);
        }
    }
}
