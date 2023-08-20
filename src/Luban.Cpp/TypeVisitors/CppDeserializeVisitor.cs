using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Cpp.TypeVisitors;

public class CppDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
{
    public static CppDeserializeVisitor Ins { get; } = new CppDeserializeVisitor();

    public override string DoAccept(TType type, string bufName, string fieldName)
    {
        if (type.IsNullable)
        {
            return $"{{ bool _has_value_; if(!{bufName}.readBool(_has_value_)){{return false;}}  if(_has_value_) {{ {fieldName}.reset({(type.IsBean ? "" : $"new {type.Apply(CppUnderlyingDeclaringTypeNameVisitor.Ins)}()")}); {type.Apply(CppUnderlyingDeserializeVisitor.Ins, bufName, $"{(type.IsBean ? "" : "*")}{fieldName}")} }} else {{ {fieldName}.reset(); }} }}";
        }
        else
        {
            return type.Apply(CppUnderlyingDeserializeVisitor.Ins, bufName, fieldName);
        }
    }
}