using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.RawDefs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.l10n
{
    public class TextTable
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public DefAssembly Assembly { get; }

        private readonly Dictionary<string, string> _key2Texts = new();

        private readonly TBean _textRowType;

        public TextTable(DefAssembly ass, string textValueFieldName)
        {
            this.Assembly = ass;
            if (string.IsNullOrWhiteSpace(textValueFieldName))
            {
                textValueFieldName = "text";
            }
            var defTextRowType = new DefBean(new CfgBean()
            {
                Namespace = "__intern__",
                Name = "__TextInfo__",
                Parent = "",
                Alias = "",
                IsValueType = false,
                Sep = "",
                TypeId = 0,
                IsSerializeCompatible = false,
                Fields = new List<Common.RawDefs.Field>
                {
                    new CfgField() { Name = "key", Type = "string" },
                    new CfgField() { Name = textValueFieldName, Type = "string" },
                }
            })
            {
                AssemblyBase = ass,
            };
            defTextRowType.PreCompile();
            defTextRowType.Compile();
            defTextRowType.PostCompile();
            _textRowType = TBean.Create(false, defTextRowType, null);
        }

        public void AddText(string key, string text)
        {
            if (!_key2Texts.TryAdd(key, text))
            {
                throw new Exception($"text key:{key} 重复");
            }
        }

        public bool TryGetText(string key, out string text)
        {
            return _key2Texts.TryGetValue(key, out text);
        }

        public void LoadFromFile(string fileName, byte[] bytes)
        {
            var records = DataLoaderUtil.LoadCfgRecords(_textRowType, fileName, null, bytes, true, null);
            foreach (var r in records)
            {
                //s_logger.Info("== read text:{}", r.Data);
                string key = (r.Data.Fields[0] as DString).Value;
                string text = (r.Data.Fields[1] as DString).Value;
                if (!_key2Texts.TryAdd(key, text))
                {
                    throw new Exception($"TextTableFile:{fileName} key:{key} text:{text} 重复");
                }
            }
        }
    }
}
