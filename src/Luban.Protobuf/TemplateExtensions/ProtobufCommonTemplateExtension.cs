using Luban.Defs;
using Luban.Protobuf.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;
using System.Text;

namespace Luban.Protobuf.TemplateExtensions;

public class ProtobufCommonTemplateExtension : ScriptObject
{

    public static string FullName(DefTypeBase type)
    {
        return TypeUtil.MakePbFullName(type.Namespace, type.Name);
    }

    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(ProtobufTypeNameVisitor.Ins);
    }

    public static string SuffixOptions(TType type)
    {
        if (type.IsCollection && !(type is TMap))
        {
            return $"[packed = {(type.ElementType.Apply(IsProtobufPackedType.Ins) ? "true" : "false")}]";
        }
        else
        {
            return "";
        }
    }

    public static string GenArrayDimension(KeyValuePair<Type, (TType, int)> keyValuePair)
    {
        StringBuilder x = new StringBuilder();

        var type = keyValuePair.Value.Item1;

        var typeName = DeclaringTypeName(type);
        bool pack = type.Apply(IsProtobufPackedType.Ins);

        var Dimension = keyValuePair.Value.Item2;

        for (int i = Dimension - 1; i > 0; i--)
        {
            x.Append("message ");
            x.Append(typeName);

            int _dimension = Dimension - i;
            while (_dimension-- > 0)
            {
                x.Append("_Array");
            }
            x.AppendLine(" {");

            x.Append("    repeated ");
            x.Append(typeName);

            _dimension = Dimension - i;
            while (_dimension-- > 1)
            {
                x.Append("_Array");
            }
            x.Append(" items = 1 ");

            bool val = pack && (Dimension - i == 1);

            x.AppendLine($"[packed = {(val ? "true" : "false")}];");

            x.AppendLine("}");
        }
        return x.ToString();
    }
}
