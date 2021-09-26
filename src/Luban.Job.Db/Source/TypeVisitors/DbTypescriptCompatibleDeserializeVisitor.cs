using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Db.TypeVisitors
{
    class DbTypescriptCompatibleDeserializeVisitor : TypescriptBinUnderingDeserializeVisitorBase
    {
        public static DbTypescriptCompatibleDeserializeVisitor Ins { get; } = new();

        public override string Accept(TBean type, string bufName, string fieldName)
        {
            if (type.Bean.IsAbstractType)
            {
                return $"{{ {fieldName} = {type.Bean.FullName}.deserialize{type.Bean.Name}Any({bufName}); }}";
            }
            else
            {
                return $"{{ {fieldName} = new {type.Bean.FullName}(); {fieldName}.deserialize({bufName}); }}";
            }
        }

        private string BeginSegment(TType type, string bufName)
        {
            if (type.Apply(CompatibleSerializeNeedEmbedVisitor.Ins))
            {
                return $"let _state2_ = {bufName}.EnterSegment();";
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
                return $"{bufName}.LeaveSegment(_state2_);";
            }
            else
            {

                return "";
            }
        }

        public override string Accept(TList type, string bufName, string fieldName)
        {
            return $"{{ /*let _tagType_ = */{bufName}.ReadInt(); for(let i = 0, n = {bufName}.ReadSize() ; i < n ; i++) {{ {BeginSegment(type.ElementType, bufName)} let _e :{type.ElementType.Apply(TypescriptDefineTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, bufName, "_e")}; {EndSegment(type.ElementType, bufName)} {fieldName}.add(_e) }} }}";
        }

        public override string Accept(TSet type, string bufName, string fieldName)
        {
            return $"{{ /*let _tagType_ = */{bufName}.ReadInt(); for(let i = 0, n = {bufName}.ReadSize() ; i < n ; i++) {{ let _e:{type.ElementType.Apply(TypescriptDefineTypeNameVisitor.Ins)};{type.ElementType.Apply(this, bufName, "_e")}; {fieldName}.add(_e);}}}}";
        }

        public override string Accept(TMap type, string bufName, string fieldName)
        {
            return $"{{ /*let _keyTagType_ = */{bufName}.ReadInt(); let _valueTagType_ = {bufName}.ReadInt(); for(let i = 0, n = {bufName}.ReadSize() ; i < n ; i++) {{ let _k:{type.KeyType.Apply(TypescriptDefineTypeNameVisitor.Ins)}; {type.KeyType.Apply(this, bufName, "_k")}; {BeginSegment(type.ValueType, bufName)} let _v:{type.ValueType.Apply(TypescriptDefineTypeNameVisitor.Ins)}; {type.ValueType.Apply(this, bufName, "_v")}; {EndSegment(type.ValueType, bufName)} {fieldName}.set(_k, _v);  }} }}";
        }
    }
}
