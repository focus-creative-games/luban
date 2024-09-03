using System.Text.Json;
using Luban.DataExporter.Builtin.Json;
using Luban.Datas;
using Luban.Defs;
using Luban.Protobuf.TypeVisitors;
using Luban.Types;
using Luban.Utils;

namespace Luban.Protobuf.DataVisitors;

public class Protobuf3JsonDataVisitor : Protobuf2JsonDataVisitor
{
    public static new Protobuf3JsonDataVisitor Ins { get; } = new();
}
