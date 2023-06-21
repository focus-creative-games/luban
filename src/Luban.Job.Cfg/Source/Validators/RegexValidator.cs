using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using System;
using System.Text.RegularExpressions;

namespace Luban.Job.Cfg.Validators
{
    [Validator("regex")]
    class RegexValidator : IValidator
    {
        public TType Type { get; }
        private Regex _regex;

        public string Source => ValidatorContext.CurrentVisitor.CurrentValidateRecord.Source;


        public RegexValidator(TType type, string strRegex)
        {
            Type = type;
            _regex = new Regex(DataUtil.UnEscapeString(strRegex), RegexOptions.Compiled);
        }

        public void Compile(DefFieldBase def)
        {
            switch (Type)
            {
                case TBean:
                    throw new Exception($"regex检查器不支持检查Bean类型：{Type.TypeName}");
            }
        }

        public void Validate(ValidatorContext ctx, TType type, DType data)
        {
            Validate(ctx, type.IsNullable, data);
        }

        private void Validate(ValidatorContext ctx, bool nullable, DType data)
        {
            var assembly = ctx.Assembly;
            void Check(string str)
            {
                var match = _regex.Match(str);
                if (!match.Success)
                {
                    assembly.Agent.Error($"记录 {ValidatorContext.CurrentRecordPath}:{data} (来自文件:{Source}) 不符合正则表达式：'{_regex}'");
                }
            }

            if (nullable && data == null)
            {
                return;
            }
            switch (data)
            {
                case DList dList:
                {
                    foreach (var d in dList.Datas)
                    {
                        Validate(ctx, nullable, d);
                    }
                    break;
                }
                case DArray dArray:
                {
                    foreach (var d in dArray.Datas)
                    {
                        Validate(ctx, nullable, d);
                    }
                    break;
                }
                case DSet dSet:
                {
                    foreach (var d in dSet.Datas)
                    {
                        Validate(ctx, nullable, d);
                    }
                    break;
                }
                case DMap dMap:
                {
                    foreach (var d in dMap.Datas.Values)
                    {
                        Validate(ctx, nullable, d);
                    }
                    break;
                }
                case DVector2 dVector2:
                {
                    Check(dVector2.Value.X.ToString());
                    Check(dVector2.Value.Y.ToString());
                    break;
                }
                case DVector3 dVector3:
                {
                    Check(dVector3.Value.X.ToString());
                    Check(dVector3.Value.Y.ToString());
                    Check(dVector3.Value.Z.ToString());
                    break;
                }
                case DVector4 dVector4:
                {
                    Check(dVector4.Value.X.ToString());
                    Check(dVector4.Value.Y.ToString());
                    Check(dVector4.Value.Z.ToString());
                    Check(dVector4.Value.W.ToString());
                    break;
                }
                case DText dText:
                {
                    Check(dText.RawValue);
                    break;
                }
                default:
                    Check(data.ToString());
                    break;
            }
        }
    }
}
