using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    class LuaUnderingSerializeVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static LuaUnderingSerializeVisitor Ins { get; } = new LuaUnderingSerializeVisitor();

        public override string DoAccept(TType type, string bufName, string fieldName)
        {
            return $"{type.Apply(LuaSerializeMethodNameVisitor.Ins)}({bufName}, {fieldName})";
        }

        public override string Accept(TText type, string bufName, string fieldName)
        {
            return $"readString({bufName}); {fieldName} = readString({bufName})";
        }

        public override string Accept(TArray type, string bufName, string fieldName)
        {
            return $"writeArray({bufName}, {fieldName}, {type.ElementType.Apply(LuaSerializeMethodNameVisitor.Ins)})";
        }

        public override string Accept(TList type, string bufName, string fieldName)
        {
            return $"writeList({bufName}, {fieldName}, {type.ElementType.Apply(LuaSerializeMethodNameVisitor.Ins)})";
        }

        public override string Accept(TSet type, string bufName, string fieldName)
        {
            return $"writeBool({bufName}, {fieldName}, {type.ElementType.Apply(LuaSerializeMethodNameVisitor.Ins)})";
        }

        public override string Accept(TMap type, string bufName, string fieldName)
        {
            return $"writeBool({bufName}, {fieldName}, {type.KeyType.Apply(LuaSerializeMethodNameVisitor.Ins)}, {type.ValueType.Apply(LuaSerializeMethodNameVisitor.Ins)})";
        }
    }
}
