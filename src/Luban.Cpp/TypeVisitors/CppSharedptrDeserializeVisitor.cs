using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Cpp.TypeVisitors;

public class CppSharedptrDeserializeVisitor : DecoratorFuncVisitor<string, string, int, string>
{
    public static CppSharedptrDeserializeVisitor Ins { get; } = new CppSharedptrDeserializeVisitor();

    public override string DoAccept(TType type, string bufName, string fieldName, int depth)
    {
        if (type.IsNullable)
        {
            return $"{{ bool _has_value_; if(!{bufName}.readBool(_has_value_)){{return false;}}  if(_has_value_) {{ {fieldName}.reset({(type.IsBean ? "" : $"new {type.Apply(CppUnderlyingDeclaringTypeNameVisitor.Ins)}()")}); {type.Apply(CppSharedptrUnderlyingDeserializeVisitor.Ins, bufName, $"{(type.IsBean ? "" : "*")}{fieldName}",depth+1, CppSharedptrDeclaringTypeNameVisitor.Ins)} }} else {{ {fieldName}.reset(); }} }}";
        }
        else
        {
            return type.Apply(CppSharedptrUnderlyingDeserializeVisitor.Ins, bufName, fieldName, depth ,CppSharedptrDeclaringTypeNameVisitor.Ins);
        }
    }
}
