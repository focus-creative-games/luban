using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Cpp.TypeVisitors;

public class CppRawptrDeserializeVisitor : DecoratorFuncVisitor<string, string, int, string>
{
    public static CppRawptrDeserializeVisitor Ins { get; } = new CppRawptrDeserializeVisitor();

    public override string DoAccept(TType type, string bufName, string fieldName, int depth)
    {
        if (type.IsNullable)
        {
            return $"{{ bool _has_value_; if(!{bufName}.readBool(_has_value_)){{return false;}}  if(_has_value_) {{ {fieldName} = {(type.IsBean ? "nullptr" : $"new {type.Apply(CppUnderlyingDeclaringTypeNameVisitor.Ins)}{{}}")}; {type.Apply(CppRawptrUnderlyingDeserializeVisitor.Ins, bufName, type.IsBean ? fieldName : $"*{fieldName}",depth + 1, CppRawptrDeclaringTypeNameVisitor.Ins)} }} else {{ {fieldName} = nullptr; }} }}";
        }
        else
        {
            return type.Apply(CppRawptrUnderlyingDeserializeVisitor.Ins, bufName, fieldName, depth, CppRawptrDeclaringTypeNameVisitor.Ins);
        }
    }
}
