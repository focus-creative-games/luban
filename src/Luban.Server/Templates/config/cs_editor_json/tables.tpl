using Bright.Serialization;

namespace {{namespace}}
{
   
public static class {{name}}
{
    public static System.Collections.Generic.Dictionary<int, Bright.Net.IProtocolFactory> Factories { get; } = new System.Collections.Generic.Dictionary<int, Bright.Net.IProtocolFactory>
    {
    {{~for proto in protos ~}}
        [{{proto.full_name}}.__ID__] = () => new {{proto.full_name}}(false),
    {{~end~}}
    };
}

}
