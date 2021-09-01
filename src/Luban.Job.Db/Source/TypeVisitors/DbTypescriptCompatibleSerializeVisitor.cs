using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Db.TypeVisitors
{
    class DbTypescriptCompatibleSerializeVisitor : TypescriptBinUnderingSerializeVisitor
    {
        public static new DbTypescriptCompatibleSerializeVisitor Ins { get; } = new();

        public override string Accept(TBean type, string bufName, string fieldName)
        {
            var bean = type.Bean;
            if (bean.IsNotAbstractType)
            {
                return $"{{{fieldName}.serialize({bufName});}}";
            }
            else
            {
                return $"{{{bean.FullName}.serialize{bean.Name}Any({bufName}, {fieldName});}}";
            }
        }

        private string BeginSegment(TType type, string bufName)
        {
            if (type.Apply(CompatibleSerializeNeedEmbedVisitor.Ins))
            {
                return $"let _state2_ = {bufName}.BeginWriteSegment();";
            }
            else
            {
                return "";
            }
        }

        private string EndSegment(TType type, string bufName)
        {
            if (type.Apply(CompatibleSerializeNeedEmbedVisitor.Ins))
            {
                return $"{bufName}.EndWriteSegment(_state2_);";
            }
            else
            {

                return "";
            }
        }

        public override string Accept(TList type, string bufName, string fieldName)
        {
            return $"{{ {bufName}.WriteInt(FieldTag.{type.ElementType.Apply(TagNameVisitor.Ins)}); {bufName}.WriteSize({fieldName}.length); for(let _e of {fieldName}) {{ {BeginSegment(type.ElementType, bufName)} {type.ElementType.Apply(this, bufName, "_e")}; {EndSegment(type.ElementType, bufName)} }}  }}";
        }

        public override string Accept(TSet type, string bufName, string fieldName)
        {
            return $"{{ {bufName}.WriteInt(FieldTag.{type.ElementType.Apply(TagNameVisitor.Ins)}); {bufName}.WriteSize({fieldName}.length);   for(let _e of {fieldName}) {{ {type.ElementType.Apply(this, bufName, "_e")} }} }}";
        }

        public override string Accept(TMap type, string bufName, string fieldName)
        {
            return $"{{ {bufName}.WriteInt(FieldTag.{type.KeyType.Apply(TagNameVisitor.Ins)}); {bufName}.WriteInt(FieldTag.{type.ValueType.Apply(TagNameVisitor.Ins)}); {bufName}.WriteSize({fieldName}.length);   for(let [_k, _v] of {fieldName}.entries()) {{ {type.KeyType.Apply(this, bufName, "_k")}; {BeginSegment(type.ValueType, bufName)} {type.ValueType.Apply(this, bufName, "_v")}; {EndSegment(type.ValueType, bufName)} }} }}";
        }
    }
}
