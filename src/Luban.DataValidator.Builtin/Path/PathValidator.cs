using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Luban.Validator;

namespace Luban.DataValidator.Builtin.Path;

[Validator("path")]
public class PathValidator : DataValidatorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
    
    private string _rawPattern;

    private IPathPattern _pathPattern;

    public PathValidator()
    {
    }

    public override void Compile(DefField field, TType type)
    {
        this._rawPattern = DefUtil.TrimBracePairs(Args);

        if (type is not TString)
        {
            ThrowCompileError(field, "只支持string类型");
        }
        
        string[] ss = _rawPattern.Split(';');
        if (ss.Length < 1)
        {
            ThrowCompileError(field, "");
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
                    ThrowCompileError(field, "");
                }
                string pat = ss[1];
                int indexOfStar = pat.IndexOf('*');
                if (indexOfStar < 0)
                {
                    ThrowCompileError(field, "必须包含 * ");
                }
                _pathPattern = new SimpleReplacePattern(pat.Substring(0, indexOfStar), pat.Substring(indexOfStar + 1));
                break;
            }
            case "unity":
            {
                if (ss.Length != 1)
                {
                    ThrowCompileError(field, "");
                }
                _pathPattern = new UnityAddressablePattern();
                break;
            }
            case "ue":
            {
                if (ss.Length != 1)
                {
                    ThrowCompileError(field, "");
                }
                _pathPattern = new Ue4ResourcePattern();
                break;
            }
            default:
            {
                ThrowCompileError(field, $"不支持的path模式类型:{patType}");
                break;
            }
        }

        _pathPattern.EmptyAble = emptyAble;
    }

    public override void Validate(DataValidatorContext ctx, TType type, DType data)
    {
        string value = ((DString)data).Value;
        if (value == "" && _pathPattern.EmptyAble)
        {
            return;
        }

        string rootDir = EnvManager.Current.GetOption(BuiltinOptionNames.PathValidatorFamily, BuiltinOptionNames.PathValidatorRootDir, false);

        if (!_pathPattern.ExistPath(rootDir, value))
        {
            s_logger.Error("{}:{} (来自文件:{}) 找不到对应文件", RecordPath, value, Source);
            GenerationContext.Current.LogValidatorFail(this);
        }
    }

    private void ThrowCompileError(DefField def, string err)
    {
        throw new System.ArgumentException($"field:{def} {_rawPattern} 定义不合法. {err}");
    }
}