using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class LuaUnderingDeserializeVisitor : DecoratorFuncVisitor<string, string>
    {
        public static LuaUnderingDeserializeVisitor Ins { get; } = new LuaUnderingDeserializeVisitor();

        public override string DoAccept(TType type, string x)
        {
            return $"{type.Apply(LuaDeserializeMethodNameVisitor.Ins)}({x})";
        }

        public override string Accept(TText type, string bufName)
        {
            return $"readString({bufName}) and readString({bufName})";
        }

        public override string Accept(TArray type, string x)
        {
            return $"readArray({x}, {type.ElementType.Apply(LuaDeserializeMethodNameVisitor.Ins)})";
        }

        public override string Accept(TList type, string x)
        {
            return $"readList({x}, {type.ElementType.Apply(LuaDeserializeMethodNameVisitor.Ins)})";
        }

        public override string Accept(TSet type, string x)
        {
            return $"readSet({x}, {type.ElementType.Apply(LuaDeserializeMethodNameVisitor.Ins)})";
        }

        public override string Accept(TMap type, string x)
        {
            return $"readMap({x}, {type.KeyType.Apply(LuaDeserializeMethodNameVisitor.Ins)}, {type.ValueType.Apply(LuaDeserializeMethodNameVisitor.Ins)})";
        }
    }
}
