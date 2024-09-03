using System.Text.Json;
using Luban.DataExporter.Builtin.Json;
using Luban.DataTarget;
using Luban.Defs;
using Luban.Protobuf.DataVisitors;
using Luban.Utils;

namespace Luban.Protobuf.DataTarget;

[DataTarget("protobuf3-json")]
public class Protobuf3JsonDataTarget : Protobuf2JsonDataTarget
{
    protected override JsonDataVisitor ImplJsonDataVisitor => Protobuf3JsonDataVisitor.Ins;
}
