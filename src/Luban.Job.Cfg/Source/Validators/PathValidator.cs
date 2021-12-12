using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Luban.Job.Cfg.Validators
{
    interface IPathPattern
    {
        string Mode { get; }

        object CalcFinalPath(string path);

        bool EmptyAble { get; set; }
    }

    class RegexPattern : IPathPattern
    {
        private readonly string _replacePattern;

        private readonly Regex _re;

        public bool EmptyAble { get; set; }

        public string Mode => "regex";

        public RegexPattern(string matchPattern, string replacePattern)
        {
            _re = new Regex(matchPattern);
            _replacePattern = replacePattern;
        }

        public object CalcFinalPath(string path)
        {
            var finalPath = _re.Replace(path, _replacePattern);
            if (finalPath == path && !_re.IsMatch(path))
            {
                return null;
            }
            return finalPath;
        }
    }

    class SimpleReplacePattern : IPathPattern
    {
        private readonly string _prefix;
        private readonly string _suffix;

        public bool EmptyAble { get; set; }

        public string Mode => "normal";

        public SimpleReplacePattern(string prefix, string suffix)
        {
            _prefix = prefix;
            _suffix = suffix;
        }

        public object CalcFinalPath(string path)
        {
            return _prefix + path + _suffix;
        }
    }

    class UnityAddressablePattern : IPathPattern
    {
        public bool EmptyAble { get; set; }

        public string Mode => "unity";

        public UnityAddressablePattern()
        {
        }

        public object CalcFinalPath(string path)
        {
            return path;
        }
    }

    class Ue4ResourcePattern : IPathPattern
    {
        private readonly Regex _pat1;
        private readonly Regex _pat2;

        public bool EmptyAble { get; set; }

        public string Mode => "ue";

        public Ue4ResourcePattern()
        {
            _pat1 = new Regex(@"^/Game/(.+?)(\..+)?$");
            _pat2 = new Regex(@"^\w+'/Game/(.+?)(\..+)?'$");
        }

        private bool CheckMatch(Match match)
        {
            var groups = match.Groups;
            if (!groups[1].Success)
            {
                return false;
            }
            if (groups[2].Success)
            {
                // 如果是  /Game/../xxx.yyy 的情形
                // 要求 yyy == xxx 或者 yyy == xxx_C
                string path = groups[1].Value;
                string suffix = groups[2].Value.Substring(1);
                if (suffix.EndsWith("_C"))
                {
                    suffix = suffix[0..^2];
                }
                return path.EndsWith(suffix);
            }
            return true;
        }

        private List<string> AlternativePaths(string rawPath)
        {
            return new List<string>() { rawPath + ".uasset", rawPath + ".umap" };
        }

        public object CalcFinalPath(string path)
        {
            var match1 = _pat1.Match(path);
            if (match1.Success)
            {
                if (!CheckMatch(match1))
                {
                    return null;
                }
                return AlternativePaths(match1.Groups[1].Value);
            }
            var match2 = _pat2.Match(path);
            if (match2.Success)
            {
                if (!CheckMatch(match2))
                {
                    return null;
                }
                return AlternativePaths(match2.Groups[1].Value);
            }
            return null;
        }
    }

    [Validator("path")]
    public class PathValidator : IValidator
    {
        public string RawPattern { get; }

        public TType Type { get; }

        internal IPathPattern PathPattern { get; private set; }

        public PathValidator(TType type, string pathPattern)
        {
            Type = type;
            this.RawPattern = DefUtil.TrimBracePairs(pathPattern);
        }

        public void Validate(ValidatorContext ctx, TType type, DType data)
        {
            var assembly = ctx.Assembly;

            if (type.IsNullable && data == null)
            {
                return;
            }
            if (data is DString s)
            {
                string value = s.Value;
                if (value == "" && PathPattern.EmptyAble)
                {
                    return;
                }

                string source = ValidatorContext.CurrentVisitor.CurrentValidateRecord.Source;
                object finalPaths = PathPattern.CalcFinalPath(value);
                if (finalPaths == null)
                {
                    assembly.Agent.Error("{0}:{1} (来自文件:{2}) 资源格式不合法", ValidatorContext.CurrentRecordPath, value, source);
                    return;
                }
                ctx.AddPathQuery(new PathQuery
                {
                    Validator = this,
                    DataPath = ValidatorContext.CurrentRecordPath,
                    Value = value,
                    Source = source,
                    QueryPath = finalPaths
                });
                return;
            }
            else
            {
                throw new ArgumentException($" path 检查只支持string, 但 {ValidatorContext.CurrentRecordPath} 不是string类型");
            }
        }

        private void ThrowCompileError(DefFieldBase def, string err)
        {
            throw new System.ArgumentException($"{((DefBean)(def.HostType)).FullName} 字段:{def.Name} {RawPattern} 定义不合法. {err}");
        }

        public void Compile(DefFieldBase def)
        {
            string[] ss = RawPattern.Split(';');
            if (ss.Length < 1)
            {
                ThrowCompileError(def, "");
            }

            string patType = ss[0];
            bool emptyAble = false;
            if (patType.EndsWith('?'))
            {
                patType = patType[0..^1];
                emptyAble = true;
            }

            switch (patType)
            {
                case "normal":
                {
                    if (ss.Length != 2)
                    {
                        ThrowCompileError(def, "");
                    }
                    string pat = ss[1];
                    int indexOfStar = pat.IndexOf('*');
                    if (indexOfStar < 0)
                    {
                        ThrowCompileError(def, "必须包含 * ");
                    }
                    PathPattern = new SimpleReplacePattern(pat.Substring(0, indexOfStar), pat.Substring(indexOfStar + 1));
                    break;
                }
                case "regex":
                {
                    if (ss.Length != 3)
                    {
                        ThrowCompileError(def, "必须包含 pattern和replace");
                    }
                    PathPattern = new RegexPattern(ss[1], ss[2]);
                    break;
                }
                case "unity":
                {
                    if (ss.Length != 1)
                    {
                        ThrowCompileError(def, "");
                    }
                    PathPattern = new UnityAddressablePattern();
                    break;
                }
                case "ue":
                {
                    if (ss.Length != 1)
                    {
                        ThrowCompileError(def, "");
                    }
                    PathPattern = new Ue4ResourcePattern();
                    break;
                }
                default:
                {
                    ThrowCompileError(def, $"不支持的path模式类型:{patType}");
                    break;
                }
            }

            PathPattern.EmptyAble = emptyAble;
        }
    }
}
